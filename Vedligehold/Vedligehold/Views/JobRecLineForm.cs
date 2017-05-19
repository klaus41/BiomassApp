using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class JobRecLineForm : ContentPage
    {
        StackLayout layout;

        string workType = null;

        Entry descriptionEntry;
        Entry amount;
        Button workTypeButton;
        Button unitCodeButton;
        Button done;
        Button cancel;
        Button cameraButton;

        DatePicker datePicker;

        JobRecLine recLine;
        PictureModel pic;
        MaintenanceTask taskGlobal;
        GlobalData gd = GlobalData.GetInstance;
        MaintenanceDatabase db = App.Database;
        public JobRecLineForm(MaintenanceTask task)
        {
            recLine = new JobRecLine();
            taskGlobal = task;

            descriptionEntry = new Entry { Text = taskGlobal.text };
            amount = new Entry { Placeholder = "Antal", Keyboard = Keyboard.Numeric };

            workTypeButton = new Button { Text = "Arbejdstype", BackgroundColor = Color.Red, TextColor = Color.White };
            unitCodeButton = new Button { Text = "Enhedskode", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            done = new Button { Text = "OK", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            cancel = new Button { Text = "Cancel", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, VerticalOptions = LayoutOptions.EndAndExpand };
            cameraButton = new Button { Text = "Tilføj billede", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            datePicker = new DatePicker()
            {
                Format = "D",
                Date = DateTime.Today
            };
            workTypeButton.Clicked += WorkTypeButton_Clicked;
            done.Clicked += Done_Clicked;
            cancel.Clicked += Cancel_Clicked;
            cameraButton.Clicked += CameraButton_Clicked;
            layout = new StackLayout
            {
                Children =
                {
                    datePicker,
                    workTypeButton,
                    descriptionEntry,
                    amount,
                    done,
                    cameraButton,
                    cancel
                }
            };
            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            Content = layout;
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.png",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
            });

            if (file == null)
                return;

            Byte[] ba;

            using (var memoryStream = new MemoryStream())
            {
                file.GetStream().CopyTo(memoryStream);
                file.Dispose();
                ba = memoryStream.ToArray();
            }
            string picture = Convert.ToBase64String(ba);
            pic = new PictureModel()
            {
                Picture = picture,
            };
            //await pdf.PostPicture(pic, _activity.UniqueID);
        }
        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void Done_Clicked(object sender, EventArgs e)
        {
            HandleElements();
            if (workType == null || amount.Text == "")
            {
                await DisplayAlert("Fejl!", "Du skal oplyse både en arbejdstype og antal", "OK");
            }
            else
            {
                Resources resource = null;
                while (resource == null)
                {
                    List<Resources> rl = await db.GetResourcesAsync();
                    resource = rl.Where(x => x.Name == gd.User.Name).FirstOrDefault();
                }
                SetRecLineValues(resource.No);
                var response = App.Database.SaveJobRecLineAsync(recLine);

                await Navigation.PopModalAsync();
            }
            HandleElements();
        }

        private void SetRecLineValues(string no)
        {
            recLine.JobRecLineGUID = Guid.NewGuid();
            recLine.Description = descriptionEntry.Text;
            //recLine.Job_No = ""; //sagsnummer - bør komme fra opgaven
            //recLine.Job_Planning_Line_No = 0;
            //recLine.Job_Task_No = ""; //sagsopgavenummer - bør komme fra opgaven
            //recLine.Journal_Batch_Name = "KG";
            //recLine.Journal_Template_Name = "SAGER";
            //recLine.Line_No = 0;
            recLine.MaintenanceTaskNo = taskGlobal.no.ToString();
            recLine.No = no;
            recLine.Posting_Date = datePicker.Date;
            recLine.Quantity = double.Parse(amount.Text, CultureInfo.InvariantCulture);
            //recLine.Type = "Resource";
            if (pic != null)
            {
                pic.id = recLine.JobRecLineGUID.ToString();
                App.Database.SavePictureAsync(pic);
            }
        }

        private async void WorkTypeButton_Clicked(object sender, EventArgs e)
        {
            string[] options = new string[4] { "Kørsel", "Rejsetid", "Stk", "Konsulenttimer" };
            workType = await DisplayActionSheet("Arbejdstype", "Cancel", null, options);

            recLine.WorkType = workType;
            workTypeButton.Text = "arbejdstype (" + workType + ")";

            switch (workType)
            {
                case "Kørsel":
                    recLine.Unit_of_Measure_Code = "KM";
                    recLine.Work_Type_Code = "KM";
                    break;
                case "Rejsetid":
                    recLine.Unit_of_Measure_Code = "TIMER";
                    recLine.Work_Type_Code = "REJSETID";
                    break;
                case "Stk":
                    recLine.Unit_of_Measure_Code = "STK";
                    recLine.Work_Type_Code = "STK";
                    break;
                case "Konsulenttimer":
                    recLine.Unit_of_Measure_Code = "TIMER";
                    recLine.Work_Type_Code = "TIMER";
                    break;
            }
            workTypeButton.BackgroundColor = Color.FromRgb(135, 206, 250);
        }

        private void HandleElements()
        {
            if (done.IsEnabled)
            {
                datePicker.IsEnabled = false;
                workTypeButton.IsEnabled = false;
                descriptionEntry.IsEnabled = false;
                amount.IsEnabled = false;
                done.IsEnabled = false;
                cancel.IsEnabled = false;
            }
            else
            {
                datePicker.IsEnabled = true;
                workTypeButton.IsEnabled = true;
                descriptionEntry.IsEnabled = true;
                amount.IsEnabled = true;
                done.IsEnabled = true;
                cancel.IsEnabled = true;
            }
        }
    }
}
