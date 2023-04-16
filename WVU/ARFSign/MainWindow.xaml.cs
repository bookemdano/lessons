using ARFCon;
using ARFLib;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace ARFSign {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ISignListener {
        SignState _signState = new SignState(SignEnum.Initialize, "", "-");
        SockListener _sock;
        DateTime _lastConnection = DateTime.Now;
        Dictionary<System.Drawing.Color, Brush> _brushes = new Dictionary<System.Drawing.Color, Brush>();
        SoundPlayer _soundPlayer;
        private int _iCam;

        public MainWindow() {
            InitializeComponent();
            _sock = new SockListener(this);
        }

        private void Timer_Tick(object? sender, EventArgs e) {
            var delta = DateTime.Now - _lastConnection;
            staComm.Visibility = Visibility.Hidden;
            sta.Text = $"Last conn. {delta.TotalSeconds.ToString("0")} secs ago";
            if (delta > Config.HeartbeatTimeout * 1.5) {
                SetSignState(new SignState(SignEnum.Error, null, "Console Disconnected"));
            }
        }

        public void Log(object o) {
            lst.Items.Insert(0, DateTime.Now.ToString("H:mm:ss") + " " + o);
            Logger.Log(o);
        }
        public async Task<SignState> StateChange(SignState signState) {
            staComm.Visibility = Visibility.Visible;
            _lastConnection = DateTime.Now;
            if (signState.State == SignEnum.Heartbeat) {
                Log("HB " + signState);
                return _signState;
            }

            Log("Changing to " + signState);
            if (!signState.Same(_signState)) {
                await Task.Delay(1000);
                SetSignState(signState);
                Log("Changed to " + _signState);
            }
            return _signState;
        }
        void PlaySound(bool b) {
            staSound.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
            if (b) {
                if (_soundPlayer != null)
                    return;
                _soundPlayer = new SoundPlayer(@"media\alarm.wav");
                _soundPlayer.PlayLooping();
            }
            else {
                if (_soundPlayer == null)
                    return;
                _soundPlayer.Stop();
                _soundPlayer = null;
            }
        }

        void SetSignState(SignState signState) {
            var fontSize = 48;
            var text = signState.Text;
            if (text.Length > 5) {
                fontSize = 18;
                text = text.Substring(0, 20);
            }

            pnl.Background = GetBrush(System.Drawing.Color.FromName(signState.ColorName));
            staArf.FontSize = fontSize;
            staArf.Text = text;
            PlaySound(signState.State == SignEnum.Alarm);
            _signState = signState;
        }
        // TODONE heartbeat to sign
        // TODONE alarm on sign
        // TODONE sound icon on console
        // TODO manual mode on sign
        // TODO stand on sign

        Brush GetBrush(System.Drawing.Color color) {
            if (!_brushes.ContainsKey(color)) 
                _brushes[color] = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            return _brushes[color];
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await ListenForever();
        }

        private async void Reset_Click(object sender, RoutedEventArgs e) {
            Config.SetCameraAddress(_iCam, entAddress.Text);
            _sock.Kill();
        }
        private async Task ListenForever() {

            _iCam = 0;
            if (!await _sock.ListenOnce(Config.GetCameraAddress(0)))
                _iCam = 1;

            entAddress.Text = Config.GetCameraAddress(_iCam);
            Title = Config.FullCameraName(_iCam);

            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            while (true) {
                if (!await _sock.ListenOnce(entAddress.Text))   // don't use LocalSignAddress because it is shared across instances
                    await Task.Delay(1000);
            }
        }
            
    }
}
