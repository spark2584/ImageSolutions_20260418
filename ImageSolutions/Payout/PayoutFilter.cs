using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Payout
{
    public class PayoutFilter
    {
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Status { get; set; }
    }
}
