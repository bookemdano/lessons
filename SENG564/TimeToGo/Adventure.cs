using System.Net.Http.Headers;

namespace TimeToGo
{
    internal class Adventure
    {
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public List<AdventureActivity> Activities { get; set; } = new List<AdventureActivity>();

        internal DateTime Start()
        {
            return Deadline.AddMinutes(0 - Duration().TotalMinutes);
        }
        internal TimeSpan Duration()
        {
            var minutes = Activities.Sum(a => a.Duration.TotalMinutes);
            return TimeSpan.FromMinutes(minutes);
        }
        internal AdventureActivity FakeActivity()
        {
            var rnd = new Random();
            var act = new AdventureActivity();
            act.Name = $"Fun Activity #" + (Activities.Count() + 1).ToString();
            var sel = rnd.Next(7);
            if (sel == 0)
                act.Location = "Reno, NV";
            else if (sel == 1)
                act.Location = "Yellowstone National Park";
            else if (sel == 2)
                act.Location = "Montana University";
            else if (sel == 3)
                act.Location = "Glacier National Park";
            else if (sel == 4)
                act.Location = "Badlands";
            else if (sel == 5)
                act.Location = "SLC";
            else
                act.Location = null;
            act.Duration = TimeSpan.FromSeconds(rnd.Next(43200));
            return act;
        }

        internal static Adventure Fake()
        {
            var rv = new Adventure();
            rv.Title = "Airport";
            rv.Deadline = DateTime.Today.AddDays(3).AddHours(13);
            return rv;
        }

        internal bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title) || Deadline == DateTime.MinValue)
                return false;
            return true;
        }
    }
}
