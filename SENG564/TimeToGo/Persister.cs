using Microsoft.Extensions.Options;
using System.Text.Json;

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
            Adventure rv = null;
            if (str != null)
            {
                try
                {
                    rv = JsonSerializer.Deserialize<Adventure>(str);
                }
                catch (JsonException exc)
                {
                    Logger.Error("json file unreadable. Trashing it.", exc);
                }
            }
            if (rv?.Validate() != true)
                rv = new Adventure();
            return rv;
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

        internal static void DeleteActivity(AdventureActivity act)
        {
            var adv = Read();
            var found = adv.Activities.SingleOrDefault(a => a.Id == act.Id);
            adv.Activities.Remove(found);
            Write(adv);
        }

        internal static void Move(AdventureActivity act, int delta)
        {
            var adv = Read();
            var found = adv.Activities.SingleOrDefault(a => a.Id == act.Id);
            var orig = adv.Activities.IndexOf(found);
            adv.Activities.Remove(found);
            adv.Activities.Insert(orig + delta, act);
            Write(adv);
        }
        internal static bool CanMove(AdventureActivity act, int delta)
        {
            if (act == null)
                return false;
            var adv = Read();
            var found = adv.Activities.SingleOrDefault(a => a.Id == act.Id);
            var orig = adv.Activities.IndexOf(found);
            if (orig + delta < 0 || orig + delta + 1 > adv.Activities.Count())
                return false;

            return true;
        }
    }
}
