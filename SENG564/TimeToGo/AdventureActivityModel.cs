using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeToGo
{
    internal class AdventureActivityModel : INotifyPropertyChanged
    {
        public AdventureActivity Activity { get; }

        public DateTime? Arrive
        {
            get
            {
                var waitTime = Activity.Duration;
                if (TravelTimeTo.HasValue)
                    waitTime += TravelTimeTo.Value;
                if (!_depart.HasValue)
                    return null;
                return _depart.Value - waitTime;
            }
        }

        private DateTime? _depart;
        public DateTime? Depart
        {
            get
            {
                return _depart;
            }
            set
            {
                _depart = value;
                OnPropertyChanged("Detail");
            }
        }

        public AdventureActivityModel(AdventureActivity act)
        {
            Activity = act;
            Location = act.Location;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _location;

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                OnPropertyChanged("Detail");
            }
        }

        private TimeSpan? _travelTimeTo;

        public TimeSpan? TravelTimeTo
        {
            get
            {
                return _travelTimeTo;
            }
            set
            {
                _travelTimeTo = value;
                OnPropertyChanged("Detail");
            }
        }

        public string Name
        {
            get
            {
                return Activity.Name;
            }
        }

        public string Detail
        {
            get
            {
                var travelTo = "";
                var durationPart = "";
                var arrivePart = "";
                var departPart = "";
                if (Depart.HasValue)
                {
                    arrivePart = DateString(Arrive);
                    departPart = DateString(Depart);
                    if (Arrive.Value.Date == Depart.Value.Date)
                        departPart = Depart.Value.ToString("H:mm");

                    if (TravelTimeTo.HasValue)
                        travelTo = " travel: " + DurationString(TravelTimeTo.Value);
                }

                if (Activity.Duration.Hours > 0)
                    durationPart += $"{Activity.Duration.Hours}h";
                if (Activity.Duration.Minutes > 0)
                    durationPart += $"{Activity.Duration.Minutes}m";

                return $"{Location}{travelTo} {arrivePart}-{departPart} duration: {DurationString(Activity.Duration)}";
            }
        }
        static string DurationString(TimeSpan duration)
        {
            var rv = "";
            if (duration.Hours > 0)
                rv += $"{duration.Hours}h";
            if (duration.Minutes > 0)
                rv += $"{duration.Minutes}m";
            return rv;
        }
        public static string DateString(DateTime? date)
        {
            if (date == null)
                return "-";
            if (date.Value.Year != DateTime.Today.Year)
                return date.Value.ToString("ddd M/d/yy H:mm");

            return date.Value.ToString("ddd M/d H:mm");
        }
    }
}
