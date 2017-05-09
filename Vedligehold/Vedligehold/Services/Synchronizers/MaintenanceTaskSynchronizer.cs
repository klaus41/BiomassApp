using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services;

namespace Vedligehold.Services.Synchronizers
{
    public class MaintenanceTaskSynchronizer
    {
        List<MaintenanceTask> onlineList;
        List<MaintenanceTask> taskList;
        int numberOfConflicts;
        int numberOfSyncs;
        int numberOfMatches;
        int numberOfNewTasks;
        bool done;
        ServiceFacade facade = ServiceFacade.GetInstance;

        public async Task<bool> HasConnectionToNAV()
        {
            bool connection;
            SalesPerson[] persons = null;
            try
            {
                persons = await facade.SalesPersonService.GetSalesPersonsAsync();
            }
            catch
            {
                connection = false;
            }
            if (persons != null)
            {
                connection = true;
            }
            else
            {
                connection = false;
            }
            return connection;
        }
        public async Task<int[]> DeleteAndPopulateDb()
        {
            List<MaintenanceTask> onlineList = new List<MaintenanceTask>();
            List<MaintenanceTask> oldlist = await App.Database.GetTasksAsync();
            await App.Database.DeleteAll();
            List<MaintenanceTask> taskList = await App.Database.GetTasksAsync();
            if (!taskList.Any())
            {
                var es = await facade.MaintenanceService.GetMaintenanceTasksAsync();

                foreach (var item in es)
                {
                    await App.Database.SaveTaskAsync(item);
                    onlineList.Add(item);
                }
            }
            int[] data = new int[2] { oldlist.Count(), onlineList.Count() };
            return data;
        }

        public async Task<bool> SyncDatabaseWithNAV()
        {
            done = false;
            taskList = await App.Database.GetTasksAsync();
            try
            {
                while (!done)
                {
                    var es = await facade.MaintenanceService.GetMaintenanceTasksAsync();
                    onlineList = new List<MaintenanceTask>();

                    foreach (var item in es)
                    {
                        onlineList.Add(item);
                    }

                    CheckIfFinishedOrDeleted();
                    PutDoneTasksToNAV();
                    CheckForConflicts();
                    CheckForNewTasks();
                    //PushNewTasks();
                    PutTextChangedTasks();
                }
            }
            catch
            {
                done = true;
            }
            return done;
        }

        private async void PutTextChangedTasks()
        {
            foreach (MaintenanceTask onlineTask in onlineList)
            {
                foreach (MaintenanceTask task in taskList)
                {
                    if ((task.no == onlineTask.no) && (task.etag == onlineTask.etag))
                    {
                        if (task.AppNotes != onlineTask.AppNotes)
                        {
                            await facade.MaintenanceService.UpdateTask(task);
                            numberOfSyncs++;
                        }
                    }
                }
            }
            done = true;

        }

        public async void DeleteDB()
        {
            await App.Database.DeleteAll();
            await App.Database.DeleteAllTimeReg();
            await App.Database.DeleteJobRecLinesAsync();
            await App.Database.DeleteAllActivities();
        }

        private async void CheckForNewTasks()
        {
            foreach (MaintenanceTask onlineTask in onlineList)
            {
                foreach (MaintenanceTask task in taskList)
                {
                    if (task.no == onlineTask.no)
                    {
                        numberOfMatches++;
                    }
                }
                if (numberOfMatches == 0)
                {
                    await App.Database.SaveTaskAsync(onlineTask);
                    numberOfNewTasks++;

                }
                numberOfMatches = 0;
            }
        }

        private async void CheckForConflicts()
        {
            foreach (MaintenanceTask onlineTask in onlineList)
            {
                foreach (MaintenanceTask task in taskList)
                {
                    if ((task.no == onlineTask.no) && (task.etag != onlineTask.etag))
                    {
                        numberOfConflicts++;
                        await App.Database.UpdateTaskAsync(onlineTask);
                    }
                }
            }
        }
        private async void PutDoneTasksToNAV()
        {
            foreach (MaintenanceTask onlineTask in onlineList)
            {
                foreach (MaintenanceTask task in taskList)
                {
                    if ((task.no == onlineTask.no) && (task.etag == onlineTask.etag))
                    {
                        if (task.status == "Completed" && onlineTask.status == "Released")
                        {
                            await facade.MaintenanceService.UpdateTask(task);
                            numberOfSyncs++;
                        }
                    }
                }
            }
        }
        private async void PushNewTasks()
        {
            foreach (MaintenanceTask task in taskList)
            {
                int i = 0;
                foreach (MaintenanceTask onlineTask in onlineList)
                {
                    if (task.no == onlineTask.no)
                    {
                        i++;
                    }
                }
                if (i == 0)
                {
                    await facade.MaintenanceService.CreateTask(task);
                }
            }
        }
        private async void CheckIfFinishedOrDeleted()
        {
            foreach (MaintenanceTask task in taskList)
            {
                int matches = 0;
                foreach (MaintenanceTask onlineTask in onlineList)
                {
                    if (task.no == onlineTask.no)
                    {
                        matches++;
                    }
                }
                if (matches == 0)
                {
                    await App.Database.DeleteTaskAsync(task);
                }
            }
        }
    }
}
