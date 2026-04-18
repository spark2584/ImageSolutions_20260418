using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Sprouts
{
    public class SproutsCustomerFilter
    {
        public Database.Filter.StringSearch.SearchFilter TeamMemberID { get; set; }
        public Database.Filter.StringSearch.SearchFilter InternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Email { get; set; }
        public bool? IsUpdated { get; set; }
    }
}
