using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTaskProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteTask ExecuteTask = new ExecuteTask();
            ExecuteTask.ExecuteTaskEntries();
        }
    }
}
