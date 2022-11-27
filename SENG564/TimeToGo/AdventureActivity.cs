using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeToGo
{
    internal class Adventure
    {
        public List<AdventureActivity> Activities { get; set; } = new List<AdventureActivity>();
    }
    internal class AdventureActivity
    {
        public AdventureActivity() 
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Duration { get; set; }

        public override string ToString()
        {
            return $"{Name} {Id} {Location} {Start} {End} {Duration}";
        }
    }
}
