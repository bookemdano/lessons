using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToGo
{
    internal class Adventure
    {
        public string Title { get; set; }
        public List<AdventureActivity> Activities { get; set; } = new List<AdventureActivity>();
    }

    internal class AdventureActivity
    {
        public AdventureActivity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public AdventureActivity(string id = null)
        {
            if (id == null)
                Id = Guid.NewGuid().ToString();
            else
                Id = id;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public TimeSpan? Duration { get; set; }

        public override string ToString()
        {
            return $"n:{Name} i:{Id} l:{Location} s:{Start} e:{End} d:{Duration}";
        }
    }
}
