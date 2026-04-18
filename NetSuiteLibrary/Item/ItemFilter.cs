using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;

namespace NetSuiteLibrary.Item
{
    public class ItemFilter
    {
        //public string TPIN { get; set; }
        public string APIExternalID { get; set; }
        public SearchStringField ItemNumber { get; set; }
        public List<string> ItemInternalIDs { get; set; }
        public DateTime? LastQtyAvailableChanged { get; set; }
        public DateTime? InventoryValidatedOn { get; set; }
        public SearchDateFieldOperator? InventoryValidatedOnOperator { get; set; }
        public List<NetSuiteItemType> NetSuiteItemTypes { get; set; }
        public SearchDateField LastModified { get; set; }
        public string PreferredVendor { get; set; }
    }
}

