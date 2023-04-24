using System.Diagnostics;
using System.IO;

namespace ARFUILib {
    static public class Logger {
        static public void Log(object o) {
			try {
                File.AppendAllText("endless.log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + o + Environment.NewLine);
            }
            catch (Exception) {
                Console.WriteLine("Failed to log, give up " + o);
			}
        }
        static public void View() {
            OpenWithDefaultProgram("endless.log");
        }
        static void OpenWithDefaultProgram(string path) {
            using Process fileopener = new();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }
    }
}
