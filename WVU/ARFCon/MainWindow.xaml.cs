using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Media;
using static System.Net.Mime.MediaTypeNames;

namespace ARFCon {
    public partial class MainWindow : Window
    {
        const int _shortChange = 2;
        const int _longChange = 3;
        const string _left = "👈";
        const string _right = "👉";
        
        SoundPlayer _soundPlayer;

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(.1);
            _timer.Tick += Timer_Tick;
            staCamera1.Content = "Camera #1- " + Config.Camera1;
            staCamera2.Content = "Camera #2- " + Config.Camera2;
            // not started
            Slow("Initializing", _longChange, false, ArfState.AllStop);
        }

        DateTime _endAt;
        DispatcherTimer _timer;
        ArfState _currentState = ArfState.NA;
        ArfState _nextState;
        private void Slow(string str, int seconds, bool allowCancel, ArfState state) {
            Log(str);
            
            if (allowCancel)
                SetArf(ArfState.AllStop);
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
        private void Timer_Tick(object? sender, EventArgs e)
        {
            var remaining = ((_endAt - DateTime.Now).TotalSeconds);
            staRemaining.Text = $"{remaining.ToString("0.0")} seconds remaining";
            prg.Value = prg.Maximum - remaining;

            if (remaining <= 0)
            {
                _timer.Stop();
                pnlChanging.Visibility = Visibility.Hidden;
                Buttonability(true);
                SetArf(_nextState);
                meOut1.Play();
                meInb1.Play();
                meOut2.Play();
                meInb2.Play();
            }
        }

        private void SetArf(ArfState state)
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

            if (state == ArfState.AllStop)
            {
                imgArf1Stop.Visibility = Visibility.Visible;
                imgArf2Stop.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Hidden;
                btnSwitch1.Content = _left + " " + Config.SlowText;
                btnSwitch2.Content = Config.SlowText + " " + _right;
                Arf1Text.Text = Config.StopText;
                Arf2Text.Text = Config.StopText;
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
            }
            else if (state == ArfState.Custom)
            {
                imgArf1White.Visibility = Visibility.Visible;
                imgArf2White.Visibility = Visibility.Visible;
                Arf1Text.Text = Config.CustomText;
                Arf2Text.Text = Config.CustomText;
            }
            staArf1Status.Text = "Conn: GOOD!";
            staArf2Status.Text = "Conn: GOOD!";
            if (_currentState != _nextState)
                Log("STATE: " +  _nextState);
            _currentState = _nextState;
        }

        void Log(string str)
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
        private void AllStop_Click(object sender, RoutedEventArgs e)
        {
            Slow("All " + Config.StopText, 3, true, ArfState.AllStop);

        }

        private void Switch1_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop1 || _currentState == ArfState.AllStop)
                Slow($"Switching {Config.Camera2} to {Config.SlowText}", _shortChange, true, ArfState.Stop2);
            else // 1 is slow, 2 is stop
                Slow("All " + Config.StopText, _shortChange, true, ArfState.AllStop);     // dont set other to slow because it might not be intended
        }

        private void Switch2_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2 || _currentState == ArfState.AllStop)
                Slow($"Switching {Config.Camera1} to {Config.SlowText}", _shortChange, true, ArfState.Stop1);
            else  // 2 is slow, 1 is stop
                Slow("All " + Config.StopText, 5, true, ArfState.AllStop);    // dont set 1 to slow because it might not be intended
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2)
                Slow($"Switching {Config.Camera2} to {Config.SlowText}", _shortChange, true, ArfState.Stop1);
            else
                Slow($"Switching {Config.Camera1} to {Config.SlowText}", _shortChange, true, ArfState.Stop2);
        }

        private void LogEvent_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new EventWindow();
            if (wnd.ShowDialog() == true) 
                Log("EVENT: " + wnd.EventType + " notes: " + wnd.EventNotes);
        }

        private void Customize_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new CustomizeWindow();
            if (wnd.ShowDialog() == true) {
                Log("SIGN CHANGE: StopText=" + Config.StopText + ", SlowText=" + Config.SlowText + ", CustomText=" + Config.CustomText);
                if (wnd.ShowCustom)
                    SetArf(ArfState.Custom);
                else
                    SetArf(_currentState);
                staCamera1.Content = "Camera #1- " + Config.Camera1;
                staCamera2.Content = "Camera #2- " + Config.Camera2;
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
            (sender as MediaElement).Position = TimeSpan.Zero;
            (sender as MediaElement).Play();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {

        }
    }
}
