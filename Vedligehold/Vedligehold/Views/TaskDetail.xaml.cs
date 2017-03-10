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
        MaintenanceTask taskGlobal;
        ListView lv;
        List<TaskDetailModel> detailList;

        public TaskDetail(MaintenanceTask task)
        {
            InitializeComponent();
            //Title = "Detaljer";
            NavigationPage.SetHasNavigationBar(this, false);
            taskGlobal = task;

            Button btn = new Button() { BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            Button mapButton = new Button() { Text = "Vis på kort", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            Button pdfButton = new Button() { Text = "Vis PDF", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            mapButton.Clicked += MapButton_Clicked;
            pdfButton.Clicked += PdfButton_Clicked;
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
                btn.Text = "Opgaven er markeret som færdig";
                btn.BackgroundColor = Color.FromRgb(205, 201, 201);

            }
            else
            {
                btn.Text = "Sæt til færdig";
            }

            if (task.longitude == 0 || task.latitude == 0)
            {
                mapButton.IsEnabled = false;
                mapButton.Text = "Opgaven har ingen koordinater endnu";
                mapButton.BackgroundColor = Color.FromRgb(205, 201, 201);
            }

            this.Content = new StackLayout
            {
                Children =
                {
                    btn,
                    mapButton,
                    pdfButton,
                    lv
                }
            };
        }

        private async void PdfButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var service = new PDFService();
                string data = await service.GetPDF(taskGlobal.anlæg);
                if (!data.Contains("NoFile"))
                {
                    int i = data.Length - 2;
                    string newdata = data.Substring(1, i);

                    Device.OpenUri(new Uri("http://demo.biomass.eliteit.dk" + newdata));
                }
                else
                {
                    await DisplayAlert("Fejl!", "Der eksisterer ingen PFD på anlæg " + taskGlobal.anlæg + ", " + taskGlobal.anlægsbeskrivelse, "OK");
                }
               
            }
            catch
            {
                await DisplayAlert("Fejl!", "Kunne ikke hente PDF", "OK");
            }
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
    }
}
