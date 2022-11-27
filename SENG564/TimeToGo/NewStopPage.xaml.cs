namespace TimeToGo;

public partial class NewStopPage : ContentPage
{
    public NewStopPage()
	{
		InitializeComponent();
    }
    
    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entName.Text))
        {
            Error("Name is required");
            return;
        }

        var act = new AdventureActivity();
        act.Name = entName.Text;
        act.Location = entLocation.Text;
        if (radDuration.IsChecked)
            act.Duration = tim.Time;
        else if (radStart.IsChecked)
            act.Start = dat.Date.Add(tim.Time);
        else if (radEnd.IsChecked)
            act.End = dat.Date.Add(tim.Time);
        await Persister.AddOrUpdate(act);
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
}