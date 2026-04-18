using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseCustomerEnterpriseEntityGroupFilter
    {
        public Database.Filter.StringSearch.SearchFilter EnterpriseCustomerID { get; set; }
        public Database.Filter.StringSearch.SearchFilter EnterpriseEntityGroupID { get; set; }
        public bool? IsUpdated { get; set; }
    }
}
