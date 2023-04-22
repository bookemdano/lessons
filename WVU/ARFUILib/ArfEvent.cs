﻿using System.IO;
using System.Text.Json;

namespace ARFUILib {
    public class ArfEvent {
        public ArfEvent()
        {
        }
        public ArfEvent(DateTime timestamp, string type, string notes) {
            Timestamp = timestamp;
            EventType = type;
            Notes = notes;
        }

        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string Notes { get; set; }
        public string Url {
            get {
                return @"https://localhost/videoarchive/" + Timestamp.Ticks;
            }
        }
        static public List<ArfEvent> GetArfEvents() {
            if (!File.Exists(Filename))
                return new List<ArfEvent>();
            return JsonSerializer.Deserialize<List<ArfEvent>>(File.ReadAllText(Filename));
        }
        static public void Add(string type, string notes) {
            var events = GetArfEvents();
            events.Add(new ArfEvent(DateTime.UtcNow, type, notes));
            var str = JsonSerializer.Serialize(events);
            File.WriteAllText(Filename, str);
        }
        public override string ToString() {
            return $"{Timestamp} {EventType} {Notes}";
        }
        static string Filename {
            get {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "arfevents.json");
            }
        }
    }
}
