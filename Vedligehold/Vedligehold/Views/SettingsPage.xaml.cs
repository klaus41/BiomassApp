using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Services;
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
        Button deleteDbButton;
        Button checkConnectionButton;
        public SettingsPage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            //MakeToolBar();
            Title = "Indstillinger";

            NavigationPage.SetHasNavigationBar(this, false);
            layout = new StackLayout { Padding = 10, };

            dropCreateButton = new Button { Text = "Nulstil lokal database", BackgroundColor = buttonColor, TextColor = Color.White };
            syncButton = new Button { Text = "Synkroniser", BackgroundColor = buttonColor, TextColor = Color.White };
            deleteDbButton = new Button { Text = "Slet data fra lokal database", BackgroundColor = buttonColor, TextColor = Color.White };
            checkConnectionButton = new Button { Text = "Tjek forbindelse til NAV", BackgroundColor = buttonColor, TextColor = Color.White };

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
                        var data = await sync.DeleteAndPopulateDb();
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
                    await sync.SyncDatabaseWithNAV();
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

            Content = new ScrollView { Content = layout };

        }
        private void RemoveActivityIndicator()
        {

            loading = false;
            layout.Children.Remove(ai);
            syncButton.IsEnabled = true;
            dropCreateButton.IsEnabled = true;
            deleteDbButton.IsEnabled = true;
            checkConnectionButton.IsEnabled = true;
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
        }
        private void MakeToolBar()
        {
            ToolbarItems.Add(new ToolbarItem("Hjem", "filter.png", async () =>
            {
                if (this.GetType() != typeof(HomePage))
                {
                    await Navigation.PushAsync(new HomePage());
                }
            }));
            ToolbarItems.Add(new ToolbarItem("Statistik", "filter.png", async () =>
            {
                string data = null;
                try
                {
                    PDFService pds = new PDFService();
                    data = await pds.GetPDF("A00005");
                    HomePage hp = new HomePage();
                    hp.StatButtonMethod();
                }
                catch
                {
                    await DisplayAlert("Forbindelse", "Enheden har ingen forbindelse til NAV", "OK");
                }
            }));

            ToolbarItems.Add(new ToolbarItem("Opgaver", "filter.png", async () =>
            {
                if (this.GetType() != typeof(MaintenancePage))
                {
                    await Navigation.PushAsync(new MaintenancePage());
                }
            }));

            ToolbarItems.Add(new ToolbarItem("Indstillinger", "filter.png", async () =>
            {
                if (this.GetType() != typeof(SettingsPage))
                {
                    await Navigation.PushAsync(new SettingsPage());
                }
            }));

        }

    }
}
