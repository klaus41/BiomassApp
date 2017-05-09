using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Views.CustomCells;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    class JobRecLineDetail : ContentPage
    {
        List<JobRecLineDetailModel> list = new List<JobRecLineDetailModel>();
        ListView lv;

        public JobRecLineDetail(JobRecLine c)
        {
            if (c.Job_No != null)
            {
                JobRecLineDetailModel jobNoModel = new JobRecLineDetailModel()
                {
                    type = "Sagsnummer",
                    value = c.Job_No.ToString()
                };
                list.Add(jobNoModel);
            }
            if (c.Journal_Template_Name != null)
            {
                JobRecLineDetailModel journalTemplateNameModel = new JobRecLineDetailModel()
                {
                    type = "Journal Template Name",
                    value = c.Journal_Template_Name
                };
                list.Add(journalTemplateNameModel);
            }
            if (c.Posting_Date > new DateTime(1950, 1, 1))
            {
                JobRecLineDetailModel postingDateModel = new JobRecLineDetailModel()
                {
                    type = "Posting Date",
                    value = c.Posting_Date.ToString("dd/MM/yyyy")
                };
                list.Add(postingDateModel);
            }
            if (c.Type != null)
            {
                JobRecLineDetailModel typeModel = new JobRecLineDetailModel()
                {
                    type = "Type",
                    value = c.Type
                };
                list.Add(typeModel);
            }
            if (c.No != null)
            {
                JobRecLineDetailModel noModel = new JobRecLineDetailModel()
                {
                    type = "Nummer",
                    value = c.No.ToString()
                };
                list.Add(noModel);
            }
            if (c.Description != null)
            {
                JobRecLineDetailModel descriptionModel = new JobRecLineDetailModel()
                {
                    type = "Beskrivelse",
                    value = c.Description
                };
                list.Add(descriptionModel);
            }
            JobRecLineDetailModel quantityModel = new JobRecLineDetailModel()
            {
                type = "Antal",
                value = c.Quantity.ToString()
            };
            list.Add(quantityModel);
            if (c.Unit_of_Measure_Code != null)
            {
                JobRecLineDetailModel unitOfMeasureCodeModel = new JobRecLineDetailModel()
                {
                    type = "Enhedskode",
                    value = c.Unit_of_Measure_Code
                };
                list.Add(unitOfMeasureCodeModel);
            }
            if (c.Work_Type_Code != null)
            {
                JobRecLineDetailModel workTypeCodeModel = new JobRecLineDetailModel()
                {
                    type = "Arbejdstype",
                    value = c.Work_Type_Code
                };
                list.Add(workTypeCodeModel);
            }
            if (c.Journal_Batch_Name != null)
            {
                JobRecLineDetailModel journalBatchNameModel = new JobRecLineDetailModel()
                {
                    type = "Batch Name",
                    value = c.Journal_Batch_Name
                };
                list.Add(journalBatchNameModel);
            }
            if (c.Job_Task_No != null)
            {
                JobRecLineDetailModel jobTaskNoModel = new JobRecLineDetailModel()
                {
                    type = "Opgavenummer",
                    value = c.Job_Task_No
                };
                list.Add(jobTaskNoModel);
            }
            if (c.MaintenanceTaskNo != null)
            {
                JobRecLineDetailModel maintenanceTaskNoModel = new JobRecLineDetailModel()
                {
                    type = "Vedligeholdsopgavenummer",
                    value = c.MaintenanceTaskNo
                };
                list.Add(maintenanceTaskNoModel);
            }
            if (c.Status != null)
            {
                JobRecLineDetailModel statusModel = new JobRecLineDetailModel()
                {
                    type = "Status",
                    value = c.Status
                };
                list.Add(statusModel);
            }
            if (c.WorkType != null)
            {
                JobRecLineDetailModel workTypeModel = new JobRecLineDetailModel()
                {
                    type = "Arbejdstype",
                    value = c.WorkType
                };
                list.Add(workTypeModel);
            }

            var temp = new DataTemplate(typeof(CustomTaskDetailCell));

            lv = new ListView()
            {
                HasUnevenRows = true,
                ItemTemplate = temp
            };

            lv.ItemsSource = list;

            Button button = new Button() { Text = "Tilbage", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            button.Clicked += Button_Clicked;
            StackLayout layout = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                    {
                        lv,
                        button
                    }

            };

            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            Content = layout;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
