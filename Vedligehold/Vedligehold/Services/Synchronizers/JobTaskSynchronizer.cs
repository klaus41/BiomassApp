using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;

namespace Vedligehold.Services.Synchronizers
{
    public class JobTaskSynchronizer
    {
        List<JobTask> onlineList = new List<JobTask>();
        List<JobTask> jobList = new List<JobTask>();

        ServiceFacade facade = ServiceFacade.GetInstance;
        MaintenanceDatabase db = App.Database;
        public async void SyncDatabaseWithNAV()
        {
            try
            {
                bool done = false;
                while (!done)
                {
                    jobList = await db.GetJobTasksAsync();
                    var s = await facade.JobTaskService.GetJobTasksAsync();
                    foreach (var item in s)
                    {
                        onlineList.Add(item);
                    }
                    done = true;
                }
                foreach (var item in onlineList)
                {
                    try
                    {
                        item.UniqueID = item.Job_No + item.Job_Task_No;
                        await db.SaveJobTaskAsync(item);
                    }
                    catch
                    {

                    }
                }
            }
            catch { }
        }
    }
}
