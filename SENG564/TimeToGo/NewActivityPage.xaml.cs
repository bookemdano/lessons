using TimeToGo.Helpers;
using TimeToGo.Models;

namespace TimeToGo;

/// <summary>
/// Create or edit an activity
/// </summary>
public partial class NewActivityPage : ContentPage
{
    // id of activity you are editing
    readonly string _id = null;

    // pass an ID to edit an existing activity
    public NewActivityPage(string id = null)
    {
        _id = id;
        InitializeComponent();
	}

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (_id == null)
            return;
        
        staTitle.Text = "Edit activity";
        var act = Persister.ReadActivity(_id);
        LoadPage(act);
    }
    
    // push activity to UI
    void LoadPage(AdventureActivity act)
    {
        if (act == null)
            return;
        entName.Text = act.Name;
        entLocation.Text = act.Location;
        entDuration.Text = AdventureActivityModel.DurationString(act.Duration);
    }

    private async void Done_Clicked(object sender, EventArgs e)
    {
        // validate
        if (string.IsNullOrWhiteSpace(entName.Text))
        {
            Error("Name is required");
            return;
        }

        // grab users input
        var act = new AdventureActivity(_id);
        act.Name = entName.Text;
        act.Location = entLocation.Text;
        act.Duration = AdventureActivityModel.FromDurationString(entDuration.Text);
        // persist
        Persister.AddOrUpdateActivity(act);

        // return to main page
        await Navigation.PopModalAsync();
    }
    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        // TODO show confirmation
        await Navigation.PopModalAsync();
    }

    // can be connected to button to auto-fill screen
    private void Fake_Clicked(object sender, EventArgs e)
    {
        var adv = Persister.Read();
        LoadPage(adv.FakeActivity());
        Error(null);
    }

    // Show users errors, not system errors
    void Error(string message)
    {
        staError.Text = message;
        staError.IsVisible = !string.IsNullOrWhiteSpace(message);
    }
}