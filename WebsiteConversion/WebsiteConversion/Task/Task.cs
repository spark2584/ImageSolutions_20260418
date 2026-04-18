using NetSuiteLibrary;
using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteConversion.Task
{
    public class Task : NetSuiteBase
    {
        public bool CreateAccount(string websiteid, string parentid, string createdby, string division, string customerinternalid, string searchid)
        {
            try
            {
                //Create Website Group for the Division
                ImageSolutions.Website.WebsiteGroup WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                ImageSolutions.Website.WebsiteGroupFilter WebsiteGroupFilter = new ImageSolutions.Website.WebsiteGroupFilter();
                WebsiteGroupFilter.GroupName = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.GroupName.SearchString = division;
                WebsiteGroupFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteGroupFilter.WebsiteID.SearchString = websiteid;
                WebsiteGroup = ImageSolutions.Website.WebsiteGroup.GetWebsiteGroup(WebsiteGroupFilter);
                if (WebsiteGroup == null)
                {
                    WebsiteGroup = new ImageSolutions.Website.WebsiteGroup();
                    WebsiteGroup.WebsiteID = websiteid;
                    WebsiteGroup.GroupName = division;
                    WebsiteGroup.CreatedBy = createdby;
                    WebsiteGroup.Create();
                }

                //Create Division Account - with Customer Internal ID of parent NS customer 
                ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account();
                ImageSolutions.Account.AccountFilter AccountFilter = new ImageSolutions.Account.AccountFilter();
                AccountFilter.AccountName = new Database.Filter.StringSearch.SearchFilter();
                AccountFilter.AccountName.SearchString = division;
                AccountFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                AccountFilter.WebsiteID.SearchString = websiteid;
                Account = ImageSolutions.Account.Account.GetAccount(AccountFilter);
                if (Account == null)
                {
                    Account = new ImageSolutions.Account.Account();
                    Account.WebsiteID = websiteid;
                    Account.CustomerInternalID = customerinternalid;
                    Account.ParentID = parentid;
                    Account.AccountName = division;
                    Account.DefaultWebsiteGroupID = WebsiteGroup.WebsiteGroupID;
                    Account.CreatedBy = createdby;
                    Account.Create();
                }

                List<NetSuiteLibrary.com.netsuite.webservices.Customer> NSCustomers = GetCustomerSavedSearch(searchid);

                foreach (NetSuiteLibrary.com.netsuite.webservices.Customer _Customer in NSCustomers)
                {
                    //Create UserInfo
                    ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                    ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                    UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                    UserInfoFilter.EmailAddress.SearchString = _Customer.email;
                    UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);
                    if (UserInfo == null)
                    {
                        UserInfo = new ImageSolutions.User.UserInfo();
                        if(_Customer.isPerson)
                        {
                            UserInfo.FirstName = _Customer.firstName;
                            UserInfo.LastName = _Customer.lastName;
                        }
                        else
                        {
                            UserInfo.FirstName = _Customer.companyName.Split(' ').FirstOrDefault();
                            UserInfo.LastName = string.Join(" ", _Customer.companyName.Split(' ').Skip(1));
                        }
                        UserInfo.EmailAddress = _Customer.email;
                        UserInfo.Password = "ImageSolutions$123";
                        UserInfo.Create();
                    }
                    else
                    {
                        if (_Customer.isPerson)
                        {
                            UserInfo.FirstName = _Customer.firstName;
                            UserInfo.LastName = _Customer.lastName;
                        }
                        else
                        {
                            UserInfo.FirstName = _Customer.companyName.Split(' ').FirstOrDefault();
                            UserInfo.LastName = string.Join(" ", _Customer.companyName.Split(' ').Skip(1));
                        }
                        UserInfo.Update();
                    }

                    //Create UserWebsite
                    ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID;
                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.WebsiteID.SearchString = websiteid;
                    UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                    ImageSolutions.Payment.PaymentTerm PaymentTerm = null;
                    ImageSolutions.Payment.PaymentTermFilter PaymentTermFilter = null;
                    if (_Customer.terms != null && !string.IsNullOrEmpty(_Customer.terms.internalId))
                    {
                        PaymentTerm = new ImageSolutions.Payment.PaymentTerm();
                        PaymentTermFilter = new ImageSolutions.Payment.PaymentTermFilter();
                        PaymentTermFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        PaymentTermFilter.InternalID.SearchString = _Customer.terms.internalId;
                        PaymentTerm = ImageSolutions.Payment.PaymentTerm.GetPaymentTerm(PaymentTermFilter);

                        if(PaymentTerm == null)
                        {
                            throw new Exception(String.Format("Missing Payment Term: internal id - ", _Customer.terms.internalId));
                        }
                    }

                    if (UserWebsite == null)
                    {
                        UserWebsite = new ImageSolutions.User.UserWebsite();
                        UserWebsite.UserInfoID = UserInfo.UserInfoID;
                        UserWebsite.WebsiteID = websiteid;
                        UserWebsite.CustomerInternalID = _Customer.internalId;
                        UserWebsite.DisablePlaceOrder = true;
                        if (PaymentTerm != null && !string.IsNullOrEmpty(PaymentTerm.PaymentTermID))
                        {
                            UserWebsite.PaymentTermID = PaymentTerm.PaymentTermID;
                        }
                        UserWebsite.CreatedBy = createdby;
                        UserWebsite.Create();
                    }
                    else
                    {
                        //UserWebsite.CustomerInternalID = _Customer.internalId;
                        //ImageSolutions.Payment.PaymentTerm PaymentTerm = new ImageSolutions.Payment.PaymentTerm();
                        //ImageSolutions.Payment.PaymentTermFilter PaymentTermFilter = new ImageSolutions.Payment.PaymentTermFilter();
                        //PaymentTermFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        //PaymentTermFilter.InternalID.SearchString = _Customer.terms.internalId;
                        //PaymentTerm = ImageSolutions.Payment.PaymentTerm.GetPaymentTerm(PaymentTermFilter);
                        if (PaymentTerm != null && !string.IsNullOrEmpty(PaymentTerm.PaymentTermID))
                        {
                            UserWebsite.PaymentTermID = PaymentTerm.PaymentTermID;
                        }
                        UserWebsite.Update();
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
                        UserAccount.WebsiteGroupID = Account.DefaultWebsiteGroupID;
                        UserAccount.CreatedBy = createdby;
                        UserAccount.Create();
                    }

                    //Create Existing Budget
                    double? BudgetRemainingAmount = NetSuiteHelper.GetDoubleCustomFieldValue(_Customer, "custentity_fy_budget_remaining");
                    double? BudgetStartAmount = NetSuiteHelper.GetDoubleCustomFieldValue(_Customer, "custentity_fy_budget_amount");
                    ListOrRecordRef BudgetProgramRecordRef = NetSuiteHelper.GetSelectCustomFieldValue(_Customer, "custentity_budget_program");
                    if (BudgetRemainingAmount != null && Convert.ToDecimal(BudgetRemainingAmount) > 0 && BudgetProgramRecordRef != null)
                    {
                        string BudgetProgram = NetSuiteHelper.GetSelectCustomFieldValue(_Customer, "custentity_budget_program").internalId;
                        DateTime? BudgetStart = NetSuiteHelper.GetDateCustomFieldValue(_Customer, "custentity_budget_program_start_date");
                        DateTime? BudgetEnd = NetSuiteHelper.GetDateCustomFieldValue(_Customer, "custentity_budget_program_end_date");

                        if (!string.IsNullOrEmpty(BudgetProgram) && BudgetStart != null && BudgetEnd != null)
                        {
                            string strBudgetName = string.Format("{0} - {1}", UserInfo.EmailAddress, BudgetProgram);

                            ImageSolutions.Budget.Budget Budget = new ImageSolutions.Budget.Budget();
                            ImageSolutions.Budget.BudgetFilter BudgetFilter = new ImageSolutions.Budget.BudgetFilter();
                            BudgetFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                            BudgetFilter.WebsiteID.SearchString = websiteid;
                            BudgetFilter.BudgetName = new Database.Filter.StringSearch.SearchFilter();
                            BudgetFilter.BudgetName.SearchString = strBudgetName;
                            Budget = ImageSolutions.Budget.Budget.GetBudget(BudgetFilter);

                            if (Budget == null)
                            {
                                Budget = new ImageSolutions.Budget.Budget();
                                Budget.BudgetName = strBudgetName;
                                Budget.WebsiteID = websiteid;
                                Budget.StartDate = Convert.ToDateTime(BudgetStart);
                                Budget.EndDate = Convert.ToDateTime(BudgetEnd);
                                Budget.BudgetAmount = Convert.ToDouble(BudgetStartAmount);
                                Budget.CreatedBy = createdby;
                                Budget.Create();

                                ImageSolutions.Budget.BudgetAssignment BudgetAssignment = new ImageSolutions.Budget.BudgetAssignment();
                                BudgetAssignment.WebsiteID = websiteid;
                                BudgetAssignment.BudgetID = Budget.BudgetID;
                                BudgetAssignment.UserWebsiteID = UserWebsite.UserWebsiteID;
                                BudgetAssignment.CreatedBy = createdby;
                                BudgetAssignment.Create();
                            }
                            else
                            {
                                Budget.StartDate = Convert.ToDateTime(BudgetStart);
                                Budget.EndDate = Convert.ToDateTime(BudgetEnd);
                                Budget.BudgetAmount = Convert.ToDouble(BudgetStartAmount);
                                Budget.Update();
                            }
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
                        if(SearchResult.searchRowList != null)
                        {
                            foreach (CustomerSearchRow _CustomerSearchRow in SearchResult.searchRowList)
                            {
                                count++;
                                Console.WriteLine(string.Format("{0}/{1}", count, SearchResult.totalRecords));

                                if (_CustomerSearchRow is CustomerSearchRow && !dicInternalIds.ContainsKey(_CustomerSearchRow.basic.internalId[0].searchValue.internalId))
                                {
                                    //int intCustomFieldIndex = 0;

                                    //Customer Customer = new Customer();
                                    //Customer.internalId = _CustomerSearchRow.basic.internalId[0].searchValue.internalId;
                                    //Customer.isPerson = Convert.ToBoolean(_CustomerSearchRow.basic.isPerson[0].searchValue);
                                    //if (Customer.isPerson)
                                    //{
                                    //    Customer.firstName = _CustomerSearchRow.basic.firstName == null ? "N/A" : Convert.ToString(_CustomerSearchRow.basic.firstName[0].searchValue);
                                    //    Customer.lastName = _CustomerSearchRow.basic.lastName == null ? "N/A" : Convert.ToString(_CustomerSearchRow.basic.lastName[0].searchValue);
                                    //}
                                    //else
                                    //{
                                    //    Customer.companyName = _CustomerSearchRow.basic.companyName == null ? "N/A" : Convert.ToString(_CustomerSearchRow.basic.companyName[0].searchValue);
                                    //}
                                    //Customer.email = Convert.ToString(_CustomerSearchRow.basic.email[0].searchValue);
                                    //Customer.terms = _CustomerSearchRow.basic.terms[0].searchValue;
                                    //string BudgetProgram = NetSuiteHelper.GetSearchColumnStringCustomFieldValue(_CustomerSearchRow, "custentity_budget_program");
                                    //string BudgetRemaining = NetSuiteHelper.GetSearchColumnStringCustomFieldValue(_CustomerSearchRow, "custentity_fy_budget_remaining");

                                    string strInternalID = _CustomerSearchRow.basic.internalId[0].searchValue.internalId;
                                    //Customers.Add(LoadNetSuiteCustomer(strInternalID));

                                    dicInternalIds.Add(strInternalID, strInternalID);
                                    Customer Customer = LoadNetSuiteCustomer(strInternalID);
                                    if(_CustomerSearchRow.basic.terms != null)
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
