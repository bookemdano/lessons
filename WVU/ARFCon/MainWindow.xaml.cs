using System;
using System.Windows;
using System.Windows.Threading;

namespace ARFCon {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            // not started

            Slow("Initializing", 3, false, ArfState.AllStop);
        }

        DateTime _endAt;
        DispatcherTimer _timer;
        ArfState _currentState = ArfState.NA;
        ArfState _nextState;
        private void Slow(string str, int seconds, bool allowCancel, ArfState state)
        {
            Log(str);
            
            SetArf(ArfState.AllStop);

            _nextState = state;
            staChanging.Text = str;
            btnCancel.Visibility = allowCancel?Visibility.Visible:Visibility.Hidden;
            staRemainig.Text = $"{seconds} seconds remaining";
            pnlChanging.Visibility = Visibility.Visible;
            _endAt = DateTime.Now.AddSeconds(seconds);
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var remaining = (int) ((_endAt - DateTime.Now).TotalSeconds);
            staRemainig.Text = $"{remaining} seconds remaining";
            if (remaining == 0)
            {
                _timer.Stop();
                pnlChanging.Visibility = Visibility.Hidden;
                SetArf(_nextState);
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
                btnSwitch1.Content = "⬅️ Slow";
                btnSwitch2.Content = "Slow ➡️";
            }
            else if (state == ArfState.Stop1)
            {
                imgArf1Stop.Visibility = Visibility.Visible;
                imgArf2Slow.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Visible;
                btnSwitch1.Content = "⬅️ Slow";
                btnSwitch2.Content = "Stop ➡️";
            }
            else if (state == ArfState.Stop2)
            {
                imgArf1Slow.Visibility = Visibility.Visible;
                imgArf2Stop.Visibility = Visibility.Visible;
                btnSwitch.Visibility = Visibility.Visible;
                btnSwitch1.Content = "⬅️ Stop";
                btnSwitch2.Content = "Slow ➡️";
            }
            if (_currentState != _nextState)
                Log("STATE: " +  _nextState);
            _currentState = _nextState;
        }

        void Log(string str)
        {
            lst.Items.Insert(0, DateTime.Now.ToString("H:mm:ss.fff") + " " + str);
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
                Slow($"Switching {_camera2} to {_slow}", 5, true, ArfState.Stop2);
            else // 1 is slow, 2 is stop
                Slow("All " + _stop, 5, true, ArfState.AllStop);     // dont set other to slow because it might not be intended
        }

        static string _slow = "🟨SLOW";
        static string _stop = "🛑STOP";
        static string _camera1 = "Cam#1 North";
        static string _camera2 = "Cam#2 North";
        private void Switch2_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2 || _currentState == ArfState.AllStop)
                Slow($"Switching {_camera1} to {_slow}", 5, true, ArfState.Stop1);
            else  // 2 is slow, 1 is stop
                Slow("All " + _stop, 5, true, ArfState.AllStop);    // dont set 1 to slow because it might not be intended
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2)
                Slow($"Switching {_camera2} to {_slow}", 5, true, ArfState.Stop1);
            else
                Slow($"Switching {_camera1} to {_slow}", 5, true, ArfState.Stop2);
        }
    }
}
