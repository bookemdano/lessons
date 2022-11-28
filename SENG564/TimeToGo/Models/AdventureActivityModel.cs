using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeToGo.Models
{
    // Model used when displaying AdventureActivities
    internal class AdventureActivityModel : INotifyPropertyChanged
    {
        public AdventureActivityModel(AdventureActivity act, DateTime deadline)
        {
            Activity = act;
            Location = act.Location;
            _deadline = deadline;
        }

        // required to notify view
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties
        public AdventureActivity Activity { get; }

        // Arrive time is calculated based on depart and duration
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

        // adventure deadline
        private DateTime _deadline;

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

        // concat detail string based on relavent parameters, try to keep short
        public string Detail
        {
            get
            {
                var travelTo = "";
                var arrivePart = "";
                var departPart = "";
                if (Depart.HasValue)
                {
                    arrivePart = DateString(Arrive, _deadline);
                    departPart = DateString(Depart, Arrive);
                    
                    // show travel time if there is any
                    if (TravelTimeTo.HasValue)
                        travelTo = " trav: " + DurationString(TravelTimeTo.Value);
                }

                return $"{Location}{travelTo} {arrivePart}-{departPart} dur: {DurationString(Activity.Duration)}";
            }
        }
        #endregion

        #region Static Helpers
        // short duration- don't show hours or minutes if they are zero
        public static string DurationString(TimeSpan duration)
        {
            var rv = "";
            if (duration.Days > 0)
                rv += $"{duration.Days}d";
            if (duration.Hours > 0)
                rv += $"{duration.Hours}h";
            if (duration.Minutes > 0)
                rv += $"{duration.Minutes}m";
            return rv;
        }
        internal static TimeSpan FromDurationString(string text)
        {
            var rv = new TimeSpan();
            text = text.Replace(" ", "");
            var parts = text.Split('d');
            if (parts.Count() > 1)
            {
                int.TryParse(parts[0], out int days);
                rv = rv.Add(TimeSpan.FromDays(days));
                text = parts[1];
            }
            parts = text.Split('h');
            if (parts.Count() > 1)
            {
                int.TryParse(parts[0], out int hours);
                rv = rv.Add(TimeSpan.FromHours(hours));
                text = parts[1];
            }
            parts = text.Split('m');
            if (parts.Count() > 1)
            {
                int.TryParse(parts[0], out int minutes);
                rv = rv.Add(TimeSpan.FromMinutes(minutes));
            }
            return rv;
        }

        // concise date and time, don't show year if it is this year
        public static string DateString(DateTime? date, DateTime? other)
        {
            if (date == null)
                return "-";
            if (other.HasValue && date.Value.Date == other.Value.Date)
                return date.Value.ToString("H:mm");
            if (date.Value.Year != DateTime.Today.Year)
                return date.Value.ToString("ddd M/d/yy H:mm");

            return date.Value.ToString("ddd M/d H:mm");
        }

        #endregion
    }
}
