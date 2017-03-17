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
            lv.ItemTapped += Lv_ItemTapped; ;
            checkIn = new Button() { Text = "Mød ind", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            checkOut = new Button() { Text = "Meld ud", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            checkInLabel = new Label();
            checkOutLabel = new Label();

            checkOut.Clicked += CheckOut_Clicked;
            checkIn.Clicked += CheckIn_Clicked;
            StackLayout layout = new StackLayout { Padding = 10 };
            layout.Children.Add(checkIn);
            layout.Children.Add(checkOut);
            layout.Children.Add(lv);
            Content = new ScrollView { Content = layout };

            MakeListView();
        }
        private void MakeListView()
        {


        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Lv_Refreshing(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void CheckOut_Clicked(object sender, EventArgs e)
        {
            checkOut.IsEnabled = false;
            TimeRegistrationModel timeReg = new TimeRegistrationModel
            {
                No = timeRegList.Last().No + 1,
                Type = "Check out",
                Time = DateTime.Now,
                User = gd.User
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
            TimeRegistrationModel timeReg = new TimeRegistrationModel
            {
                No = timeRegList.Last().No + 1,
                Type = "Check in",
                Time = DateTime.Now,
                User = gd.User
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
            gd.TimeRegisteredIn = timeReg;
            await db.SaveTimeRegAsync(timeReg);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
            foreach (var item in timeRegList)
            {
                if (item.Time > DateTime.Today && item.User == gd.User)
                {
                    if (item.Type == "Check in")
                    {
                        _in = false;
                        //checkIn.IsEnabled = false;
                        checkIn.Text = "Allerede mødt";
                        gd.TimeRegisteredIn = item;
                    }
                    if (item.Type == " Check out")
                    {
                        _out = false;
                        //checkOut.IsEnabled = false;
                        checkOut.Text = "Allerede meldt ud";
                        gd.TimeRegisteredOut = item;
                    }
                }
            }

            //if (gd.SearchUserName != null && gd.SearchDateTime > new DateTime(1900, 1, 1))
            //{
            //    lv.ItemsSource = timeRegList.Where(x => x.Time.Date == gd.SearchDateTime.Date && x.User == gd.SearchUserName);
            //}
            //else if (gd.SearchUserName == null && gd.SearchDateTime > new DateTime(1900, 1, 1))
            //{
            //    lv.ItemsSource = timeRegList.Where(x => x.Time.Date == gd.SearchDateTime.Date);

            //}
            //else if (gd.SearchUserName != null && gd.SearchDateTime < new DateTime(1900, 1, 1))
            //{
            //    lv.ItemsSource = timeRegList.Where(x => x.User == gd.SearchUserName);
            //}
            if (gd.SearchUserName != null)
            {
                lv.ItemsSource = timeRegList.Where(x => x.User == gd.SearchUserName);
            }
            else
            {
                lv.ItemsSource = timeRegList;
            }
            checkIn.IsEnabled = _in;
            checkOut.IsEnabled = _out;
        }
    }
}
