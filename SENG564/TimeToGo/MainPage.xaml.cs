using System.Collections.ObjectModel;
using TimeToGo.Helpers;
using TimeToGo.Models;

namespace TimeToGo;

public partial class MainPage : ContentPage
{
    // adventure activity models bound to on-screen list
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
        staTitle.Text = $"Adventure: {adv.Title} Deadline: {AdventureActivityModel.DateString(adv.Deadline, null)}";
        var haveAdventure = !string.IsNullOrWhiteSpace(adv.Title);

        // Hide buttons if you haven't created an adventure yet
        btnNew.IsEnabled = haveAdventure;
        btnEdit.IsEnabled = haveAdventure;
        btnMoveUp.IsEnabled = haveAdventure;
        btnMoveDown.IsEnabled = haveAdventure;
        btnDelete.IsEnabled = haveAdventure;
        
        // convert activities to models and calculate times
        Activities.Clear();
        if (adv.Activities.Any())
        {
            foreach (var act in adv.Activities)
                Activities.Add(new AdventureActivityModel(act, adv.Deadline));
        
            // go through backwards to calc travel times
            var prevLocation = Activities.Last().Location;
            foreach (var aam in Activities.Reverse())
            {
                if (string.IsNullOrWhiteSpace(aam.Location) && !string.IsNullOrWhiteSpace(prevLocation))
                {
                    // if there is no location, assume whatever it was before
                    aam.Location = ">" + prevLocation;
                }
                else if (aam.Location != prevLocation)
                {
                    var loc = await Calculator.Geocode(aam.Location);
                    var prevLoc = await Calculator.Geocode(prevLocation);
                    aam.TravelTimeTo = await Calculator.TravelTime(prevLoc, loc);
                    prevLocation = aam.Location;
                }
            }
            // go through forward again to calc how long each activity will take
            var lastStart = adv.Deadline;
            foreach (var aam in Activities)
            {
                aam.Depart = lastStart;
                lastStart = aam.Arrive.Value;
            }
            staStart.Text = "This adventure has to start at " + AdventureActivityModel.DateString(lastStart, null);
        }
        // clear out everything
        lst.SelectedItem = null;
        Error(null);
    }

    #region Activity Actions
    
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
        if (!Persister.CanMoveActivity(act, -1))
        {
            Error("Can't move selected activity up!");
            return;
        }
        Persister.MoveActivity(act, -1);
        Display();
    }

    private void MoveDown_Clicked(object sender, EventArgs e)
    {
        var act = SelectedActivity();
        if (!Persister.CanMoveActivity(act, 1))
        {
            Error("Can't move selected activity down!");
            return;
        }
        Persister.MoveActivity(act, 1);
        Display();
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
    #endregion

    private void lst_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // clear error when you switch items
        Error(null);
    }

    // create fake adventure- can be wired to button for testing
    private void Fake_Clicked(object sender, EventArgs e)
    {
        var adv = Adventure.Fake();
        for(int i = 0; i < 7; i++)
            adv.Activities.Add(adv.FakeActivity());
        Persister.Write(adv);
        Display();
    }

    // create whole new adventure
    private async void NewAdventure_Clicked(object sender, EventArgs e)
    {
        var pg = new NewAdventurePage();
        await Navigation.PushModalAsync(pg);
    }

    #region private helpers
    AdventureActivity SelectedActivity()
    {
        var model = lst.SelectedItem as AdventureActivityModel;
        if (model == null)
            return null;
        return model.Activity;

    }
    // show error on screen- these are user errors, not programmatic errors
    void Error(string msg)
    {
        sta.Text = msg;
        sta.IsVisible = !string.IsNullOrWhiteSpace(msg);
    }
    #endregion
}
