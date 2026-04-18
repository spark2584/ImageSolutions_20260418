using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.TransferOrder
{
    public class TransferOrderFilter
    {
        public Database.Filter.StringSearch.SearchFilter TransferOrderID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter NetSuiteDocumentNumber { get; set; }
        public bool? IsPurchaseOrder { get; set; }
        public Database.Filter.StringSearch.SearchFilter PINumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter AmazonMWSAccountID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShipmentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ShipmentName { get; set; }
        public Database.Filter.StringSearch.SearchFilter FromLocationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ToLocationID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ErrorMessage { get; set; }
        public bool? IsNSUpdated { get; set; }
    }
}
