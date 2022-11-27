using System.Text.Json;

namespace TimeToGo
{
    static class Persister
    {
        internal static async Task AddOrUpdate(AdventureActivity act)
        {
            var adv = await Read();
            var found = adv.Activities.SingleOrDefault(a => a.Id == act.Id);
            if (found == null)
                adv.Activities.Add(act);
            else
            {
                found.Name = act.Name;
                found.Location = act.Location;
                found.Start = act.Start;
                found.End = act.End;
                found.Duration = act.Duration;
            }
            await Write(adv);
        }

        static public async Task Write(Adventure adv)
        {
            var str = JsonSerializer.Serialize(adv);
            await WriteTextToFile("adventure.json", str);
        }
        static public async Task<Adventure> Read()
        {
            var str = await ReadTextFile("adventure.json");
            if (str == null)
                return new Adventure();
            return JsonSerializer.Deserialize<Adventure>(str);
        }
        static async Task WriteTextToFile(string filename, string text)
        {
            // Write the file content to the app data directory
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            using var outputStream = File.OpenWrite(targetFile);
            {
                using var streamWriter = new StreamWriter(outputStream);
                {
                    await streamWriter.WriteAsync(text);
                }
            }
        }
        static async Task<string> ReadTextFile(string filename)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            if (!File.Exists(targetFile))
                return null;

            using var InputStream = File.OpenRead(targetFile);
            {
                using var reader = new StreamReader(InputStream);
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

    }
}
