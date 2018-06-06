using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    /// Логика взаимодействия для AddressSelectionPage.xaml
    /// </summary>
    public partial class AddressSelectionPage : Page
    {
        private ServerConnectChat serverConnectChat;
        private Window window;

        public AddressSelectionPage(Window window)
        {
            InitializeComponent();
            this.window = window;
            serverConnectChat = new ServerConnectChat();
        }

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                serverConnectChat.ServerIp = ipTextBox.Text;
                serverConnectChat.UserName = userNameTextBox.Text;

                int port = -1;
                bool portIsCorrect = int.TryParse(portTextBox.Text, out port);
                if (!portIsCorrect)
                {
                    MessageBox.Show("Порт может состоять только из цифр!");
                }
                else
                {
                    serverConnectChat.ServerPort = port;
                    serverConnectChat.CreateConnect();
                    
                    window.Content = new MainPage(window, serverConnectChat);
                }
            }
            catch
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }
    }
}
