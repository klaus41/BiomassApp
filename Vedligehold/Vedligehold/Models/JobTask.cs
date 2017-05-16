using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class JobTask
    {
        [PrimaryKey]
        public string UniqueID { get; set; }
        public string Job_No { get; set; }
        public string Job_Task_No { get; set; }
        public string Description { get; set; }
        public string Job_Task_Type { get; set; }
        public string ETag { get; set; }
    }
}
