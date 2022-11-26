namespace TimeToGo;

public partial class NewStopPage : ContentPage
{
	public NewStopPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopModalAsync();
    }
}