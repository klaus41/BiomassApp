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
        Color color;
        Grid grid;
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

            lv = new ListView();
            Button b = new Button();
            b.Text = "Opret standard opgave";
            //b.BackgroundColor = Color.Green;
            //b.TextColor = Color.White;


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

            lv.HasUnevenRows = true;
            lv.ItemTemplate = temp;

            lv.ItemsSource = tasks;
            lv.IsPullToRefreshEnabled = true;
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
        private void MakeGrid(MaintenanceTask[] tasks)
        {
            grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection(),
                ColumnDefinitions = new ColumnDefinitionCollection(),
                ColumnSpacing = 0,
                RowSpacing = 1
            };
            foreach (var task in tasks)
            {
                if (Array.IndexOf(tasks, task) % 2 == 0)
                {
                    color = Color.FromRgb(211, 211, 211);
                    //color = Color.FromRgb(173, 255, 47);
                }
                else
                {
                    color = Color.Default;
                    //color = Color.Green;
                }

                grid.Children.Add(new Label
                {
                    Text = "Vedligeholdelsesliste",
                    //FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                    HorizontalOptions = LayoutOptions.Center
                }, 0, 4, 0, 1);

                grid.Children.Add(new Label
                {
                    Text = task.no.ToString(),
                    BackgroundColor = color
                }, 0, Array.IndexOf(tasks, task) + 2);

                grid.Children.Add(new Label
                {
                    Text = task.type,
                    BackgroundColor = color
                }, 1, Array.IndexOf(tasks, task) + 2);

                grid.Children.Add(new Label
                {
                    Text = task.anlæg,
                    BackgroundColor = color
                }, 2, Array.IndexOf(tasks, task) + 2);

                var gridButton = new Button
                {
                    Text = "Detaljer",

                    BorderWidth = 0,
                    BorderRadius = 0,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    // Bounds = new Rectangle(50*i,50*k, 50, 50)

                };
                gridButton.Clicked += delegate
                {
                    Navigation.PushAsync(new TaskDetail(task));

                };
                grid.Children.Add(gridButton, 3, Array.IndexOf(tasks, task) + 2);



                //grid.Children.Add(new Label
                //{
                //    Text = task.text,
                //    BackgroundColor = color
                //}, 3, Array.IndexOf(tasks, task) + 2);


                grid.Children.Add(new Label
                {
                    Text = "Nummer"
                }, 0, 1);

                grid.Children.Add(new Label
                {
                    Text = "Type"
                }, 1, 1);

                grid.Children.Add(new Label
                {
                    Text = "Anlæg"
                }, 2, 1);

                grid.Children.Add(new Label
                {
                    Text = "Tekst",
                }, 3, 1);
            }

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = grid;
            TapGesture();
        }
        private void TapGesture()
        {

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                Debug.WriteLine("CLICK");
            };
            grid.GestureRecognizers.Add(tapGestureRecognizer);
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

