using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ARFCon {
    public partial class MainWindow : Window
    {
        const int _shortChange = 2;
        const int _longChange = 3;
        const string _left = "👈";
        const string _right = "👉";

        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(.1);
            _timer.Tick += Timer_Tick;
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
                pnlLogEvent.Visibility = Visibility.Hidden;
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
            imgArf2Slow.Visibility = Visibility.Hidden;
            imgArf2Stop.Visibility = Visibility.Hidden;
            if (state == ArfState.AllStop)
            {
                imgArf1Stop.Visibility = Visibility.Visible;
                imgArf2Stop.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Hidden;
                btnSwitch1.Content = _left + " Slow";
                btnSwitch2.Content = "Slow " + _right;
            }
            else if (state == ArfState.Stop1)
            {
                imgArf1Stop.Visibility = Visibility.Visible;
                imgArf2Slow.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Visible;
                btnSwitch1.Content = _left + " Slow";
                btnSwitch2.Content = "Stop " + _right;
            }
            else if (state == ArfState.Stop2)
            {
                imgArf1Slow.Visibility = Visibility.Visible;
                imgArf2Stop.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Visible;
                btnSwitch1.Content = _left + " Stop";
                btnSwitch2.Content = "Slow " + _right;
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        public enum ArfState
        {
            NA,
            AllStop,
            Stop1,
            Stop2,
        }
        private void AllStop_Click(object sender, RoutedEventArgs e)
        {
            Slow("All " + _stop, 3, true, ArfState.AllStop);

        }

        private void Switch1_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop1 || _currentState == ArfState.AllStop)
                Slow($"Switching {_camera2} to {_slow}", _shortChange, true, ArfState.Stop2);
            else // 1 is slow, 2 is stop
                Slow("All " + _stop, _shortChange, true, ArfState.AllStop);     // dont set other to slow because it might not be intended
        }

        static string _slow = "🐌 SLOW";
        static string _stop = "⛔ STOP";
        static string _camera1 = "Cam#1 North";
        static string _camera2 = "Cam#2 North";
        private void Switch2_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2 || _currentState == ArfState.AllStop)
                Slow($"Switching {_camera1} to {_slow}", _shortChange, true, ArfState.Stop1);
            else  // 2 is slow, 1 is stop
                Slow("All " + _stop, 5, true, ArfState.AllStop);    // dont set 1 to slow because it might not be intended
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2)
                Slow($"Switching {_camera2} to {_slow}", _shortChange, true, ArfState.Stop1);
            else
                Slow($"Switching {_camera1} to {_slow}", _shortChange, true, ArfState.Stop2);
        }

        private void LogEvent_Click(object sender, RoutedEventArgs e)
        {
            pnlLogEvent.Visibility = Visibility.Visible;
        }

        private void LogEventCancel_Click(object sender, RoutedEventArgs e)
        {
            pnlLogEvent.Visibility = Visibility.Hidden;
        }

        private void LogEventCreate_Click(object sender, RoutedEventArgs e)
        {
            Log("EVENT: " + eventTypeCombo.Text);
            pnlLogEvent.Visibility = Visibility.Hidden;
        }


        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
            (sender as MediaElement).Position = TimeSpan.Zero;
            (sender as MediaElement).Play();
        }
    }
}
