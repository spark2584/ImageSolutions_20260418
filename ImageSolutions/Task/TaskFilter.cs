using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Task
{
    public class TaskFilter
    {
        public Database.Filter.StringSearch.SearchFilter TaskID { get; set; }
        public Database.Filter.StringSearch.SearchFilter TaskName { get; set; }
    }
}
