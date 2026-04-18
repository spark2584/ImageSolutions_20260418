using NetSuiteLibrary.com.netsuite.webservices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;

namespace NetSuiteLibrary.SalesOrder
{
    public class SalesOrder : NetSuiteBase
    {
        private static string NetSuiteSalesOrderFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteSalesOrderFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteSalesOrderFormID"].ToString();
                else
                    throw new Exception("Missing NetSuiteSalesOrderFormID");
            }
        }
        private ImageSolutions.SalesOrder.SalesOrder mImageSolutionsSalesOrder = null;
        public ImageSolutions.SalesOrder.SalesOrder ImageSolutionsSalesOrder
        {
            get
            {
                if (mImageSolutionsSalesOrder == null && mNetSuiteSalesOrder != null && !string.IsNullOrEmpty(mNetSuiteSalesOrder.internalId))
                {
                    List<ImageSolutions.SalesOrder.SalesOrder> objSalesOrders = null;
                    ImageSolutions.SalesOrder.SalesOrderFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.SalesOrder.SalesOrderFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteSalesOrder.internalId;
                        objSalesOrders = ImageSolutions.SalesOrder.SalesOrder.GetSalesOrders(objFilter);
                        if (objSalesOrders != null && objSalesOrders.Count > 0)
                        {
                            mImageSolutionsSalesOrder = objSalesOrders[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objSalesOrders = null;
                    }
                }
                return mImageSolutionsSalesOrder;
            }
            private set
            {
                mImageSolutionsSalesOrder = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.SalesOrder mNetSuiteSalesOrder = null;
        public NetSuiteLibrary.com.netsuite.webservices.SalesOrder NetSuiteSalesOrder
        {
            get
            {
                if (mNetSuiteSalesOrder == null && mImageSolutionsSalesOrder != null && !string.IsNullOrEmpty(mImageSolutionsSalesOrder.NetSuiteInternalID))
                {
                    mNetSuiteSalesOrder = LoadNetSuiteSalesOrder(mImageSolutionsSalesOrder.NetSuiteInternalID);
                }
                return mNetSuiteSalesOrder;
            }
            private set
            {
                mNetSuiteSalesOrder = value;
            }
        }

        public void GetFile(string InternalID)
        {
            NetSuiteLibrary.com.netsuite.webservices.File objReturn = null;
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;

            objSORef = new RecordRef();
            objSORef.type = RecordType.file;
            objSORef.typeSpecified = true;
            objSORef.internalId = InternalID;

            objReadResult = Service.get(objSORef);

            if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.File))
            {
                objReturn = (NetSuiteLibrary.com.netsuite.webservices.File)objReadResult.record;
            }
            else
            {
                throw new Exception("Can not find Sales Order with Internal ID : " + InternalID);
            }

        }
        public SalesOrder(string InternalID)
        {
            mNetSuiteSalesOrder = LoadNetSuiteSalesOrder(InternalID);
        }

        public SalesOrder(ImageSolutions.SalesOrder.SalesOrder ImageSolutionsSalesOrder)
        {
            mImageSolutionsSalesOrder = ImageSolutionsSalesOrder;
        }

        public SalesOrder(NetSuiteLibrary.com.netsuite.webservices.SalesOrder NetSuiteSalesOrder)
        {
            mNetSuiteSalesOrder = NetSuiteSalesOrder;
        }

        private NetSuiteLibrary.com.netsuite.webservices.SalesOrder LoadNetSuiteSalesOrder(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.salesOrder;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.SalesOrder))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.SalesOrder)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Sales Order with Internal ID : " + NetSuiteInternalID);
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
            SalesOrder objSalesOrder = null;

            try
            {
                if (ImageSolutionsSalesOrder == null) throw new Exception("ImageSolutionsSalesOrder cannot be null");
                //if (string.IsNullOrEmpty(ImageSolutionsSalesOrder.Customer.NetSuiteInternalID)) throw new Exception("Missing Customer.NetSuiteInternalID");
                if (ImageSolutionsSalesOrder.SalesOrderLines == null || ImageSolutionsSalesOrder.SalesOrderLines.Count() == 0) throw new Exception("Sales order lines are missing");
                objSalesOrder = ObjectAlreadyExists();
                if (objSalesOrder != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsSalesOrder.NetSuiteInternalID = objSalesOrder.NetSuiteSalesOrder.internalId;
                    ImageSolutionsSalesOrder.SalesOrderNumber = objSalesOrder.NetSuiteSalesOrder.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteSalesOrder());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create sales order: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsSalesOrder.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        //ImageSolutionsSalesOrder.SalesOrderNumber = NetSuiteSalesOrder.tranId;
                    }
                }
                //matching commcerhub salesorderline to netsuite salesorderline
                //foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in ImageSolutionsSalesOrder.SalesOrderLines)
                //{
                //foreach (NetSuiteLibrary.com.netsuite.webservices.SalesOrderItem objSalesOrderItem in NetSuiteSalesOrder.itemList.item)
                //{
                //    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objSalesOrderItem, "custcol_api_external_id");
                //    if (string.IsNullOrEmpty(strAPIExternalID) && (objSalesOrderItem.item.internalId == "0" || objSalesOrderItem.item.internalId == GiftWrapInternalID || objSalesOrderItem.item.internalId == TaxItemInternalID))
                //    {
                //        //This is item group end of group item, ignore
                //        //also ignore bundle discount row
                //    }
                //    else
                //    {
                //        if (ImageSolutionsSalesOrder.SalesOrderLines.Exists(m => m.SalesOrderLineID == strAPIExternalID))
                //        {
                //            ImageSolutionsSalesOrder.SalesOrderLines.Find(m => m.SalesOrderLineID == strAPIExternalID).NetSuiteLineID = objSalesOrderItem.line;
                //        }
                //        else
                //        {
                //            ImageSolutionsSalesOrder.NetSuiteInternalID = string.Empty;
                //            ImageSolutionsSalesOrder.SalesOrderNumber = string.Empty;
                //            throw new Exception("APIExternalID for sales order line " + strAPIExternalID + " did not get created, not found in NetSuite order");
                //        }
                //    }
                //}
                //}
                ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
                ImageSolutionsSalesOrder.Update();
                Console.WriteLine("Sales Order " + ImageSolutionsSalesOrder.SalesOrderID + " sync complete");
            }
            catch (Exception ex)
            {
                ImageSolutionsSalesOrder.ErrorMessage = "SalesOrder.cs - Create() - " + ex.Message;
                Console.WriteLine("Sales Order " + ImageSolutionsSalesOrder.SalesOrderID + " " + ImageSolutionsSalesOrder.ErrorMessage);
                ImageSolutionsSalesOrder.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        //public bool UpdateShippingMemo()
        //{
        //    WriteResponse objWriteResult = null;

        //    try
        //    {
        //        if (ImageSolutionsSalesOrder == null) throw new Exception("ImageSolutionsSalesOrder cannot be null");
        //        if (NetSuiteSalesOrder == null) throw new Exception("Sales order record does not exists in NetSuite");

        //        objWriteResult = Service.update(UpdateNetSuiteSalesOrderShippingMemo());

        //        if (objWriteResult.status.isSuccess != true)
        //        {
        //            throw new Exception("SalesOrder Update : sales order can not be updated " + objWriteResult.status.statusDetail[0].message);
        //        }
        //        else
        //        {
        //            mNetSuiteSalesOrder = null;
        //        }
        //        ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
        //        ImageSolutionsSalesOrder.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsSalesOrder.ErrorMessage = "SalesOrder.cs - Update() - " + ex.Message;
        //        ImageSolutionsSalesOrder.Update();
        //    }
        //    finally
        //    {
        //        objWriteResult = null;
        //    }
        //    return true;
        //}

        public bool CloseLines(List<ImageSolutions.SalesOrder.SalesOrderLine> SalesOrderLines)
        {
            WriteResponse objWriteResult = null;

            try
            {
                if (ImageSolutionsSalesOrder == null) throw new Exception("ImageSolutionsSalesOrder cannot be null");
                if (NetSuiteSalesOrder == null) throw new Exception("Sales order record does not exists in NetSuite");

                if (NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._closed)
                {
                    if (NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._pendingFulfillment && NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._partiallyFulfilled && NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._pendingBillingPartFulfilled) throw new Exception("Sales Order status must be pending fulfillment or partially fulfilled in order to cancel/close");

                    objWriteResult = Service.update(CloseNetSuiteSalesOrderLines(SalesOrderLines));

                    if (objWriteResult.status.isSuccess != true)
                    {
                        throw new Exception("SalesOrder CloseLines() : " + objWriteResult.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteSalesOrder = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWriteResult = null;
            }
            return true;
        }

        private NetSuiteLibrary.com.netsuite.webservices.SalesOrder CloseNetSuiteSalesOrderLines(List<ImageSolutions.SalesOrder.SalesOrderLine> SalesOrderLines)
        {
            //Sometimes we need to close all the lines, https://netsuite.custhelp.com/app/answers/detail/a_id/13965/kw/Invalid%20orderstatus%20reference%20key%20H
            NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = null;
            SalesOrderItem objNetSuiteSOItem = null;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.SalesOrder();
                objReturn.internalId = ImageSolutionsSalesOrder.NetSuiteInternalID;

                objReturn.itemList = new SalesOrderItemList();
                objReturn.itemList.item = new SalesOrderItem[SalesOrderLines.Count];
                objReturn.itemList.replaceAll = false; //By default this is true, if you set it to false, you only need to specifiy the lines you wish to make changes for

                for (int i=0; i < SalesOrderLines.Count; i++)
                {
                    objNetSuiteSOItem = NetSuiteSalesOrder.itemList.item.First(m => SalesOrderLines[i].SalesOrderLineID == NetSuiteHelper.GetStringCustomFieldValue(m, "custcol_api_external_id"));
                    if (objNetSuiteSOItem == null) throw new Exception("Unable to find NetSuite line with APIExternalID:" + SalesOrderLines[i].SalesOrderLineID);
                    if (objNetSuiteSOItem.quantityFulfilled == objNetSuiteSOItem.quantity) throw new Exception("Unable to close NetSuite line, item has been fully fulfilled");
                    
                    objReturn.itemList.item[i] = new SalesOrderItem();
                    objReturn.itemList.item[i].line = objNetSuiteSOItem.line;
                    objReturn.itemList.item[i].lineSpecified = true;
                    objReturn.itemList.item[i].isClosed = true;
                    objReturn.itemList.item[i].isClosedSpecified = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objNetSuiteSOItem = null;
            }
            return objReturn;
        }

        //public bool CloseNoShipment()
        //{
        //    WriteResponse objWriteResult = null;

        //    try
        //    {
        //        if (ImageSolutionsSalesOrder == null) throw new Exception("ImageSolutionsSalesOrder cannot be null");
        //        if (NetSuiteSalesOrder == null) throw new Exception("Sales order record does not exists in NetSuite");
        //        if (NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._closed)
        //        {
        //            if (NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._pendingFulfillment) throw new Exception("Sales Order status must be partially fulfilled in order to cancel/close");

        //            ValidateOrderStatusBeforeClose();

        //            switch (ImageSolutionsSalesOrder.Status)
        //            {
        //                case ImageSolutions.SalesOrder.SalesOrder.enumStatus.canceled:
        //                    if (ImageSolutionsSalesOrder.Fulfillments != null && ImageSolutionsSalesOrder.Fulfillments.Count > 0) throw new Exception("Canceled order should not have fulfillments");
        //                    if (ImageSolutionsSalesOrder.Invoice != null) throw new Exception("Canceled order should not have invoice");
        //                    break;
        //                case ImageSolutions.SalesOrder.SalesOrder.enumStatus.closed:
        //                    if (ImageSolutionsSalesOrder.Fulfillments != null && ImageSolutionsSalesOrder.Fulfillments.Count > 0) throw new Exception("Closed order should not have fulfillments");
        //                    if (ImageSolutionsSalesOrder.Invoice == null) throw new Exception("Closed order should have an invoice");
        //                    if (ImageSolutionsSalesOrder.CreditMemos == null || ImageSolutionsSalesOrder.CreditMemos.Count == 0) throw new Exception("Closed order should have credit memos");
        //                    if (ImageSolutionsSalesOrder.Invoice.Total != ImageSolutionsSalesOrder.CreditMemos.Sum(m => m.Total)) throw new Exception("Closed order's invoice total amount should match with total credit memo amount");
        //                    break;
        //                default:
        //                    throw new Exception("You can only close sales orders with status = Canceled or Closed");
        //            }

        //            objWriteResult = Service.update(CloseNetSuiteSalesOrderNoShipment());

        //            if (objWriteResult.status.isSuccess != true)
        //            {
        //                throw new Exception("SalesOrder CloseNoShipment() : sales order can not be cancelled " + objWriteResult.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                mNetSuiteSalesOrder = null;
        //            }
        //        }
        //        ImageSolutionsSalesOrder.NetSuiteIsClosed = true;
        //        ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
        //        ImageSolutionsSalesOrder.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsSalesOrder.ErrorMessage = "SalesOrder.cs - Update() - " + ex.Message;
        //        ImageSolutionsSalesOrder.Update();
        //    }
        //    finally
        //    {
        //        objWriteResult = null;
        //    }
        //    return true;
        //}

        //private bool ValidateOrderStatusBeforeClose()
        //{
        //    TransactionSearchAdvanced objTransactionSearchAdvanced = null;
        //    SearchResult objSearchResult = null;
        //    Hashtable dicParam = null;
        //    bool blnFound = false;
        //    int intCount = 0;

        //    try
        //    {
        //        objTransactionSearchAdvanced = new TransactionSearchAdvanced();

        //        objTransactionSearchAdvanced.savedSearchId = "124";

        //        objTransactionSearchAdvanced.criteria = new TransactionSearch();
        //        objTransactionSearchAdvanced.criteria.basic = new TransactionSearchBasic();

        //        SearchMultiSelectField objInternalID = new SearchMultiSelectField();
        //        objInternalID.searchValue = new RecordRef[1];
        //        objInternalID.searchValue[0] = new RecordRef();
        //        objInternalID.searchValue[0].internalId = NetSuiteSalesOrder.internalId;
        //        objInternalID.searchValue[0].type = RecordType.salesOrder;
        //        objInternalID.searchValue[0].typeSpecified = true;
        //        objInternalID.@operator = SearchMultiSelectFieldOperator.anyOf;
        //        objInternalID.operatorSpecified = true;
        //        objTransactionSearchAdvanced.criteria.basic.internalId = objInternalID;

        //        //objTransactionSearchAdvanced.criteria.basic.customFieldList = new SearchCustomField[1];
        //        //SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
        //        //objAPIExternalID.scriptId = "custcol_api_external_id";
        //        //objAPIExternalID.searchValue = APIExternalID;
        //        //objAPIExternalID.@operator = SearchStringFieldOperator.@is;
        //        //objAPIExternalID.operatorSpecified = true;
        //        //objTransactionSearchAdvanced.criteria.basic.customFieldList[0] = objAPIExternalID;

        //        objSearchResult = Service.search(objTransactionSearchAdvanced);

        //        if (objSearchResult.status.isSuccess != true) throw new Exception(objSearchResult.status.statusDetail[0].message);

        //        foreach (SalesOrderItem objItem in NetSuiteSalesOrder.itemList.item)
        //        {
        //            blnFound = false;
        //            foreach (TransactionSearchRow objRow in objSearchResult.searchRowList)
        //            {
        //                if (objRow.basic.quantityPacked[0].searchValue != objRow.basic.quantityShipRecv[0].searchValue) throw new Exception("Unable to close this sales order, there are pending fulfillments (Packed) for TPIN " + objItem.item.name);
        //                if (objRow.basic.quantityPicked[0].searchValue != objRow.basic.quantityShipRecv[0].searchValue) throw new Exception("Unable to close this sales order, there are pending fulfillments (Picked) for TPIN " + objItem.item.name);
        //                blnFound = true;
        //            }
        //            if (!blnFound) throw new Exception("Unable to find sales order line item with TPIN:" + objItem.item.name + " in saved search ID:" + objTransactionSearchAdvanced.savedSearchId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objTransactionSearchAdvanced = null;
        //        objSearchResult = null;
        //        dicParam = null;
        //    }
        //    return true;
        //}

        //public bool ClosePartiallyFulfilled()
        //{
        //    WriteResponse objWriteResult = null;

        //    try
        //    {
        //        if (ImageSolutionsSalesOrder == null) throw new Exception("ImageSolutionsSalesOrder cannot be null");
        //        if (NetSuiteSalesOrder == null) throw new Exception("Sales order record does not exists in NetSuite");

        //        //Do not do pending billingpartiallyfulfilled, because we need to bill
        //        if (NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._partiallyFulfilled) throw new Exception("Sales Order status must be partially fulfilled in order to close");

        //        switch (ImageSolutionsSalesOrder.Status)
        //        {
        //            case Toolots.SalesOrder.SalesOrder.enumStatus.complete:
        //                if (ImageSolutionsSalesOrder.Fulfillments == null || ImageSolutionsSalesOrder.Fulfillments.Count == 0) throw new Exception("Partially Fulfilled SO, Closed order should have fulfillments");
        //                if (ImageSolutionsSalesOrder.Invoice == null) throw new Exception("Closed order should have an invoice");
        //                if (ImageSolutionsSalesOrder.CreditMemos == null || ImageSolutionsSalesOrder.CreditMemos.Count == 0) throw new Exception("Paritially Fulfilled SO, Closed order should have credit memos");
        //                //if (ImageSolutionsSalesOrder.Invoice.Total != ImageSolutionsSalesOrder.CreditMemos.Sum(m => m.Total)) throw new Exception("Closed order's invoice total amount should match with total credit memo amount");
        //                break;
        //            default:
        //                throw new Exception("You can only close partially fulfilled sales orders with status = Complete");
        //        }

        //        objWriteResult = Service.update(CloseNetSuiteSalesOrderPartiallyFulfilled());

        //        if (objWriteResult.status.isSuccess != true)
        //        {
        //            throw new Exception("SalesOrder ClosePartiallyFulfilled() : sales order can not be closed " + objWriteResult.status.statusDetail[0].message);
        //        }
        //        else
        //        {
        //            mNetSuiteSalesOrder = null;
        //        }
        //        ImageSolutionsSalesOrder.NetSuiteIsClosed = true;
        //        ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
        //        ImageSolutionsSalesOrder.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsSalesOrder.ErrorMessage = "SalesOrder.cs - Update() - " + ex.Message;
        //        ImageSolutionsSalesOrder.Update();
        //    }
        //    finally
        //    {
        //        objWriteResult = null;
        //    }
        //    return true;
        //}
        public bool UpdateSalesOrderBundleKits()
        {
            WriteResponse objWriteResult = null;

            try
            {
                if (NetSuiteSalesOrder == null) throw new Exception("Sales order record does not exists in NetSuite");

                objWriteResult = Service.update(UpdateNetSuiteSalesOrderBundleKits());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("SalesOrder Update : sales order can not be updated " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteSalesOrder = null;
                }
                //ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
                //ImageSolutionsSalesOrder.Update();
            }
            catch (Exception ex)
            {
                //ImageSolutionsSalesOrder.ErrorMessage = "SalesOrder.cs - Update() - " + ex.Message;
                //ImageSolutionsSalesOrder.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return true;

        }

        public NetSuiteLibrary.com.netsuite.webservices.SalesOrder UpdateNetSuiteSalesOrderBundleKits()
        {
            //first find item with discovery kit
            //then close the line
            //then add 10 items
            NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = NetSuiteSalesOrder;

            objReturn.totalCostEstimateSpecified = false;
            objReturn.estGrossProfitPercentSpecified = false;
            objReturn.exchangeRateSpecified = false;
            objReturn.discountTotalSpecified = false;
            objReturn.taxRateSpecified = false;
            objReturn.customFieldList = null;
            objReturn.taxRateSpecified = false;
            objReturn.subTotalSpecified = false;
            objReturn.estGrossProfitSpecified = false;
            objReturn.totalSpecified = false;
            objReturn.taxTotalSpecified = false;
            objReturn.altShippingCostSpecified = false;
            objReturn.lastModifiedDateSpecified = false;
            objReturn.createdDateSpecified = false;
            //objReturn.orderStatus = OrderOrderStatus._pendingFulfillment;
            //objReturn.orderStatusSpecifienSalesd = true;
            objReturn.billingAddress = null;
            objReturn.shippingAddress = null;

            //objReturn.ord

            //SP
            List<SalesOrderItem> objItems = new List<SalesOrderItem>();
            SalesOrderItem objSalesOrderItemComponent = null;
            int totalDiscoveryKits = 0;

            for (int i = 0; i < NetSuiteSalesOrder.itemList.item.Length; i++)
            {

                SalesOrderItem objSalesOrderItem = NetSuiteSalesOrder.itemList.item[i];
                objSalesOrderItem.costEstimateSpecified = false;
                objSalesOrderItem.quantityAvailableSpecified = false;
                objSalesOrderItem.quantityBackOrderedSpecified = false;
                objSalesOrderItem.quantityOnHandSpecified = false;
                objSalesOrderItem.quantityBilledSpecified = false;
                objSalesOrderItem.quantityCommittedSpecified = false;
                objSalesOrderItem.quantityFulfilledSpecified = false;
                objSalesOrderItem.quantityPackedSpecified = false;
                objSalesOrderItem.quantityPickedSpecified = false;
                objSalesOrderItem.poRateSpecified = false;
                objSalesOrderItem.price = new RecordRef();
                objSalesOrderItem.price.internalId = "-1"; //Custom Pricing

                double rate = Convert.ToDouble(objSalesOrderItem.rate);

                bool isDiscoveryKit = NetSuiteHelper.GetBoolCustomFieldValue(objSalesOrderItem, "custcol_is_discovery_kit");
                if (isDiscoveryKit)
                {
                    totalDiscoveryKits++;
                    CustomFieldRef[] objNewItemCustomRecords = new CustomFieldRef[objSalesOrderItem.customFieldList.Length + 1];

                    for (int j = 0; j < objSalesOrderItem.customFieldList.Length; j++)
                    {
                        objNewItemCustomRecords[j] = objSalesOrderItem.customFieldList[j];
                    }

                    objNewItemCustomRecords[objSalesOrderItem.customFieldList.Length] = NetSuiteHelper.CreateStringCustomField(string.Format("Discovery Kit #{0}", Convert.ToString(totalDiscoveryKits)), "custcol_picking_memo");

                    objSalesOrderItem.customFieldList = objNewItemCustomRecords;
                    //objSalesOrderItem.isClosed = true;
                    //objSalesOrderItem.isClosedSpecified = true;
                    objSalesOrderItem.rate = "0";
                    objSalesOrderItem.amount = 0;
                    objSalesOrderItem.amountSpecified = true;

                    objItems.Add(objSalesOrderItem);

                    string skuToAdd = string.Empty;
                    double qtyToAdd = 0;
                    double totalPrice = 0;
                    int numComponents = 0;

                    for (int j = 1; j < 11; j++)
                    {
                        int intCustomFieldLineIndex = 0;

                        skuToAdd = NetSuiteHelper.GetStringCustomFieldValue(objSalesOrderItem, string.Format("custcol_item_{0}", j));
                        qtyToAdd = Convert.ToDouble(NetSuiteHelper.GetStringCustomFieldValue(objSalesOrderItem, string.Format("custcol_quantity_{0}", j)));

                        if (!string.IsNullOrEmpty(skuToAdd))
                        {
                            NetSuiteLibrary.Item.ItemFilter objItemFilter = new Item.ItemFilter();
                            objItemFilter.ItemNumber = new SearchStringField();
                            objItemFilter.ItemNumber.@operator = NetSuiteLibrary.com.netsuite.webservices.SearchStringFieldOperator.@is;
                            objItemFilter.ItemNumber.operatorSpecified = true;
                            objItemFilter.ItemNumber.searchValue = skuToAdd;
                            Item.Item objItem = Item.Item.GetItem(Service, objItemFilter);

                            objSalesOrderItemComponent = new SalesOrderItem();
                            objSalesOrderItemComponent.item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objItem.NetSuiteAssemblyItem.internalId, RecordType.assemblyItem);
                            objSalesOrderItemComponent.quantity = Convert.ToDouble(qtyToAdd);
                            objSalesOrderItemComponent.quantitySpecified = true;
                            numComponents += Convert.ToInt32(qtyToAdd);
                            objSalesOrderItemComponent.price = new RecordRef();
                            objSalesOrderItemComponent.price.internalId = "-1"; //Custom Pricing
                            objSalesOrderItemComponent.rate = Convert.ToString(rate * (Convert.ToDouble(1) / 10));
                            objSalesOrderItemComponent.customFieldList = new CustomFieldRef[99];
                            objSalesOrderItemComponent.customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(string.Format("Discovery Kit #{0}", Convert.ToString(totalDiscoveryKits)), "custcol_picking_memo");

                            totalPrice += Math.Round(Convert.ToDouble(objSalesOrderItemComponent.rate) * objSalesOrderItemComponent.quantity, 2);

                            if (numComponents == 10 && totalPrice != rate)
                            {
                                objSalesOrderItemComponent.rate = Convert.ToString(Convert.ToDouble(objSalesOrderItemComponent.rate) - Math.Abs(totalPrice - rate));
                            }

                            objItems.Add(objSalesOrderItemComponent);
                        }
                    }

                    objSalesOrderItemComponent = new SalesOrderItem();
                    objSalesOrderItemComponent.item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("352", RecordType.inventoryItem); //Fragrnce Kit Box
                    objSalesOrderItemComponent.quantity = 1;
                    objSalesOrderItemComponent.quantitySpecified = true;
                    objSalesOrderItemComponent.customFieldList = new CustomFieldRef[99];
                    objSalesOrderItemComponent.customFieldList[0] = NetSuiteHelper.CreateStringCustomField(string.Format("Discovery Kit #{0}", Convert.ToString(totalDiscoveryKits)), "custcol_picking_memo");

                    objItems.Add(objSalesOrderItemComponent);

                    objSalesOrderItemComponent = new SalesOrderItem();
                    objSalesOrderItemComponent.item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("546", RecordType.inventoryItem); //Fragrance Kit Paper Test strip
                    objSalesOrderItemComponent.quantity = 20;
                    objSalesOrderItemComponent.quantitySpecified = true;
                    objSalesOrderItemComponent.customFieldList = new CustomFieldRef[99];
                    objSalesOrderItemComponent.customFieldList[0] = NetSuiteHelper.CreateStringCustomField(string.Format("Discovery Kit #{0}", Convert.ToString(totalDiscoveryKits)), "custcol_picking_memo");
                    objItems.Add(objSalesOrderItemComponent);
                }
                else if (objSalesOrderItem.item.internalId == "0")
                {}
                else
                {
                    objItems.Add(objSalesOrderItem);
                }
            }

            SalesOrderItemList objNewSalesOrderItemList = new SalesOrderItemList();
            objNewSalesOrderItemList.item = new SalesOrderItem[objItems.Count];


            for (int k = 0; k < objItems.Count; k++)
            {
                objNewSalesOrderItemList.item[k] = objItems[k];
            }

            objReturn.itemList = objNewSalesOrderItemList;

            CustomFieldRef[] objNewCustomRecords = new CustomFieldRef[99];

            //for (int j = 0; j < NetSuiteSalesOrder.customFieldList.Length; j++)
            //{
            //    objNewCustomRecords[j] = NetSuiteSalesOrder.customFieldList[j];
            //}

            objNewCustomRecords[0] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_approve_sales_order");
            objReturn.customFieldList = objNewCustomRecords;

            return objReturn;
        }
        public bool Delete()
        {
            RecordRef objSalesOrderRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                //if (ImageSolutionsSalesOrder == null) throw new Exception("ImageSolutionsSalesOrder cannot be null");

                if (NetSuiteSalesOrder != null)
                {
                    objSalesOrderRef = new RecordRef();
                    objSalesOrderRef.internalId = NetSuiteSalesOrder.internalId;
                    objSalesOrderRef.type = RecordType.salesOrder;
                    objSalesOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objSalesOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete sales order: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteSalesOrder = null;
                    }
                }

                //ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
                //ImageSolutionsSalesOrder.NetSuiteInternalID = string.Empty;
                //ImageSolutionsSalesOrder.NetSuiteDocumentNumber = string.Empty;
                //ImageSolutionsSalesOrder.Update();
            }
            catch (Exception ex)
            {
                throw ex;
                //if (ex.Message.Contains("Can not find SalesOrder with Internal ID"))
                //{
                //    ImageSolutionsSalesOrder.ErrorMessage = string.Empty;
                //    ImageSolutionsSalesOrder.NetSuiteInternalID = string.Empty;
                //    ImageSolutionsSalesOrder.NetSuiteDocumentNumber = string.Empty;
                //    ImageSolutionsSalesOrder.Update();
                //}
                //else
                //{
                //    ImageSolutionsSalesOrder.ErrorMessage = "SalesOrder.cs - Delete() - " + ex.Message;
                //    ImageSolutionsSalesOrder.Update();
                //}
            }
            finally
            {
                objSalesOrderRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        //public bool CreateFulfillmentPlans()
        //{
        //    Toolots.Fulfillment.FulfillmentPlanMaster objPlanMaster = null;
        //    Dictionary<string, List<Toolots.Fulfillment.FulfillmentPlanLine>> objFulfillmentPlans = null;
        //    Toolots.Fulfillment.FulfillmentPlanMasterFilter objFilter = null;
        //    Toolots.Fulfillment.FulfillmentPlanLine objFulfillmentPlanLine = null;

        //    try
        //    {
        //        if (ImageSolutionsSalesOrder == null) throw new Exception("Toolots SalesOrder is required");
        //        if (NetSuiteSalesOrder == null) throw new Exception("NetSuite Salesorder is required");
        //        if (NetSuiteSalesOrder.orderStatus == SalesOrderOrderStatus._closed) throw new Exception("NetSuite Order is closed");

        //        objFilter = new Toolots.Fulfillment.FulfillmentPlanMasterFilter();
        //        objFilter.SalesOrderID = ImageSolutionsSalesOrder.SalesOrderID;
        //        objPlanMaster = Toolots.Fulfillment.FulfillmentPlanMaster.GetFulfillmentPlanMaster(objFilter);
        //        if (objPlanMaster != null) throw new Exception("FulfillmentPlanMaster already exis");

        //        objFulfillmentPlans = new Dictionary<string, List<Toolots.Fulfillment.FulfillmentPlanLine>>();
        //        objFulfillmentPlans.Add("1", new List<Toolots.Fulfillment.FulfillmentPlanLine>());
        //        objFulfillmentPlans.Add("4", new List<Toolots.Fulfillment.FulfillmentPlanLine>());

        //        foreach (Toolots.SalesOrder.SalesOrderLine objSOLine in ImageSolutionsSalesOrder.SalesOrderLines)
        //        {
        //            bool blnFoundInNetsuite = false;
        //            for (var i = 0; i < NetSuiteSalesOrder.itemList.item.Length && !blnFoundInNetsuite; i++)
        //            {
        //                if (objSOLine.NetSuiteLineID == NetSuiteSalesOrder.itemList.item[i].line)
        //                {
        //                    blnFoundInNetsuite = true;
        //                    if (NetSuiteSalesOrder.itemList.item[i].location != null && !NetSuiteSalesOrder.itemList.item[i].isClosed && objSOLine.Quantity > objSOLine.QuantityPlanned)
        //                    {
        //                        switch (NetSuiteSalesOrder.itemList.item[i].location.internalId)
        //                        {
        //                            //direct location
        //                            case "1":
        //                                objFulfillmentPlanLine = new Toolots.Fulfillment.FulfillmentPlanLine();
        //                                objFulfillmentPlanLine.ItemID = objSOLine.ItemID;
        //                                objFulfillmentPlanLine.Quantity = objSOLine.Quantity - objSOLine.QuantityPlanned;
        //                                objFulfillmentPlanLine.SalesOrderLineID = objSOLine.SalesOrderLineID;
        //                                objFulfillmentPlans[NetSuiteDirectLocationID].Add(objFulfillmentPlanLine);
        //                                break;
        //                            //consign location
        //                            case "4":
        //                                objFulfillmentPlanLine = new Toolots.Fulfillment.FulfillmentPlanLine();
        //                                objFulfillmentPlanLine.ItemID = objSOLine.ItemID;
        //                                objFulfillmentPlanLine.Quantity = objSOLine.Quantity - objSOLine.QuantityPlanned;
        //                                objFulfillmentPlanLine.SalesOrderLineID = objSOLine.SalesOrderLineID;
        //                                objFulfillmentPlans[NetSuiteConsignedLocationID].Add(objFulfillmentPlanLine);
        //                                break;
        //                            //drop ship locaion, NetSuiteBoltolToolVendorInternalID
        //                            case "8":
        //                                if (NetSuiteSalesOrder.itemList.item[i].poVendor != null && NetSuiteSalesOrder.itemList.item[i].poVendor.internalId == NetSuiteBoltolToolVendorInternalID)
        //                                {
        //                                    objFulfillmentPlanLine = new Toolots.Fulfillment.FulfillmentPlanLine();
        //                                    objFulfillmentPlanLine.ItemID = objSOLine.ItemID;
        //                                    objFulfillmentPlanLine.Quantity = objSOLine.Quantity - objSOLine.QuantityPlanned;
        //                                    objFulfillmentPlanLine.SalesOrderLineID = objSOLine.SalesOrderLineID;
        //                                    objFulfillmentPlans[NetSuiteDirectLocationID].Add(objFulfillmentPlanLine);
        //                                }
        //                                break;
        //                            default:
        //                                throw new Exception("SalesOrderLineID " + objSOLine.SalesOrderLineID + " unhandled inventory location " + NetSuiteSalesOrder.itemList.item[i].location);
        //                        }
        //                    }
        //                }
        //            }
        //            if (!blnFoundInNetsuite) throw new Exception(objSOLine.SalesOrderLineID + " not created in NetSuite");
        //        }
        //        objPlanMaster = new Toolots.Fulfillment.FulfillmentPlanMaster();
        //        objPlanMaster.FulfillmentPlans = new List<Toolots.Fulfillment.FulfillmentPlan>();

        //        //foreach (var objFulfillmentPlan in objFulfillmentPlans)
        //        //{
        //        //    if (objFulfillmentPlan.Value != null && objFulfillmentPlan.Value.Count > 0)
        //        //    {
        //        //        Toolots.Fulfillment.FulfillmentPlan objNew = new Toolots.Fulfillment.FulfillmentPlan();
        //        //        objNew.SalesOrderID = ImageSolutionsSalesOrder.SalesOrderID;
        //        //        if (objFulfillmentPlan.Key == NetSuiteDirectLocationID)
        //        //            objNew.Location = Toolots.Fulfillment.FulfillmentPlan.enumLocation.Direct;
        //        //        else if (objFulfillmentPlan.Key == NetSuiteConsignedLocationID)
        //        //            objNew.Location = Toolots.Fulfillment.FulfillmentPlan.enumLocation.Consignment;

        //        //        objNew.FulfillmentPlanLines = objFulfillmentPlan.Value;
        //        //        objPlanMaster.FulfillmentPlans.Add(objNew);
        //        //    }
        //        //}
        //        if (objPlanMaster.FulfillmentPlans != null && objPlanMaster.FulfillmentPlans.Count > 0)
        //        {
        //            objPlanMaster.SalesOrderID = ImageSolutionsSalesOrder.SalesOrderID;
        //            objPlanMaster.Create();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsSalesOrder.ErrorMessage = "CreateFulfillmentPlans() - " + ex.Message;
        //    }
        //    finally
        //    {
        //        objFulfillmentPlans = null;
        //    }
        //    return true;
        //}

        public SalesOrder ObjectAlreadyExists()
        {
            List<SalesOrder> objSalesOrders = null;
            SalesOrderFilter objFilter = null;
            SalesOrder objReturn = null;

            try
            {
                objFilter = new SalesOrderFilter();
                //objFilter.APIExternalID = ImageSolutionsSalesOrder.SalesOrderID;
                objFilter.PONumber = ImageSolutionsSalesOrder.SalesOrderNumber;
                //objFilter.Class = GetNetSuiteClass();
                objSalesOrders = GetSalesOrders(Service, objFilter);
                if (objSalesOrders != null && objSalesOrders.Count() > 0)
                {
                    //if (objSalesOrders.Count > 1) throw new Exception("More than one sales orders with API External ID:" + ImageSolutionsSalesOrder.SalesOrderID + " found in Netsuite with InternalIDs " + string.Join(", ", objSalesOrders.Select(m => m.NetSuiteSalesOrder.internalId)));
                    //if (objSalesOrders.Count > 0) throw new Exception("More than one sales orders with same po number :" + ImageSolutionsSalesOrder.PONumber + "  and class found in Netsuite with InternalIDs " + string.Join(", ", objSalesOrders.Select(m => m.NetSuiteSalesOrder.internalId)));
                    //if (objSalesOrders.Count > 0) throw new Exception("PO# " + ImageSolutionsSalesOrder.PONumber + " already exist in Netsuite");
                    objReturn = objSalesOrders[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrders = null;
                objFilter = null;
            }
            return objReturn;
        }

        //public bool HasDuplicateTransaction()
        //{
        //    List<SalesOrder> objSalesOrders = null;
        //    SalesOrderFilter objFilter = null;

        //    try
        //    {
        //        objFilter = new SalesOrderFilter();
        //        objFilter.PONumber = ImageSolutionsSalesOrder.PONumber;
        //        if (!string.IsNullOrEmpty(ImageSolutionsSalesOrder.ShopifyAccountID))
        //        {
        //            objFilter.Class = GetNetSuiteClass();
        //        }
        //        //if (string.IsNullOrEmpty(objFilter.Class)) throw new Exception("Class is required");
        //        objSalesOrders = GetSalesOrders(Service, objFilter);
        //        if (objSalesOrders != null && objSalesOrders.Count() > 0) throw new Exception("PO# " + ImageSolutionsSalesOrder.PONumber + " already exist in Netsuite");

        //        //objFilter = new SalesOrderFilter();
        //        //objFilter.APIExternalID = ImageSolutionsSalesOrder.SalesOrderID;
        //        //objSalesOrders = GetSalesOrders(Service, objFilter);
        //        //if (objSalesOrders != null && objSalesOrders.Count() > 0) throw new Exception("API External ID:" + ImageSolutionsSalesOrder.SalesOrderID + " already exist in Netsuite");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objSalesOrders = null;
        //        objFilter = null;
        //    }
        //    return false;
        //}

        //public bool HasDuplicateTransaction()
        //{
        //    List<SalesOrder> objSalesOrders = null;
        //    SalesOrderFilter objFilter = null;

        //    try
        //    {
        //        objFilter = new SalesOrderFilter();
        //        objFilter.PONumber = ImageSolutionsSalesOrder.IncrementID;
        //        objSalesOrders = GetSalesOrders(Service, objFilter);
        //        if (objSalesOrders != null && objSalesOrders.Count() > 0) throw new Exception("PO# (Increment ID) " + ImageSolutionsSalesOrder.IncrementID + " already exist in Netsuite");

        //        objFilter = new SalesOrderFilter();
        //        objFilter.APIExternalID = ImageSolutionsSalesOrder.SalesOrderID;
        //        objSalesOrders = GetSalesOrders(Service, objFilter);
        //        if (objSalesOrders != null && objSalesOrders.Count() > 0) throw new Exception("API External ID:" + ImageSolutionsSalesOrder.SalesOrderID + " already exist in Netsuite");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objSalesOrders = null;
        //        objFilter = null;
        //    }
        //    return false;
        //}

        //private string GetNetSuiteClass()
        //{
        //    string strReturn = string.Empty;

        //    if (ImageSolutionsSalesOrder.AmazonMWSAccount != null)
        //    {
        //        switch (ImageSolutionsSalesOrder.AmazonMWSAccount.AmazonMarketPlace.LocationName)
        //        {
        //            case "US":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "33" : "5";
        //                break;
        //            case "CA":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "42" : "41";
        //                break;
        //            case "DE":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "40" : "39";
        //                break;
        //            case "ES":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "36" : "35";
        //                break;
        //            case "FR":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "48" : "47";
        //                break;
        //            case "IT":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "38" : "37";
        //                break;
        //            case "UK":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "34" : "31";
        //                break;
        //            case "MX":
        //                strReturn = !ImageSolutionsSalesOrder.IsFulfilledByMerchant ? "59" : "58";
        //                break;
        //            default:
        //                throw new Exception("Amazon Class is not mapped");
        //        }
        //    }
        //    else if (ImageSolutionsSalesOrder.ShopifyAccount != null)
        //    {
        //        switch (ImageSolutionsSalesOrder.ShopifyAccount.HostName)
        //        {
        //            case "gearit-com.myshopify.com":
        //                strReturn = "3";
        //                break;
        //            case "roocase.myshopify.com":
        //                strReturn = "2";
        //                break;
        //            case "solsticemedicine.myshopify.com":
        //                strReturn = "1";
        //                break;
        //            default:
        //                throw new Exception("Shopify Class is not mapped");
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Class is not mapped");
        //    }
        //    return strReturn;
        //}

        //private string GetNetSuiteLocation()
        //{
        //    string strReturn = string.Empty;

        //    if (ImageSolutionsSalesOrder.AmazonMWSAccount != null)
        //    {
        //        if (ImageSolutionsSalesOrder.IsFulfilledByMerchant)
        //            strReturn = ImageSolutionsSalesOrder.AmazonMWSAccount.MFNPrimaryLocation.NetSuiteInternalID;
        //        else
        //            strReturn = ImageSolutionsSalesOrder.AmazonMWSAccount.AFNPrimaryLocation.NetSuiteInternalID;
        //    }
        //    else if (ImageSolutionsSalesOrder.ShopifyAccount != null)
        //    {
        //        if (ImageSolutionsSalesOrder.IsFulfilledByMerchant)
        //            strReturn = ImageSolutionsSalesOrder.ShopifyAccount.MFNPrimaryLocation.NetSuiteInternalID;
        //        else
        //            strReturn = ImageSolutionsSalesOrder.ShopifyAccount.AFNPrimaryLocation.NetSuiteInternalID;
        //    }
        //    else
        //    {
        //        throw new Exception("location is not mapped");
        //    }
        //    return strReturn;
        //}

        public NetSuiteLibrary.com.netsuite.webservices.SalesOrder CreateNetSuiteSalesOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = null;
            int intCustomFieldIndex = 0;
            int intCustomFieldLineIndex = 0;
            int intNullFieldIndex = 0;
            NetSuiteLibrary.Item.Item objItem = null;
            string strItemType = string.Empty;
            string strItemVendorID = string.Empty;
            string str3PLVendorID = string.Empty;
            decimal dcmTotal = 0;
            int intItemIndex = 0;
            string strBinNumberInternalID = string.Empty;
            double dbLocationAvailablePrimary = 0;
            double dbLocationAvailableSecondary = 0;
            NetSuiteLibrary.Item.Item objKitMember = null;
            NetSuiteLibrary.Item.ItemFilter objKitMemberFilter = null;

            try
            {
                if (!string.IsNullOrEmpty(ImageSolutionsSalesOrder.NetSuiteInternalID)) throw new Exception("NetSuiteInternalID already exists");
                //if (string.IsNullOrEmpty(ImageSolutionsSalesOrder.Customer.NetSuiteInternalID)) throw new Exception("Customer.NetSuiteInternalID is missing");
                //if (ImageSolutionsSalesOrder.DeliveryAddress == null) throw new Exception("ShipToAddress is missing");
                //if (ImageSolutionsSalesOrder.TransactionDate == null) throw new Exception("TransactionDate is missing");
                if (ImageSolutionsSalesOrder.SalesOrderLines == null || ImageSolutionsSalesOrder.SalesOrderLines.Count == 0) throw new Exception("Sales order lines is missing");
                //if (HasDuplicateTransaction()) throw new Exception("Transaction already exists in NetSuite");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.SalesOrder();
                objReturn.internalId = ImageSolutionsSalesOrder.NetSuiteInternalID;
                //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteSalesOrderFormID, RecordType.salesOrder);

                string strParentInternalID = string.Empty;

                if (ImageSolutionsSalesOrder.WebsiteID == "109619374") strParentInternalID = "13530";

                Customer.CustomerFilter objCustomerFilter = new Customer.CustomerFilter();

                if(string.IsNullOrEmpty(ImageSolutionsSalesOrder.DeliveryAddress.Email))
                {
                    throw new Exception("Invalid Email");
                }

                objCustomerFilter.Email = ImageSolutionsSalesOrder.DeliveryAddress.Email;
                //objCustomerFilter.ParentInternalIDs = new List<string>();
                //objCustomerFilter.ParentInternalIDs.Add(strParentInternalID);

                //objCustomerFilter.CustomerInternalIDs = new List<string>();
                //objCustomerFilter.CustomerInternalIDs.Add("5385015");

                List<Customer.Customer> NSCustomers = Customer.Customer.GetCustomers(Service, objCustomerFilter);

                NetSuiteLibrary.Customer.Customer NSCustomer = null;

                foreach(NetSuiteLibrary.Customer.Customer _customer in NSCustomers)
                {
                    if(_customer.NetSuiteCustomer.parent.internalId == strParentInternalID)
                    {
                        NSCustomer = _customer;
                        break;
                    }
                }

                if(NSCustomer == null)
                {
                    throw new Exception("Unable to find Customer");
                }

               
                objReturn.entity = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NSCustomer.NetSuiteCustomer.internalId, RecordType.customer);
                objReturn.email = ImageSolutionsSalesOrder.DeliveryAddress.Email;

                objReturn.tranDate = Convert.ToDateTime(ImageSolutionsSalesOrder.TransactionDate);
                objReturn.tranDateSpecified = true;

                objReturn.orderStatus = SalesOrderOrderStatus._pendingFulfillment;
                objReturn.orderStatusSpecified = true;

                //objReturn.salesEffectiveDate = ImageSolutionsSalesOrder.OrderDate.Value;
                //objReturn.salesEffectiveDateSpecified = true;

                objReturn.location = new RecordRef();
                objReturn.location.internalId = "2";
                //objReturn.shipMethod = new RecordRef();
                //objReturn.shipMethod.internalId = ImageSolutionsSalesOrder.RetailerShippingMethod.NetSuiteInternalID;

                if (ImageSolutionsSalesOrder.DeliveryAddress == null) throw new Exception("Misisng ship to address");
                objReturn.shippingAddress = new Address();
                objReturn.shippingAddress.addressee = ImageSolutionsSalesOrder.DeliveryAddress.CompanyName;
                if (!string.IsNullOrEmpty(ImageSolutionsSalesOrder.DeliveryAddress.CompanyName))
                {
                    objReturn.shippingAddress.attention = ImageSolutionsSalesOrder.DeliveryAddress.CompanyName;
                }
                objReturn.shippingAddress.addr1 = ImageSolutionsSalesOrder.DeliveryAddress.AddressLine1;
                objReturn.shippingAddress.addr2 = ImageSolutionsSalesOrder.DeliveryAddress.AddressLine2;
                objReturn.shippingAddress.addr3 = ImageSolutionsSalesOrder.DeliveryAddress.AddressLine3;
                objReturn.shippingAddress.city = ImageSolutionsSalesOrder.DeliveryAddress.City;
                if (ImageSolutionsSalesOrder.DeliveryAddress.State.Length > 30)
                    objReturn.shippingAddress.state = ImageSolutionsSalesOrder.DeliveryAddress.State.Substring(0, 30);
                else
                    objReturn.shippingAddress.state = ImageSolutionsSalesOrder.DeliveryAddress.State;
                //if (!string.IsNullOrEmpty(ImageSolutionsSalesOrder.DeliveryAddress.Phone) && ImageSolutionsSalesOrder.DeliveryAddress.Phone.Length > 5) objReturn.shippingAddress.addrPhone = ImageSolutionsSalesOrder.DeliveryAddress.Phone;
                objReturn.shippingAddress.zip = ImageSolutionsSalesOrder.DeliveryAddress.PostalCode;
                objReturn.shippingAddress.country = base.GetCountry(ImageSolutionsSalesOrder.DeliveryAddress.CountryID);
                objReturn.shippingAddress.countrySpecified = true;
                //objReturn.shippingAddress.addrPhone = ImageSolutionsSalesOrder.DeliveryAddress.Phone;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsSalesOrder.SalesOrderID, "custbody_api_external_id");
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsSalesOrder.ShipToAddress.Phone, "custbody_po2go_orderid");
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsSalesOrder.Customer.EmailAddress, "custbody4");
                //objReturn.nullFieldList = new string[99];
                //objReturn.nullFieldList[intNullFieldIndex++] = "salesrep";

                //objReturn.salesRep = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("121", RecordType.employee);

                objReturn.itemList = new SalesOrderItemList();
                objReturn.itemList.item = new SalesOrderItem[ImageSolutionsSalesOrder.SalesOrderLines.Count() + 1];

                objReturn.shippingCost = ImageSolutionsSalesOrder.ShippingAmount == null ? 0 : Convert.ToDouble(ImageSolutionsSalesOrder.ShippingAmount);
                objReturn.shippingCostSpecified = true;

                objReturn.otherRefNum = ImageSolutionsSalesOrder.SalesOrderNumber;

                dcmTotal += Convert.ToDecimal(objReturn.shippingCost);

                for (int i = 0; i < ImageSolutionsSalesOrder.SalesOrderLines.Count; i++)
                {
                    intCustomFieldLineIndex = 0;
                    string strTaxRate = string.Empty;

                    //objItem = new Item.Item(ImageSolutionsSalesOrder.SalesOrderLines[i].ItemInternalID);

                    objReturn.itemList.item[intItemIndex] = new SalesOrderItem();
                    objReturn.itemList.item[intItemIndex].item = NetSuiteHelper.GetRecordRef(ImageSolutionsSalesOrder.SalesOrderLines[i].ItemInternalID, RecordType.inventoryItem );

                    objReturn.itemList.item[intItemIndex].price = new RecordRef();
                    objReturn.itemList.item[intItemIndex].price.internalId = "-1"; //Custom Pricing

                    objReturn.itemList.item[intItemIndex].quantity = ImageSolutionsSalesOrder.SalesOrderLines[i].Quantity;
                    objReturn.itemList.item[intItemIndex].quantitySpecified = true;

                    objReturn.itemList.item[intItemIndex].rate = ImageSolutionsSalesOrder.SalesOrderLines[i].UnitPrice.ToString();

                    objReturn.itemList.item[intItemIndex].location = new RecordRef();
                    objReturn.itemList.item[intItemIndex].location.internalId = "2";


                    objReturn.itemList.item[intItemIndex].customFieldList = new CustomFieldRef[99];
                    objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = 
                        NetSuiteHelper.CreateStringCustomField(ImageSolutionsSalesOrder.SalesOrderLines[i].Embellishment, "custcolitemembroidery");

                }

                //if (ImageSolutionsSalesOrder.DiscountAmount != null && ImageSolutionsSalesOrder.DiscountAmount.Value > 0)
                //{
                //    objReturn.discountItem = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(GeneralDiscountInternalID, RecordType.discountItem);
                //    objReturn.discountRate = Convert.ToString(ImageSolutionsSalesOrder.DiscountAmount * -1);
                //}

                if (ImageSolutionsSalesOrder.TaxAmount != null && ImageSolutionsSalesOrder.TaxAmount > 0)
                {
                    objReturn.itemList.item[intItemIndex] = new SalesOrderItem();
                    objReturn.itemList.item[intItemIndex].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(TaxItemInternalID, RecordType.serviceSaleItem);
                    objReturn.itemList.item[intItemIndex].price = new RecordRef();
                    objReturn.itemList.item[intItemIndex].price.internalId = "-1";
                    objReturn.itemList.item[intItemIndex].quantity = 1;
                    objReturn.itemList.item[intItemIndex].quantitySpecified = true;
                    objReturn.itemList.item[intItemIndex].rate = ImageSolutionsSalesOrder.TaxAmount.ToString();
                    objReturn.itemList.item[intItemIndex].location = NetSuiteHelper.GetRecordRef("1", RecordType.location);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        public bool CreateInventoryAdjustment(Item.Item Item, int Quantity, string LocationInternalID, string BinInternalID, string Memo)
        {
            WriteResponse objWriteResponse = null;

            try
            {
                //objWriteResponse = Service.add(CreateNetSuiteInventoryAdjustment(Item, Quantity, LocationInternalID, BinInternalID, Memo));
                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to create inventory adjustment: " + objWriteResponse.status.statusDetail[0].message);
                }
                Console.WriteLine("Inventory adjustment " + ImageSolutionsSalesOrder.SalesOrderID + " sync complete");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.InventoryAdjustment CreateNetSuiteInventoryAdjustment(Item.Item Item, int Quantity, string LocationInternalID, string BinInternalID, string Memo)
        //{
        //    NetSuiteLibrary.Item.Item objKitMember = null;
        //    NetSuiteLibrary.Item.ItemFilter objKitMemberFilter = null;
        //    NetSuiteLibrary.com.netsuite.webservices.InventoryAdjustment objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.InventoryAdjustment();

        //        objReturn.tranDate = ImageSolutionsSalesOrder.OrderDate.Value;
        //        objReturn.tranDateSpecified = true;
        //        objReturn.account = NetSuiteHelper.GetRecordRef("504", RecordType.account);
        //        objReturn.subsidiary = NetSuiteHelper.GetRecordRef(GoDirectSubsidiaryID, RecordType.subsidiary);
        //        objReturn.adjLocation = NetSuiteHelper.GetRecordRef(LocationInternalID, RecordType.location);
        //        objReturn.memo = Memo;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(Memo, "custbodygdi_internalnotes");

        //        objReturn.inventoryList = new InventoryAdjustmentInventoryList();

        //        if (Item.NetSuiteInventoryItem != null)
        //        {
        //            objReturn.inventoryList.inventory = new InventoryAdjustmentInventory[1];
        //            objReturn.inventoryList.inventory[0] = new InventoryAdjustmentInventory();
        //            objReturn.inventoryList.inventory[0].item = new RecordRef();
        //            objReturn.inventoryList.inventory[0].item.internalId = Item.ImageSolutionsItem.NetSuiteInternalID;
        //            objReturn.inventoryList.inventory[0].location = new RecordRef();
        //            objReturn.inventoryList.inventory[0].location.internalId = LocationInternalID;
        //            objReturn.inventoryList.inventory[0].adjustQtyBy = Quantity;
        //            objReturn.inventoryList.inventory[0].adjustQtyBySpecified = true;
        //            objReturn.inventoryList.inventory[0].inventoryDetail = new InventoryDetail();
        //            objReturn.inventoryList.inventory[0].inventoryDetail.inventoryAssignmentList = new InventoryAssignmentList();
        //            List<InventoryAssignment> objInventoryAssignment = new List<InventoryAssignment>();
        //            InventoryAssignment objNew = new InventoryAssignment();
        //            if (BinInternalID != string.Empty)
        //            {
        //                //AFN
        //                objNew.binNumber = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(BinInternalID, RecordType.bin);
        //            }
        //            else
        //            {
        //                //MFN
        //                if (Item.ImageSolutionsItem.Bin == null) throw new Exception("Preferred bin not setup in the database");
        //                objNew.binNumber = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(Item.ImageSolutionsItem.Bin.NetSuiteInternalID, RecordType.bin);
        //            }
        //            objNew.quantity = Quantity;
        //            objNew.quantitySpecified = true;
        //            objInventoryAssignment.Add(objNew);
        //            objReturn.inventoryList.inventory[0].inventoryDetail.inventoryAssignmentList.inventoryAssignment = objInventoryAssignment.ToArray();
        //        }
        //        else if (Item.NetSuiteKitItem != null)
        //        {
        //            objReturn.inventoryList.inventory = new InventoryAdjustmentInventory[Item.NetSuiteKitItem.memberList.itemMember.Count()];
        //            int intIndex = 0;
        //            foreach (ItemMember objIKitMemberItem in Item.NetSuiteKitItem.memberList.itemMember)
        //            {
        //                objKitMemberFilter = new Item.ItemFilter();
        //                objKitMemberFilter.ItemInternalIDs = new List<string>();
        //                objKitMemberFilter.ItemInternalIDs.Add(objIKitMemberItem.item.internalId);
        //                objKitMember = NetSuiteLibrary.Item.Item.GetItem(objKitMemberFilter);
        //                if (objKitMember == null) throw new Exception("Unable to find KitMemberItem");
        //                if (objKitMember.NetSuiteInventoryItem == null) throw new Exception("KitMemberItem must be inventory item");
        //                if (objKitMember.ImageSolutionsItem == null) throw new Exception("Kit child item not setup in the database");

        //                objReturn.inventoryList.inventory[intIndex] = new InventoryAdjustmentInventory();
        //                objReturn.inventoryList.inventory[intIndex].item = new RecordRef();
        //                objReturn.inventoryList.inventory[intIndex].item.internalId = objKitMember.NetSuiteInventoryItem.internalId; ;
        //                objReturn.inventoryList.inventory[intIndex].location = new RecordRef();
        //                objReturn.inventoryList.inventory[intIndex].location.internalId = LocationInternalID;
        //                objReturn.inventoryList.inventory[intIndex].adjustQtyBy = objIKitMemberItem.quantity * Quantity;
        //                objReturn.inventoryList.inventory[intIndex].adjustQtyBySpecified = true;
        //                objReturn.inventoryList.inventory[intIndex].inventoryDetail = new InventoryDetail();
        //                objReturn.inventoryList.inventory[intIndex].inventoryDetail.inventoryAssignmentList = new InventoryAssignmentList();
        //                List<InventoryAssignment> objInventoryAssignment = new List<InventoryAssignment>();
        //                InventoryAssignment objNew = new InventoryAssignment();
        //                if (BinInternalID != string.Empty)
        //                {
        //                    //AFN
        //                    objNew.binNumber = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(BinInternalID, RecordType.bin);
        //                }
        //                else
        //                {
        //                    //MFN
        //                    if (objKitMember.ImageSolutionsItem.Bin == null) throw new Exception("Preferred bin not setup in the database");
        //                    objNew.binNumber = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(objKitMember.ImageSolutionsItem.Bin.NetSuiteInternalID, RecordType.bin);
        //                }
        //                objNew.quantity = objIKitMemberItem.quantity * Quantity;
        //                objNew.quantitySpecified = true;
        //                objInventoryAssignment.Add(objNew);
        //                objReturn.inventoryList.inventory[intIndex].inventoryDetail.inventoryAssignmentList.inventoryAssignment = objInventoryAssignment.ToArray();

        //                intIndex++;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //use to find out replace all saleslines or not when update salesorder to netsuite
        //private bool isAllSalesLineNew()
        //{
        //    return !(SalesOrderLines != null && SalesOrderLines.Exists(sl => sl.LineID != null && sl.LineID > 0));
        //}

        //public bool IsRequireFulfillment()
        //{
        //    List<SalesOrderLine> Fulfillablelines = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Status) && OrderStatus == null) throw new Exception("Status and OrderStatus are not defined");
        //        //Fulfillablelines = GetFulfillableLines();
        //        if ((OrderStatus != null && (OrderStatus == NetSuiteLibrary.com.netsuite.webservices.SalesOrderOrderStatus._partiallyFulfilled || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.SalesOrderOrderStatus._pendingBillingPartFulfilled
        //                || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.SalesOrderOrderStatus._pendingFulfillment)) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else if ((Status.ToLower().Contains("pending fulfillment") || Status.ToLower().Contains("partially fulfilled") || Status.Contains("待完成") || Status.Contains("部分完成")) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else
        //        UpdateSalesOrderBundleKits
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

        //public List<SalesOrderLine> GetFulfillableLines()
        //{
        //    List<SalesOrderLine> lstReturn = null;
        //    if (this.SalesOrderLines != null && this.SalesOrderLines.Count() > 0)
        //    {
        //        lstReturn = this.SalesOrderLines;
        //        //when there are other fulfillments exist for the same salesorder
        //        if (this.Fulfillments != null && this.Fulfillments.Count() > 0)
        //        {
        //            //salesorderlines check against all fulfillmentlines of all fulfillment
        //            foreach (Fulfillment.Fulfillment fulfillment in this.Fulfillments)
        //            {
        //                foreach (Fulfillment.FulfillmentLine fulfillmentLine in fulfillment.FulfillmentLines)
        //                {
        //                    if (lstReturn.Any(sl => sl.LineID == fulfillmentLine.OrderLine))
        //                        lstReturn.Where(sl => sl.LineID == fulfillmentLine.OrderLine).First().Qty -= fulfillmentLine.Quantity;
        //                }
        //            }
        //            lstReturn = lstReturn.Any(sl => sl.Qty > 0) ? lstReturn = lstReturn.Where(sl => sl.Qty > 0).ToList() : null;
        //        }
        //    }
        //    return lstReturn;
        //}
        //public List<Fulfillment.FulfillmentLine> GetAllFulfillmentLines()
        //{
        //    List<Fulfillment.FulfillmentLine> lstReturn = null;
        //    if (this.Fulfillments != null && this.Fulfillments.Count() > 0)
        //    {
        //        lstReturn = new List<Fulfillment.FulfillmentLine>();
        //        foreach (Fulfillment.Fulfillment Fulfillment in this.Fulfillments)
        //        {
        //            if (Fulfillment.FulfillmentLines != null && Fulfillment.FulfillmentLines.Count() > 0)
        //            {
        //                lstReturn = lstReturn.Concat(Fulfillment.FulfillmentLines).ToList();
        //            }
        //        }
        //    }
        //    return lstReturn;
        //}
        public List<SalesOrder> GetSalesOrders(SalesOrderFilter Filter)
        {
            return GetSalesOrders(Service, Filter);
        }

        public static SalesOrder GetSalesOrder(NetSuiteService Service, SalesOrderFilter Filter)
        {
            List<SalesOrder> objSalesOrders = null;
            SalesOrder objReturn = null;

            try
            {
                objSalesOrders = GetSalesOrders(Service, Filter);
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

        public static List<SalesOrder> GetSalesOrders(NetSuiteService Service, SalesOrderFilter Filter)
        {
            List<SalesOrder> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<SalesOrder>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetSalesOrder in objSearchResult.recordList)
                        {
                            if (objNetSalesOrder is NetSuiteLibrary.com.netsuite.webservices.SalesOrder)
                            {
                                objReturn.Add(new SalesOrder((NetSuiteLibrary.com.netsuite.webservices.SalesOrder)objNetSalesOrder));
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
        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, SalesOrderFilter Filter)
        {
            SearchResult objSearchResult = null;
            TransactionSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objTransacSearch = new TransactionSearch();
                objTransacSearch.basic = new TransactionSearchBasic();
                objTransacSearch.basic.customFieldList = new SearchCustomField[99];
                int intCutstomField = 0;

                if (Filter != null)
                {
                    if (Filter.CustomerInternalIDs != null)
                    {
                        objTransacSearch.customerJoin = new CustomerSearchBasic();
                        objTransacSearch.customerJoin.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.CustomerInternalIDs);
                    }

                    if (Filter.LastModified != null)
                    {
                        objTransacSearch.basic.lastModifiedDate = Filter.LastModified;
                    }

                    if (!string.IsNullOrEmpty(Filter.PONumber))
                    {
                        objTransacSearch.basic.otherRefNum = new SearchTextNumberField();
                        objTransacSearch.basic.otherRefNum.@operator = SearchTextNumberFieldOperator.equalTo;
                        objTransacSearch.basic.otherRefNum.operatorSpecified = true;
                        objTransacSearch.basic.otherRefNum.searchValue = Filter.PONumber;
                    }

                    if (Filter.TransactionDate != null)
                    {
                        objTransacSearch.basic.tranDate = Filter.TransactionDate;
                    }

                    if (Filter.FulfillmentIssueOccurredOn != null)
                    {
                        SearchDateCustomField objFulfillmentIssueOccurredOn = new SearchDateCustomField();
                        objFulfillmentIssueOccurredOn.scriptId = "custbody_fulfillment_issue_occurred_on";
                        objFulfillmentIssueOccurredOn.searchValue = Filter.FulfillmentIssueOccurredOn.searchValue;
                        objFulfillmentIssueOccurredOn.searchValueSpecified = Filter.FulfillmentIssueOccurredOn.searchValueSpecified;
                        objFulfillmentIssueOccurredOn.searchValue2 = Filter.FulfillmentIssueOccurredOn.searchValue2;
                        objFulfillmentIssueOccurredOn.searchValue2Specified = Filter.FulfillmentIssueOccurredOn.searchValue2Specified;
                        objFulfillmentIssueOccurredOn.@operator = Filter.FulfillmentIssueOccurredOn.@operator;
                        objFulfillmentIssueOccurredOn.operatorSpecified = true;
                        objTransacSearch.basic.customFieldList[intCutstomField] = objFulfillmentIssueOccurredOn;
                        intCutstomField++;
                    }

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custbody_api_external_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objTransacSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
                        intCutstomField++;

                    }

                    if (Filter.FulfillmentCompleted != null)
                    {
                        SearchBooleanCustomField objFulfillmentCompleted = new SearchBooleanCustomField();
                        objFulfillmentCompleted.scriptId = "custbody_fulfillment_completed";
                        objFulfillmentCompleted.searchValue = Filter.FulfillmentCompleted.Value;
                        objFulfillmentCompleted.searchValueSpecified = true;
                        objTransacSearch.basic.customFieldList[intCutstomField] = objFulfillmentCompleted;
                        intCutstomField++;
                    }

                    //if (!string.IsNullOrEmpty(Filter.PONumber))
                    //{
                    //    if (Filter.PONumberOperator == null) throw new Exception("PONumber operator must be specififed");
                    //    SearchTextNumberField objPONumber = new SearchTextNumberField();
                    //    objPONumber.@operator = Filter.PONumberOperator.Value;
                    //    objPONumber.operatorSpecified = true;
                    //    objPONumber.searchValue = Filter.PONumber;
                    //    objTransacSearch.basic.otherRefNum = objPONumber;
                    //}

                    if (Filter.InternalIDs != null)
                    {
                        objTransacSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.InternalIDs);
                        //objTransacSearch.basic.internalIdNumber = new SearchLongField();
                        //objTransacSearch.basic.internalIdNumber.@operator = SearchLongFieldOperator.equalTo;
                        //objTransacSearch.basic.internalIdNumber.operatorSpecified = true;
                        //objTransacSearch.basic.internalIdNumber.searchValue = Convert.ToInt64(Filter.InternalIDs[0]);
                        //objTransacSearch.basic.internalIdNumber.searchValueSpecified = true;

                    }

                    if (Filter.tranid != null)
                    {
                        objTransacSearch.basic.tranId = new SearchStringField();
                        objTransacSearch.basic.tranId.@operator = SearchStringFieldOperator.@is;
                        objTransacSearch.basic.tranId.operatorSpecified = true;
                        objTransacSearch.basic.tranId.searchValue = Filter.tranid;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_salesOrder" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find Sales Order - " + objSearchResult.status.statusDetail[0].message);
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
        //private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, SalesOrderFilter Filter)
        //{
        //    SearchResult objSearchResult = null;
        //    TransactionSearch objTransacSearch = null;
        //    SearchPreferences objSearchPreferences = null;

        //    try
        //    {
        //        objTransacSearch = new TransactionSearch();
        //        objTransacSearch.basic = new TransactionSearchBasic();
        //        objTransacSearch.basic.customFieldList = new SearchCustomField[99];
        //        int intCutstomField = 0;

        //        if (Filter != null)
        //        {
        //            if (Filter.CustomerInternalIDs != null)
        //            {
        //                objTransacSearch.customerJoin = new CustomerSearchBasic();
        //                objTransacSearch.customerJoin.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.CustomerInternalIDs);
        //            }

        //            if (!string.IsNullOrEmpty(Filter.APIExternalID))
        //            {
        //                objTransacSearch.basic.customFieldList = new SearchCustomField[1];

        //                SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
        //                objAPIExternalID.scriptId = "custbody_api_external_id";
        //                objAPIExternalID.searchValue = Filter.APIExternalID;
        //                objAPIExternalID.@operator = SearchStringFieldOperator.@is;
        //                objAPIExternalID.operatorSpecified = true;
        //                objTransacSearch.basic.customFieldList[intCutstomField] = objAPIExternalID;
        //                intCutstomField++;
        //            }

        //            if (Filter.ApproveSalesOrder != null)
        //            {
        //                objTransacSearch.basic.customFieldList = new SearchCustomField[1];

        //                SearchBooleanCustomField objApproveSalesOrder = new SearchBooleanCustomField();
        //                objApproveSalesOrder.scriptId = "custbody_approve_sales_order";
        //                objApproveSalesOrder.searchValue = (bool)Filter.ApproveSalesOrder;
        //                objApproveSalesOrder.searchValueSpecified = true;
        //                objTransacSearch.basic.customFieldList[intCutstomField] = objApproveSalesOrder;
        //                intCutstomField++;
        //            }

        //            if (!string.IsNullOrEmpty(Filter.Class))
        //            {
        //                SearchMultiSelectField objClass = new SearchMultiSelectField();
        //                objClass.searchValue = new RecordRef[1];
        //                objClass.searchValue[0] = new RecordRef();
        //                objClass.searchValue[0].internalId = Filter.Class;
        //                objClass.searchValue[0].type = RecordType.classification;
        //                objClass.searchValue[0].typeSpecified = true;
        //                objClass.@operator = SearchMultiSelectFieldOperator.anyOf;
        //                objClass.operatorSpecified = true;
        //                objTransacSearch.basic.@class = objClass;
        //            }

        //            if (Filter.LastModified != null)
        //            {
        //                objTransacSearch.basic.lastModifiedDate = Filter.LastModified;
        //            }

        //            if (!string.IsNullOrEmpty(Filter.PONumber))
        //            {
        //                objTransacSearch.basic.otherRefNum = new SearchTextNumberField();
        //                objTransacSearch.basic.otherRefNum.@operator = SearchTextNumberFieldOperator.equalTo;
        //                objTransacSearch.basic.otherRefNum.operatorSpecified = true;
        //                objTransacSearch.basic.otherRefNum.searchValue = Filter.PONumber;
        //            }

        //            if (Filter.TransactionDate != null)
        //            {
        //                objTransacSearch.basic.tranDate = Filter.TransactionDate;
        //            }

        //            if (Filter.FulfillmentIssueOccurredOn != null)
        //            {
        //                SearchDateCustomField objFulfillmentIssueOccurredOn = new SearchDateCustomField();
        //                objFulfillmentIssueOccurredOn.scriptId = "custbody_fulfillment_issue_occurred_on";
        //                objFulfillmentIssueOccurredOn.searchValue = Filter.FulfillmentIssueOccurredOn.searchValue;
        //                objFulfillmentIssueOccurredOn.searchValueSpecified = Filter.FulfillmentIssueOccurredOn.searchValueSpecified;
        //                objFulfillmentIssueOccurredOn.searchValue2 = Filter.FulfillmentIssueOccurredOn.searchValue2;
        //                objFulfillmentIssueOccurredOn.searchValue2Specified = Filter.FulfillmentIssueOccurredOn.searchValue2Specified;
        //                objFulfillmentIssueOccurredOn.@operator = Filter.FulfillmentIssueOccurredOn.@operator;
        //                objFulfillmentIssueOccurredOn.operatorSpecified = true;
        //                objTransacSearch.basic.customFieldList[intCutstomField] = objFulfillmentIssueOccurredOn;
        //                intCutstomField++;
        //            }

        //            if (Filter.Status != null)
        //            {
        //                objTransacSearch.basic.status = new SearchEnumMultiSelectField();
        //                objTransacSearch.basic.status.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
        //                objTransacSearch.basic.status.operatorSpecified = true;
        //                objTransacSearch.basic.status.searchValue = new string[Filter.Status.Count];
        //                for (int i = 0; i < Filter.Status.Count; i++)
        //                {
        //                    objTransacSearch.basic.status.searchValue[i] = Filter.Status[i].ToString();
        //                }
        //            }

        //            if (Filter.FulfillmentCompleted != null)
        //            {
        //                SearchBooleanCustomField objFulfillmentCompleted = new SearchBooleanCustomField();
        //                objFulfillmentCompleted.scriptId = "custbody_fulfillment_completed";
        //                objFulfillmentCompleted.searchValue = Filter.FulfillmentCompleted.Value;
        //                objFulfillmentCompleted.searchValueSpecified = true;
        //                objTransacSearch.basic.customFieldList[intCutstomField] = objFulfillmentCompleted;
        //                intCutstomField++;
        //            }

        //            //if (!string.IsNullOrEmpty(Filter.PONumber))
        //            //{
        //            //    if (Filter.PONumberOperator == null) throw new Exception("PONumber operator must be specififed");
        //            //    SearchTextNumberField objPONumber = new SearchTextNumberField();
        //            //    objPONumber.@operator = Filter.PONumberOperator.Value;
        //            //    objPONumber.operatorSpecified = true;
        //            //    objPONumber.searchValue = Filter.PONumber;
        //            //    objTransacSearch.basic.otherRefNum = objPONumber;
        //            //}

        //            if (Filter.InternalIDs != null)
        //            {
        //                objTransacSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.InternalIDs);
        //            }
        //        }

        //        objTransacSearch.basic.type = new SearchEnumMultiSelectField();
        //        objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
        //        objTransacSearch.basic.type.operatorSpecified = true;
        //        objTransacSearch.basic.type.searchValue = new string[] { "_salesOrder" };

        //        objSearchPreferences = new SearchPreferences();
        //        objSearchPreferences.bodyFieldsOnly = false;
        //        objSearchPreferences.pageSize = 10;
        //        objSearchPreferences.pageSizeSpecified = true;

        //        Service.searchPreferences = objSearchPreferences;
        //        objSearchResult = Service.search(objTransacSearch);

        //        if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find Sales Order - " + objSearchResult.status.statusDetail[0].message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objTransacSearch = null;
        //        objSearchPreferences = null;
        //    }
        //    return objSearchResult;
        //}
        public bool CloseByPO()
        {
            bool ret = true;
            WriteResponse objWriteResult = null;

            try
            {
                //if (ImageSolutionsTransferOrder == null) throw new Exception("ImageSolutionsPurchaseOrder cannot be null");
                if (NetSuiteSalesOrder == null) throw new Exception("Purchase order record does not exists in NetSuite");

                if (NetSuiteSalesOrder.status == "Fully Billed" || NetSuiteSalesOrder.status == "Closed" || NetSuiteSalesOrder.status == "Billed")
                {
                    Console.WriteLine(string.Format("Sales Order {0} already Shipped or closed", NetSuiteSalesOrder.tranId));
                    ret = false;

                    return ret;
                }

                if(NetSuiteSalesOrder.entity.name.Contains("SupplyLogic"))
                {
                    Console.WriteLine(string.Format("Sales Order {0} contains supply logic", NetSuiteSalesOrder.tranId));

                    ret = false;

                    return ret;
                }

                objWriteResult = Service.update(CloseNetSuiteSalesOrderByPurchaseOrder());

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("SalesOrder CloseByPO() : " + objWriteResult.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteSalesOrder = null;
                }
                //ImageSolutionsTransferOrder.IsNSUpdated = true;
                //ImageSolutionsTransferOrder.ErrorMessage = string.Empty;
                //ImageSolutionsTransferOrder.Update();
            }
            catch (Exception ex)
            {
                ret = false;
                //ImageSolutionsTransferOrder.ErrorMessage = "PurchaseOrder.cs - Close() - " + ex.Message;
                //ImageSolutionsTransferOrder.Update();
            }
            finally
            {
                objWriteResult = null;
            }
            return ret;
        }

        public NetSuiteLibrary.com.netsuite.webservices.SalesOrder CloseNetSuiteSalesOrderByPurchaseOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = null;
            int intCustomFieldIndex = 0;
            string strBinNumberInternalID = string.Empty;
            NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter NetSuitePurchaseOrderFilter = null;
            NetSuiteLibrary.PurchaseOrder.PurchaseOrder NetSuitePurchaseOrder = null;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.SalesOrder();
                objReturn.internalId = NetSuiteSalesOrder.internalId;
                objReturn.itemList = NetSuiteSalesOrder.itemList;

                foreach (NetSuiteLibrary.com.netsuite.webservices.SalesOrderItem objSalesOrderItem in objReturn.itemList.item)
                {
                    objSalesOrderItem.quantityAvailableSpecified = false;
                    objSalesOrderItem.quantityBilledSpecified = false;
                    objSalesOrderItem.quantityOnHandSpecified = false;
                    objSalesOrderItem.amountSpecified = false;
                    objSalesOrderItem.grossAmtSpecified = false;
                    objSalesOrderItem.poRateSpecified = false;

                    if (objSalesOrderItem.createdPo != null)
                    {
                        NetSuitePurchaseOrderFilter = new PurchaseOrder.PurchaseOrderFilter();

                        NetSuitePurchaseOrderFilter.InternalIDs = new List<string>();
                        NetSuitePurchaseOrderFilter.InternalIDs.Add(objSalesOrderItem.createdPo.internalId);

                        NetSuitePurchaseOrder = PurchaseOrder.PurchaseOrder.GetPurchaseOrder(Service, NetSuitePurchaseOrderFilter);

                        if (NetSuiteHelper.GetBoolCustomFieldValue(NetSuitePurchaseOrder.NetSuitePurchaseOrder, "custbody_omit_auto_close"))
                        {
                            throw new Exception("Omitting Auto Close");
                        }

                        com.netsuite.webservices.PurchaseOrderItem objCurrentPOItem = NetSuitePurchaseOrder.NetSuitePurchaseOrder.itemList.item.First(m => m.item.internalId == objSalesOrderItem.item.internalId);

                        if (objCurrentPOItem != null && !objSalesOrderItem.isClosed && objCurrentPOItem.isClosed && objSalesOrderItem.quantityFulfilled != objSalesOrderItem.quantity)
                        //if(NetSuitePurchaseOrder.NetSuitePurchaseOrder.status == "Closed")
                        {
                            objSalesOrderItem.isClosed = true;
                            objSalesOrderItem.isClosedSpecified = true;

                            //objSalesOrderItem.quantityAvailableSpecified = false;
                            //objSalesOrderItem.quantityBilledSpecified = false;
                            //objSalesOrderItem.quantityOnHandSpecified = false;
                            //objSalesOrderItem.amountSpecified = false;
                            //objSalesOrderItem.grossAmtSpecified = false;
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
            return objReturn;
        }

        public NetSuiteLibrary.com.netsuite.webservices.SalesOrder CloseNetSuiteSalesOrder()
        {
            NetSuiteLibrary.com.netsuite.webservices.SalesOrder objReturn = null;
            int intCustomFieldIndex = 0;
            string strBinNumberInternalID = string.Empty;
            NetSuiteLibrary.PurchaseOrder.PurchaseOrderFilter NetSuitePurchaseOrderFilter = null;
            NetSuiteLibrary.PurchaseOrder.PurchaseOrder NetSuitePurchaseOrder = null;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.SalesOrder();
                objReturn.internalId = NetSuiteSalesOrder.internalId;
                objReturn.itemList = NetSuiteSalesOrder.itemList;

                foreach (NetSuiteLibrary.com.netsuite.webservices.SalesOrderItem objSalesOrderItem in objReturn.itemList.item)
                {
                    objSalesOrderItem.isClosed = true;
                    objSalesOrderItem.isClosedSpecified = true;
                    objSalesOrderItem.quantityAvailableSpecified = false;
                    objSalesOrderItem.quantityBilledSpecified = false;
                    objSalesOrderItem.quantityOnHandSpecified = false;
                    objSalesOrderItem.amountSpecified = false;
                    objSalesOrderItem.grossAmtSpecified = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return objReturn;
        }
    }

    public enum NetSuiteTransactionStatus
    {
        _billCancelled,
        _billOpen,
        _billPaidInFull,
        _billPaymentOnlineBillPayPendingAccountingApproval,
        _billPaymentVoided,
        _billPendingApproval,
        _billRejected,
        _cashSaleDeposited,
        _cashSaleNotDeposited,
        _cashSaleUnapprovedPayment,
        _checkOnlineBillPayPendingAccountingApproval,
        _checkVoided,
        _commissionOverpaid,
        _commissionPaidInFull,
        _commissionPendingAccountingApproval,
        _commissionPendingPayment,
        _commissionRejectedByAccounting,
        _creditMemoFullyApplied,
        _creditMemoOpen,
        _customerDepositDeposited,
        _customerDepositFullyApplied,
        _customerDepositNotDeposited,
        _customerRefundVoided,
        _estimateClosed,
        _estimateExpired,
        _estimateOpen,
        _estimateProcessed,
        _estimateVoided,
        _expenseReportApprovedByAccounting,
        _expenseReportApprovedOverriddenByAccounting,
        _expenseReportInProgress,
        _expenseReportPaidInFull,
        _expenseReportPendingAccountingApproval,
        _expenseReportPendingSupervisorApproval,
        _expenseReportRejectedByAccounting,
        _expenseReportRejectedBySupervisor,
        _expenseReportRejectedOverriddenByAccounting,
        _invoiceOpen,
        _invoicePaidInFull,
        _invoicePendingApproval,
        _invoiceRejected,
        _itemFulfillmentPacked,
        _itemFulfillmentPicked,
        _itemFulfillmentShipped,
        _journalApprovedForPosting,
        _journalPendingApproval,
        _opportunityClosedLost,
        _opportunityClosedWon,
        _opportunityInProgress,
        _opportunityIssuedEstimate,
        _paycheckCommitted,
        _paycheckCreated,
        _paycheckError,
        _paycheckPendingCommitment,
        _paycheckPendingTaxCalculation,
        _paycheckPreview,
        _paycheckReversed,
        _paymentDeposited,
        _paymentNotDeposited,
        _paymentUnapprovedPayment,
        _payrollLiabilityCheckVoided,
        _purchaseOrderClosed,
        _purchaseOrderFullyBilled,
        _purchaseOrderPartiallyReceived,
        _purchaseOrderPendingBill,
        _purchaseOrderPendingBillingPartiallyReceived,
        _purchaseOrderPendingReceipt,
        _purchaseOrderPendingSupervisorApproval,
        _purchaseOrderRejectedBySupervisor,
        _requisitionCancelled,
        _requisitionClosed,
        _requisitionFullyOrdered,
        _requisitionFullyReceived,
        _requisitionPartiallyOrdered,
        _requisitionPartiallyReceived,
        _requisitionPendingApproval,
        _requisitionPendingOrder,
        _requisitionRejected,
        _returnAuthorizationCancelled,
        _returnAuthorizationClosed,
        _returnAuthorizationPartiallyReceived,
        _returnAuthorizationPendingApproval,
        _returnAuthorizationPendingReceipt,
        _returnAuthorizationPendingRefund,
        _returnAuthorizationPendingRefundPartiallyReceived,
        _returnAuthorizationRefunded,
        _salesOrderBilled,
        _salesOrderCancelled,
        _salesOrderClosed,
        _salesOrderPartiallyFulfilled,
        _salesOrderPendingApproval,
        _salesOrderPendingBilling,
        _salesOrderPendingBillingPartiallyFulfilled,
        _salesOrderPendingFulfillment,
        _salesTaxPaymentOnlineBillPayPendingAccountingApproval,
        _salesTaxPaymentVoided,
        _statementChargeOpen,
        _statementChargePaidInFull,
        _taxLiabilityChequeVoided,
        _tegataPayableEndorsed,
        _tegataPayableIssued,
        _tegataPayablePaid,
        _tegataReceivablesCollected,
        _tegataReceivablesDiscounted,
        _tegataReceivablesEndorsed,
        _tegataReceivablesHolding,
        _transferOrderClosed,
        _transferOrderPartiallyFulfilled,
        _transferOrderPendingApproval,
        _transferOrderPendingFulfillment,
        _transferOrderPendingReceipt,
        _transferOrderPendingReceiptPartiallyFulfilled,
        _transferOrderReceived,
        _transferOrderRejected,
        _vendorReturnAuthorizationCancelled,
        _vendorReturnAuthorizationClosed,
        _vendorReturnAuthorizationCredited,
        _vendorReturnAuthorizationPartiallyReturned,
        _vendorReturnAuthorizationPendingApproval,
        _vendorReturnAuthorizationPendingCredit,
        _vendorReturnAuthorizationPendingCreditPartiallyReturned,
        _vendorReturnAuthorizationPendingReturn,
        _workOrderBuilt,
        _workOrderCancelled,
        _workOrderClosed,
        _workOrderPartiallyBuilt,
        _workOrderPendingBuild,
        _workOrderPlanned
    }
}


