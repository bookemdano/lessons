using static System.Net.Mime.MediaTypeNames;

namespace ARFUILib {

    public interface IUi {
        void Log(object o);
    }
    public interface ISignListener : IUi {
        Task<SignState> StateChange(SignState signState);
    }
}
