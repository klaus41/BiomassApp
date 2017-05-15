using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class JobRecLineUpdateForm : ContentPage
    {
        StackLayout layout;

        string workType = null;

        Entry descriptionEntry;
        Entry amount;
        Button workTypeButton;
        Button unitCodeButton;
        Button done;
        Button cancel;

        DatePicker datePicker;

        JobRecLine recLine;
        JobRecLine recLineGlobal;
        public JobRecLineUpdateForm(JobRecLine jobRecLine)
        {
            recLine = new JobRecLine();
            //taskGlobal = task;
            recLineGlobal = jobRecLine;
            workType = jobRecLine.WorkType;

            descriptionEntry = new Entry { Text = recLineGlobal.Description };
            amount = new Entry { Text = recLineGlobal.Quantity.ToString(CultureInfo.InvariantCulture), Keyboard = Keyboard.Numeric };

            workTypeButton = new Button { Text = "Arbejdstype (" + recLineGlobal.WorkType + ")", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            unitCodeButton = new Button { Text = "Enhedskode", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            done = new Button { Text = "OK", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            cancel = new Button { Text = "Cancel", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, VerticalOptions = LayoutOptions.EndAndExpand };
            datePicker = new DatePicker()
            {
                Format = "D",
                Date = recLineGlobal.Posting_Date
            };
            workTypeButton.Clicked += WorkTypeButton_Clicked;
            done.Clicked += Done_Clicked;
            cancel.Clicked += Cancel_Clicked;
            layout = new StackLayout
            {
                Children =
                {
                    datePicker,
                    workTypeButton,
                    descriptionEntry,
                    amount,
                    done,
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

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void Done_Clicked(object sender, EventArgs e)
        {
            if (workType == null || amount.Text == "")
            {
                DisplayAlert("Fejl!", "Du skal oplyse både en arbejdstype og antal", "OK");
            }
            else
            {
                SetRecLineValues();
                Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + recLine.JobRecLineGUID);
                var response = App.Database.UpdateJobRecLineAsync(recLineGlobal);

                Navigation.PopModalAsync();
            }
        }

        private void SetRecLineValues()
        {
            double temp;
            Double.TryParse(amount.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out temp);
            recLineGlobal.Description = descriptionEntry.Text;
            recLineGlobal.Posting_Date = datePicker.Date;
            recLineGlobal.Quantity = temp;
            recLineGlobal.Edited = true;
        }

        private async void WorkTypeButton_Clicked(object sender, EventArgs e)
        {
            string[] options = new string[4] { "Kørsel", "Rejsetid", "Stk", "Konsulenttimer" };
            workType = await DisplayActionSheet("Arbejdstype", "Cancel", null, options);

            recLineGlobal.WorkType = workType;
            workTypeButton.Text = "arbejdstype (" + workType + ")";

            switch (workType)
            {
                case "Kørsel":
                    recLineGlobal.Unit_of_Measure_Code = "KM";
                    recLineGlobal.Work_Type_Code = "KM";
                    break;
                case "Rejsetid":
                    recLineGlobal.Unit_of_Measure_Code = "TIMER";
                    recLineGlobal.Work_Type_Code = "REJSETID";
                    break;
                case "Stk":
                    recLineGlobal.Unit_of_Measure_Code = "STK";
                    recLineGlobal.Work_Type_Code = "STK";
                    break;
                case "Konsulenttimer":
                    recLineGlobal.Unit_of_Measure_Code = "TIMER";
                    recLineGlobal.Work_Type_Code = "TIMER";
                    break;
            }
        }
    }
}
