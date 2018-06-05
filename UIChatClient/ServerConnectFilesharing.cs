using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UIChatClient
{
    public class ServerConnectFilesharing
    {
        public int ServerPort { get; set; } // = 3535 
        public string ServerIp { get; set; } //127.0.0.1

        //-------------------------------Upload----------------------------------

        private TcpClient clientUpload = new TcpClient();
        private NetworkStream stream;

        public async void ConnectUpload()
        {
            await clientUpload.ConnectAsync(IPAddress.Parse(ServerIp), ServerPort);
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

                var data = JsonConvert.SerializeObject(file);
                var bytesData = Encoding.Default.GetBytes(data);

                if (stream == null)
                {
                    stream = clientUpload.GetStream();
                }

                stream.Write(bytesData, 0, bytesData.Length);
            });
        }

        //------------------------------Download--------------------------------

        private TcpClient clientDownload;
        private TcpListener serverFilesharing;

        public void ConnectDownload()
        {
            serverFilesharing = new TcpListener(IPAddress.Parse(ServerIp), ServerPort);
        }

        public async void DownloadFileStart()
        {
            try
            {
                serverFilesharing.Start();
                while (true)
                {
                    clientDownload = await serverFilesharing.AcceptTcpClientAsync();
                    MessageBoxResult result = MessageBox.Show("Поступил запрос на передачу файла. Загрузить файл ?","Запрос на загрузку файла",MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        ThreadPool.QueueUserWorkItem(new WaitCallback(DownloadFileWork), null);
                    }
                    serverFilesharing.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (serverFilesharing != null)
                {
                    serverFilesharing.Stop();
                }
            }
        }

        private void DownloadFileWork(object obj)
        {
            using (NetworkStream stream = clientDownload.GetStream())
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        int bytes;
                        do
                        {
                            bytes = stream.Read(buffer, 0, buffer.Length);
                            stringBuilder.Append(Encoding.Default.GetString(buffer, 0, bytes));
                        } while (stream.DataAvailable);

                        FileTransport file = JsonConvert.DeserializeObject<FileTransport>(stringBuilder.ToString());
                        Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\files\");
                        using (FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + @"\files\" + file.FileName, FileMode.Create))
                        {
                            fileStream.Write(file.Data, 0, file.Data.Length);
                        }
                        MessageBox.Show("Был загружен новый файл");
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                finally
                {
                    clientDownload.Close();
                }
            }
        }
    }
}
