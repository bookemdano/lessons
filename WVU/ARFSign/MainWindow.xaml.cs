using ARFUILib;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ARFSign {
    public partial class MainWindow : Window, ISignListener {
        DateTime _lastConnection = DateTime.Now;
        private int _iCam;
        SignView _sign = null;

        public MainWindow() {
            InitializeComponent();
        }

        private void Timer_Tick(object? sender, EventArgs e) {
            _sign.DoneListeningTimer(_lastConnection, chkManual.IsChecked == true);
            if (chkManual.IsChecked == true)
                return;
            if (SignView.TimedOut(_lastConnection))
                SetManual(_sign.MySignState.State == SignEnum.Error);
        }

        public void Log(object o) {
            lst.Items.Insert(0, DateTime.Now.ToString("H:mm:ss") + " " + o);
            Logger.Log(o);
        }
        public async Task<SignState> StateChange(SignState signState) {
            _sign.StartComm();
            _lastConnection = DateTime.Now;
            if (chkManual.IsChecked == true)
                return _sign.MySignState;
            if (signState.State == SignEnum.Heartbeat) {
                Log("HB " + signState);
                return _sign.MySignState;
            }

            Log("Changing to " + signState);
            if (!signState.Same(_sign.MySignState)) {
                await Task.Delay(1000);
                _sign.SetSignState(signState);
                Log("Changed to " + _sign.MySignState);
            }
            return _sign.MySignState;
        }

        // TODONE heartbeat to sign
        // TODONE alarm on sign
        // TODONE sound icon on console
        // TODONE manual mode on sign
        // TODONE stand on sign
        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await ListenForever();
        }

        private async void Reset_Click(object sender, RoutedEventArgs e) {
            Config.SetCameraAddress(_iCam, entAddress.Text);
            _sign.KillSend();
        }
        private async Task ListenForever() {
            _iCam = 0;
            var sock = new SockListener(this);
            if (!await sock.Test(Config.GetCameraAddress(_iCam)))
                _iCam = 1;

            _sign = new SignView(this, pnl, staArf, staSound, staComm, meSiren, imgMask, sta, _iCam);

            entAddress.Text = Config.GetCameraAddress(_iCam);
            Title = Config.FullCameraName(_iCam);

            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

            while (true) {
                if (!await _sign.ListenOnce(entAddress.Text))   // don't use LocalSignAddress because it is shared across instances
                    await Task.Delay(1000);
            }
        }
        void SetManual(bool b) {
            chkManual.IsChecked = b;
            btnStop.Visibility = UIUtils.IsVisOrCollapsed(b);
            btnSlow.Visibility = UIUtils.IsVisOrCollapsed(b);
            btnCustom.Visibility = UIUtils.IsVisOrCollapsed(b);
        }
        private void Stop_Click(object sender, RoutedEventArgs e) {
            _sign.SetSignState(new SignState(SignEnum.Stop, Config.StopColor, Config.StopText));
            btnSlow.IsChecked = false;
            btnCustom.IsChecked = false;
        }

        private void Slow_Click(object sender, RoutedEventArgs e) {
            _sign.SetSignState(new SignState(SignEnum.Slow, Config.SlowColor, Config.SlowText));
            btnStop.IsChecked = false;
            btnCustom.IsChecked = false;
        }
        private void Custom_Click(object sender, RoutedEventArgs e) {
            _sign.SetSignState(new SignState(SignEnum.Custom, Config.CustomColor, Config.CustomText));
            btnStop.IsChecked = false;
            btnSlow.IsChecked = false;
        }

        private void chkManual_Click(object sender, RoutedEventArgs e) {
            SetManual(chkManual.IsChecked == true);
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
            var me = sender as MediaElement;
            if (me == null || me.Visibility == Visibility.Hidden)
                return;
            me.Position = TimeSpan.Zero;
            me.Play();
        }
    }
}
