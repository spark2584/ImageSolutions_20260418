using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;

namespace NetSuiteLibrary.SalesOrder
{
    public class SalesTaxItemFilter
    {
        public string TaxRate { get; set; }
        public List<string> salesTaxItemInternalIDs { get; set; }
    }
}

