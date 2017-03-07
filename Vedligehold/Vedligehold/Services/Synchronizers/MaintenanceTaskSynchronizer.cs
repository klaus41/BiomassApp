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

        public async Task<int[]> SyncDatabaseWithNAV()
        {
            List<MaintenanceTask> onlineList = new List<MaintenanceTask>();
            List<MaintenanceTask> taskList = await App.Database.GetTasksAsync();
            int numberOfConflicts = 0;
            int numberOfSyncs = 0;
            int numberOfMatches = 0;
            int numberOfNewTasks = 0;
            var sv = new MaintenanceService();
            var es = await sv.GetMaintenanceTasksAsync();

            foreach (var item in es)
            {
                onlineList.Add(item);
            }

            foreach (MaintenanceTask onlineTask in onlineList)
            {
                foreach (MaintenanceTask task in taskList)
                {
                    if (onlineTask.no == task.no)
                    {
                        numberOfMatches++;
                    }

                    if ((task.no == onlineTask.no) && (task.etag == onlineTask.etag))
                    {
                        if (task.done && (task.done != onlineTask.done))
                        {
                            var mts = new MaintenanceService();
                            await mts.UpdateTask(task);
                            numberOfSyncs++;
                            Debug.WriteLine("!!!!!!!!!!!!!!!!!! " + task.no);
                        }
                    }
                    else if ((task.no == onlineTask.no) && (task.etag != onlineTask.etag))
                    {
                        numberOfConflicts++;
                        await App.Database.UpdateTaskAsync(onlineTask);
                    }
                }
                if (numberOfMatches == 0)
                {
                    await App.Database.SaveTaskAsync(onlineTask);
                    numberOfNewTasks++;
                }
                numberOfMatches = 0;
            }
            int[] data = new int[3] { numberOfSyncs, numberOfConflicts, numberOfNewTasks };
            return data;
        }

    }
}
