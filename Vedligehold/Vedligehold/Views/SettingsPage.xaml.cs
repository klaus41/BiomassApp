using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class SettingsPage : ContentPage
    {
        ActivityIndicator ai;
        StackLayout layout;
        Color buttonColor;
        bool loading;

        Button syncButton;
        Button dropCreateButton;
        public SettingsPage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            NavigationPage.SetHasNavigationBar(this, false);
            layout = new StackLayout { Padding = 10, };

            dropCreateButton = new Button { Text = "Nulstil lokal database", BackgroundColor = buttonColor, TextColor = Color.White };
            syncButton = new Button { Text = "Synkroniser", BackgroundColor = buttonColor, TextColor = Color.White };

            dropCreateButton.Clicked += async (s, e) =>
            { 
                var action = await DisplayAlert("Advarsel", "Er du sikker på, du vil slette den lokale database og erstatte den med data fra NAV?", "Ja", "Cancel");
                if (action)
                {
                    ShowActivityIndicator();
                    MaintenanceTaskSynchronizer sync = new MaintenanceTaskSynchronizer();
                    var data = await sync.DeleteAndPopulateDb();
                    await DisplayAlert("Synkronisering", "Du har nu erstattet " + data[0].ToString() + " lokale opgaver med " + data[1] + " opgaver fra Navision", "OK");
                    RemoveActivityIndicator();
                }


            };
            syncButton.Clicked += async (s, e) =>
            {
                ShowActivityIndicator();
                MaintenanceTaskSynchronizer sync = new MaintenanceTaskSynchronizer();
                var data = await sync.SyncDatabaseWithNAV();
                await DisplayAlert("Synkronisering", "Der er blevet synkroniseret " + data[0].ToString() + " opgaver. Der opstod " + data[1].ToString() + " konflikter. Der blev hentet " + data[2].ToString() + " nye opgaver", "OK");
                RemoveActivityIndicator();
            };

            layout.Children.Add(dropCreateButton);
            layout.Children.Add(syncButton);

            Content = new ScrollView { Content = layout };

        }
        private void RemoveActivityIndicator()
        {

            loading = false;
            layout.Children.Remove(ai);
            syncButton.IsEnabled = true;
            dropCreateButton.IsEnabled = true;
        }

        private void ShowActivityIndicator()
        {
            ai = new ActivityIndicator()
            {
                IsRunning = true
            };
            layout.Children.Add(ai);

            syncButton.IsEnabled = false;
            dropCreateButton.IsEnabled = false;
        }
    }
}
