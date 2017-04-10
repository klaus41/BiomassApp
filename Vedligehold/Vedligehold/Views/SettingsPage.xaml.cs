using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Services;
using Vedligehold.Services.Synchronizers;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class SettingsPage : ContentPage
    {
        ActivityIndicator ai;
        StackLayout layout;
        Color buttonColor;

        Button syncButton;
        Button dropCreateButton;
        Button deleteDbButton;
        Button checkConnectionButton;
        Button searchSettingsButton;

        Label version;

        public SettingsPage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            //MakeToolBar();
            Title = "Indstillinger";

            NavigationPage.SetHasNavigationBar(this, false);
            layout = new StackLayout { Padding = 10, };

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            dropCreateButton = new Button { Text = "Nulstil lokal database", BackgroundColor = buttonColor, TextColor = Color.White };
            syncButton = new Button { Text = "Synkroniser", BackgroundColor = buttonColor, TextColor = Color.White };
            deleteDbButton = new Button { Text = "Slet data fra lokal database", BackgroundColor = buttonColor, TextColor = Color.White };
            checkConnectionButton = new Button { Text = "Tjek forbindelse til NAV", BackgroundColor = buttonColor, TextColor = Color.White };
            searchSettingsButton = new Button { Text = "Administrer søgefilter", BackgroundColor = buttonColor, TextColor = Color.White };

            version = new Label() { Text = "Version 23.0", VerticalOptions = LayoutOptions.EndAndExpand };

            searchSettingsButton.Clicked += (s, e) =>
            {
                ShowActivityIndicator();
                Navigation.PushModalAsync(new SearchSettingsPage());
                RemoveActivityIndicator();
            };

            checkConnectionButton.Clicked += async (s, e) =>
                        {
                            ShowActivityIndicator();
                            MaintenanceTaskSynchronizer sync = new MaintenanceTaskSynchronizer();
                            bool connected = await sync.HasConnectionToNAV();
                            RemoveActivityIndicator();
                            if (connected)
                            {
                                await DisplayAlert("Forbindelse", "Enheden har forbindelse til NAV", "OK");
                            }
                            else
                            {
                                await DisplayAlert("Forbindelse", "Enheden har ikke forbindelse til NAV. Tjek om telefonen er tilsluttet WiFi eller on data er slået fra", "OK");
                            }
                        };

            deleteDbButton.Clicked += async (s, e) =>
            {
                var action = await DisplayAlert("Advarsel", "Er du sikker på, du vil slette den lokale database?", "Ja", "Cancel");
                if (action)
                {
                    ShowActivityIndicator();
                    MaintenanceTaskSynchronizer sync = new MaintenanceTaskSynchronizer();
                    sync.DeleteDB();
                    RemoveActivityIndicator();
                    await DisplayAlert("Nulstilling", "Den lokale database er nu nulstillet", "OK");

                }
            };

            dropCreateButton.Clicked += async (s, e) =>
            {
                var action = await DisplayAlert("Advarsel", "Er du sikker på, du vil slette den lokale database og erstatte den med data fra NAV?", "Ja", "Cancel");
                if (action)
                {
                    ShowActivityIndicator();

                    try
                    {
                        MaintenanceTaskSynchronizer sync = new MaintenanceTaskSynchronizer();
                        TimeRegistrationSynchronizer timesync = new TimeRegistrationSynchronizer();
                        MaintenanceActivitySynchronizer actSync = new MaintenanceActivitySynchronizer();
                        var data = await sync.DeleteAndPopulateDb();
                        timesync.DeleteAndPopulateDb();
                        actSync.DeleteAndPopulateDb();
                        RemoveActivityIndicator();
                        await DisplayAlert("Synkronisering", "Du har nu erstattet " + data[0].ToString() + " lokale opgaver med " + data[1] + " opgaver fra Navision", "OK");
                    }
                    catch
                    {
                        RemoveActivityIndicator();
                        await DisplayAlert("Advarsel!", "Database nulstillet.Der kunne ikke hentes data fra NAV!", "OK");
                    }
                }


            };
            syncButton.Clicked += async (s, e) =>
            {
                ShowActivityIndicator();
                try
                {
                    MaintenanceTaskSynchronizer sync = new MaintenanceTaskSynchronizer();
                    TimeRegistrationSynchronizer timesync = new TimeRegistrationSynchronizer();
                    MaintenanceActivitySynchronizer actSync = new MaintenanceActivitySynchronizer();
                    await sync.SyncDatabaseWithNAV();
                    await timesync.SyncDatabaseWithNAV();
                    await actSync.SyncDatabaseWithNAV();

                    RemoveActivityIndicator();
                    await DisplayAlert("Synkronisering", "Enheden er nu synkroniseret med NAV", "OK");
                }
                catch
                {

                    RemoveActivityIndicator();
                    await DisplayAlert("Advarsel!", "Enheden kunne ikke synkronisere med NAV", "OK");
                }
            };

            layout.Children.Add(dropCreateButton);
            layout.Children.Add(syncButton);
            layout.Children.Add(deleteDbButton);
            layout.Children.Add(checkConnectionButton);
            layout.Children.Add(searchSettingsButton);
            layout.Children.Add(version);

            Content = new ScrollView { Content = layout };

        }
        private void RemoveActivityIndicator()
        {
            layout.Children.Remove(ai);
            syncButton.IsEnabled = true;
            dropCreateButton.IsEnabled = true;
            deleteDbButton.IsEnabled = true;
            checkConnectionButton.IsEnabled = true;
            searchSettingsButton.IsEnabled = true;
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
            deleteDbButton.IsEnabled = false;
            checkConnectionButton.IsEnabled = false;
            searchSettingsButton.IsEnabled = false;
        }
    }
}
