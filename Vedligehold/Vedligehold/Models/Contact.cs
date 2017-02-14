using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vedligehold.Models
{
    public class Contact
    {
        public string no { get; set; }
        public string name { get; set; }
        public string company_Name { get; set; }
        public string address { get; set; }
        public string postCode { get; set; }
        public string city { get; set; }
        public string countryRegionCode { get; set; }
        public string phoneNo { get; set; }
        public string mobilePhoneNo { get; set; }
        public string faxNo { get; set; }
        public string vatregNo { get; set; }
        public string salespersonCode { get; set; }
        public string territoryCode { get; set; }
        public string currencyCode { get; set; }
        public string languageCode { get; set; }
        public string searchName { get; set; }
        public string eMail { get; set; }
        public string vendorBankBranchNo { get; set; }
        public string vendorBankAccNo { get; set; }
        public bool old { get; set; }
        public double invoiceCount { get; set; }
        public double settlementCount { get; set; }
        public bool settlementExists { get; set; }
        public bool supplierRawBiomass { get; set; }
        public bool industrialSupplRawBiomass { get; set; }
        public bool buyerDegassedBiomass { get; set; }
        public bool memberSupplierAssociation { get; set; }
        public string password { get; set; }
        public string dateFilter { get; set; }


        public class Rootobject
        {
            public Contact[] contacts { get; set; }
        }
    }
}
