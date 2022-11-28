using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TimeToGo.AdventureActivity;

namespace TimeToGo
{

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
        public TimeSpan Duration { get; set; }

        public override string ToString()
        {
            return $"n:{Name} i:{Id} l:{Location} d:{Duration}";
        }
    }
}
