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
        //public Guid MaintTaskGUID { get; set; }
        public int no { get; set; }
        public string TaskType { get; set; }
        public string MaintenanceType { get; set; }
        public string anlæg { get; set; }
        public string anlægsbeskrivelse { get; set; }
        public string text { get; set; }
        public bool weekly { get; set; }
        public bool daily { get; set; }
        public string etag { get; set; }
        public string status { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public DateTime planned_Date { get; set; }
        public string responsible { get; set; }
        public byte[] image { get; set; }
        public string AppNotes { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string JobNo { get; set; }
        public string JobTaskNo { get; set; }
        public bool New { get; set; }
        public bool Sent { get; set; }

    }

    public class RootObject
    {
        public MaintenanceTask[] maintenanceTasks { get; set; }
    }
}
