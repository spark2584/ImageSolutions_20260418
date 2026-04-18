using ImageSolutions.Enterprise;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class RefreshAnniversaryBudget
    {
        public bool Execute()
        {
            //SendBudgetAnniversaryEmail("steve@imageinc.com").Wait();

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
DECLARE @Date datetime

SET @Date = GETUTCDATE()

SELECT ec.EnterpriseCustomerID
	, u.EmailAddress
	, uw.UserWebsiteID
	, ba.BudgetAssignmentID
	, ec.BudgetRefreshedOn
	, ec.HireDate
	, Anniversary.AnniversaryDate
	, YEAR(ISNULL(ec.BudgetRefreshedOn,'')) RefreshYear
	, YEAR(@Date) CurrentYear
FROM UserInfo (NOLOCK) u
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = u.UserInfoID and uw.WebsiteID = {0}
Inner Join BudgetAssignment (NOLOCK) ba on ba.UserWebsiteID = uw.UserWebsiteID
Inner Join EnterpriseCustomer (NOLOCK) ec on ec.Email = u.EmailAddress
Inner JOin EnterpriseEBAJob (NOLOCK) j on j.JobCode = ec.Job
Inner Join EnterpriseCustomer (NOLOCK) ec2 on ec2.StoreNumber = ec.StoreNumber and ec2.ParentID = {1}
Outer Apply
(
	--SELECT CASE 
	--	WHEN ec.BudgetRefreshedOn is null 
	--		THEN CAST(MONTH(ec.HireDate) as varchar) + '/'
	--			+ CAST(DAY(ec.HireDate) as varchar) + '/'
	--			+ CAST(YEAR(ISNULL(ec.BudgetRefreshedOn, @Date)) as varchar)
		--ELSE DATEADD(YEAR, 1, ISNULL(ec.BudgetRefreshedOn,'')) END as AnniversaryDate
	SELECT CAST(MONTH(ec.HireDate) as varchar) + '/'
				--+ CAST(DAY(ec.HireDate) as varchar) + '/'
				+ CASE WHEN MONTH(ec.HireDate) = 2 and DAY(ec.HireDate) = 29 THEN '28' ELSE CAST(DAY(ec.HireDate) as varchar) END + '/'
				+ CAST(YEAR(@Date) as varchar) as AnniversaryDate
) Anniversary
WHERE ( ec2.IsAirport = 0 or uw.ApplyBudgetProgram = 1 )
and ec.HireDate is not null
and Anniversary.AnniversaryDate <= @Date
and YEAR(ISNULL(ec.BudgetRefreshedOn, ec.HireDate)) < YEAR(@Date)
and YEAR(ec.HireDate) != YEAR(@Date)
and j.IsCorporate = 0
and uw.InActive = 0
"
                    , Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"])
                    , Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"])
                );

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                while (objRead.Read())
                {
                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        ImageSolutions.Enterprise.EnterpriseCustomer _Customer = new EnterpriseCustomer(Convert.ToString(objRead["EnterpriseCustomerID"]));

                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        ////Find Current Budget for the user
                        //List<ImageSolutions.Budget.BudgetAssignment> ExistBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                        //ImageSolutions.Budget.BudgetAssignmentFilter ExistBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                        //ExistBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        //ExistBudgetAssignmentFilter.UserWebsiteID.SearchString = Convert.ToString(objRead["UserWebsiteID"]);
                        //ExistBudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(ExistBudgetAssignmentFilter);

                        //string strCurrentBudgetAssignmentID = string.Empty;

                        //if (ExistBudgetAssignments == null || ExistBudgetAssignments.Count == 0)
                        //{
                        //    throw new Exception("No Budget Found");
                        //}
                        //else if (ExistBudgetAssignments.Count > 1)
                        //{
                        //    throw new Exception(string.Format("Multiple Budgets found"));
                        //}
                        //else
                        //{
                        //    strCurrentBudgetAssignmentID = ExistBudgetAssignments[0].BudgetAssignmentID;
                        //}

                        ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(Convert.ToString(objRead["BudgetAssignmentID"]));

                        DateTime AnniversaryDate = Convert.ToDateTime(objRead["AnniversaryDate"]);

                        //if(BudgetAssignment.BudgetID == Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPermPTHCBudgetID"]))
                        //{
                        //    AdjustExistingBudget(BudgetAssignment, 75, objConn, objTran);
                        //}
                        //else 
                        if (BudgetAssignment.Budget.BudgetAmount > 100)
                        {
                            decimal amount = 100;

                            if (AnniversaryDate.Year == 2024)
                            {
                                amount = 25;
                            }
                            else if (AnniversaryDate.Year == 2025)
                            {
                                switch (AnniversaryDate.Month)
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        amount = 50;
                                        break;
                                    case 4:
                                    case 5:
                                    case 6:
                                        amount = 75;
                                        break;
                                }
                            }

                            //amount = 50;

                            AdjustExistingBudget(BudgetAssignment, amount, objConn, objTran);
                        }
                        //else
                        //{
                        //    AdjustExistingBudget(BudgetAssignment, Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount), objConn, objTran);
                        //}

                        _Customer.BudgetRefreshedOn = Convert.ToDateTime(objRead["AnniversaryDate"]);
                        _Customer.Update(objConn, objTran);

                        objTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (objTran != null && objTran.Connection != null)
                        {
                            objTran.Rollback();
                        }

                        if(string.IsNullOrEmpty(Convert.ToString(objRead["EnterpriseCustomerID"])))
                        {
                            ImageSolutions.Enterprise.EnterpriseCustomer _Customer = new EnterpriseCustomer(Convert.ToString(objRead["EnterpriseCustomerID"]));
                            _Customer.ErrorMessage = ex.Message;
                            _Customer.Update();
                        }
                    }
                }
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


            //List<EnterpriseCustomer> Customers = null;
            //EnterpriseCustomerFilter CustomerFilter = null;

            //try
            //{
            //    Customers = new List<EnterpriseCustomer>();
            //    CustomerFilter = new EnterpriseCustomerFilter();
            //    CustomerFilter.IsUpdated = true;
            //    CustomerFilter.IsIndividual = true;

            //    //Test
            //    //CustomerFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
            //    //CustomerFilter.EmployeeID.SearchString = "";

            //    Customers = EnterpriseCustomer.GetEnterpriseCustomers(CustomerFilter);

            //    foreach (EnterpriseCustomer _Customer in Customers)
            //    {
            //        try
            //        {
            //            if (_Customer.HireDate != null)
            //            {
            //                DateTime AnniversaryDate = Convert.ToDateTime(string.Format("{0}/{1}/{2}", Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Date, DateTime.UtcNow.Year));

            //                if (_Customer.HireDate != null
            //                    && AnniversaryDate < DateTime.UtcNow
            //                    && Convert.ToDateTime(_Customer.BudgetRefreshedOn).Year < DateTime.UtcNow.Year)
            //                {
            //                    ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
            //                    ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
            //                    UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
            //                    UserInfoFilter.EmailAddress.SearchString = _Customer.Email;
            //                    UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

            //                    if (UserInfo == null)
            //                    {
            //                        throw new Exception("Missing UserInfo");
            //                    }

            //                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            //                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
            //                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
            //                    UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
            //                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
            //                    UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"];
            //                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

            //                    if (UserWebsite == null)
            //                    {
            //                        throw new Exception("Missing UserWebsite");
            //                    }

            //                    //Find Current Budget for the user
            //                    List<ImageSolutions.Budget.BudgetAssignment> ExistBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
            //                    ImageSolutions.Budget.BudgetAssignmentFilter ExistBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
            //                    ExistBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
            //                    ExistBudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
            //                    ExistBudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(ExistBudgetAssignmentFilter);

            //                    string strCurrentBudgetAssignmentID = string.Empty;

            //                    if (ExistBudgetAssignments == null || ExistBudgetAssignments.Count == 0)
            //                    {
            //                        throw new Exception("No Budget Found");
            //                    }
            //                    else if (ExistBudgetAssignments.Count > 1)
            //                    {
            //                        throw new Exception(string.Format("Multiple Budgets found"));
            //                    }
            //                    else
            //                    {
            //                        strCurrentBudgetAssignmentID = ExistBudgetAssignments[0].BudgetAssignmentID;
            //                    }

            //                    ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);
            //                    if (BudgetAssignment.Budget.BudgetAmount >= 100)
            //                    {
            //                        AdjustExistingBudget(BudgetAssignment, 100);
            //                    }
            //                    else
            //                    {
            //                        AdjustExistingBudget(BudgetAssignment, Convert.ToDecimal(BudgetAssignment.Budget.BudgetAmount));
            //                    }
            //                }
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _Customer.ErrorMessage = ex.Message;
            //            _Customer.Update();
            //        }                    
            //    }
            //}            
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    Customers = null;
            //    CustomerFilter = null;
            //}
            return true;
        }

        public void AdjustExistingBudget(ImageSolutions.Budget.BudgetAssignment budgetassignment, decimal amount, SqlConnection conn, SqlTransaction tran)
        {
            if (amount != 0)
            {
                if (ValidateBudget(budgetassignment.BudgetAssignmentID))
                {

                    ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                    BudgetAssignmentAdjustment.BudgetAssignmentID = budgetassignment.BudgetAssignmentID;
                    BudgetAssignmentAdjustment.Amount = amount;
                    BudgetAssignmentAdjustment.Reason = string.Format("Budget Refresh");
                    BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                    BudgetAssignmentAdjustment.Create(conn, tran);

                    string strEmail = BudgetAssignmentAdjustment.BudgetAssignment.UserWebsite.NotificationEmail;
                    if (!string.IsNullOrEmpty(strEmail))
                    {
                        //SendBudgetAnniversaryEmail(strEmail).Wait();
                        //SendBudgetAnniversaryEmail("it@imageinc.com").Wait();
                    }
                }
            }
        }

        public bool ValidateBudget(string budgetassignmentid)
        {
            bool blnReturn = true;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"

SELECT baa.BudgetAssignmentAdjustmentID, baa.CreatedOn, CAST(baa.CreatedOn as date), Cast(GETUTCDATE() as Date)
FROM BudgetAssignmentAdjustment (NOLOCK) baa
WHERE baa.BudgetAssignmentID = {0}
and baa.Reason = 'Budget Refresh'
and CAST(baa.CreatedOn as date) = Cast(GETUTCDATE() as Date)
" 
                , Database.HandleQuote(budgetassignmentid));

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                if (objRead.Read())
                {
                    blnReturn = false;
                }
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

            return blnReturn;
        }

        static async Task<Response> SendBudgetAnniversaryEmail(string toemail, List<string> ccs = null)
        {
            string strSubject = string.Empty;
            string strHTMLContent = string.Empty;

            try
            {
                strSubject = "Happy Anniversary! Let’s help you refresh your Enterprise wardrobe!";
                strHTMLContent = string.Format(@"
<table>
    <tr>
        <td style='width: 625px;'>
            <img src='{0}' />            
        </td>            
    </tr>
    <tr>
        <td>
<br><br>
Congratulations on your Enterprise Mobility anniversary! To celebrate this milestone, we are pleased to share a new budget allocation through the Enterprise Branded Apparel Program! These funds are intended to assist you in refreshing and enhancing your work attire, ensuring you maintain a professional and comfortable appearance.
<br><br>
Here are a few ways to use your new allocation:
<br><br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;•&nbsp;	Upgrade Your Look: Select from the collection to keep your go-tos fresh and find new pieces that align with your professional image.
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;•&nbsp;	Accessorize: Add new accessories to complement and personalize your look.
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;•&nbsp;	Seasonal Updates: Prepare for the upcoming season with weather-appropriate attire.
<br><br>
Have your eye on a few new pieces that might go over your refresh allocation? As a reminder, you can buy additional items at any time using a personal credit card.  
<br><br>
Visit <a href='{1}'>EnterpriseApparel.com</a> to explore available options. If you have any questions or need assistance, please contact your local HR team.
        </td>
    </tr>
</table>
"
                    , "https://portal.imageinc.com/assets/company/EnterpriseEBA/EBAAnniversaryBudget.png"
                    , "https://enterpriseApparel.com"
                );

                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() != "production"
                )
                {
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "it@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
                }

                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(toemail);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, strSubject, null, strHTMLContent);

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
