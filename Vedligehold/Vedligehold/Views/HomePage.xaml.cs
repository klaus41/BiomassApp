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
        Label tasks;
        List<MaintenanceTask> taskList;

        Color buttonColor;

        public HomePage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            Title = "Hjem";
            GlobalData gd = GlobalData.GetInstance;
            var db = App.Database;
            Label user = new Label() { Text = "Du er logget ind som: " + gd.User, FontSize = 20, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            tasks = new Label() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            Grid mainGrid = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto)},
                    new RowDefinition { Height = GridLength.Auto},
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                }
            };
            NavigationPage.SetHasNavigationBar(this, false);
            //MakeToolBar();

            layout = new StackLayout { Padding = 10, };

            logOutButton = new Button { Text = "Log ud", BackgroundColor = buttonColor, TextColor = Color.White };
            logOutButton.Clicked += (s, e) =>
             {
                 Application.Current.MainPage.Navigation.PopAsync();
                 //Navigation.PushAsync(new LoginPage());
             };

            Image image = new Image();

            image.Source = "eistor.png";
            image.Opacity = 0.7;
            image.VerticalOptions = LayoutOptions.End;
            image.HorizontalOptions = LayoutOptions.End;

            layout.Children.Add(logOutButton);
            layout.Children.Add(image);

            mainGrid.Children.Add(user, 0, 0);
            mainGrid.Children.Add(tasks, 0, 1);
            mainGrid.Children.Add(image, 0, 2);
            mainGrid.Children.Add(logOutButton, 0, 5);


            Content = new ScrollView { Content = mainGrid };
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

                        await Navigation.PushModalAsync(new StatisticsPage(stats));
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
        }

        private void ShowActivityIndicator()
        {
            ai = new ActivityIndicator()
            {
                IsRunning = true
            };
            layout.Children.Add(ai);

            logOutButton.IsEnabled = false;

        }
        protected async override void OnAppearing()
        {
            taskList = null;
            int notdone = 0;
            while (taskList == null)
            {
                taskList = await App.Database.GetTasksAsync();
            }
            foreach (var item in taskList)
            {
                if (!item.done)
                {
                    notdone++;
                }
            }
            tasks.Text = "Du har " + notdone + " ufærdige opgaver, der venter.";
        }
    }
}
