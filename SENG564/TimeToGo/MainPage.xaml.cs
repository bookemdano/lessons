using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace TimeToGo;



public partial class MainPage : ContentPage
{
    internal ObservableCollection<AdventureActivityModel> Activities = new ObservableCollection<AdventureActivityModel>();

    public MainPage()
    {
        InitializeComponent();
        lst.ItemsSource = Activities;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        Display();
    }
    private async void Display()
    { 
        var adv = Persister.Read();
        staTitle.Text = $"Adventure: {adv.Title} Deadline: {adv.Deadline.ToString("ddd M/d H:mm")}";
        var haveAdventure = !string.IsNullOrWhiteSpace(adv.Title);

        btnNew.IsEnabled = haveAdventure;
        btnEdit.IsEnabled = haveAdventure;
        btnMoveUp.IsEnabled = haveAdventure;
        btnMoveDown.IsEnabled = haveAdventure;
        btnDelete.IsEnabled = haveAdventure;
        var lastStart = adv.Deadline;
        Activities.Clear();
        //var models = new ObservableCollection<AdventureActivityModel>();
        foreach (var act in adv.Activities)
        {
            var end = lastStart;
            var start = end - act.Duration;
            Activities.Add(new AdventureActivityModel(act, start, end));
            lastStart = start;
        }
        var prevLocation = Activities.Last().Location;
        foreach(var aam in Activities.Reverse())
        {
            if (string.IsNullOrWhiteSpace(aam.Location))
                aam.Location = ">" + prevLocation;
            else if (aam.Location != prevLocation)
            {
                var loc = await Calculator.Geocode(aam.Location);
                var prevLoc = await Calculator.Geocode(prevLocation);
                aam.TravelTimeTo = TimeSpan.FromMinutes(Math.Abs(loc.Latitude - prevLoc.Latitude));
                prevLocation = aam.Location;
            }
        }
        lst.SelectedItem = null;
        Error(null);
    }

    private async void New_Clicked(object sender, EventArgs e)
    {
        var pg = new NewActivityPage();
        await Navigation.PushModalAsync(pg);
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        var act = SelectedActivity();
        if (act == null)
        {
            Error("Select activity before editing!");
            return;
        }
        var pg = new NewActivityPage(act.Id);
        await Navigation.PushModalAsync(pg);
    }

    private void MoveUp_Clicked(object sender, EventArgs e)
    {
        var act = SelectedActivity();
        if (!Persister.CanMove(act, -1))
        {
            Error("Can't move selected activity up!");
            return;
        }
        Persister.Move(act, -1);
        Display();
    }

    private void MoveDown_Clicked(object sender, EventArgs e)
    {
        var act = SelectedActivity();
        if (!Persister.CanMove(act, 1))
        {
            Error("Can't move selected activity down!");
            return;
        }
        Persister.Move(act, 1);
        Display();
    }
    void Error(string msg)
    {
        sta.Text = msg;
    }
    private void Delete_Clicked(object sender, EventArgs e)
    {
        var act = SelectedActivity();
        if (act == null)
        { 
            Error("Select activity before deleting!");
            return;
        }
        Persister.DeleteActivity(act);
        Display();
    }
    private void lst_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        Error(null);
    }

    private void Fake_Clicked(object sender, EventArgs e)
    {
        var adv = Adventure.Fake();
        for(int i = 0; i < 7; i++)
            adv.Activities.Add(adv.FakeActivity());
        Persister.Write(adv);
        Display();
    }

    private async void NewAdventure_Clicked(object sender, EventArgs e)
    {
        var pg = new NewAdventurePage();
        await Navigation.PushModalAsync(pg);
    }

    private AdventureActivity SelectedActivity()
    {
        var model = lst.SelectedItem as AdventureActivityModel;
        if (model == null)
            return null;
        return model.Activity;

    }
}
