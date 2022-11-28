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
            act.Location = "Third star from the left";
            act.Duration = TimeSpan.FromSeconds(rnd.Next(43200));
            return act;
        }
    }
}
