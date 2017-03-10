using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;
using Vedligehold.Services;

namespace Vedligehold.Views
{
    public class MaintenanceTaskSynchronizer
    {
        List<MaintenanceTask> onlineList;
        List<MaintenanceTask> taskList;
        int numberOfConflicts;
        int numberOfSyncs;
        int numberOfMatches;
        int numberOfNewTasks;
        MaintenanceService ms;
        bool done;

        public async Task<bool> HasConnectionToNAV()
        {
            bool connection;
            try
            {
                var service = new PDFService();
                string data = await service.GetPDF("A00005");
                connection = true;
            }
            catch
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
                var sv = new MaintenanceService();
                var es = await sv.GetMaintenanceTasksAsync();

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
                    ms = new MaintenanceService();
                    var es = await ms.GetMaintenanceTasksAsync();
                    onlineList = new List<MaintenanceTask>();

                    foreach (var item in es)
                    {
                        onlineList.Add(item);
                    }


                    PutDoneTasksToNAV();
                    CheckForConflicts();
                    CheckForNewTasks();
                    PushNewTasks();
                }
            }
            catch
            {
                done = true;
            }
            return done;
        }

        public async void DeleteDB()
        {
            await App.Database.DeleteAll();
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
                    Debug.WriteLine("NUMBER OF NEW TASKS " + numberOfNewTasks + "!!!!!!!!!!!!!!!!!! " + onlineTask.no);

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
                        Debug.WriteLine("NUMBER OF CONFLICTS !!!!!!!!!!!!!!!!!! " + numberOfConflicts + task.no);

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
                        if (task.done && !onlineTask.done)
                        {
                            var mts = new MaintenanceService();
                            await mts.UpdateTask(task);
                            numberOfSyncs++;
                            Debug.WriteLine("NUMBER OF SYNCS !!!!!!!!!!!!!!!!!! " + numberOfSyncs + task.no);
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
                    var mts = new MaintenanceService();
                    await mts.CreateTask(task);
                }
            }
            done = true;
        }
    }
}
