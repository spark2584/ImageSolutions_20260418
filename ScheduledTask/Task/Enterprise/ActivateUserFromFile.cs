using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class ActivateUserFromFile
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE ec SET IsUpdated = 1
--SELECT uw.InActive, ec.InActive, ec.*
FROM UserWebsite (NOLOCK) uw
Inner JOin UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
Inner JOin EnterpriseCustomer (NOLOCK) ec on ec.Email = u.EmailAddress
Inner JOin EnterpriseCustomer (NOLOCK) ec2 on ec2.StoreNumber = ec.StoreNumber and ec2.IsIndividual = 0
Inner JOIn EnterpriseEBAJob (NOLOCK) j on j.JobCode = ec.Job
WHERE uw.WebsiteID = {0}
and ( ec2.IsAirport = 0 or uw.ApplyBudgetProgram = 1 )
and ec.InActive = 0
and uw.InActive = 1
"
                    , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]))
                );

                Database.ExecuteSQL(strSQL, Database.DefaultConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return true;
        }
    }
}
