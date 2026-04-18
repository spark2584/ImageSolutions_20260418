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

namespace NetSuiteLibrary.Bill
{
    public class Bill
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

        private static string NetSuiteBillFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteBillFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteBillFormID"].ToString();
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
                    return string.Empty;
            }
        }

        private static string ToolotsUSClass
        {
            get
            {
                if (ConfigurationManager.AppSettings["ToolotsUSClass"] != null)
                    return ConfigurationManager.AppSettings["ToolotsUSClass"].ToString();
                else
                    return string.Empty;
            }
        }

        //private static string NetSuiteDropshipLocationID
        //{
        //    get
        //    {
        //        if (ConfigurationManager.AppSettings["NetSuiteDropshipLocationID"] != null)
        //            return ConfigurationManager.AppSettings["NetSuiteDropshipLocationID"].ToString();
        //        else
        //            return string.Empty;
        //    }
        //}

        private Toolots.Bill.Bill mToolotsBill = null;
        public Toolots.Bill.Bill ToolotsBill
        {
            get
            {
                if (mToolotsBill == null && mNetSuiteBill != null && !string.IsNullOrEmpty(mNetSuiteBill.internalId))
                {
                    List<Toolots.Bill.Bill> objBills = null;
                    Toolots.Bill.BillFilter objFilter = null;

                    try
                    {
                        objFilter = new Toolots.Bill.BillFilter();
                        objFilter.NetSuiteInternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.NetSuiteInternalID.SearchString = mNetSuiteBill.internalId;
                        objBills = Toolots.Bill.Bill.GetBills(objFilter);
                        if (objBills != null && objBills.Count > 0)
                        {
                            mToolotsBill = objBills[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objBills = null;
                    }
                }
                return mToolotsBill;
            }
            private set
            {
                mToolotsBill = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.VendorBill mNetSuiteBill = null;
        public NetSuiteLibrary.com.netsuite.webservices.VendorBill NetSuiteBill
        {
            get
            {
                if (mNetSuiteBill == null && mToolotsBill != null && !string.IsNullOrEmpty(mToolotsBill.NetSuiteInternalID))
                {
                    mNetSuiteBill = LoadNetSuiteBill(mToolotsBill.NetSuiteInternalID);
                }
                return mNetSuiteBill;
            }
            private set
            {
                mNetSuiteBill = value;
            }
        }

        private Fulfillment.Fulfillment mFulfillment = null;
        public Fulfillment.Fulfillment NetSuiteFulfillment
        {
            get
            {
                if (mFulfillment == null && mToolotsBill != null && !string.IsNullOrEmpty(mToolotsBill.NetSuiteFulfillmentInternalID))
                {
                    mFulfillment = new Fulfillment.Fulfillment(new Toolots.Fulfillment.Fulfillment(mToolotsBill.NetSuiteFulfillmentInternalID));
                }
                return mFulfillment;
            }
            private set
            {
                mFulfillment = value;
            }
        }

        public Bill(Toolots.Bill.Bill ToolotsBill)
        {
            mToolotsBill = ToolotsBill;
        }

        public Bill(NetSuiteLibrary.com.netsuite.webservices.VendorBill NetSuiteBill)
        {
            mNetSuiteBill = NetSuiteBill;
        }

        private NetSuiteLibrary.com.netsuite.webservices.VendorBill LoadNetSuiteBill(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.VendorBill objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.vendorBill;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.VendorBill))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.VendorBill)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Bill with Internal ID : " + NetSuiteInternalID);
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
            Bill objBill = null;

            try
            {
                if (ToolotsBill == null) throw new Exception("ToolotsBill cannot be null");
                //if (NetSuiteBill != null) throw new Exception("Bill record already exists in NetSuite");
                if (ToolotsBill.BillLines == null || ToolotsBill.BillLines.Count() == 0) throw new Exception("Bill lines are missing");
                //foreach (Toolots.Bill.BillLine objBillLine in ToolotsBill.BillLines)
                //{
                //    //salesorderline fulfilled qty (Bill created in netsuite)
                //    int intTotalFulfilledQuantity = objBillLine.SalesOrderLine.BillLines.FindAll(m => !string.IsNullOrEmpty(m.Bill.NetSuiteInternalID)).Sum(n => n.Quantity);
                //    int intTotalQuantity = objBillLine.SalesOrderLine.Quantity;

                //    if (intTotalFulfilledQuantity >= intTotalQuantity)
                //    {
                //        throw new Exception("SalesOrderLineID: " + objBillLine.SalesOrderLine.SalesOrderLineID + " has already been fully fulfilled");
                //    }
                //}

                objBill = ObjectAlreadyExists();
                if (objBill != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    //UpdateFulfillment(objBill);
                    ToolotsBill.NetSuiteInternalID = objBill.NetSuiteBill.internalId;
                    ToolotsBill.NetSuiteDocumentNumber = NetSuiteBill.tranId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteBill());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Bill: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        //UpdateFulfillment(objBill);
                        ToolotsBill.NetSuiteInternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                        ToolotsBill.NetSuiteDocumentNumber = NetSuiteBill.tranId;
                    }
                }

                //Update BillLine LineID

                foreach (NetSuiteLibrary.com.netsuite.webservices.VendorBillItem objBillItem in NetSuiteBill.itemList.item)
                {
                    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objBillItem, "custcol_api_external_id");
                    if (ToolotsBill.BillLines.Exists(m => m.BillLineID == strAPIExternalID))
                    {
                        ToolotsBill.BillLines.Find(m => m.BillLineID == strAPIExternalID).NetSuiteLineID = objBillItem.line.ToString();
                    }
                    else
                    {
                        throw new Exception("BillLineID " + strAPIExternalID + " did not get created, not found in NetSuite Bill");
                    }
                }
                if (ToolotsBill.BillLines.Exists(m => string.IsNullOrEmpty(m.NetSuiteLineID))) throw new Exception("There is bill line that is missing NetSuiteLineID");

                ToolotsBill.ErrorMessage = string.Empty;
                ToolotsBill.Update();
            }
            catch (Exception ex)
            {
                ToolotsBill.ErrorMessage = "Bill.cs - Create() - " + ex.Message;
                ToolotsBill.Update();
            }
            finally
            {
                objWriteResponse = null;

            }
            return true;
        }

        public bool Update()
        {
            WriteResponse objWriteResponse = null;

            try
            {
                if (ToolotsBill == null) throw new Exception("ToolotsBill cannot be null");
                if (NetSuiteBill == null) throw new Exception("Bill record does not exists in NetSuite, the Bill could be cancelled"); //do not auto cancel for now, manual check

                //if (NetSuiteBill.shipStatus == VendorBillShipStatus._picked || NetSuiteBill.shipStatus == VendorBillShipStatus._packed)
                //{
                //    objWriteResponse = Service.update(UpdateNetSuiteBill());
                //    if (objWriteResponse.status.isSuccess != true)
                //    {
                //        throw new Exception("Unable to create Bill: " + objWriteResponse.status.statusDetail[0].message);
                //    }
                //    else
                //    {
                //        mNetSuiteBill = null;
                //    }
                //}

                //if (NetSuiteBill.shipStatus == VendorBillShipStatus._packed)
                //{
                //    ToolotsBill.IsPacked = true;
                //}
                //else if (NetSuiteBill.shipStatus == VendorBillShipStatus._shipped)
                //{
                //    ToolotsBill.IsPacked = true;
                //    ToolotsBill.IsShipped = true;
                //}

                ToolotsBill.ErrorMessage = string.Empty;
                ToolotsBill.Update();
            }
            catch (Exception ex)
            {
                ToolotsBill.ErrorMessage = "Bill.cs - Update() - " + ex.Message;
                ToolotsBill.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        public bool UpdateFulfillment(NetSuiteLibrary.Bill.Bill Bill)
        {
            //WriteResponse objWriteResponse = null;

            //try
            //{
            //    if (NetSuiteCustomer == null) throw new Exception("Customer record is not found");

            //    objWriteResponse = Service.update(UpdateNetsuiteFulfillment(Bill));
            //    if (objWriteResponse.status.isSuccess != true)
            //    {
            //        throw new Exception("Unable to update Customer: " + objWriteResponse.status.statusDetail[0].message);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    objWriteResponse = null;

            //}
            return true;
        }

        private com.netsuite.webservices.ItemFulfillment UpdateNetsuiteFulfillment(NetSuiteLibrary.Bill.Bill Bill)
        {
            com.netsuite.webservices.ItemFulfillment objReturn = null;

            //try
            //{
            //    objReturn = new com.netsuite.webservices.ItemFulfillment();
            //    objReturn.internalId = Bill.ToolotsBill.NetSuiteFulfillmentInternalID;


            //    objReturn.itemList = new com.netsuite.webservices.ItemFulfillmentItemList();
            //    objReturn.itemList.replaceAll = false;

            //    objReturn.itemList
            //    .packageList = new com.netsuite.webservices.ItemFulfillmentPackageList();
            //    //            objReturn.packageList.replaceAll = true;

            //    //            objReturn.packageList.package = new com.netsuite.webservices.ItemFulfillmentPackage[1];
            //    //            objReturn.packageList.package[0] = new com.netsuite.webservices.ItemFulfillmentPackage();
            //    //            objReturn.packageList.package[0].packageWeight = Convert.ToDouble(ToolotsFulfillment.FulfillmentLines[0].SalesOrderLine.Item.ItemWeight);
            //    //            objReturn.packageList.package[0].packageWeightSpecified = true;
            //    //            objReturn.packageList.package[0].packageTrackingNumber = ToolotsFulfillment.TrackingNumber;
            //    //            objReturn.packageList.package[0].packageDescr = ToolotsFulfillment.ShippingMethod.Description;


            //    objReturn.customFieldList = new CustomFieldRef[1];
            //    objReturn.customFieldList[0] = NetSuiteHelper.CreateSelectCustomField(Vendor.NetSuiteVendor.internalId, "custentity_lead_vendor_source");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally { }
            return objReturn;
        }

        public bool Delete()
        {
            RecordRef objBillRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                //if (ToolotsBill == null) throw new Exception("ToolotsBill cannot be null");
                //if (ToolotsBill.IsShipped) throw new Exception("Unable to delete Bill with shipped status, please make sure to void tracking number, delete invoice...etc");
                //if (ToolotsBill.Invoice != null) throw new Exception("Unabel to delete Bill, invoice has already been created");
                //if (SalesOrder.NetSuiteSalesOrder == null) throw new Exception("Unable to load NetSuite sales order");

                //if (ToolotsBill.SalesOrder.IsCompleted && ToolotsBill.IsCancelled)
                //{
                //    //closed sales order commcerhub salesorder qty is completed (either shipped or cancelled)
                //    SalesOrder.Update();
                //    SalesOrder = null;
                //}

                //if (NetSuiteBill != null)
                //{
                //    objBillRef = new RecordRef();
                //    objBillRef.internalId = NetSuiteBill.internalId;
                //    objBillRef.type = RecordType.VendorBill;
                //    objBillRef.typeSpecified = true;
                //    objDeleteResponse = Service.delete(objBillRef);

                //    if (objDeleteResponse.status.isSuccess != true)
                //    {
                //        throw new Exception("Unable to delete Bill: " + objDeleteResponse.status.statusDetail[0].message);
                //    }
                //    else
                //    {
                //        mNetSuiteBill = null;
                //    }
                //}

                ToolotsBill.ErrorMessage = string.Empty;
                ToolotsBill.NetSuiteInternalID = string.Empty;
                ToolotsBill.NetSuiteDocumentNumber = string.Empty;
                ToolotsBill.Update();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Can not find Bill with Internal ID"))
                {
                    ToolotsBill.ErrorMessage = string.Empty;
                    ToolotsBill.NetSuiteInternalID = string.Empty;
                    ToolotsBill.NetSuiteDocumentNumber = string.Empty;
                    ToolotsBill.Update();
                }
                else
                {
                    ToolotsBill.ErrorMessage = "Bill.cs - Delete() - " + ex.Message;
                    ToolotsBill.Update();
                }
            }
            finally
            {
                objBillRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        public Bill ObjectAlreadyExists()
        {
            List<Bill> objBills = null;
            BillFilter objFilter = null;
            Bill objReturn = null;

            try
            {
                objFilter = new BillFilter();
                objFilter.APIExternalID = ToolotsBill.BillID;

                //objFilter.VendorInternalIDs = new List<string>();
                //objFilter.VendorInternalIDs.Add(ToolotsBill.VendorID.Retailer.NetSuiteCustomerInternalID);

                //objFilter.SalesOrderInternalID = ToolotsBill.SalesOrder.NetSuiteInternalID;
                //objFilter.SalesOrderInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;

                //objFilter.PONumber = ToolotsBill.PONumber;
                //objFilter.PONumberOperator = SearchTextNumberFieldOperator.equalTo;
                //objFilter.CustomerInternalIDs = new List<string>();
                //objFilter.CustomerInternalIDs.Add(ToolotsBill.Retailer.NetSuiteCustomerInternalID);

                objBills = GetBills(Service, objFilter);
                if (objBills != null && objBills.Count() > 0)
                {
                    if (objBills.Count > 1) throw new Exception("More than one Bills with API External ID:" + ToolotsBill.BillID + " found in Netsuite with InternalIDs " + string.Join(", ", objBills.Select(m => m.NetSuiteBill.internalId)));
                    objReturn = objBills[0];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBills = null;
                objFilter = null;
            }
            return objReturn;
        }

        private NetSuiteLibrary.com.netsuite.webservices.VendorBill CreateNetSuiteBill()
        {
            NetSuiteLibrary.com.netsuite.webservices.VendorBill objReturn = null;
            int intCustomFieldIndex = 0;
            int intCustomFieldLineIndex = 0;

            try
            {
                if (ToolotsBill.BillLines == null || ToolotsBill.BillLines.Count == 0) throw new Exception("Bill lines is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.VendorBill();
                objReturn.internalId = ToolotsBill.NetSuiteInternalID;
                objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteBillFormID, RecordType.vendorBill);
                //objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
                //objReturn.createdFrom.internalId = ToolotsBill.SalesOrder.NetSuiteInternalID;
                //objReturn.createdFrom.type = RecordType.salesOrder;
                //objReturn.createdFrom.typeSpecified = true;

                objReturn.tranDate = ToolotsBill.TransactionDate.Value;

                objReturn.entity = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ToolotsBill.Vendor.NetSuiteInternalID, RecordType.vendor);

                objReturn.tranId = ToolotsBill.ReferenceNumber;

                objReturn.@class = NetSuiteHelper.GetRecordRef(ToolotsUSClass, RecordType.classification);

                //objReturn.shipMethod = new RecordRef();
                //objReturn.shipMethod.internalId = ToolotsBill.ShippingMethod.NetSuiteInternalID;

                //objReturn.transactionShipAddress = null;
                //objReturn.shipAddress = null;

                //objReturn.generateIntegratedShipperLabel = false;
                //objReturn.generateIntegratedShipperLabelSpecified = true;

                //objReturn.shipStatus = VendorBillShipStatus._picked;

                //Custom Fields
                objReturn.customFieldList = new CustomFieldRef[1];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsBill.BillID, "custbody_api_external_id");

                objReturn.itemList = new VendorBillItemList();
                objReturn.itemList.item = new VendorBillItem[ToolotsBill.BillLines.Count];
                objReturn.itemList.replaceAll = false;

                //for (int i = 0; i < ToolotsBill.BillLines.Count; i++)
                //{
                //    ValidateOnHandQuantity(ToolotsBill.BillLines[i]);

                //    objReturn.itemList.item[i] = new com.netsuite.webservices.VendorBillItem();
                //    objReturn.itemList.item[i].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ToolotsBill.BillLines[i].SalesOrderLine.NetSuiteItemInternalID, RecordType.inventoryItem);
                //    objReturn.itemList.item[i].quantity = ToolotsBill.BillLines[i].Quantity;
                //    objReturn.itemList.item[i].quantitySpecified = true;
                //    objReturn.itemList.item[i].orderLine = ToolotsBill.BillLines[i].SalesOrderLine.NetSuiteLineID.Value;
                //    objReturn.itemList.item[i].orderLineSpecified = true;
                //    objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteDropshipLocationID, RecordType.inventoryItem);

                //    StringCustomFieldRef objAPIExternalIDLine = new StringCustomFieldRef();
                //    objAPIExternalIDLine.value = ToolotsBill.BillLines[i].BillLineID;
                //    objAPIExternalIDLine.internalId = "5266";
                //    objAPIExternalIDLine.scriptId = "custcol_api_external_id";

                //    objReturn.itemList.item[i].customFieldList = new CustomFieldRef[1];
                //    objReturn.itemList.item[i].customFieldList[0] = objAPIExternalIDLine;
                //}

                for (int i = 0; i < ToolotsBill.BillLines.Count; i++)
                {
                    intCustomFieldLineIndex = 0;

                    objReturn.itemList.item[i] = new com.netsuite.webservices.VendorBillItem();
                    objReturn.itemList.item[i].item = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(ToolotsBill.BillLines[i].Item.NetSuiteInternalID, RecordType.inventoryItem);
                    objReturn.itemList.item[i].description = ToolotsBill.BillLines[i].Description;
                    objReturn.itemList.item[i].quantity = ToolotsBill.BillLines[i].Quantity;
                    objReturn.itemList.item[i].quantitySpecified = true;
                    objReturn.itemList.item[i].rate = ToolotsBill.BillLines[i].UnitPrice.ToString();

                    //objReturn.itemList.item[i].orderLine = ToolotsBill.BillLines.Find(m => m.SalesOrderLineID == ToolotsBill.SalesOrder.SalesOrderLines[i].SalesOrderLineID).SalesOrderLine.NetSuiteLineID.Value;
                    //objReturn.itemList.item[i].orderLineSpecified = true;
                    objReturn.itemList.item[i].location = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteConsignedLocationID, RecordType.location);

                    objReturn.itemList.item[i].customFieldList = new CustomFieldRef[99];
                    objReturn.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsBill.BillLines[i].BillLineID, "custcol_api_external_id");
                    objReturn.itemList.item[i].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ToolotsBill.BillLines[i].Description, "custcol_line_memo");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.VendorBill UpdateNetSuiteBill()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.VendorBill objReturn = null;

        //    try
        //    {
        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.VendorBill();
        //        objReturn.internalId = ToolotsBill.NetSuiteInternalID;

        //        if (!string.IsNullOrEmpty(ToolotsBill.TrackingNumber) && ((!ToolotsBill.SalesOrder.RequiredEDILabel || !string.IsNullOrEmpty(ToolotsBill.EDILabelPath))
        //                    && (ToolotsBill.Retailer.ToolotsID == Toolots.ToolotsID.WalmartDVS || !string.IsNullOrEmpty(ToolotsBill.PackingSlipPath))))
        //        {
        //            if (ToolotsBill.IsScanned)
        //                objReturn.shipStatus = VendorBillShipStatus._shipped;
        //            else
        //                objReturn.shipStatus = VendorBillShipStatus._packed;
        //            objReturn.shipStatusSpecified = true;

        //            objReturn.generateIntegratedShipperLabel = false;
        //            objReturn.generateIntegratedShipperLabelSpecified = true;

        //            objReturn.packageList = new com.netsuite.webservices.VendorBillPackageList();
        //            objReturn.packageList.replaceAll = true;

        //            objReturn.packageList.package = new com.netsuite.webservices.VendorBillPackage[1];
        //            objReturn.packageList.package[0] = new com.netsuite.webservices.VendorBillPackage();
        //            objReturn.packageList.package[0].packageWeight = Convert.ToDouble(ToolotsBill.BillLines[0].SalesOrderLine.Item.ItemWeight);
        //            objReturn.packageList.package[0].packageWeightSpecified = true;
        //            objReturn.packageList.package[0].packageTrackingNumber = ToolotsBill.TrackingNumber;
        //            objReturn.packageList.package[0].packageDescr = ToolotsBill.ShippingMethod.Description;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        //private bool ValidateOnHandQuantity(Toolots.BillLine BillLine)
        //{
        //    Item objItem = null;
        //    bool blnFound = false;
        //    bool blnReturn = false;

        //    try
        //    {
        //        objItem = new NetSuiteLibrary.Item(BillLine.SalesOrderLine.Item);
        //        if (objItem.NetSuiteItem.locationsList.locations != null)
        //        {
        //            foreach (com.netsuite.webservices.InventoryItemLocations objInventoryItemLocation in objItem.NetSuiteItem.locationsList.locations)
        //            {
        //                if (objInventoryItemLocation.locationId.internalId == NetSuiteDropshipLocationID)
        //                {
        //                    blnFound = true;

        //                    //Use Quantity OnHand not available
        //                    if (objInventoryItemLocation.quantityOnHand < BillLine.Quantity)
        //                    {
        //                        throw new Exception("There is not enough on hand quantity (" + objInventoryItemLocation.quantityOnHand + ") to create this Bill");
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //        if (!blnFound) throw new Exception("Dropship location is not found for this item");
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

        //use to find out replace all saleslines or not when update Bill to netsuite
        //private bool isAllSalesLineNew()
        //{
        //    return !(BillLines != null && BillLines.Exists(sl => sl.LineID != null && sl.LineID > 0));
        //}

        //public bool IsRequireBill()
        //{
        //    List<BillLine> Fulfillablelines = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Status) && OrderStatus == null) throw new Exception("Status and OrderStatus are not defined");
        //        //Fulfillablelines = GetFulfillableLines();
        //        if ((OrderStatus != null && (OrderStatus == NetSuiteLibrary.com.netsuite.webservices.BillOrderStatus._partiallyFulfilled || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.BillOrderStatus._pendingBillingPartFulfilled
        //                || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.BillOrderStatus._pendingBill)) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else if ((Status.ToLower().Contains("pending Bill") || Status.ToLower().Contains("partially fulfilled") || Status.Contains("待完成") || Status.Contains("部分完成")) && Fulfillablelines != null && Fulfillablelines.Count > 0)
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

        //public List<BillLine> GetFulfillableLines()
        //{
        //    List<BillLine> lstReturn = null;
        //    if (this.BillLines != null && this.BillLines.Count() > 0)
        //    {
        //        lstReturn = this.BillLines;
        //        //when there are other Bills exist for the same Bill
        //        if (this.Bills != null && this.Bills.Count() > 0)
        //        {
        //            //Billlines check against all Billlines of all Bill
        //            foreach (Bill.Bill Bill in this.Bills)
        //            {
        //                foreach (Bill.BillLine BillLine in Bill.BillLines)
        //                {
        //                    if (lstReturn.Any(sl => sl.LineID == BillLine.OrderLine))
        //                        lstReturn.Where(sl => sl.LineID == BillLine.OrderLine).First().Qty -= BillLine.Quantity;
        //                }
        //            }
        //            lstReturn = lstReturn.Any(sl => sl.Qty > 0) ? lstReturn = lstReturn.Where(sl => sl.Qty > 0).ToList() : null;
        //        }
        //    }
        //    return lstReturn;
        //}
        //public List<Bill.BillLine> GetAllBillLines()
        //{
        //    List<Bill.BillLine> lstReturn = null;
        //    if (this.Bills != null && this.Bills.Count() > 0)
        //    {
        //        lstReturn = new List<Bill.BillLine>();
        //        foreach (Bill.Bill Bill in this.Bills)
        //        {
        //            if (Bill.BillLines != null && Bill.BillLines.Count() > 0)
        //            {
        //                lstReturn = lstReturn.Concat(Bill.BillLines).ToList();
        //            }
        //        }
        //    }
        //    return lstReturn;
        //}

        public static Bill GetBill(NetSuiteService Service, BillFilter Filter)
        {
            List<Bill> objBills = null;
            Bill objReturn = null;

            try
            {
                objBills = GetBills(Service, Filter);
                if (objBills != null && objBills.Count >= 1) objReturn = objBills[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objBills = null;
            }
            return objReturn;
        }

        public static List<Bill> GetBills(NetSuiteService Service, BillFilter Filter)
        {
            List<Bill> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Bill>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetBill in objSearchResult.recordList)
                        {
                            if (objNetBill is NetSuiteLibrary.com.netsuite.webservices.VendorBill)
                            {
                                objReturn.Add(new Bill((NetSuiteLibrary.com.netsuite.webservices.VendorBill)objNetBill));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, BillFilter Filter)
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
                    if (Filter.VendorInternalIDs != null)
                    {
                        objTransacSearch.vendorJoin = new VendorSearchBasic();
                        objTransacSearch.vendorJoin.internalId = NetSuiteHelper.LoadMultiSearchField(Filter.VendorInternalIDs);
                    }

                    //objTransacSearch.itemJoin = new com.netsuite.webservices.ItemSearchBasic();
                    //objTransacSearch.itemJoin...line

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

                    //if (!string.IsNullOrEmpty(Filter.SalesOrderInternalID))
                    //{
                    //    if (Filter.SalesOrderInternalIDOperator == null) throw new Exception("SalesOrderInternalIDOperator must be specified");

                    //    //SearchBooleanCustomField objIsConsignedField = new com.netsuite.webservices.SearchBooleanCustomField();
                    //    //objIsConsignedField.searchValue = true;
                    //    //objIsConsignedField.internalId = "";
                    //    //objIsConsignedField.scriptId = "";


                    //    SearchMultiSelectField objMultiSelectField = new SearchMultiSelectField();
                    //    objMultiSelectField.searchValue = new RecordRef[1];
                    //    objMultiSelectField.searchValue[0] = new RecordRef();
                    //    objMultiSelectField.searchValue[0].internalId = Filter.SalesOrderInternalID;
                    //    objMultiSelectField.@operator = Filter.SalesOrderInternalIDOperator.Value;
                    //    objMultiSelectField.operatorSpecified = true;

                    //    objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                    //    objTransacSearch.createdFromJoin.internalId = objMultiSelectField;
                    //}
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_vendorBill" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find Bill - " + objSearchResult.status.statusDetail[0].message);
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


