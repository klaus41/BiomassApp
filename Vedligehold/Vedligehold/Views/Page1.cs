using Android.Util;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class Page1 : ContentPage
    {
        Button cameraButton = new Button() { Text = "Kamera" };
        Image image = new Image();
        public Page1()
        {
            Title = "Kamera";
            cameraButton.Clicked += CameraButton_Clicked;
            Content = new StackLayout
            {
                Children =
                {
                    cameraButton,
                    //image
                }
            };
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg"
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
            MaintenanceTask task = null; 
            while (task == null)
            {
                task = await App.Database.GetTaskAsync(0);
            }
            task.image = ba;
            await App.Database.UpdateTaskAsync(task);
        }
    }
}
