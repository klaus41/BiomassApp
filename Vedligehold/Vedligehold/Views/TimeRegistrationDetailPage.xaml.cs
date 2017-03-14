using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class TimeRegistrationDetailPage : ContentPage
    {
        MaintenanceDatabase db = App.Database;

        Label endTimeLabel;
        Button timeButton;
        TimeRegistrationModel timeRegGlobal;
        public TimeRegistrationDetailPage(TimeRegistrationModel timeReg)
        {

            if (timeReg.Time > new DateTime(1950,1,1))
            {
                //timeButton.IsEnabled = false;
            }
            timeRegGlobal = timeReg;
            var layout = new StackLayout { Padding = 10 };
            BackgroundColor = Color.White;
            
            endTimeLabel = new Label();
            endTimeLabel.Text = timeReg.Time.ToString();

            timeButton = new Button { Text = "Meld ind", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            timeButton.Clicked += timeButton_Clicked;
            layout.Children.Add(endTimeLabel);
            layout.Children.Add(timeButton);

            Content = new ScrollView { Content = layout };
            
        }

     
        private async void timeButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);

                timeRegGlobal.Latitude = position.Latitude;
                timeRegGlobal.Longitude = position.Longitude;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get location, may need to increase timeout: " + ex);
            }
            endTimeLabel.Text = DateTime.Now.ToString();
            timeRegGlobal.Time = DateTime.Now;
                        
            await db.UpdateTimeRegAsync(timeRegGlobal);
        }
        
    }
}
