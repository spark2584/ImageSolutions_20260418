using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Invoice
{
    public class InvoicePaymentLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter InvoicePaymentID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InvoiceInternalID { get; set; }
    }
}
