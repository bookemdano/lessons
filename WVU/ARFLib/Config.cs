using System.IO;

namespace ARFCon {
    static public class Config {
        static public string CameraName1 {
            get {
                return ConfigCore.Get("CameraName1", "North");
            }
            set {
                ConfigCore.Set("CameraName1", value);
            }
        }
        static public string CameraName2 {
            get {
                return ConfigCore.Get("CameraName2", "South");
            }
            set {
                ConfigCore.Set("CameraName2", value);
            }
        }
        static public string CameraAddress1 {
            get {
                return ConfigCore.Get("CameraAddress1", "0");
            }
            set {
                ConfigCore.Set("CameraAddress1", value);
            }
        }
        static public string CameraAddress2 {
            get {
                return ConfigCore.Get("CameraAddress2", "1");
            }
            set {
                ConfigCore.Set("CameraAddress2", value);
            }
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
                return ConfigCore.Get("custom_text", "");
            }
            set {
                ConfigCore.Set("custom_text", value);
            }
        }
        static public string LocalSignAddress {
            get {
                return ConfigCore.Get("LocalSignAddress", "0");
            }
            set {
                ConfigCore.Set("LocalSignAddress", value);
            }
        }

        static public bool LocalTesting {
            get {
                return bool.Parse(ConfigCore.Get("LocalTesting", "false"));
            }
            set {
                ConfigCore.Set("LocalTesting", value.ToString());
            }
        }

    }
    static internal class ConfigCore {
           
        static internal string Get(string key, string def) {
            if (!File.Exists(key + ".conf"))
                return def;
            return File.ReadAllText(key + ".conf");
        }
        static internal void Set(string key, string val) {
            File.WriteAllText(key + ".conf", val);
        }
    }
}
