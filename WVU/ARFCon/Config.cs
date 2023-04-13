
namespace ARFCon {
    static internal class Config {
        static internal string StopText {
            get {
                return ConfigCore.Get("stop_text", "STOP");
            }
            set {
                ConfigCore.Set("stop_text", value);
            }
        }
        static internal string SlowText {
            get {
                return ConfigCore.Get("slow_text", "SLOW");
            }
            set {
                ConfigCore.Set("slow_text", value);
            }
        }
        static internal string CustomText {
            get {
                return ConfigCore.Get("custom_text", "");
            }
            set {
                ConfigCore.Set("custom_text", value);
            }
        }
    }
    static internal class ConfigCore {
           
        static internal string Get(string key, string def) {
            if (!System.IO.File.Exists(key + ".conf"))
                return def;
            return System.IO.File.ReadAllText(key + ".conf");
        }
        static internal void Set(string key, string val) {
            System.IO.File.WriteAllText(key + ".conf", val);
        }
    }
}
