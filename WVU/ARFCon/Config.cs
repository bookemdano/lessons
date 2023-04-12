
namespace ARFCon {
    static internal class Config {
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
