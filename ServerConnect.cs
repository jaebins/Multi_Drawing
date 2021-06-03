using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharedDrawing
{
    class ServerConnect
    {
        public string userIP = string.Empty;
        public Dictionary<Socket, string> clientList = new Dictionary<Socket, string>();

        public ServerConnect(string targetIP, bool isMakeRoom)
        {
            if (isMakeRoom)
            {
                userIP = Get_MyIP();
                Thread t1 = new Thread(ServerMake);
                t1.IsBackground = true;
                t1.Start();
            }

            else
            {
                userIP = targetIP;
            }

            Room room = new Room();
            room.userIP = userIP;
            room.Show();
        }

        private void ServerMake()
        {
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint point = new IPEndPoint(IPAddress.Any, 8000);
            server.Bind(point);
            server.Listen(5);
            while (true)
            {
                try
                {
                    Socket client = server.Accept();
                    string userName = client.RemoteEndPoint.ToString();
                    clientList.Add(client, userName);
                    Handler handler = new Handler();
                    handler.MessageReceived += new Handler.MessageReceiveEvent(MessageReceived);
                    handler.Disconnected += new Handler.DiscoonectReceiveEvent(Disconnected);
                    handler.HandlerStart(client, clientList);
                }
                catch (SocketException se)
                {
                    Console.WriteLine(se.Message);
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }
            }

            server.Close();
         }
        
        private void MessageReceived(string msg)
        {
            SendMessageAll(msg);
        }

        private void Disconnected(Socket client)
        {
            if (clientList.ContainsKey(client))
            {
                clientList.Remove(client);
            }
        }

        private void SendMessageAll(string msg)
        {
            foreach(var pair in clientList)
            {
                Socket client = pair.Key;
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                client.Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
        }

        public string Get_MyIP()
        {
            IPHostEntry host = Dns.GetHostByName(Dns.GetHostName());
            string myip = host.AddressList[0].ToString();
            return myip;
        }
    }
}
