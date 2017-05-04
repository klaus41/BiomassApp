using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Vedligehold.Services.Synchronizers;
using Vedligehold.Views;

namespace Vedligehold.Services
{
    public class ThreadManager
    {
        MaintenanceTaskSynchronizer mts = new MaintenanceTaskSynchronizer();
        TimeRegistrationSynchronizer trs = new TimeRegistrationSynchronizer();
        MaintenanceActivitySynchronizer mas = new MaintenanceActivitySynchronizer();
        JobRecLineSynchronizer jrls = new JobRecLineSynchronizer();
        PictureSynchronizer ps = new PictureSynchronizer();
        public async void StartSynchronizationThread()
        {

            int i = 0;
            bool done = false;
            while (!done)
            {
               await Task.Run( async() =>
               {
                   await mts.SyncDatabaseWithNAV();
                   await trs.SyncDatabaseWithNAV();
                   await mas.SyncDatabaseWithNAV();
                    jrls.SyncDatabaseWithNAV();                   
                   ps.PutPicturesToNAV();
                   Debug.WriteLine(i + "!!!!!!! SYNCED!!!!!");
                   i++;
               });
               await Task.Delay(30000);
            }
        }
        private void DoWork()
        {
            Debug.WriteLine("AKK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!LSAKKLASDKLSADS");
        }
    }
}
