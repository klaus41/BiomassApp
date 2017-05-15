using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Vedligehold.Database;
using Vedligehold.Models;
using Vedligehold.Services;
using Xamarin.Forms;

namespace Vedligehold.Views
{
    public class MaintenanceTaskForm : ContentPage
    {
        MaintenanceDatabase db = App.Database;

        Button taskTypeButton;
        Button maintenanceTypeButton;
        Button customerNoButton;
        Button createButton;

        Entry textEntry;

        string taskType;
        string status;

        DatePicker plannedDateDatePicker;

        MaintenanceTask task;
        List<MaintenanceTask> taskList;
        List<Customer> customerList;
        public MaintenanceTaskForm()
        {
            task = new MaintenanceTask()
            {
                AppNotes = "Oprettet via app",
                status = "Released"
            };

            taskTypeButton = new Button() { Text = "Opgavetype", BackgroundColor = Color.Red, TextColor = Color.White };
            maintenanceTypeButton = new Button() { Text = "Vedligeholdstype", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };
            customerNoButton = new Button() { Text = "Kunde", BackgroundColor = Color.Red, TextColor = Color.White };
            createButton = new Button() { Text = "Opret Opgave", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White };

            taskTypeButton.Clicked += TaskTypeButton_Clicked;
            createButton.Clicked += CreateButton_Clicked;
            customerNoButton.Clicked += CustomerNoButton_Clicked;
            Content = new StackLayout
            {
                Children = {
                    taskTypeButton,
                    customerNoButton,
                    createButton
                }
            };
        }

        private async void CustomerNoButton_Clicked(object sender, EventArgs e)
        {
            customerList = customerList.OrderByDescending(x => x.Name).ToList();
            string[] customerArray = new string[customerList.Count()];
            for (int i = 0; i < customerList.Count(); i++)
            {
                customerArray[i] = customerList.ElementAt(i).Name;
            }
            string selection = await DisplayActionSheet("Kunder", "Cancel", null, customerArray);
            if (selection != "Cancel")
            {
                task.CustomerNo = customerList.Where(x => x.Name == selection).FirstOrDefault().No;
                customerNoButton.Text = "Kunde (" + selection + ")";
                customerNoButton.BackgroundColor = Color.FromRgb(135, 206, 250);
            }
        }

        private async void CreateButton_Clicked(object sender, EventArgs e)
        {
            SetValues();
            await db.SaveTaskAsync(task);
        }

        private void SetValues()
        {
            task.no = taskList.OrderByDescending(x => x.no).FirstOrDefault().no + 1;
            task.CustomerNo = "10009";
            task.planned_Date = DateTime.Today;
            task.MaintenanceType = "Rundering";
            task.text = "Test fra app";
            task.JobNo = "INTERN";
            task.JobTaskNo = "10020";
            task.New = true;
            task.Sent = false;
        }

        private async void TaskTypeButton_Clicked(object sender, EventArgs e)
        {
            string[] options = new string[6] { "Vedligehold", "Sag", "CRM", "Produktion", "Service", "Lager" };
            taskType = await DisplayActionSheet("Arbejdstype", "Cancel", null, options);

            task.TaskType = taskType;
            taskTypeButton.Text = "arbejdstype (" + taskType + ")";
            taskTypeButton.BackgroundColor = Color.FromRgb(135, 206, 250);
        }

        protected async override void OnAppearing()
        {
            customerList = await db.GetCustomersAsync();
            taskList = await db.GetTasksAsync();
            base.OnAppearing();
        }
    }
}
