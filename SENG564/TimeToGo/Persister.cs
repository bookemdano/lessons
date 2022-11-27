﻿using System.Text.Json;

namespace TimeToGo
{
    static class Persister
    {
        internal static void AddOrUpdate(AdventureActivity act)
        {
            Logger.Log("Persister.AddOrUpdate " + act);
            var adv = Read();
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
            Write(adv);
        }

        internal static AdventureActivity ReadActivity(string id)
        {
            var adv = Read();
            return adv.Activities.SingleOrDefault(a => a.Id.Equals(id));
        }

        static public void Write(Adventure adv)
        {
            var str = JsonSerializer.Serialize(adv);
            WriteTextToFile("adventure.json", str);
        }
        static public Adventure Read()
        {
            var str = ReadTextFile("adventure.json");
            if (str == null)
                return new Adventure();
            try
            {
                return JsonSerializer.Deserialize<Adventure>(str);

            }
            catch (JsonException exc)
            {
                Logger.Error("json file unreadable. Trashing it.", exc);
                return new Adventure();
            }
        }
        static void WriteTextToFile(string filename, string text)
        {
            // Write the file content to the app data directory
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            File.WriteAllText(targetFile, text);
        }
        static string ReadTextFile(string filename)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            if (!File.Exists(targetFile))
                return null;

            return File.ReadAllText(targetFile);
        }

    }
}
