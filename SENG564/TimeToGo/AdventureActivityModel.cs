namespace TimeToGo
{
    internal class AdventureActivityModel
    {
        private AdventureActivity _act;
        private DateTime _start;
        private TimeSpan _duration;
        private DateTime _end;

        public AdventureActivityModel(AdventureActivity act, DateTime start, TimeSpan duration, DateTime end) 
        {
            _act = act;
            _start = start;
            _duration = duration;
            _end = end;
        }

        public string Name
        {
            get
            {
                return _act.Name;
            }
        }
        public string Detail
        {
            get
            {
                return _act.Location;
            }
        }
    }
}
