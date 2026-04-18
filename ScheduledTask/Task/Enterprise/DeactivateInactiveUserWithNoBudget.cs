using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class DeactivateInactiveUserWithNoBudget
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
UPDATE uw SET InActive = 1
--SELECT *
FROM EnterpriseCustomer (NOLOCK) ec
Inner JOin UserInfo (NOLOCK) u on u.EmailAddress = ec.Email
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = u.UserInfoID and uw.WebsiteID = {0}
Left Outer Join EnterpriseEBAJob (NOLOCK) j on j.JobCode = ec.Job
Left Outer Join BudgetAssignment (NOLOCK) ba on ba.UserWebsiteID = uw.UserWebsiteID
Left Outer Join Budget (NOLOCK) b on b.BudgetID = ba.BudgetID
Outer Apply
(
	SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2
	Inner Join SalesOrder (NOLOCK) s2 on s2.SalesOrderID = p2.SalesOrderID
	WHERE p2.BudgetAssignmentID = ba.BudgetAssignmentID
	and s2.Status != 'Rejected'
) Payment
Outer Apply
(
	SELECT SUM(ba2.Amount) Amount
	FROM BudgetAssignmentAdjustment (NOLOCK) ba2
	WHERE ba2.BudgetAssignmentID = ba.BudgetAssignmentID
) Adjustmnet
Outer Apply
(
    SELECT b.BudgetAmount - ISNULL(Payment.Amount,0) + ISNULL(Adjustmnet.Amount,0) Amount
) Balance
WHERE (ec.InActive = 1 or j.EnterpriseEBAJobID is null)
and uw.InActive = 0
and ec.IsIndividual = 1
and Balance.Amount <= 0
and ISNULL(ec.EmployeeID,'') not in ('E888KJ','E36CPR','E989XS','E371S4','E1649G','E621WM','E938N1')
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
