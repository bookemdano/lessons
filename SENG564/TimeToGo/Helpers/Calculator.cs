using System.Text.Json;

namespace TimeToGo.Helpers
{
    /// <summary>
    /// Use Bing Maps API to get geocodes and travel times
    /// </summary>
    static internal class Calculator
    {
        // cache during app lifetime so we don't have to keep harassing the server
        static Dictionary<string, Location> _locations = new Dictionary<string, Location>();
        static Dictionary<string, TimeSpan> _travelTimes = new Dictionary<string, TimeSpan>();

        // Pass in a place, like "Seattle" or "WVU" or "BWI" and get back a lat lng, from Bing Maps
        public static async Task<Location> Geocode(string query)
        {
            if (!_locations.ContainsKey(query))
            {

                var url = $"http://dev.virtualearth.net/REST/v1/Locations/{query}?maxResults=1&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO";
                var str = await Communicator.GetStringAsync(url);
                if (!string.IsNullOrWhiteSpace(str))
                {
                    var options = new JsonSerializerOptions();
                    options.PropertyNameCaseInsensitive = true;
                    var json = JsonSerializer.Deserialize<GeoPackage>(str, options);
                    _locations[query] = json.GetLatLng();
                }
                else
                    _locations[query] = null;
            }
            return _locations[query];
        }

        // pass in two lat/lngs and get travel time between them from Bing Maps
        public static async Task<TimeSpan?> TravelTime(Location from, Location to)
        {
            if (from == null || to == null)
                return null;

            var key = LocationString(from) + "," + LocationString(to);
            if (!_travelTimes.ContainsKey(key))
            {
                //https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?origins=47.6044,-122.3345;47.6731,-122.1185;47.6149,-122.1936&destinations=45.5347,-122.6231;47.4747,-122.2057&travelMode=driving&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO
                var url = $"https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?origins={LocationString(from)}&destinations={LocationString(to)}&travelMode=driving&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO";
                var str = await Communicator.GetStringAsync(url);
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                var json = JsonSerializer.Deserialize<GeoPackage>(str, options);
                _travelTimes[key] = json.GetTravelDuration();
            }
            return _travelTimes[key];
        }

        // format lat/lng nicely from location
        static string LocationString(Location loc)
        {
            return $"{loc.Latitude},{loc.Longitude}";
        }
    }

 
}
