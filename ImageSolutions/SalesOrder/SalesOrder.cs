using ImageSolutions.Website;
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


namespace ImageSolutions.SalesOrder
{
    public class SalesOrder : ISBase.BaseClass
    {
        public string SalesOrderID { get; private set; }
        public bool IsNew { get { return string.IsNullOrEmpty(SalesOrderID); } }
        public string WebsiteID { get; set; }
        public string AccountID { get; set; }
        public string UserInfoID { get; set; }
        public string UserWebsiteID { get; set; }
        public string NetSuiteInternalID { get; set; }
        public string SalesOrderNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string BillingAddressTransID { get; set; }
        public string DeliveryAddressTransID { get; set; }
        public double ShippingAmount { get; set; }
        public double TaxAmount { get; set; }
        public double DiscountAmount { get; set; }
        public double BudgetShippingAmount { get; set; }
        public double BudgetTaxAmount { get; set; }
        public double IPDDutiesAndTaxesAmount { get; set; }
        public string WebsiteShippingServiceID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CustomerID { get; set; }
        public string CustomerGroup { get; set; }
        public string ShippingMethod { get; set; }
        public enumPaymentMethod PaymentMethod { get; set; }
        public string ShippingMemo { get; set; }
        public decimal? VendorTotal { get; set; }
        public bool IsBoltonDropShip { get; set; }
        public bool IsCancellationRequested { get; set; }
        public DateTime? CancellationRequestedOn { get; set; }
        public string CancellationReason { get; set; }
        public Decimal CancellationRequestedRefundAmount { get; set; }
        public string CancellationRequestedBy { get; set; }
        public bool IsPendingApproval { get; set; }
        public bool IsPendingItemPersonalizationApproval { get; set; }
        public bool IsClosed { get; set; }
        public string OrderFilePath { get; set; }
        public string InvoiceFilePath { get; set; }
        public string Status { get; set; }
        public bool ApprovalNotificationSent { get; set; }
        public bool OrderConfirmationSent { get; set; }
        public bool IsTaxExempt { get; set; }
        public string RejectionReason { get; set; }
        public string TermPaymentPONumber { get; set; }
        public string TermPaymentStoreNumber { get; set; }

        public bool IsBudgetPayEntrySubmitted { get; set; }
        public string BudgetPayEntryReference { get; set; }
        public DateTime? BudgetPayEntrySubmittedOn { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }
        public bool IsPartialShipping { get; set; }
        public bool DisplayTariffCharge
        {
            get
            {
                return SalesOrderLines.Exists(m => m.TariffCharge != null);
            }
        }
        public string PackageID { get; set; }
        public bool ExcludeOptional { get; set; }
        public bool InActive { get; set; }


        public bool IsRejected
        {
            get
            {
                return Convert.ToString(Status) == "Rejected";
            }
        }


        private List<Payment.Payment> mPayments = null;
        public List<Payment.Payment> Payments
        {
            get
            {
                if (mPayments == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    Payment.PaymentFilter objFilter = null;

                    try
                    {
                        objFilter = new Payment.PaymentFilter();
                        objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderID.SearchString = SalesOrderID;

                        mPayments = Payment.Payment.GetPayments(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return mPayments;
            }
        }


        private WebsiteShippingService mWebsiteShippingService = null;

        public WebsiteShippingService WebsiteShippingService
        {
            get
            {
                if (mWebsiteShippingService == null && !string.IsNullOrEmpty(WebsiteShippingServiceID))
                {
                    mWebsiteShippingService = new WebsiteShippingService(WebsiteShippingServiceID);
                }
                return mWebsiteShippingService;
            }
        }

        public double PaymentTotal
        {
            get
            {
                double dcmReturn = 0;
                if (Payments != null)
                {
                    dcmReturn = Payments.Sum(m => m.AmountPaid);
                }
                return dcmReturn;
            }
        }

        public double LineTotal
        {
            get
            {
                double dcmReturn = 0;
                if (SalesOrderLines != null)
                {
                    dcmReturn = SalesOrderLines.Sum(m => m.LineTotal);
                }
                return Math.Round(dcmReturn, 2);
            }
        }

        public double TariffChargeTotal
        {
            get
            {
                double dcmReturn = 0;
                if (SalesOrderLines != null)
                {
                    dcmReturn = SalesOrderLines.Sum(m => m.TariffCharge == null ? 0 : Convert.ToDouble(m.TariffCharge * m.Quantity));
                }
                return Math.Round(dcmReturn, 2);
            }
        }

        public double Total
        {
            get
            {
                return Math.Round(LineTotal + ShippingAmount + TaxAmount + IPDDutiesAndTaxesAmount + TariffChargeTotal - DiscountAmount, 2);
            }
        }

        public int TotalQuantity
        {
            get
            {
                int intReturn = 0;
                if (SalesOrderLines != null)
                {
                    intReturn = SalesOrderLines.Sum(m => m.Quantity);
                }
                return intReturn;
            }
        }

        public double BudgetApplied
        {
            get
            {
                if (Payments.Find(x => !string.IsNullOrEmpty(x.BudgetAssignmentID)) != null && Payments.Find(x => !string.IsNullOrEmpty(x.BudgetAssignmentID)).AmountPaid > 0)
                {
                    return Payments.Find(x => !string.IsNullOrEmpty(x.BudgetAssignmentID)).AmountPaid;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Balance
        {
            get
            {
                return Total - BudgetApplied;
            }
        }

        private User.UserInfo mUserInfo = null;
        public User.UserInfo UserInfo
        {
            get
            {
                if (mUserInfo == null && !string.IsNullOrEmpty(UserInfoID))
                {
                    mUserInfo = new ImageSolutions.User.UserInfo(UserInfoID);
                }
                return mUserInfo;
            }
        }

        private Customer.Customer mCustomer = null;
        public Customer.Customer Customer
        {
            get
            {
                if (mCustomer == null && !string.IsNullOrEmpty(CustomerID))
                {
                    mCustomer = new ImageSolutions.Customer.Customer(CustomerID);
                }
                return mCustomer;
            }
        }
        private User.UserWebsite mUserWebsite = null;
        public User.UserWebsite UserWebsite
        {
            get
            {
                if (mUserWebsite == null && !string.IsNullOrEmpty(UserWebsiteID))
                {
                    mUserWebsite = new User.UserWebsite(UserWebsiteID);
                }
                return mUserWebsite;
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

        public string DisplayDeliveryAddress()
        {
            string _ret = string.Empty;

            if (this.DeliveryAddress != null)
                _ret = DeliveryAddress.GetDisplayFormat(true);

            return _ret;
        }

        private List<SalesOrderLine> mSalesOrderLines = null;
        public List<SalesOrderLine> SalesOrderLines
        {
            get
            {
                if (mSalesOrderLines == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    SalesOrderLineFilter objFilter = null;

                    try
                    {
                        //objFilter = new SalesOrderLineFilter();
                        //objFilter.SalesOrderID = SalesOrderID;

                        objFilter = new SalesOrderLineFilter();
                        objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderID.SearchString = SalesOrderID;

                        mSalesOrderLines = SalesOrderLine.GetSalesOrderLines(objFilter);

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
                return mSalesOrderLines;
            }
            set
            {
                mSalesOrderLines = value;
            }
        }

        private List<Fulfillment.Fulfillment> mFulfillments = null;
        public List<Fulfillment.Fulfillment> Fulfillments
        {
            get
            {
                if (mFulfillments == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    Fulfillment.FulfillmentFilter objFilter = null;
                    try
                    {
                        objFilter = new Fulfillment.FulfillmentFilter();
                        objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderID.SearchString = SalesOrderID;
                        mFulfillments = Fulfillment.Fulfillment.GetFulfillments(objFilter);
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
                return mFulfillments;
            }
            set
            {
                mFulfillments = value;
            }
        }

        private List<RMA.RMA> mRMAs = null;
        public List<RMA.RMA> RMAs
        {
            get
            {
                if (mRMAs == null && !string.IsNullOrEmpty(SalesOrderID))
                {
                    RMA.RMAFilter objFilter = null;
                    try
                    {
                        objFilter = new RMA.RMAFilter();
                        objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.SalesOrderID.SearchString = SalesOrderID;
                        mRMAs = RMA.RMA.GetRMAs(objFilter);
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
                return mRMAs;
            }
            set
            {
                mRMAs = value;
            }
        }

        //private Invoice.Invoice mInvoice = null;
        //public Invoice.Invoice Invoice
        //{
        //    get
        //    {
        //        if (mInvoice == null && !string.IsNullOrEmpty(SalesOrderID))
        //        {
        //            Invoice.InvoiceFilter objFilter = null;

        //            try
        //            {
        //                objFilter = new Invoice.InvoiceFilter();
        //                objFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
        //                objFilter.SalesOrderID.SearchString = SalesOrderID;
        //                mInvoice = ImageSolutions.Invoice.Invoice.GetInvoice(objFilter);
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //            finally
        //            {
        //                objFilter = null;
        //            }
        //        }
        //        return mInvoice;
        //    }
        //    set
        //    {
        //        mInvoice = value;
        //    }
        //}

        private Website.Website mWebsite = null;
        public Website.Website Website
        {
            get
            {
                if (mWebsite == null && !string.IsNullOrEmpty(WebsiteID))
                {
                    mWebsite = new Website.Website(WebsiteID);
                }
                return mWebsite;
            }
            set
            {
                mWebsite = value;
            }
        }


        private Account.Account mAccount = null;
        public Account.Account Account
        {
            get
            {
                if (mAccount == null && !string.IsNullOrEmpty(AccountID))
                {
                    mAccount = new Account.Account(AccountID);
                }
                return mAccount;
            }
            set
            {
                mAccount = value;
            }
        }

        private ImageSolutions.Package.Package mPackage = null;
        public ImageSolutions.Package.Package Package
        {
            get
            {
                if (mPackage == null && !string.IsNullOrEmpty(PackageID))
                {
                    Package.PackageFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Package.PackageFilter();
                        objFilter.PackageID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PackageID.SearchString = PackageID;

                        mPackage = ImageSolutions.Package.Package.GetPackage(objFilter);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return mPackage;
            }

        }
        public SalesOrder()
        {
        }
        public SalesOrder(string SalesOrderID)
        {
            this.SalesOrderID = SalesOrderID;
            Load();
        }
        public SalesOrder(DataRow objRow)
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
                         "FROM SalesOrder (NOLOCK) " +
                         "WHERE SalesOrderID=" + Database.HandleQuote(SalesOrderID);
                objData = Database.GetDataSet(strSQL);
                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    Load(objData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("SalesOrderID=" + SalesOrderID + " is not found");
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
        private void Load(DataRow objRow)
        {
            DataColumnCollection objColumns = null;

            try
            {
                objColumns = objRow.Table.Columns;

                if (objColumns.Contains("SalesOrderID")) SalesOrderID = Convert.ToString(objRow["SalesOrderID"]);
                if (objColumns.Contains("WebsiteID")) WebsiteID = Convert.ToString(objRow["WebsiteID"]);
                if (objColumns.Contains("AccountID")) AccountID = Convert.ToString(objRow["AccountID"]);
                if (objColumns.Contains("UserWebsiteID")) UserWebsiteID = Convert.ToString(objRow["UserWebsiteID"]);
                //if (objColumns.Contains("ExternalID")) ExternalID = Convert.ToString(objRow["ExternalID"]);
                //if (objColumns.Contains("IncrementID")) IncrementID = Convert.ToString(objRow["IncrementID"]);
                if (objColumns.Contains("TransactionDate") && objRow["TransactionDate"] != DBNull.Value) TransactionDate = Convert.ToDateTime(objRow["TransactionDate"]);

                if (objColumns.Contains("NetSuiteInternalID")) NetSuiteInternalID = Convert.ToString(objRow["NetSuiteInternalID"]);
                if (objColumns.Contains("SalesOrderNumber")) SalesOrderNumber = Convert.ToString(objRow["SalesOrderNumber"]);
         
                if (objColumns.Contains("BillingAddressTransID")) BillingAddressTransID = Convert.ToString(objRow["BillingAddressTransID"]);
                if (objColumns.Contains("DeliveryAddressTransID")) DeliveryAddressTransID = Convert.ToString(objRow["DeliveryAddressTransID"]);
                if (objColumns.Contains("WebsiteShippingServiceID")) WebsiteShippingServiceID = Convert.ToString(objRow["WebsiteShippingServiceID"]);

                if (objColumns.Contains("ShippingAmount")) ShippingAmount = Convert.ToDouble(objRow["ShippingAmount"]);
                if (objColumns.Contains("TaxAmount") && objRow["TaxAmount"] != DBNull.Value) TaxAmount = Convert.ToDouble(objRow["TaxAmount"]);
                if (objColumns.Contains("DiscountAmount") && objRow["DiscountAmount"] != DBNull.Value) DiscountAmount = Convert.ToDouble(objRow["DiscountAmount"]);
                if (objColumns.Contains("BudgetShippingAmount") && objRow["BudgetShippingAmount"] != DBNull.Value) BudgetShippingAmount = Convert.ToDouble(objRow["BudgetShippingAmount"]);
                if (objColumns.Contains("BudgetTaxAmount") && objRow["BudgetTaxAmount"] != DBNull.Value) BudgetTaxAmount = Convert.ToDouble(objRow["BudgetTaxAmount"]);
                if (objColumns.Contains("IPDDutiesAndTaxesAmount") && objRow["IPDDutiesAndTaxesAmount"] != DBNull.Value) IPDDutiesAndTaxesAmount = Convert.ToDouble(objRow["IPDDutiesAndTaxesAmount"]);

                if (objColumns.Contains("IsPendingApproval")) IsPendingApproval = Convert.ToBoolean(objRow["IsPendingApproval"]);
                if (objColumns.Contains("IsPendingItemPersonalizationApproval")) IsPendingItemPersonalizationApproval = Convert.ToBoolean(objRow["IsPendingItemPersonalizationApproval"]);
                if (objColumns.Contains("IsClosed")) IsClosed = Convert.ToBoolean(objRow["IsClosed"]);
                if (objColumns.Contains("OrderFilePath")) OrderFilePath = Convert.ToString(objRow["OrderFilePath"]);
                if (objColumns.Contains("InvoiceFilePath")) InvoiceFilePath = Convert.ToString(objRow["InvoiceFilePath"]);
                if (objColumns.Contains("ErrorMessage")) ErrorMessage = Convert.ToString(objRow["ErrorMessage"]);
                if (objColumns.Contains("CreatedOn")) CreatedOn = Convert.ToDateTime(objRow["CreatedOn"]);
                if (objColumns.Contains("UpdatedOn") && objRow["UpdatedOn"] != DBNull.Value) UpdatedOn = Convert.ToDateTime(objRow["UpdatedOn"]);
                if (objColumns.Contains("UserInfoID")) UserInfoID = Convert.ToString(objRow["UserInfoID"]);
                if (objColumns.Contains("Status")) Status = Convert.ToString(objRow["Status"]);
                if (objColumns.Contains("ApprovalNotificationSent")) ApprovalNotificationSent = Convert.ToBoolean(objRow["ApprovalNotificationSent"]);
                if (objColumns.Contains("OrderConfirmationSent")) OrderConfirmationSent = Convert.ToBoolean(objRow["OrderConfirmationSent"]);

                if (objColumns.Contains("RejectionReason")) RejectionReason = Convert.ToString(objRow["RejectionReason"]);

                if (objColumns.Contains("IsTaxExempt") && objRow["IsTaxExempt"] != DBNull.Value) IsTaxExempt = Convert.ToBoolean(objRow["IsTaxExempt"]);

                if (objColumns.Contains("TermPaymentPONumber")) TermPaymentPONumber = Convert.ToString(objRow["TermPaymentPONumber"]);
                if (objColumns.Contains("TermPaymentStoreNumber")) TermPaymentStoreNumber = Convert.ToString(objRow["TermPaymentStoreNumber"]);

                if (objColumns.Contains("IsBudgetPayEntrySubmitted") && objRow["IsBudgetPayEntrySubmitted"] != DBNull.Value) IsBudgetPayEntrySubmitted = Convert.ToBoolean(objRow["IsBudgetPayEntrySubmitted"]);
                if (objColumns.Contains("BudgetPayEntryReference")) BudgetPayEntryReference = Convert.ToString(objRow["BudgetPayEntryReference"]);
                if (objColumns.Contains("BudgetPayEntrySubmittedOn") && objRow["BudgetPayEntrySubmittedOn"] != DBNull.Value) BudgetPayEntrySubmittedOn = Convert.ToDateTime(objRow["BudgetPayEntrySubmittedOn"]);

                if (objColumns.Contains("ApprovedBy")) ApprovedBy = Convert.ToString(objRow["ApprovedBy"]);
                if (objColumns.Contains("ApprovedOn") && objRow["ApprovedOn"] != DBNull.Value) ApprovedOn = Convert.ToDateTime(objRow["ApprovedOn"]);

                if (objColumns.Contains("IsPartialShipping") && objRow["IsPartialShipping"] != DBNull.Value) IsPartialShipping = Convert.ToBoolean(objRow["IsPartialShipping"]);

                if (objColumns.Contains("PackageID")) PackageID = Convert.ToString(objRow["PackageID"]);
                if (objColumns.Contains("ExcludeOptional") && objRow["ExcludeOptional"] != DBNull.Value) ExcludeOptional = Convert.ToBoolean(objRow["ExcludeOptional"]);

                if (objColumns.Contains("InActive") && objRow["InActive"] != DBNull.Value) InActive = Convert.ToBoolean(objRow["InActive"]);

                if (string.IsNullOrEmpty(SalesOrderID)) throw new Exception("Missing SalesOrderID in the datarow");
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
                if (TransactionDate == null) throw new Exception("TransactionDate is required");

                //if (string.IsNullOrEmpty(ExternalID)) throw new Exception("ExternalID is required");
                //if (string.IsNullOrEmpty(IncrementID)) throw new Exception("IncrementID is required");
                //if (string.IsNullOrEmpty(CustomerID)) throw new Exception("CustomerID is required");
                //if (string.IsNullOrEmpty(BillingAddressTransID)) throw new Exception("BillingAddressTransID is required");
                //if (string.IsNullOrEmpty(DeliveryAddressTransID)) throw new Exception("DeliveryAddressTransID is required");
                if (!IsNew) throw new Exception("Create cannot be performed, SalesOrderID already exists");

                dicParam["WebsiteID"] = WebsiteID;
                dicParam["AccountID"] = AccountID;
                dicParam["UserWebsiteID"] = UserWebsiteID;
                dicParam["SalesOrderNumber"] = SalesOrderNumber;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["TaxAmount"] = TaxAmount;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["BudgetShippingAmount"] = BudgetShippingAmount;
                dicParam["BudgetTaxAmount"] = BudgetTaxAmount;
                dicParam["IPDDutiesAndTaxesAmount"] = IPDDutiesAndTaxesAmount;
                dicParam["BillingAddressTransID"] = BillingAddressTransID;
                dicParam["DeliveryAddressTransID"] = DeliveryAddressTransID;
                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["IsPendingApproval"] = IsPendingApproval;
                dicParam["IsPendingItemPersonalizationApproval"] = IsPendingItemPersonalizationApproval;
                dicParam["IsClosed"] = IsClosed;
                dicParam["OrderFilePath"] = OrderFilePath;
                dicParam["InvoiceFilePath"] = InvoiceFilePath;
                dicParam["Status"] = string.IsNullOrEmpty(Status) ? "Pending Fulfillment" : Status;
                dicParam["ApprovalNotificationSent"] = ApprovalNotificationSent;
                dicParam["OrderConfirmationSent"] = OrderConfirmationSent;
                dicParam["IsTaxExempt"] = IsTaxExempt;
                dicParam["RejectionReason"] = RejectionReason;
                dicParam["TermPaymentPONumber"] = TermPaymentPONumber;
                dicParam["TermPaymentStoreNumber"] = TermPaymentStoreNumber;
                dicParam["IsBudgetPayEntrySubmitted"] = IsBudgetPayEntrySubmitted;
                dicParam["BudgetPayEntryReference"] = BudgetPayEntryReference;
                dicParam["BudgetPayEntrySubmittedOn"] = BudgetPayEntrySubmittedOn;

                dicParam["ApprovedBy"] = ApprovedBy;
                dicParam["ApprovedOn"] = ApprovedOn;

                dicParam["IsPartialShipping"] = IsPartialShipping;

                dicParam["PackageID"] = PackageID;
                dicParam["ExcludeOptional"] = ExcludeOptional;

                dicParam["InActive"] = InActive;
                dicParam["ErrorMessage"] = ErrorMessage;
                dicParam["UpdatedOn"] = DateTime.UtcNow;

                SalesOrderID = Database.ExecuteSQLWithIdentity(Database.GetInsertSQL(dicParam, "SalesOrder"), objConn, objTran).ToString();

                foreach (SalesOrderLine objSalesOrderLine in SalesOrderLines)
                {
                    objSalesOrderLine.SalesOrderID = SalesOrderID;
                    objSalesOrderLine.Create(objConn, objTran);
                }

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
                if (TransactionDate == null) throw new Exception("TransactionDate is required");
                //if (string.IsNullOrEmpty(ExternalID)) throw new Exception("ExternalID is required");
                //if (string.IsNullOrEmpty(IncrementID)) throw new Exception("IncrementID is required");
                //if (string.IsNullOrEmpty(CustomerID)) throw new Exception("CustomerID is required");
                //if (string.IsNullOrEmpty(BillingAddressTransID)) throw new Exception("BillingAddressTransID is required");
                //if (string.IsNullOrEmpty(DeliveryAddressTransID)) throw new Exception("DeliveryAddressTransID is required");
                if (IsNew) throw new Exception("Update cannot be performed, SalesOrderID is missing");

                dicParam["AccountID"] = AccountID;
                dicParam["SalesOrderNumber"] = SalesOrderNumber;
                dicParam["TransactionDate"] = TransactionDate;
                dicParam["UserInfoID"] = UserInfoID;
                dicParam["ShippingAmount"] = ShippingAmount;
                dicParam["TaxAmount"] = TaxAmount;
                dicParam["DiscountAmount"] = DiscountAmount;
                dicParam["BudgetShippingAmount"] = BudgetShippingAmount;
                dicParam["BudgetTaxAmount"] = BudgetTaxAmount;
                dicParam["IPDDutiesAndTaxesAmount"] = IPDDutiesAndTaxesAmount;
                dicParam["BillingAddressTransID"] = BillingAddressTransID;
                dicParam["DeliveryAddressTransID"] = DeliveryAddressTransID;
                dicParam["WebsiteShippingServiceID"] = WebsiteShippingServiceID;
                dicParam["IsPendingApproval"] = IsPendingApproval;
                dicParam["IsPendingItemPersonalizationApproval"] = IsPendingItemPersonalizationApproval;
                dicParam["IsClosed"] = IsClosed;
                dicParam["OrderFilePath"] = OrderFilePath;
                dicParam["InvoiceFilePath"] = InvoiceFilePath;
                dicParam["NetSuiteInternalID"] = NetSuiteInternalID;
                dicParam["ApprovalNotificationSent"] = ApprovalNotificationSent;
                dicParam["OrderConfirmationSent"] = OrderConfirmationSent;
                dicParam["IsTaxExempt"] = IsTaxExempt;
                dicParam["RejectionReason"] = RejectionReason;
                dicParam["TermPaymentPONumber"] = TermPaymentPONumber;
                dicParam["TermPaymentStoreNumber"] = TermPaymentStoreNumber;
                dicParam["IsBudgetPayEntrySubmitted"] = IsBudgetPayEntrySubmitted;
                dicParam["BudgetPayEntryReference"] = BudgetPayEntryReference;
                dicParam["BudgetPayEntrySubmittedOn"] = BudgetPayEntrySubmittedOn;

                dicParam["ApprovedBy"] = ApprovedBy;
                dicParam["ApprovedOn"] = ApprovedOn;

                dicParam["IsPartialShipping"] = IsPartialShipping;

                dicParam["PackageID"] = PackageID;
                dicParam["ExcludeOptional"] = ExcludeOptional;

                dicParam["InActive"] = InActive;
                dicParam["Status"] = string.IsNullOrEmpty(Status) ? "Pending Fulfillment" : Status;
                dicParam["ErrorMessage"] = ErrorMessage;

                dicWParam["SalesOrderID"] = SalesOrderID;

                Database.ExecuteSQL(Database.GetUpdateSQL(dicParam, dicWParam, "SalesOrder"), objConn, objTran);

                foreach (SalesOrderLine objSalesOrderLine in SalesOrderLines)
                {
                    objSalesOrderLine.Update(objConn, objTran);
                }

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
                if (IsNew) throw new Exception("Delete cannot be performed, SalesOrderID is missing");

                dicDParam["SalesOrderID"] = SalesOrderID;
                Database.ExecuteSQL(Database.GetDeleteSQL(dicDParam, "SalesOrder"), objConn, objTran);
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

        public static SalesOrder GetSalesOrder(SalesOrderFilter Filter)
        {
            List<SalesOrder> objSalesOrders = null;
            SalesOrder objReturn = null;

            try
            {
                objSalesOrders = GetSalesOrders(Filter);
                if (objSalesOrders != null && objSalesOrders.Count >= 1) objReturn = objSalesOrders[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrders = null;
            }
            return objReturn;
        }

        public static List<SalesOrder> GetSalesOrders()
        {
            int intTotalCount = 0;
            return GetSalesOrders(null, null, null, out intTotalCount);
        }

        public static List<SalesOrder> GetSalesOrders(SalesOrderFilter Filter)
        {
            int intTotalCount = 0;
            return GetSalesOrders(Filter, null, null, out intTotalCount);
        }

        public static List<SalesOrder> GetSalesOrders(SalesOrderFilter Filter, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrders(Filter, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }

        public static List<SalesOrder> GetSalesOrders(SalesOrderFilter Filter, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrder> objReturn = null;
            SalesOrder objNew = null;
            DataSet objData = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrder>();

                strSQL = "SELECT * " +
                         "FROM SalesOrder (NOLOCK) " +
                         "WHERE 1=1 ";

                if (Filter != null)
                {
                    if (Filter.WebsiteID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.WebsiteID, "WebsiteID");
                    if (Filter.AccountID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.AccountID, "AccountID");
                    if (Filter.SalesOrderID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderID, "SalesOrderID");
                    if (Filter.NetSuiteInternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.NetSuiteInternalID, "NetSuiteInternalID");
                    if (Filter.SalesOrderNumber != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.SalesOrderNumber, "SalesOrderNumber");
                    if (Filter.ExternalID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ExternalID, "ExternalID");
                    if (Filter.UpdatedOn != null) strSQL += Database.Filter.DateTimeSearch.GetSQLQuery(Filter.UpdatedOn, "UpdatedOn");
                    if (Filter.IncrementID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.IncrementID, "IncrementID");
                    if (Filter.CustomerID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.CustomerID, "CustomerID");
                    if (Filter.ErrorMessage != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.ErrorMessage, "ErrorMessage");
                    if (Filter.UserInfoID != null) strSQL += Database.Filter.StringSearch.GetSQLQuery(Filter.UserInfoID, "UserInfoID");
                    if (Filter.IsPendingApproval != null) strSQL += "AND IsPendingApproval=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPendingApproval.Value).ToString());
                    if (Filter.IsPendingItemPersonalizationApproval != null) strSQL += "AND IsPendingItemPersonalizationApproval=" + Database.HandleQuote(Convert.ToInt32(Filter.IsPendingItemPersonalizationApproval.Value).ToString());
                    if (Filter.IsClosed != null) strSQL += "AND IsClosed=" + Database.HandleQuote(Convert.ToInt32(Filter.IsClosed.Value).ToString());
                    if (Filter.ApprovalNotificationSent != null) strSQL += "AND ApprovalNotificationSent=" + Database.HandleQuote(Convert.ToInt32(Filter.ApprovalNotificationSent.Value).ToString());
                    if (Filter.OrderConfirmationSent != null) strSQL += "AND OrderConfirmationSent=" + Database.HandleQuote(Convert.ToInt32(Filter.OrderConfirmationSent.Value).ToString());
                    if (Filter.BudgetID != null)
                    {
                        strSQL += "AND SalesOrderID IN (SELECT SalesOrderID FROM Payment (NOLOCK) WHERE BudgetAssignmentID IN (SELECT BudgetAssignmentID FROM BudgetAssignment WHERE BudgetID=" + Filter.BudgetID.SearchString + ")) ";
                    }
                }

                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrder), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                if (objData != null && objData.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                    {
                        objNew = new SalesOrder(objData.Tables[0].Rows[i]);
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

        //Sales Orders with line item from a Vendor
        public static List<SalesOrder> GetSalesOrdersByVendor(string VendorID)
        {
            int intTotalCount = 0;
            return GetSalesOrdersByVendor(VendorID, null, null, out intTotalCount);
        }

        public static List<SalesOrder> GetSalesOrdersByVendor(string VendorID, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            return GetSalesOrdersByVendor(VendorID, string.Empty, true, PageSize, PageNumber, out TotalRecord);
        }
        public static List<SalesOrder> GetSalesOrdersByVendor(string VendorID, string SortExpression)
        {
            int intTotalCount = 0;
            return GetSalesOrdersByVendor(VendorID, SortExpression, true, null, null, out intTotalCount);
        }
        public static List<SalesOrder> GetSalesOrdersByVendor(string VendorID, string SortExpression, bool SortAscending, int? PageSize, int? PageNumber, out int TotalRecord)
        {
            List<SalesOrder> objReturn = null;
            SalesOrder objNew = null;
            DataSet objData = null;
            DataTable objDataTable = null;
            string strSQL = string.Empty;

            try
            {
                TotalRecord = 0;

                objReturn = new List<SalesOrder>();

                //                strSQL = string.Format(@"
                //SELECT DISTINCT s.*
                //FROM SalesOrder (NOLOCK) s
                //Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
                //Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
                //WHERE i.VendorID = {0} 
                //and paymentmethod not in ('virtualterm')", VendorID);

                //                strSQL = string.Format(@"
                //SELECT s.SalesOrderID, s.TransactionDate, s.IncrementID, s.SalesSource
                //	, SUM(case when isnull(sl.discountamount,0) != 0 
                //		then  sl.Quantity * sl.UnitPrice - sl.DiscountAmount
                //		else sl.Quantity * sl.UnitPrice end) as VendorTotal
                //FROM SalesOrder (NOLOCK) s
                //Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
                //Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
                //WHERE i.VendorID = {0}
                //and paymentmethod not in ('virtualterm')
                //and s.Status = 'complete'
                //GROUP BY s.SalesOrderID, s.TransactionDate, s.IncrementID, s.SalesSource", Database.HandleQuote(VendorID));

                strSQL = string.Format(@"
SELECT s.SalesOrderID, s.TransactionDate, s.IncrementID, s.SalesSource
	, SUM(sl.Quantity * sl.UnitPrice - isnull(sl.DiscountAmount,0)- isnull(Cred.Amount,0))  as VendorTotal
FROM SalesOrder (NOLOCK) s
Inner Join SalesOrderLine (NOLOCK) sl on sl.SalesOrderID = s.SalesOrderID
Inner Join Item (NOLOCK) i on i.ItemID = sl.ItemID
Outer Apply
(
	SELECT SUM(cl.Quantity) as Qty, SUM(cl.UnitPrice * cl.Quantity) as Amount, SUM(cl.TaxAmount) as TaxAmount
	FROM CreditMemo (NOLOCK) c
	Inner Join CreditMemoLine (NOLOCK) cl on cl.CreditMemoID = c.CreditMemoID
	WHERE cl.SalesOrderLineID = sl.SalesOrderLineID
	and c.Status in ('Refunded','2')
) Cred
WHERE i.VendorID = {0}
and s.paymentmethod not in ('virtualterm')
and s.Status in ('complete','closed')
GROUP BY s.SalesOrderID, s.TransactionDate, s.IncrementID, s.SalesSource
HAVING SUM(sl.Quantity * sl.UnitPrice - isnull(sl.DiscountAmount,0)- isnull(Cred.Amount,0)) != 0
", Database.HandleQuote(VendorID));


                if (PageSize != null && PageNumber != null) strSQL = Database.GetPagingSQL(strSQL, string.IsNullOrEmpty(SortExpression) ? "SalesOrderID" : Utility.CustomSorting.GetSortExpression(typeof(SalesOrder), SortExpression), string.IsNullOrEmpty(SortExpression) ? false : SortAscending, PageSize.Value, PageNumber.Value);
                objData = Database.GetDataSet(strSQL);

                //if (objData != null && objData.Tables[0].Rows.Count > 0)
                //{
                //    for (int i = 0; i < objData.Tables[0].Rows.Count; i++)
                //    {
                //        objNew = new SalesOrder(objData.Tables[0].Rows[i]);
                //        objNew.IsLoaded = true;
                //        objReturn.Add(objNew);
                //    }
                //}

                if (!string.IsNullOrEmpty(SortExpression))
                {
                    objData.Tables[0].DefaultView.Sort = SortExpression;
                    objDataTable = objData.Tables[0].DefaultView.ToTable();
                }
                else
                {
                    objDataTable = objData.Tables[0];
                }

                if (objData != null && objDataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < objDataTable.Rows.Count; i++)
                    {
                        objNew = new SalesOrder(objDataTable.Rows[i]);
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
        //Sales Orders with line item from a Vendor

        public static SalesOrder IfExternalIDExists(string ExternalID)
        {
            SalesOrder objReturn = null;
            SalesOrderFilter objFilter = null;
            try
            {
                objFilter = new ImageSolutions.SalesOrder.SalesOrderFilter();
                objFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ExternalID.SearchString = ExternalID;
                objReturn = ImageSolutions.SalesOrder.SalesOrder.GetSalesOrder(objFilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFilter = null;
            }
            return objReturn;
        }
    }
    public enum enumPaymentMethod
    {
        ccsave,
        paypaluk_express,
        free,
        paypal_express,
        merchant,
        checkmo,
        mpadaptivepayment,
        m2epropayment,
        verisign,
        paypal_direct,
        virtualterm,
        gene_braintree_creditcard,
        paypal_credit_card,
        paypal_express_bml
    }
    public enum enumDepositAccount
    {
        Bank,
        Paypal
    }
}
