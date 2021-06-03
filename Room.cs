using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharedDrawing
{
    public partial class Room : Form
    {
        public string userIP = string.Empty;
        Socket client;

        public Room()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse(userIP), 8000);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }

            Thread t1 = new Thread(MessageReceived);
            t1.IsBackground = true;
            t1.Start();
        }

        public void MessageReceived()
        {
            string msg = string.Empty;
            int length = 0;
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    length = client.Receive(buffer);
                    msg = Encoding.UTF8.GetString(buffer, 0, length);
                    if (msg.Contains(","))
                    {
                        string[] mousePos = msg.Split(',');
                        Drawing(int.Parse(mousePos[0]), int.Parse(mousePos[1]));
                    }
                    if (msg.Equals("Command-AllDelete"))
                    {
                        RemoveAll();
                    }
                } catch(SocketException se)
                {
                    MessageBox.Show(se.Message);
                    break;
                } catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    break;
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left && e.X < 1500)
            {
                string msg = e.X.ToString() + "," + e.Y.ToString();
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
        }

        private void Drawing(int x, int y)
        {
            Brush brush = new SolidBrush(Color.Black);
            Rectangle rect = new Rectangle(x, y, 10, 10);
            Graphics screen = CreateGraphics();
            screen.FillRectangle(brush, rect);
            screen.Dispose();
        }

        private void but_AllDelete_Click(object sender, EventArgs e)
        {
            string msg = "Command-AllDelete";
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            client.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private void RemoveAll()
        {
            Graphics newScreen = CreateGraphics();
            newScreen.Clear(Color.White);
        }
    }
}
