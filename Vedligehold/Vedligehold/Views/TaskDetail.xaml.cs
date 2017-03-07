using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services;
using Vedligehold.Views.CustomCells;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class TaskDetail : ContentPage
    {
        int i;
        Color color;
        Grid grid;
        MaintenanceTask taskGlobal;
        ListView lv;
        List<TaskDetailModel> detailList;

        public TaskDetail(MaintenanceTask task)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Labl.Text = task.anlæg + task.no.ToString();
            taskGlobal = task;

            Button btn = new Button() { BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            Button mapButton = new Button() { Text = "Vis på kort", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            mapButton.Clicked += MapButton_Clicked;

            btn.Clicked += async (s, e) =>
            {
                if (!task.done)
                {
                    task.done = true;
                    try
                    {
                        var locator = CrossGeolocator.Current;
                        locator.DesiredAccuracy = 50;
                        var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                        task.latitude = position.Latitude;
                        task.longitude = position.Longitude;

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
                    }
                }

                await App.Database.UpdateTaskAsync(task);
                await Application.Current.MainPage.Navigation.PopAsync();
            };

            //MakeGrid(task);
            MakeListView();
            if (task.done)
            {
                btn.IsEnabled = false;
                //btn.Text = "Sæt til ikke færdig";
            }
            else
            {
                btn.Text = "Sæt til færdig";
            }


            this.Content = new StackLayout
            {
                Children =
                {
                    btn,
                    mapButton,
                    lv
                }
            };
        }

        private void MapButton_Clicked(object sender, EventArgs e)
        {
            if (taskGlobal.longitude != 0 && taskGlobal.latitude != 0)
            {
                string s = "https://www.google.dk/maps/place/" + taskGlobal.latitude + "," + taskGlobal.longitude + "/" + taskGlobal.latitude + "," + taskGlobal.longitude + "z/";
                Uri uri = new Uri(s);
                Device.OpenUri(uri);
            }
            else
            {
                DisplayAlert("Ingen koordinater", "Der er ingen koordinater på opgaven. Bekræft at opgaven er afsluttet, og prøv igen.", "OK");
            }
        }

        private void MakeListView()
        {
            var temp = new DataTemplate(typeof(CustomTaskDetailCell));
            Application.Current.Properties["gridrowindex"] = 1;
            lv = new ListView();

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    lv
                }
            };
            lv.HasUnevenRows = true;
            lv.ItemTemplate = temp;

            detailList = new List<TaskDetailModel>();
            PopulateDetailList();

            lv.ItemsSource = detailList;
        }

        private void PopulateDetailList()
        {
            string weekly;
            string daily;
            string done;

            if (taskGlobal.daily == true)
            {
                daily = "Ja";
            }
            else
            {
                daily = "Nej";
            }
            if (taskGlobal.done == true)
            {
                done = "Ja";
            }
            else
            {
                done = "Nej";
            }
            if (taskGlobal.weekly == true)
            {
                weekly = "Ja";
            }
            else
            {
                weekly = "Nej";
            }
            detailList.Add(new TaskDetailModel() { type = "Nummer", value = taskGlobal.no.ToString() });
            detailList.Add(new TaskDetailModel() { type = "Type", value = taskGlobal.type });
            detailList.Add(new TaskDetailModel() { type = "Anlæg", value = taskGlobal.anlæg });
            detailList.Add(new TaskDetailModel() { type = "Anlægsbeskrivelse", value = taskGlobal.anlægsbeskrivelse });
            detailList.Add(new TaskDetailModel() { type = "Tekst", value = taskGlobal.text });
            detailList.Add(new TaskDetailModel() { type = "Ugentlig", value = weekly });
            detailList.Add(new TaskDetailModel() { type = "Daglig", value = daily });
            detailList.Add(new TaskDetailModel() { type = "Færdig", value = done });
            detailList.Add(new TaskDetailModel() { type = "Længdegrad", value = taskGlobal.latitude.ToString() });
            detailList.Add(new TaskDetailModel() { type = "Breddegrad", value = taskGlobal.longitude.ToString() });

        }

        private void MakeGrid(MaintenanceTask task)
        {
            i = 0;

            grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection(),
                ColumnDefinitions = new ColumnDefinitionCollection(),
                ColumnSpacing = 0,
                RowSpacing = 1,

            };
            AddChildren();

            //TypeInfo ti = task.GetType().GetTypeInfo();
            //foreach (var prop in ti.DeclaredProperties)
            foreach (var item in grid.Children)
            {
                if (i % 2 == 0)
                {

                    color = Color.FromRgb(211, 211, 211);
                    //color = Color.FromRgb(173, 255, 47);
                }
                else
                {
                    color = Color.Default;
                    //color = Color.Green;
                }
                item.BackgroundColor = color;
                Debug.WriteLine(item.X + item.Y);

                i++;
            }
            // Accomodate iPhone status bar.
            this.Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 5);

            // Build the page.
            this.Content = grid;


        }

        private void AddChildren()
        {
            grid.Children.Add(new Label { Text = "Nummer" }, 0, 0);
            grid.Children.Add(new Label { Text = taskGlobal.no.ToString(), }, 1, 0);

            grid.Children.Add(new Label { Text = "Anlæg", }, 0, 1);
            grid.Children.Add(new Label { Text = taskGlobal.anlæg, }, 1, 1);

            grid.Children.Add(new Label { Text = "Done", }, 0, 2);
            grid.Children.Add(new Label { Text = taskGlobal.done.ToString(), }, 1, 2);
        }
    }
}
