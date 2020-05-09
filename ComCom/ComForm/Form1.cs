using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComForm
{
    public partial class Form1 : Form
    {
        Connection com1 = new Connection();
        Connection com2 = new Connection();

        public Form1()
        {
            InitializeComponent();
        }

        private void b_open5_Click(object sender, EventArgs e)
        {
            com1.Log = richTextBox1;
            com1.setPortName("COM1"); //давать список для выбора из существующих?
            com1.OpenPort();
            
            richTextBox1.AppendText(com1.Port.PortName + "  is opened: " + com1.Port.IsOpen + "\n");
        }

        private void b_open6_Click(object sender, EventArgs e)
        {
            com2.Log = richTextBox1;
            com2.setPortName("COM2");
            com2.OpenPort();
            
            richTextBox1.AppendText(com2.Port.PortName + "  is opened: " + com2.Port.IsOpen + "\n");
        }

        private void b_con_Click(object sender, EventArgs e)
        {
            richTextBox1.AppendText("COM1 is connected: " + com1.IsConnected() + "\n");
            richTextBox1.AppendText("COM2 is connected: " + com2.IsConnected() + "\n");
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (com1.IsConnected())
            {
                com1.WriteData("Hello world", Connection.FrameType.MSG);
            }
            else
            {
                richTextBox1.AppendText("error: no connection\n");
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (com1.IsConnected())
            {
                com1.WriteData("filebytesbytesbytes", Connection.FrameType.FILE);
            }
            else
            {
                richTextBox1.AppendText("error: no connection\n");
            }
        }
    }
}
