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
    public class SyncPreEmployeeCustomer
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
                CustomerFilter.IsPreEmployee = true;

                //Test
                //CustomerFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.EmployeeID.SearchString = "E976W3";

                Customers = EnterpriseCustomer.GetEnterpriseCustomers(CustomerFilter);

                //DateTime dtSyncedOn = DateTime.UtcNow;
                //string strSyncID = String.Format("{0}", DateTime.UtcNow.ToString("yyyyMMddHHmmssffffff"));

                foreach (EnterpriseCustomer _Customer in Customers)
                {
                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        bool isNew = true;

                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        Console.WriteLine("Syncing Customer: " + _Customer.EnterpriseCustomerID);

                        //Must have a valid Job code for EBA
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
                                UserInfo.Password = string.Format("{0}{1}"
                                    , _Customer.WorkdayID
                                    , _Customer.HireDate == null ? string.Empty : Convert.ToString(Convert.ToDateTime(_Customer.HireDate).Year)
                                );
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
                                UserInfo.Password = string.Format("{0}{1}"
                                    , _Customer.WorkdayID
                                    , _Customer.HireDate == null ? string.Empty : Convert.ToString(Convert.ToDateTime(_Customer.HireDate).Year)
                                );
                                UserInfo.Update(objConn, objTran);
                            }

                            //Create UserWebsite
                            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                            UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"];
                            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                            if (UserWebsite == null)
                            {
                                isNew = true;

                                UserWebsite = new ImageSolutions.User.UserWebsite();


                                if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                                {
                                    if (!string.IsNullOrEmpty(_Customer.EBAInternalID))
                                    {
                                        UserWebsite.CustomerInternalID = _Customer.EBAInternalID;
                                    }
                                    else
                                    {
                                        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                                        string CustomerInternalID = NSCustomer.CreateEnterpriseCustomerEBA(_Customer);
                                        if (string.IsNullOrEmpty(CustomerInternalID))
                                        {
                                            throw new Exception("Unable to create customer in NS");
                                        }
                                        UserWebsite.CustomerInternalID = CustomerInternalID;
                                        _Customer.EBAInternalID = CustomerInternalID;
                                        _Customer.Update(objConn, objTran);
                                    }
                                }

                                UserWebsite.UserInfoID = UserInfo.UserInfoID;
                                UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                UserWebsite.IsStore = false;
                                UserWebsite.EmployeeID = _Customer.EmployeeID;

                                //if (EnterpriseEBAJob.AllowAddressChange)
                                //{
                                //    UserWebsite.AddressPermission = string.Empty;
                                //}
                                //else
                                //{
                                //    UserWebsite.AddressPermission = "Account";
                                //}
                                UserWebsite.EnableShipToAccount = EnterpriseEBAJob.AllowAddressChange;
                                UserWebsite.AddressPermission = "Account";

                                UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);

                                UserWebsite.DisplayNotificaitonEmailAtCheckout = true;
                                UserWebsite.DisableNotificationEmail = false;

                                if (EnterpriseEBAJob.IsViewAccess)
                                {
                                    UserWebsite.IsAdmin = true;
                                    UserWebsite.BudgetManagement = true;
                                    UserWebsite.IsBudgetAdmin = true;
                                    UserWebsite.IsBudgetViewOnly = true;
                                    UserWebsite.OrderManagement = true;
                                    UserWebsite.UserManagement = false;
                                }
                                else if (EnterpriseEBAJob.IsAdminAccess)
                                {
                                    UserWebsite.IsAdmin = true;
                                    UserWebsite.BudgetManagement = true;
                                    UserWebsite.IsBudgetAdmin = true;
                                    UserWebsite.IsBudgetViewOnly = false;
                                    UserWebsite.OrderManagement = true;
                                    UserWebsite.UserManagement = true;
                                }
                                else
                                {
                                    UserWebsite.IsAdmin = false;
                                    UserWebsite.BudgetManagement = false;
                                    UserWebsite.IsBudgetAdmin = false;
                                    UserWebsite.IsBudgetViewOnly = false;
                                    UserWebsite.OrderManagement = false;
                                    UserWebsite.UserManagement = false;
                                }

                                if (EnterpriseEBAJob.IsCorporate)
                                {
                                    UserWebsite.AddressPermission = String.Empty;
                                }

                                UserWebsite.OptInForNotification = true;
                                UserWebsite.Create(objConn, objTran);
                            }
                            else
                            {
                                isNew = false;

                                if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]) == "production")
                                {
                                    if (!string.IsNullOrEmpty(_Customer.EBAInternalID))
                                    {
                                        UserWebsite.CustomerInternalID = _Customer.EBAInternalID;
                                    }
                                    else
                                    {
                                        NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                                        string CustomerInternalID = NSCustomer.CreateEnterpriseCustomerEBA(_Customer);
                                        if (string.IsNullOrEmpty(CustomerInternalID))
                                        {
                                            throw new Exception("Unable to create customer in NS");
                                        }
                                        UserWebsite.CustomerInternalID = CustomerInternalID;
                                        _Customer.EBAInternalID = CustomerInternalID;
                                        _Customer.Update(objConn, objTran);
                                    }
                                }

                                UserWebsite.IsStore = false;
                                UserWebsite.EmployeeID = _Customer.EmployeeID;

                                //if (EnterpriseEBAJob.AllowAddressChange)
                                //{
                                //    UserWebsite.AddressPermission = string.Empty;
                                //}
                                //else
                                //{
                                //    UserWebsite.AddressPermission = "Account";
                                //}
                                UserWebsite.EnableShipToAccount = EnterpriseEBAJob.AllowAddressChange;
                                UserWebsite.AddressPermission = "Account";

                                UserWebsite.InActive = false;

                                UserWebsite.DisplayNotificaitonEmailAtCheckout = true;
                                UserWebsite.DisableNotificationEmail = false;

                                if (UserWebsite.EmployeeID != "E888KJ" && UserWebsite.EmployeeID != "E36CPR" && UserWebsite.EmployeeID != "E989XS" && UserWebsite.EmployeeID != "E371S4" && UserWebsite.EmployeeID != "E1649G" && UserWebsite.EmployeeID != "E621WM" && UserWebsite.EmployeeID != "E938N1")
                                {
                                    if (EnterpriseEBAJob.IsViewAccess)
                                    {
                                        UserWebsite.IsAdmin = true;
                                        UserWebsite.BudgetManagement = true;
                                        UserWebsite.IsBudgetAdmin = true;
                                        UserWebsite.IsBudgetViewOnly = true;
                                        UserWebsite.OrderManagement = true;
                                        UserWebsite.UserManagement = false;
                                    }
                                    else if (EnterpriseEBAJob.IsAdminAccess)
                                    {
                                        UserWebsite.IsAdmin = true;
                                        UserWebsite.BudgetManagement = true;
                                        UserWebsite.IsBudgetAdmin = true;
                                        UserWebsite.IsBudgetViewOnly = false;
                                        UserWebsite.OrderManagement = true;
                                        UserWebsite.UserManagement = true;
                                    }
                                    else
                                    {
                                        UserWebsite.IsAdmin = false;
                                        UserWebsite.BudgetManagement = false;
                                        UserWebsite.IsBudgetAdmin = false;
                                        UserWebsite.IsBudgetViewOnly = false;
                                        UserWebsite.OrderManagement = false;
                                        UserWebsite.UserManagement = false;
                                    }
                                }

                                if (EnterpriseEBAJob.IsCorporate)
                                {
                                    UserWebsite.AddressPermission = String.Empty;
                                }

                                UserWebsite.OptInForNotification = true;
                                UserWebsite.Update(objConn, objTran);                               
                            }                           

                            //Store Customer
                            EnterpriseCustomer StoreCustomer = new EnterpriseCustomer();
                            EnterpriseCustomerFilter StoreCustomerFilter = new EnterpriseCustomerFilter();
                            StoreCustomerFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                            StoreCustomerFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                            StoreCustomerFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                            StoreCustomerFilter.ParentID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"];
                            StoreCustomer = EnterpriseCustomer.GetEnterpriseCustomer(StoreCustomerFilter);

                            // Find Store
                            ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                            ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                            AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);

                            AccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                            if (EnterpriseEBAJob.IsAdminAccess || EnterpriseEBAJob.IsViewAccess)
                            {
                                AccountFilter.StoreNumber.SearchString = _Customer.StoreNumber.Trim().Substring(0, 2);
                            }
                            if (_Customer.StoreNumber.Substring(0, 2) == "C1"
                                || _Customer.StoreNumber.Substring(0, 2) == "C2"
                                || _Customer.StoreNumber.Substring(0, 2) == "C3"
                                || _Customer.StoreNumber.Substring(0, 2) == "C6"
                                || _Customer.StoreNumber.Substring(0, 2) == "C7"
                                || _Customer.StoreNumber.Substring(0, 2) == "C9"

                                || _Customer.StoreNumber.Substring(0, 2) == "03"
                                || _Customer.StoreNumber.Substring(0, 2) == "12"
                                || _Customer.StoreNumber.Substring(0, 2) == "20"
                                || _Customer.StoreNumber.Substring(0, 2) == "21"
                                || _Customer.StoreNumber.Substring(0, 2) == "26"
                                || _Customer.StoreNumber.Substring(0, 2) == "34"
                                || _Customer.StoreNumber.Substring(0, 2) == "38"
                                || _Customer.StoreNumber.Substring(0, 2) == "45"
                                || _Customer.StoreNumber.Substring(0, 2) == "46"
                                || _Customer.StoreNumber.Substring(0, 2) == "47"
                                || _Customer.StoreNumber.Substring(0, 2) == "48"
                                || _Customer.StoreNumber.Substring(0, 2) == "50"
                                || _Customer.StoreNumber.Substring(0, 2) == "51"
                                || _Customer.StoreNumber.Substring(0, 2) == "52"
                                || _Customer.StoreNumber.Substring(0, 2) == "58"
                                )
                            {
                                AccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                            }
                            else
                            {
                                AccountFilter.StoreNumber.SearchString = string.IsNullOrEmpty(StoreCustomer.BranchAdminLgcyID) ? _Customer.StoreNumber : StoreCustomer.BranchAdminLgcyID;
                            }

                            Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);

                            if (Account == null)
                            {
                                throw new Exception(String.Format("Store #{0} not found", _Customer.StoreNumber));
                            }

                            if (UserWebsite.EmployeeID != "E888KJ" && UserWebsite.EmployeeID != "E36CPR" && UserWebsite.EmployeeID != "E989XS" && UserWebsite.EmployeeID != "E371S4" && UserWebsite.EmployeeID != "E1649G" && UserWebsite.EmployeeID != "E621WM" && UserWebsite.EmployeeID != "E938N1")
                            {
                                foreach (ImageSolutions.User.UserAccount _UserAccount in UserWebsite.UserAccounts.FindAll(x => x.AccountID != Account.AccountID))
                                {
                                    _UserAccount.Delete(objConn, objTran);
                                }

                                //Create UserAccount 
                                if (EnterpriseEBAJob.IsViewAccess || EnterpriseEBAJob.IsAdminAccess || !EnterpriseEBAJob.IsCorporate)
                                {
                                    ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                                    ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                    UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                    UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                    UserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAGroupID"]);
                                    UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                                    UserAccountFilter.AccountID.SearchString = Account.AccountID;
                                    UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                                    if (UserAccount == null)
                                    {
                                        UserAccount = new ImageSolutions.User.UserAccount();
                                        UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                        UserAccount.AccountID = Account.AccountID;
                                        UserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAGroupID"]);
                                        UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                        UserAccount.Create(objConn, objTran);
                                    }
                                }

                                //Assign Group for Corporate
                                ImageSolutions.User.UserAccount CorporateEBAUserAccount = new ImageSolutions.User.UserAccount();
                                ImageSolutions.User.UserAccountFilter CorporateEBAUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                CorporateEBAUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                CorporateEBAUserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                CorporateEBAUserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                CorporateEBAUserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBACorporateGroupID"]);
                                CorporateEBAUserAccount = ImageSolutions.User.UserAccount.GetUserAccount(CorporateEBAUserAccountFilter);
                                if (EnterpriseEBAJob.IsCorporate)
                                {
                                    if (CorporateEBAUserAccount == null)
                                    {
                                        CorporateEBAUserAccount = new ImageSolutions.User.UserAccount();
                                        CorporateEBAUserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                        CorporateEBAUserAccount.AccountID = Account.AccountID;
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
                                AdminEBAUserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                AdminEBAUserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                                AdminEBAUserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAdminGroupID"]);
                                AdminEBAUserAccount = ImageSolutions.User.UserAccount.GetUserAccount(AdminEBAUserAccountFilter);
                                if (EnterpriseEBAJob.IsAdminAccess)
                                {
                                    if (AdminEBAUserAccount == null)
                                    {
                                        AdminEBAUserAccount = new ImageSolutions.User.UserAccount();
                                        AdminEBAUserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                        AdminEBAUserAccount.AccountID = Account.AccountID;
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
                                AddressEBAAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                                AddressEBAAccountFilter.StoreNumber.SearchString = string.IsNullOrEmpty(StoreCustomer.BranchAdminLgcyID) ? _Customer.StoreNumber : StoreCustomer.BranchAdminLgcyID;
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
                                //    UserWebsite.AddressPermission = string.Empty;
                                //}
                                //else
                                //{
                                //    UserWebsite.AddressPermission = "Default";
                                //}
                                UserWebsite.EnableShipToAccount = EnterpriseEBAJob.AllowAddressChange;
                                UserWebsite.AddressPermission = "Default";

                                UserInfo.DefaultShippingAddressBookID = ShipToAddressBook.AddressBookID;
                                UserInfo.DefaultBillingAddressBookID = BillToAddressBook.AddressBookID;
                                UserInfo.Update(objConn, objTran);

                                UserWebsite.DefaultShippingAddressID = ShipToAddressBook.AddressBookID;
                                UserWebsite.DefaultBillingAddressID = BillToAddressBook.AddressBookID;
                                UserWebsite.Update(objConn, objTran);

                                //Assign to Group Account
                                List<ImageSolutions.User.UserAccount> EBAUserAccounts = new List<ImageSolutions.User.UserAccount>();
                                ImageSolutions.User.UserAccountFilter EBAUserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                                EBAUserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                EBAUserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                EBAUserAccounts = ImageSolutions.User.UserAccount.GetUserAccounts(EBAUserAccountFilter);

                                foreach (ImageSolutions.User.UserAccount _UserAccount in EBAUserAccounts)
                                {
                                    if (_UserAccount.Account.ParentAccount != null
                                        && _UserAccount.Account.ParentAccount.AccountName.Contains("Group")
                                        && _UserAccount.Account.ParentAccount.ParentID == Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAccountParentID"])
                                    )
                                    {
                                        if (_UserAccount.UserWebsite.UserAccounts != null
                                            && _UserAccount.UserWebsite.UserAccounts.Exists(x
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


                            if (
                                (!UserWebsite.InActive && !StoreCustomer.IsAirport && !EnterpriseEBAJob.IsAdminAccess && !EnterpriseEBAJob.IsViewAccess && _Customer.CustomerBrand == "Enterprise" && !EnterpriseEBAJob.IsCorporate)
                                ||
                                UserWebsite.ApplyBudgetProgram
                            )
                            {
                                //Find Current Budget for the user
                                List<ImageSolutions.Budget.BudgetAssignment> ExistBudgetAssignments = new List<ImageSolutions.Budget.BudgetAssignment>();
                                ImageSolutions.Budget.BudgetAssignmentFilter ExistBudgetAssignmentFilter = new ImageSolutions.Budget.BudgetAssignmentFilter();
                                ExistBudgetAssignmentFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                ExistBudgetAssignmentFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                                ExistBudgetAssignmentFilter.InActive = false;
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

                                //Assign Budget - EBA
                                //EnterpriseCustomer StoreCustomer = new EnterpriseCustomer();
                                //EnterpriseCustomerFilter StoreCustomerFilter = new EnterpriseCustomerFilter();
                                //StoreCustomerFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                                //StoreCustomerFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                                //StoreCustomerFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                //StoreCustomerFilter.ParentID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeCustomerParentID"];
                                //StoreCustomer = EnterpriseCustomer.GetEnterpriseCustomer(StoreCustomerFilter);

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
                                            CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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
                                            CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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
                                                CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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
                                                CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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
                                                CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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
                                                CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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

                                            CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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

                                            CreateBudgetAssignment(Budget.BudgetID, UserWebsite.UserWebsiteID, objConn, objTran);
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
                            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            UserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                            if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID))
                            {
                                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                if (UserWebsite != null)
                                {
                                    if (UserWebsite.EmployeeID != "E888KJ" && UserWebsite.EmployeeID != "E36CPR" && UserWebsite.EmployeeID != "E989XS" && UserWebsite.EmployeeID != "E371S4" && UserWebsite.EmployeeID != "E1649G" && UserWebsite.EmployeeID != "E621WM" && UserWebsite.EmployeeID != "E938N1")
                                    {
                                        //if (_Customer.TermDate != null && !UserWebsite.InActive)
                                        //{
                                        //    _Customer.BudgetEndDate = _Customer.TermDate;
                                        //    _Customer.Update(objConn, objTran);
                                        //}
                                        if (GetBudgetBalance(UserWebsite.UserWebsiteID) <= 0)
                                        {
                                            UserWebsite.IsStore = false;
                                            UserWebsite.EmployeeID = _Customer.EmployeeID;
                                            UserWebsite.InActive = true;
                                            UserWebsite.Update(objConn, objTran);
                                        }
                                    }                                        
                                }
                            }
                        }

                        //if (_Customer.TermDate != null)
                        //{
                        //    _Customer.BudgetEndDate = _Customer.TermDate;
                        //    _Customer.Update(objConn, objTran);
                        //}

                        if (_Customer.InActive)
                        {
                            //Deactivate EBA
                            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            UserInfoFilter.EmailAddress.SearchString = _Customer.Email;
                            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                            if (UserInfo != null && !string.IsNullOrEmpty(UserInfo.UserInfoID))
                            {
                                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                if (UserWebsite != null)
                                {
                                    //if (_Customer.TermDate != null && !UserWebsite.InActive)
                                    //{
                                    //    _Customer.BudgetEndDate = _Customer.TermDate;
                                    //    _Customer.Update(objConn, objTran);
                                    //}


                                    //if (GetBudgetBalance(UserWebsite.UserWebsiteID) <= 0)
                                    //{
                                        UserWebsite.IsStore = false;
                                        UserWebsite.EmployeeID = _Customer.EmployeeID;
                                        UserWebsite.InActive = true;
                                        UserWebsite.Update(objConn, objTran);
                                    //}
                                }
                            }
                        }

                        _Customer.IsUpdated = false;
                        _Customer.ErrorMessage = String.Empty;
                        _Customer.Update(objConn, objTran);
                      
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

        public void AdjustExistingBudget(ImageSolutions.Budget.BudgetAssignment budgetassignment, ImageSolutions.Budget.Budget budget, SqlConnection conn , SqlTransaction tran)
        {
            decimal amount = Convert.ToDecimal(budgetassignment.Budget.BudgetAmount - budget.BudgetAmount);

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
