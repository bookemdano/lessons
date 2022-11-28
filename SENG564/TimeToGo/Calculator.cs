using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TimeToGo
{
    static internal class Calculator
    {
        static Dictionary<string, Location> _locations = new Dictionary<string, Location>();
        
        //https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?origins=47.6044,-122.3345;47.6731,-122.1185;47.6149,-122.1936&destinations=45.5347,-122.6231;47.4747,-122.2057&travelMode=driving&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO
        //https://dev.virtualearth.net/REST/v1/Routes/DistanceMatrix?origins=47.6044,-122.3345;47.6731,-122.1185;47.6149,-122.1936&destinations=45.5347,-122.6231;47.4747,-122.2057&travelMode=driving&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO
        public static async Task<Location> Geocode(string query)
        {
            if (!_locations.ContainsKey(query))
            {

                var url = $"http://dev.virtualearth.net/REST/v1/Locations/{query}?maxResults=1&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO";
                var comm = new Communicator();
                var str = await comm.GetStringAsync(url);
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                var json = JsonSerializer.Deserialize<GeocodeResult>(str, options);
                _locations[query] = json.GetLatLng();
            }
            return _locations[query];
        }

    }
    public class Communicator
    {
        HttpClient _client;
        JsonSerializerOptions _serializerOptions;

        //public List<TodoItem> Items { get; private set; }

        public Communicator()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<string> GetStringAsync(string url)
        {

            // var url = "http://dev.virtualearth.net/REST/v1/Locations/Seattle?maxResults=1&key=Ap-3kVj6sZXi76ogqzkYm7YiCewvoch5nSAIf9HyZwAsbNizzB4ymxF7ZtDjoqVO";
            Uri uri = new Uri(url);
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetStringAsync", ex);
            }

            return null;
        }
    }

    internal class GeocodeResult
    {
        //public string AuthenticationResultCode { get; set; }
        //public string BrandLogoUri { get; set; }
        public List<GeoResourceSet> ResourceSets { get; set; }
        internal Location GetLatLng()
        {
            var lat = ResourceSets[0].Resources[0].Point.Coordinates[0];
            var lng = ResourceSets[0].Resources[0].Point.Coordinates[1];
            return new Location(lat, lng);
        }
    }
    internal class GeoResourceSet
    {
        public int EstimatedTotal { get; set; }
        public List<GeoResource> Resources { get; set; }
    }
    internal class GeoResource
    {
        public string Name { get; set; }
        public GeoPoint Point { get; set; }
    }
    internal class GeoPoint
    {
        public string Type { get; set; }
        public List<double> Coordinates{ get; set; }
    }
}
