using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;

namespace Vedligehold.Services.Synchronizers
{
    public class JobRecLineSynchronizer
    {
        List<JobRecLine> onlineList = new List<JobRecLine>();
        List<JobRecLine> jobList = new List<JobRecLine>();

        JobRecLineService ms = new JobRecLineService();

        MaintenanceDatabase db = App.Database;
        public async void SyncDatabaseWithNAV()
        {
            try
            {
                bool done = false;
                while (!done)
                {
                    jobList = await db.GetJobRecLinesAsync();
                    var s = await ms.GetJobRecLines();
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
                        await db.SaveJobRecLineAsync(item);
                    }
                    catch
                    {

                    }
                }
                CreateNewJobRecLines();
                UpdateJobRecLines();
            }
            catch { }
        }

        private async void UpdateJobRecLines()
        {
            foreach (var item in jobList)
            {
                if (item.Edited)
                {

                    await ms.UpdateJobRecLine(item);
                    item.Edited = false;
                    await db.UpdateJobRecLineAsync(item);
                }
            }
        }

        private async void CreateNewJobRecLines()
        {
            bool newItem = true;

            foreach (var item in jobList)
            {
                foreach (var s in onlineList)
                {
                    if (item.JobRecLineGUID == s.JobRecLineGUID)
                    {
                        newItem = false;
                    }
                }
                if (newItem)
                {
                    await ms.CreateJobRecLine(item);
                }
                newItem = true;
            }
        }
    }
}
