using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TimeToGo.AdventureActivity;

namespace TimeToGo
{
    public enum ActivityTypeEnum
    {
        NA = 0,
        Start = 1,
        Duration = 2,
        End = 3
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

        public ActivityTypeEnum ActivityType()
        {
            if (Start != null)
                return ActivityTypeEnum.Start;
            else if (End != null)
                return ActivityTypeEnum.End;
            else
                return ActivityTypeEnum.Duration;

        }
        public override string ToString()
        {
            return $"n:{Name} i:{Id} l:{Location} s:{Start} e:{End} d:{Duration}";
        }
    }
}
