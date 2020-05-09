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




/*
delegate void StringArgReturningVoidDelegate(string text);

namespace COMchat
{
    public class CommManager
    {
        /// <summary>
        /// Перечислитель для типов кадров
        /// </summary>
        public enum FrameType : byte
        {
            UPLINK,
            ACK_UPLINK,
            RET_UPLINK,
            LINKACTIVE,
            ACK_LINKACTIVE,
            RET_LINKACTIVE,
            DOWNLINK,
            ACK_DOWNLINK,
            RET_DOWNLINK,
            NICKNAME,
            ACK_NICKNAME
        }

        public void WriteData(string input, byte receiver, FrameType type, byte sender)
        {
            byte[] Header = { STARTBYTE, receiver, (byte)type, sender };
            byte[] BufferToSend;
            byte[] spd;
            byte[] size;
            byte[] ByteToEncode;
            byte[] ByteEncoded;
            byte[] fileId = { 0 };
            int i;

            switch (type)
            {
               
                case FrameType.UPLINK:
                    #region UPLINK

                    Header[1] = 0;
                    if (Port.IsOpen)
                    {
                        spd = new byte[12];
                        spd = Encoding.Unicode.GetBytes(Port.BaudRate.ToString());
                        BufferToSend = new byte[Header.Length + m_ByteUserName.Length + 12];
                        Header.CopyTo(BufferToSend, 0);
                        spd.CopyTo(BufferToSend, Header.Length);
                        int sum;
                        sum = Header.Length + 12;
                        m_ByteUserName.CopyTo(BufferToSend, sum);
                        Port.Write(BufferToSend, 0, BufferToSend.Length);
                    }
                    t_UpLink.Start();
                    t_ActiveLink.Start();
                    break;
                #endregion
                case FrameType.ACK_UPLINK:
                    #region ACK_UPLINK
                    if (Port.IsOpen)
                    {
                        BufferToSend = new byte[Header.Length + m_ByteUserName.Length];
                        Header.CopyTo(BufferToSend, 0);
                        m_ByteUserName.CopyTo(BufferToSend, Header.Length);
                        Port.Write(BufferToSend, 0, BufferToSend.Length);
                    }
                    /* if (Form1.b_Uplink.InvokeRequired && Form1.b_Uplink.Enabled)
                     {
                         Form1.b_Uplink.Invoke(new Action(() => { Form1.b_Uplink.PerformClick(); }));
                     } else*/
                   /* if (!last)
                    {
                        Form1.Com1.WriteData(null, 0, CommManager.FrameType.UPLINK, CommManager.UserID);
                        Form1.tb_CurrentMessage.Enabled = true;
                        Form1.b_Uplink.Enabled = false;
                        Form1.b_SendMsg.Enabled = true;
                    }
                    else if (last)
                    {
                        last = false;
                    }
                    break;
                #endregion
                case FrameType.NICKNAME:
                    #region NICKNAME
                    Header[1] = 0;
                    if (Port.IsOpen)
                    {
                        BufferToSend = new byte[Header.Length + m_ByteUserName.Length];
                        Header.CopyTo(BufferToSend, 0);
                        m_ByteUserName.CopyTo(BufferToSend, Header.Length);
                        Port.Write(BufferToSend, 0, BufferToSend.Length);
                    }
                    break;
                #endregion
                case FrameType.ACK_NICKNAME:
                    #region ACK_NICKNAME
                    if (Port.IsOpen)
                    {
                        BufferToSend = new byte[Header.Length + m_ByteUserName.Length];
                        Header.CopyTo(BufferToSend, 0);
                        m_ByteUserName.CopyTo(BufferToSend, Header.Length);
                        Port.Write(BufferToSend, 0, BufferToSend.Length);
                    }
                    break;
                #endregion
                case FrameType.DOWNLINK:
                    #region DOWNLINK

                    Header[1] = 0;
                    if (Port.IsOpen)
                    {
                        t_DownLink.Start();
                        t_ActiveLink.Stop();
                        t_UpLink.Stop();
                        Port.Write(Header, 0, Header.Length);
                    }
                    break;
                #endregion
                default:
                    if (Port.IsOpen)
                        Port.Write(Header, 0, Header.Length);

                    break;
            }
            WriteLog(DirectionType.OUTGOING, type);
        }
        /// <summary>
        /// Обработчик приема данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Err = false;
            if (Port.ReadByte() == STARTBYTE)
            {
                byte read = (byte)Port.ReadByte();
                if (read == m_UserID || read == 0)
                {
                    FrameAnalysis((byte)Port.ReadByte(), read);
                }

            }

        }
        /// <summary>
        /// Метод обработки кадра
        /// </summary>
        /// <param name="frametype">Тип обрабатываемого кадра</param>
        private void FrameAnalysis(byte frametypeid, byte to)
        {
            FrameType frametype = (FrameType)Enum.ToObject(typeof(FrameType), frametypeid);
            int bytesToRead;
            byte[] byteBuffer;
            int i;
            byte[] ToDecode;
            byte[] Decoded;
            WriteLog(DirectionType.INCOMING, frametype);

            switch (frametype)
            {

                case FrameType.UPLINK:
                    #region UPLINK
                    Link.IsActive = true;
                    if (RadioButton2.Visible && RadioButton1.Visible) b_UpLink.Enabled = false;
                    Link.ID = (byte)Port.ReadByte();
                    byte[] spd;
                    string spd_s;
                    spd = new byte[12];
                    Port.Read(spd, 0, 12);
                    spd_s = Encoding.Unicode.GetString(spd);
                    //MessageBox.Show(spd_s);
                    Port.BaudRate = int.Parse(spd_s);
                    bytesToRead = Port.BytesToRead;
                    byteBuffer = new byte[bytesToRead];
                    Port.Read(byteBuffer, 0, bytesToRead);
                    Link.Name = Encoding.Unicode.GetString(byteBuffer); //имя пользователя
                    if (Form1.b_Uplink.Enabled)
                    {
                        first = false;
                    }


                    if (!RadioButton1.Visible)
                    {
                        RadioButton1.Invoke(new EventHandler(delegate
                        {
                            RadioButton1.Text = Link.Name;
                            Form1.User1ID = Link.ID;
                            RadioButton1.Visible = true;
                        }));
                    }
                    if (Form1.User1ID != Link.ID)
                    {
                        RadioButton2.Invoke(new EventHandler(delegate
                        {
                            RadioButton2.Text = Link.Name + (Link.Name == RadioButton1.Text ? "(2)" : "");
                            Form1.User2ID = Link.ID;
                            RadioButton2.Visible = true;
                        }));
                    }
                    Settings.cb_BaudRate1.SelectedValue = spd_s.ToString();

                    Form1.Com1.Port.BaudRate = int.Parse(spd_s);
                    Form1.Com2.Port.BaudRate = int.Parse(spd_s);
                    WriteData(m_UserName, Link.ID, FrameType.ACK_UPLINK, m_UserID);*/

                    /*if (Form1.b_Uplink.InvokeRequired && Form1.b_Uplink.Enabled)
                    {  
                        Form1.b_Uplink.Invoke(new Action(() => { Form1.b_Uplink.PerformClick(); }));
                    }*/

/*

                    break;
                #endregion
                case FrameType.ACK_UPLINK:
                    #region ACK_UPLINK
                    Link.IsActive = true;
                    Link.ID = (byte)Port.ReadByte();
                    bytesToRead = Port.BytesToRead;
                    byteBuffer = new byte[bytesToRead];
                    Port.Read(byteBuffer, 0, bytesToRead);
                    Link.Name = Encoding.Unicode.GetString(byteBuffer);
                    if (!RadioButton1.Visible)
                    {
                        RadioButton1.Invoke(new EventHandler(delegate
                        {
                            RadioButton1.Text = Link.Name;
                            Form1.User1ID = Link.ID;
                            RadioButton1.Visible = true;
                        }));
                    }
                    else if (Form1.User1ID != Link.ID)
                    {
                        RadioButton2.Invoke(new EventHandler(delegate
                        {
                            RadioButton2.Text = Link.Name + (Link.Name == RadioButton1.Text ? "(2)" : "");
                            Form1.User2ID = Link.ID;
                            RadioButton2.Visible = true;
                        }));
                    }
                    c_upLinkTry = 3;//восстановили число попыток
                    t_UpLink.Stop();//успешно соединились
                    Form1.b_SendMsg.Enabled = true;

                    WriteData(null, Link.ID, FrameType.LINKACTIVE, UserID);
                    // Log.AppendText("Соединение по " + Port.PortName + " установлено \n");
                    //Log.ScrollToCaret();

                    break;
                #endregion
                case FrameType.NICKNAME:
                    #region NICKNAME

                    Link.ID = (byte)Port.ReadByte();

                    if (RadioButton2.Visible && RadioButton1.Visible) b_UpLink.Enabled = false;


                    bytesToRead = Port.BytesToRead;
                    byteBuffer = new byte[bytesToRead];
                    Port.Read(byteBuffer, 0, bytesToRead);
                    Link.Name = Encoding.Unicode.GetString(byteBuffer); //имя пользователя

                    if (Form1.User1ID == Link.ID)
                    {
                        RadioButton1.Invoke(new EventHandler(delegate
                        {
                            RadioButton1.Text = Link.Name;
                        }));
                    }
                    if (Form1.User2ID == Link.ID)
                    {
                        RadioButton1.Invoke(new EventHandler(delegate
                        {
                            RadioButton2.Text = Link.Name;
                        }));
                    }
                    WriteData(m_UserName, Link.ID, FrameType.ACK_NICKNAME, m_UserID);

                    break;
                #endregion
                case FrameType.ACK_NICKNAME:
                    #region ACK_NICKNAME



                    break;
                #endregion
                case FrameType.RET_UPLINK:
                    #region RET_UPLINK
                    Link.IsActive = false;
                    WriteData(null, Link.ID, FrameType.UPLINK, m_UserID);
                    break;
                #endregion
                case FrameType.DOWNLINK:
                    #region DOWNLINK
                    Link.IsActive = false;
                    t_ActiveLink.Stop();
                    WriteData(null, Link.ID, FrameType.ACK_DOWNLINK, m_UserID);
                    b_UpLink.Enabled = false;
                    if (RadioButton2.Visible ^ RadioButton1.Visible)
                    {
                        b_UpLink.Enabled = false;
                        b_DownLink.Enabled = false;
                        b_SendFile.Enabled = false;
                        b_SendMessage.Enabled = false;
                        tb_CurrentMsg.Enabled = false;
                    }
                    if (RadioButton1.Text == Link.Name)
                    {
                        RadioButton1.Invoke(new EventHandler(delegate
                        {
                            RadioButton1.Visible = false;
                            Log.AppendText("Пользователь " + RadioButton1.Text + " разорвал соединение\n");
                        }));
                    }
                    if (RadioButton2.Text == Link.Name)
                    {
                        RadioButton2.Invoke(new EventHandler(delegate
                        {
                            RadioButton2.Visible = false;
                            Log.AppendText("Пользователь " + RadioButton2.Text + " разорвал соединение\n");
                        }));
                    }
                    Form1.b_Uplink.Enabled = true;
                    break;
                #endregion
                case FrameType.ACK_DOWNLINK:
                    #region ACK_DOWNLINK
                    Link.IsActive = false;

                    t_ActiveLink.Stop();

                    if (RadioButton1.Text == Link.Name)
                    {
                        RadioButton1.Invoke(new EventHandler(delegate
                        {
                            RadioButton1.Visible = false;
                        }));
                    }
                    if (RadioButton2.Text == Link.Name)
                    {
                        RadioButton2.Invoke(new EventHandler(delegate
                        {
                            RadioButton2.Visible = false;
                        }));
                    }

                    break;
                #endregion
                case FrameType.RET_DOWNLINK:
                    #region RET_DOWNLINK
                    Link.IsActive = false;
                    WriteData(null, Link.ID, FrameType.DOWNLINK, m_UserID);
                    break;
                #endregion
                case FrameType.LINKACTIVE:
                    #region LINKACTIVE
                    Link.IsActive = true;
                    WriteData(null, Link.ID, FrameType.ACK_LINKACTIVE, UserID);
                    break;
                #endregion
                case FrameType.ACK_LINKACTIVE:
                    #region ACK_LINKACTIVE
                    Link.IsActive = true;


                    break;
                #endregion
                case FrameType.RET_LINKACTIVE:
                    #region RET_LINKACTIVE
                    Link.IsActive = false;
                    WriteData(null, Link.ID, FrameType.LINKACTIVE, m_UserID);
                    break;
                #endregion
                case FrameType.RET_FILE:
                    #region RET_FILE
                    WriteData(null, Link.ID, FrameType.FILE, m_UserID);
                    break;
                #endregion
     
        }
        private string TypeFileAnalysis(byte fileId)


        }



    }
}*/
