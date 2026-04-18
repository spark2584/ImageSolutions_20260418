using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.BinTransfer
{
    public class BinTransferFilter
    {
        public SearchDateField LastModified { get; set; }
        public List<string> NetSuiteInternalIDs { get; set; }
    }
}
