using System.Resources;

namespace ARFLib {
    public enum SignEnum {
        NA,
        Initialize,
        Stop,
        Slow,
        Custom
    }
    public class SignState {
        public SignState()
        {
            
        }

        static public SignState Parse(string response) {
            var parts = response.Split('|');
            if (parts.Length == 2)
                return new SignState(Enum.Parse<SignEnum>(parts[0]), parts[1]);
            else
                return null;
        }

        public SignState(SignEnum state, string text)
        {
            State = state;
            Text = text;
        }
        public SignEnum State { get; set; }
        public string Text { get; set; }

        internal string Serialize() {
            return $"{State}|{Text}";
        }
        public override string ToString() {
            return $"{State} with {Text}";
        }

        public bool Same(SignState signState) {
            if (State != signState.State)
                return false;
            if (Text != signState.Text) 
                return false;
            return true;
        }
    }
}
