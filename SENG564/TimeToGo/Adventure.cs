namespace TimeToGo
{
    internal class Adventure
    {
        public string Title { get; set; }
        public List<AdventureActivity> Activities { get; set; } = new List<AdventureActivity>();

        internal AdventureActivity Start()
        {
            return Activities.FirstOrDefault(a => a.Start != null);
        }
        internal AdventureActivity FakeActivity(ActivityTypeEnum activityEnum)
        {
            var rnd = new Random();
            if (activityEnum == ActivityTypeEnum.NA)
                activityEnum = (ActivityTypeEnum) (rnd.Next(2) + 1);
            var tripStart = Start()?.Start;
            if (tripStart == null)
                activityEnum = ActivityTypeEnum.Start;

            var act = new AdventureActivity();
            act.Name = $"Fun {activityEnum} Activity #" + (Activities.Count() + 1).ToString();
            act.Location = "Third star from the left";

            if (activityEnum == ActivityTypeEnum.Duration)
                act.Duration = TimeSpan.FromSeconds(rnd.Next(86400));
            else
            {
                if (activityEnum == ActivityTypeEnum.Start)
                {
                    if (tripStart == null)
                        act.Start = DateTime.Now.AddDays(3);
                    else
                        act.Start = tripStart.Value.AddSeconds(rnd.Next(86400));
                }
                else if (activityEnum == ActivityTypeEnum.End)
                    act.Start = tripStart.Value.AddSeconds(rnd.Next(86400));
            }
            return act;
        }
    }
}
