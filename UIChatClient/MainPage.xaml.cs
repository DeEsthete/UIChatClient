using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIChatClient
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Window window;
        private ServerConnectChat serverConnectChat;
        public MainPage(Window window, ServerConnectChat connectChat)
        {
            InitializeComponent();

            this.window = window;
            serverConnectChat = connectChat;
            serverConnectChat.StartAcceptMessage(historyChatListBox);

            window.Closing += WindowClosing;

            ipAddressTextBlock.Text = serverConnectChat.ServerIp;
            portTextBlock.Text = serverConnectChat.ServerPort.ToString();

            StartAcceptMessage();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serverConnectChat.CloseConnect();
        }

        private void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            if (messageTextBox.Text == "")
            {
                MessageBox.Show("Введите сообщение!");
            }
            else
            {
                serverConnectChat.SendMessage(messageTextBox.Text);
                messageTextBox.Text = "";
            }
        }

        private async void StartAcceptMessage()
        {
            await AcceptMessage();
        }

        private Task AcceptMessage()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    if (historyChatListBox.Items.Count < serverConnectChat.Messages.Count)
                    {
                        
                        Dispatcher.Invoke(() => {
                            MessageEntity temp = new MessageEntity(serverConnectChat.Messages[historyChatListBox.Items.Count]);
                            historyChatListBox.Items.Add(temp);
                        });
                     
                    }
                }
            });
        }

        private void SendFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();

            if (d.ShowDialog() == true)
            {
                serverConnectChat.UploadFile(d.FileName);
            }
        }
    }
}
