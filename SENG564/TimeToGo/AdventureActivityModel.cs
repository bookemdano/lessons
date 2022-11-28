using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeToGo
{
    internal class AdventureActivityModel : INotifyPropertyChanged
    {
        public AdventureActivity Activity { get; }

        private DateTime _arrive;
        private DateTime _depart;

        private string _location;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public AdventureActivityModel(AdventureActivity act, DateTime arrive, DateTime depart) 
        {
            Activity = act;
            _arrive = arrive;
            _depart = depart;
            Location = act.Location;
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
                var arrivePart = _arrive.ToString("ddd M/d H:mm");
                var departPart = _depart.ToString("ddd M/d H:mm");
                if (_arrive.Date == _depart.Date)
                    departPart = _depart.ToString("H:mm");
                var durationPart = "";
                if (Activity.Duration.Hours > 0)
                    durationPart += $"{Activity.Duration.Hours}h";
                if (Activity.Duration.Minutes > 0)
                    durationPart += $"{Activity.Duration.Minutes}m";
                return $"{Location} {arrivePart}-{departPart} duration: {durationPart}";
            }
        }
    }
}
