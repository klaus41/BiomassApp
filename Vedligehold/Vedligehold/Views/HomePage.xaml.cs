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
        MaintenanceTask[] tasks;
        string[] contactNumbers;
        bool loading;
        StackLayout layout;
        ActivityIndicator ai;
        Button logOutButton;
        Button statButton;
        Button taskButton;
        Color buttonColor;

        public HomePage()
        {
            buttonColor = Color.FromRgb(135, 206, 250);
            BackgroundColor = Color.White;
            NavigationPage.SetHasNavigationBar(this, false);
            layout = new StackLayout { Padding = 10, };

            logOutButton = new Button { Text = "Log ud", BackgroundColor = buttonColor, TextColor = Color.White };
            statButton = new Button { Text = "Leverandørstatistikker", BackgroundColor = buttonColor, TextColor = Color.White };
            taskButton = new Button { Text = "Vedligeholdsopgaver", BackgroundColor = buttonColor, TextColor = Color.White };

            logOutButton.Clicked += (s, e) =>
            {
                Application.Current.MainPage.Navigation.PopAsync();
                //Navigation.PushAsync(new LoginPage());
            };
            statButton.Clicked += async (s, e) =>
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
                        RemoveActivityIndicator();
                    }
                }
            };
            taskButton.Clicked += async (s, e) =>
            {
                loading = true;
                while (loading)
                {
                    ShowActivityIndicator();
                    tasks = null;

                    while (tasks == null)
                    {
                        var sv = new MaintenanceService();
                        var es = await sv.GetMaintenanceTasksAsync();
                        tasks = es;
                    }


                    await Navigation.PushAsync(new MaintenancePage(tasks));
                    RemoveActivityIndicator();
                }
                
            };

            Image image = new Image();

            image.Source = "eistor.png";
            image.Opacity = 0.7;
            image.VerticalOptions = LayoutOptions.End;

            layout.Children.Add(statButton);
            layout.Children.Add(taskButton);
            layout.Children.Add(logOutButton);
            layout.Children.Add(image);

            Content = new ScrollView { Content = layout };


        }

        private void RemoveActivityIndicator()
        {

            loading = false;
            layout.Children.Remove(ai);
            logOutButton.IsEnabled = true;
            statButton.IsEnabled = true;
            taskButton.IsEnabled = true;
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
        }
    }
}
