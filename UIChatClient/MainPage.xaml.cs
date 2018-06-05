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
        private ServerConnectFilesharing serverConnectFilesharing;
        public MainPage(Window window, ServerConnectChat connectChat, ServerConnectFilesharing connectFilesharing)
        {
            InitializeComponent();

            this.window = window;
            serverConnectChat = connectChat;
            serverConnectFilesharing = connectFilesharing;
            serverConnectChat.StartAcceptMessage(historyChatListBox);

            window.Closing += WindowClosing;

            ipAddressTextBlock.Text = serverConnectChat.ServerIp;
            portTextBlock.Text = serverConnectChat.ServerPort.ToString();
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
            }
        }
    }
}
