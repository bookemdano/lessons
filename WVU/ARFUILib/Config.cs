using System.IO;

namespace ARFUILib {
    static public class Config {
        static public string FullCameraName(int index) {
            var address = Config.GetCameraAddress(index);
            if (Config.LocalTesting)
                address = "testing";
            return $"Camera #{index + 1}- {Config.GetCameraName(index)} ({address})";
        }

        static public string GetCameraName(int index) {
            var def = "North";
            if (index == 1)
                def = "South";
            return ConfigCore.Get("CameraName" + index, def);
        }
        static public void SetCameraName(int index, string name) {
            ConfigCore.Set("CameraName" + index, name);
        }

        static public string GetCameraAddress(int index) {
            var def = "11160";
            if (index == 1)
                def = "11161";
            return ConfigCore.Get("CameraAddress" + index, def);
        }
        static public void SetCameraAddress(int index, string name) {
            ConfigCore.Set("CameraAddress" + index, name);
        }

        static public string StopText {
            get {
                return ConfigCore.Get("stop_text", "STOP");
            }
            set {
                ConfigCore.Set("stop_text", value);
            }
        }
        static public string SlowText {
            get {
                return ConfigCore.Get("slow_text", "SLOW");
            }
            set {
                ConfigCore.Set("slow_text", value);
            }
        }
        static public string CustomText {
            get {
                return ConfigCore.Get("custom_text", "Info");
            }
            set {
                ConfigCore.Set("custom_text", value);
            }
        }

        static public bool LocalTesting {
            get {
                return bool.Parse(ConfigCore.Get("LocalTesting", "true"));
            }
            set {
                ConfigCore.Set("LocalTesting", value.ToString());
            }
        }

        static public string StopColor {
            get {
                return ConfigCore.Get("StopColor", "Red");
            }
            set {
                ConfigCore.Set("StopColor", value);
            }
        }
        static public string SlowColor {
            get {
                return ConfigCore.Get("SlowColor", "Goldenrod");
            }
            set {
                ConfigCore.Set("SlowColor", value);
            }
        }
        static public string CustomColor {
            get {
                return ConfigCore.Get("CustomColor", "White");
            }
            set {
                ConfigCore.Set("CustomColor", value);
            }
        }
        static public string ErrorColor {
            get {
                return ConfigCore.Get("ErrorColor", "Orchid");
            }
            set {
                ConfigCore.Set("ErrorColor", value);
            }
        }
        static public TimeSpan HeartbeatTimeout {
            get {
                return TimeSpan.FromSeconds(5);// TimeSpan.Parse(ConfigCore.Get("Heartbeat(s)", "0:0:10"));
            }
            set {
                ConfigCore.Set("Heartbeat(s)", value.ToString());
            }
        }
    }
    static internal class ConfigCore {
        static Dictionary<string, string> _dict = null;
        static string Filename {
            get {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "arf.cfg");
            }
        }
        static void ReadAll() {
            _dict = new Dictionary<string, string>();
            if (!File.Exists(Filename))
                return;
            var lines = File.ReadAllLines(Filename);
            foreach(var line in lines) {
                var parts = line.Split("=");
                _dict.Add(parts[0].Trim(), parts[1].Trim());
            }
        }
        static void WriteAll() {
            var lines = _dict.Select(s => $"{s.Key} = {s.Value}");
            File.WriteAllLines(Filename, lines);
        }
        static internal string Get(string key, string def) {
            if (_dict == null)
                ReadAll();
            if (!_dict.ContainsKey(key))
                return def;
            return _dict[key];
        }
        static internal void Set(string key, string val) {
            if (_dict.ContainsKey(key) && _dict[key] == val)
                return;
            ReadAll();  // read first to sync in-memory with on-disk
            _dict[key] = val;
            WriteAll();
        }
    }
}
