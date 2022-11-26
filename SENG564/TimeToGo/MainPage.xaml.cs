using System.Collections.ObjectModel;

namespace TimeToGo;

public partial class MainPage : ContentPage
{
	int count = 0;
	ObservableCollection<string> _items = new ObservableCollection<string>();

	public MainPage()
	{
		InitializeComponent();
		lst.ItemsSource = _items;
	}


	private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";
		_items.Add(CounterBtn.Text);
		
		SemanticScreenReader.Announce(CounterBtn.Text);
		var pg = new NewStopPage();
		await Navigation.PushModalAsync(pg);
	}
}

