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

namespace MockATM
{
    public enum StateEnum
    {
        Waiting,
        RequestPIN
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Update();
        }
        static StateEnum _state = StateEnum.Waiting;
        
        private void InsertCard_Click(object sender, RoutedEventArgs e)
        {
            if (_state != StateEnum.Waiting)
                return;
            _state = StateEnum.RequestPIN;
            _attempt = 0;
            Update();
        }

        private void Update()
        {
            pnlWaiting.Visibility = Visibility.Collapsed;
            pnlPIN.Visibility = Visibility.Collapsed;
            btnInsertCard.Content = "CARD INSERTED";
            if (_state == StateEnum.Waiting)
            {
                btnInsertCard.Content = "Insert Card";
                pnlWaiting.Visibility = Visibility.Visible;
            }
            else if (_state == StateEnum.RequestPIN)
            {
                pnlPIN.Visibility = Visibility.Visible;
                if (_attempt == 0)
                    staPIN.Text = "Enter PIN:";
                else
                    staPIN.Text = $"Enter PIN({_attempt + 1} try):";
            }
        }

        private void ReturnCard_Click(object sender, RoutedEventArgs e)
        {
            _state = StateEnum.Waiting;
            Update();
        }
        int _attempt = 0;
        private void PIN_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (entPIN.Password.Length != 4)
                return;
            if (entPIN.Password != "1234")
            {
                _attempt++;
                entPIN.Password = "";
                if (_attempt == 3)
                {
                    MessageBox.Show("PIN Incorrect. Card eaten! Nom nom nom");
                    _state = StateEnum.Waiting;
                }
                else
                    MessageBox.Show("PIN Incorrect. After 3 attempts card will be eaten.");
                Update();
            }    
        }
    }
}
