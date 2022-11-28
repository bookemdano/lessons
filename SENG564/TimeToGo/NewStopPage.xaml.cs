namespace TimeToGo;

public partial class NewStopPage : ContentPage
{
    readonly string _id = null;
    public NewStopPage(string id = null)
	{   
        _id = id;
		InitializeComponent();
    }
    
    private async void Done_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entName.Text))
        {
            Error("Name is required");
            return;
        }

        var act = new AdventureActivity(_id);
        act.Name = entName.Text;
        act.Location = entLocation.Text;
        act.Start = datStart.Date.Add(timStart.Time);
        if (chkDuration.IsChecked)
            act.Duration = timDuration.Time;
        if (chkStart.IsChecked)
            act.Start = datStart.Date.Add(timStart.Time);
        if (chkEnd.IsChecked)
            act.End = datEnd.Date.Add(timEnd.Time);
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
    List<CheckBox> _chks = new List<CheckBox>();
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var chk = sender as CheckBox;
        if (!chk.IsChecked)
            _chks.Remove(chk);
        else
        {
            _chks.Add(chk);
            if (_chks.Count > 2) 
            {
                var first = _chks.First();
                first.IsChecked = false;
            }
        }
        FixChecks();
    }

    private void Fake_Clicked(object sender, EventArgs e)
    {
        var adv = Persister.Read();
        LoadPage(adv.FakeActivity(ActivityTypeEnum.NA));
        Error(null);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (_id == null)
        {
            chkDuration.IsChecked = true;
            FixChecks();
            return;
        }
        staTitle.Text = "Edit activity";
        var act = Persister.ReadActivity(_id);
        LoadPage(act);
    }
    void FixChecks()
    {
        datStart.IsEnabled = chkStart.IsChecked;
        timStart.IsEnabled = chkStart.IsChecked;
        timDuration.IsEnabled = chkDuration.IsChecked;
        datEnd.IsEnabled = chkEnd.IsChecked;
        timEnd.IsEnabled = chkEnd.IsChecked;
    }
    void LoadPage(AdventureActivity act)
    {
        if (act == null)
            return;
        entName.Text = act.Name;
        entLocation.Text = act.Location;
        if (act.Duration != null)
        {
            chkDuration.IsChecked = true;
            timDuration.Time = act.Duration.Value;
        }
        else if (act.Start != null)
        {
            chkStart.IsChecked = true;
            datStart.Date = act.Start.Value.Date;
            timStart.Time = act.Start.Value.TimeOfDay;
        }
        else if (act.End != null)
        {
            chkEnd.IsChecked = true;
            datEnd.Date = act.End.Value.Date;
            timEnd.Time = act.End.Value.TimeOfDay;
        }
        FixChecks();
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}