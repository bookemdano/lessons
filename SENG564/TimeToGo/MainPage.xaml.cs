using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace TimeToGo;



public partial class MainPage : ContentPage
{
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
        staTitle.Text = $"Adventure: {adv.Title} Deadline: {adv.Deadline.ToString("g")}";
        var haveAdventure = !string.IsNullOrWhiteSpace(adv.Title);

        btnNew.IsEnabled = haveAdventure;
        btnEdit.IsEnabled = haveAdventure;
        btnMoveUp.IsEnabled = haveAdventure;
        btnMoveDown.IsEnabled = haveAdventure;
        btnDelete.IsEnabled = haveAdventure;
        Activities.Clear();
        var lastEnd = adv.Start();
        foreach (var act in adv.Activities)
        {
            Activities.Add(act);
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
        var act = lst.SelectedItem as AdventureActivity;
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

    private void Fake_Clicked(object sender, EventArgs e)
    {
        var adv = new Adventure();
        adv.Activities.Add(adv.FakeActivity());
        adv.Activities.Add(adv.FakeActivity());
        adv.Activities.Add(adv.FakeActivity());
        adv.Activities.Add(adv.FakeActivity());
        adv.Activities.Add(adv.FakeActivity());
        Persister.Write(adv);
        Display();
    }

    private async void NewAdventure_Clicked(object sender, EventArgs e)
    {
        var pg = new NewAdventurePage();
        await Navigation.PushModalAsync(pg);
    }
}
