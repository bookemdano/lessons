using static System.Net.Mime.MediaTypeNames;

namespace ARFLib {

    public interface IUi {
        void Log(object o);
    }
    public interface ISignListener : IUi {
        Task<SignState> StateChange(SignState signState);
    }
}
