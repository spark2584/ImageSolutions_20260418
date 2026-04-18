using ImageSolutions.Sprouts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Sprouts
{
    public class SyncCustomer
    {
        public bool Execute()
        {
            List<SproutsCustomer> Customers = null;
            SproutsCustomerFilter CustomerFilter = null;

            try
            {
                Customers = new List<SproutsCustomer>();
                CustomerFilter = new SproutsCustomerFilter();
                CustomerFilter.IsUpdated = true;

                //Test
                //CustomerFilter.EmployeeNumber = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.EmployeeNumber.SearchString = "414170";

                Customers = SproutsCustomer.GetSproutsCustomers(CustomerFilter);

                int counter = 0;

                foreach (SproutsCustomer _Customer in Customers)
                {
                    counter++;

                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        Console.WriteLine(String.Format("{0}. Syncing Customer: {1}", counter, _Customer.SproutsCustomerID));

                        string strEmail = String.Format("{0}@sprouts.com", _Customer.TeamMemberID);

                        if (!string.IsNullOrEmpty(strEmail))
                        {
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.EmployeeID.SearchString = _Customer.TeamMemberID;
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["SproutsEmployeeWebsiteID"];
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();

                            if (UserWebsite == null)
                            {
                                UserInfo = new ImageSolutions.User.UserInfo();
                                UserInfo.EmailAddress = strEmail;

                                UserInfo.LastName = _Customer.TeamMemberID;
                                UserInfo.FirstName = "Member";

                                UserInfo.Password = "Sprouts$1";

                                UserInfo.RequirePasswordReset = true;
                                UserInfo.Create(objConn, objTran);
                            }
                            else
                            {
                                UserInfo = UserWebsite.UserInfo;
                                UserInfo.EmailAddress = strEmail;

                                UserInfo.LastName = _Customer.TeamMemberID;
                                UserInfo.FirstName = "Member";

                                UserInfo.Update(objConn, objTran);
                            }

                            if (UserWebsite == null)
                            {
                                UserWebsite = new ImageSolutions.User.UserWebsite();
                                UserWebsite.UserInfoID = UserInfo.UserInfoID;
                                UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["SproutsEmployeeWebsiteID"]);
                                UserWebsite.EmployeeID = _Customer.TeamMemberID;
                                if (_Customer.HireDate != null) UserWebsite.HiredDate = _Customer.HireDate;
                                UserWebsite.OptInForNotification = true;
                                UserWebsite.NotificationEmail = _Customer.Email;
                                UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                UserWebsite.InActive = _Customer.Status.ToLower() == "terminated";
                                UserWebsite.Create(objConn, objTran);
                            }
                            else
                            {
                                UserWebsite.EmployeeID = _Customer.TeamMemberID;
                                if (_Customer.HireDate != null) UserWebsite.HiredDate = _Customer.HireDate;
                                UserWebsite.OptInForNotification = true;
                                UserWebsite.NotificationEmail = _Customer.Email;
                                UserWebsite.InActive = _Customer.Status.ToLower() == "terminated";
                                UserWebsite.Update(objConn, objTran);
                            }

                            //Store
                            if (!string.IsNullOrEmpty(_Customer.CostCenterID))
                            {
                                ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                                ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                                CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                                CustomValueFilter.CustomFieldID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["SproutsStoreCustomFieldID"]);
                                CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                CustomValueFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);

                                if (CustomValue != null && !string.IsNullOrEmpty(CustomValue.CustomValueID))
                                {
                                    CustomValue.Value = _Customer.CostCenterID;
                                    CustomValue.Update(objConn, objTran);
                                }
                                else
                                {
                                    CustomValue = new ImageSolutions.Custom.CustomValue();
                                    CustomValue.CustomFieldID = Convert.ToString(ConfigurationManager.AppSettings["SproutsStoreCustomFieldID"]);
                                    CustomValue.UserWebsiteID = UserWebsite.UserWebsiteID;
                                    CustomValue.Value = _Customer.CostCenterID;
                                    CustomValue.Create(objConn, objTran);
                                }
                            }

                            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                            ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                            UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.AccountID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["SproutsEmployeeAccountID"]);
                            UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                            if (UserAccount == null)
                            {
                                UserAccount = new ImageSolutions.User.UserAccount();
                                UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                UserAccount.AccountID = Convert.ToString(ConfigurationManager.AppSettings["SproutsEmployeeAccountID"]);
                                UserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["SproutsEmployeeWebsiteGroupID"]);
                                UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                UserAccount.Create(objConn, objTran);
                            }

                            if (_Customer.EnableBudget)
                            {
                                string strBudgetName = string.Empty;
                                strBudgetName = string.Format("{0} - {1}", DateTime.UtcNow.Year, _Customer.TeamMemberID);
                                DateTime BudgetStartDate = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                if (!UserWebsite.BudgetAssignments.Exists(x => x.Budget.BudgetName == strBudgetName))
                                {
                                    ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget();
                                    Budget.BudgetName = strBudgetName;
                                    Budget.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["SproutsEmployeeWebsiteID"]);
                                    Budget.StartDate = BudgetStartDate;
                                    Budget.EndDate = BudgetStartDate.AddYears(1).AddDays(-1);
                                    Budget.BudgetAmount = Convert.ToDouble(50);
                                    Budget.AllowOverBudget = true;
                                    Budget.IsSystemGenerated = true;
                                    Budget.IncludeShippingAndTaxes = true;
                                    Budget.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    Budget.Create(objConn, objTran);

                                    ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                    BudgetAssignment.BudgetID = Budget.BudgetID;
                                    BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["SproutsEmployeeWebsiteID"]);
                                    BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                    BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    BudgetAssignment.Create(objConn, objTran);
                                }
                            }

                            _Customer.IsUpdated = false;
                            _Customer.ErrorMessage = String.Empty;
                            _Customer.Update(objConn, objTran);
                        }
                        else
                        {
                            if (_Customer.Status.ToLower() == "terminated")
                            {
                                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                UserWebsiteFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.EmployeeID.SearchString = _Customer.TeamMemberID;
                                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["SproutsEmployeeWebsiteID"];
                                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                if (UserWebsite != null)
                                {
                                    UserWebsite.InActive = true;
                                    UserWebsite.Update(objConn, objTran);
                                }

                                _Customer.IsUpdated = false;
                                _Customer.ErrorMessage = String.Empty;
                                _Customer.Update(objConn, objTran);

                                string strBudgetName = string.Empty;
                                strBudgetName = string.Format("{0} - {1}", DateTime.UtcNow.Year, _Customer.TeamMemberID);
                                if (UserWebsite.BudgetAssignments.Exists(x => x.Budget.BudgetName == strBudgetName))
                                {
                                    ImageSolutions.Budget.BudgetAssignment BudgetAssignment = UserWebsite.BudgetAssignments.Find(x => x.Budget.BudgetName == strBudgetName);
                                    BudgetAssignment.Budget.BudgetName = String.Format("{0} - inactive");
                                    BudgetAssignment.Budget.Update(objConn, objTran);
                                }
                            }
                            else
                            {
                                throw new Exception("Missing Email");
                            }
                        }

                        objTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        if (objTran != null && objTran.Connection != null)
                        {
                            objTran.Rollback();
                        }

                        _Customer.ErrorMessage = ex.Message;
                        _Customer.Update();
                    }
                    finally
                    {
                        if (objConn != null) objConn.Dispose();
                        objConn = null;

                        if (objTran != null) objTran.Dispose();
                        objTran = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Customers = null;
                CustomerFilter = null;
            }
            return true;
        }
    }
}
