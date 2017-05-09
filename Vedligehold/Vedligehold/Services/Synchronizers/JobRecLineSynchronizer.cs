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

        ServiceFacade facade = ServiceFacade.GetInstance;
        MaintenanceDatabase db = App.Database;
        public async void SyncDatabaseWithNAV()
        {
            try
            {
                jobList = await db.GetJobRecLinesAsync();
                bool done = false;
                while (!done)
                {
                    var s = await facade.JobRecLineService.GetJobRecLines();
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
                        switch (item.Work_Type_Code)
                        {
                            case "KM":
                                item.WorkType = "Kilometer";
                                break;
                            case "REJSETID":
                                item.WorkType = "Rejsetid";
                                break;
                            case "STK":
                                item.WorkType = "Stk";
                                break;
                            case "TIMER":
                                item.WorkType = "Konsulenttimer";
                                break;
                        }
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

                    await facade.JobRecLineService.UpdateJobRecLine(item);
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
                    await facade.JobRecLineService.CreateJobRecLine(item);
                }
                newItem = true;
            }
        }
    }
}
