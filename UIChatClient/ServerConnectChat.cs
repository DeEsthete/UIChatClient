using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace UIChatClient
{
    public class ServerConnectChat
    {
        public List<UserMessage> Messages { get; set; }

        public int ServerPort { get; set; } // = 3535
        public string ServerIp { get; set; } //127.0.0.1
        public bool ServerIsConnect { get; set; }
        public string UserName { get; set; }
        
        private Socket remoteServerSocket;
        private IPEndPoint endPoint;

        public ServerConnectChat()
        {
            Messages = new List<UserMessage>();
        }

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
                    MessageBox.Show(ex.Message);
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
                    if (newMessage.Message == "upload")
                    {
                        MessageBoxResult result = MessageBox.Show("Поступил запрос на передачу файла. Загрузить файл ?", "Запрос на загрузку файла", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\files\");
                            using (FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\files\" + newMessage.File.FileName, FileMode.Create))
                            {
                                fileStream.Write(newMessage.File.Data, 0, newMessage.File.Data.Length);
                            }
                        }
                    }
                    Messages.Add(newMessage);
                }
            });
        }

        public async void UploadFile(string path)
        {
            await UploadFileWork(path);
        }

        private Task UploadFileWork(string path)
        {
            return Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(path);
                FileTransport file = new FileTransport();
                file.Data = File.ReadAllBytes(path);
                file.FileName = fileInfo.Name;

                UserMessage message = new UserMessage();
                message.UserName = UserName;
                message.Message = "upload";
                message.File = file;

                try
                {
                    string serialized = JsonConvert.SerializeObject(message);
                    remoteServerSocket.Send(Encoding.Default.GetBytes(serialized));
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
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
