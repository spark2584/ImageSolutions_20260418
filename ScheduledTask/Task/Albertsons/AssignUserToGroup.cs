using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace ScheduledTask.Task.Albertsons
{
    public class AssignUserToGroup : NetSuiteBase
    {
        public bool Execute()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                //SP - Temp
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                //Mid-Atlantic Pharmacy Group ACME | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=293734&whence=
                AssignUserToGroupFromSearch("Mid-Atlantic Pharmacy Group ACME", "293734", objConn, objTran);

                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }

            return true;
        }

        public bool AssignUserToGroupFromSearch(string groupname, string searchid, SqlConnection objconn, SqlTransaction objtran)
        {
            try
            {
                //Create Website Group for the Division
                ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.GroupName.SearchString = groupname;
                WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["ABSCompaniesWebsiteID"]);
                WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);
                if (WebsiteGroup == null)
                {
                    throw new Exception("Group Not Found");
                }

                List<NetSuiteLibrary.com.netsuite.webservices.Customer> NSCustomers = GetCustomerSavedSearch(searchid);

                foreach (NetSuiteLibrary.com.netsuite.webservices.Customer _Customer in NSCustomers)
                {
                    try
                    {
                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                        ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                        UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        UserInfoFilter.EmailAddress.SearchString = _Customer.email;
                        UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                        if (UserInfo == null)
                        {
                            throw new Exception(String.Format("Missing Email: internalid {0} - {1}", _Customer.internalId, _Customer.email));
                        }

                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();

                        //Search By Email
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["ABSCompaniesWebsiteID"]);

                        //Search by Customer ID
                        //UserWebsiteFilter.CustomerInternalID = new Database.Filter.StringSearch.SearchFilter();
                        //UserWebsiteFilter.CustomerInternalID.SearchString = _Customer.internalId;

                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["ABSCompaniesWebsiteID"]);
                            UserWebsite.CustomerInternalID = _Customer.internalId;
                            UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserWebsite.Create(objconn, objtran);
                        }

                        if (_Customer.parent != null && !string.IsNullOrEmpty(_Customer.parent.internalId))
                        {
                            ImageSolutions.Account.Account ParentAccount = new ImageSolutions.Account.Account();

                            string strParentName = _Customer.parent.name;
                            string[] Accounts = strParentName.Split(':');
                            string strParentID = string.Empty;
                            for (int i = 0; i < Accounts.Length; i++)
                            {
                                string strAccount = Accounts[i].Trim();
                                //Create Division Account - with Customer Internal ID of parent NS customer 
                                ImageSolutions.Account.AccountFilter ParentAccountFilter = new ImageSolutions.Account.AccountFilter();
                                ParentAccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                                ParentAccountFilter.AccountName.SearchString = Convert.ToString(strAccount);
                                ParentAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                ParentAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["ABSCompaniesWebsiteID"]);

                                ParentAccountFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                if (!string.IsNullOrEmpty(strParentID))
                                {
                                    ParentAccountFilter.ParentID.SearchString = strParentID;
                                }
                                else
                                {
                                    ParentAccountFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                                }

                                ParentAccount = ImageSolutions.Account.Account.GetAccount(ParentAccountFilter);
                                if (ParentAccount == null)
                                {
                                    ParentAccount = new ImageSolutions.Account.Account();
                                    ParentAccount.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["ABSCompaniesWebsiteID"]);
                                    ParentAccount.CustomerInternalID = i == Accounts.Length - 1 ? _Customer.parent.internalId : string.Empty;
                                    //ParentAccount.StoreNumber = i == Accounts.Length - 1 ? Convert.ToString(NetSuiteHelper.GetStringCustomFieldValue(_Customer.parent, "custentity_store_org_number")) : string.Empty;
                                    //ParentAccount.SiteNumber = i == Accounts.Length - 1 ? Convert.ToString(NetSuiteHelper.GetStringCustomFieldValue(_Customer.parent, "custentity_kotn_add_cust_id1")) : string.Empty;
                                    ParentAccount.ParentID = strParentID;
                                    ParentAccount.AccountName = strAccount;
                                    ParentAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    ParentAccount.Create(objconn, objtran);
                                }
                                //else
                                //{
                                //    if (i == Accounts.Length - 1)
                                //    {
                                //        ParentAccount.CustomerInternalID = _Customer.parent.internalId;
                                //        ParentAccount.Update(objconn, objtran);
                                //    }
                                //}

                                strParentID = ParentAccount.AccountID;
                            }

                            string strAccountID = strParentID;
                            if (!_Customer.isPerson)
                            {
                                ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                                ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                                AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.AccountName.SearchString = _Customer.entityId;
                                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                AccountFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                if (!string.IsNullOrEmpty(strParentID))
                                {
                                    AccountFilter.ParentID.SearchString = strParentID;
                                }
                                else
                                {
                                    AccountFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                                }

                                Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);
                                if (Account == null)
                                {
                                    Account = new ImageSolutions.Account.Account();
                                    Account.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                    Account.CustomerInternalID = _Customer.internalId;
                                    Account.ParentID = strParentID;
                                    Account.AccountName = _Customer.entityId;
                                    Account.StoreNumber = Convert.ToString(NetSuiteHelper.GetStringCustomFieldValue(_Customer, "custentity_store_org_number"));
                                    Account.SiteNumber = Convert.ToString(NetSuiteHelper.GetStringCustomFieldValue(_Customer, "custentity_kotn_add_cust_id1"));
                                    Account.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    Account.Create(objconn, objtran);
                                }

                                strAccountID = Account.AccountID;
                            }

                            //Create UserAccount - UserInfo/UserWebsite/UserWebsiteGroup
                            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                            ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                            UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                            UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.AccountID.SearchString = strAccountID;
                            UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                            if (UserAccount == null)
                            {
                                UserAccount = new ImageSolutions.User.UserAccount();
                                UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                UserAccount.AccountID = strAccountID;
                                UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                                UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                UserAccount.Create(objconn, objtran);
                            }
                        }
                        else
                        {
                            throw new Exception(String.Format("Parent Not Found: internalid {0} - {1}", _Customer.internalId, _Customer.email));
                            //Account = new ImageSolutions.Account.Account("6914");  //Parent
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("{0}", ex.Message));
            }
            finally
            {

            }
            return true;
        }

        public bool AssignUserToGroupFromSQL(string groupname, string tablename, SqlConnection objconn, SqlTransaction objtran)
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                //Create Website Group for the Division
                ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.GroupName.SearchString = groupname;
                WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);
                if (WebsiteGroup == null)
                {
                    throw new Exception("Group Not Found");
                }


                strSQL = string.Format(@"
SELECT *
FROM {0} a
Outer Apply
(
	SELECT CASE WHEN CHARINDEX(':', a.Name) > 0 THEN SUBSTRING(a.Name, LEN(a.Name) - CHARINDEX(':',REVERSE(a.Name)) + 3, LEN(a.Name)) ELSE a.Name END as Name2
) Name2
WHERE ISNULL(a.Email,'') != ''
"
                    , tablename
                );

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                while (objRead.Read())
                {
                    try
                    {
                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                        ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                        UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        UserInfoFilter.EmailAddress.SearchString = Convert.ToString(objRead["Email"]);
                        UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                        if (UserInfo == null)
                        {
                            throw new Exception(String.Format("Missing Email: internalid {0} - {1}", Convert.ToString(objRead["InternalID"]), Convert.ToString(objRead["Email"])));
                        }

                        ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                        ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();

                        //Search By Email
                        UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                        UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                        //Search by Customer ID
                        //UserWebsiteFilter.CustomerInternalID = new Database.Filter.StringSearch.SearchFilter();
                        //UserWebsiteFilter.CustomerInternalID.SearchString = _Customer.internalId;

                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                            UserWebsite.CustomerInternalID = Convert.ToString(objRead["InternalID"]);
                            UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            //SP - Temp
                            UserWebsite.Create(objconn, objtran);
                            //UserWebsite.Create();
                        }

                        if (!string.IsNullOrEmpty(Convert.ToString(objRead["ParentInternalID"])))
                        {
                            ImageSolutions.Account.Account ParentAccount = new ImageSolutions.Account.Account();

                            string strParentName = Convert.ToString(objRead["ParentName"]);
                            string[] Accounts = strParentName.Split(':');
                            string strParentID = string.Empty;
                            for (int i = 0; i < Accounts.Length; i++)
                            {
                                string strAccount = Accounts[i].Trim();
                                //Create Division Account - with Customer Internal ID of parent NS customer 
                                ImageSolutions.Account.AccountFilter ParentAccountFilter = new ImageSolutions.Account.AccountFilter();
                                ParentAccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                                ParentAccountFilter.AccountName.SearchString = Convert.ToString(strAccount);
                                ParentAccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                ParentAccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);

                                ParentAccountFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                if (!string.IsNullOrEmpty(strParentID))
                                {
                                    ParentAccountFilter.ParentID.SearchString = strParentID;
                                }
                                else
                                {
                                    ParentAccountFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                                }

                                ParentAccount = ImageSolutions.Account.Account.GetAccount(ParentAccountFilter);
                                if (ParentAccount == null)
                                {
                                    ParentAccount = new ImageSolutions.Account.Account();
                                    ParentAccount.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                    ParentAccount.CustomerInternalID = i == Accounts.Length - 1 ? Convert.ToString(objRead["ParentInternalID"]) : string.Empty;
                                    ParentAccount.ParentID = strParentID;
                                    ParentAccount.AccountName = strAccount;
                                    ParentAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    //SP - Temp
                                    ParentAccount.Create(objconn, objtran);
                                    //ParentAccount.Create();
                                }
                                //else
                                //{
                                //    if (i == Accounts.Length - 1)
                                //    {
                                //        Account.CustomerInternalID = string.IsNullOrEmpty(Account.CustomerInternalID) ? Convert.ToString(objRead["ParentInternalID"]) : String.Empty;
                                //        Account.Update(objconn, objtran);
                                //    }
                                //}

                                strParentID = ParentAccount.AccountID;
                            }

                            string strAccountID = strParentID;
                            if (Convert.ToString(objRead["IsIndividual"]) != "Yes")
                            {
                                ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                                ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                                AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.AccountName.SearchString = Convert.ToString(objRead["Name2"]);
                                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                AccountFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                                if (!string.IsNullOrEmpty(strParentID))
                                {
                                    AccountFilter.ParentID.SearchString = strParentID;
                                }
                                else
                                {
                                    AccountFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                                }

                                Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);
                                if (Account == null)
                                {
                                    Account = new ImageSolutions.Account.Account();
                                    Account.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["DiscountTireWebsiteID"]);
                                    Account.CustomerInternalID = Convert.ToString(objRead["InternalID"]);
                                    Account.ParentID = strParentID;
                                    Account.AccountName = Convert.ToString(objRead["Name2"]);
                                    Account.StoreNumber = Convert.ToString(objRead["StoreNumber"]);
                                    Account.SiteNumber = Convert.ToString(objRead["AdditionalCustID1"]);
                                    Account.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    //SP - Temp
                                    Account.Create(objconn, objtran);
                                    //Account.Create();
                                }

                                strAccountID = Account.AccountID;
                            }

                            //Create UserAccount - UserInfo/UserWebsite/UserWebsiteGroup
                            ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                            ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                            UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                            UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                            UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                            UserAccountFilter.AccountID.SearchString = strAccountID;
                            UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                            if (UserAccount == null)
                            {
                                UserAccount = new ImageSolutions.User.UserAccount();
                                UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                                UserAccount.AccountID = strAccountID;
                                UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                                UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                //SP - Temp
                                UserAccount.Create(objconn, objtran);
                                //UserAccount.Create();
                            }
                        }
                        else
                        {
                            throw new Exception(String.Format("Parent Not Found: internalid {0} - {1}", Convert.ToString(objRead["InternalID"]), Convert.ToString(objRead["Email"])));
                            //Account = new ImageSolutions.Account.Account("6914");  //Parent?
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));
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

            return true;
        }

        public List<NetSuiteLibrary.com.netsuite.webservices.Customer> GetCustomerSavedSearch(string savedsearchid)
        {
            List<Customer> Customers = null;
            SearchResult SearchResult = null;
            Dictionary<string, string> dicInternalIds = new Dictionary<string, string>();

            try
            {
                Customers = new List<NetSuiteLibrary.com.netsuite.webservices.Customer>();
                SearchResult = GetSavedSearch(savedsearchid);

                int count = 0;

                if (SearchResult != null && SearchResult.totalRecords > 0)
                {
                    do
                    {
                        if (SearchResult.searchRowList != null)
                        {
                            foreach (CustomerSearchRow _CustomerSearchRow in SearchResult.searchRowList)
                            {
                                count++;
                                Console.WriteLine(string.Format("{0}/{1}", count, SearchResult.totalRecords));

                                if (_CustomerSearchRow is CustomerSearchRow && !dicInternalIds.ContainsKey(_CustomerSearchRow.basic.internalId[0].searchValue.internalId))
                                {
                                    string strInternalID = _CustomerSearchRow.basic.internalId[0].searchValue.internalId;

                                    dicInternalIds.Add(strInternalID, strInternalID);
                                    Customer Customer = LoadNetSuiteCustomer(strInternalID);
                                    if (_CustomerSearchRow.basic.terms != null)
                                    {
                                        Customer.terms = _CustomerSearchRow.basic.terms[0].searchValue;
                                    }
                                    Customers.Add(Customer);
                                }
                            }
                            Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                            SearchResult = Service.searchMoreWithId(SearchResult.searchId, SearchResult.pageIndex + 1);
                        }
                    }
                    while (SearchResult.searchRowList != null && SearchResult.pageSizeSpecified == true && SearchResult.totalPages >= SearchResult.pageIndex);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Customers;
        }
        public SearchResult GetSavedSearch(string savedsearchid)
        {
            CustomerSearchAdvanced CustomerSearchAdvanced = null;
            SearchResult SearchResult = null;
            CustomerSearch CustomerSearch = null;
            SearchPreferences SearchPreferences = null;

            try
            {
                CustomerSearchAdvanced = new CustomerSearchAdvanced();
                CustomerSearchAdvanced.savedSearchId = savedsearchid;

                SearchPreferences = new SearchPreferences();
                SearchPreferences.returnSearchColumns = true;
                Service.searchPreferences = SearchPreferences;

                SearchResult = Service.search(CustomerSearchAdvanced);

                if (SearchResult.status.isSuccess != true) throw new Exception(string.Format("Cannot find Saved Search - {0}: {1}", savedsearchid, SearchResult.status.statusDetail[0].message));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return SearchResult;
        }
        private NetSuiteLibrary.com.netsuite.webservices.Customer LoadNetSuiteCustomer(string NetSuiteInternalID)
        {
            RecordRef objCustomerRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Customer objReturn = null;
            try
            {
                objCustomerRef = new RecordRef();
                objCustomerRef.internalId = NetSuiteInternalID;
                objCustomerRef.type = RecordType.customer;
                objCustomerRef.typeSpecified = true;

                objReadResult = Service.get(objCustomerRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.Customer))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.Customer)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Customer with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerRef = null;
                objReadResult = null;
            }
            return objReturn;
        }
    }
}
