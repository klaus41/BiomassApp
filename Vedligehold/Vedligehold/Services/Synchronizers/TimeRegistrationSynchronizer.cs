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
    public class TimeRegistrationSynchronizer
    {
        bool done;
        List<TimeRegistrationModel> timeList;
        List<TimeRegistrationModel> onlineList;

        ServiceFacade facade = ServiceFacade.GetInstance;
        MaintenanceDatabase db = App.Database;

        public async void DeleteAndPopulateDb()
        {
            List<TimeRegistrationModel> onlineList = new List<TimeRegistrationModel>();
            List<TimeRegistrationModel> oldlist = await App.Database.GetTimeRegsAsync();
            await App.Database.DeleteAllTimeReg();
            List<TimeRegistrationModel> taskList = await App.Database.GetTimeRegsAsync();
            if (!taskList.Any())
            {
                var es = await facade.TimeRegistrationService.GetTimeRegsAsync();

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
                    var es = await facade.TimeRegistrationService.GetTimeRegsAsync();
                    onlineList = new List<TimeRegistrationModel>();

                    foreach (var item in es)
                    {
                        onlineList.Add(item);
                    }

                    CheckIfDeleted();
                    CreateTimeRegs();
                    if (timeList.Count == 0)
                    {
                        GetTimeRegs();
                    }
                }
            }
            catch
            {
                done = true;
            }
            return done;
        }

        private void GetTimeRegs()
        {
            foreach (var onlineTimeReg in onlineList)
            {
                onlineTimeReg.Sent = true;
                onlineTimeReg.New = false;
                db.SaveTimeRegAsync(onlineTimeReg);
            }
        }

        private async void CheckIfDeleted()
        {
            foreach (var timeReg in timeList)
            {
                int matches = 0;
                foreach (var onlineTimeReg in onlineList)
                {
                    if (timeReg.No == onlineTimeReg.No)
                    {
                        matches++;
                    }
                }
                if (matches == 0 && timeReg.Sent && !timeReg.New)
                {
                    await db.DeleteTimeRegAsync(timeReg);
                }
            }
        }

        private async void CreateTimeRegs()
        {
            foreach (var timeReg in timeList)
            {
                int matches = 0;
                foreach (var onlineTimeReg in onlineList)
                {
                    if (timeReg.No == onlineTimeReg.No)
                    {
                        matches++;
                    }
                }
                if (matches == 0 && !timeReg.Sent && timeReg.New)
                {
                    await facade.TimeRegistrationService.CreateTimeReg(timeReg);
                    timeReg.Sent = true;
                    timeReg.New = false;
                    await db.UpdateTimeRegAsync(timeReg);
                }
            }
            done = true;
        }
    }
}
