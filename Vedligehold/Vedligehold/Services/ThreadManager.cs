using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vedligehold.Services.Synchronizers;
using Vedligehold.Views;

namespace Vedligehold.Services
{
    public class ThreadManager
    {
        SynchronizerFacade facade = SynchronizerFacade.GetInstance;
    
        public async void StartSynchronizationThread()
        {

            int i = 0;
            bool done = false;
            while (!done)
            {
                await Task.Run(async () =>
               {
                   await facade.MaintenanceTaskSynchronizer.SyncDatabaseWithNAV();
                   await facade.TimeRegistrationSynchronizer.SyncDatabaseWithNAV();
                   await facade.MaintenanceActivitySynchronizer.SyncDatabaseWithNAV();
                   facade.JobRecLineSynchronizer.SyncDatabaseWithNAV();
                   facade.PictureSynchronizer.PutPicturesToNAV();
                   facade.ResourcesSynchronizer.SyncDatabaseWithNAV();
                   Debug.WriteLine(i + "!!!!!!! SYNCED!!!!!");
                   i++;
               });
                await Task.Delay(30000);
            }
        }
    }
}
