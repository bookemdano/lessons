using ARFCon;
using System.Drawing;
using System.Resources;

namespace ARFLib {
    public enum SignEnum {
        NA,
        Initialize,
        Stop,
        Slow,
        Custom,
        Error
    }
    public class SignState {
        public SignState()
        {
            
        }
        public Color CalcColor() {
            return CalcColor(State);
        }
        static public Color CalcColor(SignEnum signEnum) {
            if (signEnum == SignEnum.Stop)
                return System.Drawing.Color.FromName(Config.StopColor);
            else if (signEnum == SignEnum.Slow)
                return System.Drawing.Color.FromName(Config.SlowColor);
            else if (signEnum == SignEnum.Custom)
                return System.Drawing.Color.FromName(Config.CustomColor);
            else
                return System.Drawing.Color.FromName(Config.ErrorColor);
        }
        static public SignState Parse(string response) {
            var parts = response.Split('|');
            if (parts.Length == 3)
                return new SignState(Enum.Parse<SignEnum>(parts[0]), parts[1], parts[2]);
            else
                return null;
        }

        public SignState(SignEnum state, string colorName = "", string text = "")
        {
            State = state;
            ColorName = colorName;
            Text = text;
        }
        public SignEnum State { get; set; }
        public string Text { get; set; }
        public string ColorName { get; set; }
        internal string Serialize() {
            return $"{State}|{ColorName}|{Text}";
        }
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
