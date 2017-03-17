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
        Label searchCriteria;
        Label user;
        Label timeRegisteredIn;
        Label timeRegisteredOut;
        List<MaintenanceTask> taskList;
        GlobalData gd = GlobalData.GetInstance;
        Color buttonColor;

        public HomePage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            Title = "Hjem";
            user = new Label() { Text = "Du er logget ind som: " + gd.User, FontSize = 20, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            searchCriteria = new Label() { Text = "Dine opgaver er filtreret efter bruger " + gd.SearchUserName + " på dato " + gd.SearchDateTime.ToString("dd/MM/yyyy"), HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            tasks = new Label() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            timeRegisteredIn = new Label() { Text = "Du er ikke mødt ind endnu", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, FontSize = 20, FontAttributes = FontAttributes.Bold };
            timeRegisteredOut = new Label() { Text = "Du er ikke meldt ud endnu", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, FontSize = 20, FontAttributes = FontAttributes.Bold };

            NavigationPage.SetHasNavigationBar(this, false);
            //MakeToolBar();

            layout = new StackLayout { Padding = 10, };

            logOutButton = new Button { Text = "Log ud", BackgroundColor = buttonColor, TextColor = Color.White, VerticalOptions = LayoutOptions.EndAndExpand };
            logOutButton.Clicked += (s, e) =>
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

            };

            Image image = new Image();

            image.Source = "eistor.png";
            image.Opacity = 0.7;
            image.VerticalOptions = LayoutOptions.EndAndExpand;

            layout.Children.Add(user);
            layout.Children.Add(tasks);
            layout.Children.Add(searchCriteria);
            layout.Children.Add(image);
            layout.Children.Add(timeRegisteredIn);
            layout.Children.Add(timeRegisteredOut);
            layout.Children.Add(logOutButton);

            Content = new ScrollView { Content = layout };
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
                if (!item.done)
                {
                    notdone++;
                }
            }
            tasks.Text = "Du har " + notdone + " ufærdige opgaver, der venter.";
            if (gd.SearchUserName == null)
            {
                searchCriteria.Text = "Dine opgaver er filtreret på dato " + gd.SearchDateTime.ToString("dd/MM/yyyy");
            }
            if (gd.SearchUserName == null && gd.SearchDateTime < new DateTime(1900, 1, 1))
            {
                searchCriteria.Text = "Du har ingen filtre på dine søgninger";
            }
            else
            {
                searchCriteria.Text = "Dine opgaver er filtreret efter bruger " + gd.SearchUserName + " på dato " + gd.SearchDateTime.ToString("dd/MM/yyyy");
            }
            user.Text = "Du er logget ind som: " + gd.User;
            if (gd.TimeRegisteredIn != null)
            {
                timeRegisteredIn.Text = "Du er mødt ind: " + gd.TimeRegisteredIn.Time.ToString("HH:mm");
            }
            if (gd.TimeRegisteredOut != null)
            {
                timeRegisteredOut.Text = "Du er meldt ud: " + gd.TimeRegisteredOut.Time.ToString("HH:mm");
            }
        }


    }
}
