using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Vedligehold.Database;
using Vedligehold.Models;
using Vedligehold.Services;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class MaintenancePage : ContentPage
    {
        ListView lv;
        MaintenanceDatabase db = App.Database;
        List<MaintenanceTask> tasks;
        public MaintenancePage()
        {
            Title = "Opgaver";
            //MakeToolBar();
            NavigationPage.SetHasNavigationBar(this, false);
            MakeListView();
        }
        public void ShowOnMap(MaintenanceTask _task)
        {
            if (_task.longitude != 0 && _task.latitude != 0)
            {
                string s = "https://www.google.dk/maps/place/" + _task.latitude + "," + _task.longitude + "/" + _task.latitude + "," + _task.longitude + "z/";
                Uri uri = new Uri(s);
                Device.OpenUri(uri);
            }
            else
            {
                DisplayAlert("Ingen koordinater", "Der er ingen koordinater på opgaven. Bekræft at opgaven er afsluttet, og prøv igen.", "OK");
            }
        }
        public async void SetDone(MaintenanceTask _task)
        {
            int i = 0;
            while (i == 0)
            {
                if (!_task.done)
                {
                    _task.done = true;
                    try
                    {
                        var locator = CrossGeolocator.Current;
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                        _task.latitude = position.Latitude;
                        _task.longitude = position.Longitude;

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
                    }
                    i = await App.Database.UpdateTaskAsync(_task);
                }
                else
                {
                    i = 1;
                    await DisplayAlert("OBS!", "Opgaven er allerede markeret som færdig", "OK");
                }
            }
            UpdateItemSource();
        }

        public async void ShowPDF(MaintenanceTask _task)
        {
            try
            {
                var service = new PDFService();
                string data = await service.GetPDF(_task.anlæg);
                if (!data.Contains("NoFile"))
                {
                    int i = data.Length - 2;
                    string newdata = data.Substring(1, i);

                    Device.OpenUri(new Uri("http://demo.biomass.eliteit.dk" + newdata));
                }
                else
                {
                    await DisplayAlert("Fejl!", "Der eksisterer ingen PDF på anlæg " + _task.anlæg + ", " + _task.anlægsbeskrivelse, "OK");
                }

            }
            catch
            {
                await DisplayAlert("Fejl!", "Har ingen forbindelse til NAV", "OK");
            }
        }

        private void MakeListView()
        {
            var temp = new DataTemplate(typeof(CustomTaskCell));
            Application.Current.Properties["gridrowindex"] = 1;

            lv = new ListView()
            {
                HasUnevenRows = true,
                ItemTemplate = temp,
                IsPullToRefreshEnabled = true
            };

            Button b = new Button() { Text = "Opret standardopgave", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            b.Clicked += async (s, e) =>
            {
                MaintenanceTask task = new MaintenanceTask
                {
                    no = tasks.Last().no + 1,
                    type = "Rundering",
                    anlæg = "A00005",
                    anlægsbeskrivelse = "GasAlertMicro ",
                    text = "Test",
                    weekly = true,
                    daily = false,
                    done = false
                };
                await db.SaveTaskAsync(task);
                UpdateItemSource();
            };

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    b,
                    lv
                }
            };

            lv.Refreshing += Lv_Refreshing;
            lv.ItemTapped += Lv_ItemTapped;

        }
        void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            MaintenanceTask _task;
            //Navigation.PushAsync(new TaskDetail());

            var action = ((ListView)sender).SelectedItem;
            MaintenanceTask tsk = (MaintenanceTask)action;

            _task = tasks.Where(x => x.no == tsk.no).First();
        
            this.Navigation.PushModalAsync(new TaskDetail(_task));

        }

        async void Lv_Refreshing(object sender, EventArgs e)
        {
            bool response = false;
            while (!response)
            {
                MaintenanceTaskSynchronizer mst = new MaintenanceTaskSynchronizer();
                response = await mst.SyncDatabaseWithNAV();
            }
            UpdateItemSource();
            if (lv.IsRefreshing)
            {
                lv.EndRefresh();
            }
        }

        private async void UpdateItemSource()
        {
            tasks = await db.GetTasksAsync();
            lv.ItemsSource = tasks;
            Title = tasks.Count().ToString() + " opgaver";
        }

        private void MakeToolBar()
        {
            ToolbarItems.Add(new ToolbarItem("Hjem", "filter.png", async () =>
            {
                if (this.GetType() != typeof(HomePage))
                {
                    await Navigation.PushModalAsync(new HomePage());
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
                    await Navigation.PushModalAsync(new MaintenancePage());
                }
            }));

            ToolbarItems.Add(new ToolbarItem("Indstillinger", "filter.png", async () =>
            {
                if (this.GetType() != typeof(SettingsPage))
                {
                    await Navigation.PushModalAsync(new SettingsPage());
                }
            }));

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateItemSource();
        }
    }
}

