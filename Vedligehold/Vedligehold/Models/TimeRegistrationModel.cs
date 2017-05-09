using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class TimeRegistrationModel
    {
        [PrimaryKey, AutoIncrement]
        public int No { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
        public string User { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ETag { get; set; }
        public bool New { get; set; }
        public bool Sent { get; set; }

    }
}
