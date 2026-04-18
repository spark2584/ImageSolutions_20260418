using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseCustomerImportFilter
    {
        public Database.Filter.StringSearch.SearchFilter EnterpriseCustomerImportID { get; set; }
        public Database.Filter.StringSearch.SearchFilter FileName { get; set; }
        public bool? IsProcessed { get; set; }
        public bool? IsStore { get; set; }
        public bool? IsPreEmployee { get; set; }
        public bool? IsEncrypted { get; set; }
    }
}
