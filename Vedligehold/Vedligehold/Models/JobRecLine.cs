using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class JobRecLine
    {
        [PrimaryKey ]
        public Guid JobRecLineGUID { get; set; }
        public string Job_No { get; set; }
        public string Journal_Template_Name { get; set; }
        public double Line_No { get; set; }
        public DateTime Posting_Date { get; set; }
        public string Type { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public double Quantity { get; set; }
        public string Unit_of_Measure_Code { get; set; }
        public string Work_Type_Code { get; set; }
        public string Journal_Batch_Name { get; set; }
        public string Job_Task_No { get; set; }
        public double Job_Planning_Line_No { get; set; }
        public string UniqueID { get; set; }
        public string MaintenanceTaskNo { get; set; }
        public string Status { get; set; }
        public string ETag { get; set; }
        public string WorkType { get; set; }
        public bool Edited { get; set; }
    }
}
