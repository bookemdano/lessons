namespace TimeToGo;

public partial class NewStopPage : ContentPage
{
    readonly string _id = null;
    public NewStopPage(string id = null)
	{   
        _id = id;
		InitializeComponent();
    }
    
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entName.Text))
        {
            Error("Name is required");
            return;
        }

        var act = new AdventureActivity(_id);
        act.Name = entName.Text;
        act.Location = entLocation.Text;
        if (radDuration.IsChecked)
            act.Duration = tim.Time;
        else if (radStart.IsChecked)
            act.Start = dat.Date.Add(tim.Time);
        else if (radEnd.IsChecked)
            act.End = dat.Date.Add(tim.Time);
        Persister.AddOrUpdate(act);
        await Navigation.PopModalAsync();
        // TODO Rearrange activities
        // TODO Show activities in list
        // TODO Calc travel times
        // TODO CRUD activities
        // TODO custom timesspan
        // TODO Run on Android
        // TODO edit activity
    }

    void Error(string message)
    {
        staError.Text = message;
        staError.IsVisible = !string.IsNullOrWhiteSpace(message);
    }
    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var btn = sender as RadioButton;
        if (staTime != null)
            staTime.Text = btn.Content as string;

        if (dat != null)
            dat.IsVisible = !radDuration.IsChecked;
    }

    private void Fake_Clicked(object sender, EventArgs e)
    {
        entName.Text = "Fun Activity #" + (Persister.Read().Activities.Count() + 1).ToString();
        entLocation.Text = "Third star from the left";
        radDuration.IsChecked = true;
        var rnd = new System.Random();
        tim.Time = TimeSpan.FromSeconds(rnd.Next(86400));
        Error(null);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (_id == null)
            return;
        staTitle.Text = "Edit activity";
        var act = Persister.ReadActivity(_id);
        if (act == null)
            return;
        entName.Text = act.Name;
        entLocation.Text = act.Location;
        if (act.Duration != null)
        {
            radDuration.IsChecked = true;
            tim.Time = act.Duration.Value;
        }
        else if (act.Start != null)
        {
            radStart.IsChecked = true;
            dat.Date = act.Start.Value.Date;
            tim.Time = act.Start.Value.TimeOfDay;
        }
        else if (act.End != null)
        {
            radEnd.IsChecked = true;
            dat.Date = act.End.Value.Date;
            tim.Time = act.End.Value.TimeOfDay;
        }
        if (dat != null)
            dat.IsVisible = !radDuration.IsChecked;
    }
}