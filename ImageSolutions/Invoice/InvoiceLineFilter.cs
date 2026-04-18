using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Invoice
{
    public class InvoiceLineFilter
    {
        public Database.Filter.StringSearch.SearchFilter InvoiceID { get; set; }
    }
}
