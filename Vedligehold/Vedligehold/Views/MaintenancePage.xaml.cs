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
using Vedligehold.Services.Synchronizers;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class MaintenancePage : ContentPage
    {
        ListView lv;
        MaintenanceDatabase db = App.Database;
        List<MaintenanceTask> tasks;
        GlobalData gd = GlobalData.GetInstance;
        IEnumerable<MaintenanceTask> itemssourceList;
        Button showDoneButton;
        bool showDone = false;
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
                if (_task.status == "Released")
                {
                    _task.status = "Completed";
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
                    await DisplayAlert("OBS!", "Opgaven er allerede markeret som udført", "OK");
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

            showDoneButton = new Button() { Text = "Vis udførte opgaver", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            showDoneButton.Clicked += ShowDoneButton_Clicked;
     
            StackLayout layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                    {
                        showDoneButton,
                        lv
                    }

            };
            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            Content = layout;
            lv.Refreshing += Lv_Refreshing;
            lv.ItemTapped += Lv_ItemTapped;

        }

        private void ShowDoneButton_Clicked(object sender, EventArgs e)
        {
            if (showDone)
            {
                showDone = false;
                showDoneButton.Text = "Vis udførte opgaver";
            }
            else
            {
                showDone = true;
                showDoneButton.Text = "Skjul udførte opgaver";
            }
            UpdateItemSource();
        }

        void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var action = ((ListView)sender).SelectedItem;
            MaintenanceTask tsk = (MaintenanceTask)action;

            this.Navigation.PushModalAsync(new MaintenanceDetail(tsk));

        }

        async void Lv_Refreshing(object sender, EventArgs e)
        {
            bool response = false;
            while (!response)
            {
                MaintenanceTaskSynchronizer mst = new MaintenanceTaskSynchronizer();
                MaintenanceActivitySynchronizer mas = new MaintenanceActivitySynchronizer();
                await mas.SyncDatabaseWithNAV();
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
            DateTime nullDate = new DateTime(1900, 1, 1);
            if (gd.SearchUserName != null && gd.SearchDateTime > nullDate && gd.SearchDateTimeLast > nullDate)
            {
                itemssourceList = tasks.Where(x => x.planned_Date.Date >= gd.SearchDateTime.Date && x.planned_Date <= gd.SearchDateTimeLast && x.responsible == gd.SearchUserName);
            }
            else if (gd.SearchUserName == null && gd.SearchDateTime > nullDate && gd.SearchDateTimeLast > nullDate)
            {
                itemssourceList = tasks.Where(x => x.planned_Date.Date >= gd.SearchDateTime.Date && x.planned_Date <= gd.SearchDateTimeLast);
            }
            else if (gd.SearchUserName != null && gd.SearchDateTime < new DateTime(1900, 1, 1))
            {
                itemssourceList = tasks.Where(x => x.responsible == gd.SearchUserName);
            }
            else
            {
                itemssourceList = tasks;
            }
            if (showDone)
            {
                lv.ItemsSource = itemssourceList.Where(x => x.status == "Released" || x.status == "Completed");
            }
            else if (!showDone)
            {
                lv.ItemsSource = itemssourceList.Where(x => x.status == "Released");
            }
        }
      
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateItemSource();
        }
    }
}

