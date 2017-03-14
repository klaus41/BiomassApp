using System;
using System.Collections.Generic;
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
        public TimeRegistrationPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            Title = "Tidsregistrering";

            MakeListView();
        }

        private void MakeListView()
        {
            var temp = new DataTemplate(typeof(CustomTimeRegCell));

            lv = new ListView()
            {
                HasUnevenRows = true,
                ItemTemplate = temp,
                IsPullToRefreshEnabled = true
            };

            Button b = new Button() { Text = "Opret tidsregistrering", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            b.Clicked += async (s, e) =>
            {
                int no = 0;
                if (timeRegList.Count != 0)
                {
                    no = timeRegList.LastOrDefault().No + 1;
                }
                TimeRegistrationModel timeReg = new TimeRegistrationModel
                { 
                    No = no,
                    Type = "Check in"
                };
                await db.SaveTimeRegAsync(timeReg);
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
            TimeRegistrationModel _reg;
            //Navigation.PushAsync(new TaskDetail());

            var action = ((ListView)sender).SelectedItem;
            TimeRegistrationModel reg = (TimeRegistrationModel)action;

            _reg = timeRegList.Where(x => x.No == reg.No).First();

            Navigation.PushModalAsync(new TimeRegistrationDetailPage(_reg));

        }

        async void Lv_Refreshing(object sender, EventArgs e)
        {
            bool response = false;
            while (!response)
            {
                TimeRegistrationSynchronizer trs = new TimeRegistrationSynchronizer();
                response = await trs.SyncDatabaseWithNAV();
            }
            UpdateItemSource();
            if (lv.IsRefreshing)
            {
                lv.EndRefresh();
            }
        }

        private async void UpdateItemSource()
        {
            timeRegList = await db.GetTimeRegsAsync();
            lv.ItemsSource = timeRegList;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateItemSource();
        }
    }
}
