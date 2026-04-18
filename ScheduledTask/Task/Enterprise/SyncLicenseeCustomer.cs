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
    public class SyncLicenseeCustomer
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
                CustomerFilter.IsIndividual = false;

                //CustomerFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                //CustomerFilter.ExternalID.SearchString = "1049744";

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

                            UserInfo.Password = _Customer.Password;
                            UserInfo.UserName = _Customer.StoreNumber;
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

                            UserInfo.Password = _Customer.Password;
                            UserInfo.UserName = _Customer.StoreNumber;
                            UserInfo.Update(objConn, objTran);
                        }

                        //Create UserWebsite
                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeWebsiteID"];
                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeWebsiteID"]);

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

                            UserWebsite.IsStore = true;
                            //UserWebsite.AddressPermission = "Default";
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

                            UserWebsite.IsStore = true;
                            //UserWebsite.AddressPermission = "Default";
                            UserWebsite.Update(objConn, objTran);
                        }                        

                        // Address for user and account
                        ImageSolutions.Address.AddressCountryCode AddressCountryCode = new ImageSolutions.Address.AddressCountryCode();
                        ImageSolutions.Address.AddressCountryCodeFilter AddressCountryCodeFilter = new ImageSolutions.Address.AddressCountryCodeFilter();
                        AddressCountryCodeFilter.Alpha3Code = new Database.Filter.StringSearch.SearchFilter();
                        AddressCountryCodeFilter.Alpha3Code.SearchString = _Customer.CountryCode;
                        AddressCountryCode = ImageSolutions.Address.AddressCountryCode.GetAddressCountryCode(AddressCountryCodeFilter);

                        ImageSolutions.Address.AddressBook ShipToAddressBook = null;
                        //if (string.IsNullOrEmpty(UserWebsite.DefaultShippingAddressID))
                        //{                           
                        //    ShipToAddressBook = new ImageSolutions.Address.AddressBook();
                        //}
                        //else
                        //{
                        //    ShipToAddressBook = new ImageSolutions.Address.AddressBook(UserWebsite.DefaultShippingAddressID);
                        //}
                        if (string.IsNullOrEmpty(UserInfo.DefaultShippingAddressBookID))
                        {
                            ShipToAddressBook = new ImageSolutions.Address.AddressBook();
                        }
                        else
                        {
                            ShipToAddressBook = new ImageSolutions.Address.AddressBook(UserInfo.DefaultShippingAddressBookID);
                        }
                        ShipToAddressBook.UserInfoID = UserInfo.UserInfoID;
                        ShipToAddressBook.AddressLabel = _Customer.CompanyName;
                        ShipToAddressBook.FirstName = _Customer.FirstName;
                        ShipToAddressBook.LastName = _Customer.LastName;
                        ShipToAddressBook.AddressLine1 = _Customer.ShipToAddress.Address1;
                        ShipToAddressBook.AddressLine2 = _Customer.ShipToAddress.Address2;
                        ShipToAddressBook.City = _Customer.ShipToAddress.City;
                        ShipToAddressBook.State = _Customer.ShipToAddress.State;
                        if (_Customer.ShipToAddress.State.Length == 2)
                        {
                            ShipToAddressBook.State = _Customer.ShipToAddress.State;
                        }
                        else
                        {
                            ImageSolutions.Address.AddressState AddressState = ImageSolutions.Address.AddressState.GetState(AddressCountryCode.Alpha2Code, _Customer.ShipToAddress.State);

                            if (AddressState != null)
                            {
                                ShipToAddressBook.State = AddressState.StateID;
                            }
                        }

                        ShipToAddressBook.PostalCode = _Customer.ShipToAddress.PostalCode;
                        ShipToAddressBook.CountryCode = AddressCountryCode.Alpha2Code;
                        ShipToAddressBook.PhoneNumber = _Customer.PhoneNumber;
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
                        //if (string.IsNullOrEmpty(UserWebsite.DefaultBillingAddressID))
                        //{
                        //    BillToAddressBook = new ImageSolutions.Address.AddressBook();
                        //}
                        //else
                        //{
                        //    BillToAddressBook = new ImageSolutions.Address.AddressBook(UserWebsite.DefaultBillingAddressID);
                        //}
                        if (string.IsNullOrEmpty(UserInfo.DefaultBillingAddressBookID))
                        {
                            BillToAddressBook = new ImageSolutions.Address.AddressBook();
                        }
                        else
                        {
                            BillToAddressBook = new ImageSolutions.Address.AddressBook(UserInfo.DefaultBillingAddressBookID);
                        }
                        BillToAddressBook.UserInfoID = UserInfo.UserInfoID;
                        BillToAddressBook.AddressLabel = _Customer.CompanyName;
                        BillToAddressBook.FirstName = _Customer.FirstName;
                        BillToAddressBook.LastName = _Customer.LastName;
                        BillToAddressBook.AddressLine1 = _Customer.ShipToAddress.Address1;
                        BillToAddressBook.AddressLine2 = _Customer.ShipToAddress.Address2;
                        BillToAddressBook.City = _Customer.ShipToAddress.City;
                        BillToAddressBook.State = _Customer.ShipToAddress.State;
                        if (_Customer.ShipToAddress.State.Length == 2)
                        {
                            BillToAddressBook.State = _Customer.ShipToAddress.State;
                        }
                        else
                        {
                            ImageSolutions.Address.AddressState AddressState = ImageSolutions.Address.AddressState.GetState(AddressCountryCode.Alpha2Code, _Customer.ShipToAddress.State);

                            if (AddressState != null)
                            {
                                BillToAddressBook.State = AddressState.StateID;
                            }
                        }

                        BillToAddressBook.PostalCode = _Customer.ShipToAddress.PostalCode;
                        BillToAddressBook.CountryCode = AddressCountryCode.Alpha2Code;
                        BillToAddressBook.PhoneNumber = _Customer.PhoneNumber;
                        if (BillToAddressBook.IsNew)
                        {
                            BillToAddressBook.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            BillToAddressBook.Create(objConn, objTran);
                        }
                        else
                        {
                            BillToAddressBook.Update(objConn, objTran);
                        }

                        UserInfo.DefaultShippingAddressBookID = ShipToAddressBook.AddressBookID;
                        UserInfo.DefaultBillingAddressBookID = BillToAddressBook.AddressBookID;
                        UserInfo.Update(objConn, objTran);

                        UserWebsite.DefaultShippingAddressID = ShipToAddressBook.AddressBookID;
                        UserWebsite.DefaultBillingAddressID = BillToAddressBook.AddressBookID;
                        UserWebsite.Update(objConn, objTran);

                        // _Customer.StoreNumber;
                        // Create Account for Corporate website - Store for Corporate
                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                        ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                        //AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                        //AccountFilter.AccountName.SearchString = _Customer.CompanyName;
                        AccountFilter.AccountNameExact = _Customer.CompanyName;

                        //AccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                        //AccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;

                        AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]);
                        Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);
                        if (Account == null)
                        {
                            Account = new ImageSolutions.Account.Account();
                            Account.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateWebsiteID"]);

                            //EnterpriseCustomer EnterpriseCustomer = new EnterpriseCustomer();
                            //EnterpriseCustomerFilter EnterpriseCustomerFilter = new EnterpriseCustomerFilter();
                            //EnterpriseCustomerFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                            //EnterpriseCustomerFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                            //EnterpriseCustomerFilter.IsIndividual = false;
                            //EnterpriseCustomer = EnterpriseCustomer.GetEnterpriseCustomer(EnterpriseCustomerFilter);

                            //if(EnterpriseCustomer == null)
                            //{
                            //    //SP - to assign parent when user gets created                            
                            //    NetSuiteLibrary.Customer.Customer NSCustomer = new NetSuiteLibrary.Customer.Customer();
                            //    string CustomerInternalID = NSCustomer.CreateEnterpriseCustomer(_Customer);
                            //    Account.CustomerInternalID = CustomerInternalID;
                            //}
                            //else
                            //{
                            //    Account.CustomerInternalID = EnterpriseCustomer.InternalID;
                            //}

                            if (!string.IsNullOrEmpty(UserWebsite.CustomerInternalID))
                            {
                                Account.CustomerInternalID = UserWebsite.CustomerInternalID;
                            }

                            Account.ParentID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseCorporateAccountParentID"]);
                            Account.AccountName = _Customer.CompanyName;
                            Account.StoreNumber = _Customer.StoreNumber;
                            Account.SiteNumber = string.Format(@"{0} - {1}", _Customer.BrandName, _Customer.BranchType);
                            Account.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            Account.Create(objConn, objTran);                   
                        }

                        // Create Account for EBA website - Store for Corporate

                        string strGroup = _Customer.StoreNumber.Trim().Substring(0, 2);
                        ImageSolutions.Account.Account GroupEBAAccount = new ImageSolutions.Account.Account();
                        ImageSolutions.Account.AccountFilter GroupEBAAccountFilter = new ImageSolutions.Account.AccountFilter();
                        GroupEBAAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                        GroupEBAAccountFilter.StoreNumber.SearchString = strGroup;
                        GroupEBAAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        GroupEBAAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                        GroupEBAAccount = ImageSolutions.Account.Account.GetAccount(GroupEBAAccountFilter);
                        if (GroupEBAAccount == null)
                        {
                            GroupEBAAccount = new ImageSolutions.Account.Account();
                            GroupEBAAccount.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                            GroupEBAAccount.ParentID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAccountParentID"]);
                            GroupEBAAccount.AccountName = String.Format("Enterprise Mobility Branch Group #{0}", strGroup);// _Customer.CompanyName;
                            GroupEBAAccount.StoreNumber = strGroup;
                            GroupEBAAccount.SiteNumber = strGroup;
                            GroupEBAAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            GroupEBAAccount.Create(objConn, objTran);
                        }

                        ImageSolutions.Account.Account EBAAccount = new ImageSolutions.Account.Account();
                        ImageSolutions.Account.AccountFilter EBAAccountFilter = new ImageSolutions.Account.AccountFilter();
                        EBAAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                        EBAAccountFilter.StoreNumber.SearchString = _Customer.StoreNumber;
                        EBAAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        EBAAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                        EBAAccount = ImageSolutions.Account.Account.GetAccount(EBAAccountFilter);

                        if (EBAAccount == null)
                        {
                            EBAAccount = new ImageSolutions.Account.Account();
                            EBAAccount.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);

                            if (!string.IsNullOrEmpty(UserWebsite.CustomerInternalID))
                            {
                                EBAAccount.CustomerInternalID = UserWebsite.CustomerInternalID;
                            }

                            //if (_Customer.StoreNumber == _Customer.BranchAdminLgcyID)
                            //{
                            //    EBAAccount.ParentID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAccountParentID"]);
                            //}
                            //else
                            //{
                            //    ImageSolutions.Account.Account EBAParentAccount = new ImageSolutions.Account.Account();
                            //    ImageSolutions.Account.AccountFilter EBAParentAccountFilter = new ImageSolutions.Account.AccountFilter();
                            //    EBAParentAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            //    EBAParentAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                            //    EBAParentAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                            //    EBAParentAccountFilter.StoreNumber.SearchString = _Customer.BranchAdminLgcyID; //string.Format("{0}99",_Customer.StoreNumber.Trim().Substring(0, 2));
                            //    EBAParentAccount = ImageSolutions.Account.Account.GetAccount(EBAParentAccountFilter);

                            //    if(EBAParentAccount == null)
                            //    {
                            //        throw new Exception(String.Format("Missing Parent Account: {0}", string.Format("{0}", _Customer.BranchAdminLgcyID)));
                            //    }

                            //    EBAAccount.ParentID = EBAParentAccount.AccountID;
                            //}

                            EBAAccount.ParentID = GroupEBAAccount.AccountID;

                            EBAAccount.AccountName = String.Format("Enterprise Mobility Branch #{0}", _Customer.StoreNumber);// _Customer.CompanyName;
                            EBAAccount.StoreNumber = _Customer.StoreNumber;
                            EBAAccount.SiteNumber = string.Format(@"{0} - {1}", _Customer.BrandName, _Customer.BranchType);
                            EBAAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            EBAAccount.Create(objConn, objTran);

                            //Address
                            if (_Customer.ShipToAddress != null && AddressCountryCode != null)
                            {
                                ImageSolutions.Address.AddressBook AddressBook = new ImageSolutions.Address.AddressBook();
                                AddressBook.AccountID = EBAAccount.AccountID;
                                AddressBook.AddressLabel = String.Format("Enterprise Mobility Branch #{0}", _Customer.StoreNumber); //_Customer.CompanyName;
                                AddressBook.FirstName = _Customer.FirstName;
                                AddressBook.LastName = _Customer.LastName;
                                AddressBook.AddressLine1 = _Customer.ShipToAddress.Address1;
                                AddressBook.AddressLine2 = _Customer.ShipToAddress.Address2;
                                AddressBook.City = _Customer.ShipToAddress.City;
                                AddressBook.State = _Customer.ShipToAddress.State;
                                if (_Customer.ShipToAddress.State.Length == 2)
                                {
                                    AddressBook.State = _Customer.ShipToAddress.State;
                                }
                                else
                                {
                                    ImageSolutions.Address.AddressState AddressState = ImageSolutions.Address.AddressState.GetState(AddressCountryCode.Alpha2Code, _Customer.ShipToAddress.State);

                                    if (AddressState != null)
                                    {
                                        AddressBook.State = AddressState.StateID;
                                    }
                                }

                                AddressBook.PostalCode = _Customer.ShipToAddress.PostalCode;
                                AddressBook.CountryCode = AddressCountryCode.Alpha2Code;
                                AddressBook.PhoneNumber = _Customer.PhoneNumber;
                                AddressBook.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                AddressBook.Create(objConn, objTran);

                                EBAAccount.DefaultShippingAddressBookID = AddressBook.AddressBookID;
                                EBAAccount.Update(objConn, objTran);
                            }
                        }
                        else
                        {
                            //if (_Customer.StoreNumber == _Customer.BranchAdminLgcyID)
                            ////if (_Customer.StoreNumber.Trim().Substring(2, 2) == "99")
                            //{
                            //    EBAAccount.ParentID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAAccountParentID"]);
                            //}
                            //else
                            //{
                            //    ImageSolutions.Account.Account EBAParentAccount = new ImageSolutions.Account.Account();
                            //    ImageSolutions.Account.AccountFilter EBAParentAccountFilter = new ImageSolutions.Account.AccountFilter();
                            //    EBAParentAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            //    EBAParentAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseEBAWebsiteID"]);
                            //    EBAParentAccountFilter.StoreNumber = new Database.Filter.StringSearch.SearchFilter();
                            //    EBAParentAccountFilter.StoreNumber.SearchString = _Customer.BranchAdminLgcyID; //string.Format("{0}99", _Customer.StoreNumber.Trim().Substring(0, 2));
                            //    EBAParentAccount = ImageSolutions.Account.Account.GetAccount(EBAParentAccountFilter);

                            //    if (EBAParentAccount == null)
                            //    {
                            //        throw new Exception(String.Format("Missing Parent Account: {0}", string.Format("{0}99", _Customer.StoreNumber.Trim().Substring(0, 2))));
                            //    }

                            //    EBAAccount.ParentID = EBAParentAccount.AccountID;
                            //}

                            EBAAccount.ParentID = GroupEBAAccount.AccountID;

                            EBAAccount.AccountName = String.Format("Enterprise Mobility Branch #{0}", _Customer.StoreNumber); //_Customer.CompanyName;
                            EBAAccount.StoreNumber = _Customer.StoreNumber;
                            EBAAccount.SiteNumber = string.Format(@"{0} - {1}", _Customer.BrandName, _Customer.BranchType);
                            EBAAccount.Update(objConn, objTran);

                            EBAAccount.DefaultShippingAddressBook.AccountID = EBAAccount.AccountID;
                            EBAAccount.DefaultShippingAddressBook.AddressLabel = String.Format("Enterprise Mobility Branch #{0}", _Customer.StoreNumber); //_Customer.CompanyName;
                            EBAAccount.DefaultShippingAddressBook.FirstName = _Customer.FirstName;
                            EBAAccount.DefaultShippingAddressBook.LastName = _Customer.LastName;
                            EBAAccount.DefaultShippingAddressBook.AddressLine1 = _Customer.ShipToAddress.Address1;
                            EBAAccount.DefaultShippingAddressBook.AddressLine2 = _Customer.ShipToAddress.Address2;
                            EBAAccount.DefaultShippingAddressBook.City = _Customer.ShipToAddress.City;
                            EBAAccount.DefaultShippingAddressBook.State = _Customer.ShipToAddress.State;
                            if (_Customer.ShipToAddress.State.Length == 2)
                            {
                                EBAAccount.DefaultShippingAddressBook.State = _Customer.ShipToAddress.State;
                            }
                            else
                            {
                                ImageSolutions.Address.AddressState AddressState = ImageSolutions.Address.AddressState.GetState(AddressCountryCode.Alpha2Code, _Customer.ShipToAddress.State);

                                if (AddressState != null)
                                {
                                    EBAAccount.DefaultShippingAddressBook.State = AddressState.StateID;
                                }
                            }

                            EBAAccount.DefaultShippingAddressBook.PostalCode = _Customer.ShipToAddress.PostalCode;
                            EBAAccount.DefaultShippingAddressBook.CountryCode = AddressCountryCode.Alpha2Code;
                            EBAAccount.DefaultShippingAddressBook.PhoneNumber = _Customer.PhoneNumber;
                            EBAAccount.DefaultShippingAddressBook.Update(objConn, objTran);
                        }


                        //Create UserAccount - default licensee group and account
                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeGroupID"]);
                        UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.AccountID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeAccountID"]);
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeAccountID"]);
                            UserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeGroupID"]);
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
         
        public void ManualCreate()
        {
            EnterpriseCustomer _Customer = new EnterpriseCustomer();

            _Customer.ParentID = "2";
            _Customer.Email = "LL99@ehi.com";
            _Customer.CompanyName = "Enterprise Mobility Licensee #LL99";
            _Customer.Password = "ITA00000";
            _Customer.StoreNumber = "LL99";
            _Customer.IsIndividual = false;

            _Customer.FirstName = String.Empty;
            _Customer.LastName = String.Empty;
            _Customer.PhoneNumber = String.Empty;
            _Customer.CountryCode = "N/A";
            _Customer.BrandName = String.Empty;

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

                UserInfo.Password = _Customer.Password;
                UserInfo.UserName = _Customer.StoreNumber;
                UserInfo.Create();
            }

            //Create UserWebsite
            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
            UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
            UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
            UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
            UserWebsiteFilter.WebsiteID.SearchString = ConfigurationManager.AppSettings["EnterpriseLicenseeWebsiteID"];
            UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

            if (UserWebsite == null)
            {
                UserWebsite = new ImageSolutions.User.UserWebsite();
                UserWebsite.UserInfoID = UserInfo.UserInfoID;
                UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeWebsiteID"]);

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
                        //_Customer.Update();
                    }
                }

                UserWebsite.IsStore = true;
                //UserWebsite.AddressPermission = "Default";
                UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                UserWebsite.Create();
            }

            //Create UserAccount - default licensee group and account
            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
            ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
            UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
            UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
            UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
            UserAccountFilter.WebsiteGroupID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeGroupID"]);
            UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
            UserAccountFilter.AccountID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeAccountID"]);
            UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
            if (UserAccount == null)
            {
                UserAccount = new ImageSolutions.User.UserAccount();
                UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                UserAccount.AccountID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeAccountID"]);
                UserAccount.WebsiteGroupID = Convert.ToString(ConfigurationManager.AppSettings["EnterpriseLicenseeGroupID"]);
                UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                UserAccount.Create();
            }
        }
    }
}
