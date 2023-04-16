namespace ARFLib {
    static public class Logger {
        static public void Log(object o) {
			try {
                File.AppendAllText("endless.log", DateTime.Now.ToString("H:mm:ss.fff") + " " + o + Environment.NewLine);
            }
            catch (Exception) {
                Console.WriteLine("Failed to log, give up " + o);
			}
        }
    }
}
