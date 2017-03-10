using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class HomePage : ContentPage
    {
        Contact[] contacts;
        Statistic[] stats;
        string[] contactNumbers;
        bool loading;
        bool syncing;
        StackLayout layout;
        ActivityIndicator ai;
        Button logOutButton;
        Button statButton;
        Button taskButton;
        Button settingsButton;

        Color buttonColor;

        public HomePage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            Title = "Hjem";

            NavigationPage.SetHasNavigationBar(this, false);
            //MakeToolBar();
         
            layout = new StackLayout { Padding = 10, };

            logOutButton = new Button { Text = "Log ud", BackgroundColor = buttonColor, TextColor = Color.White };
            statButton = new Button { Text = "Leverandørstatistikker", BackgroundColor = buttonColor, TextColor = Color.White };
            taskButton = new Button { Text = "Vedligeholdsopgaver", BackgroundColor = buttonColor, TextColor = Color.White };
            settingsButton = new Button { Text = "Indstillinger", BackgroundColor = buttonColor, TextColor = Color.White };

            logOutButton.Clicked += (s, e) =>
            {
                Application.Current.MainPage.Navigation.PopAsync();
                //Navigation.PushAsync(new LoginPage());
            };
            statButton.Clicked += async (s, e) =>
            {
                string data = null;
                try
                {
                        PDFService pds = new PDFService();
                        data = await pds.GetPDF("A00005");
                        StatButtonMethod();
                }
                catch
                {
                    await DisplayAlert("Forbindelse", "Enheden har ingen forbindelse til NAV", "OK");
                }
            };
            taskButton.Clicked += async (s, e) =>
            {
                syncing = true;
                loading = true;
                while (loading)
                {
                    ShowActivityIndicator();

                    await Navigation.PushAsync(new MaintenancePage());
                    RemoveActivityIndicator();
                }
            };
            settingsButton.Clicked += (s, e) =>
            {
                Navigation.PushAsync(new SettingsPage());
            };


            Image image = new Image();

            image.Source = "eistor.png";
            image.Opacity = 0.7;
            image.VerticalOptions = LayoutOptions.End;

            layout.Children.Add(statButton);
            layout.Children.Add(taskButton);
            layout.Children.Add(settingsButton);
            layout.Children.Add(logOutButton);
            layout.Children.Add(image);

            Content = new ScrollView { Content = layout };
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
                    StatButtonMethod();
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

        public async void StatButtonMethod()
        {
            loading = true;
            while (loading)
            {
                ShowActivityIndicator();
                stats = null;
                contacts = null;

                while (contacts == null)
                {
                    ActivityIndicator ai = new ActivityIndicator()
                    {
                        IsRunning = true
                    };
                    var sv = new ContactService();
                    var es = await sv.GetContactsAsync();
                    contacts = es;
                }


                contactNumbers = new string[contacts.Count()];

                for (int i = 0; i < contacts.Count(); i++)
                {
                    contactNumbers[i] = contacts[i].no + " - " + contacts[i].company_Name;
                }

                var action = await DisplayActionSheet("Vælg leverandørnummer", "Cancel", null, contactNumbers);

                Debug.WriteLine("ACTION!!!!" + action);
                try
                {
                    if (action != "Cancel")
                    {
                        int l = action.IndexOf(" ");
                        string id = action.Substring(0, l);

                        while (stats == null)
                        {
                            var sv = new StatisticService();
                            var es = await sv.GetStatsAsync(id);
                            stats = es;
                        }

                        await Navigation.PushAsync(new StatisticsPage(stats));
                    }
                }

                catch { }
                RemoveActivityIndicator();
            }
        }

        private void RemoveActivityIndicator()
        {

            loading = false;
            layout.Children.Remove(ai);
            logOutButton.IsEnabled = true;
            statButton.IsEnabled = true;
            taskButton.IsEnabled = true;
            settingsButton.IsEnabled = true;
        }

        private void ShowActivityIndicator()
        {
            ai = new ActivityIndicator()
            {
                IsRunning = true
            };
            layout.Children.Add(ai);

            logOutButton.IsEnabled = false;
            statButton.IsEnabled = false;
            taskButton.IsEnabled = false;
            settingsButton.IsEnabled = false;

        }
        protected async override void OnAppearing()
        {
            bool response = false;
            while (!response)
            {
                MaintenanceTaskSynchronizer mst = new MaintenanceTaskSynchronizer();
                response = await mst.SyncDatabaseWithNAV();
            }
        }
    }
}
