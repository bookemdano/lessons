using Microsoft.Extensions.Options;
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


    internal ObservableCollection<AdventureActivity> Activities = new ObservableCollection<AdventureActivity>();

    public MainPage()
    {
        InitializeComponent();
        lst.ItemsSource = Activities;
    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        Display();
    }
    private void Display()
    { 
        var adv = Persister.Read();
        Activities.Clear();
        foreach (var act in adv.Activities)
            Activities.Add(act);
        lst.SelectedItem = null;
        Error(null);
    }

    private async void Cell_Tapped(object sender, EventArgs e)
    {
        var cell = sender as TextCell;
        var act = cell.BindingContext as AdventureActivity;
        var pg = new NewStopPage(act.Id);
        await Navigation.PushModalAsync(pg);
    }

    private async void New_Clicked(object sender, EventArgs e)
    {
        var pg = new NewStopPage();
        await Navigation.PushModalAsync(pg);
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        var act = lst.SelectedItem as AdventureActivity;
        if (act == null)
        {
            Error("Select activity before editing!");
            return;
        }
        var pg = new NewStopPage(act.Id);
        await Navigation.PushModalAsync(pg);
    }

    private void MoveUp_Clicked(object sender, EventArgs e)
    {
        var act = lst.SelectedItem as AdventureActivity;
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
        var act = lst.SelectedItem as AdventureActivity;
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
        var act = lst.SelectedItem as AdventureActivity;
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
}
