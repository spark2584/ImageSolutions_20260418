using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;
using System.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ScheduledTask.Task
{
    public class SendDTBudgetReport
    {
        public bool Execute()
        {
            try
            {
                string strLocalFolder = (ConfigurationManager.AppSettings["CustomerLocalPath"]) + DateTime.UtcNow.ToString("yyyyMM") + "\\DT_Budget";

                if (!Directory.Exists(strLocalFolder))
                {
                    Directory.CreateDirectory(strLocalFolder);

                }

                string strfilename = string.Format("{0}.csv", DateTime.UtcNow.ToString("yyyyMMddHHmmssffff"));
                string strFileExportPath =  string.Format("{0}\\{1}", strLocalFolder, strfilename);
                CreateExportCSVBySQL("9", strFileExportPath, strfilename);


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        protected void CreateExportCSVBySQL(string websiteid, string filepath, string filename)
        {
            StringBuilder objReturn = new StringBuilder();

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {

                objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5}"
                    , "Employee ID"
                    , "First Name"
                    , "Last Name"
                    , "Budget Name"
                    , "Budget Amount"
                    , "Budget Balance")
                );

                objReturn.AppendLine();

                strSQL = string.Format(@"

SELECT 

	u.EmailAddress as email
	, REPLACE(u.UserName, ',', '') as employeeid
	, REPLACE(u.FirstName, ',', '') as firstname
	, REPLACE(u.LastName, ',', '') as lastname
	, REPLACE(b.BudgetName, ',', '') as budgetname
	, convert(varchar, b.StartDate, 101) as start_date
	, convert(varchar, b.EndDate, 101) as end_date
	, b.BudgetAmount as budgetamount

	, CASE WHEN Balance.Amount < 0 THEN 0 ELSE Balance.Amount END as remainingbalance

	--, CASE WHEN ua.Inactive = 1 THEN 'Yes' ELSE '' END as userinactive
    --, CASE WHEN ba.Inactive = 1 or b.StartDate > GETUTCDATE() or b.EndDate < GETUTCDATE() THEN 'Yes' ELSE '' END as budgetinactive

FROM Budget (NOLOCK) b
Left Outer Join BudgetAssignment (NOLOCK) ba on ba.BudgetID = b.BudgetID
Left Outer Join UserWebsite (NOLOCK) ua on ua.UserWebsiteID = ba.UserWebsiteID
Left Outer Join UserInfo (NOLOCK) u on u.UserInfoID = ua.UserInfoID

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
    SELECT SUM(p2.AmountPaid) Amount
	FROM Payment (NOLOCK) p2
	Inner Join SalesOrder (NOLOCK) s2 on s2.SalesOrderID = p2.SalesOrderID
	WHERE p2.BudgetAssignmentID = ba.BudgetAssignmentID
	and s2.Status != 'Rejected'
	and (s2.IsPendingApproval = 1 or s2.IsPendingItemPersonalizationApproval = 1)
) Pending
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

WHERE b.WebsiteID = {0}

AND b.StartDate <= GETUTCDATE() and b.EndDate >= GETUTCDATE()
AND ua.Inactive = 0
AND ba.Inactive = 0 

ORDER BY b.BudgetID

"
                        , Database.HandleQuote(websiteid));


                objRead = Database.GetDataReader(strSQL);


                while (objRead.Read())
                {
                    objReturn.Append(string.Format("{0},{1},{2},{3},{4},{5}"
                        , Convert.ToString(objRead["employeeid"])
                        , Convert.ToString(objRead["firstname"])
                        , Convert.ToString(objRead["lastname"])
                        , Convert.ToString(objRead["budgetname"])
                        , Convert.ToString(objRead["budgetamount"])
                        , Convert.ToString(objRead["remainingbalance"])
                    ));

                    objReturn.AppendLine();
                }

                if (objReturn != null)
                {
                    using (StreamWriter _streamwriter = new StreamWriter(filepath))
                    {
                        _streamwriter.Write(objReturn.ToString());
                    }
                }

                SendEmail("steve@imageinc.com", "DT - Budget Report", "see attachment", filepath, filename).Wait();
                SendEmail("misato@imageinc.com", "DT - Budget Report", "see attachment", filepath, filename).Wait();
                SendEmail("brent@imageinc.com", "DT - Budget Report", "see attachment", filepath, filename).Wait();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static async Task<Response> SendEmail(string toemail, string subject, string htmlcontent, string attachmentpath, string filename, List<string> ccs = null)
        {
            try
            {
                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() != "production"
                )
                {
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@iamgeinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
                }

                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(toemail);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, subject, null, htmlcontent);

                var bytes = File.ReadAllBytes(attachmentpath);
                var file = Convert.ToBase64String(bytes);
 
                SendGridMessage.AddAttachment(filename, file);

                if (ccs != null)
                {
                    foreach (string _cc in ccs)
                        SendGridMessage.AddCc(_cc);
                }

                return await Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
    }
}
