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
    public class SyncCorporateCustomer
    {
        public bool Execute()
        {
            List<EnterpriseCustomer> Customers = null;
            EnterpriseCustomerFilter CustomerFilter = null;

            try
            {
                Customers = new List<EnterpriseCustomer>();
                CustomerFilter = new EnterpriseCustomerFilter();
                CustomerFilter.IsUpdated = true;
                CustomerFilter.IsIndividual = true;
                CustomerFilter.IsPreEmployee = false;

                //Test
                //CustomerFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.EmployeeID.SearchString = "E99BXZ";

                Customers = EnterpriseCustomer.GetEnterpriseCustomers(CustomerFilter);

                int counter = 0;

                foreach (EnterpriseCustomer _Customer in Customers)
                {
                    counter++;

                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        Console.WriteLine(String.Format("{0}. Syncing Customer: {1}", counter, _Customer.EnterpriseCustomerID));

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

                            if (!string.IsNullOrEmpty(_Customer.FirstName) && !string.IsNullOrEmpty(_Customer.LastName))
                            {
                                UserInfo.FirstName = Convert.ToString(_Customer.FirstName);
                                UserInfo.LastName = Convert.ToString(_Customer.LastName);
                            }
                            else
                            {
                                string strLastName = Convert.ToString(_Customer.CompanyName).Split(' ').Last();
                                UserInfo.FirstName = Convert.ToString(_Customer.CompanyName).Substring(0, Convert.ToString(_Customer.CompanyName).Length - strLastName.Length - 1);
                                UserInfo.LastName = strLastName;
                            }

                            UserInfo.UserName = _Customer.EmployeeID;
                            UserInfo.Password = _Customer.Password;
                            UserInfo.Create(objConn, objTran);
                        }
                        else
                        {
                            UserInfo.EmailAddress = _Customer.Email;

                            if (!string.IsNullOrEmpty(_Customer.FirstName) && !string.IsNullOrEmpty(_Customer.LastName))
                            {
                                UserInfo.FirstName = Convert.ToString(_Customer.FirstName);
                                UserInfo.LastName = Convert.ToString(_Customer.LastName);
                            }
                            else
                            {
                                string strLastName = Convert.ToString(_Customer.CompanyName).Split(' ').Last();
                                UserInfo.FirstName = Convert.ToString(_Customer.CompanyName).Substring(0, Convert.ToString(_Customer.CompanyName).Length - strLastName.Length - 1);
                                UserInfo.LastName = strLastName;
                            }

                            UserInfo.UserName = _Customer.EmployeeID;
                            UserInfo.Password = _Customer.Password;
                            UserInfo.Update(objConn, objTran);
                        }

                        //Create UserWebsite
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"];
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();

                            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                            {
                                if (!string.IsNullOrEmpty(_Customer.InternalID))
                                {
                                    UserWebsite.CustomerInternalID = _Customer.InternalID;
                                }
                                else
                                {
                                    NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                                    string CustomerInternalID = NSCustomer.CreateEnterpriseCustomer(_Customer);
                                    if(string.IsNullOrEmpty(CustomerInternalID))
                                    {
                                        throw new Exception("Unable to create customer in NS");
                                    }
                                    UserWebsite.CustomerInternalID = CustomerInternalID;

                                    _Customer.InternalID = CustomerInternalID;
                                    _Customer.Update(objConn, objTran);
                                }
                            }

                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]);
                            UserWebsite.IsStore = false;
                            UserWebsite.EmployeeID = _Customer.EmployeeID;
                            UserWebsite.OptInForNotification = true;
                            UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserWebsite.Create(objConn, objTran);
                        }
                        else
                        {
                            if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                            {
                                if (!string.IsNullOrEmpty(_Customer.InternalID))
                                {
                                    UserWebsite.CustomerInternalID = _Customer.InternalID;
                                }
                                else
                                {
                                    NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                                    string CustomerInternalID = NSCustomer.CreateEnterpriseCustomer(_Customer);
                                    if (string.IsNullOrEmpty(CustomerInternalID))
                                    {
                                        throw new Exception("Unable to create customer in NS");
                                    }
                                    UserWebsite.CustomerInternalID = CustomerInternalID;

                                    _Customer.InternalID = CustomerInternalID;
                                    _Customer.Update(objConn, objTran);
                                }
                            }

                            UserWebsite.IsStore = false;
                            UserWebsite.EmployeeID = _Customer.EmployeeID;
                            UserWebsite.OptInForNotification = true;
                            UserWebsite.Update(objConn, objTran);
                        }

                        // Find Store
                        //string strAccountID = FindStore(_Customer.StoreNumber);
                        //ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(strAccountID);

                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                        ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                        AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]);
                        AccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                        Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                        if (Account == null)
                        {
                            throw new Exception(String.Format("Store #{0} not found", _Customer.StoreNumber));
                        }

                        //Find Group and Assign                       
                        List<EnterpriseCustomerEnterpriseEntityGroup> CustomerEntityGroups = new List<EnterpriseCustomerEnterpriseEntityGroup>();
                        EnterpriseCustomerEnterpriseEntityGroupFilter CustomerEntityGroupFilter = new EnterpriseCustomerEnterpriseEntityGroupFilter();
                        CustomerEntityGroupFilter.IsUpdated = true;
                        CustomerEntityGroupFilter.EnterpriseCustomerID = new Database.Filter.StringSearch.SearchFilter();
                        CustomerEntityGroupFilter.EnterpriseCustomerID.SearchString = _Customer.EnterpriseCustomerID;
                        CustomerEntityGroups = EnterpriseCustomerEnterpriseEntityGroup.GetEnterpriseCustomerEnterpriseEntityGroups(CustomerEntityGroupFilter);

                        foreach (EnterpriseCustomerEnterpriseEntityGroup _CustomerEntityGroup in CustomerEntityGroups)
                        {
                            ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                            ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                            WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]); 
                            WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                            WebsiteGroupFilter.GroupName.SearchString = _CustomerEntityGroup.EnterpriseEntityGroup.Name;
                            WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);

                            if(WebsiteGroup == null)
                            {
                                WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                                WebsiteGroup.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]);
                                WebsiteGroup.GroupName = _CustomerEntityGroup.EnterpriseEntityGroup.Name;
                                WebsiteGroup.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                WebsiteGroup.Create(objConn, objTran);
                            }

                            if (!_CustomerEntityGroup.Inactive)
                            {
                                //Create UserAccount 
                                ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                                ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                                UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.AccountID.SearchString = Account.AccountID; //strAccountID;
                                UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                                if (UserAccount == null)
                                {
                                    UserAccount = new ImageSolutions.User.UserAccount();
                                    UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                    UserAccount.AccountID = Account.AccountID;
                                    UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                                    UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    UserAccount.Create(objConn, objTran);
                                }
                            }
                            else
                            {
                                //Remove UserAccount
                                ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                                ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                                UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                                UserAccountFilter.AccountID.SearchString = Account.AccountID; //strAccountID;
                                UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);

                                if (UserAccount != null)
                                {
                                    UserAccount.Delete(objConn, objTran);
                                }
                            }                           
                        }

                        //EBA
                        EnterpriseEBAJob EnterpriseEBAJob = new EnterpriseEBAJob();
                        EnterpriseEBAJobFilter EnterpriseEBAJobFilter = new EnterpriseEBAJobFilter();
                        EnterpriseEBAJobFilter.JobCode = new Database.Filter.StringSearch.SearchFilter();
                        EnterpriseEBAJobFilter.JobCode.SearchString = _Customer.Job;
                        EnterpriseEBAJob = EnterpriseEBAJob.GetEnterpriseEBAJob(EnterpriseEBAJobFilter);

                        //if (EnterpriseEBAJob != null)
                        if (EnterpriseEBAJob != null

                            && (
                                _Customer.StoreNumber != "2347"
                                && _Customer.StoreNumber != "2359"
                                && _Customer.StoreNumber != "2361"
                            )

                            && (
                                EnterpriseEBAJob.IsAdminAccess
                                || EnterpriseEBAJob.IsCorporate
                                || (
                                    _Customer.StoreNumber.Substring(0, 2) != "66"
                                    && _Customer.StoreNumber.Substring(0, 2) != "76"
                                    && _Customer.StoreNumber.Substring(0, 2) != "77"
                                    && _Customer.StoreNumber.Substring(0, 2) != "98"
                                    && _Customer.StoreNumber.Substring(0, 2) != "2R"
                                    && _Customer.StoreNumber.Substring(0, 2) != "F9"
                                    && _Customer.StoreNumber.Substring(0, 2) != "N7"
                                    && _Customer.StoreNumber.Substring(0, 2) != "V8"
                                    && _Customer.StoreNumber.Substring(0, 2) != "VG"

                                    && _Customer.StoreNumber.Substring(0, 2) != "36"
                                    )
                                )
                        )
                        {
                            bool isNew = true;

                            //Create UserWebsite
                            ImageSolutions.User.UserWebsite EBAUserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter EBAUserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            EBAUserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                            EBAUserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                            EBAUserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            EBAUserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"];
                            EBAUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(EBAUserWebsiteFilter);

                            if (EBAUserWebsite == null)
                            {
                                isNew = true;

                                EBAUserWebsite = new ImageSolutions.User.UserWebsite();

                                if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                                {
                                    if (!string.IsNullOrEmpty(_Customer.EBAInternalID))
                                    {
                                        EBAUserWebsite.CustomerInternalID = _Customer.EBAInternalID;
                                    }
                                    else
                                    {
                                        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                                        string CustomerInternalID = NSCustomer.CreateEnterpriseCustomerEBA(_Customer);
                                        EBAUserWebsite.CustomerInternalID = CustomerInternalID;
                                        _Customer.EBAInternalID = CustomerInternalID;
                                        _Customer.Update(objConn, objTran);
                                    }
                                }

                                EBAUserWebsite.UserInfoID = UserInfo.UserInfoID;
                                EBAUserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                EBAUserWebsite.IsStore = false;
                                EBAUserWebsite.EmployeeID = _Customer.EmployeeID;

                                //if (EnterpriseEBAJob.AllowAddressChange)
                                //{
                                //    EBAUserWebsite.AddressPermission = string.Empty;
                                //}
                                //else
                                //{
                                //    EBAUserWebsite.AddressPermission = "Account";
                                //}
                                EBAUserWebsite.EnableShipToAccount = EnterpriseEBAJob.AllowAddressChange;

                                EBAUserWebsite.AddressPermission = "Account";

                                EBAUserWebsite.NotificationEmail = Convert.ToString(_Customer.NotificationEmail);
                                EBAUserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);

                                EBAUserWebsite.DisplayNotificaitonEmailAtCheckout = false;
                                EBAUserWebsite.DisableNotificationEmail = true;

                                if (EnterpriseEBAJob.IsViewAccess)
                                {
                                    EBAUserWebsite.IsAdmin = true;
                                    EBAUserWebsite.BudgetManagement = true;
                                    EBAUserWebsite.IsBudgetAdmin = true;
                                    EBAUserWebsite.IsBudgetViewOnly = true;
                                    EBAUserWebsite.OrderManagement = true;
                                    EBAUserWebsite.UserManagement = false;
                                }
                                else if (EnterpriseEBAJob.IsAdminAccess)
                                {
                                    EBAUserWebsite.IsAdmin = true;
                                    EBAUserWebsite.BudgetManagement = true;
                                    EBAUserWebsite.IsBudgetAdmin = true;
                                    EBAUserWebsite.IsBudgetViewOnly = false;
                                    EBAUserWebsite.OrderManagement = true;
                                    EBAUserWebsite.UserManagement = true;
                                }
                                else
                                {
                                    EBAUserWebsite.IsAdmin = false;
                                    EBAUserWebsite.BudgetManagement = false;
                                    EBAUserWebsite.IsBudgetAdmin = false;
                                    EBAUserWebsite.IsBudgetViewOnly = false;
                                    EBAUserWebsite.OrderManagement = false;
                                    EBAUserWebsite.UserManagement = false;
                                }

                                if (EnterpriseEBAJob.IsCorporate)
                                {
                                    EBAUserWebsite.AddressPermission = String.Empty;
                                }

                                EBAUserWebsite.Create(objConn, objTran);
                            }
                            else
                            {
                                isNew = false;

                                if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                                {

                                    if (!string.IsNullOrEmpty(_Customer.EBAInternalID))
                                    {
                                        EBAUserWebsite.CustomerInternalID = _Customer.EBAInternalID;
                                    }
                                    else
                                    {
                                        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                                        string CustomerInternalID = NSCustomer.CreateEnterpriseCustomerEBA(_Customer);
                                        EBAUserWebsite.CustomerInternalID = CustomerInternalID;
                                        _Customer.EBAInternalID = CustomerInternalID;
                                        _Customer.Update(objConn, objTran);
                                    }
                                }

                                EBAUserWebsite.IsStore = false;
                                EBAUserWebsite.EmployeeID = _Customer.EmployeeID;

                                //if (EnterpriseEBAJob.AllowAddressChange)
                                //{
                                //    EBAUserWebsite.AddressPermission = string.Empty;
                                //}
                                //else
                                //{
                                //    EBAUserWebsite.AddressPermission = "Account";
                                //}
                                EBAUserWebsite.EnableShipToAccount = EnterpriseEBAJob.AllowAddressChange;

                                EBAUserWebsite.AddressPermission = "Account";

                                EBAUserWebsite.NotificationEmail = Convert.ToString(_Customer.NotificationEmail);
                                EBAUserWebsite.InActive = false;

                                EBAUserWebsite.DisplayNotificaitonEmailAtCheckout = false;
                                EBAUserWebsite.DisableNotificationEmail = true;


                                if (EBAUserWebsite.EmployeeID != "E888KJ" && EBAUserWebsite.EmployeeID != "E36CPR" && EBAUserWebsite.EmployeeID != "E989XS" && EBAUserWebsite.EmployeeID != "E371S4" && EBAUserWebsite.EmployeeID != "E1649G" && EBAUserWebsite.EmployeeID != "E621WM" && EBAUserWebsite.EmployeeID != "E938N1")
                                {
                                    if (EnterpriseEBAJob.IsViewAccess)
                                    {
                                        EBAUserWebsite.IsAdmin = true;
                                        EBAUserWebsite.BudgetManagement = true;
                                        EBAUserWebsite.IsBudgetAdmin = true;
                                        EBAUserWebsite.IsBudgetViewOnly = true;
                                        EBAUserWebsite.OrderManagement = true;
                                        EBAUserWebsite.UserManagement = false;
                                    }
                                    else if (EnterpriseEBAJob.IsAdminAccess)
                                    {
                                        EBAUserWebsite.IsAdmin = true;
                                        EBAUserWebsite.BudgetManagement = true;
                                        EBAUserWebsite.IsBudgetAdmin = true;
                                        EBAUserWebsite.IsBudgetViewOnly = false;
                                        EBAUserWebsite.OrderManagement = true;
                                        EBAUserWebsite.UserManagement = true;
                                    }
                                    else
                                    {
                                        EBAUserWebsite.IsAdmin = false;
                                        EBAUserWebsite.BudgetManagement = false;
                                        EBAUserWebsite.IsBudgetAdmin = false;
                                        EBAUserWebsite.IsBudgetViewOnly = false;
                                        EBAUserWebsite.OrderManagement = false;
                                        EBAUserWebsite.UserManagement = false;
                                    }
                                }

                                if (EnterpriseEBAJob.IsCorporate)
                                {
                                    EBAUserWebsite.AddressPermission = String.Empty;
                                }

                                EBAUserWebsite.Update(objConn, objTran);                                                               
                            }
                            
                            // Find Store
                            ImageSolutions.Account.Account EBAAccount = new ImageSolutions.Account.Account();
                            ImageSolutions.Account.AccountFilter EBAAccountFilter = new ImageSolutions.Account.AccountFilter();
                            EBAAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            EBAAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                            //EBAAccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;

                            EBAAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();

                            //Admin
                            if (EnterpriseEBAJob.IsAdminAccess || EnterpriseEBAJob.IsViewAccess)
                            {
                                EBAAccountFilter.StoreNumber.SearchString = _Customer.StoreNumber.Trim().Substring(0, 2);
                            }
                            //Prelaunch
                            //else if (_Customer.StoreNumber.Substring(0,2) == "C9")
                            //{
                            //    EBAAccountFilter.StoreNumber.SearchString = "C999";
                            //}
                            //else 
                            if (_Customer.StoreNumber.Substring(0, 2) == "P1")
                            {
                                EBAAccountFilter.StoreNumber.SearchString = "P199";
                            }
                            else
                            {
                                EBAAccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                            }

                            EBAAccount = ImageSolutions.Account.Account.GetAccount(EBAAccountFilter);

                            if (EBAAccount == null)
                            {
                                throw new Exception(String.Format("Store #{0} not found", _Customer.StoreNumber));
                            }


                            if (EBAUserWebsite.EmployeeID != "E888KJ" && EBAUserWebsite.EmployeeID != "E36CPR" && EBAUserWebsite.EmployeeID != "E989XS" && EBAUserWebsite.EmployeeID != "E371S4" && EBAUserWebsite.EmployeeID != "E1649G" && EBAUserWebsite.EmployeeID != "E621WM" && EBAUserWebsite.EmployeeID != "E938N1")
                            {
                                foreach (ImageSolutions.User.UserAccount _UserAccount in EBAUserWebsite.UserAccounts.FindAll(x => x.AccountID != EBAAccount.AccountID))
                                {
                                    _UserAccount.Delete(objConn, objTran);
                                }


                                if (EnterpriseEBAJob.IsViewAccess || EnterpriseEBAJob.IsAdminAccess || !EnterpriseEBAJob.IsCorporate)
                                {
                                    ImageSolutions.User.UserAccount EBAUserAccount = new ImageSolutions.User.UserAccount();
                                    ImageSolutions.User.UserAccountFilter EBAUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                    EBAUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    EBAUserAccountFilter.UserWebsiteID.SearchString = EBAUserWebsite.UserWebsiteID;
                                    EBAUserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                    EBAUserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAGroupID"]);
                                    EBAUserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                                    EBAUserAccountFilter.AccountID.SearchString = EBAAccount.AccountID;
                                    EBAUserAccount = ImageSolutions.User.UserAccount.GetUserAccount(EBAUserAccountFilter);
                                    if (EBAUserAccount == null)
                                    {
                                        EBAUserAccount = new ImageSolutions.User.UserAccount();
                                        EBAUserAccount.UserWebsiteID = EBAUserWebsite.UserWebsiteID;
                                        EBAUserAccount.AccountID = EBAAccount.AccountID;
                                        EBAUserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAGroupID"]);
                                        EBAUserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        EBAUserAccount.Create(objConn, objTran);
                                    }
                                }

                                //Assign Group for Corporate
                                ImageSolutions.User.UserAccount CorporateEBAUserAccount = new ImageSolutions.User.UserAccount();
                                ImageSolutions.User.UserAccountFilter CorporateEBAUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                CorporateEBAUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                CorporateEBAUserAccountFilter.UserWebsiteID.SearchString = EBAUserWebsite.UserWebsiteID;
                                CorporateEBAUserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                CorporateEBAUserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBACorporateGroupID"]);
                                CorporateEBAUserAccount = ImageSolutions.User.UserAccount.GetUserAccount(CorporateEBAUserAccountFilter);
                                if (EnterpriseEBAJob.IsCorporate)
                                {
                                    if (CorporateEBAUserAccount == null)
                                    {
                                        CorporateEBAUserAccount = new ImageSolutions.User.UserAccount();
                                        CorporateEBAUserAccount.UserWebsiteID = EBAUserWebsite.UserWebsiteID;
                                        CorporateEBAUserAccount.AccountID = EBAAccount.AccountID;
                                        CorporateEBAUserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBACorporateGroupID"]);
                                        CorporateEBAUserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        CorporateEBAUserAccount.Create(objConn, objTran);
                                    }
                                }
                                else
                                {
                                    if (CorporateEBAUserAccount != null && !string.IsNullOrEmpty(CorporateEBAUserAccount.UserAccountID))
                                    {
                                        CorporateEBAUserAccount.Delete(objConn, objTran);
                                    }
                                }

                                //Assign Group for Admin
                                ImageSolutions.User.UserAccount AdminEBAUserAccount = new ImageSolutions.User.UserAccount();
                                ImageSolutions.User.UserAccountFilter AdminEBAUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                AdminEBAUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                AdminEBAUserAccountFilter.UserWebsiteID.SearchString = EBAUserWebsite.UserWebsiteID;
                                AdminEBAUserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                AdminEBAUserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAdminGroupID"]);
                                AdminEBAUserAccount = ImageSolutions.User.UserAccount.GetUserAccount(AdminEBAUserAccountFilter);
                                if (EnterpriseEBAJob.IsAdminAccess)
                                {
                                    if (AdminEBAUserAccount == null)
                                    {
                                        AdminEBAUserAccount = new ImageSolutions.User.UserAccount();
                                        AdminEBAUserAccount.UserWebsiteID = EBAUserWebsite.UserWebsiteID;
                                        AdminEBAUserAccount.AccountID = EBAAccount.AccountID;
                                        AdminEBAUserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAdminGroupID"]);
                                        AdminEBAUserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        AdminEBAUserAccount.Create(objConn, objTran);
                                    }
                                }
                                else
                                {
                                    if (AdminEBAUserAccount != null && !string.IsNullOrEmpty(AdminEBAUserAccount.UserAccountID))
                                    {
                                        AdminEBAUserAccount.Delete(objConn, objTran);
                                    }
                                }
                            }

                            //Admin Update
                            if (EnterpriseEBAJob.IsAdminAccess || EnterpriseEBAJob.IsViewAccess)
                            {                               
                                // Find Store
                                ImageSolutions.Account.Account AddressEBAAccount = new ImageSolutions.Account.Account();
                                ImageSolutions.Account.AccountFilter AddressEBAAccountFilter = new ImageSolutions.Account.AccountFilter();
                                AddressEBAAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                AddressEBAAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                //Prelaunch
                                AddressEBAAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                                if (_Customer.StoreNumber.Substring(0, 2) == "C9")
                                {
                                    AddressEBAAccountFilter.StoreNumber.SearchString = "C999";
                                }
                                else if (_Customer.StoreNumber.Substring(0, 2) == "P1")
                                {
                                    AddressEBAAccountFilter.StoreNumber.SearchString = "P199";
                                }
                                else
                                {
                                    AddressEBAAccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                                }
                                AddressEBAAccount = ImageSolutions.Account.Account.GetAccount(AddressEBAAccountFilter);
                               
                                ImageSolutions.Address.AddressBook ShipToAddressBook = null;
                                if (string.IsNullOrEmpty(UserInfo.DefaultShippingAddressBookID))
                                {
                                    ShipToAddressBook = new ImageSolutions.Address.AddressBook();
                                }
                                else
                                {
                                    ShipToAddressBook = new ImageSolutions.Address.AddressBook(UserInfo.DefaultShippingAddressBookID);
                                }
                                ShipToAddressBook.UserInfoID = UserInfo.UserInfoID;
                                ShipToAddressBook.AddressLabel = AddressEBAAccount.DefaultShippingAddressBook.AddressLabel;
                                ShipToAddressBook.FirstName = AddressEBAAccount.DefaultShippingAddressBook.FirstName;
                                ShipToAddressBook.LastName = AddressEBAAccount.DefaultShippingAddressBook.LastName;
                                ShipToAddressBook.AddressLine1 = AddressEBAAccount.DefaultShippingAddressBook.AddressLine1;
                                ShipToAddressBook.AddressLine2 = AddressEBAAccount.DefaultShippingAddressBook.AddressLine2;
                                ShipToAddressBook.City = AddressEBAAccount.DefaultShippingAddressBook.City;
                                ShipToAddressBook.State = AddressEBAAccount.DefaultShippingAddressBook.State;
                                ShipToAddressBook.PostalCode = AddressEBAAccount.DefaultShippingAddressBook.PostalCode;
                                ShipToAddressBook.CountryCode = AddressEBAAccount.DefaultShippingAddressBook.CountryCode;
                                ShipToAddressBook.PhoneNumber = AddressEBAAccount.DefaultShippingAddressBook.PhoneNumber;
                                if (ShipToAddressBook.IsNew)
                                {
                                    ShipToAddressBook.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    ShipToAddressBook.Create(objConn, objTran);
                                }
                                else
                                {
                                    ShipToAddressBook.Update(objConn, objTran);
                                }

                                ImageSolutions.Address.AddressBook BillToAddressBook = null;
                                if (string.IsNullOrEmpty(UserInfo.DefaultBillingAddressBookID))
                                {
                                    BillToAddressBook = new ImageSolutions.Address.AddressBook();
                                }
                                else
                                {
                                    BillToAddressBook = new ImageSolutions.Address.AddressBook(UserInfo.DefaultBillingAddressBookID);
                                }
                                BillToAddressBook.UserInfoID = UserInfo.UserInfoID;
                                BillToAddressBook.AddressLabel = AddressEBAAccount.DefaultShippingAddressBook.AddressLabel;
                                BillToAddressBook.FirstName = AddressEBAAccount.DefaultShippingAddressBook.FirstName;
                                BillToAddressBook.LastName = AddressEBAAccount.DefaultShippingAddressBook.LastName;
                                BillToAddressBook.AddressLine1 = AddressEBAAccount.DefaultShippingAddressBook.AddressLine1;
                                BillToAddressBook.AddressLine2 = AddressEBAAccount.DefaultShippingAddressBook.AddressLine2;
                                BillToAddressBook.City = AddressEBAAccount.DefaultShippingAddressBook.City;
                                BillToAddressBook.State = AddressEBAAccount.DefaultShippingAddressBook.State;
                                BillToAddressBook.PostalCode = AddressEBAAccount.DefaultShippingAddressBook.PostalCode;
                                BillToAddressBook.CountryCode = AddressEBAAccount.DefaultShippingAddressBook.CountryCode;
                                BillToAddressBook.PhoneNumber = AddressEBAAccount.DefaultShippingAddressBook.PhoneNumber;
                                if (BillToAddressBook.IsNew)
                                {
                                    BillToAddressBook.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    BillToAddressBook.Create(objConn, objTran);
                                }
                                else
                                {
                                    BillToAddressBook.Update(objConn, objTran);
                                }

                                //if (EnterpriseEBAJob.AllowAddressChange)
                                //{
                                //    EBAUserWebsite.AddressPermission = string.Empty;
                                //}
                                //else
                                //{
                                //    EBAUserWebsite.AddressPermission = "Default";
                                //}
                                EBAUserWebsite.EnableShipToAccount = EnterpriseEBAJob.AllowAddressChange;

                                EBAUserWebsite.AddressPermission = "Default";

                                UserInfo.DefaultShippingAddressBookID = ShipToAddressBook.AddressBookID;
                                UserInfo.DefaultBillingAddressBookID = BillToAddressBook.AddressBookID;
                                UserInfo.Update(objConn, objTran);

                                EBAUserWebsite.DefaultShippingAddressID = ShipToAddressBook.AddressBookID;
                                EBAUserWebsite.DefaultBillingAddressID = BillToAddressBook.AddressBookID;
                                EBAUserWebsite.Update(objConn, objTran);

                                //Assign to Group Account
                                List<ImageSolutions.User.UserAccount> EBAUserAccounts = new List<ImageSolutions.User.UserAccount>();
                                ImageSolutions.User.UserAccountFilter EBAUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                EBAUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                EBAUserAccountFilter.UserWebsiteID.SearchString = EBAUserWebsite.UserWebsiteID;
                                EBAUserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(EBAUserAccountFilter);
                                
                                foreach (ImageSolutions.User.UserAccount _UserAccount in EBAUserAccounts)
                                {
                                    if (_UserAccount.Account.ParentAccount != null 
                                        && _UserAccount.Account.ParentAccount.AccountName.Contains("Group")
                                        && _UserAccount.Account.ParentAccount.ParentID == Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAccountParentID"])
                                    )
                                    {
                                        if (_UserAccount.UserWebsite.UserAccounts != null
                                            &&_UserAccount.UserWebsite.UserAccounts.Exists(x 
                                                => x.AccountID == _UserAccount.Account.ParentID 
                                                    && x.WebsiteGroupID == _UserAccount.WebsiteGroupID)
                                        )
                                        {
                                            _UserAccount.Delete(objConn, objTran);
                                        }
                                        else
                                        {
                                            _UserAccount.AccountID = _UserAccount.Account.ParentID;
                                            _UserAccount.Update(objConn, objTran);
                                        }
                                    }
                                }
                            }


                            //Assign Budget - EBA
                            EnterpriseCustomer StoreCustomer = new EnterpriseCustomer();
                            EnterpriseCustomerFilter StoreCustomerFilter = new EnterpriseCustomerFilter();
                            StoreCustomerFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                            StoreCustomerFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                            StoreCustomerFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                            StoreCustomerFilter.ParentID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"];
                            StoreCustomer = EnterpriseCustomer.GetEnterpriseCustomer(StoreCustomerFilter);

                            if (
                                (!EBAUserWebsite.InActive && !StoreCustomer.IsAirport && !EnterpriseEBAJob.IsAdminAccess && !EnterpriseEBAJob.IsViewAccess && _Customer.CustomerBrand == "Enterprise" && !EnterpriseEBAJob.IsCorporate)
                                ||
                                EBAUserWebsite.ApplyBudgetProgram
                            )
                            {
                                //Find Current Budget for the user
                                List<ImageSolutions.Budget.BudgetAssignment> ExistBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                                ImageSolutions.Budget.BudgetAssignmentFilter ExistBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                                ExistBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                ExistBudgetAssignmentFilter.UserWebsiteID.SearchString = EBAUserWebsite.UserWebsiteID;
                                //ExistBudgetAssignmentFilter.InActive = false;
                                ExistBudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(ExistBudgetAssignmentFilter);

                                string strCurrentBudgetAssignmentID = string.Empty;

                                if (ExistBudgetAssignments == null || ExistBudgetAssignments.Count == 0)
                                {
                                    isNew = true;
                                }
                                else if (ExistBudgetAssignments.Count > 1)
                                {
                                    throw new Exception(string.Format("Multiple Budgets found"));
                                }
                                else
                                {
                                    isNew = false;
                                    strCurrentBudgetAssignmentID = ExistBudgetAssignments[0].BudgetAssignmentID;
                                }

                                if (!string.IsNullOrEmpty(strCurrentBudgetAssignmentID))
                                {
                                    ImageSolutions.Budget.BudgetAssignment CurrentBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);
                                    if (CurrentBudgetAssignment.InActive)
                                    {
                                        CurrentBudgetAssignment.InActive = false;
                                        CurrentBudgetAssignment.Update(objConn, objTran);
                                    }
                                }

                                //Intern
                                if (_Customer.Job == "AA0025" || _Customer.Job == "10060")
                                {
                                    //Full Time Intern
                                    if (_Customer.CustomerStatus == "FT")
                                    {
                                        //Find Budget
                                        ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAFTInternBudgetID"]));

                                        if (Budget == null)
                                        {
                                            throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAFTInternBudgetID"])));
                                        }

                                        if (isNew)
                                        {
                                            CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                            //if (_Customer.HireDate != null)
                                            //{
                                            //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                            //    _Customer.Update(objConn, objTran);                                                
                                            //}
                                            _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                            _Customer.Update(objConn, objTran);
                                        }
                                        else
                                        {
                                            ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                            if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                            {
                                                AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                            }

                                            if (_Customer.TermDate != null
                                                    && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                    && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                            {
                                                ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                _Customer.BudgetEndDate = _Customer.TermDate;
                                                _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                _Customer.Update(objConn, objTran);
                                            }
                                        }
                                    }
                                    //Part Time Intern
                                    else if (_Customer.CustomerStatus == "PT")
                                    {
                                        //Find Budget
                                        ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPTInternBudgetID"]));

                                        if (Budget == null)
                                        {
                                            throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPTInternBudgetID"])));
                                        }

                                        if (isNew)
                                        {
                                            CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                            //if (_Customer.HireDate != null)
                                            //{
                                            //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                            //    _Customer.Update(objConn, objTran);
                                            //}
                                            _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                            _Customer.Update(objConn, objTran);
                                        }
                                        else
                                        {
                                            ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                            if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                            {
                                                AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                            }
                                            if (_Customer.TermDate != null
                                                    && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                    && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                            {
                                                ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                _Customer.BudgetEndDate = _Customer.TermDate;
                                                _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                _Customer.Update(objConn, objTran);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Invalid Status");
                                    }
                                }
                                //Customer Assistance Representative Senior
                                else if (_Customer.Job == "DR0061-1" || _Customer.Job == "10682") //(!StoreCustomer.IsAirport)
                                {
                                    //Temp
                                    if (_Customer.CustomerRegTemp == "T")
                                    {
                                        //Full Time
                                        if (_Customer.CustomerStatus == "FT")
                                        {
                                            //Find Budget
                                            ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBATempFTHCBudgetID"]));

                                            if (Budget == null)
                                            {
                                                throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBATempFTHCBudgetID"])));
                                            }

                                            if (isNew)
                                            {
                                                CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                                //if (_Customer.HireDate != null)
                                                //{
                                                //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                                //    _Customer.Update(objConn, objTran);
                                                //}
                                                _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                                _Customer.Update(objConn, objTran);
                                            }
                                            else
                                            {
                                                ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                                if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                                {
                                                    AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                                }
                                                if (_Customer.TermDate != null
                                                        && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                        && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                                {
                                                    ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                    _Customer.BudgetEndDate = _Customer.TermDate;
                                                    _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                    _Customer.Update(objConn, objTran);
                                                }
                                            }
                                        }
                                        //Part Time
                                        else if (_Customer.CustomerStatus == "PT")
                                        {
                                            //Find Budget
                                            ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBATempPTHCBudgetID"]));

                                            if (Budget == null)
                                            {
                                                throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBATempPTHCBudgetID"])));
                                            }
                                            if (isNew)
                                            {
                                                CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                                //if (_Customer.HireDate != null)
                                                //{
                                                //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                                //    _Customer.Update(objConn, objTran);
                                                //}
                                                _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                                _Customer.Update(objConn, objTran);
                                            }
                                            else
                                            {
                                                ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                                if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                                {
                                                    AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                                }
                                                if (_Customer.TermDate != null
                                                        && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                        && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                                {
                                                    ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                    _Customer.BudgetEndDate = _Customer.TermDate;
                                                    _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                    _Customer.Update(objConn, objTran);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid Status");
                                        }
                                    }
                                    //Perm
                                    else if (_Customer.CustomerRegTemp == "R")
                                    {
                                        //Full Time
                                        if (_Customer.CustomerStatus == "FT")
                                        {
                                            //Find Budget
                                            ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPermFTHCBudgetID"]));

                                            if (Budget == null)
                                            {
                                                throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPermFTHCBudgetID"])));
                                            }

                                            if (isNew)
                                            {
                                                CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                                //if (_Customer.HireDate != null)
                                                //{
                                                //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                                //    _Customer.Update(objConn, objTran);
                                                //}
                                                _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                                _Customer.Update(objConn, objTran);
                                            }
                                            else
                                            {
                                                ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                                if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                                {
                                                    AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                                }
                                                if (_Customer.TermDate != null
                                                        && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                        && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                                {
                                                    ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                    _Customer.BudgetEndDate = _Customer.TermDate;
                                                    _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                    _Customer.Update(objConn, objTran);
                                                }
                                            }
                                        }
                                        //Part Time
                                        else if (_Customer.CustomerStatus == "PT")
                                        {
                                            //Find Budget
                                            ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPermPTHCBudgetID"]));

                                            if (Budget == null)
                                            {
                                                throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPermPTHCBudgetID"])));
                                            }

                                            if (isNew)
                                            {
                                                CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                                //if (_Customer.HireDate != null)
                                                //{
                                                //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                                //    _Customer.Update(objConn, objTran);
                                                //}
                                                _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                                _Customer.Update(objConn, objTran);
                                            }
                                            else
                                            {
                                                ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                                if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                                {
                                                    AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                                }
                                                if (_Customer.TermDate != null
                                                        && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                        && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                                {
                                                    ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                    _Customer.BudgetEndDate = _Customer.TermDate;
                                                    _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                    _Customer.Update(objConn, objTran);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid Status");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Invalid Reg/Temp");
                                    }
                                }
                                else
                                {
                                    //Full Time
                                    if (_Customer.CustomerStatus == "FT")
                                    {
                                        //Find Budget
                                        ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBABudgetID"]));

                                        if (isNew)
                                        {
                                            if (Budget == null)
                                            {
                                                throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBABudgetID"])));
                                            }

                                            CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                            //if (_Customer.HireDate != null)
                                            //{
                                            //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                            //    _Customer.Update(objConn, objTran);
                                            //}
                                            _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                            _Customer.Update(objConn, objTran);
                                        }
                                        else
                                        {
                                            ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                            if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                            {
                                                AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                            }
                                            if (_Customer.TermDate != null
                                                    && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                    && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                            {
                                                ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                _Customer.BudgetEndDate = _Customer.TermDate;
                                                _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                _Customer.Update(objConn, objTran);
                                            }
                                        }
                                    }
                                    //Part Time
                                    else if (_Customer.CustomerStatus == "PT")
                                    {
                                        //Find Budget
                                        ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPTBudgetID"]));

                                        if (isNew)
                                        {
                                            if (Budget == null)
                                            {
                                                throw new Exception(string.Format("Cannot find budget: {0}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAPTBudgetID"])));
                                            }

                                            CreateBudgetAssignment(Budget.BudgetID, EBAUserWebsite.UserWebsiteID, objConn, objTran);
                                            //if (_Customer.HireDate != null)
                                            //{
                                            //    _Customer.BudgetRefreshedOn = new DateTime(DateTime.UtcNow.Year, Convert.ToDateTime(_Customer.HireDate).Month, Convert.ToDateTime(_Customer.HireDate).Day);
                                            //    _Customer.Update(objConn, objTran);
                                            //}
                                            _Customer.BudgetRefreshedOn = DateTime.UtcNow;
                                            _Customer.Update(objConn, objTran);
                                        }
                                        else
                                        {
                                            ImageSolutions.Budget.BudgetAssignment ExistBudgetAssignment = new ImageSolutions.Budget.BudgetAssignment(strCurrentBudgetAssignmentID);

                                            if (Budget.BudgetID != ExistBudgetAssignment.BudgetID)
                                            {
                                                AdjustExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);
                                            }
                                            if (_Customer.TermDate != null
                                                    && (_Customer.BudgetEndDate == null || _Customer.TermDate > _Customer.BudgetEndDate)
                                                    && Convert.ToDateTime(_Customer.TermDate).AddYears(1) <= DateTime.UtcNow)
                                            {
                                                ResetExistingBudget(ExistBudgetAssignment, Budget, objConn, objTran);

                                                _Customer.BudgetEndDate = _Customer.TermDate;
                                                _Customer.BudgetRefreshedOn = _Customer.HireDate;
                                                _Customer.Update(objConn, objTran);
                                            }
                                        }
                                    }
                                }

                                //if (_Customer.BudgetEndDate != null)
                                //{
                                //    _Customer.BudgetEndDate = null;
                                //    _Customer.Update(objConn, objTran);
                                //}
                            }

                            //else
                            //{
                            //    //Find Current Budget for the user
                            //    List<ImageSolutions.Budget.BudgetAssignment> ExistBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                            //    ImageSolutions.Budget.BudgetAssignmentFilter ExistBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                            //    ExistBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            //    ExistBudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            //    ExistBudgetAssignments = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignments(ExistBudgetAssignmentFilter);

                            //    foreach (ImageSolutions.Budget.BudgetAssignment _BudgetAssignment in ExistBudgetAssignments)
                            //    {
                            //        _BudgetAssignment.InActive = true;
                            //        _BudgetAssignment.Update(objConn, objTran);
                            //    }
                            //}
                        }
                        else
                        {
                            //Deactivate EBA
                            ImageSolutions.User.UserInfo EBAUserInfo = new ImageSolutions.User.UserInfo();
                            ImageSolutions.User.UserInfoFilter EBAUserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            EBAUserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            EBAUserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                            EBAUserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                            if (EBAUserInfo != null && !string.IsNullOrEmpty(EBAUserInfo.UserInfoID))
                            {
                                ImageSolutions.User.UserWebsite EBAUserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter EBAUserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                EBAUserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                EBAUserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                                EBAUserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                EBAUserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                EBAUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(EBAUserWebsiteFilter);

                                if (EBAUserWebsite != null)
                                {
                                    if (EBAUserWebsite.EmployeeID != "E888KJ" && EBAUserWebsite.EmployeeID != "E36CPR" && EBAUserWebsite.EmployeeID != "E989XS" && EBAUserWebsite.EmployeeID != "E371S4" && EBAUserWebsite.EmployeeID != "E1649G" && EBAUserWebsite.EmployeeID != "E621WM" && EBAUserWebsite.EmployeeID != "E938N1")
                                    {
                                        //if (_Customer.TermDate != null && !EBAUserWebsite.InActive)
                                        //{
                                        //    _Customer.BudgetEndDate = _Customer.TermDate;
                                        //    _Customer.Update(objConn, objTran);
                                        //}
                                        if (GetBudgetBalance(EBAUserWebsite.UserWebsiteID) <= 0)
                                        {
                                            EBAUserWebsite.IsStore = false;
                                            EBAUserWebsite.EmployeeID = _Customer.EmployeeID;
                                            EBAUserWebsite.InActive = true;
                                            EBAUserWebsite.Update(objConn, objTran);
                                        }
                                    }
                                }
                            }
                            
                        }

                        if (_Customer.InActive)
                        {
                            //Deactivate EBA
                            ImageSolutions.User.UserInfo EBAUserInfo = new ImageSolutions.User.UserInfo();
                            ImageSolutions.User.UserInfoFilter EBAUserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            EBAUserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            EBAUserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                            EBAUserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                            if (EBAUserInfo != null && !string.IsNullOrEmpty(EBAUserInfo.UserInfoID))
                            {
                                ImageSolutions.User.UserWebsite EBAUserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter EBAUserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                EBAUserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                EBAUserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                                EBAUserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                EBAUserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                EBAUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(EBAUserWebsiteFilter);

                                if (EBAUserWebsite != null)
                                {
                                    //if (_Customer.TermDate != null && !EBAUserWebsite.InActive)
                                    //{
                                    //    _Customer.BudgetEndDate = _Customer.TermDate;
                                    //    _Customer.Update(objConn, objTran);
                                    //}

                                    if(_Customer.InActivePreEmployee || GetBudgetBalance(EBAUserWebsite.UserWebsiteID) <= 0)
                                    {
                                        EBAUserWebsite.IsStore = false;
                                        EBAUserWebsite.EmployeeID = _Customer.EmployeeID;
                                        EBAUserWebsite.InActive = true;
                                        EBAUserWebsite.Update(objConn, objTran);
                                    }
                                }
                            }
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

        public decimal GetBudgetBalance(string userwebsiteid)
        {
            decimal ret = 0;
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT Balance.Amount
FROM UserInfo (NOLOCK) u
Inner Join UserWebsite (NOLOCK) uw on uw.UserInfoID = u.UserInfoID
Inner Join BudgetAssignment (NOLOCK) ba on ba.UserWebsiteID = uw.UserWebsiteID and ba.InActive = 0
Inner Join Budget (NOLOCK) b on b.BudgetID = ba.BudgetID
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
WHERE uw.UserWebsiteID = {0}
"
                    , Database.HandleQuote(userwebsiteid)
                );

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                if (objRead.Read())
                {
                    ret = Convert.ToDecimal(objRead["Amount"]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        public void CreateBudgetAssignment(string budgetid, string userwebsiteid, SqlConnection conn, SqlTransaction tran)
        {
            ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
            ImageSolutions.Budget.BudgetAssignmentFilter BudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
            BudgetAssignmentFilter.BudgetID = new Database.Filter.StringSearch.SearchFilter();
            BudgetAssignmentFilter.BudgetID.SearchString = budgetid;
            BudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
            BudgetAssignmentFilter.UserWebsiteID.SearchString = userwebsiteid;
            BudgetAssignment = ImageSolutions.Budget.BudgetAssignment.GetBudgetAssignment(BudgetAssignmentFilter);

            BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
            BudgetAssignment.BudgetID = budgetid;
            BudgetAssignment.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
            BudgetAssignment.UserWebsiteID = userwebsiteid;
            BudgetAssignment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
            BudgetAssignment.Create(conn, tran);            
        }

        public void AdjustExistingBudget(ImageSolutions.Budget.BudgetAssignment budgetassignment, ImageSolutions.Budget.Budget budget, SqlConnection conn, SqlTransaction tran)
        {
            Decimal amount = Convert.ToDecimal(budgetassignment.Budget.BudgetAmount - budget.BudgetAmount);

            if (amount > 0)
            {
                ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                BudgetAssignmentAdjustment.BudgetAssignmentID = budgetassignment.BudgetAssignmentID;
                BudgetAssignmentAdjustment.Amount = Convert.ToDecimal(budgetassignment.Budget.BudgetAmount - budget.BudgetAmount);
                BudgetAssignmentAdjustment.Reason = string.Format("Budget Adjustment - From {0} To {1}", budgetassignment.Budget.BudgetName, budget.BudgetName);
                BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                BudgetAssignmentAdjustment.Create(conn, tran);
            }

            budgetassignment.BudgetID = budget.BudgetID;
            budgetassignment.Update(conn, tran);
        }

        public void ResetExistingBudget(ImageSolutions.Budget.BudgetAssignment budgetassignment, ImageSolutions.Budget.Budget budget, SqlConnection conn, SqlTransaction tran)
        {
            ImageSolutions.Budget.MyBudgetAssignment MyBudgetAssignment = new ImageSolutions.Budget.MyBudgetAssignment(budgetassignment.UserWebsite.UserInfoID, budgetassignment.BudgetAssignmentID);

            decimal amount = Convert.ToDecimal(budgetassignment.Budget.BudgetAmount) - Convert.ToDecimal(MyBudgetAssignment.Balance);

            if (amount != 0)
            {
                ImageSolutions.Budget.BudgetAssignmentAdjustment BudgetAssignmentAdjustment = new ImageSolutions.Budget.BudgetAssignmentAdjustment();
                BudgetAssignmentAdjustment.BudgetAssignmentID = budgetassignment.BudgetAssignmentID;
                BudgetAssignmentAdjustment.Amount = amount;
                BudgetAssignmentAdjustment.Reason = "Renew Budget";
                BudgetAssignmentAdjustment.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                BudgetAssignmentAdjustment.Create(conn, tran);
            }

            budgetassignment.BudgetID = budget.BudgetID;
            budgetassignment.Update(conn, tran);
        }

        public string FindStore(string strStoreNumber)
        {
            string objReturn = null;
            int counter = 0;

            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT a.AccountID
FROM Account a
WHERE a.WebsiteID = {0}
and a.AccountName like '%{1}'
"
                        , Database.HandleQuote(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]))
                        , strStoreNumber);

                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    counter++;
                    objReturn = Convert.ToString(objRead["AccountID"]);

                    if (counter > 1)
                    {
                        throw new Exception(string.Format("Multiple accounts found: {0}", strStoreNumber));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objReturn;
        }
    }
}
