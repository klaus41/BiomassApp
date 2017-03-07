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
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            MakeListView();
            //MakeGrid(tasks);

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
                //var sv = new MaintenanceService();
                //var es = await sv.CreateTask(task);
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

            Navigation.PushAsync(new TaskDetail(_task));

        }

        void Lv_Refreshing(object sender, EventArgs e)
        {
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateItemSource();
        }


    }
}

