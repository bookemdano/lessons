namespace TimeToGo.Helpers
{
    // classes required by Bing to parse JSON results
    internal class GeoPackage
    {
        public List<GeoResourceSet> ResourceSets { get; set; }
        
        // Get the good stuff from the results
        internal Location GetLatLng()
        {
            var lat = ResourceSets[0].Resources[0].Point.Coordinates[0];
            var lng = ResourceSets[0].Resources[0].Point.Coordinates[1];
            return new Location(lat, lng);
        }
        
        internal TimeSpan GetTravelDuration()
        {
            return TimeSpan.FromMinutes(ResourceSets[0].Resources[0].Results[0].TravelDuration);
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
        public List<GeoResult> Results { get; set; }
    }
    internal class GeoPoint
    {
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }
    internal class GeoResult
    {
        public double TravelDistance { get; set; }
        public double TravelDuration { get; set; }
    }
}
