using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Payment
{
    public class CustomerPayment
    {
        private static NetSuiteService Service
        {
            get
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                NetSuiteService mNetSuiteService = new NetSuiteService();
                NetSuiteLibrary.User objUser = new NetSuiteLibrary.User();
                mNetSuiteService.tokenPassport = objUser.TokenPassport();
                mNetSuiteService.Url = new Uri(mNetSuiteService.getDataCenterUrls(objUser.Passport().account).dataCenterUrls.webservicesDomain + new Uri(mNetSuiteService.Url).PathAndQuery).ToString();

                return mNetSuiteService;
            }
        }

        private static string NetSuiteCustomerPaymentFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteCustomerPaymentFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteCustomerPaymentFormID"].ToString();
                else
                    throw new Exception("Missing NetSuiteCustomerPaymentFormID");
            }
        }

        private static string ToolotsUSClass
        {
            get
            {
                if (ConfigurationManager.AppSettings["ToolotsUSClass"] != null)
                    return ConfigurationManager.AppSettings["ToolotsUSClass"].ToString();
                else
                    throw new Exception("Missing ToolotsUSClass");
            }
        }

        private static string NetSuiteUSCurrency
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteUSCurrency"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteUSCurrency"].ToString();
                else
                    throw new Exception("Missing NetSuiteUSCurrency");
            }
        }

        private static string NetSuitePayPalReserveAccount
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuitePayPalReserveAccount"] != null)
                    return ConfigurationManager.AppSettings["NetSuitePayPalReserveAccount"].ToString();
                else
                    throw new Exception("Missing NetSuitePayPalReserveAccount");
            }
        }

        private static string NetSuiteToolotsEWBankAccount
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteToolotsEWBankAccount"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteToolotsEWBankAccount"].ToString();
                else
                    throw new Exception("Missing NetSuiteToolotsEWBankAccount");
            }
        }

        private Toolots.Invoice.Invoice mToolotsInvoice = null;

        public Toolots.Invoice.Invoice ToolotsInvoice
        {
            get
            {
                return mToolotsInvoice;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomerPayment mNetSuiteCustomerPayment = null;
        public NetSuiteLibrary.com.netsuite.webservices.CustomerPayment NetSuiteCustomerPayment
        {
            get
            {
                if (mNetSuiteCustomerPayment == null && mToolotsInvoice != null && !string.IsNullOrEmpty(mToolotsInvoice.PaymentNetSuiteInternalID))
                {
                    mNetSuiteCustomerPayment = LoadNetSuiteCustomerPayment(mToolotsInvoice.PaymentNetSuiteInternalID);
                }
                return mNetSuiteCustomerPayment;
            }
        }

        public CustomerPayment(Toolots.Invoice.Invoice ToolotsInvoice)
        {
            mToolotsInvoice = ToolotsInvoice;
        }

        private CustomerPayment(NetSuiteLibrary.com.netsuite.webservices.CustomerPayment NetSuiteCustomerPayment)
        {
            mNetSuiteCustomerPayment = NetSuiteCustomerPayment;
        }

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            CustomerPayment objPayment = null;
            Invoice.Invoice objInvoice = null;

            try
            {
                if (ToolotsInvoice == null) throw new Exception("ToolotsInvoice cannot be null");
                //if (NetSuiteCustomerPayment != null) throw new Exception("CustomerPayment record already exists in NetSuite");

                objInvoice = new Invoice.Invoice(ToolotsInvoice);
                if (objInvoice.NetSuiteInvoice == null) throw new Exception("NetSuite invoice is not found");

                if (objInvoice.NetSuiteInvoice.total == 0 && objInvoice.NetSuiteInvoice.status == "Paid In Full")
                {
                    //No need for customer payment, invoice is $0
                    ToolotsInvoice.PaymentNetSuiteInternalID = "0";
                    ToolotsInvoice.PaymentNetSuiteDocumentNumber = "0";
                }
                else if (
                         ((objInvoice.SalesOrder.ToolotsSalesOrder.SalesSource == Toolots.SalesOrder.SalesOrder.enumSalesSource.Boltontool_com)
                         ||
                         (objInvoice.SalesOrder.ToolotsSalesOrder.SalesSource == Toolots.SalesOrder.SalesOrder.enumSalesSource.Boltontool_Amazon)
                         ||
                         (objInvoice.SalesOrder.ToolotsSalesOrder.SalesSource == Toolots.SalesOrder.SalesOrder.enumSalesSource.Boltontool_Ebay))
                         &&
                         objInvoice.SalesOrder.ToolotsSalesOrder.IsBoltonDropShip
                         &&
                         objInvoice.SalesOrder.ToolotsSalesOrder.PaymentMethod != Toolots.SalesOrder.enumPaymentMethod.paypal_direct
                         &&
                         objInvoice.SalesOrder.ToolotsSalesOrder.PaymentMethod != Toolots.SalesOrder.enumPaymentMethod.paypaluk_express
                         &&
                         objInvoice.SalesOrder.ToolotsSalesOrder.PaymentMethod != Toolots.SalesOrder.enumPaymentMethod.paypaluk_express
                        )
                {
                    //Bolton order before 6/1
                    ToolotsInvoice.PaymentNetSuiteInternalID = "0";
                    ToolotsInvoice.PaymentNetSuiteDocumentNumber = "0";
                }
                else
                {
                    //netsuite invoice is payable status
                    objPayment = ObjectAlreadyExists();

                    if (objPayment != null)
                    {
                        ToolotsInvoice.PaymentNetSuiteInternalID = objPayment.NetSuiteCustomerPayment.internalId;
                        ToolotsInvoice.PaymentNetSuiteDocumentNumber = objPayment.NetSuiteCustomerPayment.tranId;
                    }
                    else
                    {
                        objWriteResponse = Service.add(CreateNetSuiteCustomerPayment());
                        if (objWriteResponse.status.isSuccess != true)
                        {
                            throw new Exception("Unable to create Customer Payment: " + objWriteResponse.status.statusDetail[0].message);
                        }
                        else
                        {
                            ToolotsInvoice.PaymentNetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                            ToolotsInvoice.PaymentNetSuiteDocumentNumber = NetSuiteCustomerPayment.tranId;
                        }
                    }
                }
                ToolotsInvoice.ErrorMessage = string.Empty;
                ToolotsInvoice.Update();
            }
            catch (Exception ex)
            {
                ToolotsInvoice.ErrorMessage = "CustomerPayment.cs - Create() - " + ex.Message;
                ToolotsInvoice.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        public CustomerPayment ObjectAlreadyExists()
        {
            List<CustomerPayment> objCustomerPayments = null;
            CustomerPaymentFilter objFilter = null;
            CustomerPayment objReturn = null;

            try
            {
                objFilter = new CustomerPaymentFilter();
                objFilter.APIExternalID = ToolotsInvoice.InvoiceID;

                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ToolotsInvoice.SalesOrder.Customer.NetSuiteInternalID);

                //objFilter.InvoiceInternalID = ToolotsInvoice.NetSuiteInternalID;
                //objFilter.InvoiceInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;

                objCustomerPayments = GetCustomerPayments(Service, objFilter);
                if (objCustomerPayments != null && objCustomerPayments.Count() > 0)
                {
                    if (objCustomerPayments.Count > 1) throw new Exception("More than one CustomerPayments with API External ID:" + ToolotsInvoice.InvoiceID + " found in Netsuite with InternalIDs " + string.Join(", ", objCustomerPayments.Select(m => m.NetSuiteCustomerPayment.internalId)));
                    objReturn = objCustomerPayments[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerPayments = null;
                objFilter = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomerPayment CreateNetSuiteCustomerPayment()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomerPayment objReturn = null;
            try
            {
                objReturn = new com.netsuite.webservices.CustomerPayment();
                objReturn.internalId = ToolotsInvoice.PaymentNetSuiteInternalID;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCustomerPaymentFormID, RecordType.customerPayment);
                objReturn.customer = NetSuiteHelper.GetRecordRef(ToolotsInvoice.SalesOrder.Customer.NetSuiteInternalID, RecordType.customer);
                objReturn.currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);//us dollars
                objReturn.@class = NetSuiteHelper.GetRecordRef(ToolotsUSClass, RecordType.classification);

                switch (ToolotsInvoice.SalesOrder.PaymentMethod)
                {
                    case Toolots.SalesOrder.enumPaymentMethod.paypaluk_express:
                    case Toolots.SalesOrder.enumPaymentMethod.paypal_direct:
                    case Toolots.SalesOrder.enumPaymentMethod.paypal_express:
                    case Toolots.SalesOrder.enumPaymentMethod.m2epropayment:
                        objReturn.account = NetSuiteHelper.GetRecordRef(NetSuitePayPalReserveAccount, RecordType.account);
                        objReturn.undepFunds = false;
                        objReturn.undepFundsSpecified = true;
                        
                        break;
                    case Toolots.SalesOrder.enumPaymentMethod.merchant:
                    case Toolots.SalesOrder.enumPaymentMethod.virtualterm:
                    case Toolots.SalesOrder.enumPaymentMethod.free:
                    case Toolots.SalesOrder.enumPaymentMethod.mpadaptivepayment:
                        //make sure if this section changes, changes customer payment as well
                        objReturn.undepFunds = true;
                        objReturn.undepFundsSpecified = true;
                        break;
                    case Toolots.SalesOrder.enumPaymentMethod.checkmo:
                        if (ToolotsInvoice.SalesOrder.SalesSource == Toolots.SalesOrder.SalesOrder.enumSalesSource.Toolots)
                        {
                            switch (ToolotsInvoice.DepositAccount)
                            {
                                case Toolots.SalesOrder.enumDepositAccount.Paypal:
                                    objReturn.account = NetSuiteHelper.GetRecordRef(NetSuitePayPalReserveAccount, RecordType.account);
                                    objReturn.undepFunds = false;
                                    objReturn.undepFundsSpecified = true;
                                    break;
                                case Toolots.SalesOrder.enumDepositAccount.Bank:
                                    objReturn.account = NetSuiteHelper.GetRecordRef(NetSuiteToolotsEWBankAccount, RecordType.account);
                                    objReturn.undepFunds = false;
                                    objReturn.undepFundsSpecified = true;
                                    break;
                                default:
                                    throw new Exception(" unahndled deposit account");
                            }
                            if (ToolotsInvoice.DepositDate == null) throw new Exception("Deposit date required for check/ money order");
                            objReturn.tranDate = ToolotsInvoice.DepositDate.Value;
                            objReturn.tranDateSpecified = true;
                        }
                        else
                        {
                            objReturn.undepFunds = true;
                            objReturn.undepFundsSpecified = true;
                        }
                        break;
                    default:
                        throw new Exception("Unhandled paymentmethod " + ToolotsInvoice.SalesOrder.PaymentMethod);
                }
                //handle account -
                if (ToolotsInvoice.SalesOrder.PaymentTrans != null && ToolotsInvoice.SalesOrder.PaymentTrans.Count > 0)
                {
                    var Capture = ToolotsInvoice.SalesOrder.PaymentTrans.FirstOrDefault(p => p.TransType == Toolots.Payment.enumTransType.Capture);
                    if (Capture != null)
                        objReturn.memo = Capture.TransNumber;
                }
                if (objReturn.tranDate == new DateTime() || !objReturn.tranDateSpecified)
                {
                    objReturn.tranDate = ToolotsInvoice.TransactionDate;
                    objReturn.tranDateSpecified = true;
                }
                objReturn.payment = Convert.ToDouble(ToolotsInvoice.Total);
                //objReturn.paymentSpecified = true;

                StringCustomFieldRef objAPIExternalID = new StringCustomFieldRef();
                objAPIExternalID.value = ToolotsInvoice.InvoiceID;
                objAPIExternalID.scriptId = "custbody_api_external_id";

                //SelectCustomFieldRef objSalesSource = new SelectCustomFieldRef();
                //objSalesSource.scriptId = "custbody_sales_source";
                //objSalesSource.value = new ListOrRecordRef();
                //objSalesSource.value.internalId = "1";
                //objSalesSource.value.typeId = "37";

                objReturn.customFieldList = new CustomFieldRef[2];
                objReturn.customFieldList[0] = objAPIExternalID;
                //objReturn.customFieldList[1] = objSalesSource;

                objReturn.applyList = new CustomerPaymentApplyList();
                objReturn.applyList.replaceAll = true;
                objReturn.applyList.apply = new CustomerPaymentApply[1];
                objReturn.applyList.apply[0] = new CustomerPaymentApply();
                objReturn.applyList.apply[0].applyDate = ToolotsInvoice.DepositDate != null ? ToolotsInvoice.DepositDate.Value : ToolotsInvoice.TransactionDate;
                objReturn.applyList.apply[0].applyDateSpecified = true;
                objReturn.applyList.apply[0].amount = Convert.ToDouble(ToolotsInvoice.Total);
                objReturn.applyList.apply[0].amountSpecified = true;
                objReturn.applyList.apply[0].doc = Convert.ToInt64(ToolotsInvoice.NetSuiteInternalID);
                objReturn.applyList.apply[0].docSpecified = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objReturn;
        }

        public bool Delete()
        {
            RecordRef objPurchaseOrderRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                if (ToolotsInvoice == null) throw new Exception("ToolotsInvoice cannot be null");

                if (NetSuiteCustomerPayment != null)
                {
                    objPurchaseOrderRef = new RecordRef();
                    objPurchaseOrderRef.internalId = NetSuiteCustomerPayment.internalId;
                    objPurchaseOrderRef.type = RecordType.customerPayment;
                    objPurchaseOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objPurchaseOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete customer payment: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteCustomerPayment = null;
                    }
                }

                ToolotsInvoice.ErrorMessage = string.Empty;
                ToolotsInvoice.PaymentNetSuiteInternalID = string.Empty;
                ToolotsInvoice.PaymentNetSuiteDocumentNumber = string.Empty;
                ToolotsInvoice.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find Customer Payment with Internal ID"))
                {
                    ToolotsInvoice.ErrorMessage = string.Empty;
                    ToolotsInvoice.PaymentNetSuiteInternalID = string.Empty;
                    ToolotsInvoice.PaymentNetSuiteDocumentNumber = string.Empty;
                    ToolotsInvoice.Update();
                }
                else
                {
                    ToolotsInvoice.ErrorMessage = "CustomerPayment.cs - Delete() - " + ex.Message;
                    ToolotsInvoice.Update();
                }
            }
            finally
            {
                objPurchaseOrderRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomerPayment LoadNetSuiteCustomerPayment(string NetSuiteInternalID)
        {
            RecordRef objPaymentRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.CustomerPayment objReturn = null;

            try
            {
                objPaymentRef = new RecordRef();
                objPaymentRef.type = RecordType.customerPayment;
                objPaymentRef.typeSpecified = true;
                objPaymentRef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objPaymentRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.CustomerPayment))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.CustomerPayment)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find customer payment with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objPaymentRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        private static CustomerPayment GetCustomerPayment(NetSuiteService Service, CustomerPaymentFilter Filter)
        {
            List<CustomerPayment> objCustomerPayments = null;
            CustomerPayment objReturn = null;

            try
            {
                objCustomerPayments = GetCustomerPayments(Service, Filter);
                if (objCustomerPayments != null && objCustomerPayments.Count >= 1) objReturn = objCustomerPayments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerPayments = null;
            }
            return objReturn;
        }

        private static List<CustomerPayment> GetCustomerPayments(NetSuiteService Service, CustomerPaymentFilter Filter)
        {
            List<CustomerPayment> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<CustomerPayment>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objCustomerPayment in objSearchResult.recordList)
                        {
                            if (objCustomerPayment is NetSuiteLibrary.com.netsuite.webservices.CustomerPayment)
                            {
                                objReturn.Add(new CustomerPayment((NetSuiteLibrary.com.netsuite.webservices.CustomerPayment)objCustomerPayment));
                            }
                        }
                        Service.tokenPassport = new NetSuiteLibrary.User().TokenPassport();
                        objSearchResult = Service.searchMoreWithId(objSearchResult.searchId, objSearchResult.pageIndex + 1);
                    }
                    while (objSearchResult.pageSizeSpecified = true && objSearchResult.totalPages >= objSearchResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSearchResult = null;
            }
            return objReturn;
        }

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, CustomerPaymentFilter Filter)
        {
            SearchResult objSearchResult = null;
            TransactionSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objTransacSearch = new TransactionSearch();
                objTransacSearch.basic = new TransactionSearchBasic();

                if (Filter != null)
                {
                    if (Filter.CustomerInternalIDs != null)
                    {
                        objTransacSearch.customerJoin = new CustomerSearchBasic();
                        objTransacSearch.customerJoin.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.CustomerInternalIDs);
                    }

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        objTransacSearch.basic.customFieldList = new SearchCustomField[1];

                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custbody_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objTransacSearch.basic.customFieldList[0] = objAPIExternalID;
                    }

                    if (!string.IsNullOrEmpty(Filter.InvoiceInternalID))
                    {
                        if (Filter.InvoiceInternalIDOperator == null) throw new Exception("InvoiceInernalIDOperator must be specified");

                        SearchMultiSelectField objMultiSelectField = new SearchMultiSelectField();
                        objMultiSelectField.searchValue = new RecordRef[1];
                        objMultiSelectField.searchValue[0] = new RecordRef();
                        objMultiSelectField.searchValue[0].internalId = Filter.InvoiceInternalID;
                        objMultiSelectField.@operator = Filter.InvoiceInternalIDOperator.Value;
                        objMultiSelectField.operatorSpecified = true;

                        objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                        objTransacSearch.createdFromJoin.internalId = objMultiSelectField;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_customerPayment" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find customer payment - " + objSearchResult.status.statusDetail[0].message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objTransacSearch = null;
                objSearchPreferences = null;
            }
            return objSearchResult;
        }

        public static SearchResult GetNetSuiteTransactions(CustomerPaymentFilter Filter)
        {
            return GetNetSuiteTransactions(Service, Filter);
        }

        public static List<CustomerPayment> GetDeletedPaymentsAfter(DateTime Date)
        {
            List<CustomerPayment> objReturn = null;
            GetDeletedResult objDeletedResult = null;
            GetDeletedFilter objFilter = null;
            try
            {
                objFilter = new GetDeletedFilter();
                objFilter.deletedDate = new SearchDateField();
                objFilter.deletedDate.@operator = SearchDateFieldOperator.onOrAfter;
                objFilter.deletedDate.operatorSpecified = true;
                objFilter.deletedDate.searchValue = Date;
                objFilter.deletedDate.searchValueSpecified = true;

                objFilter.type = new SearchEnumMultiSelectField();
                objFilter.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objFilter.type.operatorSpecified = true;
                objFilter.type.searchValue = new string[] { NetSuiteHelper.DeletedRecordType.customerPayment.ToString() };
                objDeletedResult = Service.getDeleted(objFilter, 1);

                objDeletedResult = Service.getDeleted(objFilter, 1);
                if (objDeletedResult != null && objDeletedResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objCustomerPayment in objDeletedResult.deletedRecordList)
                        {
                            if (objCustomerPayment is NetSuiteLibrary.com.netsuite.webservices.CustomerPayment)
                            {
                                objReturn.Add(new CustomerPayment((NetSuiteLibrary.com.netsuite.webservices.CustomerPayment)objCustomerPayment));
                            }
                        }
                        objDeletedResult = Service.getDeleted(objFilter, objDeletedResult.pageIndex + 1);
                    }
                    while (objDeletedResult.pageSizeSpecified = true && objDeletedResult.totalPages >= objDeletedResult.pageIndex);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFilter = null;
                objDeletedResult = null;
            }
            return objReturn;
        }


    }

    
}
