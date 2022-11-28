using TimeToGo.Helpers;
using TimeToGo.Models;

namespace TimeToGo;

public partial class NewAdventurePage : ContentPage
{
	public NewAdventurePage()
	{
		InitializeComponent();
        // defaults
        datDeadline.Date = DateTime.Today.AddDays(1);
        timDeadline.Time = new TimeSpan(15, 0, 0);
	}

    private async void Save_Clicked(object sender, EventArgs e)
    {
        // TODO Show confirmation- this will clear your current adventure
        var adv = new Adventure();
        adv.Title = entName.Text;
        adv.Deadline = datDeadline.Date.Add(timDeadline.Time);
        Persister.Write(adv);
        await Navigation.PopModalAsync();
    }
    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        // TODO Show confirmation
        await Navigation.PopModalAsync();
    }
}