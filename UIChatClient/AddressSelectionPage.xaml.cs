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
    /// Логика взаимодействия для AddressSelectionPage.xaml
    /// </summary>
    public partial class AddressSelectionPage : Page
    {
        private ServerConnect serverConnect;
        private Window window;
        public AddressSelectionPage(Window window)
        {
            InitializeComponent();
            this.window = window;
            serverConnect = new ServerConnect();
        }

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                serverConnect.ServerIp = ipTextBox.Text;
                serverConnect.UserName = userNameTextBox.Text;

                int port = -1;
                bool portIsCorrect = int.TryParse(portTextBox.Text, out port);
                if (!portIsCorrect)
                {
                    MessageBox.Show("Порт может состоять только из цифр!");
                }
                else
                {
                    serverConnect.ServerPort = port;
                    serverConnect.CreateConnect();
                    window.Content = new MainPage(window, serverConnect);
                }
            }
            catch
            {
                MessageBox.Show("Не все поля заполнены верно!");
            }
        }
    }
}
