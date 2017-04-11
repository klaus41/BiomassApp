using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;
using Vedligehold.Services.Synchronizers;
using Vedligehold.Views.CustomCells;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class TimeRegistrationPage : ContentPage
    {
        ListView lv;
        MaintenanceDatabase db = App.Database;
        List<TimeRegistrationModel> timeRegList;
        Button checkIn;
        Button checkOut;
        Label checkInLabel;
        Label checkOutLabel;
        GlobalData gd = GlobalData.GetInstance;
        bool _in;
        bool _out;
        public TimeRegistrationPage()
        {
            _in = true;
            _out = false;
            NavigationPage.SetHasNavigationBar(this, false);
            Title = "Tidsregistrering";

            var temp = new DataTemplate(typeof(CustomTimeRegCell));

            lv = new ListView()
            {
                HasUnevenRows = true,
                ItemTemplate = temp,
                IsPullToRefreshEnabled = true
            };


            lv.Refreshing += Lv_Refreshing;
            //lv.ItemTapped += Lv_ItemTapped; ;
            checkIn = new Button() { Text = "Mød ind", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            checkOut = new Button() { Text = "Meld ud", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            checkInLabel = new Label();
            checkOutLabel = new Label();

            checkOut.Clicked += CheckOut_Clicked;
            checkIn.Clicked += CheckIn_Clicked;
            StackLayout layout = new StackLayout { Padding = 10 };

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            layout.Children.Add(checkIn);
            layout.Children.Add(checkOut);
            layout.Children.Add(lv);
            Content = new ScrollView { Content = layout };
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void Lv_Refreshing(object sender, EventArgs e)
        {
            bool response = false;
            while (!response)
            {
                TimeRegistrationSynchronizer trs = new TimeRegistrationSynchronizer();
                response = await trs.SyncDatabaseWithNAV();
            }
            GetData();
            if (lv.IsRefreshing)
            {
                lv.EndRefresh();
            }
        }

        private async void CheckOut_Clicked(object sender, EventArgs e)
        {
            checkOut.IsEnabled = false;
            TimeRegistrationModel timeReg = new TimeRegistrationModel
            {
                No = timeRegList.Last().No + 1,
                Type = "Check out",
                Time = DateTime.Now,
                User = gd.User.Code
            };
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                timeReg.Latitude = position.Latitude;
                timeReg.Longitude = position.Longitude;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
            }
            gd.TimeRegisteredOut = timeReg;
            await db.SaveTimeRegAsync(timeReg);
        }

        private async void CheckIn_Clicked(object sender, EventArgs e)
        {
            checkIn.IsEnabled = false;
            TimeRegistrationModel timeReg = new TimeRegistrationModel();

            timeReg.No = timeRegList.Last().No + 1;
            timeReg.Type = "Check in";
            timeReg.Time = DateTime.Now;
            timeReg.User = gd.User.Code;


            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                timeReg.Latitude = position.Latitude;
                timeReg.Longitude = position.Longitude;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
            }
            gd.TimeRegisteredIn = timeReg;
            await db.SaveTimeRegAsync(timeReg);
        }

        protected override void OnAppearing()
        {
            gd.Sync();
            //base.OnAppearing();
            GetData();
        }


        public async void GetData()
        {
            List<TimeRegistrationModel> tempList = new List<TimeRegistrationModel>();
            tempList = timeRegList;
            while (tempList == timeRegList)
            {
                timeRegList = await db.GetTimeRegsAsync();
            }
            if (timeRegList.Count == 0)
            {
                TimeRegistrationSynchronizer mts = new TimeRegistrationSynchronizer();
                mts.DeleteAndPopulateDb();
            }
            foreach (var item in timeRegList)
            {
                if (item.Time > DateTime.Today && item.User == gd.User.Code)
                {
                    if (item.Type == "Check in")
                    {
                        _in = false;
                        _out = true;
                        //checkIn.IsEnabled = false;
                        checkIn.Text = "Allerede mødt";
                        gd.TimeRegisteredIn = item;
                    }
                    if (item.Type == "Check out")
                    {
                        _out = false;
                        //checkOut.IsEnabled = false;
                        checkOut.Text = "Allerede meldt ud";
                        gd.TimeRegisteredOut = item;
                    }
                }
            }

            if (gd.SearchUserName != null)
            {
                lv.ItemsSource = timeRegList.Where(x => x.User == gd.SearchUserName).OrderByDescending(x => x.Time);
            }
            else
            {
                lv.ItemsSource = timeRegList.OrderByDescending(x => x.Time);
            }
            checkIn.IsEnabled = _in;
            checkOut.IsEnabled = _out;
        }
    }
}
