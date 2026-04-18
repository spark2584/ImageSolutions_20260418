using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Ahold
{
    public class AssignUserToGroup : NetSuiteBase
    {
        public bool Execute()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {

                //objConn = new SqlConnection(Database.DefaultConnectionString);
                //objConn.Open();
                //objTran = objConn.BeginTransaction();

                //Ahold Asset Protection Customer List | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281006&whence=
                //AssignUserToGroupFromSearch("Ahold Asset Protection Audience Group", "281006"); //, objConn, objTran);;

                //AholdGear (View All) | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281307&whence=
                //AssignUserToGroupFromSearch("AholdGear (View All)", "281307"); //, objConn, objTran);

                //Aholdgear.com - Food Lion RPA & Hannaford Test | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281308&whence=
                //AssignUserToGroupFromSearch("Aholdgear.Com - Food Lion RPA & Hannaford Test", "281308"); //, objConn, objTran);

                //Bfresh Audience 6.7.21 | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281709&whence=
                //AssignUserToGroupFromSearch("Bfresh Audience 6.7.21", "281709"); //, objConn, objTran);

                //Food Lion Soft Shell Fit Kit | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281712&whence=
                //AssignUserToGroupFromSearch("Food Lion Soft Shell Fit Kit", "281712"); //, objConn, objTran);

                //Group: Aholdgear.Com - Food Lion Only -> All Users are inactive

                //Aholdgear.com - Food Lion Wave 8 | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281714&whence=
                //AssignUserToGroupFromSearch("Aholdgear.com - Food Lion Wave 8", "281714");//, objConn, objTran);

                //*Ahold Store Customer Search | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281715&whence=
                //AssignUserToGroupFromSearch("Food Lion Grocery Store Dynamic Group", "281715");//, objConn, objTran);

                //Food Lion static group 11.11.21 - 1 | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281717&whence=
                //Food Lion static group 11.11.21 - 2 | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281718&whence=
                //Food Lion static group 11.11.21 - 3 | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281719&whence=
                //Food Lion static group 11.11.21 - 4 | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281720&whence=
                //AssignUserToGroupFromSearch("Food Lion static group 11.11.21", "281717");//, objConn, objTran);
                //AssignUserToGroupFromSearch("Food Lion static group 11.11.21", "281718");//, objConn, objTran);
                //AssignUserToGroupFromSearch("Food Lion static group 11.11.21", "281719");//, objConn, objTran);
                //AssignUserToGroupFromSearch("Food Lion static group 11.11.21", "281720");//, objConn, objTran);

                //Food Lion Grocery Store Store Search | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281721&whence=
                //AssignUserToGroupFromSearch("Food Lion Grocery Dynamic Store Search", "281721"); //, objConn, objTran);

                //Food Lion Omnichannel Wave Audience Group | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281722&whence=
                //AssignUserToGroupFromSearch("Food Lion Omnichannel Wave Audience Group", "281722");//, objConn, objTran);

                //DM - Ahold Giant Carlisle Website Audience Saved Search | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281723&whence=
                //AssignUserToGroupFromSearch("Ahold Giant Carlisle Website Audience List-2.16.16", "281723");//, objConn, objTran);

                //Peapod Approved Website Audience Group | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281724&whence=
                //AssignUserToGroupFromSearch("Peapod Approved Website Audience Group", "281724"); //, objConn, objTran);

                //DM - Ahold Giant Landover Website Audience Saved Search | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281725&whence=
                //AssignUserToGroupFromSearch("Ahold Giant Landover Website Audience - 2.16.16", "281725"); //, objConn, objTran);

                //Hannaford Store Customer Search | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281726&whence=
                AssignUserToGroupFromSearch("Hannaford Dynamic Group", "281726"); //, objConn, objTran);

                //Ahold Giant Heirloom Stores | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281727&whence=
                //AssignUserToGroupFromSearch("Ahold Giant Heirloom Stores", "281727"); //, objConn, objTran);

                //Jessup Aholdgear.Com | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281728&whence=
                //AssignUserToGroupFromSearch("Jessup Aholdgear.Com", "281728"); //, objConn, objTran);

                //DM - Ahold SS New England Website Audience Saved Search | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281729&whence=
                //AssignUserToGroupFromSearch("Ahold SS New England Website Audience List-2.16.16", "281729"); //, objConn, objTran);

                //Stop & Shop - Click & Collect Zone Managers | DYN - https://acct88641.app.netsuite.com/app/common/search/searchresults.nl?searchid=281730&whence=
                //AssignUserToGroupFromSearch("Stop & Shop - Click & Collect Zone Managers Group", "281730");//, objConn, objTran);

                //objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }

            return true;
        }

        public bool AssignUserToGroupFromSearch(string groupname, string searchid) //, SqlConnection objconn, SqlTransaction objtran)
        {
            try
            {
                //Create Website Group for the Division
                ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.GroupName.SearchString = groupname;
                WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);
                if (WebsiteGroup == null)
                {
                    throw new Exception("Group Not Found");
                }

                List<NetSuiteLibrary.com.netsuite.webservices.Customer> NSCustomers = GetCustomerSavedSearch(searchid);

                foreach (NetSuiteLibrary.com.netsuite.webservices.Customer _Customer in NSCustomers)
                {
                    SqlConnection objConn = null;
                    SqlTransaction objTran = null;

                    try
                    {
                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

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
                        UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);

                        //Search by Customer ID
                        //UserWebsiteFilter.CustomerInternalID = new Database.Filter.StringSearch.SearchFilter();
                        //UserWebsiteFilter.CustomerInternalID.SearchString = _Customer.internalId;

                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                            UserWebsite.CustomerInternalID = _Customer.internalId;
                            UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserWebsite.Create(objConn, objTran);
                        }

                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                        if (_Customer.parent != null && !string.IsNullOrEmpty(_Customer.parent.internalId))
                        {
                            string strParentName = _Customer.parent.name;

                            string[] Accounts = strParentName.Split(':');
                            string strParentID = string.Empty;
                            for (int i = 0; i < Accounts.Length; i++)
                            {
                                string strAccount = Accounts[i].Trim();
                                //Create Division Account - with Customer Internal ID of parent NS customer 
                                ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                                AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.AccountName.SearchString = Convert.ToString(strAccount);
                                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);

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
                                    Account.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                                    Account.CustomerInternalID = i == Accounts.Length - 1 ? _Customer.parent.internalId : string.Empty;
                                    Account.ParentID = strParentID;
                                    Account.AccountName = strAccount;
                                    Account.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    Account.Create(objConn, objTran);
                                }
                                else
                                {
                                    if (i == Accounts.Length - 1)
                                    {
                                        Account.CustomerInternalID = string.IsNullOrEmpty(Account.CustomerInternalID) ? _Customer.parent.internalId : String.Empty;
                                        Account.Update(objConn, objTran);
                                    }
                                }

                                strParentID = Account.AccountID;
                            }

                            ////Create Division Account - with Customer Internal ID of parent NS customer 
                            //ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                            //AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                            //AccountFilter.AccountName.SearchString = Convert.ToString(_Customer.parent.name);
                            //AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            //AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                            //Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);
                            //if (Account == null)
                            //{
                            //    Account = new ImageSolutions.Account.Account();
                            //    Account.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                            //    Account.CustomerInternalID = _Customer.parent.internalId;
                            //    Account.ParentID = "";
                            //    Account.AccountName = Convert.ToString(_Customer.parent.name);
                            //    Account.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            //    Account.Create();

                            //    //Parent of Parent?
                            //}
                        }
                        else
                        {
                            throw new Exception(String.Format("Parent Not Found: internalid {0} - {1}", _Customer.internalId, _Customer.email));
                            //Account = new ImageSolutions.Account.Account("6914");  //Ahold
                        }

                        //Create UserAccount - UserInfo/UserWebsite/UserWebsiteGroup
                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccountFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.AccountID.SearchString = Account.AccountID;
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

                        objTran.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));

                        if (objTran != null && objTran.Connection != null) objTran.Rollback();
                        //throw ex;
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
                WebsiteGroupFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
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
                        UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                        //Search by Customer ID
                        //UserWebsiteFilter.CustomerInternalID = new Database.Filter.StringSearch.SearchFilter();
                        //UserWebsiteFilter.CustomerInternalID.SearchString = _Customer.internalId;

                        UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);
                        if (UserWebsite == null)
                        {
                            UserWebsite = new ImageSolutions.User.UserWebsite();
                            UserWebsite.UserInfoID = UserInfo.UserInfoID;
                            UserWebsite.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                            UserWebsite.CustomerInternalID = Convert.ToString(objRead["InternalID"]);
                            UserWebsite.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserWebsite.Create(objconn, objtran);
                        }

                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                        if (!string.IsNullOrEmpty(Convert.ToString(objRead["ParentInternalID"])))
                        {
                            string strParentName = Convert.ToString(objRead["ParentName"]);

                            string[] Accounts = strParentName.Split(':');
                            string strParentID = string.Empty;
                            for (int i = 0; i < Accounts.Length; i++)
                            {
                                string strAccount = Accounts[i].Trim();
                                //Create Division Account - with Customer Internal ID of parent NS customer 
                                ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                                AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.AccountName.SearchString = Convert.ToString(strAccount);
                                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                AccountFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);

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
                                    Account.WebsiteID = Convert.ToString(ConfigurationManager.AppSettings["AholdWebsiteID"]);
                                    Account.CustomerInternalID = i == Accounts.Length - 1 ? Convert.ToString(objRead["ParentInternalID"]) : string.Empty;
                                    Account.ParentID = strParentID;
                                    Account.AccountName = strAccount;
                                    Account.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                                    Account.Create(objconn, objtran);
                                }
                                else
                                {
                                    if (i == Accounts.Length - 1)
                                    {
                                        Account.CustomerInternalID = string.IsNullOrEmpty(Account.CustomerInternalID) ? Convert.ToString(objRead["ParentInternalID"]) : String.Empty;
                                        Account.Update(objconn, objtran);
                                    }
                                }

                                strParentID = Account.AccountID;
                            }
                        }
                        else
                        {
                            throw new Exception(String.Format("Parent Not Found: internalid {0} - {1}", Convert.ToString(objRead["InternalID"]), Convert.ToString(objRead["Email"])));
                            //Account = new ImageSolutions.Account.Account("6914");  //Ahold
                        }

                        //Create UserAccount - UserInfo/UserWebsite/UserWebsiteGroup
                        ImageSolutions.User.UserAccount UserAccount = new ImageSolutions.User.UserAccount();
                        ImageSolutions.User.UserAccountFilter UserAccountFilter = new ImageSolutions.User.UserAccountFilter();
                        UserAccountFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.UserWebsiteID.SearchString = UserWebsite.UserWebsiteID;
                        UserAccountFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                        UserAccountFilter.WebsiteGroupID.SearchString = WebsiteGroup.WebsiteGroupID;
                        UserAccount = ImageSolutions.User.UserAccount.GetUserAccount(UserAccountFilter);
                        if (UserAccount == null)
                        {
                            UserAccount = new ImageSolutions.User.UserAccount();
                            UserAccount.UserWebsiteID = UserWebsite.UserWebsiteID;
                            UserAccount.AccountID = Account.AccountID;
                            UserAccount.WebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                            UserAccount.CreatedBy = Convert.ToString(ConfigurationManager.AppSettings["CreatedByUserInfoID"]);
                            UserAccount.Create(objconn, objtran);
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
