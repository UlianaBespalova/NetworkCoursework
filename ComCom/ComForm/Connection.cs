using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace ComForm
{
    class Connection
    {
        SerialPort _Port = new SerialPort();
        public SerialPort Port 
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
                if (_Port.IsOpen)
                {
                    _Port.DiscardInBuffer();
                    _Port.DiscardOutBuffer();
                }
            }
        }

        public bool setPortName(string name)
        {
            string[] PortList = SerialPort.GetPortNames();

            if (Port.IsOpen)
            {
                Log.AppendText("port " + name + ": you can't change port name while it is opened\n");
                return false;
            }
            
            if (PortList.Contains(name))
            {
                Port.PortName = name;
                return true;
            }
            Log.AppendText("port " + name + " not found\n");  //нет такого порта
            return false;
        }

        public bool OpenPort()
        {
            try
            {
                Port.Open();
                Port.DtrEnable = true;
                InitializeHandlers();

                return true;
            }

            catch (System.IO.IOException) 
            {
                Log.AppendText("port " + Port.PortName + " not found\n");
                return false;
            }

            catch (System.InvalidOperationException) //открыт в этом приложении
            {
                Log.AppendText("port " + Port.PortName + "  is already opened\n");
                return false;
            }

            catch (System.UnauthorizedAccessException) //уже открыт в другом приложении/другим окном
            {
                Log.AppendText("port " + Port.PortName + "  is already used\n");
                return false;
            }
        }

        public bool ClosePort()
        {
            if (!Port.IsOpen)
            {
                Log.AppendText("fail: port is already closed\n");
                return false;
            }
            Port.Close();
            return true;
        }

        public bool IsConnected() //оба порта открыты и готовы слать данные
        {
            return Port.IsOpen && Port.DsrHolding;
        }



        //==================================================*/*/*/*

        public const byte STARTBYTE = 0xFF;

        public enum FrameType : byte
        {
            ACK,
            MSG,
            RET_MSG,
            RET_FILE,
            FILE,
        }



        public void WriteData(string input, FrameType type)
            //пока считаем, что input строка символов
        {
            byte[] Header = { STARTBYTE, (byte)type };
            byte[] BufferToSend;
            byte[] Telegram;

            switch (type)
            {
                case FrameType.MSG:
                    #region MSG
                    if (IsConnected())
                    {
                        // Telegram[] = Coding(input); 
                        Telegram = Encoding.Default.GetBytes(input); //потом это кыш

                        BufferToSend = new byte[Header.Length + Telegram.Length]; //буфер для отправки = заголовок+сообщение
                        Header.CopyTo(BufferToSend, 0);
                        Telegram.CopyTo(BufferToSend, Header.Length);

                        Port.Write(BufferToSend, 0, BufferToSend.Length);
                        Log.AppendText("(" + Port.PortName + ") WriteData: sent message >  " + Encoding.Default.GetString(Telegram) + "\n");
                    }
                    break;
                #endregion


                case FrameType.FILE:
                    #region FILE
                    if (IsConnected())
                    {
                        byte[] size = new byte[10];
                        size = Encoding.Default.GetBytes((input.Length).ToString()); //размер файла

                        //Telegram = Coding(input); 
                        Telegram = Encoding.Default.GetBytes(input); //кыш

                        BufferToSend = new byte[Header.Length + 10 + Telegram.Length];
                       
                        Header.CopyTo(BufferToSend, 0);    
                        size.CopyTo(BufferToSend, Header.Length);
                        Telegram.CopyTo(BufferToSend, Header.Length + 10);//!

                        Port.Write(BufferToSend, 0, BufferToSend.Length);
                        Log.AppendText("(" + Port.PortName + ") WriteData: sent file >  " + input.Length + " bytes\n");

                        //чёрная магия на случай, когда файл больше 2048 и не влезает в буфер
                        //нужна ли она
                    }
                    break;
                #endregion

                default:
                    if (IsConnected())
                        Port.Write(Header, 0, Header.Length);
                    break;
            }

            Log.Invoke(new EventHandler(delegate
            {
                Log.AppendText("sent frame " + type + "\n"); //всё записываем, мы же снобы
            }));
        }


        public void InitializeHandlers()
        {
            Port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
        }


        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (Port.ReadByte() == STARTBYTE)
            {
                GetData(Port.ReadByte());
            }
        }

        public void GetData(int typeId)
        {
            FrameType type = (FrameType)typeId;
            
            Log.Invoke(new EventHandler(delegate
            {
                Log.AppendText("get frame " + type +"\n");
            }));


            switch (type)
            {
                case FrameType.MSG:
                    #region MSG
                    if (IsConnected())
                    {
                        int n = Port.BytesToRead;
                        byte[] msgByteBuffer = new byte[n];
                        
                        Port.Read(msgByteBuffer, 0, n); //считываем сообщение
                        string Message = Encoding.Default.GetString(msgByteBuffer);
                        Log.Invoke(new EventHandler(delegate
                        {
                            Log.AppendText("(" + Port.PortName + ") GetData: new message > " + Message + "\n");
                        }));

                        WriteData(null, FrameType.ACK);
                    }
                    else
                    {
                        WriteData(null, FrameType.RET_MSG);
                    }
                    break;
                #endregion

                
                case FrameType.FILE:

                    #region FILE
                    if (IsConnected())
                    {
                        byte[] byteBuffer;

                        byte[] size = new byte[10];
                        Port.Read(size, 0, 10);
                        string size_s = Encoding.Default.GetString(size);

                        double ssize = Double.Parse(size_s); //размер файла //нужен ли он вообще /наверное, нужен

                        int n = Port.BytesToRead;
                        byteBuffer = new byte[n];
                        Port.Read(byteBuffer, 0, n);

                        Log.Invoke(new EventHandler(delegate
                        {
                            Log.AppendText("(" + Port.PortName + ") : GetData: new file > " + byteBuffer.Length + " bytes\n");
                        }));

                        WriteData(null, FrameType.ACK);
                    }
                    else
                    {
                        WriteData(null, FrameType.RET_FILE);
                    }

                    break;
                #endregion
                //======================================================



                case FrameType.ACK:
                    #region ACK
                    break;
                #endregion

                case FrameType.RET_MSG:
                    #region RET_MSG
                    Log.AppendText("Message error! No connection\n");
                    break;
                #endregion

                case FrameType.RET_FILE:
                    #region RET_FILE
                    Log.AppendText("File error! No connection\n");
                    break;
                    #endregion
            }
        }


        private RichTextBox _Log; //штука, чтобы видеть, что творится
        public RichTextBox Log
        {
            get
            {
                return _Log;
            }
            set
            {
                _Log = value;
            }
        }

    }
}
