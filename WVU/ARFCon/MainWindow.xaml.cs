using ARFLib;
using System;
using System.Media;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ARFCon {
    public partial class MainWindow : Window, IUi
    {
        const int _shortChange = 2;
        const int _longChange = 3;
        const string _left = "👈";
        const string _right = "👉";
        
        SoundPlayer? _soundPlayer;
        SockSender _sock1;
        SockSender _sock2;


        public MainWindow()
        {
            InitializeComponent();
            _sock1 = new SockSender(this, Config.CameraAddress1);
            _sock2 = new SockSender(this, Config.CameraAddress2);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(.1);
            _timer.Tick += Timer_Tick;
            staCamera1.Content = "Camera #1- " + Config.CameraName1;
            staCamera2.Content = "Camera #2- " + Config.CameraName2;
            // not started
        }

        DateTime _endAt;
        DispatcherTimer _timer;
        ArfState _currentState = ArfState.NA;
        ArfState _nextState;
        private Socket _client;

        private async Task Slow(string str, int seconds, bool allowCancel, ArfState state) {
            Log(str);
            
            if (allowCancel)
                await SetArf(ArfState.AllStop);
            prg.Value = 0;
            prg.Maximum = seconds;
            if (state == ArfState.Stop1 || state == ArfState.AllStop)
                staArf2Status.Text = "comm...";
            if (state == ArfState.Stop2 || state == ArfState.AllStop)
                staArf1Status.Text = "comm...";
            Buttonability(false);
            _nextState = state;
            staChanging.Text = str;
            btnCancel.Visibility = allowCancel?Visibility.Visible:Visibility.Hidden;
            staRemaining.Text = $"{seconds} seconds remaining";
            pnlChanging.Visibility = Visibility.Visible;
            _endAt = DateTime.Now.AddSeconds(seconds);
            _timer.Start();
        }
        private void Buttonability(bool enable) {
            btnSwitch.IsEnabled = enable;
            btnSwitch1.IsEnabled = enable;
            btnSwitch2.IsEnabled = enable;
            btnLogEvent.IsEnabled = enable;
            btnCustomize.IsEnabled = enable;
        }
        private async void Timer_Tick(object? sender, EventArgs e)
        {
            var remaining = ((_endAt - DateTime.Now).TotalSeconds);
            staRemaining.Text = $"{remaining.ToString("0.0")} seconds remaining";
            prg.Value = prg.Maximum - remaining;

            if (remaining <= 0)
            {
                _timer.Stop();
                pnlChanging.Visibility = Visibility.Hidden;
                Buttonability(true);
                await SetArf(_nextState);
                meOut1.Play();
                meInb1.Play();
                meOut2.Play();
                meInb2.Play();
            }
        }

        private async Task SetArf(ArfState state)
        {
            imgArf1Slow.Visibility = Visibility.Hidden;
            imgArf1Stop.Visibility = Visibility.Hidden;
            imgArf1Hollow.Visibility = Visibility.Visible;
            imgArf1White.Visibility = Visibility.Hidden;
            Arf1Text.Text = "";
            imgArf2Slow.Visibility = Visibility.Hidden;
            imgArf2Stop.Visibility = Visibility.Hidden;
            imgArf2Hollow.Visibility = Visibility.Visible;
            imgArf2White.Visibility = Visibility.Hidden;
            Arf2Text.Text = "";

            var sign1 = new SignState();
            var sign2 = new SignState();
            if (state == ArfState.AllStop)
            {
                imgArf1Stop.Visibility = Visibility.Visible;
                imgArf2Stop.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Hidden;
                btnSwitch1.Content = _left + " " + Config.SlowText;
                btnSwitch2.Content = Config.SlowText + " " + _right;
                Arf1Text.Text = Config.StopText;
                Arf2Text.Text = Config.StopText;
                sign1.State = SignEnum.Stop;
                sign1.Text = Config.StopText;
                sign2.State = SignEnum.Stop;
                sign2.Text = Config.StopText;
            }
            else if (state == ArfState.Stop1)
            {
                imgArf1Stop.Visibility = Visibility.Visible;
                imgArf2Slow.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Visible;
                btnSwitch1.Content = _left + " " + Config.SlowText;
                btnSwitch2.Content = Config.StopText + " " + _right;
                Arf1Text.Text = Config.StopText;
                Arf2Text.Text = Config.SlowText;

                sign1.State = SignEnum.Stop;
                sign1.Text = Config.StopText;
                sign2.State = SignEnum.Slow;
                sign2.Text = Config.SlowText;
            }
            else if (state == ArfState.Stop2)
            {
                imgArf1Slow.Visibility = Visibility.Visible;
                imgArf2Stop.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Visible;
                btnSwitch1.Content = _left + " " + Config.StopText;
                btnSwitch2.Content = Config.SlowText + " " + _right;
                Arf1Text.Text = Config.SlowText;
                Arf2Text.Text = Config.StopText;
                sign1.State = SignEnum.Slow;
                sign1.Text = Config.SlowText;
                sign2.State = SignEnum.Stop;
                sign2.Text = Config.StopText;
            }
            else if (state == ArfState.Custom)
            {
                imgArf1White.Visibility = Visibility.Visible;
                imgArf2White.Visibility = Visibility.Visible;
                Arf1Text.Text = Config.CustomText;
                Arf2Text.Text = Config.CustomText;
                sign1.State = SignEnum.Custom;
                sign1.Text = Config.CustomText;
                sign2.State = SignEnum.Custom;
                sign2.Text = Config.CustomText;
            }
            if (await _sock1.Send(sign1))
                staArf1Status.Text = "Conn: good!";
            else
                staArf1Status.Text = "Conn: FAILED!";
            if (await _sock2.Send(sign2))
                staArf2Status.Text = "Conn: good!";
            else
                staArf2Status.Text = "Conn: FAILED!";
            //UpdateDefaultStyle local signs to reflect returned state from remotes
            if (_currentState != _nextState)
                Log("STATE: " +  _nextState);
            _currentState = _nextState;
        }

        public void Log(object str)
        {
            lst.Items.Insert(0, DateTime.Now.ToString("H:mm:ss") + " " + str);
            System.IO.File.AppendAllText("endless.log", DateTime.Now.ToString("H:mm:ss.fff") + " " + str + Environment.NewLine);
        }

        public enum ArfState
        {
            NA,
            AllStop,
            Stop1,
            Stop2,
            Custom,
        }
        private async void AllStop_Click(object sender, RoutedEventArgs e)
        {
            await Slow("All " + Config.StopText, 3, true, ArfState.AllStop);

        }

        private async void Switch1_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop1 || _currentState == ArfState.AllStop)
                await Slow($"Switching {Config.CameraName2} to {Config.SlowText}", _shortChange, true, ArfState.Stop2);
            else // 1 is slow, 2 is stop
                await Slow("All " + Config.StopText, _shortChange, true, ArfState.AllStop);     // dont set other to slow because it might not be intended
        }

        private async void Switch2_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2 || _currentState == ArfState.AllStop)
                await Slow($"Switching {Config.CameraName1} to {Config.SlowText}", _shortChange, true, ArfState.Stop1);
            else  // 2 is slow, 1 is stop
                await Slow("All " + Config.StopText, 5, true, ArfState.AllStop);    // dont set 1 to slow because it might not be intended
        }

        private async void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2)
                await Slow($"Switching {Config.CameraName2} to {Config.SlowText}", _shortChange, true, ArfState.Stop1);
            else
                await Slow($"Switching {Config.CameraName1} to {Config.SlowText}", _shortChange, true, ArfState.Stop2);
        }

        private void LogEvent_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new EventWindow();
            if (wnd.ShowDialog() == true) 
                Log("EVENT: " + wnd.EventType + " notes: " + wnd.EventNotes);
        }

        private async void Customize_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new CustomizeWindow();
            if (wnd.ShowDialog() == true) {
                Log("SIGN CHANGE: StopText=" + Config.StopText + ", SlowText=" + Config.SlowText + ", CustomText=" + Config.CustomText);
                if (wnd.ShowCustom)
                    await SetArf(ArfState.Custom);
                else
                    await SetArf(_currentState);
                staCamera1.Content = "Camera #1- " + Config.CameraName1;
                staCamera2.Content = "Camera #2- " + Config.CameraName2;

                _sock1 = new SockSender(this, Config.CameraAddress1);
                _sock2 = new SockSender(this, Config.CameraAddress2);
            }
        }

        private void SoundAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (_soundPlayer == null)
            {
                Log("Alarm triggered.");
                btnAlarm.Content = "Silence";
                _soundPlayer = new SoundPlayer(@"media\alarm.wav");
                _soundPlayer.PlayLooping();
            }
            else
            {
                Log("Alarm stopped.");
                btnAlarm.Content = "Alarm!";
                _soundPlayer.Stop();
                _soundPlayer = null;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
            var me = sender as MediaElement;
            if (me == null)
                return;
            me.Position = TimeSpan.Zero;
            me.Play();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await Slow("Initializing", _longChange, false, ArfState.AllStop);
        }
    }
}
