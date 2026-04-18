using ImageSolutions.Enterprise;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class UpdateInactivePreEmployee
    {
        public bool Execute()
        {
            return true;
        }

        public string UpdateInActivePreEmployeeCustomer(string syncid)
        {
            string objReturn = null;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE ec SET InActive = 1, IsPreEmployee = 1, IsUpdated = 1
--SELECT ec.InActive
FROM EnterpriseCustomer (NOLOCK) ec
Inner Join UserInfo (NOLOCK) ui on ui.EmailAddress = ec.Email
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = ui.UserInfoID
Outer Apply
(
	SELECT Max(SyncID) LatestSyncID
	FROM EnterpriseCustomer (NOLOCK) ec2
	WHERE ec2.IsPreEmployee = 0
	and ec.IsIndividual = 1
) sync
WHERE ec.IsIndividual = 1
and ec.IsPreEmployee = 0
and ec.InActivePreEmployee = 1
and uw.WebsiteID = {1}
and ISNULL(ec.SyncID,'') != sync.LatestSyncID
and ec.InActive = 0
"
                        , Database.HandleQuote(syncid)
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]))
                    );

                Database.ExecuteSQL(strSQL);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objReturn;
        }
    }
}
