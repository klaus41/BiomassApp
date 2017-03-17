using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Vedligehold.Views
{
    public partial class SearchSettingsPage : ContentPage
    {
        GlobalData gd = GlobalData.GetInstance;
        Entry userNameEntry;
        DatePicker datePicker;
        Button reset;
        public SearchSettingsPage()
        {
            DateTime today = DateTime.Today;
            userNameEntry = new Entry()
            {
                Text = gd.SearchUserName
            };
            datePicker = new DatePicker()
            {
                Format = "D",
                Date = gd.SearchDateTime,
                MaximumDate = today.AddDays(7)
            };
            reset = new Button { Text = "Nulstil", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            reset.Clicked += Reset_Clicked;
            datePicker.DateSelected += DatePicker_DateSelected;
            userNameEntry.TextChanged += UserNameEntry_TextChanged;
            Content = new StackLayout
            {
                Children =
                {
                    userNameEntry,
                    datePicker,
                    reset
                }
            };
        }

        private void Reset_Clicked(object sender, EventArgs e)
        {
            gd.SearchDateTime = new DateTime(1800, 1, 1);
            gd.SearchUserName = null;
            Navigation.PopModalAsync();
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            gd.SearchDateTime = datePicker.Date;
        }

        private void UserNameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (userNameEntry.Text == "")
            {
                gd.SearchUserName = null;
            }
            else
            {
                gd.SearchUserName = userNameEntry.Text;
            }
        }
    }
}
