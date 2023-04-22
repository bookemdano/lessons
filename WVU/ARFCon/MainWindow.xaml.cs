using ARFUILib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace ARFCon {
    public partial class MainWindow : Window, ISignListener
    {
        const string _left = "👈";
        const string _right = "👉";
        
        ArfState _currentState = ArfState.NA;
        List<SignView> _signs = new ();
        public MainWindow()
        {
            InitializeComponent();
            _signs.Add(new SignView(this, pnlArf1, staArf1, staSound1, staComm1, null, imgMask1, staArf1Status, 0));
            _signs.Add(new SignView(this, pnlArf2, staArf2, staSound2, staComm2, null, imgMask2, staArf2Status, 1));

            meInb1.Play();
            meInb2.Play();
            meOut1.Play();
            meOut2.Play();
            UpdateCameraName();
            var timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = Config.HeartbeatTimeout;
            timer.Start();
            // not started
        }

        private async void Timer_Tick(object? sender, EventArgs e) {
            if (_changing > 0) {
                Log("Delay ping because busy.");
                return;
            }
            var reqSignStates = new List<SignState>();
            reqSignStates.Add(new SignState(SignEnum.Heartbeat, "", DateTime.Now.Ticks.ToString("X")));
            reqSignStates.Add(new SignState(SignEnum.Heartbeat, "", DateTime.Now.Ticks.ToString("X")));

            var tasks = new Dictionary<Tuple<int, DateTime>, Task<SignState>>();
            
            for (int i = 0; i < 2; i++)
                tasks.Add(Tuple.Create(i, DateTime.Now), _signs[i].Send(reqSignStates[i])); ;

            while (tasks.Any()) {
                var kvp = tasks.FirstOrDefault(t => t.Value.IsCompleted);
                if (kvp.Value == null) {
                    await Task.Delay(10);
                    continue;
                }
                var index = kvp.Key.Item1;
                var dt = kvp.Key.Item2;
                var resultState = kvp.Value.Result;
                _signs[index].UpdateAfterHeartbeatSend(dt, resultState);
                tasks.Remove(kvp.Key);
            }
            UpdateButtons();

        }

        void UpdateCameraName() {
            staCamera1.Content = Config.FullCameraName(0);
            staCamera2.Content = Config.FullCameraName(1);
        }

        int _changing = 0;
        private async Task Change(ArfState state) {
            Log("Set " + state);
            _changing++;
            if (_changing > 1)
                Log("Overlapping");

            Buttonability(false);

            _signs[0].StartComm();
            _signs[1].StartComm();

            var tasks = new Dictionary<int, Task<SignState>>();
            tasks.Add(0, _signs[0].Send(ArfStateToSignState(state, 0)));
            tasks.Add(1, _signs[1].Send(ArfStateToSignState(state, 1)));

            while(tasks.Any()) {
                var kvp = tasks.FirstOrDefault(t => t.Value.IsCompleted);
                if (kvp.Value == null) {
                    await Task.Delay(100);
                    continue;
                }
                var index = kvp.Key;
                _signs[kvp.Key].UpdateAfterSend(kvp.Value.Result, ArfStateToSignState(state, index));
                tasks.Remove(index);
            }
            UpdateButtons();
            Buttonability(true);
            _currentState = state;
            _changing--;
            return;
        }

        private void Buttonability(bool enable) {
            if (enable && _slowChange)
                return;
            btnLogEvent.IsEnabled = enable;
            btnCustomize.IsEnabled = enable;
            btnSwap.IsEnabled = enable;
            btnSetArf1.IsEnabled = enable;
            btnSetArf2.IsEnabled = enable;
            if (enable) {
                if (_signs[0].MySignState.State == SignEnum.Error) {
                    btnSwap.IsEnabled = false;
                    btnSetArf1.IsEnabled = false;
                }
                if (_signs[1].MySignState.State == SignEnum.Error) {
                    btnSwap.IsEnabled = false;
                    btnSetArf2.IsEnabled = false;
                }
            }
        }

        private SignState ArfStateToSignState(ArfState state, int camera) {
            var rv = new SignState();
            if (state == ArfState.AllStop || state == ArfState.Alarm ||
                (state == ArfState.Stop1 && camera == 0) ||
                (state == ArfState.Stop2 && camera == 1)) {
                rv.State = SignEnum.Stop;
                rv.Text = Config.StopText;
                rv.ColorName = Config.StopColor;
            }
            else if ((state == ArfState.Stop1 && camera == 1) ||
                (state == ArfState.Stop2 && camera == 0)) {
                rv.State = SignEnum.Slow;
                rv.Text = Config.SlowText;
                rv.ColorName = Config.SlowColor;
            }
            else if (state == ArfState.Custom) {
                rv.State = SignEnum.Custom;
                rv.Text = Config.CustomText;
                rv.ColorName = Config.CustomColor;
            }
            if (state == ArfState.Alarm)
                rv.State = SignEnum.Alarm;
            return rv;
        }

        void UpdateButtons() {
            if (_slowChange)
                return;
            var state1 = _signs[0].MySignState;
            var state2 = _signs[1].MySignState;
            if (state1.State == SignEnum.Slow && state2.State == SignEnum.Stop) {
                btnSwap.Visibility = UIUtils.IsVis(true);
                btnSetArf1.Content = _left + " " + Config.StopText;
                btnSetArf2.Content = Config.SlowText + " " + _right;
            }
            else if (state1.State == SignEnum.Stop && state2.State == SignEnum.Slow) {
                btnSwap.Visibility = UIUtils.IsVis(true);
                btnSetArf1.Content = _left + " " + Config.SlowText;
                btnSetArf2.Content = Config.StopText + " " + _right;
            }
            else { 
                btnSwap.Visibility = UIUtils.IsVis(false);
                btnSetArf1.Content = _left + " " + Config.SlowText;
                btnSetArf2.Content = Config.SlowText + " " + _right;
            }
            if (state1.State == SignEnum.Error)
                btnSetArf1.Content = _left + " " + Config.StopText;
            if (state2.State == SignEnum.Error)
                btnSetArf2.Content = Config.StopText + " " + _right;
        }

        public void Log(object str)
        {
            lst.Items.Insert(0, DateTime.Now.ToString("H:mm:ss") + " " + str);
            Logger.Log(str);
        }

        public enum ArfState
        {
            NA,
            AllStop,
            Stop1,
            Stop2,
            Custom,
            Alarm
        }
        private async void AllStop_Click(object sender, RoutedEventArgs e)
        {
            Log("USER All stop");
            await Change(ArfState.AllStop);
        }

        private async void SetArf1_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop1 || _currentState == ArfState.AllStop) {
                Log("USER Set " + Config.FullCameraName(0) + " " + Config.SlowText);
                await Change(ArfState.Stop2);
            }
            else { // 1 is slow, 2 is stop
                await Change(ArfState.AllStop);     // dont set other to slow because it might not be intended
                Log("USER Set " + Config.FullCameraName(0) + " " + Config.StopText);
            }
        }

        private async void SetArf2_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2 || _currentState == ArfState.AllStop) {
                Log("USER Set " + Config.FullCameraName(1) + " " + Config.SlowText);
                await Change(ArfState.Stop1);
            }
            else { // 1 is slow, 2 is stop
                await Change(ArfState.AllStop);     // dont set other to slow because it might not be intended
                Log("USER Set " + Config.FullCameraName(1) + " " + Config.StopText);
            }
        }
        bool _slowChange = false;
        private async void Swap_Click(object sender, RoutedEventArgs e)
        {
            Log("USER Swapping signs");
            _slowChange = true;
            var oldState = _currentState;

            await Change(ArfState.AllStop);
            
            await Task.Delay(Config.SwapDelay);

            _slowChange = false;
            if (oldState == ArfState.Stop2)
                await Change(ArfState.Stop1);
            else
                await Change(ArfState.Stop2);
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
                    await Change(ArfState.Custom);
                else
                    await Change(_currentState);
                UpdateCameraName();
            }
        }

        bool _alarming = false;
        private async void SoundAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (!_alarming)
            {
                Log("Alarm triggered.");
                await Change(ArfState.Alarm);
            }
            else
            {
                Log("Alarm stopped.");
                await Change(ArfState.AllStop);
            }
            _alarming = !_alarming;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e) {
            var me = sender as MediaElement;
            if (me == null)
                return;
            me.Position = TimeSpan.Zero;
            me.Play();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            await Change(ArfState.AllStop);
        }

        public Task<SignState> StateChange(SignState signState) {
            // used only for listener
            throw new NotImplementedException();
        }
    }
}
