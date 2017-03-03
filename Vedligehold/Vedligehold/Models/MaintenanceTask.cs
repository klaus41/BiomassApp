using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class MaintenanceTask
    {
        public int no { get; set; }
        public string type { get; set; }
        public string anlæg { get; set; }
        public string anlægsbeskrivelse { get; set; }
        public string text { get; set; }
        public bool weekly { get; set; }
        public bool daily { get; set; }
        private string etag { get; }
        public bool done { get; set; }

    }

    public class RootObject
    {
        public MaintenanceTask[] maintenanceTasks { get; set; }
    }
}
