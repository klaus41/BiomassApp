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
    public partial class MeetLeavePage : ContentPage
    {
        ListView lv;
        MaintenanceDatabase db = App.Database;
        List<TimeRegistrationModel> timeRegList;
        Button checkIn;
        Button checkOut;
        Label checkInLabel;
        Label checkOutLabel;
        StackLayout layout;
        SynchronizerFacade syncFace = SynchronizerFacade.GetInstance;
        GlobalData gd = GlobalData.GetInstance;
        bool _in;
        bool _out;
        public MeetLeavePage()
        {
            _in = true;
            _out = false;
            NavigationPage.SetHasNavigationBar(this, false);
            Title = "Komme/Gå";

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
            layout = new StackLayout { Padding = 10 };

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
                response = await syncFace.TimeRegistrationSynchronizer.SyncDatabaseWithNAV();
            }
            GetData();
            if (lv.IsRefreshing)
            {
                lv.EndRefresh();
            }
        }
        private async Task<TimeRegistrationModel> MakeNewTimeReg(string type)
        {
            ActivityIndicator ai = new ActivityIndicator()
            {
                IsRunning = true
            };
            layout.Children.Remove(lv);
            layout.Children.Add(ai);
            TimeRegistrationModel timeReg = new TimeRegistrationModel
            {
                //No = timeRegList.OrderByDescending(x => x.No).FirstOrDefault().No + 1,
                Type = type,
                Time = DateTime.Now.ToUniversalTime(),
                User = gd.User.Code,
                New = true,
                Sent = false
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

            await db.SaveTimeRegAsync(timeReg);
            GetData();
            layout.Children.Add(lv);
            layout.Children.Remove(ai);
            return timeReg;
        }
        private async void CheckOut_Clicked(object sender, EventArgs e)
        {
            checkOut.IsEnabled = false;
            gd.TimeRegisteredOut = await MakeNewTimeReg("Check out");


        }

        private async void CheckIn_Clicked(object sender, EventArgs e)
        {
            checkIn.IsEnabled = false;
            gd.TimeRegisteredIn = await MakeNewTimeReg("Check in");
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();
            GetData();
        }


        public async void GetData()
        {

            timeRegList = await db.GetTimeRegsAsync();

            List<TimeRegistrationModel> itemssourceList = timeRegList.Where(x => x.User == gd.User.Code).OrderByDescending(x => x.Time).ToList();
            TimeRegistrationModel first = itemssourceList.FirstOrDefault();
            if (first != null && first.Time.Date == DateTime.Today.Date)
            {
                if (first.Type == "Check in")
                {
                    _in = false;
                    _out = true;
                    //checkIn.IsEnabled = false;
                    checkOut.Text = "Meld ud";
                    checkIn.Text = "Allerede mødt";
                    //gd.TimeRegisteredIn = first;
                }
                else
                {
                    _out = false;
                    _in = true;
                    //checkOut.IsEnabled = false;       
                    checkIn.Text = "Mød ind";
                    checkOut.Text = "Allerede meldt ud";
                    //gd.TimeRegisteredOut = first;
                }
                foreach (var item in itemssourceList)
                {
                    if (item.Type == "Check in" && (gd.TimeRegisteredIn == null || item.Time > gd.TimeRegisteredIn.Time))
                    {
                        //item.Time = item.Time.AddHours(2);
                        gd.TimeRegisteredIn = item;
                    }
                    else if (item.Type == "Check out" && (gd.TimeRegisteredOut == null || item.Time > gd.TimeRegisteredOut.Time))
                    {
                        //item.Time = item.Time.AddHours(2);
                        gd.TimeRegisteredOut = item;
                    }
                }
            }
         
            checkIn.IsEnabled = _in;
            checkOut.IsEnabled = _out;

            lv.ItemsSource = itemssourceList.OrderByDescending(x => x.No);
        }
    }
}
