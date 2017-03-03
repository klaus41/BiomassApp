using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Vedligehold.Models;
using Vedligehold.Services;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class MaintenancePage : ContentPage
    {
        ListView lv;
        MaintenanceTask[] tasksGlobal;

        public MaintenancePage(MaintenanceTask[] tasks)
        {
            tasksGlobal = tasks;
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            MakeListView(tasks);
            //MakeGrid(tasks);

        }

        private void MakeListView(MaintenanceTask[] tasks)
        {
            var temp = new DataTemplate(typeof(CustomTaskCell));
            Application.Current.Properties["gridrowindex"] = 1;

            lv = new ListView()
            {
                HasUnevenRows = true,
                ItemTemplate = temp,
                ItemsSource = tasks,
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
                var sv = new MaintenanceService();
                var es = await sv.CreateTask(task);
                OnAppearing();
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

            _task = tasksGlobal.Where(x => x.no == tsk.no).First();

            Navigation.PushAsync(new TaskDetail(_task));

        }

        async void Lv_Refreshing(object sender, EventArgs e)
        {
            MaintenanceTask[] _tasks = null;

            while (_tasks == null)
            {
                var sv = new MaintenanceService();
                var es = await sv.GetMaintenanceTasksAsync();
                _tasks = es;
            }
            MakeListView(_tasks);

            if (lv.IsRefreshing)
            {
                lv.EndRefresh();
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            MaintenanceService ms = new MaintenanceService();
            tasksGlobal = await ms.GetMaintenanceTasksAsync();
            MakeListView(tasksGlobal);
        }
    }
}

