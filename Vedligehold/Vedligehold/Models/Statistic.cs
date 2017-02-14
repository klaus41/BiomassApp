using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class Statistic
    {
        public int N { get; set; }
        public string mdr { get; set; }
        public double gns_torstof_raa { get; set; }
        public double gns_torstof_afg { get; set; }
        public double maengde_afg { get; set; }
        public double maengde_raa { get; set; }
        public string SupplNo { get; set; }
        public string ETag { get; set; }

        public string Summary
        {
            get { return string.Format("Supplier Number: {0}, Month {1}", SupplNo, mdr); }
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}", gns_torstof_afg, gns_torstof_raa, maengde_afg, maengde_raa);
        }
    }

    public class Rootobject
    {
        public Statistic[] statistics { get; set; }
    }
}

