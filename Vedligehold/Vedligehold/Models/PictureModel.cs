using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class PictureModel
    {
        [PrimaryKey]
        public string id { get; set; }
        public string Picture { get; set; }
    }
}
