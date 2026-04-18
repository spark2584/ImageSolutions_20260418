using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.TaskEntry
{
    public class TaskErrorFilter
    {
        public Database.Filter.StringSearch.SearchFilter TaskErrorID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TaskEntryID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TaskID { get; set; }
    }
}
