using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace NetSuiteLibrary.File
{
    public class FileFilter
    {
        //public SearchStringField FolderInternalID { get; set; }
        //public List<string> FileInternalIDs { get; set; }
        public DateTime CreatedAfter { get; set; }
        public string Folder { get; set; }

    }
}
