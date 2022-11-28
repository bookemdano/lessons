namespace TimeToGo.Models
{
    /// <summary>
    /// See Adventure comment
    /// </summary>
    internal class AdventureActivity
    {
        #region Properties
        //Unique ID for each activity
        public string Id { get; set; }
        // required but doesn't have to be unique
        public string Name { get; set; }
        // plain text location where the activity takes place, like "BWI", "WVU", or "The White House", optional
        public string Location { get; set; }
        // how long you are spending at this activity
        public TimeSpan Duration { get; set; }
        #endregion
        
        public AdventureActivity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public AdventureActivity(string id = null)
        {
            if (id == null)
                Id = Guid.NewGuid().ToString();
            else
                Id = id;
        }

        // used only for diagnostics, not shown to users
        public override string ToString()
        {
            return $"n:{Name} i:{Id} l:{Location} d:{Duration}";
        }
    }
}
