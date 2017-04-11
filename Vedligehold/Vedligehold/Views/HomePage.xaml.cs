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
        Label searchCriteriaHeader;
        Label timeRegisteredIn;
        Label timeRegisteredOut;
        Label userInfo;
        Label searchTimeFrom;
        Label searchTimeTo;
        Label searchUser;

        Image image;

        List<MaintenanceTask> taskList;
        GlobalData gd = GlobalData.GetInstance;
        Color buttonColor;

        Grid grid;
        Grid gridCriteria;

        public HomePage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            Title = "Hjem";


            NavigationPage.SetHasNavigationBar(this, false);
            //MakeToolBar();
            MakeGrid();
            MakeGridCriteria();
            HandleImage();

            logOutButton = new Button { Text = "Log ud", BackgroundColor = buttonColor, TextColor = Color.White, VerticalOptions = LayoutOptions.EndAndExpand };
            logOutButton.Clicked += LogOutButton_Clicked;

            layout = new StackLayout { Padding = 10, };

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            layout.Children.Add(grid);
            layout.Children.Add(gridCriteria);
            layout.Children.Add(image);
            layout.Children.Add(logOutButton);

            Content = layout;
        }

        private void HandleImage()
        {
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                var answer = await DisplayAlert("Support", "Vil du ringe til EliteIT for support?", "Ja", "Nej");
                if (answer)
                {
                    Device.OpenUri(new Uri("tel:+4588168810"));
                }
            };

            image = new Image();

            image.Source = "eistor.png";
            image.Opacity = 0.7;
            image.VerticalOptions = LayoutOptions.Start;
            image.Scale = 0.7;
            image.GestureRecognizers.Add(tapGestureRecognizer);
        }

        private void MakeGridCriteria()
        {
            searchCriteriaHeader = new Label() { Text = "Søgekriterier", HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = BackgroundColor };
            tasks = new Label() { TextColor = BackgroundColor, HorizontalOptions = LayoutOptions.Center };
            searchTimeFrom = new Label() { Text = "Tid fra", TextColor = BackgroundColor };
            searchTimeTo = new Label() { Text = "Tid til", TextColor = BackgroundColor };
            searchUser = new Label() { Text = "Bruger", TextColor = BackgroundColor };

            gridCriteria = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                },
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = buttonColor
            };

            gridCriteria.Children.Add(searchCriteriaHeader, 0, 0);
            Grid.SetColumnSpan(searchCriteriaHeader, 2);
            gridCriteria.Children.Add(searchUser, 0, 1);
            gridCriteria.Children.Add(tasks, 0, 3);
            Grid.SetColumnSpan(tasks, 2);
            gridCriteria.Children.Add(searchTimeFrom, 1, 1);
            gridCriteria.Children.Add(searchTimeTo, 1, 2);

        }
        private void MakeGrid()
        {
            userInfo = new Label() { Text = "Velkommen " + gd.User.Name, HorizontalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = BackgroundColor };
            timeRegisteredIn = new Label() { Text = "Du er ikke mødt ind endnu", TextColor = BackgroundColor };
            timeRegisteredOut = new Label() { Text = "Du er ikke meldt ud endnu", TextColor = BackgroundColor };

            grid = new Grid
            {
                Padding = new Thickness(10),
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1,GridUnitType.Star) }
                },
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = buttonColor
            };

            grid.Children.Add(userInfo, 0, 0);
            Grid.SetColumnSpan(userInfo, 2);
            grid.Children.Add(timeRegisteredIn, 0, 1);
            grid.Children.Add(timeRegisteredOut, 1, 1);

        }

        private void LogOutButton_Clicked(object sender, EventArgs e)
        {
            List<Page> pl = new List<Page>();
            foreach (var item in gd.TabbedPage.Children)
            {
                pl.Add(item);
            }
            foreach (var item in pl)
            {
                gd.TabbedPage.Children.Remove(item);
            }
            gd.TabbedPage.Children.Add(gd.LoginPage);
            gd.IsLoggedIn = false;
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
            gd.Sync();
            TimeRegistrationPage timepage = new TimeRegistrationPage();
            timepage.GetData();

            taskList = null;
            int notdone = 0;
            while (taskList == null)
            {
                taskList = await App.Database.GetTasksAsync();
            }
            foreach (var item in taskList)
            {
                if (item.status == "Released" && item.responsible == gd.User.Code && item.planned_Date <= gd.SearchDateTimeLast && item.planned_Date >= gd.SearchDateTime)
                {
                    notdone++;
                }
            }
            if (notdone == 1)
            {
                tasks.Text = "Du har " + notdone + " frigivet opgave.";
            }
            else
            {
                tasks.Text = "Du har " + notdone + " frigivede opgaver.";
            }
            if (gd.TimeRegisteredIn != null)
            {
                timeRegisteredIn.Text = "Du er mødt ind: " + gd.TimeRegisteredIn.Time.ToString("HH:mm");
            }
            if (gd.TimeRegisteredOut != null)
            {
                timeRegisteredOut.Text = "Du er meldt ud: " + gd.TimeRegisteredOut.Time.ToString("HH:mm");
            }
            if (gd.SearchDateTime > new DateTime(1950, 1, 1))
            {
                searchTimeFrom.Text = "Fra d.: " + gd.SearchDateTime.ToString("dd/MM/yyyy");
            }
            else
            {
                searchTimeFrom.Text = "Fra dato: Ingen dato sat";
            }
            if (gd.SearchDateTimeLast < new DateTime(2050, 1, 1))
            {
                searchTimeTo.Text = "Til d.: " + gd.SearchDateTimeLast.ToString("dd/MM/yyyy");
            }
            else
            {
                searchTimeTo.Text = "Til dato: Ingen dato sat";
            }
            if (gd.SearchUserName != null)
            {
                searchUser.Text = "Brugernavn: " + gd.SearchUserName;
            }
            else
            {
                searchUser.Text = "Brugernavn: Intet brugernavn";

            }
        }
    }
}
