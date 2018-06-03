using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace UIChatClient
{
    public class ServerConnect
    {
        public int ServerPort { get; set; } // = 3535 
        public string ServerIp { get; set; } //127.0.0.1
        public bool ServerIsConnect { get; set; }
        public string UserName { get; set; }

        //Chat variables
        private Socket remoteServerSocket;
        private IPEndPoint endPoint;

        public void CreateConnect()
        {
            remoteServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);

            try
            {
                Console.WriteLine("Соединяемся с сервером...");
                remoteServerSocket.Connect(endPoint);
                Console.WriteLine("Соединено..");
                ServerIsConnect = true;
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(new UserMessage { UserName = UserName, Message = message });
                remoteServerSocket.Send(Encoding.Default.GetBytes(serialized));
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void StartAcceptMessage()
        {
            await AcceptMessage();
        }

        public Task AcceptMessage()
        {
            return Task.Run(() =>
            {
                while (ServerIsConnect)
                {
                    int bytes;
                    byte[] buffer = new byte[1024];
                    StringBuilder stringBuilder = new StringBuilder();

                    do
                    {
                        bytes = remoteServerSocket.Receive(buffer);
                        stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bytes));
                    }
                    while (remoteServerSocket.Available > 0);

                    //Проблема на сервере он отправаляет не json файл а готовую строку (исправить)
                    UserMessage newMessage = JsonConvert.DeserializeObject<UserMessage>(stringBuilder.ToString());
                    //Отправить в ListBox
                }
            });
        }

        public void CloseConnect()
        {
            SendMessage("exit");
            ServerIsConnect = false;
            remoteServerSocket.Close();
            Console.WriteLine("Сеанс завершен...");
        }
    }
}
