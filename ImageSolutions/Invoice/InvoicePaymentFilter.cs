using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Invoice
{
    public class InvoicePaymentFilter
    {
        public Database.Filter.StringSearch.SearchFilter CustomerInternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PaymentLinkID { get; set; }
        public Database.Filter.StringSearch.SearchFilter PaymentLinkURL { get; set; }
        public Database.Filter.StringSearch.SearchFilter Status { get; set; }
    }
}
