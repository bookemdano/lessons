namespace TimeToGo.Helpers
{
    static public class Communicator
    {
        // get a string from a rest service
        static public async Task<string> GetStringAsync(string url)
        {

            using var client = new HttpClient();
            Uri uri = new Uri(url);
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
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
}
