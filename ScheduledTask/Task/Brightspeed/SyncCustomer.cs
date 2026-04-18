using ImageSolutions.Brightspeed;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Brightspeed
{
    public class SyncCustomer
    {
        public bool Execute()
        {
            List<BrightspeedCustomer> Customers = null;
            BrightspeedCustomerFilter CustomerFilter = null;

            try
            {
                Customers = new List<BrightspeedCustomer>();
                CustomerFilter = new BrightspeedCustomerFilter();
                CustomerFilter.IsUpdated = true;

                //CustomerFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.ExternalID.SearchString = "1056339";

                Customers = BrightspeedCustomer.GetBrightspeedCustomers(CustomerFilter);

                int counter = 0;

                foreach (BrightspeedCustomer _Customer in Customers)
                {
                    counter++;

                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {

                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        Console.WriteLine(String.Format("{0}. Syncing Customer: {1}", counter, _Customer.BrightspeedCustomerID));

                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                        ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                        UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        UserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                        UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                        //Create UserInfo
                        if (UserInfo == null)
                        {
                            UserInfo = new ImageSolutions.User.UserInfo();
                            UserInfo.EmailAddress = _Customer.Email;

                            string strLastName = Convert.ToString(_Customer.DisplayName).Split(' ').Last();
                            UserInfo.FirstName = Convert.ToString(_Customer.DisplayName).Substring(0, Convert.ToString(_Customer.DisplayName).Length - strLastName.Length - 1);
                            UserInfo.LastName = strLastName;
                            UserInfo.Password = "Brightspeed$1";
                            UserInfo.Create(objConn, objTran);
                        }
                        else
                        {
                            UserInfo.EmailAddress = _Customer.Email;

                            string strLastName = Convert.ToString(_Customer.DisplayName).Split(' ').Last();
                            UserInfo.FirstName = Convert.ToString(_Customer.DisplayName).Substring(0, Convert.ToString(_Customer.DisplayName).Length - strLastName.Length - 1);
                            UserInfo.LastName = strLastName;
                            //UserInfo.Password = "Brightspeed$1";
                            UserInfo.Update(objConn, objTran);
                        }

                        //Create UserWebsite
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["BrightspeedWebsiteID"];
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedWebsiteID"]);
                            UserWebsite.EmployeeID = _Customer.EmployeeID;
                            //if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                            //{
                            //    if (!string.IsNullOrEmpty(_Customer.InternalID))
                            //    {
                            //        UserWebsite.CustomerInternalID = _Customer.InternalID;
                            //    }
                            //    else
                            //    {
                            //        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                            //        string CustomerInternalID = NSCustomer.CreateBrightspeedCustomer(_Customer);
                            //        if (string.IsNullOrEmpty(CustomerInternalID))
                            //        {
                            //            throw new Exception("Unable to create customer in NS");
                            //        }
                            //        UserWebsite.CustomerInternalID = CustomerInternalID;

                            //        _Customer.InternalID = CustomerInternalID;
                            //        _Customer.Update(objConn, objTran);
                            //    }
                            //}

                            //UserWebsite.IsStore = true;
                            //UserWebsite.AddressPermission = "Default";

                            UserWebsite.OptInForNotification = true;

                            UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserWebsite.InActive = _Customer.InActive;

                            //Temp
                            //UserWebsite.DisablePlaceOrder = true;

                            UserWebsite.Create(objConn, objTran);
                        }
                        else
                        {
                            //if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                            //{
                            //    if (!string.IsNullOrEmpty(_Customer.InternalID))
                            //    {
                            //        UserWebsite.CustomerInternalID = _Customer.InternalID;
                            //    }
                            //    else
                            //    {
                            //        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                            //        string CustomerInternalID = NSCustomer.CreateBrightspeedCustomer(_Customer);
                            //        if (string.IsNullOrEmpty(CustomerInternalID))
                            //        {
                            //            throw new Exception("Unable to create customer in NS");
                            //        }
                            //        UserWebsite.CustomerInternalID = CustomerInternalID;

                            //        _Customer.InternalID = CustomerInternalID;
                            //        _Customer.Update(objConn, objTran);
                            //    }
                            //}

                            //UserWebsite.IsStore = true;
                            //UserWebsite.AddressPermission = "Default";
                            UserWebsite.EmployeeID = _Customer.EmployeeID;

                            UserWebsite.OptInForNotification = true;

                            UserWebsite.InActive = _Customer.InActive;
                            UserWebsite.Update(objConn, objTran);
                        }

                        //Cost-Center
                        if(!string.IsNullOrEmpty(_Customer.CostCenterCode))
                        {
                            ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                            ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                            CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueFilter.CustomFieldID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCostCenterCustomFieldID"]);
                            CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            CustomValueFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);

                            if (CustomValue != null && !string.IsNullOrEmpty(CustomValue.CustomValueID))
                            {
                                CustomValue.Value = _Customer.CostCenterCode;
                                CustomValue.Update(objConn, objTran);
                            }
                            else
                            {
                                CustomValue = new ImageSolutions.Custom.CustomValue();
                                CustomValue.CustomFieldID = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCostCenterCustomFieldID"]);
                                CustomValue.UserWebsiteID = UserWebsite.UserWebsiteID;
                                CustomValue.Value = _Customer.CostCenterCode;
                                CustomValue.Create(objConn, objTran);
                            }
                        }

                        //Remove Users
                        if (!UserWebsite.InActive && (
                                UserWebsite.EmployeeID == "10001065"
                                || UserWebsite.EmployeeID == "00171690"
                                || UserWebsite.EmployeeID == "10001480"
                                || UserWebsite.EmployeeID == "10002150"
                                || UserWebsite.EmployeeID == "10001598"
                                || UserWebsite.EmployeeID == "10002220"
                                || UserWebsite.EmployeeID == "00098072"
                                || UserWebsite.EmployeeID == "10001863"
                                || UserWebsite.EmployeeID == "10001621"
                                || UserWebsite.EmployeeID == "10001631"
                                || UserWebsite.EmployeeID == "00097283"
                                || UserWebsite.EmployeeID == "00141202"
                            )
                        )
                        {
                            UserWebsite.InActive = true;
                            UserWebsite.Update(objConn, objTran);
                        }

                        //Always Keep Users Active
                        if (UserWebsite.InActive && (
                                UserWebsite.EmployeeID == "00078954"
                                || UserWebsite.EmployeeID == "00071446"
                                || UserWebsite.EmployeeID == "01000313"
                                || UserWebsite.EmployeeID == "01000286"
                                || UserWebsite.EmployeeID == "10001869"
                                || UserWebsite.EmployeeID == "10002027"
                                || UserWebsite.EmployeeID == "1000369"

                                || UserWebsite.EmployeeID == "00075030"
                                || UserWebsite.EmployeeID == "10001971"
                                || UserWebsite.EmployeeID == "00067309"
                                || UserWebsite.EmployeeID == "10002241"
                                || UserWebsite.EmployeeID == "10002328"
                                || UserWebsite.EmployeeID == "10001840"
                                || UserWebsite.EmployeeID == "10002648"
                                || UserWebsite.EmployeeID == "10002310"
                            )
                        )
                        {
                            UserWebsite.InActive = false;
                            UserWebsite.Update(objConn, objTran);
                        }
                        
                        if (!UserWebsite.InActive && UserInfo.EmailAddress != "jackie.johnson@Brightspeed.com" && UserInfo.EmailAddress != "Garrett.Garbelman@brightspeed.com")
                        {
                            //Assign Budget For Active                       
                            ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                            ImageSolutions.Budget.BudgetAssignmentFilter ExistBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                            ExistBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            ExistBudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            ExistBudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
                            ExistBudgetAssignmentFilter.BudgetID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedBudgetID"]);
                            ExistBudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(ExistBudgetAssignmentFilter);

                            if (ExistBudgetAssignment == null)
                            {
                                ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                BudgetAssignment.BudgetID = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedBudgetID"]);
                                BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedWebsiteID"]);
                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                BudgetAssignment.Create(objConn, objTran);
                            }
                        }

                        //Create UserAccount - default licensee group and account
                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedGroupID"]);
                        UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.AccountID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedAccountID"]);
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);

                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedAccountID"]);
                            UserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["BrightspeedGroupID"]);
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create(objConn, objTran);
                        }

                        //if (string.IsNullOrEmpty(_Customer.ErrorMessage))
                        //{
                        _Customer.IsUpdated = false;
                        _Customer.ErrorMessage = String.Empty;
                        _Customer.Update(objConn, objTran);
                        //}

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
