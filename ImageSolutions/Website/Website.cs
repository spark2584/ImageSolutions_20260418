using ImageSolutions.Custom;
using ImageSolutions.Item;
using ImageSolutions.User;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSolutions.Website
{
    public class Website : ISBase.BaseClass
    {
        public enum enumItemDisplayType
        {
            [Description("Grid")]
            Grid,
            [Description("List")]
            List
        }
        public string WebsiteID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(WebsiteID); } }
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Domain { get; set; }
        public string LogoPath { get; set; }
        public string BannerInternalID { get; set; }
        public string EmailLogoPath { get; set; }
        public string BannerPath { get; set; }
        public bool UserRegistration { get; set; }
        public bool UserRegistrationKeyRequired { get; set; }
        public bool UserApprovalRequired { get; set; }
        public bool AccountRegistration { get; set; }
        public bool AccountRegistrationKeyRequired { get; set; }
        public bool AccountApprovalRequired { get; set; }
        public bool ShowAvailableInventory { get; set; }
        public bool ShowSalesDescription { get; set; }
        public bool ShowDetailedDescription { get; set; }
        public string DefaultWebsiteGroupID { get; set; }
        public bool OrderApprovalRequired { get; set; }
        public bool EnableEmployeeCredit { get; set; }
        public bool MustUseExistingEmployeeCredit { get; set; }
        public bool EnableCreditCard { get; set; }
        public bool EnablePaymentTerm { get; set; }
        public bool EnablePromoCode { get; set; }
        public bool EnablePasswordReset { get; set; }
        public string PasswordHint { get; set; }
        public string ItemDisplayType { get; set; }
        public string ProductDetailDisplayType { get; set; }
        public string BillingAddressTransID { get; set; }
        public string DeliveryAddressTransID { get; set; }
        public string StartingPath { get; set; }
        public string NetSuiteDiscountItemInternalID { get; set; }
        public string NetSuiteParentCustomerInternalID { get; set; }
        public string DefaultPaymentTermID { get; set; }

        public bool DisplaySupportFAQ { get; set; }
        public bool DisplayContactUsAddress { get; set; }
        public bool DisplayContactUsPhoneNumber { get; set; }
        public string DefaultCheckoutPaymentMethod { get; set; }

        public bool DisplayDefaultGroupPerAccount { get; set; }
        public bool DisplaySubCategory { get; set; }
        public bool DisplayAttributeFilter { get; set; }
        public bool DisplayLeftNavigation { get; set; }
        public bool CombineWebsiteGroup { get; set; }

        public string DefaultSizeChartPath { get; set; }

        public bool AllowNameChange { get; set; }

        public string RegistrationPath { get; set; }

        public bool IsPunchout { get; set; }
        public string DisplayUserPermission { get; set; }
        public bool UseDomain { get; set; }

        public string BudgetAlias { get; set; }
        public string OverBudgetMessage { get; set; }

        public string RegistrationFormPath { get; set; }

        public bool DisableAddressValidation { get; set; }

        public bool EnableSSO { get; set; }

        public bool DisallowBackOrder { get; set; }
        public bool HideEmail { get; set; }
        public bool EnableZendesk { get; set; }

        public bool HideMyAccountOrderReport { get; set; }
        public bool HideMyAccountOrderApproval { get; set; }
        public bool HideAdminOrderApproval { get; set; }
        public bool HideReturnPolicy { get; set; }
        public bool DisplayNonInventoryMessage { get; set; }
        public decimal? CreditCardLimitPerOrder { get; set; }
        public bool IsOneBudgetPerUser { get; set; }
        public bool AllowGuestCheckout { get; set; }
        public string DefaultGuestAccountID { get; set; }

        public string CurrencyConvert { get; set; }
        public decimal? CurrentyConvertPercentage { get; set; }

        public bool AllowBackOrderForAllItems { get; set; }
        public bool EnableLocalize { get; set; }
        public string CorporateAddressBookID { get; set; }
        public decimal? DiscountPerItem { get; set; }
        public bool AllowPartialShipping { get; set; }

        public bool EnableEmailOptIn { get; set; }

        public bool ReloadAfterAddToCart { get; set; }

        public bool EnablePackagePayment { get; set; }

        public bool EnableIPD { get; set; }
        public decimal IPDShippingAdjustPercent { get; set; }
        public decimal IPDTaxAdjustPercent { get; set; }

        public bool EnableSMSOptIn { get; set; }
        public bool DisplayTariffCharge { get; set; }

        public bool DisplayNewAccountSetupForm { get; set; }
        public bool SuggestedSelling { get; set; }

        public string BannerHTML { get; set; }
        public string FeaturedProductHTML { get; set; }

        public bool InActive { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        private List<WebsiteTab> mWebsiteTabs = null;
        public List<WebsiteTab> WebsiteTabs
        {
            get
            {
                if (mWebsiteTabs == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    WebsiteTabFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteTabFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        //objFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                        //objFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                        mWebsiteTabs = WebsiteTab.GetWebsiteTabs(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteTabs;
            }
        }

        private List<Account.Account> mAccounts = null;
        public List<Account.Account> Accounts
        {
            get
            {
                if (mAccounts == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    Account.AccountFilter objFilter = null;

                    try
                    {
                        objFilter = new Account.AccountFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mAccounts = Account.Account.GetAccounts(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mAccounts;
            }
        }

        private List<Budget.Budget> mBudgets = null;
        public List<Budget.Budget> Budgets
        {
            get
            {
                if (mBudgets == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    Budget.BudgetFilter objFilter = null;

                    try
                    {
                        objFilter = new Budget.BudgetFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mBudgets = Budget.Budget.GetBudgets(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mBudgets;
            }
        }

        private List<Item.ItemWebsite> mItemWebsites = null;
        public List<Item.ItemWebsite> ItemWebsites
        {
            get
            {
                if (mItemWebsites == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    ItemWebsiteFilter objFilter = null;

                    try
                    {
                        objFilter = new ItemWebsiteFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mItemWebsites = Item.ItemWebsite.GetItemWebsites(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mItemWebsites;
            }
        }

        private List<UserWebsite> mUserWebsites = null;
        public List<UserWebsite> UserWebsites
        {
            get
            {
                if (mUserWebsites == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    UserWebsiteFilter objFilter = null;

                    try
                    {
                        objFilter = new UserWebsiteFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mUserWebsites = UserWebsite.GetUserWebsites(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mUserWebsites;
            }
        }

        private List<WebsiteGroup> mWebsiteGroups = null;
        public List<WebsiteGroup> WebsiteGroups
        {
            get
            {
                if (mWebsiteGroups == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    WebsiteGroupFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteGroupFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mWebsiteGroups = WebsiteGroup.GetWebsiteGroups(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteGroups;
            }
        }
        private List<WebsiteCountry> mWebsiteCountries = null;
        public List<WebsiteCountry> WebsiteCountries
        {
            get
            {
                if (mWebsiteCountries == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    WebsiteCountryFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteCountryFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mWebsiteCountries = WebsiteCountry.GetWebsiteCountries(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteCountries;
            }
        }
        private List<CustomList> mCustomLists = null;
        public List<CustomList> CustomLists
        {
            get
            {
                if (mCustomLists == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    CustomListFilter objFilter = null;

                    try
                    {
                        objFilter = new CustomListFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mCustomLists = CustomList.GetCustomLists(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mCustomLists;
            }
        }
        private List<SalesOrder.SalesOrder> mSalesOrders = null;
        public List<SalesOrder.SalesOrder> SalesOrders
        {
            get
            {
                if (mSalesOrders == null &&  !string.IsNullOrEmpty(WebsiteID))
                {
                    SalesOrder.SalesOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new SalesOrder.SalesOrderFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mSalesOrders = SalesOrder.SalesOrder.GetSalesOrders(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mSalesOrders;
            }
        }

        private List<WebsiteShippingService> mWebsiteShippingServices = null;
        public List<WebsiteShippingService> WebsiteShippingServices
        {
            get
            {
                if (mWebsiteShippingServices == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    WebsiteShippingServiceFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteShippingServiceFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mWebsiteShippingServices = WebsiteShippingService.GetWebsiteShippingServices(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteShippingServices;
            }
        }

        private List<WebsiteMessage> mWebsiteMessages = null;
        public List<WebsiteMessage> WebsiteMessages
        {
            get
            {
                if (mWebsiteMessages == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    WebsiteMessageFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteMessageFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mWebsiteMessages = WebsiteMessage.GetWebsiteMessages(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteMessages;
            }
        }

        private UserInfo mCreatedByUser = null;
        public UserInfo CreatedByUser
        {
            get
            {
                if (mCreatedByUser == null && !string.IsNullOrEmpty(CreatedBy))
                {
                    mCreatedByUser = new UserInfo(CreatedBy);
                }
                return mCreatedByUser;
            }
        }
        private Address.AddressTrans mBillingAddress = null;
        public Address.AddressTrans BillingAddress
        {
            get
            {
                if (mBillingAddress == null && !string.IsNullOrEmpty(BillingAddressTransID))
                {
                    mBillingAddress = new Address.AddressTrans(BillingAddressTransID);
                }
                return mBillingAddress;
            }
        }
        public Address.AddressTrans mDeliveryAddress = null;
        public Address.AddressTrans DeliveryAddress
        {
            get
            {
                if (mDeliveryAddress == null && !string.IsNullOrEmpty(DeliveryAddressTransID))
                {
                    mDeliveryAddress = new Address.AddressTrans(DeliveryAddressTransID);
                }
                return mDeliveryAddress;
            }
        }

        private WebsiteGroup mDefaultWebsiteGroup = null;
        public WebsiteGroup DefaultWebsiteGroup
        {
            get
            {
                if (mDefaultWebsiteGroup == null && !string.IsNullOrEmpty(DefaultWebsiteGroupID))
                {
                    mDefaultWebsiteGroup = new WebsiteGroup(DefaultWebsiteGroupID);
                }
                return mDefaultWebsiteGroup;
            }
        }
        private Account.Account mDefaultGuestAccount = null;
        public Account.Account DefaultGuestAccount
        {
            get
            {
                if (mDefaultGuestAccount == null && !string.IsNullOrEmpty(DefaultGuestAccountID))
                {
                    mDefaultGuestAccount = new Account.Account(DefaultGuestAccountID);
                }
                return mDefaultGuestAccount;
            }
        }
        private List<WebsiteUsefulLink> mWebsiteUsefulLinks = null;
        public List<WebsiteUsefulLink> WebsiteUsefulLinks
        {
            get
            {
                if (mWebsiteUsefulLinks == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    WebsiteUsefulLinkFilter objFilter = null;

                    try
                    {
                        objFilter = new WebsiteUsefulLinkFilter();
                        objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.WebsiteID.SearchString = WebsiteID;
                        mWebsiteUsefulLinks = WebsiteUsefulLink.GetWebsiteUsefulLinks(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFilter = null;
                    }
                }
                return mWebsiteUsefulLinks;
            }
        }
        public Website()
        {
        }
        public Website(string WebsiteID)
        {
            this.WebsiteID = WebsiteID;
            Load();
        }
        public Website(DataRow objRow)
        {
            Load(objRow);
        }

        protected override void Load()
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM Website (NOLOCK) " +
                         "WHERE WebsiteID=" + Database.HandleQuote(WebsiteID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("WebsiteID=" + WebsiteID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }

        protected void Load(string GUID)
        {
            base.Load();

            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = "SELECT * " +
                         "FROM Website (NOLOCK) " +
                         "WHERE GUID=" + Database.HandleQuote(GUID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("GUID=" + GUID + " is not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
        }
        protected void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("GUID")) GUID = Convert.ToString(objRow["GUID"]);
                if (objColumns.Contains("Name")) Name = Convert.ToString(objRow["Name"]);
                if (objColumns.Contains("Abbreviation")) Abbreviation = Convert.ToString(objRow["Abbreviation"]);
                if (objColumns.Contains("Domain")) Domain = Convert.ToString(objRow["Domain"]);
                if (objColumns.Contains("LogoPath")) LogoPath = Convert.ToString(objRow["LogoPath"]);
                if (objColumns.Contains("BannerInternalID")) BannerInternalID = Convert.ToString(objRow["BannerInternalID"]);
                if (objColumns.Contains("EmailLogoPath")) EmailLogoPath = Convert.ToString(objRow["EmailLogoPath"]);
                if (objColumns.Contains("BannerPath")) BannerPath = Convert.ToString(objRow["BannerPath"]);
                if (objColumns.Contains("UserRegistration")) UserRegistration = Convert.ToBoolean(objRow["UserRegistration"]);
                if (objColumns.Contains("UserRegistrationKeyRequired")) UserRegistrationKeyRequired = Convert.ToBoolean(objRow["UserRegistrationKeyRequired"]);
                if (objColumns.Contains("UserApprovalRequired")) UserApprovalRequired = Convert.ToBoolean(objRow["UserApprovalRequired"]);
                if (objColumns.Contains("AccountRegistration")) AccountRegistration = Convert.ToBoolean(objRow["AccountRegistration"]);
                if (objColumns.Contains("AccountRegistrationKeyRequired")) AccountRegistrationKeyRequired = Convert.ToBoolean(objRow["AccountRegistrationKeyRequired"]);
                if (objColumns.Contains("AccountApprovalRequired")) AccountApprovalRequired = Convert.ToBoolean(objRow["AccountApprovalRequired"]);
                if (objColumns.Contains("ShowAvailableInventory")) ShowAvailableInventory = Convert.ToBoolean(objRow["ShowAvailableInventory"]);
                if (objColumns.Contains("ShowSalesDescription")) ShowSalesDescription = Convert.ToBoolean(objRow["ShowSalesDescription"]);
                if (objColumns.Contains("ShowDetailedDescription")) ShowDetailedDescription = Convert.ToBoolean(objRow["ShowDetailedDescription"]);
                if (objColumns.Contains("DefaultWebsiteGroupID")) DefaultWebsiteGroupID = Convert.ToString(objRow["DefaultWebsiteGroupID"]);
                if (objColumns.Contains("OrderApprovalRequired")) OrderApprovalRequired = Convert.ToBoolean(objRow["OrderApprovalRequired"]);
                if (objColumns.Contains("EnableEmployeeCredit")) EnableEmployeeCredit = Convert.ToBoolean(objRow["EnableEmployeeCredit"]);
                if (objColumns.Contains("MustUseExistingEmployeeCredit")) MustUseExistingEmployeeCredit = Convert.ToBoolean(objRow["MustUseExistingEmployeeCredit"]);
                if (objColumns.Contains("EnableCreditCard")) EnableCreditCard = Convert.ToBoolean(objRow["EnableCreditCard"]);
                if (objColumns.Contains("EnablePaymentTerm")) EnablePaymentTerm = Convert.ToBoolean(objRow["EnablePaymentTerm"]);
                if (objColumns.Contains("EnablePromoCode")) EnablePromoCode = Convert.ToBoolean(objRow["EnablePromoCode"]);
                if (objColumns.Contains("EnablePasswordReset")) EnablePasswordReset = Convert.ToBoolean(objRow["EnablePasswordReset"]);
                if (objColumns.Contains("PasswordHint")) PasswordHint = Convert.ToString(objRow["PasswordHint"]);
                if (objColumns.Contains("ItemDisplayType")) ItemDisplayType = Convert.ToString(objRow["ItemDisplayType"]);
                if (objColumns.Contains("ProductDetailDisplayType")) ProductDetailDisplayType = Convert.ToString(objRow["ProductDetailDisplayType"]);
                if (objColumns.Contains("BillingAddressTransID")) BillingAddressTransID = Convert.ToString(objRow["BillingAddressTransID"]);
                if (objColumns.Contains("DeliveryAddressTransID")) DeliveryAddressTransID = Convert.ToString(objRow["DeliveryAddressTransID"]);
                if (objColumns.Contains("StartingPath")) StartingPath = Convert.ToString(objRow["StartingPath"]);
                if (objColumns.Contains("NetSuiteDiscountItemInternalID")) NetSuiteDiscountItemInternalID = Convert.ToString(objRow["NetSuiteDiscountItemInternalID"]);
                if (objColumns.Contains("NetSuiteParentCustomerInternalID")) NetSuiteParentCustomerInternalID = Convert.ToString(objRow["NetSuiteParentCustomerInternalID"]);
                if (objColumns.Contains("DefaultPaymentTermID")) DefaultPaymentTermID = Convert.ToString(objRow["DefaultPaymentTermID"]);
                if (objColumns.Contains("DisplaySupportFAQ") && objRow["DisplaySupportFAQ"] != DBNull.Value) DisplaySupportFAQ = Convert.ToBoolean(objRow["DisplaySupportFAQ"]);
                if (objColumns.Contains("DisplayContactUsAddress") && objRow["DisplayContactUsAddress"] != DBNull.Value) DisplayContactUsAddress = Convert.ToBoolean(objRow["DisplayContactUsAddress"]);
                if (objColumns.Contains("DisplayContactUsPhoneNumber") && objRow["DisplayContactUsPhoneNumber"] != DBNull.Value) DisplayContactUsPhoneNumber = Convert.ToBoolean(objRow["DisplayContactUsPhoneNumber"]);
                if (objColumns.Contains("DefaultCheckoutPaymentMethod")) DefaultCheckoutPaymentMethod = Convert.ToString(objRow["DefaultCheckoutPaymentMethod"]);
                if (objColumns.Contains("DisplayDefaultGroupPerAccount") && objRow["DisplayDefaultGroupPerAccount"] != DBNull.Value) DisplayDefaultGroupPerAccount = Convert.ToBoolean(objRow["DisplayDefaultGroupPerAccount"]);
                if (objColumns.Contains("DisplaySubCategory") && objRow["DisplaySubCategory"] != DBNull.Value) DisplaySubCategory = Convert.ToBoolean(objRow["DisplaySubCategory"]);
                if (objColumns.Contains("DisplayAttributeFilter") && objRow["DisplayAttributeFilter"] != DBNull.Value) DisplayAttributeFilter = Convert.ToBoolean(objRow["DisplayAttributeFilter"]);
                if (objColumns.Contains("DisplayLeftNavigation") && objRow["DisplayLeftNavigation"] != DBNull.Value) DisplayLeftNavigation = Convert.ToBoolean(objRow["DisplayLeftNavigation"]);
                if (objColumns.Contains("CombineWebsiteGroup") && objRow["CombineWebsiteGroup"] != DBNull.Value) CombineWebsiteGroup = Convert.ToBoolean(objRow["CombineWebsiteGroup"]);

                if (objColumns.Contains("DefaultSizeChartPath")) DefaultSizeChartPath = Convert.ToString(objRow["DefaultSizeChartPath"]);

                if (objColumns.Contains("AllowNameChange") && objRow["AllowNameChange"] != DBNull.Value) AllowNameChange = Convert.ToBoolean(objRow["AllowNameChange"]);

                if (objColumns.Contains("RegistrationPath")) RegistrationPath = Convert.ToString(objRow["RegistrationPath"]);
                if (objColumns.Contains("IsPunchout") && objRow["IsPunchout"] != DBNull.Value) IsPunchout = Convert.ToBoolean(objRow["IsPunchout"]);

                if (objColumns.Contains("DisplayUserPermission")) DisplayUserPermission = Convert.ToString(objRow["DisplayUserPermission"]);
                if (objColumns.Contains("UseDomain") && objRow["UseDomain"] != DBNull.Value) UseDomain = Convert.ToBoolean(objRow["UseDomain"]);

                if (objColumns.Contains("BudgetAlias")) BudgetAlias = Convert.ToString(objRow["BudgetAlias"]);
                if (objColumns.Contains("OverBudgetMessage")) OverBudgetMessage = Convert.ToString(objRow["OverBudgetMessage"]);

                if (objColumns.Contains("RegistrationFormPath")) RegistrationFormPath = Convert.ToString(objRow["RegistrationFormPath"]);
                if (objColumns.Contains("DisableAddressValidation") && objRow["DisableAddressValidation"] != DBNull.Value) DisableAddressValidation = Convert.ToBoolean(objRow["DisableAddressValidation"]);

                if (objColumns.Contains("EnableSSO") && objRow["EnableSSO"] != DBNull.Value) EnableSSO = Convert.ToBoolean(objRow["EnableSSO"]);
                if (objColumns.Contains("DisallowBackOrder") && objRow["DisallowBackOrder"] != DBNull.Value) DisallowBackOrder = Convert.ToBoolean(objRow["DisallowBackOrder"]);
                if (objColumns.Contains("HideEmail") && objRow["HideEmail"] != DBNull.Value) HideEmail = Convert.ToBoolean(objRow["HideEmail"]);
                if (objColumns.Contains("EnableZendesk") && objRow["EnableZendesk"] != DBNull.Value) EnableZendesk = Convert.ToBoolean(objRow["EnableZendesk"]);

                if (objColumns.Contains("HideMyAccountOrderReport") && objRow["HideMyAccountOrderReport"] != DBNull.Value) HideMyAccountOrderReport = Convert.ToBoolean(objRow["HideMyAccountOrderReport"]);
                if (objColumns.Contains("HideMyAccountOrderApproval") && objRow["HideMyAccountOrderApproval"] != DBNull.Value) HideMyAccountOrderApproval = Convert.ToBoolean(objRow["HideMyAccountOrderApproval"]);
                if (objColumns.Contains("HideAdminOrderApproval") && objRow["HideAdminOrderApproval"] != DBNull.Value) HideAdminOrderApproval = Convert.ToBoolean(objRow["HideAdminOrderApproval"]);

                if (objColumns.Contains("HideReturnPolicy") && objRow["HideReturnPolicy"] != DBNull.Value) HideReturnPolicy = Convert.ToBoolean(objRow["HideReturnPolicy"]);
                if (objColumns.Contains("DisplayNonInventoryMessage") && objRow["DisplayNonInventoryMessage"] != DBNull.Value) DisplayNonInventoryMessage = Convert.ToBoolean(objRow["DisplayNonInventoryMessage"]);

                if (objColumns.Contains("CreditCardLimitPerOrder") && objRow["CreditCardLimitPerOrder"] != DBNull.Value) CreditCardLimitPerOrder = Convert.ToDecimal(objRow["CreditCardLimitPerOrder"]);
                if (objColumns.Contains("IsOneBudgetPerUser") && objRow["IsOneBudgetPerUser"] != DBNull.Value) IsOneBudgetPerUser = Convert.ToBoolean(objRow["IsOneBudgetPerUser"]);
                
                if (objColumns.Contains("AllowGuestCheckout") && objRow["AllowGuestCheckout"] != DBNull.Value) AllowGuestCheckout = Convert.ToBoolean(objRow["AllowGuestCheckout"]);
                if (objColumns.Contains("DefaultGuestAccountID")) DefaultGuestAccountID = Convert.ToString(objRow["DefaultGuestAccountID"]);

                if (objColumns.Contains("CurrencyConvert")) CurrencyConvert = Convert.ToString(objRow["CurrencyConvert"]);
                if (objColumns.Contains("CurrentyConvertPercentage") && objRow["CurrentyConvertPercentage"] != DBNull.Value) CurrentyConvertPercentage = Convert.ToDecimal(objRow["CurrentyConvertPercentage"]);

                if (objColumns.Contains("AllowBackOrderForAllItems") && objRow["AllowBackOrderForAllItems"] != DBNull.Value) AllowBackOrderForAllItems = Convert.ToBoolean(objRow["AllowBackOrderForAllItems"]);

                if (objColumns.Contains("EnableLocalize") && objRow["EnableLocalize"] != DBNull.Value) EnableLocalize = Convert.ToBoolean(objRow["EnableLocalize"]);

                if (objColumns.Contains("CorporateAddressBookID")) CorporateAddressBookID = Convert.ToString(objRow["CorporateAddressBookID"]);

                if (objColumns.Contains("DiscountPerItem") && objRow["DiscountPerItem"] != DBNull.Value) DiscountPerItem = Convert.ToDecimal(objRow["DiscountPerItem"]);

                if (objColumns.Contains("AllowPartialShipping") && objRow["AllowPartialShipping"] != DBNull.Value) AllowPartialShipping = Convert.ToBoolean(objRow["AllowPartialShipping"]);

                if (objColumns.Contains("EnableSMSOptIn")) EnableSMSOptIn = Convert.ToBoolean(objRow["EnableSMSOptIn"]);
                if (objColumns.Contains("EnableEmailOptIn")) EnableEmailOptIn = Convert.ToBoolean(objRow["EnableEmailOptIn"]);

                if (objColumns.Contains("ReloadAfterAddToCart") && objRow["ReloadAfterAddToCart"] != DBNull.Value) ReloadAfterAddToCart = Convert.ToBoolean(objRow["ReloadAfterAddToCart"]);

                if (objColumns.Contains("EnablePackagePayment")) EnablePackagePayment = Convert.ToBoolean(objRow["EnablePackagePayment"]);

                if (objColumns.Contains("EnableIPD") && objRow["EnableIPD"] != DBNull.Value) EnableIPD = Convert.ToBoolean(objRow["EnableIPD"]);
                if (objColumns.Contains("IPDShippingAdjustPercent") && objRow["IPDShippingAdjustPercent"] != DBNull.Value) IPDShippingAdjustPercent = Convert.ToDecimal(objRow["IPDShippingAdjustPercent"]);
                if (objColumns.Contains("IPDTaxAdjustPercent") && objRow["IPDTaxAdjustPercent"] != DBNull.Value) IPDTaxAdjustPercent = Convert.ToDecimal(objRow["IPDTaxAdjustPercent"]);

                if (objColumns.Contains("DisplayTariffCharge")) DisplayTariffCharge = Convert.ToBoolean(objRow["DisplayTariffCharge"]);

                if (objColumns.Contains("DisplayNewAccountSetupForm")) DisplayNewAccountSetupForm = Convert.ToBoolean(objRow["DisplayNewAccountSetupForm"]);

                if (objColumns.Contains("SuggestedSelling")) SuggestedSelling = Convert.ToBoolean(objRow["SuggestedSelling"]);

                if (objColumns.Contains("BannerHTML")) BannerHTML = Convert.ToString(objRow["BannerHTML"]);
                if (objColumns.Contains("FeaturedProductHTML")) FeaturedProductHTML = Convert.ToString(objRow["FeaturedProductHTML"]);

                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("CreatedBy")) CreatedBy = Convert.ToString(objRow["CreatedBy"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);

                if (string.IsNullOrEmpty(WebsiteID)) throw new Exception("Missing WebsiteID in the datarow");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objColumns = null;
            }
        }
        protected bool ExistsDomain()
        {
            bool _ret = false;

            try
            {
                Website Website = new Website();
                WebsiteFilter WebsiteFilter = new WebsiteFilter();
                WebsiteFilter.Domain = new Database.Filter.StringSearch.SearchFilter();
                WebsiteFilter.Domain.SearchString = Domain;
                Website = Website.GetWebsite(WebsiteFilter);
                if (Website != null && !string.IsNullOrEmpty(Website.WebsiteID) && Website.WebsiteID != WebsiteID)
                {
                    _ret = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return _ret;
        }
        public override bool Create()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Create(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Create();

            if (!IsActive) return true;

            Hashtable dicParam = new Hashtable();
            try
            {
                if (Name == null) throw new Exception("Name is required");
                if (UserRegistration && string.IsNullOrEmpty(DefaultWebsiteGroupID)) throw new Exception("Default Website Group is required when user registration is enabled");
                //if (ExistsDomain()) throw new Exception("Domain already exists");
                if (!IsNew) throw new Exception("Create cannot be performed, WebsiteID already exists");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");

                dicParam["Name"] = Name;
                dicParam["Abbreviation"] = Abbreviation;
                dicParam["Domain"] = Domain;
                dicParam["LogoPath"] = LogoPath;
                dicParam["BannerInternalID"] = BannerInternalID;
                dicParam["EmailLogoPath"] = EmailLogoPath;
                dicParam["BannerPath"] = BannerPath;
                dicParam["UserRegistration"] = UserRegistration;
                dicParam["UserRegistrationKeyRequired"] = UserRegistrationKeyRequired;
                dicParam["UserApprovalRequired"] = UserApprovalRequired;
                dicParam["AccountRegistration"] = AccountRegistration;
                dicParam["AccountRegistrationKeyRequired"] = AccountRegistrationKeyRequired;
                dicParam["AccountApprovalRequired"] = AccountApprovalRequired;
                dicParam["ShowAvailableInventory"] = ShowAvailableInventory;
                dicParam["ShowSalesDescription"] = ShowSalesDescription;
                dicParam["ShowDetailedDescription"] = ShowDetailedDescription;
                dicParam["DefaultWebsiteGroupID"] = DefaultWebsiteGroupID;
                dicParam["OrderApprovalRequired"] = OrderApprovalRequired;
                dicParam["EnableEmployeeCredit"] = EnableEmployeeCredit;
                dicParam["MustUseExistingEmployeeCredit"] = MustUseExistingEmployeeCredit;
                dicParam["EnableCreditCard"] = EnableCreditCard;
                dicParam["EnablePaymentTerm"] = EnablePaymentTerm;
                dicParam["EnablePromoCode"] = EnablePromoCode;
                dicParam["EnablePasswordReset"] = EnablePasswordReset;
                dicParam["PasswordHint"] = PasswordHint;
                dicParam["ItemDisplayType"] = ItemDisplayType;
                dicParam["ProductDetailDisplayType"] = ProductDetailDisplayType;
                dicParam["BillingAddressTransID"] = BillingAddressTransID;
                dicParam["DeliveryAddressTransID"] = DeliveryAddressTransID;
                dicParam["StartingPath"] = StartingPath;
                dicParam["NetSuiteDiscountItemInternalID"] = NetSuiteDiscountItemInternalID;
                dicParam["NetSuiteParentCustomerInternalID"] = NetSuiteParentCustomerInternalID;
                dicParam["DefaultPaymentTermID"] = DefaultPaymentTermID;
                dicParam["DisplaySupportFAQ"] = DisplaySupportFAQ;
                dicParam["DisplayContactUsAddress"] = DisplayContactUsAddress;
                dicParam["DisplayContactUsPhoneNumber"] = DisplayContactUsPhoneNumber;
                dicParam["DefaultCheckoutPaymentMethod"] = DefaultCheckoutPaymentMethod;
                dicParam["DisplayDefaultGroupPerAccount"] = DisplayDefaultGroupPerAccount;
                dicParam["DisplaySubCategory"] = DisplaySubCategory;
                dicParam["DisplayAttributeFilter"] = DisplayAttributeFilter;
                dicParam["DisplayLeftNavigation"] = DisplayLeftNavigation;
                dicParam["CombineWebsiteGroup"] = CombineWebsiteGroup;
                dicParam["DefaultSizeChartPath"] = DefaultSizeChartPath;
                dicParam["AllowNameChange"] = AllowNameChange;
                dicParam["RegistrationPath"] = RegistrationPath;
                dicParam["IsPunchout"] = IsPunchout;
                dicParam["DisplayUserPermission"] = DisplayUserPermission;
                dicParam["UseDomain"] = UseDomain;

                dicParam["BudgetAlias"] = BudgetAlias;
                dicParam["OverBudgetMessage"] = OverBudgetMessage;

                dicParam["RegistrationFormPath"] = RegistrationFormPath;
                dicParam["DisableAddressValidation"] = DisableAddressValidation;

                dicParam["EnableSSO"] = EnableSSO;
                dicParam["DisallowBackOrder"] = DisallowBackOrder;
                dicParam["HideEmail"] = HideEmail;
                dicParam["EnableZendesk"] = EnableZendesk;

                dicParam["HideMyAccountOrderReport"] = HideMyAccountOrderReport;
                dicParam["HideMyAccountOrderApproval"] = HideMyAccountOrderApproval;
                dicParam["HideAdminOrderApproval"] = HideAdminOrderApproval;

                dicParam["HideReturnPolicy"] = HideReturnPolicy;
                dicParam["DisplayNonInventoryMessage"] = DisplayNonInventoryMessage;

                dicParam["CreditCardLimitPerOrder"] = CreditCardLimitPerOrder;
                dicParam["IsOneBudgetPerUser"] = IsOneBudgetPerUser;
                dicParam["AllowGuestCheckout"] = AllowGuestCheckout;
                dicParam["DefaultGuestAccountID"] = DefaultGuestAccountID;

                dicParam["CurrencyConvert"] = CurrencyConvert;
                dicParam["CurrentyConvertPercentage"] = CurrentyConvertPercentage;

                dicParam["AllowBackOrderForAllItems"] = AllowBackOrderForAllItems;

                dicParam["EnableLocalize"] = EnableLocalize;

                dicParam["CorporateAddressBookID"] = CorporateAddressBookID;

                dicParam["DiscountPerItem"] = DiscountPerItem;

                dicParam["AllowPartialShipping"] = AllowPartialShipping;

                dicParam["EnableSMSOptIn"] = EnableSMSOptIn;
                dicParam["EnableEmailOptIn"] = EnableEmailOptIn;

                dicParam["ReloadAfterAddToCart"] = ReloadAfterAddToCart;

                dicParam["EnablePackagePayment"] = EnablePackagePayment;

                dicParam["EnableIPD"] = EnableIPD;
                dicParam["IPDShippingAdjustPercent"] = IPDShippingAdjustPercent;
                dicParam["IPDTaxAdjustPercent"] = IPDTaxAdjustPercent;

                dicParam["DisplayTariffCharge"] = DisplayTariffCharge;

                dicParam["DisplayNewAccountSetupForm"] = DisplayNewAccountSetupForm;

                dicParam["BannerHTML"] = BannerHTML;
                dicParam["FeaturedProductHTML"] = FeaturedProductHTML;

                dicParam["SuggestedSelling"] = SuggestedSelling;
                dicParam["InActive"] = InActive;
                dicParam["CreatedBy"] = CreatedBy;

                WebsiteID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "Website"), objConn, objTran).ToString();

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
            }
            return true;
        }

        public override bool Update()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Update(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Update();

            if (!IsActive) return Delete(objConn, objTran);

            Hashtable dicParam = new Hashtable();
            Hashtable dicWParam = new Hashtable();

            try
            {
                if (Name == null) throw new Exception("Name is required");
                if (UserRegistration && string.IsNullOrEmpty(DefaultWebsiteGroupID)) throw new Exception("Default Website Group is required when user registration is enabled");
                //if (ExistsDomain()) throw new Exception("Domain already exists");
                if (IsNew) throw new Exception("Update cannot be performed, WebsiteID is missing");
                if (ObjectAlreadyExists()) throw new Exception("This record already exists");


                dicParam["Name"] = Name;
                dicParam["Abbreviation"] = Abbreviation;
                dicParam["Domain"] = Domain;
                dicParam["LogoPath"] = LogoPath;
                dicParam["BannerInternalID"] = BannerInternalID;
                dicParam["EmailLogoPath"] = EmailLogoPath;
                dicParam["BannerPath"] = BannerPath;
                dicParam["UserRegistration"] = UserRegistration;
                dicParam["UserRegistrationKeyRequired"] = UserRegistrationKeyRequired;
                dicParam["UserApprovalRequired"] = UserApprovalRequired;
                dicParam["AccountRegistration"] = AccountRegistration;
                dicParam["AccountRegistrationKeyRequired"] = AccountRegistrationKeyRequired;
                dicParam["AccountApprovalRequired"] = AccountApprovalRequired;
                dicParam["ShowAvailableInventory"] = ShowAvailableInventory;
                dicParam["ShowSalesDescription"] = ShowSalesDescription;
                dicParam["ShowDetailedDescription"] = ShowDetailedDescription;
                dicParam["DefaultWebsiteGroupID"] = DefaultWebsiteGroupID;
                dicParam["OrderApprovalRequired"] = OrderApprovalRequired;
                dicParam["EnableEmployeeCredit"] = EnableEmployeeCredit;
                dicParam["MustUseExistingEmployeeCredit"] = MustUseExistingEmployeeCredit;
                dicParam["EnableCreditCard"] = EnableCreditCard;
                dicParam["EnablePaymentTerm"] = EnablePaymentTerm;
                dicParam["EnablePromoCode"] = EnablePromoCode;
                dicParam["EnablePasswordReset"] = EnablePasswordReset;
                dicParam["PasswordHint"] = PasswordHint;
                dicParam["ItemDisplayType"] = ItemDisplayType;
                dicParam["ProductDetailDisplayType"] = ProductDetailDisplayType;
                dicParam["BillingAddressTransID"] = BillingAddressTransID;
                dicParam["DeliveryAddressTransID"] = DeliveryAddressTransID;
                dicParam["StartingPath"] = StartingPath;
                dicParam["NetSuiteDiscountItemInternalID"] = NetSuiteDiscountItemInternalID;
                dicParam["NetSuiteParentCustomerInternalID"] = NetSuiteParentCustomerInternalID;
                dicParam["DefaultPaymentTermID"] = DefaultPaymentTermID;
                dicParam["DisplaySupportFAQ"] = DisplaySupportFAQ;
                dicParam["DisplayContactUsAddress"] = DisplayContactUsAddress;
                dicParam["DisplayContactUsPhoneNumber"] = DisplayContactUsPhoneNumber;
                dicParam["DefaultCheckoutPaymentMethod"] = DefaultCheckoutPaymentMethod;
                dicParam["DisplayDefaultGroupPerAccount"] = DisplayDefaultGroupPerAccount;
                dicParam["DisplaySubCategory"] = DisplaySubCategory;
                dicParam["DisplayAttributeFilter"] = DisplayAttributeFilter;
                dicParam["DisplayLeftNavigation"] = DisplayLeftNavigation;
                dicParam["CombineWebsiteGroup"] = CombineWebsiteGroup;
                dicParam["DefaultSizeChartPath"] = DefaultSizeChartPath;
                dicParam["AllowNameChange"] = AllowNameChange;
                dicParam["RegistrationPath"] = RegistrationPath;
                dicParam["IsPunchout"] = IsPunchout;
                dicParam["DisplayUserPermission"] = DisplayUserPermission;
                dicParam["UseDomain"] = UseDomain;

                dicParam["BudgetAlias"] = BudgetAlias;
                dicParam["OverBudgetMessage"] = OverBudgetMessage;

                dicParam["RegistrationFormPath"] = RegistrationFormPath;
                dicParam["DisableAddressValidation"] = DisableAddressValidation;

                dicParam["EnableSSO"] = EnableSSO;
                dicParam["DisallowBackOrder"] = DisallowBackOrder;
                dicParam["HideEmail"] = HideEmail;
                dicParam["EnableZendesk"] = EnableZendesk;

                dicParam["HideMyAccountOrderReport"] = HideMyAccountOrderReport;
                dicParam["HideMyAccountOrderApproval"] = HideMyAccountOrderApproval; 
                dicParam["HideAdminOrderApproval"] = HideAdminOrderApproval;

                dicParam["HideReturnPolicy"] = HideReturnPolicy;
                dicParam["DisplayNonInventoryMessage"] = DisplayNonInventoryMessage;

                dicParam["CreditCardLimitPerOrder"] = CreditCardLimitPerOrder;
                dicParam["IsOneBudgetPerUser"] = IsOneBudgetPerUser;
                dicParam["AllowGuestCheckout"] = AllowGuestCheckout;
                dicParam["DefaultGuestAccountID"] = DefaultGuestAccountID;

                dicParam["CurrencyConvert"] = CurrencyConvert;
                dicParam["CurrentyConvertPercentage"] = CurrentyConvertPercentage;

                dicParam["AllowBackOrderForAllItems"] = AllowBackOrderForAllItems;

                dicParam["EnableLocalize"] = EnableLocalize;

                dicParam["CorporateAddressBookID"] = CorporateAddressBookID;

                dicParam["DiscountPerItem"] = DiscountPerItem;

                dicParam["AllowPartialShipping"] = AllowPartialShipping;

                dicParam["EnableSMSOptIn"] = EnableSMSOptIn;
                dicParam["EnableEmailOptIn"] = EnableEmailOptIn;

                dicParam["ReloadAfterAddToCart"] = ReloadAfterAddToCart;

                dicParam["EnablePackagePayment"] = EnablePackagePayment;

                dicParam["EnableIPD"] = EnableIPD;
                dicParam["IPDShippingAdjustPercent"] = IPDShippingAdjustPercent;
                dicParam["IPDTaxAdjustPercent"] = IPDTaxAdjustPercent;
                
                dicParam["DisplayTariffCharge"] = DisplayTariffCharge;

                dicParam["DisplayNewAccountSetupForm"] = DisplayNewAccountSetupForm;
                dicParam["SuggestedSelling"] = SuggestedSelling;

                dicParam["BannerHTML"] = BannerHTML;
                dicParam["FeaturedProductHTML"] = FeaturedProductHTML;

                dicParam["InActive"] = InActive;
                dicParam["UpdatedOn"] = DateTime.UtcNow;
                dicWParam["WebsiteID"] = WebsiteID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "Website"), objConn, objTran);

                Load(objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicParam = null;
                dicWParam = null;
            }
            return true;
        }

        public override bool Delete()
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();
                Delete(objConn, objTran);
                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public override bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            base.Delete();

            Hashtable dicDParam = new Hashtable();

            try
            {
                if (IsNew) throw new Exception("Delete cannot be performed, WebsiteID is missing");

                dicDParam["WebsiteID"] = WebsiteID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "Website"), objConn, objTran);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dicDParam = null;
            }
            return true;
        }

        public bool ObjectAlreadyExists()
        {
            string strSQL = string.Empty;

            strSQL = "SELECT TOP 1 p.* " +
                     "FROM Website (NOLOCK) p " +
                     "WHERE " +
                    "(p.Name=" + Database.HandleQuote(Name) + ") ";

            if (!string.IsNullOrEmpty(WebsiteID)) strSQL += "AND p.WebsiteID<>" + Database.HandleQuote(WebsiteID);
            return Database.HasRows(strSQL);
        }

        public static Website GetWebsite(WebsiteFilter Filter)
        {
            List<Website> objWebsites = null;
            Website objReturn = null;

            try
            {
                objWebsites = GetWebsites(Filter);
                if (objWebsites != null && objWebsites.Count >= 1) objReturn = objWebsites[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsites = null;
            }
            return objReturn;
        }

        public static List<Website> GetWebsites()
        {
            int intTotalCount = 0;
            return GetWebsites(null, null, null, out intTotalCount);
        }

        public static List<Website> GetWebsites(WebsiteFilter Filter)
        {
            int intTotalCount = 0;
            return GetWebsites(Filter, null, null, out intTotalCount);
        }

        public static List<Website> GetWebsites(WebsiteFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetWebsites(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<Website> GetWebsites(WebsiteFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<Website> objReturn = null;
            Website objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<Website>();

                strSQL = "SELECT * " +
                         "FROM Website (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.InternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InternalID, "InternalID");
                    if (Filter.GUID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.GUID, "GUID");
                    if (Filter.Name != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Name, "Name");
                    if (Filter.Domain != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.Domain, "Domain");
                    if (Filter.InActive != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.InActive, "InActive");
                    if (Filter.BannerInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.BannerInternalID, "BannerInternalID");
                    if (Filter.EnableSMSOptIn != null) strSQL += "AND EnableSMSOptIn=" + Database.HandleQuote(Convert.ToInt32(Filter.EnableSMSOptIn.Value).ToString());
                    if (Filter.EnableEmailOptIn != null) strSQL += "AND EnableEmailOptIn=" + Database.HandleQuote(Convert.ToInt32(Filter.EnableEmailOptIn.Value).ToString());
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "WebsiteID" : Utility.CustomSorting.GetSortExpression(typeof(Website), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new Website(objData.Tables[0].Rows[i]);
                        objNew.IsLoaded = true;
                        objReturn.Add(objNew);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objData = null;
            }
            return objReturn;
        }
    }
}
