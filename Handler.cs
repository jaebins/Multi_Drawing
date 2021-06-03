using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedDrawing
{
    class Handler
    {
        Socket client;
        Dictionary<Socket, string> clientList = new Dictionary<Socket, string>();

        public delegate void MessageReceiveEvent(string msg);
        public event MessageReceiveEvent MessageReceived;

        public delegate void DiscoonectReceiveEvent(Socket client);
        public event DiscoonectReceiveEvent Disconnected;

        public void HandlerStart(Socket client, Dictionary<Socket, string> clientList)
        {
            this.client = client;
            this.clientList = clientList;

            Thread t1 = new Thread(ReceiveChat);
            t1.IsBackground = true;
            t1.Start();
        }

        public void ReceiveChat()
        {
            string msg = string.Empty;
            int length = 0;
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    length = client.Receive(buffer);
                    msg = Encoding.UTF8.GetString(buffer, 0, length);
                    MessageReceived(msg);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
                Disconnected(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Disconnected(client);
            }
        }
    }
}
