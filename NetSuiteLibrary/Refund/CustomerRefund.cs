using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetSuiteLibrary.Refund
{
    public class CustomerRefund
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

        private static string NetSuiteCustomerRefundFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteCustomerRefundFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteCustomerRefundFormID"].ToString();
                else
                    throw new Exception("Missing NetSuiteCustomerRefundFormID");
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
        private static string NetSuiteUndepositedFunds
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteUndepositedFunds"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteUndepositedFunds"].ToString();
                else
                    throw new Exception("Missing NetSuiteUndepositedFunds");
            }
        }

        private Toolots.CreditMemo.CreditMemo mToolotsCreditMemo = null;

        public Toolots.CreditMemo.CreditMemo ToolotsCreditMemo
        {
            get
            {
                return mToolotsCreditMemo;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomerRefund mNetSuiteCustomerRefund = null;
        public NetSuiteLibrary.com.netsuite.webservices.CustomerRefund NetSuiteCustomerRefund
        {
            get
            {
                if (mNetSuiteCustomerRefund == null && mToolotsCreditMemo != null && !string.IsNullOrEmpty(mToolotsCreditMemo.RefundNetSuiteInternalID))
                {
                    mNetSuiteCustomerRefund = LoadNetSuiteCustomerRefund(mToolotsCreditMemo.RefundNetSuiteInternalID);
                }
                return mNetSuiteCustomerRefund;
            }
        }

        public CustomerRefund(Toolots.CreditMemo.CreditMemo ToolotsCreditMemo)
        {
            mToolotsCreditMemo = ToolotsCreditMemo;
        }

        private CustomerRefund(NetSuiteLibrary.com.netsuite.webservices.CustomerRefund NetSuiteCustomerRefund)
        {
            mNetSuiteCustomerRefund = NetSuiteCustomerRefund;
        }

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            CustomerRefund objRefund = null;
            CreditMemo.CreditMemo objCreditMemo = null;

            try
            {
                if (ToolotsCreditMemo == null) throw new Exception("ToolotsCreditMemo cannot be null");
                if (NetSuiteCustomerRefund != null) throw new Exception("CustomerRefund record already exists in NetSuite");

                objCreditMemo = new CreditMemo.CreditMemo(ToolotsCreditMemo);
                if (objCreditMemo.NetSuiteCreditMemo == null) throw new Exception("NetSuite CreditMemo is not found");
                

                if (objCreditMemo.NetSuiteCreditMemo.total == 0 && objCreditMemo.NetSuiteCreditMemo.status == "Fully Applied")
                {
                    //No need for customer Refund, CreditMemo is $0
                    ToolotsCreditMemo.RefundNetSuiteInternalID = "0";
                    ToolotsCreditMemo.RefundNetSuiteDocumentNumber = "0";
                }
                else
                {
                    //netsuite CreditMemo is payable status
                    objRefund = ObjectAlreadyExists();

                    if (objRefund != null)
                    {
                        ToolotsCreditMemo.RefundNetSuiteInternalID = objRefund.NetSuiteCustomerRefund.internalId;
                        ToolotsCreditMemo.RefundNetSuiteDocumentNumber = objRefund.NetSuiteCustomerRefund.tranId;
                    }
                    else
                    {
                        if (objCreditMemo.NetSuiteCreditMemo.applied > 0) throw new Exception("NetSuite creditMemo already applied to other transactions");
                        objWriteResponse = Service.add(CreateNetSuiteCustomerRefund());
                        if (objWriteResponse.status.isSuccess != true)
                        {
                            throw new Exception("Unable to create Customer Refund: " + objWriteResponse.status.statusDetail[0].message);
                        }
                        else
                        {
                            ToolotsCreditMemo.RefundNetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                            ToolotsCreditMemo.RefundNetSuiteDocumentNumber = NetSuiteCustomerRefund.tranId;
                        }
                    }
                }
                ToolotsCreditMemo.ErrorMessage = string.Empty;
                ToolotsCreditMemo.Update();
            }
            catch (Exception ex)
            {
                ToolotsCreditMemo.ErrorMessage = "CustomerRefund.cs - Create() - " + ex.Message;
                ToolotsCreditMemo.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        public CustomerRefund ObjectAlreadyExists()
        {
            List<CustomerRefund> objCustomerRefunds = null;
            CustomerRefundFilter objFilter = null;
            CustomerRefund objReturn = null;

            try
            {
                objFilter = new CustomerRefundFilter();
                objFilter.APIExternalID = ToolotsCreditMemo.CreditMemoID;

                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ToolotsCreditMemo.SalesOrder.Customer.NetSuiteInternalID);

                //objFilter.CreditMemoInternalID = ToolotsCreditMemo.NetSuiteInternalID;
                //objFilter.CreditMemoInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;

                objCustomerRefunds = GetCustomerRefunds(Service, objFilter);
                if (objCustomerRefunds != null && objCustomerRefunds.Count() > 0)
                {
                    if (objCustomerRefunds.Count > 1) throw new Exception("More than one CustomerRefunds with API External ID:" + ToolotsCreditMemo.CreditMemoID + " found in Netsuite with InternalIDs " + string.Join(", ", objCustomerRefunds.Select(m => m.NetSuiteCustomerRefund.internalId)));
                    objReturn = objCustomerRefunds[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerRefunds = null;
                objFilter = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomerRefund CreateNetSuiteCustomerRefund()
        {
            NetSuiteLibrary.com.netsuite.webservices.CustomerRefund objReturn = null;

            try
            {
                objReturn = new com.netsuite.webservices.CustomerRefund();
                objReturn.internalId = ToolotsCreditMemo.RefundNetSuiteInternalID;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCustomerRefundFormID, RecordType.customerRefund);
                objReturn.customer = NetSuiteHelper.GetRecordRef(ToolotsCreditMemo.SalesOrder.Customer.NetSuiteInternalID, RecordType.customer);
                objReturn.currency = NetSuiteHelper.GetRecordRef(NetSuiteUSCurrency, RecordType.currency);//us dollars
                objReturn.@class = NetSuiteHelper.GetRecordRef(ToolotsUSClass, RecordType.classification);

                //default behavior 
                //switch (ToolotsCreditMemo.SalesOrder.PaymentMethod)
                //{
                //    case Toolots.SalesOrder.enumPaymentMethod.paypaluk_express:
                //    case Toolots.SalesOrder.enumPaymentMethod.paypal_direct:
                //    case Toolots.SalesOrder.enumPaymentMethod.paypal_express:
                //    case Toolots.SalesOrder.enumPaymentMethod.m2epropayment:
                //        objReturn.account = NetSuiteHelper.GetRecordRef(NetSuitePayPalReserveAccount, RecordType.account);
                //        break;
                //    default:
                //        //make sure if this section changes, changes customer payment as well
                //        objReturn.account = NetSuiteHelper.GetRecordRef(NetSuiteUndepositedFunds, RecordType.account);
                //        break;
                //}
                if (!string.IsNullOrEmpty(ToolotsCreditMemo.TransactionID))
                {
                    //payPal refund
                    objReturn.account = NetSuiteHelper.GetRecordRef(NetSuitePayPalReserveAccount, RecordType.account);
                    objReturn.tranDate = ToolotsCreditMemo.TransactionDate;
                    objReturn.memo = ToolotsCreditMemo.TransactionID;
                }
                else
                {
                    //off line refund 
                    if (ToolotsCreditMemo.DepositAccount == null) throw new Exception("Offline refund Deposit account is required");
                    if (ToolotsCreditMemo.DepositDate == null) throw new Exception("Offline refund Deposit Date is required");
                    switch (ToolotsCreditMemo.DepositAccount)
                    {
                        case Toolots.SalesOrder.enumDepositAccount.Paypal:
                            objReturn.account = NetSuiteHelper.GetRecordRef(NetSuitePayPalReserveAccount, RecordType.account);
                            break;
                        case Toolots.SalesOrder.enumDepositAccount.Bank:
                            objReturn.account = NetSuiteHelper.GetRecordRef(NetSuiteToolotsEWBankAccount, RecordType.account);
                            break;
                        default:
                            throw new Exception("Unhandled deposit account");
                    }
                    objReturn.tranDate = ToolotsCreditMemo.DepositDate.Value;
                }

                if (objReturn.tranDate == null) throw new Exception("TranDate is required");
                objReturn.tranDateSpecified = true;

                //objReturn.balance = Convert.ToDouble(ToolotsCreditMemo.Total);
                //objReturn.balanceSpecified = true;

                StringCustomFieldRef objAPIExternalID = new StringCustomFieldRef();
                objAPIExternalID.value = ToolotsCreditMemo.CreditMemoID;
                objAPIExternalID.scriptId = "custbody_api_external_id";

                //SelectCustomFieldRef objSalesSource = new SelectCustomFieldRef();
                //objSalesSource.scriptId = "custbody_sales_source";
                //objSalesSource.value = new ListOrRecordRef();
                //objSalesSource.value.internalId = "1";
                //objSalesSource.value.typeId = "37";

                objReturn.customFieldList = new CustomFieldRef[2];
                objReturn.customFieldList[0] = objAPIExternalID;
                //objReturn.customFieldList[1] = objSalesSource;

                objReturn.applyList = new CustomerRefundApplyList();
                objReturn.applyList.replaceAll = true;
                objReturn.applyList.apply = new CustomerRefundApply[1];
                objReturn.applyList.apply[0] = new CustomerRefundApply();
                objReturn.applyList.apply[0].applyDate = ToolotsCreditMemo.DepositDate != null ? ToolotsCreditMemo.DepositDate.Value : ToolotsCreditMemo.TransactionDate;
                objReturn.applyList.apply[0].applyDateSpecified = true;
                objReturn.applyList.apply[0].amount = Convert.ToDouble(ToolotsCreditMemo.Total);
                objReturn.applyList.apply[0].amountSpecified = true;
                objReturn.applyList.apply[0].doc = Convert.ToInt64(ToolotsCreditMemo.NetSuiteInternalID);
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
                if (ToolotsCreditMemo == null) throw new Exception("ToolotsCreditMemo cannot be null");

                if (NetSuiteCustomerRefund != null)
                {
                    objPurchaseOrderRef = new RecordRef();
                    objPurchaseOrderRef.internalId = NetSuiteCustomerRefund.internalId;
                    objPurchaseOrderRef.type = RecordType.customerRefund;
                    objPurchaseOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objPurchaseOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete customer Refund: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteCustomerRefund = null;
                    }
                }

                ToolotsCreditMemo.ErrorMessage = string.Empty;
                ToolotsCreditMemo.NetSuiteInternalID = string.Empty;
                ToolotsCreditMemo.NetSuiteDocumentNumber = string.Empty;
                ToolotsCreditMemo.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find Customer Refund with Internal ID"))
                {
                    ToolotsCreditMemo.ErrorMessage = string.Empty;
                    ToolotsCreditMemo.NetSuiteInternalID = string.Empty;
                    ToolotsCreditMemo.NetSuiteDocumentNumber = string.Empty;
                    ToolotsCreditMemo.Update();
                }
                else
                {
                    ToolotsCreditMemo.ErrorMessage = "CustomerRefund.cs - Delete() - " + ex.Message;
                    ToolotsCreditMemo.Update();
                }
            }
            finally
            {
                objPurchaseOrderRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CustomerRefund LoadNetSuiteCustomerRefund(string NetSuiteInternalID)
        {
            RecordRef objRefundRef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.CustomerRefund objReturn = null;

            try
            {
                objRefundRef = new RecordRef();
                objRefundRef.type = RecordType.customerRefund;
                objRefundRef.typeSpecified = true;
                objRefundRef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objRefundRef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.CustomerRefund))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.CustomerRefund)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find customer Refund with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objRefundRef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        private static CustomerRefund GetCustomerRefund(NetSuiteService Service, CustomerRefundFilter Filter)
        {
            List<CustomerRefund> objCustomerRefunds = null;
            CustomerRefund objReturn = null;

            try
            {
                objCustomerRefunds = GetCustomerRefunds(Service, Filter);
                if (objCustomerRefunds != null && objCustomerRefunds.Count >= 1) objReturn = objCustomerRefunds[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCustomerRefunds = null;
            }
            return objReturn;
        }

        private static List<CustomerRefund> GetCustomerRefunds(NetSuiteService Service, CustomerRefundFilter Filter)
        {
            List<CustomerRefund> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<CustomerRefund>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objCustomerRefund in objSearchResult.recordList)
                        {
                            if (objCustomerRefund is NetSuiteLibrary.com.netsuite.webservices.CustomerRefund)
                            {
                                objReturn.Add(new CustomerRefund((NetSuiteLibrary.com.netsuite.webservices.CustomerRefund)objCustomerRefund));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, CustomerRefundFilter Filter)
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

                    if (!string.IsNullOrEmpty(Filter.CreditMemoInternalID))
                    {
                        if (Filter.CreditMemoInternalIDOperator == null) throw new Exception("CreditMemoInernalIDOperator must be specified");

                        SearchMultiSelectField objMultiSelectField = new SearchMultiSelectField();
                        objMultiSelectField.searchValue = new RecordRef[1];
                        objMultiSelectField.searchValue[0] = new RecordRef();
                        objMultiSelectField.searchValue[0].internalId = Filter.CreditMemoInternalID;
                        objMultiSelectField.@operator = Filter.CreditMemoInternalIDOperator.Value;
                        objMultiSelectField.operatorSpecified = true;

                        objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                        objTransacSearch.createdFromJoin.internalId = objMultiSelectField;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_customerRefund" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find customer Refund - " + objSearchResult.status.statusDetail[0].message);
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

        public static SearchResult GetNetSuiteTransactions(CustomerRefundFilter Filter)
        {
            return GetNetSuiteTransactions(Service, Filter);
        }
    }

    
}
