using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.TaskEntry
{
    public class TaskEntryFilter
    {
        public Database.Filter.StringSearch.SearchFilter TaskEntryID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TaskID { get; set; }
        public Database.Filter.StringSearch.SearchFilter ExternalID { get; set; }
        public Database.Filter.StringSearch.SearchFilter Status { get; set; }
        public bool ScheduleRunDateDelay { get; set; }
    }
}
