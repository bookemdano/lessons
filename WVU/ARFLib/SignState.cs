using ARFCon;
using System.Drawing;

namespace ARFLib {
    public enum SignEnum {
        NA,
        Initialize,
        Stop,
        Slow,
        Custom,
        Alarm,
        Error,
        Heartbeat
    }
    public class SignState {
        public SignState()
        {
        }
        public SignState(SignEnum state, string colorName, string text) {
            State = state;
            if (colorName == null)
                ColorName = CalcColor(state).Name;
            else
                ColorName = colorName;
            Text = text;
        }
        public Color CalcColor() {
            return CalcColor(State);
        }
        static public Color CalcColor(SignEnum signEnum) {
            if (signEnum == SignEnum.Stop)
                return Color.FromName(Config.StopColor);
            else if (signEnum == SignEnum.Slow)
                return Color.FromName(Config.SlowColor);
            else if (signEnum == SignEnum.Custom)
                return Color.FromName(Config.CustomColor);
            else
                return Color.FromName(Config.ErrorColor);
        }

        public SignEnum State { get; set; }
        public string Text { get; set; }
        public string ColorName { get; set; }
        public override string ToString() {
            return $"{State} with {Text}";
        }

        public bool Same(SignState signState) {
            if (signState == null)
                return false;
            if (State != signState.State)
                return false;
            if (Text != signState.Text)
                return false;
            if (ColorName != signState.ColorName)
                return false;
            return true;
        }
    }
}
