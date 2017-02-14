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

        public HomePage()
        {
            BackgroundColor = Color.White;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            //Title = "Biomasse App";


            StatButton.Clicked += async (s, e) =>
            {
                stats = null;
                contacts = null;

                while (contacts == null)
                {
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
                    Navigation.PushAsync(new StatisticsPage(stats));
                }
            };



            MaintButton.Clicked += async (s, e) =>
            {
                tasks = null;

                while (tasks == null)
                {
                    var sv = new MaintenanceService();
                    var es = await sv.GetMaintenanceTasksAsync();
                    tasks = es;
                }
                Navigation.PushAsync(new MaintenancePage(tasks));
            };

            image.Source = "sbg.jpg";
        }
        private async void WithPicker()
        {
            tasks = null;
            contacts = null;
            while (contacts == null)
            {
                var sv = new ContactService();
                var es = await sv.GetContactsAsync();
                contacts = es;
            }
            Label header = new Label
            {
                Text = "Picker",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            Picker picker = new Picker
            {
                Title = "Leverandører:",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            foreach (var item in contacts)
            {
                picker.Items.Add(item.no);
            }
            BoxView boxView = new BoxView
            {
                WidthRequest = 150,
                HeightRequest = 150,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            picker.SelectedIndexChanged += async (sender, args) =>
            {
                if (picker.SelectedIndex == -1)
                {

                }
                else
                {
                    while (tasks == null)
                    {
                        var sv = new StatisticService();
                        string id = picker.Items[picker.SelectedIndex];
                        var es = await sv.GetStatsAsync(id);
                        stats = es;
                    }
                    Navigation.PushAsync(new StatisticsPage(stats));
                }
            };

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,
                    picker,
                    boxView
                }
            };
        }
    }
}
