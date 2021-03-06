﻿//using Plugin.Geolocator;
//using Plugin.Media;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Vedligehold.Models;
//using Vedligehold.Services;
//using Vedligehold.Views.CustomCells;
//using Xamarin.Forms;

//namespace Vedligehold.Views
//{
//    public partial class TaskDetail : ContentPage
//    {
//        MaintenanceTask taskGlobal;
//        ListView lv;
//        List<TaskDetailModel> detailList;
//        StackLayout sl;
//        PictureModel pm;
//        public TaskDetail(MaintenanceTask task)
//        {
//            InitializeComponent();
//            //Title = "Detaljer";
//            NavigationPage.SetHasNavigationBar(this, false);
//            taskGlobal = task;

//            Button btn = new Button() { BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
//            Button mapButton = new Button() { Text = "Vis på kort", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
//            Button pdfButton = new Button() { Text = "Vis PDF", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
//            Button textEditButton = new Button() { Text = "Tilpas tekst", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
//            Button doneEdit = new Button() { Text = "udført", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
//            Button cancelEdit = new Button() { Text = "Fortryd", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
//            Editor entryEdit = new Editor() { HeightRequest = 200, TextColor = Color.FromRgb(135, 206, 250) };

//            Button takeImageButton = new Button() { Text = "Tag billede", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

//            Image image = new Image();
//            takeImageButton.Clicked += async (s, e) =>
//            {
//                await CrossMedia.Current.Initialize();

//                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
//                {
//                    DisplayAlert("No Camera", ":( No camera available.", "OK");
//                    return;
//                }
//                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
//                {
//                    Directory = "Sample",
//                    Name = "test.jpg"
//                });

//                if (file == null)
//                    return;

//                Byte[] ba;

//                using (var memoryStream = new MemoryStream())
//                {
//                    file.GetStream().CopyTo(memoryStream);
//                    file.Dispose();
//                    ba = memoryStream.ToArray();
//                }
//                pm = new PictureModel();
//                pm.Task_No = task.no;
//                pm.Picture = Convert.ToBase64String(ba);
//                //task.image = ba;
//                //await App.Database.UpdateTaskAsync(task);
//            };
            
//            var tapGestureRecognizer = new TapGestureRecognizer();
//            tapGestureRecognizer.Tapped += (s, e) =>
//            {
//                Content = sl;
//            };
//            image.GestureRecognizers.Add(tapGestureRecognizer);


//            StackLayout sl2 = new StackLayout
//            {
//                Children =
//                {
//                  entryEdit,
//                  cancelEdit,
//                  doneEdit
//                }
//            };

//            textEditButton.Clicked += (s, e) =>
//            {
//                entryEdit.Text = task.text;
//                Content = sl2;
//            };


//            mapButton.Clicked += MapButton_Clicked;
//            pdfButton.Clicked += PdfButton_Clicked;
//            btn.Clicked += async (s, e) =>
//            {
//                if (task.status == "Released")
//                {
//                    task.status = "Completed";
//                    try
//                    {
//                        var locator = CrossGeolocator.Current;
//                        locator.DesiredAccuracy = 50;
//                        var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

//                        task.latitude = position.Latitude;
//                        task.longitude = position.Longitude;

//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
//                    }
//                }

//                await App.Database.UpdateTaskAsync(task);
//                await Application.Current.MainPage.Navigation.PopModalAsync();
//            };

//            //MakeGrid(task);
//            MakeListView();
//            if (task.status == "Completed")
//            {
//                btn.IsEnabled = false;
//                btn.Text = "Opgaven er markeret som udført";
//                btn.BackgroundColor = Color.FromRgb(205, 201, 201);

//            }
//            else
//            {
//                btn.Text = "Sæt til udført";
//            }

//            if (task.longitude == 0 || task.latitude == 0)
//            {
//                mapButton.IsEnabled = false;
//                mapButton.Text = "Opgaven har ingen koordinater endnu";
//                mapButton.BackgroundColor = Color.FromRgb(205, 201, 201);
//            }
//            sl = new StackLayout
//            {
//                Children =
//                {
//                    btn,
//                    mapButton,
//                    pdfButton,
//                    textEditButton,
//                    takeImageButton,
//                    lv
//                }
//            };
//            doneEdit.Clicked += async (s, e) =>
//            {
//                task.text = entryEdit.Text;
//                await App.Database.UpdateTaskAsync(task);
//                await Application.Current.MainPage.Navigation.PopModalAsync();
//            };
//            cancelEdit.Clicked += (s, e) =>
//            {
//                Content = sl;
//            };
//            this.Content = sl;
//        }


//        private async void PdfButton_Clicked(object sender, EventArgs e)
//        {
//            try
//            {
//                var service = new PDFService();
//                string data = await service.GetPDF(taskGlobal.anlæg);
//                if (!data.Contains("NoFile"))
//                {
//                    int i = data.Length - 2;
//                    string newdata = data.Substring(1, i);

//                    Device.OpenUri(new Uri("http://demo.biomass.eliteit.dk" + newdata));
//                }
//                else
//                {
//                    await DisplayAlert("Fejl!", "Der eksisterer ingen PFD på anlæg " + taskGlobal.anlæg + ", " + taskGlobal.anlægsbeskrivelse, "OK");
//                }

//            }
//            catch
//            {
//                await DisplayAlert("Fejl!", "Kunne ikke hente PDF", "OK");
//            }
//        }

//        private void MapButton_Clicked(object sender, EventArgs e)
//        {
//            if (taskGlobal.longitude != 0 && taskGlobal.latitude != 0)
//            {
//                string s = "https://www.google.dk/maps/place/" + taskGlobal.latitude + "," + taskGlobal.longitude + "/" + taskGlobal.latitude + "," + taskGlobal.longitude + "z/";
//                Uri uri = new Uri(s);
//                Device.OpenUri(uri);
//            }
//            else
//            {
//                DisplayAlert("Ingen koordinater", "Der er ingen koordinater på opgaven. Bekræft at opgaven er afsluttet, og prøv igen.", "OK");
//            }
//        }

//        private void MakeListView()
//        {
//            var temp = new DataTemplate(typeof(CustomTaskDetailCell));
//            Application.Current.Properties["gridrowindex"] = 1;
//            lv = new ListView();

//            Content = new StackLayout
//            {
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Children =
//                {
//                    lv
//                }
//            };
//            lv.HasUnevenRows = true;
//            lv.ItemTemplate = temp;

//            detailList = new List<TaskDetailModel>();
//            PopulateDetailList();

//            lv.ItemsSource = detailList;
//        }

//        private void PopulateDetailList()
//        {
//            string weekly;
//            string daily;
//            string done;

//            if (taskGlobal.daily == true)
//            {
//                daily = "Ja";
//            }
//            else
//            {
//                daily = "Nej";
//            }
//            done = taskGlobal.status;

//            if (taskGlobal.weekly == true)
//            {
//                weekly = "Ja";
//            }
//            else
//            {
//                weekly = "Nej";
//            }
//            detailList.Add(new TaskDetailModel() { type = "Nummer", value = taskGlobal.no.ToString() });
//            detailList.Add(new TaskDetailModel() { type = "Type", value = taskGlobal.type });
//            detailList.Add(new TaskDetailModel() { type = "Anlæg", value = taskGlobal.anlæg });
//            detailList.Add(new TaskDetailModel() { type = "Anlægsbeskrivelse", value = taskGlobal.anlægsbeskrivelse });
//            detailList.Add(new TaskDetailModel() { type = "Tekst", value = taskGlobal.text });
//            detailList.Add(new TaskDetailModel() { type = "Ugentlig", value = weekly });
//            detailList.Add(new TaskDetailModel() { type = "Daglig", value = daily });
//            detailList.Add(new TaskDetailModel() { type = "Status", value = done });
//            detailList.Add(new TaskDetailModel() { type = "Længdegrad", value = taskGlobal.latitude.ToString() });
//            detailList.Add(new TaskDetailModel() { type = "Breddegrad", value = taskGlobal.longitude.ToString() });
//            detailList.Add(new TaskDetailModel() { type = "Planlagt dato", value = taskGlobal.planned_Date.ToString() });
//            detailList.Add(new TaskDetailModel() { type = "Ansvarlig", value = taskGlobal.responsible });

//        }
//    }
//}
