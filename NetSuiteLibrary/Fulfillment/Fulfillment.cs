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

namespace NetSuiteLibrary.Fulfillment
{
    public class Fulfillment : NetSuiteBase
    {
        private static string NetSuiteFulfillmentFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteFulfillmentFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteFulfillmentFormID"].ToString();
                else
                    return string.Empty;
            }
        }

        private ImageSolutions.Fulfillment.Fulfillment mImageSolutionsFulfillment = null;
        public ImageSolutions.Fulfillment.Fulfillment ImageSolutionsFulfillment
        {
            get
            {
                if (mImageSolutionsFulfillment == null && mNetSuiteFulfillment != null && !string.IsNullOrEmpty(mNetSuiteFulfillment.internalId))
                {
                    List<ImageSolutions.Fulfillment.Fulfillment> objFulfillments = null;
                    ImageSolutions.Fulfillment.FulfillmentFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Fulfillment.FulfillmentFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuiteFulfillment.internalId;
                        objFulfillments = ImageSolutions.Fulfillment.Fulfillment.GetFulfillments(objFilter);
                        if (objFulfillments != null && objFulfillments.Count > 0)
                        {
                            mImageSolutionsFulfillment = objFulfillments[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objFulfillments = null;
                    }
                }
                return mImageSolutionsFulfillment;
            }
            private set
            {
                mImageSolutionsFulfillment = value;
            }
        }

        //private SalesOrder.SalesOrder mSalesOrder = null;
        //public SalesOrder.SalesOrder SalesOrder
        //{
        //    get
        //    {
        //        if (mSalesOrder == null && ImageSolutionsFulfillment != null && ImageSolutionsFulfillment.SalesOrder != null)
        //        {
        //            mSalesOrder = new SalesOrder.SalesOrder(ImageSolutionsFulfillment.SalesOrder);
        //        }
        //        return mSalesOrder;
        //    }
        //    private set
        //    {
        //        mSalesOrder = value;
        //    }
        //}

        private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment mNetSuiteFulfillment = null;
        public NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment NetSuiteFulfillment
        {
            get
            {
                if (mNetSuiteFulfillment == null && mImageSolutionsFulfillment != null && !string.IsNullOrEmpty(mImageSolutionsFulfillment.InternalID))
                {
                    mNetSuiteFulfillment = LoadNetSuiteFulfillment(mImageSolutionsFulfillment.InternalID);
                }
                //if (mNetSuiteFulfillment == null && mShipStationShipment != null && !string.IsNullOrEmpty(mShipStationShipment.NetSuiteFulfillmentInternalID))
                //{
                //    mNetSuiteFulfillment = LoadNetSuiteFulfillment(mShipStationShipment.NetSuiteFulfillmentInternalID);
                //}
                return mNetSuiteFulfillment;
            }
            private set
            {
                mNetSuiteFulfillment = value;
            }
        }

        public Fulfillment(ImageSolutions.Fulfillment.Fulfillment ImageSolutionsFulfillment)
        {
            mImageSolutionsFulfillment = ImageSolutionsFulfillment;
        }

        public Fulfillment(NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment NetSuiteFulfillment)
        {
            mNetSuiteFulfillment = NetSuiteFulfillment;
        }

        private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment LoadNetSuiteFulfillment(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.itemFulfillment;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Fulfillment with Internal ID : " + NetSuiteInternalID);
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
            if (mImageSolutionsFulfillment != null)
                return false;//CreateFulfillment();

            else
                return false;
        }

        //private bool DeletePickedFulfillments(NetSuiteLibrary.SalesOrder.SalesOrder SalesOrder)
        //{
        //    List<NetSuiteLibrary.Fulfillment.Fulfillment> objFulfillmenets = null;
        //    NetSuiteLibrary.Fulfillment.FulfillmentFilter objFilter = null;

        //    try
        //    {
        //        objFilter = new FulfillmentFilter();
        //        objFilter.SalesOrderInternalID = SalesOrder.NetSuiteSalesOrder.internalId;
        //        objFilter.Status = NetSuiteLibrary.SalesOrder.NetSuiteTransactionStatus._itemFulfillmentPicked;
        //        objFulfillmenets = NetSuiteLibrary.Fulfillment.Fulfillment.GetFulfillments(Service, objFilter);
        //        if (objFulfillmenets != null && objFulfillmenets.Count > 0)
        //        {
        //            foreach (NetSuiteLibrary.Fulfillment.Fulfillment objFulfillment in objFulfillmenets)
        //            {
        //                objFulfillment.Delete();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objFulfillmenets = null;
        //    }
        //    return true;
        //}

        private bool CreateFulfillment()
        {
            WriteResponse objWriteResponse = null;
            Fulfillment objFulfillment = null;

            try
            {
                if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");
                //if (NetSuiteFulfillment != null) throw new Exception("Fulfillment record already exists in NetSuite");
                //if (string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrderID)) throw new Exception("SalesOrderID is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrder.NetSuiteInternalID)) throw new Exception("Sales order has not yet been created in NetSuite");
                if (ImageSolutionsFulfillment.FulfillmentLines == null || ImageSolutionsFulfillment.FulfillmentLines.Count() == 0) throw new Exception("Fulfillmentr lines is missing");
                //if (ImageSolutionsFulfillment.ShipmentTrackings == null || ImageSolutionsFulfillment.ShipmentTrackings.Count() == 0) throw new Exception("ShipmentTrackings is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsFulfillment.NetSuiteInternalID))
                //{
                //    foreach (ImageSolutions.Fulfillment.FulfillmentLine objFulfillmentLine in ImageSolutionsFulfillment.FulfillmentLines)
                //    {
                //        //salesorderline fulfilled qty (fulfillment created in netsuite)
                //        int intTotalFulfilledQuantity = objFulfillmentLine.SalesOrderLine.FulfillmentLines.FindAll(m => !string.IsNullOrEmpty(m.Fulfillment.NetSuiteInternalID)).Sum(n => n.Quantity);
                //        int intTotalQuantity = objFulfillmentLine.SalesOrderLine.Quantity;

                //        if (intTotalFulfilledQuantity >= intTotalQuantity)
                //        {
                //            throw new Exception("SalesOrderLineID: " + objFulfillmentLine.SalesOrderLine.SalesOrderLineID + " has already been fully fulfilled");
                //        }
                //    }
                //}

                objFulfillment = ObjectAlreadyExists();
                if (objFulfillment != null)
                {
                    //In case the previous update failed
                    if (string.IsNullOrEmpty(ImageSolutionsFulfillment.InternalID))
                    {
                        ImageSolutionsFulfillment.InternalID = objFulfillment.NetSuiteFulfillment.internalId;
                    }
                }
                else
                {
                    //objWriteResponse = Service.add(CreateNetSuiteFulfillment());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create fulfillment: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsFulfillment.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                    }
                }

                foreach (ImageSolutions.Fulfillment.FulfillmentLine objFulfillmentLine in ImageSolutionsFulfillment.FulfillmentLines)
                {
                    bool blnFound = false;
                    foreach (NetSuiteLibrary.com.netsuite.webservices.ItemFulfillmentItem objFulfillmentItem in NetSuiteFulfillment.itemList.item)
                    {
                        string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objFulfillmentItem, "custcol_api_external_id");
                        if (objFulfillmentLine.FulfillmentLineID == strAPIExternalID)
                        {
                            objFulfillmentLine.LineID = objFulfillmentItem.orderLine;
                            blnFound = true;
                            break;
                        }
                    }
                    //Cannot check this because item group components will not exist
                    //if (!blnFound) throw new Exception("SalesOrderLineID " + objFulfillmentLine.SalesOrderLine.SalesOrderLineID + " did not get created, not found in NetSuite fulfillment");
                }
                ImageSolutionsFulfillment.ErrorMessage = string.Empty;
                ImageSolutionsFulfillment.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsFulfillment.ErrorMessage = "Fulfillment.cs - Create() - " + ex.Message;
                ImageSolutionsFulfillment.Update();
            }
            finally
            {
                objWriteResponse = null;

            }
            return true;
        }

        //public bool Update()
        //{
        //    WriteResponse objWriteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");
        //        if (NetSuiteFulfillment == null) throw new Exception("Fulfillment record does not exists in NetSuite, the fulfillment could be cancelled"); //do not auto cancel for now, manual check
        //        //if (ImageSolutionsFulfillment.FulfillmentPackages == null || ImageSolutionsFulfillment.FulfillmentPackages.Count == 0) throw new Exception("FulfillmentPackages is required");

        //        if (NetSuiteFulfillment.shipStatus == ItemFulfillmentShipStatus._picked || NetSuiteFulfillment.shipStatus == ItemFulfillmentShipStatus._packed)
        //        {
        //            objWriteResponse = Service.update(UpdateNetSuiteFulfillment());
        //            if (objWriteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to create fulfillment: " + objWriteResponse.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                mNetSuiteFulfillment = null;
        //            }
        //        }

        //        if (NetSuiteFulfillment.shipStatus == ItemFulfillmentShipStatus._shipped)
        //        {
        //            ImageSolutionsFulfillment.IsPrinted = true;
        //            ImageSolutionsFulfillment.IsLabeled = true;
        //            ImageSolutionsFulfillment.IsShipped = true;
        //        }

        //        ImageSolutionsFulfillment.ErrorMessage = string.Empty;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsFulfillment.ErrorMessage = "Fulfillment.cs - Update() - " + ex.Message;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    finally
        //    {
        //        objWriteResponse = null;
        //    }
        //    return true;
        //}

        //public bool UpdatePackage()
        //{
        //    WriteResponse objWriteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");
        //        if (NetSuiteFulfillment == null) throw new Exception("Fulfillment record does not exists in NetSuite, the fulfillment could be cancelled"); //do not auto cancel for now, manual check
        //        if (ImageSolutionsFulfillment.FulfillmentPackages == null || ImageSolutionsFulfillment.FulfillmentPackages.Count == 0) throw new Exception("FulfillmentPackages is required");
        //        if (ImageSolutionsFulfillment.FulfillmentPackages.Count > 1) throw new Exception("There shouldn't be more than 1 FulfillmentPackage");

        //        objWriteResponse = Service.update(UpdateNetSuiteFulfillmentPackage());
        //        if (objWriteResponse.status.isSuccess != true)
        //        {
        //            throw new Exception("Unable to create fulfillment: " + objWriteResponse.status.statusDetail[0].message);
        //        }
        //        else
        //        {
        //            mNetSuiteFulfillment = null;
        //        }

        //        foreach (ImageSolutions.Fulfillment.FulfillmentPackage objFulfillmentPackage in ImageSolutionsFulfillment.FulfillmentPackages)
        //        {
        //            objFulfillmentPackage.IsInvoiceSynced = true;
        //            objFulfillmentPackage.Update();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsFulfillment.ErrorMessage = "Fulfillment.cs - Update() - " + ex.Message;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    finally
        //    {
        //        objWriteResponse = null;
        //    }
        //    return true;
        //}

        public bool UpdateTracking()
        {
            WriteResponse objWriteResponse = null;

            try
            {
                if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");
                if (NetSuiteFulfillment == null) throw new Exception("Fulfillment record does not exists in NetSuite, the fulfillment could be cancelled"); //do not auto cancel for now, manual check

                objWriteResponse = Service.update(UpdateNetSuiteFulfillmentSynced());
                if (objWriteResponse.status.isSuccess != true)
                {
                    throw new Exception("Unable to update fulfillment: " + objWriteResponse.status.statusDetail[0].message);
                }
                else
                {
                    mNetSuiteFulfillment = null;
                }

                ImageSolutionsFulfillment.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsFulfillment.ErrorMessage = "Fulfillment.cs - Update() - " + ex.Message;
                ImageSolutionsFulfillment.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment UpdateNetSuiteFulfillment()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;
        //    int intPackageCount = 0;
        //    decimal dcmTotalShippingRate = 0;

        //    try
        //    {
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        objReturn.internalId = ImageSolutionsFulfillment.NetSuiteInternalID;

        //        if (ImageSolutionsFulfillment.IsLabeled)
        //            objReturn.shipStatus = ItemFulfillmentShipStatus._shipped;
        //        else
        //            objReturn.shipStatus = ItemFulfillmentShipStatus._packed;
        //        objReturn.shipStatusSpecified = true;

        //        objReturn.generateIntegratedShipperLabel = false;
        //        objReturn.generateIntegratedShipperLabelSpecified = true;

        //        objReturn.packageList = new com.netsuite.webservices.ItemFulfillmentPackageList();
        //        objReturn.packageList.replaceAll = true;

        //        if (ImageSolutionsFulfillment.FulfillmentPackages != null)
        //        {
        //            objReturn.packageList.package = new com.netsuite.webservices.ItemFulfillmentPackage[ImageSolutionsFulfillment.FulfillmentPackages.Count];

        //            foreach (ImageSolutions.Fulfillment.FulfillmentPackage objFulfillmentPackage in ImageSolutionsFulfillment.FulfillmentPackages)
        //            {
        //                objReturn.packageList.package[intPackageCount] = new com.netsuite.webservices.ItemFulfillmentPackage();
        //                objReturn.packageList.package[intPackageCount].packageWeight = Convert.ToDouble(objFulfillmentPackage.GetWeight(ImageSolutions.Item.Item.enumItemWeightUnit._lb));
        //                objReturn.packageList.package[intPackageCount].packageWeightSpecified = true;
        //                objReturn.packageList.package[intPackageCount].packageTrackingNumber = objFulfillmentPackage.TrackingNumber;
        //                objReturn.packageList.package[intPackageCount].packageDescr = objFulfillmentPackage.ShippingService.ShippingDescription + (objFulfillmentPackage.ShippingRate == null ? string.Empty : string.Format("{0:c}", objFulfillmentPackage.ShippingRate.Value));

        //                if (objFulfillmentPackage.ShippingRate != null) dcmTotalShippingRate += objFulfillmentPackage.ShippingRate.Value;
        //                intPackageCount++;
        //            }

        //            objReturn.shippingCost = Convert.ToDouble(dcmTotalShippingRate);
        //            objReturn.shippingCostSpecified = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment UpdateNetSuiteFulfillmentPackage()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        objReturn.internalId = ImageSolutionsFulfillment.NetSuiteInternalID;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceNumber, "custbody_invoice_number");
        //        if (ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceShippingRate != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDoubleCustomField(Convert.ToDouble(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceShippingRate.Value), "custbody_invoiced_shipping_cost");
        //        if (ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceZoneCode != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceZoneCode.Value, "custbody_invoiced_shipping_zone");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment UpdateNetSuiteFulfillmentSynced()
        {
            NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
                objReturn.internalId = ImageSolutionsFulfillment.InternalID;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_is_fulfillment_exported");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //private List<Toolots.Fulfillment.FulfillmentInventoryDetail> GetInventoyDetails()
        //{
        //    TransactionSearchAdvanced objSearch = null;
        //    SearchResult objSearchResult = null;
        //    Toolots.Fulfillment.FulfillmentInventoryDetail objFulfillmentInventoryDetail = null;
        //    Toolots.Item.Item objItem = null;
        //    Toolots.Item.ItemFilter objItemFilter = null;
        //    Toolots.Bins.Bin objBin = null;
        //    Toolots.Bins.BinFilter objBinFilter = null;
        //    List<Toolots.Fulfillment.FulfillmentInventoryDetail> objReturn = null;

        //    try
        //    {
        //        objSearch = new TransactionSearchAdvanced();
        //        objSearch.savedSearchId = "300";
        //        objSearch.criteria = new TransactionSearch();
        //        objSearch.criteria.basic = new TransactionSearchBasic();
        //        objSearch.criteria.basic.internalId = new SearchMultiSelectField();
        //        objSearch.criteria.basic.internalId.operatorSpecified = true;
        //        objSearch.criteria.basic.internalId.@operator = SearchMultiSelectFieldOperator.anyOf;
        //        objSearch.criteria.basic.internalId.searchValue = new RecordRef[1];
        //        objSearch.criteria.basic.internalId.searchValue[0] = NetSuiteHelper.GetRecordRef(mFulfillment.NetSuiteFulfillment.internalId, RecordType.itemFulfillment);

        //        objSearchResult = Service.search(objSearch);

        //        if (objSearchResult != null && objSearchResult.status != null && (!objSearchResult.status.isSuccessSpecified || !objSearchResult.status.isSuccess))
        //        {
        //            throw new Exception("Error getting inventory detail for item fulfillment: " + mFulfillment.NetSuiteFulfillment.tranId);
        //        }
        //        else
        //        {
        //            objReturn = new List<Toolots.Fulfillment.FulfillmentInventoryDetail>();

        //            foreach (TransactionSearchRow irow in objSearchResult.searchRowList)
        //            {
        //                if (irow.inventoryDetailJoin != null)
        //                {
        //                    objFulfillmentInventoryDetail = new Toolots.Fulfillment.FulfillmentInventoryDetail();

        //                    objItemFilter = new Toolots.Item.ItemFilter();
        //                    objItemFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
        //                    objItemFilter.NetSuiteInternalID.SearchString = irow.itemJoin.internalId[0].searchValue.internalId.ToString();
        //                    objItem = Toolots.Item.Item.GetItem(objItemFilter);
        //                    if (objItem == null) throw new Exception("Item InternalID: " + objItemFilter.NetSuiteInternalID + " is not found in database");

        //                    objBinFilter = new Toolots.Bins.BinFilter();
        //                    objBinFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
        //                    objBinFilter.NetSuiteInternalID.SearchString = irow.inventoryDetailJoin.binNumber[0].searchValue.internalId;
        //                    objBin = Toolots.Bins.Bin.GetBin(objBinFilter);
        //                    if (objBin == null) throw new Exception("Bin InternalID: " + objBinFilter.NetSuiteInternalID + " is not found in database");

        //                    objFulfillmentInventoryDetail.ItemID = objItem.ItemID;
        //                    objFulfillmentInventoryDetail.BinID = objBin.BinID;
        //                    objFulfillmentInventoryDetail.LocationID = objBin.Location.LocationID;
        //                    objFulfillmentInventoryDetail.Quantity = Convert.ToInt32(irow.inventoryDetailJoin.quantity[0].searchValue);
        //                    objReturn.Add(objFulfillmentInventoryDetail);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objSearch = null;
        //        objSearchResult = null;
        //    }
        //    return objReturn;
        //}


        //public bool UpdateShippingMemo()
        //{
        //    WriteResponse objWriteResponse = null;

        //    try
        //    {
        //        if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");
        //        if (NetSuiteFulfillment == null) throw new Exception("Fulfillment record does not exists in NetSuite, the fulfillment could be cancelled"); //do not auto cancel for now, manual check

        //        if (NetSuiteFulfillment.shipStatus == ItemFulfillmentShipStatus._picked || NetSuiteFulfillment.shipStatus == ItemFulfillmentShipStatus._packed)
        //        {
        //            objWriteResponse = Service.update(UpdateNetSuiteFulfillmentShippingMemo());
        //            if (objWriteResponse.status.isSuccess != true)
        //            {
        //                throw new Exception("Unable to create fulfillment: " + objWriteResponse.status.statusDetail[0].message);
        //            }
        //            else
        //            {
        //                mNetSuiteFulfillment = null;
        //            }
        //        }
        //        ImageSolutionsFulfillment.ErrorMessage = string.Empty;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsFulfillment.ErrorMessage = "Fulfillment.cs - Update() - " + ex.Message;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    finally
        //    {
        //        objWriteResponse = null;
        //    }
        //    return true;
        //}

        public bool Delete()
        {
            RecordRef objFulfillmentRef = null;
            WriteResponse objDeleteResponse = null;
            NetSuiteLibrary.SalesOrder.SalesOrder objNSSalesOrder = null;
            List<ImageSolutions.SalesOrder.SalesOrderLine> objSalesOrderLines = null;

            try
            {
                //if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");

                //objSalesOrderLines = new List<ImageSolutions.SalesOrder.SalesOrderLine>();
                //foreach (ImageSolutions.Fulfillment.FulfillmentLine objFulfillmentLine in ImageSolutionsFulfillment.FulfillmentLines)
                //{
                //    objSalesOrderLines.Add(objFulfillmentLine.SalesOrderLine);
                //}

                if (NetSuiteFulfillment != null)
                {
                    objFulfillmentRef = new RecordRef();
                    objFulfillmentRef.internalId = NetSuiteFulfillment.internalId;
                    objFulfillmentRef.type = RecordType.itemFulfillment;
                    objFulfillmentRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objFulfillmentRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete fulfillment: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteFulfillment = null;
                    }
                }

                if (mImageSolutionsFulfillment != null)
                {
                    mImageSolutionsFulfillment.InternalID = "0";
                    //mImageSolutionsFulfillment.NetSuiteDocumentNumber = "0";
                    mImageSolutionsFulfillment.Update();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find Fulfillment with Internal ID"))
                {
                    ImageSolutionsFulfillment.Delete();
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                objFulfillmentRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        public Fulfillment ObjectAlreadyExists()
        {
            List<Fulfillment> objFulfillments = null;
            FulfillmentFilter objFilter = null;
            Fulfillment objReturn = null;

            try
            {
                //This might grab commercehubFulfillment
                objFilter = new FulfillmentFilter();
                objFilter.APIExternalID = ImageSolutionsFulfillment.FulfillmentID;
                //objFilter.SalesOrderInternalID = ImageSolutionsFulfillment.SalesOrder.NetSuiteInternalID;
                objFulfillments = GetFulfillments(Service, objFilter);
                if (objFulfillments != null && objFulfillments.Count() > 0)
                {
                    if (objFulfillments.Count > 1) throw new Exception("More than one fulfillments with API External ID:" + ImageSolutionsFulfillment.FulfillmentID + " found in Netsuite with InternalIDs " + string.Join(", ", objFulfillments.Select(m => m.NetSuiteFulfillment.internalId)));
                    objReturn = objFulfillments[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFulfillments = null;
                objFilter = null;
            }
            return objReturn;
        }


        //public NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment CreateNetSuiteFulfillment()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;
        //    NetSuiteLibrary.SalesOrder.SalesOrder objSalesOrder = null;
        //    NetSuiteLibrary.SalesOrder.SalesOrderFilter objSalesOrderFilter = null;
        //    NetSuiteLibrary.Item.Item objItemGroupHeader = null;
        //    SalesOrderItem objItemGroupItem = null;
        //    ImageSolutions.Fulfillment.FulfillmentLine objItemGroupFulfillmentLine = null;

        //    bool blnIsGroupHeader = false;
        //    bool blnIsGroupFooter = false;
        //    bool blnIsGroupComponent = false;

        //    int intCustomFieldIndex = 0;
        //    int intCustomFieldLineIndex = 0;

        //    try
        //    {

        //        if (string.IsNullOrEmpty(ImageSolutionsFulfillment.PurchaseOrder.InternalID)) throw new Exception("PurchaseOrder.NetSuiteInternalID is missing");
        //        if (ImageSolutionsFulfillment.FulfillmentLines == null || ImageSolutionsFulfillment.FulfillmentLines.Count == 0) throw new Exception("Fulfillment lines is missing");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteFulfillmentFormID, RecordType.itemFulfillment);
        //        objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
        //        objReturn.createdFrom.internalId = ImageSolutionsFulfillment.PurchaseOrder.InternalID;
        //        objReturn.createdFrom.type = RecordType.purchaseOrder;
        //        objReturn.createdFrom.typeSpecified = true;

        //        if (ImageSolutionsFulfillment.ShipDate != null)
        //            objReturn.tranDate = ImageSolutionsFulfillment.ShipDate.Value;
        //        else
        //            objReturn.tranDate = DateTime.Now;

        //        objReturn.tranDateSpecified = true;

        //        //objReturn = NetSuiteHelper.GetRecordRef(NetSuiteProcomDepartmentID, RecordType.department);
        //        //objReturn.transactionShipAddress = null;
        //        //objReturn.shipAddress = null;

        //        objReturn.generateIntegratedShipperLabel = false;
        //        objReturn.generateIntegratedShipperLabelSpecified = true;

        //        //if (ImageSolutionsFulfillment.SalesOrder.IsFulfilledByMerchant)
        //        //    objReturn.shipStatus = ItemFulfillmentShipStatus._packed;
        //        //else
        //        //    objReturn.shipStatus = ItemFulfillmentShipStatus._shipped;

        //        objReturn.shipStatus = ItemFulfillmentShipStatus._shipped;
        //        objReturn.shipStatusSpecified = true;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentID, "custbody_api_external_id");
        //        //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.Carrier, "custbody_shiphawk_carrier");
        //        //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.ServiceCode, "custbody_shiphawk_servicename");

        //        objReturn.itemList = new ItemFulfillmentItemList();
        //        objReturn.itemList.item = new ItemFulfillmentItem[objSalesOrder.NetSuiteSalesOrder.itemList.item.Length];
        //        objReturn.itemList.replaceAll = false;

        //        for (int i = 0; i < objSalesOrder.NetSuiteSalesOrder.itemList.item.Length; i++)
        //        {                                
        //            if (!ImageSolutionsFulfillment.FulfillmentLines.Exists(m => m.SKU == ItemSKU))
        //            {
        //                if (blnIsGroupComponent)
        //                {
        //                    objReturn.itemList.item[i] = new com.netsuite.webservices.ItemFulfillmentItem();
        //                    objReturn.itemList.item[i].quantity = objSalesOrder.NetSuiteSalesOrder.itemList.item[i].quantity / objItemGroupItem.quantity * objItemGroupFulfillmentLine.Quantity;
        //                    objReturn.itemList.item[i].quantitySpecified = true;
        //                    objReturn.itemList.item[i].orderLine = objSalesOrder.NetSuiteSalesOrder.itemList.item[i].line;
        //                    objReturn.itemList.item[i].orderLineSpecified = true;
        //                    objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("1", RecordType.inventoryItem);
        //                }
        //                else
        //                {
        //                    if (objSalesOrder.NetSuiteSalesOrder.itemList.item[i].quantityFulfilled < objSalesOrder.NetSuiteSalesOrder.itemList.item[i].quantity)
        //                    {
        //                        objReturn.itemList.item[i] = new com.netsuite.webservices.ItemFulfillmentItem();
        //                        objReturn.itemList.item[i].quantity = 0;
        //                        objReturn.itemList.item[i].quantitySpecified = true;
        //                        objReturn.itemList.item[i].orderLine = objSalesOrder.NetSuiteSalesOrder.itemList.item[i].line;
        //                        objReturn.itemList.item[i].orderLineSpecified = true;
        //                        objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("1", RecordType.inventoryItem);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //need to find the SKU only as a whole and not the first because there are items that start witht he same SKU 

        //                objReturn.itemList.item[i] = new com.netsuite.webservices.ItemFulfillmentItem();
        //                objReturn.itemList.item[i].quantity = ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SKU == ItemSKU).Quantity;
        //                objReturn.itemList.item[i].quantitySpecified = true;
        //                objReturn.itemList.item[i].orderLine = objSalesOrder.NetSuiteSalesOrder.itemList.item[i].line;
        //                objReturn.itemList.item[i].orderLineSpecified = true;
        //                objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef("1", RecordType.location);
        //                objReturn.itemList.item[i].customFieldList = new CustomFieldRef[99];
        //                objReturn.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SKU == ItemSKU).FulfillmentLineID, "custcol_api_external_id");
        //            }



        //        }

        //        if (objSalesOrder.NetSuiteSalesOrder.shipMethod != null)
        //        {
        //            objReturn.shipMethod = new RecordRef();
        //            objReturn.shipMethod.internalId = objSalesOrder.NetSuiteSalesOrder.shipMethod.internalId;
        //        }

        //        //if (ImageSolutionsFulfillment.FulfillmentPackages != null && ImageSolutionsFulfillment.FulfillmentPackages.Count > 0)
        //        //{
        //        //    if (ImageSolutionsFulfillment.FulfillmentPackages.Count > 1) throw new Exception("We currently only allow one FulfillmentPackage per fulfillment");

        //        //    objReturn.shippingCost = ImageSolutionsFulfillment.FulfillmentPackages[0].ShippingRate == null ? 0 : Convert.ToDouble(ImageSolutionsFulfillment.FulfillmentPackages[0].ShippingRate.Value);
        //        //    objReturn.shippingCostSpecified = true;


        //        //    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(objReturn.shippingCost.ToString(), "custbody_shiphawk_shipping_cost");

        //        //    //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].TrackingNumber, "custbody_tracking_number");
        //        //    //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].PackageCount, "custbody_package_count");
        //        //    //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceNumber, "custbody_invoice_number");
        //        //    //if (ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceShippingRate != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDoubleCustomField(Convert.ToDouble(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceShippingRate.Value), "custbody_invoiced_shipping_cost");
        //        //    //if (ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceZoneCode != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceZoneCode.Value, "custbody_invoiced_shipping_zone");

        //        //    objReturn.packageList = new com.netsuite.webservices.ItemFulfillmentPackageList();
        //        //    objReturn.packageList.replaceAll = true;
        //        //    objReturn.packageList.package = new com.netsuite.webservices.ItemFulfillmentPackage[ImageSolutionsFulfillment.FulfillmentPackages.Count];

        //        //    for (int i = 0; i < ImageSolutionsFulfillment.FulfillmentPackages.Count; i++)
        //        //    {
        //        //        objReturn.packageList.package[i] = new com.netsuite.webservices.ItemFulfillmentPackage();

        //        //        if (ImageSolutionsFulfillment.FulfillmentPackages[i].TrackingNumber.Length > 64)
        //        //            objReturn.packageList.package[i].packageTrackingNumber = ImageSolutionsFulfillment.FulfillmentPackages[i].TrackingNumber.Substring(0, 63);
        //        //        else
        //        //            objReturn.packageList.package[i].packageTrackingNumber = ImageSolutionsFulfillment.FulfillmentPackages[i].TrackingNumber;

        //        //        objReturn.packageList.package[i].packageWeight = Convert.ToDouble(ImageSolutions.Item.Item.GetWeight(ImageSolutionsFulfillment.FulfillmentPackages[i].Weight.Value, ImageSolutions.Item.Item.enumItemWeightUnit._oz, ImageSolutions.Item.Item.enumItemWeightUnit._lb));
        //        //        objReturn.packageList.package[i].packageWeightSpecified = true;
        //        //    }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //public NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment CreateNetSuiteFulfillment_ShipStation()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;
        //    NetSuiteLibrary.SalesOrder.SalesOrder objSalesOrder = null;
        //    NetSuiteLibrary.SalesOrder.SalesOrderFilter objFilter = null;
        //    int intCustomFieldIndex = 0;
        //    int intCustomFieldLineIndex = 0;


        //    try
        //    {
        //        //if (ImageSolutionsFulfillment.SalesOrder == null) throw new Exception("SalesOrder is missing");
        //        //if (string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrder.NetSuiteInternalID)) throw new Exception("SalesOrder.NetSuiteInternalID is missing");
        //        //if (ImageSolutionsFulfillment.FulfillmentLines == null || ImageSolutionsFulfillment.FulfillmentLines.Count == 0) throw new Exception("Fulfillment lines is missing");
        //        //if (ImageSolutionsFulfillment.SalesOrder.RetailerShippingMethod == null) throw new Exception("RetailerShippingMethod is missing");

        //        objFilter = new SalesOrder.SalesOrderFilter();
        //        objFilter.PONumber = ShipStationShipment.OrderNumber;
        //        objSalesOrder = NetSuiteLibrary.SalesOrder.SalesOrder.GetSalesOrder(Service, objFilter);
        //        if (objSalesOrder == null || objSalesOrder.NetSuiteSalesOrder == null) throw new Exception("Unable to find NetSuite SalesOrder");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
        //        objReturn.createdFrom.internalId = objSalesOrder.NetSuiteSalesOrder.internalId;
        //        objReturn.createdFrom.type = RecordType.salesOrder;
        //        objReturn.createdFrom.typeSpecified = true;

        //        if (ShipStationShipment.ShipDate != null)
        //            objReturn.tranDate = ShipStationShipment.ShipDate;
        //        else
        //            objReturn.tranDate = ShipStationShipment.OrderDate;

        //        objReturn.tranDateSpecified = true;

        //        //objReturn = NetSuiteHelper.GetRecordRef(NetSuiteProcomDepartmentID, RecordType.department);
        //        //objReturn.transactionShipAddress = null;
        //        //objReturn.shipAddress = null;

        //        objReturn.generateIntegratedShipperLabel = false;
        //        objReturn.generateIntegratedShipperLabelSpecified = true;

        //        //if (ImageSolutionsFulfillment.SalesOrder.IsFulfilledByMerchant)
        //        //    objReturn.shipStatus = ItemFulfillmentShipStatus._packed;
        //        //else
        //        //    objReturn.shipStatus = ItemFulfillmentShipStatus._shipped;

        //        objReturn.shipStatus = ItemFulfillmentShipStatus._shipped;
        //        objReturn.shipStatusSpecified = true;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField("2", "custbody_thsi_delivery_method");

        //        if (!string.IsNullOrEmpty(ShipStationShipment.TrackingNumber))
        //        {
        //            objReturn.packageList = new com.netsuite.webservices.ItemFulfillmentPackageList();
        //            objReturn.packageList.replaceAll = true;
        //            objReturn.packageList.package = new com.netsuite.webservices.ItemFulfillmentPackage[1];
        //            objReturn.packageList.package[0] = new com.netsuite.webservices.ItemFulfillmentPackage();

        //            if (ShipStationShipment.TrackingNumber.Length > 64)
        //                objReturn.packageList.package[0].packageTrackingNumber = ShipStationShipment.TrackingNumber.Substring(0, 63);
        //            else
        //                objReturn.packageList.package[0].packageTrackingNumber = ShipStationShipment.TrackingNumber;

        //            objReturn.packageList.package[0].packageWeight = Convert.ToDouble(ImageSolutions.Item.Item.GetWeight(ShipStationShipment.WeightOz == 0 ? 1 : ShipStationShipment.WeightOz, ImageSolutions.Item.Item.enumItemWeightUnit._oz, ImageSolutions.Item.Item.enumItemWeightUnit._lb));
        //            objReturn.packageList.package[0].packageWeightSpecified = true;
        //        }


        //        //objReturn.itemList = new ItemFulfillmentItemList();
        //        //objReturn.itemList.item = new ItemFulfillmentItem[ImageSolutionsFulfillment.SalesOrder.SalesOrderLines.Count];
        //        //objReturn.itemList.replaceAll = false;

        //        //objSalesOrder = new SalesOrder.SalesOrder(ImageSolutionsFulfillment.SalesOrder);
        //        //for (int i = 0; i < ImageSolutionsFulfillment.SalesOrder.SalesOrderLines.Count; i++)
        //        //{
        //        //    if (!ImageSolutionsFulfillment.FulfillmentLines.Exists(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID))
        //        //    {
        //        //        var unfulfilledLocation = objSalesOrder.NetSuiteSalesOrder.itemList.item.First(s => s.line == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].NetSuiteLineID).location.internalId;
        //        //        if (string.IsNullOrEmpty(unfulfilledLocation)) throw new Exception("Netsuite Salesorder Line " + ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].NetSuiteLineID + " missing location in NetSuite");
        //        //        //This is to specify all SO line items that are not fulfilled by this fulfillment
        //        //        if (ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].QuantityPicked < ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Quantity)
        //        //        {
        //        //            objReturn.itemList.item[i] = new com.netsuite.webservices.ItemFulfillmentItem();
        //        //            if (ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.IsKit)
        //        //                objReturn.itemList.item[i].item = NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.NetSuiteInternalID, RecordType.kitItem);
        //        //            else
        //        //                objReturn.itemList.item[i].item = NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.NetSuiteInternalID, RecordType.inventoryItem);
        //        //            objReturn.itemList.item[i].quantity = 0;
        //        //            objReturn.itemList.item[i].quantitySpecified = true;
        //        //            objReturn.itemList.item[i].orderLine = ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].NetSuiteLineID.Value;
        //        //            objReturn.itemList.item[i].orderLineSpecified = true;
        //        //            objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(unfulfilledLocation, RecordType.inventoryItem);
        //        //        }
        //        //    }
        //        //    else
        //        //    {
        //        //        //var mOnHandDropshipLocationID = objSalesOrder.NetSuiteSalesOrder.itemList.item.First(s => s.line == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].NetSuiteLineID).location.internalId;
        //        //        //if (string.IsNullOrEmpty(mOnHandDropshipLocationID)) throw new Exception("Netsuite Salesorder Line " + ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].NetSuiteLineID + " missing location in NetSuite");
        //        //        //ValidateOnHandQuantityBySOLocation(ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID));
        //        //        objReturn.itemList.item[i] = new com.netsuite.webservices.ItemFulfillmentItem();
        //        //        if (ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.IsKit)
        //        //            objReturn.itemList.item[i].item = NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.NetSuiteInternalID, RecordType.kitItem);
        //        //        else
        //        //            objReturn.itemList.item[i].item = NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.NetSuiteInternalID, RecordType.inventoryItem);
        //        //        objReturn.itemList.item[i].quantity = ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID).Quantity;
        //        //        objReturn.itemList.item[i].quantitySpecified = true;
        //        //        objReturn.itemList.item[i].orderLine = ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID).SalesOrderLine.NetSuiteLineID.Value;
        //        //        objReturn.itemList.item[i].orderLineSpecified = true;
        //        //        //objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(mOnHandDropshipLocationID, RecordType.location);
        //        //        objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID).Location.NetSuiteInternalID, RecordType.location);

        //        //        objReturn.itemList.item[i].inventoryDetail = new InventoryDetail();
        //        //        objReturn.itemList.item[i].inventoryDetail.inventoryAssignmentList = new InventoryAssignmentList();
        //        //        List<InventoryAssignment> objInventoryAssignment = new List<InventoryAssignment>();
        //        //        InventoryAssignment objNew = new InventoryAssignment();
        //        //        objNew.binNumber = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID).Location.SellableBin.NetSuiteInternalID, RecordType.bin);
        //        //        objNew.quantity = objReturn.itemList.item[i].quantity;
        //        //        objNew.quantitySpecified = true;
        //        //        //objNew.inventoryStatus = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(, RecordType.inventorysta)
        //        //        objInventoryAssignment.Add(objNew);
        //        //        objReturn.itemList.item[i].inventoryDetail.inventoryAssignmentList.inventoryAssignment = objInventoryAssignment.ToArray();

        //        //        objReturn.itemList.item[i].customFieldList = new CustomFieldRef[99];
        //        //        objReturn.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentLines.Find(m => m.SalesOrderLineID == ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].SalesOrderLineID).FulfillmentLineID, "custcol_api_external_id");
        //        //    }
        //        //}

        //        //objReturn.shipMethod = new RecordRef();
        //        //objReturn.shipMethod.internalId = ImageSolutionsFulfillment.SalesOrder.RetailerShippingMethod.NetSuiteInternalID;

        //        //if (ImageSolutionsFulfillment.FulfillmentPackages != null && ImageSolutionsFulfillment.FulfillmentPackages.Count > 0)
        //        //{
        //        //    if (ImageSolutionsFulfillment.FulfillmentPackages.Count > 1) throw new Exception("We currently only allow one FulfillmentPackage per fulfillment");

        //        //    objReturn.shippingCost = ImageSolutionsFulfillment.FulfillmentPackages[0].ShippingRate == null ? 0 : Convert.ToDouble(ImageSolutionsFulfillment.FulfillmentPackages[0].ShippingRate.Value);
        //        //    objReturn.shippingCostSpecified = true;
        //        //    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].TrackingNumber, "custbody_tracking_number");
        //        //    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].PackageCount, "custbody_package_count");
        //        //    objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceNumber, "custbody_invoice_number");
        //        //    if (ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceShippingRate != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDoubleCustomField(Convert.ToDouble(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceShippingRate.Value), "custbody_invoiced_shipping_cost");
        //        //    if (ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceZoneCode != null) objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(ImageSolutionsFulfillment.FulfillmentPackages[0].InvoiceZoneCode.Value, "custbody_invoiced_shipping_zone");

        //        //    if (ImageSolutionsFulfillment.SalesOrder.IsFulfilledByMerchant)
        //        //    {
        //        //        objReturn.packageList = new com.netsuite.webservices.ItemFulfillmentPackageList();
        //        //        objReturn.packageList.replaceAll = true;
        //        //        objReturn.packageList.package = new com.netsuite.webservices.ItemFulfillmentPackage[ImageSolutionsFulfillment.FulfillmentPackages.Count];

        //        //        for (int i = 0; i < ImageSolutionsFulfillment.FulfillmentPackages.Count; i++)
        //        //        {
        //        //            objReturn.packageList.package[i] = new com.netsuite.webservices.ItemFulfillmentPackage();

        //        //            if (ImageSolutionsFulfillment.FulfillmentPackages[i].TrackingNumber.Length > 64)
        //        //                objReturn.packageList.package[i].packageTrackingNumber = ImageSolutionsFulfillment.FulfillmentPackages[i].TrackingNumber.Substring(0, 63);
        //        //            else
        //        //                objReturn.packageList.package[i].packageTrackingNumber = ImageSolutionsFulfillment.FulfillmentPackages[i].TrackingNumber;

        //        //            objReturn.packageList.package[i].packageWeight = Convert.ToDouble(ImageSolutions.Item.Item.GetWeight(ImageSolutionsFulfillment.FulfillmentPackages[i].Weight.Value, ImageSolutions.Item.Item.enumItemWeightUnit._oz, ImageSolutions.Item.Item.enumItemWeightUnit._lb));
        //        //            objReturn.packageList.package[i].packageWeightSpecified = true;
        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}



        //public bool UpdatePrintedBy()
        //{
        //    ImageSolutions.Employee.Employee objEmployee = null;
        //    ImageSolutions.Employee.EmployeeFilter objFilter = null;
        //    try
        //    {
        //        if (ImageSolutionsFulfillment == null) throw new Exception("ImageSolutionsFulfillment cannot be null");
        //        if (NetSuiteFulfillment == null) throw new Exception("Fulfillment record does not exists in NetSuite, might have been deleted"); //do not auto cancel for now, manual check

        //        if (string.IsNullOrEmpty(ImageSolutionsFulfillment.PrintedBy))
        //        {
        //            objFilter = new ImageSolutions.Employee.EmployeeFilter();
        //            objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
        //            objFilter.NetSuiteInternalID.SearchString = NetSuiteHelper.GetSelectCustomFieldValueID(NetSuiteFulfillment, "custbody_printed_by");
        //            objEmployee = ImageSolutions.Employee.Employee.GetEmployee(objFilter);
        //            if (objEmployee == null) throw new Exception("Employee is not found");

        //            ImageSolutionsFulfillment.PrintedBy = objEmployee.EmployeeID;
        //            ImageSolutionsFulfillment.PrintedOn = NetSuiteHelper.GetDateCustomFieldValue(NetSuiteFulfillment, "custbody_printed_on");
        //        }
        //        ImageSolutionsFulfillment.ErrorMessage = string.Empty;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsFulfillment.ErrorMessage = "Fulfillment.cs - Update() - " + ex.Message;
        //        ImageSolutionsFulfillment.Update();
        //    }
        //    finally
        //    {
        //        objEmployee = null;
        //        objFilter = null;
        //    }
        //    return true;
        //}

        //public NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment UpdateNetSuiteFulfillmentShippingMemo()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;

        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        if (ImageSolutionsFulfillment.SalesOrder == null) throw new Exception("SalesOrder is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrder.NetSuiteInternalID)) throw new Exception("SalesOrder.NetSuiteInternalID is missing");
        //        //if (ImageSolutionsFulfillment.ShippingMethod == null) throw new Exception("Shipping method is missing");
        //        if (ImageSolutionsFulfillment.TransactionDate == null) throw new Exception("TransactionDate is missing");


        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        objReturn.internalId = ImageSolutionsFulfillment.NetSuiteInternalID;
        //        objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteFulfillmentFormID, RecordType.itemFulfillment);

        //        string strShippingMemo = ImageSolutionsFulfillment.SalesOrder.ShippingMemo + (string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrder.ThirdPartyShipping) ? string.Empty : Environment.NewLine + ImageSolutionsFulfillment.SalesOrder.ThirdPartyShipping);
        //        if (ImageSolutionsFulfillment.SalesOrder.BoltonTransferOrder != null)
        //        {
        //            strShippingMemo += " " + ImageSolutionsFulfillment.SalesOrder.BoltonTransferOrder.ShippingMethod;
        //            if (!string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrder.BoltonTransferOrder.Comment)) strShippingMemo += " " + ImageSolutionsFulfillment.SalesOrder.BoltonTransferOrder.Comment;
        //        }

        //        //objReturn.shipMethod = new RecordRef();
        //        //objReturn.shipMethod.internalId = ImageSolutionsFulfillment.ShippingMethod.NetSuiteInternalID;

        //        //objReturn.transactionShipAddress = null;
        //        //objReturn.shipAddress = null;
        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(strShippingMemo, "custbody_shipping_memo");

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //    }
        //    return objReturn;
        //}

        //private ItemFulfillmentItem CreateNotFulfillItem(ImageSolutions.SalesOrder.SalesOrderLine SalesOrderLine)
        //{
        //    com.netsuite.webservices.ItemFulfillmentItem objReturn = new com.netsuite.webservices.ItemFulfillmentItem();
        //    objReturn.item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(SalesOrderLine.Item.NetSuiteInternalID, RecordType.inventoryItem);
        //    objReturn.quantity = 0;
        //    objReturn.quantitySpecified = true;
        //    objReturn.orderLine = SalesOrderLine.NetSuiteLineID.Value;
        //    objReturn.orderLineSpecified = true;
        //    //objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ImageSolutionsFulfillment.SalesOrder.SalesOrderLines[i].Item.IsConsignment ? NetSuiteConsignedLocationID : NetSuiteAssetLocationID, RecordType.inventoryItem);
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment UpdateNetSuiteFulfillment()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;
        //    int intCustomFieldIndex = 0;

        //    try
        //    {
        //        if (ImageSolutionsFulfillment.SalesOrder == null) throw new Exception("SalesOrder is missing");
        //        if (string.IsNullOrEmpty(ImageSolutionsFulfillment.SalesOrder.NetSuiteInternalID)) throw new Exception("SalesOrder.NetSuiteInternalID is missing");
        //        if (ImageSolutionsFulfillment.TransactionDate == null) throw new Exception("TransactionDate is missing");
        //        if (ImageSolutionsFulfillment.FulfillmentLines == null || ImageSolutionsFulfillment.FulfillmentLines.Count == 0) throw new Exception("Fulfillment lines is missing");
        //        if (ImageSolutionsFulfillment.ShipmentTrackings == null || ImageSolutionsFulfillment.ShipmentTrackings.Count == 0) throw new Exception("Missing tracking number");
        //        //if (ImageSolutionsFulfillment.PickedUpByEmployee == null) throw new Exception("Missing Picked Up By Employee");
        //        //if (ImageSolutionsFulfillment.PickedByEmployee == null) throw new Exception("Missing Picked By Employee");
        //        //if (ImageSolutionsFulfillment.PackedByEmployee == null) throw new Exception("Missing Packed By Employee");
        //        //if (ImageSolutionsFulfillment.ShippedByEmployee == null) throw new Exception("Missing Shipped By Employee");
        //        //if (ImageSolutionsFulfillment.PickedUpOn == null) throw new Exception("Missing Picked Up By Date");
        //        //if (ImageSolutionsFulfillment.PickedOn == null) throw new Exception("Missing Picked By Date");
        //        //if (ImageSolutionsFulfillment.PackedOn == null) throw new Exception("Missing Packed By Date");
        //        //if (ImageSolutionsFulfillment.ShippedOn == null) throw new Exception("Missing Shipped By Date");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        objReturn.internalId = ImageSolutionsFulfillment.NetSuiteInternalID;

        //        objReturn.shipStatus = ItemFulfillmentShipStatus._shipped;
        //        objReturn.shipStatusSpecified = true;

        //        objReturn.customFieldList = new CustomFieldRef[99];

        //        objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsFulfillment.FulfillmentID, "custbody_api_external_id");

        //        if (ImageSolutionsFulfillment.PickedUpByEmployee != null)
        //        {
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsFulfillment.PickedUpByEmployee.NetSuiteInternalID, "custbody_picked_up_by");
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsFulfillment.PickedUpOn.Value, "custbody_picked_up_on");
        //        }
        //        if (ImageSolutionsFulfillment.PickedByEmployee != null)
        //        {
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsFulfillment.PickedByEmployee.NetSuiteInternalID, "custbody_picked_by");
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsFulfillment.PickedOn.Value, "custbody_picked_on");
        //        }
        //        if (ImageSolutionsFulfillment.PackedByEmployee != null)
        //        {
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsFulfillment.PackedByEmployee.NetSuiteInternalID, "custbody_packed_by");
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsFulfillment.PackedOn.Value, "custbody_packed_on");
        //        }
        //        if (ImageSolutionsFulfillment.ShippedByEmployee != null)
        //        {
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateSelectCustomField(ImageSolutionsFulfillment.ShippedByEmployee.NetSuiteInternalID, "custbody_shipped_by");
        //            objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateDateCustomField(ImageSolutionsFulfillment.ShippedOn.Value, "custbody_shipped_on");
        //        }

        //        switch (ImageSolutionsFulfillment.Carrier.ToLower())
        //        {
        //            case "ups":
        //                objReturn.shipMethod = new RecordRef();
        //                objReturn.shipMethod.internalId = Convert.ToInt32(enumShipMethod.ParcelUPS).ToString();
        //                break;
        //            case "dhl":
        //                objReturn.shipMethod = new RecordRef();
        //                objReturn.shipMethod.internalId = Convert.ToInt32(enumShipMethod.ParcelDHL).ToString();
        //                break;
        //            case "usps":
        //                objReturn.shipMethod = new RecordRef();
        //                objReturn.shipMethod.internalId = Convert.ToInt32(enumShipMethod.ParcelUSPS).ToString();
        //                break;
        //            case "fedex":
        //                objReturn.shipMethod = new RecordRef();
        //                objReturn.shipMethod.internalId = Convert.ToInt32(enumShipMethod.ParcelFedEx).ToString();
        //                break;
        //            case "ltl":
        //                objReturn.shipMethod = new RecordRef();
        //                objReturn.shipMethod.internalId = Convert.ToInt32(enumShipMethod.LTL).ToString();
        //                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateLongCustomField(0, "custbody_bol_shipping_cost");
        //                break;
        //            case "custom":
        //                objReturn.shipMethod = new RecordRef();
        //                objReturn.shipMethod.internalId = Convert.ToInt32(enumShipMethod.Custom).ToString();
        //                break;
        //            default:
        //                throw new Exception("Unhandled shipment tracking carrier");
        //        }

        //        if (ImageSolutionsFulfillment.ShipmentTrackings != null && ImageSolutionsFulfillment.ShipmentTrackings.Count > 0)
        //        {
        //            objReturn.packageList = new com.netsuite.webservices.ItemFulfillmentPackageList();
        //            objReturn.packageList.replaceAll = true;
        //            objReturn.packageList.package = new com.netsuite.webservices.ItemFulfillmentPackage[ImageSolutionsFulfillment.ShipmentTrackings.Count];

        //            for (int i = 0; i < ImageSolutionsFulfillment.ShipmentTrackings.Count; i++)
        //            {
        //                objReturn.packageList.package[i] = new com.netsuite.webservices.ItemFulfillmentPackage();

        //                if (ImageSolutionsFulfillment.ShipmentTrackings[i].TrackingNumber.Length > 64)
        //                    objReturn.packageList.package[i].packageTrackingNumber = ImageSolutionsFulfillment.ShipmentTrackings[i].TrackingNumber.Substring(0, 63);
        //                else
        //                    objReturn.packageList.package[i].packageTrackingNumber = ImageSolutionsFulfillment.ShipmentTrackings[i].TrackingNumber;

        //                objReturn.packageList.package[i].packageWeight = 1;
        //                objReturn.packageList.package[i].packageWeightSpecified = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //    }
        //    return objReturn;
        //}

        //private NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment DeleteNetSuiteFulfillment()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment objReturn = null;

        //    try
        //    {
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment();
        //        objReturn.internalId = ImageSolutionsFulfillment.NetSuiteInternalID;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //private bool ValidateOnHandQuantity(ImageSolutions.Fulfillment.FulfillmentLine FulfillmentLine, string Location)
        //{
        //    Item.Item objItem = null;
        //    bool blnFound = false;
        //    bool blnReturn = false;

        //    try
        //    {
        //        objItem = new Item.Item(FulfillmentLine.Item);
        //        if (objItem.NetSuiteInventoryItem != null)
        //        {
        //            if (objItem.NetSuiteInventoryItem.locationsList.locations == null) throw new Exception("Item:" + objItem.ImageSolutionsItem.TPIN + " is missing location setup");
        //            string strItemType = NetSuiteHelper.GetSelectCustomFieldValueID(objItem.NetSuiteInventoryItem, "custitem_item_type");
        //            switch (strItemType)
        //            {
        //                case "1": //itemtype consignment
        //                    if (Location != NetSuiteConsignedLocationID) throw new Exception("Consignment item location must be ImageSolutions-Consignment");

        //                    //Create inventory adjustment if there is not enough inventory to fulfill
        //                    int intQtyByDate = SalesOrder.SalesOrder.GetInventoryByDate(objItem.NetSuiteInventoryItem.internalId, NetSuiteConsignedLocationID, FulfillmentLine.Fulfillment.TransactionDate);
        //                    if (intQtyByDate < FulfillmentLine.Quantity)
        //                    {
        //                        //throw new Exception(string.Format("Consignment - There is not enough on hand quantity({0}) on {1}", intQtyByDate, FulfillmentLine.Fulfillment.TransactionDate.ToShortDateString()));
        //                        NetSuiteLibrary.Inventory.InventoryAdjust.AdjustByVendor(FulfillmentLine, NetSuiteConsignedLocationID, (FulfillmentLine.Quantity - intQtyByDate));
        //                        objItem = new Item.Item(FulfillmentLine.Item);
        //                    }
        //                    break;
        //                case "3"://item type direct
        //                    if (Location != NetSuiteDirectLocationID) throw new Exception("Asset item location must be ImageSolutions-Direct");
        //                    foreach (com.netsuite.webservices.InventoryItemLocations objInventoryItemLocation in objItem.NetSuiteInventoryItem.locationsList.locations)
        //                    {
        //                        if (objInventoryItemLocation.locationId.internalId == Location)
        //                        {
        //                            //Use Quantity OnHand not available
        //                            if (objInventoryItemLocation.quantityOnHand < FulfillmentLine.Quantity)
        //                            {
        //                                NetSuiteLibrary.Inventory.InventoryAdjust.AdjustDirectItem(FulfillmentLine, FulfillmentLine.Quantity - Convert.ToInt32(objInventoryItemLocation.quantityOnHand));
        //                                objItem = new Item.Item(FulfillmentLine.Item);
        //                            }
        //                        }
        //                    }
        //                    break;
        //            }
        //            foreach (com.netsuite.webservices.InventoryItemLocations objInventoryItemLocation in objItem.NetSuiteInventoryItem.locationsList.locations)
        //            {
        //                if (objInventoryItemLocation.locationId.internalId == Location)
        //                {
        //                    //Use Quantity OnHand not available
        //                    if (objInventoryItemLocation.quantityOnHand < FulfillmentLine.Quantity)
        //                    {
        //                        throw new Exception(string.Format("There is not enough on hand quantity ({0}) to create this fulfillment location {1} for TPIN: {2}", objInventoryItemLocation.quantityOnHand, Location, objItem.ImageSolutionsItem.TPIN));
        //                    }
        //                    blnFound = true;
        //                    break;
        //                }
        //            }
        //        }
        //        else if (objItem.NetSuiteServiceItem != null)
        //        {
        //            blnReturn = true;
        //        }

        //        if (!blnFound) throw new Exception("No locatoin has enough on hand quantity to fulfill this item");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objItem = null;
        //    }

        //    return blnReturn;
        //}

        //private bool ValidateOnHandQuantity(ImageSolutions.Fulfillment.FulfillmentLine FulfillmentLine, SalesOrder.SalesOrder SalesOrder)
        //{
        //    Item.Item objItem = null;
        //    bool blnFound = false;
        //    bool blnReturn = false;

        //    try
        //    {
        //        objItem = new Item.Item(FulfillmentLine.Item);

        //        if (objItem.NetSuiteInventoryItem != null)
        //        {
        //            if (objItem.NetSuiteInventoryItem.locationsList.locations == null) throw new Exception("Item:" + objItem.ImageSolutionsItem.TPIN + " is missing location setup");

        //            if (SalesOrder.NetSuiteSalesOrder.itemList != null)
        //            {
        //                foreach (SalesOrderItem objSalesOrderItem in SalesOrder.NetSuiteSalesOrder.itemList.item)
        //                {
        //                    string strAPIExternalID = NetSuiteHelper.GetStringCustomFieldValue(objSalesOrderItem, "custcol_api_external_id");
        //                    //get salesorderline for fulfillment base on ImageSolutions id 
        //                    if (strAPIExternalID == FulfillmentLine.SalesOrderLineID)
        //                    {
        //                        foreach (com.netsuite.webservices.InventoryItemLocations objInventoryItemLocation in objItem.NetSuiteInventoryItem.locationsList.locations)
        //                        {
        //                            if (objInventoryItemLocation.locationId.internalId == objSalesOrderItem.location.internalId)
        //                            {
        //                                //Use Quantity OnHand not available
        //                                if (objInventoryItemLocation.quantityOnHand < FulfillmentLine.Quantity)
        //                                {
        //                                    throw new Exception("There is not enough on hand quantity (" + objInventoryItemLocation.quantityOnHand + ") to create this fulfillment");
        //                                }
        //                                blnFound = true;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else if (objItem.NetSuiteServiceItem != null)
        //        {
        //            blnReturn = true;
        //        }

        //        if (!blnFound) throw new Exception("No locatoin has enough on hand quantity to fulfill this item");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objItem = null;
        //    }

        //    return blnReturn;
        //}

        //use to find out replace all saleslines or not when update Fulfillment to netsuite
        //private bool isAllSalesLineNew()
        //{
        //    return !(FulfillmentLines != null && FulfillmentLines.Exists(sl => sl.LineID != null && sl.LineID > 0));
        //}

        //public bool IsRequireFulfillment()
        //{
        //    List<FulfillmentLine> Fulfillablelines = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Status) && OrderStatus == null) throw new Exception("Status and OrderStatus are not defined");
        //        //Fulfillablelines = GetFulfillableLines();
        //        if ((OrderStatus != null && (OrderStatus == NetSuiteLibrary.com.netsuite.webservices.FulfillmentOrderStatus._partiallyFulfilled || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.FulfillmentOrderStatus._pendingBillingPartFulfilled
        //                || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.FulfillmentOrderStatus._pendingFulfillment)) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else if ((Status.ToLower().Contains("pending fulfillment") || Status.ToLower().Contains("partially fulfilled") || Status.Contains("待完成") || Status.Contains("部分完成")) && Fulfillablelines != null && Fulfillablelines.Count > 0)
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

        //public List<FulfillmentLine> GetFulfillableLines()
        //{
        //    List<FulfillmentLine> lstReturn = null;
        //    if (this.FulfillmentLines != null && this.FulfillmentLines.Count() > 0)
        //    {
        //        lstReturn = this.FulfillmentLines;
        //        //when there are other fulfillments exist for the same Fulfillment
        //        if (this.Fulfillments != null && this.Fulfillments.Count() > 0)
        //        {
        //            //Fulfillmentlines check against all fulfillmentlines of all fulfillment
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

        public Fulfillment GetFulfillment(FulfillmentFilter Filter)
        {
            return GetFulfillment(Service, Filter);
        }

        public static Fulfillment GetFulfillment(NetSuiteService Service, FulfillmentFilter Filter)
        {
            List<Fulfillment> objFulfillments = null;
            Fulfillment objReturn = null;

            try
            {
                objFulfillments = GetFulfillments(Service, Filter);
                if (objFulfillments != null && objFulfillments.Count >= 1) objReturn = objFulfillments[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFulfillments = null;
            }
            return objReturn;
        }

        public List<Fulfillment> GetFulfillments(FulfillmentFilter Filter)
        {
            return GetFulfillments(Service, Filter);
        }

        public static List<Fulfillment> GetFulfillments(NetSuiteService Service, FulfillmentFilter Filter)
        {
            List<Fulfillment> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Fulfillment>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetFulfillment in objSearchResult.recordList)
                        {
                            if (objNetFulfillment is NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment)
                            {
                                objReturn.Add(new Fulfillment((NetSuiteLibrary.com.netsuite.webservices.ItemFulfillment)objNetFulfillment));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, FulfillmentFilter Filter)
        {
            SearchResult objSearchResult = null;
            TransactionSearch objTransacSearch = null;
            SearchPreferences objSearchPreferences = null;

            try
            {
                objTransacSearch = new TransactionSearch();
                objTransacSearch.basic = new TransactionSearchBasic();

                List<SearchCustomField> objSearchCustomFields = new List<SearchCustomField>();

                if (Filter != null)
                {
                    if (Filter.CustomerInternalIDs != null)
                    {
                        objTransacSearch.customerJoin = new CustomerSearchBasic();
                        objTransacSearch.customerJoin.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.CustomerInternalIDs);
                    }
                    //objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                    //objTransacSearch.createdFromJoin.type = 
                    //objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                    //objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                    //objTransacSearch.basic.type.operatorSpecified = true;
                    //objTransacSearch.basic.type.searchValue = new string[] { "_itemFulfillment" };
                    //transactiontype
                    if (Filter.InternalIDs != null)
                    {
                        objTransacSearch.basic.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.InternalIDs);
                    }

                    if (Filter.Status != null)
                    {
                        objTransacSearch.basic.status = new SearchEnumMultiSelectField();
                        objTransacSearch.basic.status.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                        objTransacSearch.basic.status.operatorSpecified = true;
                        objTransacSearch.basic.status.searchValue = new string[] { Filter.Status.ToString() };
                    }
                    //objTransacSearch.inventoryDetailJoin = new InventoryDetailSearchBasic();
                    //objTransacSearch.inventoryDetailJoin.binNumber = 

                    if (Filter.MarkedShipped != null)
                    {
                        SearchBooleanCustomField objMarkedShipped = new SearchBooleanCustomField();
                        objMarkedShipped.scriptId = "custbody_marked_shipped";
                        objMarkedShipped.searchValue = Filter.MarkedShipped.Value;
                        objMarkedShipped.searchValueSpecified = true;
                        objSearchCustomFields.Add(objMarkedShipped);

                    }

                    if (!string.IsNullOrEmpty(Filter.APIExternalID))
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custbody_is_stv_txn_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objSearchCustomFields.Add(objAPIExternalID);
                    }
                    else if (Filter.APIExternalIDEmpty != null)
                    {
                        SearchStringCustomField objAPIExternalID = new SearchStringCustomField();
                        objAPIExternalID.scriptId = "custbody_api_external_id";
                        objAPIExternalID.@operator = Filter.APIExternalIDEmpty.Value ? SearchStringFieldOperator.empty : SearchStringFieldOperator.notEmpty;
                        objAPIExternalID.operatorSpecified = true;
                        objSearchCustomFields.Add(objAPIExternalID);
                    }

                    if (!string.IsNullOrEmpty(Filter.FulfillmentPlanAPIExternalID))
                    {
                        SearchStringCustomField objFulfillmentPlanAPIExternalID = new SearchStringCustomField();
                        objFulfillmentPlanAPIExternalID.scriptId = "custbody_fulfill_plan_api_external_id";
                        objFulfillmentPlanAPIExternalID.searchValue = Filter.FulfillmentPlanAPIExternalID;
                        objFulfillmentPlanAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objFulfillmentPlanAPIExternalID.operatorSpecified = true;
                        objSearchCustomFields.Add(objFulfillmentPlanAPIExternalID);
                    }
                    else if (Filter.FulfillmnetPlanAPIExternalIDEmpty != null)
                    {
                        SearchStringCustomField objFulfillmnetPlanAPIExternalIDNotEmpty = new SearchStringCustomField();
                        objFulfillmnetPlanAPIExternalIDNotEmpty.scriptId = "custbody_fulfill_plan_api_external_id";
                        objFulfillmnetPlanAPIExternalIDNotEmpty.@operator = Filter.FulfillmnetPlanAPIExternalIDEmpty.Value ? SearchStringFieldOperator.empty : SearchStringFieldOperator.notEmpty;
                        objFulfillmnetPlanAPIExternalIDNotEmpty.operatorSpecified = true;
                        objSearchCustomFields.Add(objFulfillmnetPlanAPIExternalIDNotEmpty);
                    }

                    if (!string.IsNullOrEmpty(Filter.SalesOrderInternalID))
                    {
                        //SearchBooleanCustomField objIsConsignedField = new com.netsuite.webservices.SearchBooleanCustomField();
                        //objIsConsignedField.searchValue = true;
                        //objIsConsignedField.internalId = "";
                        //objIsConsignedField.scriptId = "";

                        SearchMultiSelectField objMultiSelectField = new SearchMultiSelectField();
                        objMultiSelectField.searchValue = new RecordRef[1];
                        objMultiSelectField.searchValue[0] = new RecordRef();
                        objMultiSelectField.searchValue[0].internalId = Filter.SalesOrderInternalID;
                        objMultiSelectField.@operator = SearchMultiSelectFieldOperator.anyOf;
                        objMultiSelectField.operatorSpecified = true;

                        objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                        objTransacSearch.createdFromJoin.internalId = objMultiSelectField;
                    }

                    if (!string.IsNullOrEmpty(Filter.TranID))
                    {
                        objTransacSearch.basic.tranId = new SearchStringField();
                        objTransacSearch.basic.tranId.@operator = SearchStringFieldOperator.@is;
                        objTransacSearch.basic.tranId.operatorSpecified = true;
                        objTransacSearch.basic.tranId.searchValue = Filter.TranID;
                    }
                }

                if (objSearchCustomFields != null && objSearchCustomFields.Count > 0)
                {
                    objTransacSearch.basic.customFieldList = objSearchCustomFields.ToArray();
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_itemFulfillment" };


                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true) throw new Exception("Cannot find Fulfillment - " + objSearchResult.status.statusDetail[0].message);
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

        //public static List<DeletedRecord> GetDeletedRecords(DateTime Date)
        //{
        //    List<DeletedRecord> objReturn = null;
        //    GetDeletedResult objDeletedResult = null;
        //    GetDeletedFilter objFilter = null;
        //    try
        //    {
        //        objReturn = new List<DeletedRecord>();

        //        objFilter = new GetDeletedFilter();
        //        objFilter.deletedDate = new SearchDateField();
        //        objFilter.deletedDate.@operator = SearchDateFieldOperator.after;
        //        objFilter.deletedDate.operatorSpecified = true;
        //        objFilter.deletedDate.searchValue = Date;
        //        objFilter.deletedDate.searchValueSpecified = true;

        //        objFilter.type = new SearchEnumMultiSelectField();
        //        objFilter.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
        //        objFilter.type.operatorSpecified = true;
        //        objFilter.type.searchValue = new string[] { NetSuiteHelper.DeletedRecordType.itemFulfillment.ToString() };
        //        objDeletedResult = Service.getDeleted(objFilter, 1);

        //        if (objDeletedResult != null && objDeletedResult.totalRecords > 0)
        //        {
        //            do
        //            {
        //                foreach (DeletedRecord objDeleteRecord in objDeletedResult.deletedRecordList)
        //                {
        //                    objReturn.Add(objDeleteRecord);
        //                }
        //                objDeletedResult = Service.getDeleted(objFilter, objDeletedResult.pageIndex + 1);
        //            }
        //            while (objDeletedResult.pageSizeSpecified = true && objDeletedResult.totalPages > 0 && objDeletedResult.totalPages >= objDeletedResult.pageIndex);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objFilter = null;
        //        objDeletedResult = null;
        //    }
        //    return objReturn;
        //}
    }
}


