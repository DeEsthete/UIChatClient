using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace UIChatClient
{
    public class ServerConnectChat
    {
        public int ServerPort { get; set; } // = 3535
        public string ServerIp { get; set; } //127.0.0.1
        public bool ServerIsConnect { get; set; }
        public string UserName { get; set; }
        
        private Socket remoteServerSocket;
        private IPEndPoint endPoint;

        public void CreateConnect()
        {
            remoteServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);

            try
            {
                remoteServerSocket.Connect(endPoint);
                ServerIsConnect = true;
                SendMessage("init");
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void SendMessage(string message)
        {
            await SendMessageWork(message);
        }

        private Task SendMessageWork(string message)
        {
            return Task.Run(() =>
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
            });
        }

        public async void StartAcceptMessage(ListBox chatListBox)
        {
            await AcceptMessage(chatListBox);
        }

        private Task AcceptMessage(ListBox chatListBox)
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
                    
                    UserMessage newMessage = JsonConvert.DeserializeObject<UserMessage>(stringBuilder.ToString());
                    MessageEntity temp = new MessageEntity(newMessage);
                    chatListBox.Items.Add(temp);
                    //Отправить в ListBox
                }
            });
        }

        public void CloseConnect()
        {
            SendMessageWork("exit");
            ServerIsConnect = false;
            remoteServerSocket.Close();
        }
    }
}
