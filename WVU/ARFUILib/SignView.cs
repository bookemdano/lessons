using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;

namespace ARFUILib {
    public class SignView {
        private ISignListener _ui;
        private Panel _pnl;
        private TextBlock _staArf;
        private TextBlock _icoSound;
        private TextBlock _icoComm;
        private MediaElement _meSiren;
        private Image _imgMask;
        private TextBlock _staStatus;
        private int _index;
        private SockSender _sockSender = null;
        private SockListener _sockListener = null;
        private SoundPlayer _soundPlayer;

        SignState _signState = new SignState(SignEnum.Initialize, "", "-");
        public SignState MySignState {
            get {
                return _signState;
            }
        }

        //pnl, staArf, staSound, staComm, imgMask
       public SignView(ISignListener ui, Panel pnl, TextBlock staArf, TextBlock staSound, TextBlock staComm, MediaElement siren, Image imgMask, TextBlock staStatus, int index) {
            _ui = ui;
            _pnl = pnl;
            _staArf = staArf;
            _icoSound = staSound;
            _icoComm = staComm;
            _meSiren = siren;
            _imgMask = imgMask;
            _staStatus = staStatus;
            _index = index;
        }

        public void SetSignState(SignState signState) {
            var fontSize = 48;
            var text = signState.Text;
            if (text.Length > 5) {
                fontSize = 18;
                text = text.Substring(0, 15);
            }
            _imgMask.Visibility = UIUtils.IsVis(!SignState.IsStopState(signState.State));
            _pnl.Background = UIUtils.GetBrush(System.Drawing.Color.FromName(signState.ColorName));
            _staArf.FontSize = fontSize;
            _staArf.Text = text;
            PlaySound(signState.State == SignEnum.Alarm);
            _signState = signState;
        }

        public void PlaySound(bool b) {
            _icoSound.Visibility = UIUtils.IsVis(b);
            if (_meSiren != null) {
                _meSiren.Visibility = UIUtils.IsVis(b);
                if (b)
                    _meSiren.Play();
            }
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
        public async Task<SignState> Send(SignState signState) {
            _icoComm.Visibility = Visibility.Visible;
            if (Config.LocalTesting) {
                await Task.Delay(500);
                if (signState.State == SignEnum.Heartbeat)
                    return MySignState;
                else
                    return signState;
            }
            else {
                if (_sockSender == null)
                    _sockSender = new SockSender(_ui, Config.GetCameraAddress(_index));

                return await _sockSender.Send(signState);
            }
        }
        static public bool TimedOut(DateTime lastConnection) {
            return (DateTime.Now - lastConnection) > Config.HeartbeatTimeout * 1.5; 
        }

        public bool DoneListeningTimer(DateTime lastConnection, bool manual) {
            var delta = DateTime.Now - lastConnection;
            _staStatus.Text = $"Last conn. {delta.TotalSeconds.ToString("0")} secs ago";
            _icoComm.Visibility = Visibility.Hidden;
            if (TimedOut(lastConnection) && !manual) {
                SetSignState(new SignState(SignEnum.Error, null, "Console Disconnected"));
                return false;
            }
            return true;
        }

        public void UpdateAfterHeartbeatSend(DateTime dt, SignState resultState) {
            var delta = DateTime.Now - dt;
            if (resultState.State == SignEnum.Error) {
                _staStatus.Text = $"Conn: FAILED!({delta.TotalMilliseconds.ToString("0")}ms)";
                if (MySignState.State != SignEnum.Error)    // is this new
                    _ui.Log($"Camera #{_index + 1} {resultState}");
            }
            else 
                _staStatus.Text = $"Conn: good!({delta.TotalMilliseconds.ToString("0")}ms)";
            
            if (!MySignState.Same(resultState))
                SetSignState(resultState);
            _icoComm.Visibility = Visibility.Hidden;
        }
        public void UpdateAfterSend(SignState resultState, SignState requestedState) {
            if (resultState?.Same(requestedState) == true)
                _staStatus.Text = "Conn: good!";
            else {
                _staStatus.Text = "Conn: FAILED!";
                _ui.Log($"Camera #{_index + 1} {resultState}");
            }
            SetSignState(resultState);
            _icoComm.Visibility = Visibility.Hidden;
        }


        public void StartComm() {
            _staStatus.Text = "comm...";
            _icoComm.Visibility = Visibility.Visible;
        }

        public void KillSend() {
            _sockListener.Kill();
        }

        public async Task<bool> ListenOnce(string text) {
            if (_sockListener == null)
                _sockListener = new SockListener(_ui);
            return await _sockListener.ListenOnce(text);
        }
    }
}
