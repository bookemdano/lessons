using ARFLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ARFCon {
    public partial class MainWindow : Window, IUi
    {
        const string _left = "👈";
        const string _right = "👉";
        
        SoundPlayer? _soundPlayer;
        List<SockSender> _socks = new List<SockSender>();

        List<SignState> _signStates = new List<SignState>();
        ArfState _currentState = ArfState.NA;

        public MainWindow()
        {
            InitializeComponent();
            _signStates.Add(new SignState(SignEnum.NA));
            _signStates.Add(new SignState(SignEnum.NA));
            _socks.Add(new SockSender(this, Config.CameraAddress1));
            _socks.Add(new SockSender(this, Config.CameraAddress2));
            meInb1.Play();
            meInb2.Play();
            meOut1.Play();
            meOut2.Play();
            UpdateCameraName();
            // not started
        }
        void UpdateCameraName() {
            var address1 = $"localhost:{Config.CameraAddress1}";
            var address2 = $"localhost:{Config.CameraAddress2}";
            if (Config.LocalTesting) {
                address1 = "testing";
                address2 = "testing";
            }
            staCamera1.Content = $"Camera #1- {Config.CameraName1} ({address1})";
            staCamera2.Content = $"Camera #2- {Config.CameraName2} ({address2})";
        }
        public async Task<SignState> Send(SignState signState, int index) {
            if (Config.LocalTesting) {
                await Task.Delay(1000);
                return signState;
            }
            else
                return await _socks[index].Send(signState);
        }

        private async Task Switch(ArfState state) {
            Log("Switch to " + state);

            Buttonability(false);

            staArf1Status.Text = "comm...";
            staArf2Status.Text = "comm...";
            var reqSignStates = new List<SignState>();
            reqSignStates.Add(ArfStateToSignState(state, 0));
            reqSignStates.Add(ArfStateToSignState(state, 1));

            var staArfs = new List<TextBlock>() {staArf1Status, staArf2Status};
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
                    staArfs[index].Text = "Conn: good!";
                else {
                    staArfs[index].Text = "Conn: FAILED!";
                    Log($"Camera #{index + 1} {resultState}");
                }
                _signStates[index] = resultState;
                UpdateLocalSignState(resultState, index);
                tasks.Remove(index);
            }
            UpdateButtons();
            Buttonability(true);
            _currentState = state;
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
            if (state == ArfState.AllStop ||
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
            if (index == 0) {
                pnlArf1.Background = UILib.GetBrush(state.CalcColor());
                staArf1.Text = text;
                staArf1.FontSize = fontSize;
            }
            if (index == 1) {
                pnlArf2.Background = UILib.GetBrush(state.CalcColor());
                staArf2.Text = text;
                staArf2.FontSize = fontSize;
            }
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

                _socks[0] = new SockSender(this, Config.CameraAddress1);
                _socks[1] = new SockSender(this, Config.CameraAddress2);
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
            await Switch(ArfState.AllStop);
        }
    }
}
