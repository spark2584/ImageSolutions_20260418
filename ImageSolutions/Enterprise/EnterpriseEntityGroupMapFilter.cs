using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Enterprise
{
    public class EnterpriseEntityGroupMapFilter
    {
        public Database.Filter.StringSearch.SearchFilter EnterpriseEntityGroupID { get; set; }
        public Database.Filter.StringSearch.SearchFilter GroupName { get; set; }
        public Database.Filter.StringSearch.SearchFilter Code { get; set; }
    }
}
