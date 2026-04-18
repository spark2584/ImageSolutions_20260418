using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Invoice
{
    public class InvoiceFilter
    {
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InvoiceNumber { get; set; }
        public Database.Filter.StringSearch.SearchFilter PaymentLinkID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PaymentLinkURL { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter OrderNumber { get; set; }
    }
}
