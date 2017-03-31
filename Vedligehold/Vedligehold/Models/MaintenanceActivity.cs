using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class MaintenanceActivity
    {
        [PrimaryKey]
        public string UniqueID { get; set; }
        public string Task_No { get; set; } 
        public int Maint_Activity_No { get; set; }
        public string Activity_Description { get; set; }
        public double Reading { get; set; }
        public double Min_reading { get; set; }
        public double Max_reading { get; set; }
        public bool Done { get; set; }
        public bool Alarm { get; set; }
        public string Status { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ETag { get; set; }
        public DateTime DoneTime { get; set; }
    }
}
