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
        private ServerConnect serverConnect;
        public MainPage(Window window, ServerConnect connect)
        {
            InitializeComponent();
            this.window = window;
            serverConnect = connect;
            window.Closing += WindowClosing;
            ipAddressTextBlock.Text = serverConnect.ServerIp;
            portTextBlock.Text = serverConnect.ServerPort.ToString();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            serverConnect.CloseConnect();
        }

        private void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            if (messageTextBox.Text == "")
            {
                MessageBox.Show("Введите сообщение!");
            }
            else
            {
                serverConnect.SendMessage(messageTextBox.Text);
            }
        }
    }
}
