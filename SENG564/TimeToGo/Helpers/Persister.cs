using System.Text.Json;
using TimeToGo.Models;

namespace TimeToGo.Helpers
{
    /// <summary>
    /// CRUD for adventures and activities. Stores as JSON in the devices App directory.
    /// </summary>
    static class Persister
    {
        #region adventure methods
        // Read whole adventure, on error, return a blank adventure
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

        // Write whole adventure
        static public void Write(Adventure adv)
        {
            // TODO store backups, maybe on a central server
            Logger.Log("Persister.Write " + adv);
            var str = JsonSerializer.Serialize(adv);
            WriteTextToFile("adventure.json", str);
        }

        #endregion

        #region activity methods
        internal static AdventureActivity ReadActivity(string id)
        {
            Logger.Log("Persister.ReadActivity " + id);
            var adv = Read();
            return adv.Activities.SingleOrDefault(a => a.Id.Equals(id));
        }

        internal static void AddOrUpdateActivity(AdventureActivity act)
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

        internal static void DeleteActivity(AdventureActivity act)
        {
            var adv = Read();
            var found = adv.Activities.SingleOrDefault(a => a.Id == act.Id);
            adv.Activities.Remove(found);
            Write(adv);
        }

        // change order of an activity
        internal static void MoveActivity(AdventureActivity act, int delta)
        {
            var adv = Read();
            var found = adv.Activities.SingleOrDefault(a => a.Id == act.Id);
            var orig = adv.Activities.IndexOf(found);
            adv.Activities.Remove(found);
            adv.Activities.Insert(orig + delta, act);
            Write(adv);
        }
        
        // before moving, check if you can move it to show appropriate error
        internal static bool CanMoveActivity(AdventureActivity act, int delta)
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

        #endregion

        #region private low-level
        // write to app directory
        static void WriteTextToFile(string filename, string text)
        {
            var targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            File.WriteAllText(targetFile, text);
        }
        
        // read from app directory or return null
        static string ReadTextFile(string filename)
        {
            var targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            if (!File.Exists(targetFile))
                return null;

            return File.ReadAllText(targetFile);
        }
        #endregion
    }
}
