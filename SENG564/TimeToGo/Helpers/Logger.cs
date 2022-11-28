namespace TimeToGo.Helpers
{
    // Logging internal to device
    static internal class Logger
    {
        static internal void Error(string message, Exception exc)
        {
            Log($"[ERROR] {message} exc:{exc.Message} details:{exc}");
            // TODO Send errors to central server
        }
        static internal void Log(string message)
        {
            var line = $"{DateTime.Now}{message}";
            Console.WriteLine(line);
            AppendTextToFile("endless.log", line);
            // TODO eventually need to clean up this file
        }

        // Append the line to to a file in the app's data directory
        static void AppendTextToFile(string filename, string line)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            using var sw = File.AppendText(targetFile);
            {
                sw.WriteLine(line);
            }
        }
    }
}
