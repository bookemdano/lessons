using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ARFCon;
using ARFLib;

namespace ARFSign {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISignListener {
        SignState _signState = new SignState();
        private SockListener _sock;

        public MainWindow() {
            InitializeComponent();
            _sock = new SockListener(this);
            entAddress.Text = Config.LocalSignAddress.ToString();
        }

        public void Log(object response) {
            lst.Items.Insert(0, response);
        }
        public async Task<SignState> StateChange(SignState signState) {
            Log("Changing to " + signState);
            if (!signState.Same(_signState)) {
                await Task.Delay(1000);
                _signState = signState;
                Log("Changed to " + _signState);
            }
            return _signState;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await _sock.Listen(Config.LocalSignAddress);
        }

        private async void Reset_Click(object sender, RoutedEventArgs e) {
            Config.LocalSignAddress = int.Parse(entAddress.Text);
            await _sock.Listen(Config.LocalSignAddress);
        }
    }
}
