using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLRFramework
{
    public class Employee
    {
        private string strEmployeeID { get; set; }
        private string State { get; set; }

        public Employee()
        {
        }

        public Employee(string EmployeeID)
        {
            strEmployeeID = EmployeeID;
            LoadEmployeeFromDatabase();   
        }

        private void LoadEmployeeFromDatabase()
        {
        }

        public static decimal GetOverTimeHours()
        {
                return 10;

        }

        public static decimal GetTexasOverTimeHours()
        {
                return 10;
        }

        public static decimal GetWashingtonOverTimeHours()
        {
                return 10;
        }
    }
}
