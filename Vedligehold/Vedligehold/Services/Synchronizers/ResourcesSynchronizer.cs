using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Database;
using Vedligehold.Models;

namespace Vedligehold.Services.Synchronizers
{
    public class ResourcesSynchronizer
    {
        List<Resources> onlineList = new List<Resources>();
        List<Resources> resourcesList = new List<Resources>();

        ServiceFacade facade = ServiceFacade.GetInstance;
        MaintenanceDatabase db = App.Database;
        public async void SyncDatabaseWithNAV()
        {
            try
            {
                bool done = false;
                while (!done)
                {
                    resourcesList = await db.GetResourcesAsync();
                    var s = await facade.ResourcesService.GetResourcesAsync();
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
                        await db.SaveResourcesAsync(item);
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
