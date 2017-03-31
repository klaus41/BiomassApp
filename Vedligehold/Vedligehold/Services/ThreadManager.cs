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
        public async void StartSynchronizationThread()
        {
            bool done = false;
            while (!done)
            {
               await Task.Factory.StartNew( async() =>
               {
                   await mts.SyncDatabaseWithNAV();
                   await trs.SyncDatabaseWithNAV();
                   await mas.SyncDatabaseWithNAV();
                   Debug.WriteLine("!!!!!!! SYNCED!!!!!");
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
