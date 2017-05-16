using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class Job
    {
        [PrimaryKey]
        public string No { get; set; }
        public string Description { get; set; }
        public string Bill_to_Customer_No { get; set; }
    }
}
