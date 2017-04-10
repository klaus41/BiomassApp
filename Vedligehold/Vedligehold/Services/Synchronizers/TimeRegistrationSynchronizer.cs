using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;

namespace Vedligehold.Services.Synchronizers
{
    public class TimeRegistrationSynchronizer
    {
        bool done;
        List<TimeRegistrationModel> timeList;
        List<TimeRegistrationModel> onlineList;
        TimeRegistrationService ts;

        int numberOfConflicts;
        int numberOfSyncs;
        int numberOfMatches;
        int numberOfNewTasks;

        public async void DeleteAndPopulateDb()
        {
            List<TimeRegistrationModel> onlineList = new List<TimeRegistrationModel>();
            List<TimeRegistrationModel> oldlist = await App.Database.GetTimeRegsAsync();
            await App.Database.DeleteAllTimeReg();
            List<TimeRegistrationModel> taskList = await App.Database.GetTimeRegsAsync();
            if (!taskList.Any())
            {
                var sv = new TimeRegistrationService();
                var es = await sv.GetTimeRegsAsync();

                foreach (var item in es)
                {
                    await App.Database.SaveTimeRegAsync(item);
                    onlineList.Add(item);
                }
            }
        }
        public async Task<bool> SyncDatabaseWithNAV()
        {
            done = false;
            timeList = await App.Database.GetTimeRegsAsync();
            try
            {
                while (!done)
                {
                    ts = new TimeRegistrationService();
                    var es = await ts.GetTimeRegsAsync();
                    onlineList = new List<TimeRegistrationModel>();

                    foreach (var item in es)
                    {
                        onlineList.Add(item);
                    }

                    CheckIfFinishedOrDeleted();
                    PutDoneTimeRegsToNAV();
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

        private async void PutDoneTimeRegsToNAV()
        {
            DateTime datetime = new DateTime(1950, 1, 1);

            foreach (TimeRegistrationModel onlineTimeReg in onlineList)
            {
                foreach (TimeRegistrationModel timeReg in timeList)
                {
                    if ((timeReg.No == onlineTimeReg.No) && (timeReg.ETag == onlineTimeReg.ETag))
                    {
                        if (timeReg.Time > datetime)
                        {
                            var ts = new TimeRegistrationService();
                            await ts.UpdateTimeReg(timeReg);
                            numberOfSyncs++;
                            Debug.WriteLine("NUMBER OF SYNCS !!!!!!!!!!!!!!!!!! " + numberOfSyncs + timeReg.No);
                        }
                    }
                }
            }
        }
        private async void CheckIfFinishedOrDeleted()
        {
            foreach (TimeRegistrationModel timereg in timeList)
            {
                int matches = 0;
                foreach (TimeRegistrationModel onlineTask in onlineList)
                {
                    if (timereg.No == onlineTask.No)
                    {
                        matches++;
                    }
                }
                if (matches == 0)
                {
                    await App.Database.DeleteTimeRegAsync(timereg);
                }
            }
        }
        private async void CheckForNewTasks()
        {
            foreach (TimeRegistrationModel onlineTask in onlineList)
            {
                foreach (TimeRegistrationModel task in timeList)
                {
                    if (task.No == onlineTask.No)
                    {
                        numberOfMatches++;
                    }
                }
                if (numberOfMatches == 0)
                {
                    await App.Database.SaveTimeRegAsync(onlineTask);
                    numberOfNewTasks++;
                    Debug.WriteLine("NUMBER OF NEW TASKS " + numberOfNewTasks + "!!!!!!!!!!!!!!!!!! " + onlineTask.No);

                }
                numberOfMatches = 0;
            }
        }
        private async void CheckForConflicts()
        {
            foreach (TimeRegistrationModel onlineTask in onlineList)
            {
                foreach (TimeRegistrationModel task in timeList)
                {
                    if ((task.No == onlineTask.No) && (task.ETag != onlineTask.ETag))
                    {
                        numberOfConflicts++;
                        await App.Database.UpdateTimeRegAsync(onlineTask);
                        Debug.WriteLine("NUMBER OF CONFLICTS !!!!!!!!!!!!!!!!!! " + numberOfConflicts + task.No);

                    }
                }
            }
        }

        private async void PushNewTasks()
        {
            foreach (TimeRegistrationModel task in timeList)
            {
                int i = 0;
                foreach (TimeRegistrationModel onlineTask in onlineList)
                {
                    if (task.No == onlineTask.No)
                    {
                        i++;
                    }
                }
                if (i == 0)
                {
                    var mts = new TimeRegistrationService();
                    await mts.CreateTimeReg(task);
                }
            }
            done = true;
        }
    }
}
