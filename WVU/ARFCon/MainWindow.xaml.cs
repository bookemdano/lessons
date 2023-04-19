using ARFLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace ARFCon {
    public partial class MainWindow : Window, IUi
    {
        const string _left = "👈";
        const string _right = "👉";
        
        SoundPlayer? _soundPlayer;
        List<SockSender> _socks;

        List<SignState> _signStates;
        List<TextBlock> _lstCommStatus;
        List<TextBlock> _lstArfSoundIcon;
        List<Panel> _lstArfPanel;
        List<TextBlock> _lstArfText;
        List<TextBlock> _lstArfStatus;
        List<System.Windows.Controls.Image> _lstArfMask;
        ArfState _currentState = ArfState.NA;

        public MainWindow()
        {
            InitializeComponent();
            _signStates = new();
            _socks = new();
            for (var i = 0; i < 2; i++) {
                _signStates.Add(new SignState());
                _socks.Add(new SockSender(this, Config.GetCameraAddress(i)));
            }
            _lstCommStatus = new() { staComm1, staComm2 };
            _lstArfPanel = new() { pnlArf1, pnlArf2 };
            _lstArfSoundIcon = new() { staSound1, staSound2 };
            _lstArfText = new() { staArf1, staArf2 };
            _lstArfStatus = new() { staArf1Status, staArf2Status };
            _lstArfMask = new() { imgMask1, imgMask2 };

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
            if (_switching > 0) {
                Log("Delay ping because busy.");
                return;
            }
            var reqSignStates = new List<SignState>();
            reqSignStates.Add(new SignState(SignEnum.Heartbeat, "", DateTime.Now.Ticks.ToString("X")));
            reqSignStates.Add(new SignState(SignEnum.Heartbeat, "", DateTime.Now.Ticks.ToString("X")));

            var tasks = new Dictionary<Tuple<int, DateTime>, Task<SignState>>();
            
            for (int i = 0; i < 2; i++)
                tasks.Add(Tuple.Create(i, DateTime.Now), Send(reqSignStates[i], i)); ;

            while (tasks.Any()) {
                var kvp = tasks.FirstOrDefault(t => t.Value.IsCompleted);
                if (kvp.Value == null) {
                    await Task.Delay(10);
                    continue;
                }
                var index = kvp.Key.Item1;
                var dt = kvp.Key.Item2;
                var resultState = kvp.Value.Result;
                var delta = DateTime.Now - dt;
                if (resultState.State == SignEnum.Error && _signStates[index].State != SignEnum.Error) {
                    _lstArfStatus[index].Text = $"Conn: FAILED!({delta.TotalMilliseconds.ToString("0")}ms)";
                    Log($"Camera #{index + 1} {resultState}");
                }
                else {
                    _lstArfStatus[index].Text = $"Conn: good!({delta.TotalMilliseconds.ToString("0")}ms)";
                }
                if (!_signStates[index].Same(resultState))
                    UpdateLocalSignState(resultState, index);
                _lstCommStatus[index].Visibility = Visibility.Hidden;

                tasks.Remove(kvp.Key);
            }
            UpdateButtons();

        }

        void UpdateCameraName() {
            staCamera1.Content = Config.FullCameraName(0);
            staCamera2.Content = Config.FullCameraName(1);
        }
        public async Task<SignState> Send(SignState signState, int index) {
            _lstCommStatus[index].Visibility = Visibility.Visible;
            if (Config.LocalTesting) {
                await Task.Delay(1000);
                if (signState.State == SignEnum.Heartbeat)
                    return _signStates[index];
                else
                    return signState;
            }
            else
                return await _socks[index].Send(signState);

        }

        int _switching = 0;
        private async Task Switch(ArfState state) {
            Log("Switch to " + state);
            _switching++;
            if (_switching > 1)
                Log("Overlapping");

            Buttonability(false);

            staArf1Status.Text = "comm...";
            staArf2Status.Text = "comm...";
            var reqSignStates = new List<SignState>();
            reqSignStates.Add(ArfStateToSignState(state, 0));
            reqSignStates.Add(ArfStateToSignState(state, 1));

            var tasks = new Dictionary<int, Task<SignState>>();
            int i = 0;
            foreach (var signState in reqSignStates) {
                tasks.Add(i, Send(signState, i));
                i++;
            }
            while(tasks.Any()) {
                var kvp = tasks.FirstOrDefault(t => t.Value.IsCompleted);
                if (kvp.Value == null) {
                    await Task.Delay(100);
                    continue;
                }
                var index = kvp.Key;
                var resultState = kvp.Value.Result;
                if (resultState?.Same(reqSignStates[index]) == true)
                    _lstArfStatus[index].Text = "Conn: good!";
                else {
                    _lstArfStatus[index].Text = "Conn: FAILED!";
                    Log($"Camera #{index + 1} {resultState}");
                }
                UpdateLocalSignState(resultState, index);
                _lstCommStatus[index].Visibility = Visibility.Hidden;
                tasks.Remove(index);
            }
            UpdateButtons();
            Buttonability(true);
            _currentState = state;
            _switching--;
            return;
        }
        private void Buttonability(bool enable) {
            btnLogEvent.IsEnabled = enable;
            btnCustomize.IsEnabled = enable;
            btnSwitch.IsEnabled = enable;
            btnSwitch1.IsEnabled = enable;
            btnSwitch2.IsEnabled = enable;
            if (enable) {
                if (_signStates[0].State == SignEnum.Error) {
                    btnSwitch.IsEnabled = false;
                    btnSwitch1.IsEnabled = false;
                }
                if (_signStates[1].State == SignEnum.Error) {
                    btnSwitch.IsEnabled = false;
                    btnSwitch2.IsEnabled = false;
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
        Visibility IsVis(bool b) {
            if (b)
                return Visibility.Visible;
            else 
                return Visibility.Hidden;
        }
        void UpdateLocalSignState(SignState state, int index) {
            var fontSize = 48;
            var text = state.Text;
            if (text.Length > 5) {
                fontSize = 18;
                text = text.Substring(0, 15);
            }
            _lstArfText[index].Text = text;
            _lstArfText[index].FontSize = fontSize;
            _lstArfMask[index].Visibility = IsVis(!SignState.IsStopState(state.State));
            _lstArfPanel[index].Background = UILib.GetBrush(state.CalcColor());
            _lstArfSoundIcon[index].Visibility = IsVis(state.State == SignEnum.Alarm);

            PlaySound(state.State == SignEnum.Alarm, index);
            _signStates[index] = state;
        }


        void UpdateButtons() {
            var state1 = _signStates[0];
            var state2 = _signStates[1];
            if (state1.State == SignEnum.Slow && state2.State == SignEnum.Stop) {
                btnSwitch.Visibility = IsVis(true);
                btnSwitch1.Content = _left + " " + Config.StopText;
                btnSwitch2.Content = Config.SlowText + " " + _right;
            }
            else if (state1.State == SignEnum.Stop && state2.State == SignEnum.Slow) {
                btnSwitch.Visibility = IsVis(true);
                btnSwitch1.Content = _left + " " + Config.SlowText;
                btnSwitch2.Content = Config.StopText + " " + _right;
            }
            else { 
                btnSwitch.Visibility = IsVis(false);
                btnSwitch1.Content = _left + " " + Config.SlowText;
                btnSwitch2.Content = Config.SlowText + " " + _right;
            }
            if (state1.State == SignEnum.Error)
                btnSwitch1.Content = _left + " " + Config.StopText;
            if (state2.State == SignEnum.Error)
                btnSwitch2.Content = Config.StopText + " " + _right;
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
            await Switch(ArfState.AllStop);
        }

        private async void Switch1_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop1 || _currentState == ArfState.AllStop)
                await Switch(ArfState.Stop2);
            else // 1 is slow, 2 is stop
                await Switch(ArfState.AllStop);     // dont set other to slow because it might not be intended
        }

        private async void Switch2_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2 || _currentState == ArfState.AllStop)
                await Switch(ArfState.Stop1);
            else  // 2 is slow, 1 is stop
                await Switch(ArfState.AllStop);    // dont set 1 to slow because it might not be intended
        }

        private async void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (_currentState == ArfState.Stop2)
                await Switch(ArfState.Stop1);
            else
                await Switch(ArfState.Stop2);
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
                    await Switch(ArfState.Custom);
                else
                    await Switch(_currentState);
                UpdateCameraName();
            }
        }

        void PlaySound(bool b, int index) {
            _lstArfSoundIcon[index].Visibility = b ? Visibility.Visible : Visibility.Collapsed;
            if (b) {
                if (_soundPlayer != null)
                    return;
                //_soundPlayer = new SoundPlayer(@"media\alarm.wav");
                //_soundPlayer.PlayLooping();
            }
            else {
                if (_soundPlayer == null)
                    return;
                _soundPlayer.Stop();
                _soundPlayer = null;
            }
        }

        private async void SoundAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (_soundPlayer == null)
            {
                Log("Alarm triggered.");
                await Switch(ArfState.Alarm);
            }
            else
            {
                Log("Alarm stopped.");
                await Switch(ArfState.AllStop);
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
            await Switch(ArfState.AllStop);
        }
    }
}
