using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToGo
{
    static internal class Logger
    {
        static internal void Error(string message, Exception exc)
        {
            Log($"[ERROR] {message} exc:{exc.Message} details:{exc}");
        }
        static internal void Log(string message)
        {
            var line = $"{DateTime.Now}{message}";
            Console.WriteLine(line);
            AppendTextToFile("endless.log", line);
        }
        static void AppendTextToFile(string filename, string line)
        {
            // Write the file content to the app data directory
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            using var sw = File.AppendText(targetFile);
            {
                sw.WriteLine(line);
            }
        }
    }
}
