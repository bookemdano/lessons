using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeToGo;

public partial class NewActivityPage : ContentPage
{
    readonly string _id = null;

    public NewActivityPage(string id = null)
    {
        _id = id;
        InitializeComponent();
	}

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        if (_id == null)
        {
            return;
        }
        staTitle.Text = "Edit activity";
        var act = Persister.ReadActivity(_id);
        LoadPage(act);
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
        act.Duration = timDuration.Time;
        Persister.AddOrUpdate(act);
        await Navigation.PopModalAsync();
    }
    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void Fake_Clicked(object sender, EventArgs e)
    {
        var adv = Persister.Read();
        LoadPage(adv.FakeActivity());
        Error(null);
    }

    void LoadPage(AdventureActivity act)
    {
        if (act == null)
            return;
        entName.Text = act.Name;
        entLocation.Text = act.Location;
        timDuration.Time = act.Duration;
    }

    void Error(string message)
    {
        staError.Text = message;
        staError.IsVisible = !string.IsNullOrWhiteSpace(message);
    }

}