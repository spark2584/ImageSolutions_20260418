using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLRFramework.Database
{
    public class AppSettings
    {
        public ConnectionString ConnectionStrings { get; set; }
        public Setting Settings { get; set; }
    }
    public class ConnectionString
    {
        public string Default { get; set; }
        public string Identity { get; set; }
    }
    public class Setting
    { 
        public string IdentityServerURL { get; set; }
        public string IdentityServerRegisterURL { get; set; }
    }
}
