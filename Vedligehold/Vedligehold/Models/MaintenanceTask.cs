using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Vedligehold.Models
{
    public class MaintenanceTask
    {
        [PrimaryKey]
        public int no { get; set; }
        public string type { get; set; }
        public string anlæg { get; set; }
        public string anlægsbeskrivelse { get; set; }
        public string text { get; set; }
        public bool weekly { get; set; }
        public bool daily { get; set; }
        public string etag { get; set; }
        public bool done { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        //public bool synced { get; set; }


    }

    public class RootObject
    {
        public MaintenanceTask[] maintenanceTasks { get; set; }
    }
}
