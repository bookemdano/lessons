using System.Collections.ObjectModel;

namespace TimeToGo;



public partial class MainPage : ContentPage
{
    internal class ActivityCell : Cell
    {
        private AdventureActivity _act;

        internal ActivityCell(AdventureActivity act)
        {
            _act = act;
        }

        internal string Name
        {
            get
            {
                return _act.Name;
            }
        }
        internal string Location
        {
            get
            {
                return _act.Location;
            }
        }
    }

    int count = 0;
	
	internal ObservableCollection<AdventureActivity> Activities = new ObservableCollection<AdventureActivity>();

	public MainPage()
	{
		InitializeComponent();
		lst.ItemsSource = Activities;
	}


	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";
		
		
		SemanticScreenReader.Announce(CounterBtn.Text);
		var pg = new NewStopPage();
		await Navigation.PushModalAsync(pg);
	}

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        var adv = Persister.Read();
        Activities.Clear();
        foreach (var act in adv.Activities)
            Activities.Add(act);

    }

    private async void Cell_Tapped(object sender, EventArgs e)
    {
        var cell = sender as TextCell;
        var act = cell.BindingContext as AdventureActivity;
        var pg = new NewStopPage(act.Id);
        await Navigation.PushModalAsync(pg);
    }
}
