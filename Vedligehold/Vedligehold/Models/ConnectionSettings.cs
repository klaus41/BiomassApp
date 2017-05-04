using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class ConnectionSettings
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string BaseAddress { get; set; }
        public string LastUser { get; set; }
    }
}
