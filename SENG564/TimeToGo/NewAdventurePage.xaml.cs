namespace TimeToGo;

public partial class NewAdventurePage : ContentPage
{
	public NewAdventurePage()
	{
		InitializeComponent();
        datDeadline.Date = DateTime.Today.AddDays(3);
        timDeadline.Time = new TimeSpan(15, 0, 0);
	}

    private async void Save_Clicked(object sender, EventArgs e)
    {
        var adv = new Adventure();
        adv.Title = entName.Text;
        adv.Deadline = datDeadline.Date.Add(timDeadline.Time);
        Persister.Write(adv);
        await Navigation.PopModalAsync();
    }
    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

}