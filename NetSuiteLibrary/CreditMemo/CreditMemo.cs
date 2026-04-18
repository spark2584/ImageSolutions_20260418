using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;

namespace NetSuiteLibrary.CreditMemo
{
    public class CreditMemo
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

        private static string NetSuiteCreditMemoFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteCreditMemoFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteCreditMemoFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        private static string NetSuiteShippingAndHandlingRefundOnlyItemInternalID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteShippingAndHandlingRefundOnlyItemInternalID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteShippingAndHandlingRefundOnlyItemInternalID"].ToString();
                else
                    return string.Empty;
            }
        }

        private static string NetSuiteConsignedLocationID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteConsignedLocationID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteConsignedLocationID"].ToString();
                else
                    throw new Exception("Missing NetSuiteConsignedLocationID");
            }
        }
        private static string NetSuiteDropshipLocationID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteDropshipLocationID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteDropshipLocationID"].ToString();
                else
                    throw new Exception("Missing NetSuiteDropshipLocationID");
            }
        }
        private static string NetSuiteDirectLocationID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteDirectLocationID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteDirectLocationID"].ToString();
                else
                    throw new Exception("Missing NetSuiteDirectLocationID");
            }
        }
        private static string NotTaxable
        {
            get
            {
                if (ConfigurationManager.AppSettings["NotTaxable"] != null)
                    return ConfigurationManager.AppSettings["NotTaxable"].ToString();
                else
                    throw new Exception("Missing NotTaxable");
            }
        }
        private static string Tax9_75
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax9_75"] != null)
                    return ConfigurationManager.AppSettings["Tax9_75"].ToString();
                else
                    throw new Exception("Missing Tax9_75");
            }
        }
        private static string Tax9_50
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax9_50"] != null)
                    return ConfigurationManager.AppSettings["Tax9_50"].ToString();
                else
                    throw new Exception("Missing Tax9_50");
            }
        }
        private static string Tax9_25
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax9_25"] != null)
                    return ConfigurationManager.AppSettings["Tax9_25"].ToString();
                else
                    throw new Exception("Missing Tax9_25");
            }
        }
        private static string Tax9_00
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax9_00"] != null)
                    return ConfigurationManager.AppSettings["Tax9_00"].ToString();
                else
                    throw new Exception("Missing Tax9_00");
            }
        }
        private static string Tax8_25
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax8_25"] != null)
                    return ConfigurationManager.AppSettings["Tax8_25"].ToString();
                else
                    throw new Exception("Missing Tax8_25");
            }
        }
        private static string Tax8_50
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax8_50"] != null)
                    return ConfigurationManager.AppSettings["Tax8_50"].ToString();
                else
                    throw new Exception("Missing Tax8_50");
            }
        }
        private static string Tax4_4375
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax4_4375"] != null)
                    return ConfigurationManager.AppSettings["Tax4_4375"].ToString();
                else
                    throw new Exception("Missing Tax4_4375");
            }
        }
        private static string Tax3_90
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax3_90"] != null)
                    return ConfigurationManager.AppSettings["Tax3_90"].ToString();
                else
                    throw new Exception("Missing Tax3_90");
            }
        }
        private static string Tax6_00
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax6_00"] != null)
                    return ConfigurationManager.AppSettings["Tax6_00"].ToString();
                else
                    throw new Exception("Missing Tax6_00");
            }
        }
        private static string Tax8_60
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax8_60"] != null)
                    return ConfigurationManager.AppSettings["Tax8_60"].ToString();
                else
                    throw new Exception("Missing Tax8_60");
            }
        }
        private static string Tax9_40
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax9_40"] != null)
                    return ConfigurationManager.AppSettings["Tax9_40"].ToString();
                else
                    throw new Exception("Missing Tax9_40");
            }
        }
        private static string Tax8_90
        {
            get
            {
                if (ConfigurationManager.AppSettings["Tax8_90"] != null)
                    return ConfigurationManager.AppSettings["Tax8_90"].ToString();
                else
                    throw new Exception("Missing Tax8_90");
            }
        }
        private static string NetSuiteGeneralDiscountID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteGeneralDiscountID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteGeneralDiscountID"].ToString();
                else
                    throw new Exception("Missing NetSuiteGeneralDiscountID");
            }
        }
        private static string NetSuiteBundleDiscountID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteBundleDiscountID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteBundleDiscountID"].ToString();
                else
                    throw new Exception("Missing NetSuiteBundleDiscountID");
            }
        }

        private Toolots.CreditMemo.CreditMemo mToolotsCreditMemo = null;
        public Toolots.CreditMemo.CreditMemo ToolotsCreditMemo
        {
            get
            {
                if (mToolotsCreditMemo == null && mNetSuiteCreditMemo != null && !string.IsNullOrEmpty(mNetSuiteCreditMemo.internalId))
                {
                    List<Toolots.CreditMemo.CreditMemo> objCreditMemos = null;
                    Toolots.CreditMemo.CreditMemoFilter objFilter = null;

                    try
                    {
                        objFilter = new Toolots.CreditMemo.CreditMemoFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteCreditMemo.internalId;
                        objCreditMemos = Toolots.CreditMemo.CreditMemo.GetCreditMemos(objFilter);
                        if (objCreditMemos != null && objCreditMemos.Count > 0)
                        {
                            mToolotsCreditMemo = objCreditMemos[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objCreditMemos = null;
                    }
                }
                return mToolotsCreditMemo;
            }
            private set
            {
                mToolotsCreditMemo = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.CreditMemo mNetSuiteCreditMemo = null;
        public NetSuiteLibrary.com.netsuite.webservices.CreditMemo NetSuiteCreditMemo
        {
            get
            {
                if (mNetSuiteCreditMemo == null && mToolotsCreditMemo != null && !string.IsNullOrEmpty(mToolotsCreditMemo.NetSuiteInternalID))
                {
                    mNetSuiteCreditMemo = LoadNetSuiteCreditMemo(mToolotsCreditMemo.NetSuiteInternalID);
                }
                return mNetSuiteCreditMemo;
            }
            private set
            {
                mNetSuiteCreditMemo = value;
            }
        }

        private SalesOrder.SalesOrder mSalesOrder = null;
        public SalesOrder.SalesOrder SalesOrder
        {
            get
            {
                if (mSalesOrder == null && ToolotsCreditMemo != null && ToolotsCreditMemo.SalesOrder != null)
                {
                    mSalesOrder = new SalesOrder.SalesOrder(ToolotsCreditMemo.SalesOrder);
                }
                return mSalesOrder;
            }
            private set
            {
                mSalesOrder = value;
            }
        }

        public CreditMemo(Toolots.CreditMemo.CreditMemo ToolotsCreditMemo)
        {
            mToolotsCreditMemo = ToolotsCreditMemo;
        }

        public CreditMemo(NetSuiteLibrary.com.netsuite.webservices.CreditMemo NetSuiteCreditMemo)
        {
            mNetSuiteCreditMemo = NetSuiteCreditMemo;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CreditMemo LoadNetSuiteCreditMemo(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.CreditMemo objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.creditMemo;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.CreditMemo))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.CreditMemo)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find CreditMemo with Internal ID : " + NetSuiteInternalID);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSORef = null;
                objReadResult = null;
            }
            return objReturn;
        }

        public bool Create()
        {
            WriteResponse objWriteResponse = null;
            CreditMemo objCreditMemo = null;

            try
            {
                if (ToolotsCreditMemo == null) throw new Exception("ToolotsCreditMemo cannot be null");
                if (SalesOrder == null) throw new Exception("Sales order is missing");
                //if (NetSuiteCreditMemo != null) throw new Exception("CreditMemo record already exists in NetSuite");
                if (string.IsNullOrEmpty(ToolotsCreditMemo.SalesOrderID)) throw new Exception("SalesOrderID is missing");
                if ((ToolotsCreditMemo.CreditMemoLines == null || ToolotsCreditMemo.CreditMemoLines.Count() == 0) && ToolotsCreditMemo.ShippingAmount != ToolotsCreditMemo.Total)
                {
                    if (ToolotsCreditMemo.SalesOrder.Fulfillments == null || ToolotsCreditMemo.SalesOrder.Fulfillments.Count == 0)
                    {
                        throw new Exception("Manual credit memo were generated (Adjustment Refund)");
                    }
                }
                //if (!ToolotsCreditMemo.SalesOrder.IsCompleted) throw new Exception("Sales order is not yet completed, cannot generate CreditMemo");
                //if (SalesOrder.NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._pendingBilling) throw new Exception("Unable to create NetSuite CreditMemo, sales order stauts is not 'PendingBilling'");

                objCreditMemo = ObjectAlreadyExists();

                if (objCreditMemo != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ToolotsCreditMemo.NetSuiteInternalID = objCreditMemo.NetSuiteCreditMemo.internalId;
                    ToolotsCreditMemo.NetSuiteDocumentNumber = objCreditMemo.NetSuiteCreditMemo.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteCreditMemo());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create CreditMemo: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ToolotsCreditMemo.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ToolotsCreditMemo.NetSuiteDocumentNumber = NetSuiteCreditMemo.tranId;
                    }
                }

                foreach (Toolots.CreditMemo.CreditMemoLine objCreditMemoLines in ToolotsCreditMemo.CreditMemoLines)
                {
                    foreach (NetSuiteLibrary.com.netsuite.webservices.CreditMemoItem objCreditMemoItem in NetSuiteCreditMemo.itemList.item)
                    {
                        string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objCreditMemoItem, "custcol_api_external_id");
                        if (string.IsNullOrEmpty(strAPIExternalID) && (objCreditMemoItem.item.internalId == "0" || objCreditMemoItem.item.internalId == NetSuiteBundleDiscountID))
                        {
                            //This is item group end of group item, ignore
                            //also ignore bundle discount row
                        }
                        else
                        {
                            if (ToolotsCreditMemo.CreditMemoLines.Exists(m => m.CreditMemoLineID == strAPIExternalID))
                            {
                                ToolotsCreditMemo.CreditMemoLines.Find(m => m.CreditMemoLineID == strAPIExternalID).NetSuiteOrderLineID = objCreditMemoItem.orderLine;
                            }
                            else
                            {
                                ToolotsCreditMemo.NetSuiteInternalID = string.Empty;
                                ToolotsCreditMemo.NetSuiteDocumentNumber = string.Empty;
                                throw new Exception("CreditMemoLineID " + objCreditMemoLines.SalesOrderLine.SalesOrderLineID + " did not get created, not found in NetSuite CreditMemo");
                            }
                        }
                    }
                }

                ToolotsCreditMemo.ErrorMessage = string.Empty;
                ToolotsCreditMemo.Update();
            }
            catch (Exception ex)
            {
                ToolotsCreditMemo.ErrorMessage = "CreditMemo.cs - Create() - " + ex.Message;
                ToolotsCreditMemo.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        //public bool Update()
        //{
        //    try
        //    {
        //        //Check to see if CreditMemo total amount match with shipped total amount
        //        if (SalesOrder.NetSuiteSalesOrder == null) throw new Exception("Unable to load NetSuite sales order");

        //        //if (ToolotsCreditMemo.CancelledFulfillmentLines != null && ToolotsCreditMemo.CancelledFulfillmentLines.Count > 0)
        //        //{
        //        //    SalesOrder.Update();
        //        //    SalesOrder = null;
        //        //}

        //        if (SalesOrder.NetSuiteSalesOrder.orderStatus == SalesOrderOrderStatus._closed || SalesOrder.NetSuiteSalesOrder.orderStatus == SalesOrderOrderStatus._fullyBilled)
        //        {
        //            ToolotsCreditMemo.IsReadyToSync = true;
        //        }
        //        else
        //        {
        //            throw new Exception("NetSuite sales order status is currently set to " + SalesOrder.NetSuiteSalesOrder.orderStatus.ToString() + ", it's supposed to be CLOSED or BILLED when invoicing");
        //        }

        //        ToolotsCreditMemo.ErrorMessage = string.Empty;
        //        ToolotsCreditMemo.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ToolotsCreditMemo.ErrorMessage = "CreditMemo.cs - Update() - " + ex.Message;
        //        ToolotsCreditMemo.Update();
        //    }
        //    finally { }
        //    return true;
        //}

        public CreditMemo ObjectAlreadyExists()
        {
            List<CreditMemo> objCreditMemos = null;
            CreditMemoFilter objFilter = null;
            CreditMemo objReturn = null;

            try
            {
                objFilter = new CreditMemoFilter();
                objFilter.APIExternalID = ToolotsCreditMemo.CreditMemoID;

                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ToolotsCreditMemo.SalesOrder.Customer.NetSuiteInternalID);

                //objFilter.SalesOrderInternalID = ToolotsCreditMemo.SalesOrder.NetSuiteInternalID;
                //objFilter.SalesOrderInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;

                //objFilter.PONumber = ToolotsCreditMemo.PONumber;
                //objFilter.PONumberOperator = SearchTextNumberFieldOperator.equalTo;
                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ToolotsCreditMemo.Retailer.NetSuiteCustomerInternalID);

                objCreditMemos = GetCreditMemos(Service, objFilter);
                if (objCreditMemos != null && objCreditMemos.Count() > 0)
                {
                    if (objCreditMemos.Count > 1) throw new Exception("More than one CreditMemos with API External ID:" + ToolotsCreditMemo.CreditMemoID + " found in Netsuite with InternalIDs " + string.Join(", ", objCreditMemos.Select(m => m.NetSuiteCreditMemo.internalId)));
                    objReturn = objCreditMemos[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCreditMemos = null;
                objFilter = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.CreditMemo CreateNetSuiteCreditMemo()
        {
            NetSuiteLibrary.com.netsuite.webservices.CreditMemo objReturn = null;
            int intCustomFieldIndex = 0;
            int intCustomFieldLineIndex = 0;
            int intNullFieldIndex = 0;
            NetSuiteLibrary.Item.Item objItem = null;
            NetSuiteLibrary.SalesOrder.SalesOrder objSalesOrder = null;
            string strItemType = string.Empty;
            string strItemVendorID = string.Empty;
            string str3PLVendorID = string.Empty;
            bool blnIs3PL = false;
            decimal dcmTotal = 0;
            decimal dcmTotalDiscount = 0;
            int intItemIndex = 0;
            string strTaxRate = string.Empty;

            try
            {
                if (ToolotsCreditMemo.SalesOrder == null) throw new Exception("SalesOrder is missing");
                if (ToolotsCreditMemo.SalesOrder.Invoice == null) throw new Exception("Invoice is missing");
                if (ToolotsCreditMemo.SalesOrderID != ToolotsCreditMemo.SalesOrder.Invoice.SalesOrderID) throw new Exception("SalesOrderID do not match");
                if (string.IsNullOrEmpty(ToolotsCreditMemo.SalesOrder.NetSuiteInternalID)) throw new Exception("SalesOrder.NetSuiteInternalID is missing");
                if (string.IsNullOrEmpty(ToolotsCreditMemo.SalesOrder.Invoice.NetSuiteInternalID)) throw new Exception("Invoice.NetSuiteInternalID is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.CreditMemo();
                objReturn.internalId = ToolotsCreditMemo.NetSuiteInternalID;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteCreditMemoFormID, RecordType.creditMemo);
                objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
                objReturn.createdFrom.internalId = ToolotsCreditMemo.SalesOrder.Invoice.NetSuiteInternalID;
                objReturn.createdFrom.type = RecordType.invoice;
                objReturn.createdFrom.typeSpecified = true;

                //change made here need to reflected on custome refund
                objReturn.tranDate = ToolotsCreditMemo.DepositDate != null ? ToolotsCreditMemo.DepositDate.Value : ToolotsCreditMemo.TransactionDate;
                objReturn.tranDateSpecified = true;

                objReturn.salesEffectiveDate = ToolotsCreditMemo.TransactionDate;
                objReturn.salesEffectiveDateSpecified = true;

                //objReturn.shippingAddress = null;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsCreditMemo.CreditMemoID, "custbody_api_external_id");
                objReturn.nullFieldList = new string[99];
                //objReturn.nullFieldList[intNullFieldIndex++] = "salesrep";

                objSalesOrder = new NetSuiteLibrary.SalesOrder.SalesOrder(ToolotsCreditMemo.SalesOrder);
                blnIs3PL = NetSuiteHelper.GetBoolCustomFieldValue(objSalesOrder.NetSuiteSalesOrder, "custbody_is_3pl");
                //if (blnIs3PL) throw new Exception("Should not have credit memo for 3PL transaction");

                objReturn.itemList = new CreditMemoItemList();
                objReturn.itemList.item = new CreditMemoItem[ToolotsCreditMemo.CreditMemoLines.Count() * 2];
                //objReturn.itemList.replaceAll = true;

                if (blnIs3PL)
                {
                    objReturn.shippingCost = 0;
                    objReturn.shippingCostSpecified = true;
                }
                else
                {
                    objReturn.shippingCost = ToolotsCreditMemo.ShippingAmount == null ? 0 : Convert.ToDouble(ToolotsCreditMemo.ShippingAmount.Value);
                    objReturn.shippingCostSpecified = true;
                    dcmTotal += Convert.ToDecimal(objReturn.shippingCost);
                }

                if (ToolotsCreditMemo.CreditMemoLines != null && ToolotsCreditMemo.CreditMemoLines.Count > 0)
                {
                    for (int i = 0; i < ToolotsCreditMemo.CreditMemoLines.Count; i++)
                    {
                        intCustomFieldLineIndex = 0;
                        decimal dcmItemRate = 0;
                        strTaxRate = string.Empty;

                        objItem = new Item.Item(ToolotsCreditMemo.CreditMemoLines[i].Item);

                        objReturn.itemList.item[intItemIndex] = new CreditMemoItem();
                        objReturn.itemList.item[intItemIndex].customFieldList = new CustomFieldRef[99];

                        if (ToolotsCreditMemo.CreditMemoLines[i].Item == null) throw new Exception("Unable to locate VendorSKU");

                        if (objItem.NetSuiteItemGroup != null)
                        {
                            objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ToolotsCreditMemo.CreditMemoLines[i].Item.NetSuiteInternalID, RecordType.itemGroup);
                        }
                        else if (objItem.NetSuiteInventoryItem != null)
                        {
                            objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ToolotsCreditMemo.CreditMemoLines[i].Item.NetSuiteInternalID, RecordType.inventoryItem);
                        }
                        else if (objItem.NetSuiteServiceItem != null)
                        {
                            objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ToolotsCreditMemo.CreditMemoLines[i].Item.NetSuiteInternalID, RecordType.serviceResaleItem);
                        }
                        else
                        {
                            throw new Exception("NetSuite Item not handeled");
                        }

                        objReturn.itemList.item[intItemIndex].price = new RecordRef();
                        objReturn.itemList.item[intItemIndex].price.internalId = "-1"; //Custom Pricing

                        objReturn.itemList.item[intItemIndex].quantity = ToolotsCreditMemo.CreditMemoLines[i].Quantity;
                        objReturn.itemList.item[intItemIndex].quantitySpecified = true;

                        //objReturn.itemList.item[intItemIndex].orderLine = ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.NetSuiteLineID.Value;
                        //objReturn.itemList.item[intItemIndex].orderLineSpecified = true;

                        if (objItem.NetSuiteItemGroup != null)
                        {
                            //no location is needed
                        }
                        else if (objItem.NetSuiteServiceItem != null)
                        {
                            //no location is needed
                        }
                        else
                        {
                            strItemType = NetSuiteHelper.GetSelectCustomFieldValueID(objItem.NetSuiteInventoryItem, "custitem_item_type");
                            switch (strItemType)
                            {
                                case "1": //consignment
                                    objReturn.itemList.item[intItemIndex].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteConsignedLocationID, RecordType.location);
                                    break;
                                case "2": //dropship
                                    objReturn.itemList.item[intItemIndex].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteDropshipLocationID, RecordType.location);
                                    break;
                                case "3": //direct
                                    objReturn.itemList.item[intItemIndex].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteDirectLocationID, RecordType.location);
                                    break;
                                default:
                                    throw new Exception("Unhandeled Item Type");
                            }
                        }

                        if (blnIs3PL)
                        {
                            objReturn.itemList.item[intItemIndex].rate = "0";
                            dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity);
                        }
                        else
                        {
                            if (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine != null)
                            {
                                //Bundle Component
                                if (ToolotsCreditMemo.CreditMemoLines[i].UnitPrice > 0) throw new Exception("Bundle component unit price cannot be greater than $0 (based on Magento's logic)");

                                objReturn.itemList.item[intItemIndex].rate = ToolotsCreditMemo.CreditMemoLines[i].Item.Price.ToString();

                                if (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ItemGroupComponentTotalPrice == ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.UnitPrice)
                                {
                                    dcmItemRate = ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.Item.Price;
                                }
                                //evently distibute price
                                else if (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ItemGroupComponentLines.Select(g => g.Item.Price).Distinct().Count() == 1)
                                {
                                    dcmItemRate = ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.UnitPrice / (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ItemGroupComponentLines.Sum(g => g.Quantity) / ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.Quantity);
                                }
                                else if (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ItemGroupComponentTotalPrice == ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.Item.Price)
                                {
                                    dcmItemRate = (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.UnitPrice / ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ItemGroupComponentTotalPrice.Value) * (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.Item.Price);
                                }
                                else
                                {
                                    throw new Exception("Item group component price not match bundle price ");
                                }
                                dcmItemRate = Math.Round(dcmItemRate, 2);
                                objReturn.itemList.item[intItemIndex].rate = dcmItemRate.ToString();

                                if (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.TaxRate != null && ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.TaxRate != 0)
                                {
                                    strTaxRate = ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.ParentSalesOrderLine.TaxRate.Value.ToString("N4");
                                }
                            }
                            else
                            {
                                if (objItem.NetSuiteItemGroup != null)
                                {
                                    //If it's an item group header
                                    objReturn.itemList.item[intItemIndex].rate = "0";

                                    //item group header does not get taxed
                                }
                                else
                                {
                                    objReturn.itemList.item[intItemIndex].rate = ToolotsCreditMemo.CreditMemoLines[i].UnitPrice.ToString();

                                    if (ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.TaxRate != null && ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.TaxRate != 0)
                                    {
                                        strTaxRate = ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.TaxRate.Value.ToString("N4");
                                    }
                                }
                            }
                            dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity);

                            //if (ToolotsSalesOrder.SalesOrderLines[i].DiscountAmount != null) dcmTotalDiscount += ToolotsSalesOrder.SalesOrderLines[i].DiscountAmount.Value;

                            if (!string.IsNullOrEmpty(strTaxRate))
                            {
                                switch (strTaxRate)
                                {
                                    case "3.9000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax3_90, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0390);
                                        break;
                                    case "4.4375":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax4_4375, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.044375);
                                        break;
                                    case "6.0000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax6_00, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.06);
                                        break;
                                    case "8.2500":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax8_25, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0825);
                                        break;
                                    case "8.5000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax8_50, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0850);
                                        break;
                                    case "8.6000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax8_60, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0860);
                                        break;
                                    case "8.9000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax8_90, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0890);
                                        break;
                                    case "9.0000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax9_00, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.09);
                                        break;
                                    case "9.2500":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax9_25, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0925);
                                        break;
                                    case "9.4000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax9_40, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.094);
                                        break;
                                    case "9.5000":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax9_50, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.095);
                                        break;
                                    case "9.7500":
                                        objReturn.itemList.item[intItemIndex].taxCode = NetSuiteHelper.GetRecordRef(Tax9_75, RecordType.salesTaxItem);
                                        dcmTotal += Convert.ToDecimal(Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity * 0.0975);
                                        break;
                                    default:
                                        throw new Exception("Tax Rate " + ToolotsCreditMemo.CreditMemoLines[i].SalesOrderLine.TaxRate.Value + " not handeled");
                                }
                            }

                            //If there were no shipment, do not entere quantity otherwise NetSuite will automatically receive 
                            if (objSalesOrder.ToolotsSalesOrder.Fulfillments == null || objSalesOrder.ToolotsSalesOrder.Fulfillments.Count == 0)
                            {
                                if (objItem.NetSuiteServiceItem == null)
                                {
                                    objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateLongCustomField(ToolotsCreditMemo.CreditMemoLines[i].Quantity, "custcol_refund_only_quantity");
                                    objReturn.itemList.item[intItemIndex].amount = Convert.ToDouble(objReturn.itemList.item[intItemIndex].rate) * objReturn.itemList.item[intItemIndex].quantity;
                                    objReturn.itemList.item[intItemIndex].amountSpecified = true;
                                    objReturn.itemList.item[intItemIndex].quantity = 0;
                                }
                            }
                            else
                            {
                                throw new Exception("Fulfillment was created, might need to generate RMA");
                            }
                        }

                        objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsCreditMemo.CreditMemoLines[i].CreditMemoLineID, "custcol_api_external_id");
                        
                        intItemIndex++;
                    }

                    dcmTotal = Math.Round(dcmTotal - dcmTotalDiscount, 2);
                    if (!blnIs3PL)
                    {
                        if (dcmTotal != ToolotsCreditMemo.Total) throw new Exception("NetSuite Total: " + dcmTotal + " does not match with DB total " + ToolotsCreditMemo.Total);
                    }
                    else
                    {
                        if (dcmTotal != 0) throw new Exception("3PL credit memo should be $0.00");
                    }
                }
                else
                {
                    //There is no credit memo line
                    //If there were no shipment, do not entere quantity otherwise NetSuite will automatically receive 
                    if (objSalesOrder.ToolotsSalesOrder.Fulfillments == null || objSalesOrder.ToolotsSalesOrder.Fulfillments.Count == 0)
                    {
                        if (!blnIs3PL)
                        {
                            if (ToolotsCreditMemo.ShippingAmount == ToolotsCreditMemo.Total)
                            {
                                objReturn.itemList = new CreditMemoItemList();
                                objReturn.itemList.item = new CreditMemoItem[1];
                                objReturn.itemList.item[0] = new CreditMemoItem();
                                objReturn.itemList.item[0].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteShippingAndHandlingRefundOnlyItemInternalID, RecordType.otherChargeSaleItem);
                                objReturn.itemList.item[0].price = new RecordRef();
                                objReturn.itemList.item[0].price.internalId = "-1"; //Custom Pricing
                                objReturn.itemList.item[0].rate = "0";
                                objReturn.itemList.item[0].quantity = 0;
                                objReturn.itemList.item[0].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteDirectLocationID, RecordType.location);
                            }
                            else if (ToolotsCreditMemo.SalesOrder.CreditMemos.Sum(m => m.Total) != ToolotsCreditMemo.SalesOrder.Total)
                            {
                                throw new Exception("No fulfillment was generated, TOTAL credit memo(s) amount " + ToolotsCreditMemo.Total + " should match with order total " + ToolotsCreditMemo.SalesOrder.Total);
                            }
                        }
                        else
                        {
                            throw new Exception("Cannot create credit memo for 3PL order");
                        }
                    }
                    else
                    {
                        throw new Exception("Fulfillment was created, might need to generate RMA");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.CreditMemo UpdateNetSuiteCreditMemo()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.CreditMemo objReturn = null;

        //    //try
        //    //{
        //    //    objReturn = new NetSuiteLibrary.com.netsuite.webservices.CreditMemo.CreditMemo();
        //    //    objReturn.internalId = ToolotsCreditMemo.NetSuiteInternalID;

        //    //    if (!string.IsNullOrEmpty(ToolotsCreditMemo.TrackingNumber) && !string.IsNullOrEmpty(ToolotsCreditMemo.PackingSlipPath))
        //    //    {
        //    //        objReturn.shipStatus = CreditMemoShipStatus._packed;
        //    //        objReturn.shipStatusSpecified = true;

        //    //        objReturn.generateIntegratedShipperLabel = false;
        //    //        objReturn.generateIntegratedShipperLabelSpecified = true;

        //    //        objReturn.packageList = new com.netsuite.webservices.CreditMemo.CreditMemoPackageList();
        //    //        objReturn.packageList.replaceAll = true;

        //    //        objReturn.packageList.package = new com.netsuite.webservices.CreditMemo.CreditMemoPackage[1];
        //    //        objReturn.packageList.package[0] = new com.netsuite.webservices.CreditMemo.CreditMemoPackage();
        //    //        objReturn.packageList.package[0].packageWeight = 1;
        //    //        objReturn.packageList.package[0].packageWeightSpecified = true;
        //    //        objReturn.packageList.package[0].packageTrackingNumber = ToolotsCreditMemo.TrackingNumber;
        //    //        objReturn.packageList.package[0].packageDescr = ToolotsCreditMemo.SalesOrder.ShippingCode;
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //    //finally { }
        //    return objReturn;
        //}

        public bool Delete()
        {
            RecordRef objPurchaseOrderRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                if (ToolotsCreditMemo == null) throw new Exception("ToolotsCreditMemo cannot be null");

                if (NetSuiteCreditMemo != null)
                {
                    objPurchaseOrderRef = new RecordRef();
                    objPurchaseOrderRef.internalId = NetSuiteCreditMemo.internalId;
                    objPurchaseOrderRef.type = RecordType.creditMemo;
                    objPurchaseOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objPurchaseOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete CreditMemo: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteCreditMemo = null;
                    }
                }

                ToolotsCreditMemo.ErrorMessage = string.Empty;
                ToolotsCreditMemo.NetSuiteInternalID = string.Empty;
                ToolotsCreditMemo.NetSuiteDocumentNumber = string.Empty;
                ToolotsCreditMemo.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find CreditMemo with Internal ID"))
                {
                    ToolotsCreditMemo.ErrorMessage = string.Empty;
                    ToolotsCreditMemo.NetSuiteInternalID = string.Empty;
                    ToolotsCreditMemo.NetSuiteDocumentNumber = string.Empty;
                    ToolotsCreditMemo.Update();
                }
                else
                {
                    ToolotsCreditMemo.ErrorMessage = "CreditMemo.cs - Delete() - " + ex.Message;
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

        //use to find out replace all saleslines or not when update CreditMemo to netsuite
        //private bool isAllSalesLineNew()
        //{
        //    return !(CreditMemoLines != null && CreditMemoLines.Exists(sl => sl.LineID != null && sl.LineID > 0));
        //}

        //public bool IsRequireCreditMemo()
        //{
        //    List<CreditMemoLine> Fulfillablelines = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Status) && OrderStatus == null) throw new Exception("Status and OrderStatus are not defined");
        //        //Fulfillablelines = GetFulfillableLines();
        //        if ((OrderStatus != null && (OrderStatus == NetSuiteLibrary.com.netsuite.webservices.CreditMemo.CreditMemoOrderStatus._partiallyFulfilled || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.CreditMemo.CreditMemoOrderStatus._pendingBillingPartFulfilled
        //                || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.CreditMemo.CreditMemoOrderStatus._pendingCreditMemo)) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else if ((Status.ToLower().Contains("pending CreditMemo") || Status.ToLower().Contains("partially fulfilled") || Status.Contains("待完成") || Status.Contains("部分完成")) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        Fulfillablelines = null;
        //    }
        //}

        //public List<CreditMemoLine> GetFulfillableLines()
        //{
        //    List<CreditMemoLine> lstReturn = null;
        //    if (this.CreditMemo.CreditMemoLines != null && this.CreditMemo.CreditMemoLines.Count() > 0)
        //    {
        //        lstReturn = this.CreditMemo.CreditMemoLines;
        //        //when there are other CreditMemos exist for the same CreditMemo
        //        if (this.CreditMemo.CreditMemos != null && this.CreditMemo.CreditMemos.Count() > 0)
        //        {
        //            //CreditMemolines check against all CreditMemolines of all CreditMemo
        //            foreach (CreditMemo.CreditMemo.CreditMemo CreditMemo in this.CreditMemo.CreditMemos)
        //            {
        //                foreach (CreditMemo.CreditMemo.CreditMemoLine CreditMemoLine in CreditMemo.CreditMemo.CreditMemoLines)
        //                {
        //                    if (lstReturn.Any(sl => sl.LineID == CreditMemoLine.OrderLine))
        //                        lstReturn.Where(sl => sl.LineID == CreditMemoLine.OrderLine).First().Qty -= CreditMemoLine.Quantity;
        //                }
        //            }
        //            lstReturn = lstReturn.Any(sl => sl.Qty > 0) ? lstReturn = lstReturn.Where(sl => sl.Qty > 0).ToList() : null;
        //        }
        //    }
        //    return lstReturn;
        //}
        //public List<CreditMemo.CreditMemo.CreditMemoLine> GetAllCreditMemoLines()
        //{
        //    List<CreditMemo.CreditMemo.CreditMemoLine> lstReturn = null;
        //    if (this.CreditMemo.CreditMemos != null && this.CreditMemo.CreditMemos.Count() > 0)
        //    {
        //        lstReturn = new List<CreditMemo.CreditMemo.CreditMemoLine>();
        //        foreach (CreditMemo.CreditMemo.CreditMemo CreditMemo in this.CreditMemo.CreditMemos)
        //        {
        //            if (CreditMemo.CreditMemo.CreditMemoLines != null && CreditMemo.CreditMemo.CreditMemoLines.Count() > 0)
        //            {
        //                lstReturn = lstReturn.Concat(CreditMemo.CreditMemo.CreditMemoLines).ToList();
        //            }
        //        }
        //    }
        //    return lstReturn;
        //}

        public static CreditMemo GetCreditMemo(CreditMemoFilter Filter)
        {
            return GetCreditMemo(Service, Filter);
        }

        private static CreditMemo GetCreditMemo(NetSuiteService Service, CreditMemoFilter Filter)
        {
            List<CreditMemo> objCreditMemos = null;
            CreditMemo objReturn = null;

            try
            {
                objCreditMemos = GetCreditMemos(Service, Filter);
                if (objCreditMemos != null && objCreditMemos.Count >= 1) objReturn = objCreditMemos[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objCreditMemos = null;
            }
            return objReturn;
        }

        private static List<CreditMemo> GetCreditMemos(NetSuiteService Service, CreditMemoFilter Filter)
        {
            List<CreditMemo> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<CreditMemo>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetCreditMemo in objSearchResult.recordList)
                        {
                            if (objNetCreditMemo is NetSuiteLibrary.com.netsuite.webservices.CreditMemo)
                            {
                                objReturn.Add(new CreditMemo((NetSuiteLibrary.com.netsuite.webservices.CreditMemo)objNetCreditMemo));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, CreditMemoFilter Filter)
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

                    if (!string.IsNullOrEmpty(Filter.DocumentNumber))
                    {
                        objTransacSearch.basic.tranId = new SearchStringField();
                        objTransacSearch.basic.tranId.searchValue = Filter.DocumentNumber;
                        objTransacSearch.basic.tranId.@operator = SearchStringFieldOperator.@is;
                        objTransacSearch.basic.tranId.operatorSpecified = true;
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

                    if (!string.IsNullOrEmpty(Filter.SalesOrderInternalID))
                    {
                        if (Filter.SalesOrderInternalIDOperator == null) throw new Exception("SalesOrderInternalIDOperator must be specified");

                        SearchMultiSelectField objMultiSelectField = new SearchMultiSelectField();
                        objMultiSelectField.searchValue = new RecordRef[1];
                        objMultiSelectField.searchValue[0] = new RecordRef();
                        objMultiSelectField.searchValue[0].internalId = Filter.SalesOrderInternalID;
                        objMultiSelectField.@operator = Filter.SalesOrderInternalIDOperator.Value;
                        objMultiSelectField.operatorSpecified = true;

                        objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                        objTransacSearch.createdFromJoin.internalId = objMultiSelectField;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_creditMemo" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find CreditMemo - " + objSearchResult.status.statusDetail[0].message);
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
    }
}


