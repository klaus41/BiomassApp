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
        Button customerNoButton;
        Button createButton;
        Button maintenanceTypeButton;
        Button jobNoButton;
        Button taskJobNoButton;
        Button backButton;

        Entry textEntry;

        DatePicker plannedDatePicker;

        string taskType;
        string status;

        MaintenanceTask task;
        List<MaintenanceTask> taskList;
        List<Customer> customerList;
        List<Job> jobList;
        List<Job> tempJobList;
        List<JobTask> jobTaskList;
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
            jobNoButton = new Button() { Text = "Sag", BackgroundColor = Color.Red, TextColor = Color.White, IsEnabled = false };
            taskJobNoButton = new Button() { Text = "sagsopgave", BackgroundColor = Color.Red, TextColor = Color.White, IsEnabled = false };
            createButton = new Button() { Text = "Opret Opgave", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White, IsEnabled = false };
            backButton = new Button() { Text = "Tilbage", BackgroundColor = Color.FromRgb(135, 206, 250), TextColor = Color.White};

            taskTypeButton.Clicked += TaskTypeButton_Clicked;
            customerNoButton.Clicked += CustomerNoButton_Clicked;
            jobNoButton.Clicked += JobNoButton_Clicked;
            taskJobNoButton.Clicked += TaskJobNoButton_Clicked;
            createButton.Clicked += CreateButton_Clicked;
            backButton.Clicked += BackButton_Clicked;

            textEntry = new Entry() { Placeholder = "Opgavebeskrivelse" };

            plannedDatePicker = new DatePicker() { Format = "D", Date = DateTime.Today };
            plannedDatePicker.DateSelected += PlannedDatePicker_DateSelected;

            StackLayout layout = new StackLayout
            {
                Children = {
                    plannedDatePicker,
                    taskTypeButton,
                    customerNoButton,
                    jobNoButton,
                    //taskJobNoButton,
                    textEntry,
                    createButton,
                    backButton
                }
            };
            if (Device.OS == TargetPlatform.iOS)
            {
                // move layout under the status bar
                layout.Padding = new Thickness(0, 20, 0, 0);
            }
            Content = layout;

        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void TaskJobNoButton_Clicked(object sender, EventArgs e)
        {
            jobTaskList = jobTaskList.OrderByDescending(x => x.UniqueID).ToList();
            string[] jobTaskArray = new string[jobTaskList.Count()];
            for (int i = 0; i < jobTaskList.Count(); i++)
            {
                jobTaskArray[i] = jobTaskList.ElementAt(i).Description;
            }
            string selection = await DisplayActionSheet("Sagsopgaver", "Cancel", null, jobTaskArray);
            if (selection != "Cancel")
            {
                task.JobTaskNo = jobTaskList.Where(x => x.Description == selection).FirstOrDefault().Job_Task_No;
                textEntry.Text = selection;
                taskJobNoButton.Text = "Sagsopgave (" + selection + ")";
                taskJobNoButton.BackgroundColor = Color.FromRgb(135, 206, 250);
                createButton.IsEnabled = true;
            }
        }

        private async void JobNoButton_Clicked(object sender, EventArgs e)
        {
            tempJobList = tempJobList.OrderByDescending(x => x.No).ToList();
            string[] jobArray = new string[tempJobList.Count()];
            for (int i = 0; i < tempJobList.Count(); i++)
            {
                jobArray[i] = tempJobList.ElementAt(i).Description;
            }
            string selection = await DisplayActionSheet("Sager", "Cancel", null, jobArray);
            if (selection != "Cancel")
            {
                task.JobNo = jobList.Where(x => x.Description == selection).FirstOrDefault().No;
                jobNoButton.Text = "Sag (" + selection + ")";
                jobNoButton.BackgroundColor = Color.FromRgb(135, 206, 250);
                taskJobNoButton.IsEnabled = true;
                jobTaskList = jobTaskList.Where(x => x.Job_No == task.JobNo).ToList();
            }
        }

        private void PlannedDatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            task.planned_Date = plannedDatePicker.Date;
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
                jobNoButton.IsEnabled = true;
                createButton.IsEnabled = true;
                tempJobList = jobList.Where(x => x.Bill_to_Customer_No == task.CustomerNo).ToList();
            }
        }

        private async void CreateButton_Clicked(object sender, EventArgs e)
        {
            SetValues();
            await db.SaveTaskAsync(task);
            await Navigation.PopModalAsync();
        }

        private void SetValues()
        {
            task.no = taskList.OrderByDescending(x => x.no).FirstOrDefault().no + 1;
            //task.CustomerNo = "10009";
            task.planned_Date = plannedDatePicker.Date;
            //task.MaintenanceType = "Rundering";
            task.text = textEntry.Text;
            //task.JobNo = "INTERN";
            //task.JobTaskNo = "10020";
            task.New = true;
            task.Sent = false;
        }

        private async void TaskTypeButton_Clicked(object sender, EventArgs e)
        {
            string[] options = new string[6] { "Vedligehold", "Sag", "CRM", "Produktion", "Service", "Lager" };
            taskType = await DisplayActionSheet("Arbejdstype", "Cancel", null, options);
            if (taskType != "Cancel")
            {
                task.TaskType = taskType;
                taskTypeButton.Text = "arbejdstype (" + taskType + ")";
                taskTypeButton.BackgroundColor = Color.FromRgb(135, 206, 250);
            }
        }

        protected async override void OnAppearing()
        {
            customerList = await db.GetCustomersAsync();
            taskList = await db.GetTasksAsync();
            jobList = await db.GetJobsAsync();
            jobTaskList = await db.GetJobTasksAsync();
            base.OnAppearing();
        }
    }
}
