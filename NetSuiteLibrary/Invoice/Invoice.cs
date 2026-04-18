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

namespace NetSuiteLibrary.Invoice
{
    public class Invoice : NetSuiteBase
    {
        private static string NetSuiteInvoiceFormID
        {
            get
            {
                if (ConfigurationManager.AppSettings["NetSuiteInvoiceFormID"] != null)
                    return ConfigurationManager.AppSettings["NetSuiteInvoiceFormID"].ToString();
                else
                    return string.Empty;
            }
        }
        private ImageSolutions.Invoice.Invoice mImageSolutionsInvoice = null;
        public ImageSolutions.Invoice.Invoice ImageSolutionsInvoice
        {
            get
            {
                if (mImageSolutionsInvoice == null && mNetSuiteInvoice != null && !string.IsNullOrEmpty(mNetSuiteInvoice.internalId))
                {
                    List<ImageSolutions.Invoice.Invoice> objInvoices = null;
                    ImageSolutions.Invoice.InvoiceFilter objFilter = null;

                    try
                    {
                        objFilter = new ImageSolutions.Invoice.InvoiceFilter();
                        objFilter.InternalID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.InternalID.SearchString = mNetSuiteInvoice.internalId;
                        objInvoices = ImageSolutions.Invoice.Invoice.GetInvoices(objFilter);
                        if (objInvoices != null && objInvoices.Count > 0)
                        {
                            mImageSolutionsInvoice = objInvoices[0];
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objInvoices = null;
                    }
                }
                return mImageSolutionsInvoice;
            }
            private set
            {
                mImageSolutionsInvoice = value;
            }
        }

        private NetSuiteLibrary.com.netsuite.webservices.Invoice mNetSuiteInvoice = null;
        public NetSuiteLibrary.com.netsuite.webservices.Invoice NetSuiteInvoice
        {
            get
            {
                if (mNetSuiteInvoice == null && mImageSolutionsInvoice != null && !string.IsNullOrEmpty(mImageSolutionsInvoice.InternalID))
                {
                    mNetSuiteInvoice = LoadNetSuiteInvoice(mImageSolutionsInvoice.InternalID);
                }
                return mNetSuiteInvoice;
            }
            private set
            {
                mNetSuiteInvoice = value;
            }
        }

        //private SalesOrder.SalesOrder mSalesOrder = null;
        //public SalesOrder.SalesOrder SalesOrder
        //{
        //    get
        //    {
        //        if (mSalesOrder == null && ImageSolutionsInvoice != null && ImageSolutionsInvoice.SalesOrder != null)
        //        {
        //            mSalesOrder = new SalesOrder.SalesOrder(ImageSolutionsInvoice.SalesOrder);
        //        }
        //        return mSalesOrder;
        //    }
        //    private set
        //    {
        //        mSalesOrder = value;
        //    }
        //}
        public Invoice()
        {
            mImageSolutionsInvoice = null;
        }

        public Invoice(ImageSolutions.Invoice.Invoice ImageSolutionsInvoice)
        {
            mImageSolutionsInvoice = ImageSolutionsInvoice;
        }

        public Invoice(NetSuiteLibrary.com.netsuite.webservices.Invoice NetSuiteInvoice)
        {
            mNetSuiteInvoice = NetSuiteInvoice;
        }

        private NetSuiteLibrary.com.netsuite.webservices.Invoice LoadNetSuiteInvoice(string NetSuiteInternalID)
        {
            RecordRef objSORef = null;
            ReadResponse objReadResult = null;
            NetSuiteLibrary.com.netsuite.webservices.Invoice objReturn = null;

            try
            {
                objSORef = new RecordRef();
                objSORef.type = RecordType.invoice;
                objSORef.typeSpecified = true;
                objSORef.internalId = NetSuiteInternalID;

                objReadResult = Service.get(objSORef);

                if (objReadResult != null && objReadResult.record != null && (objReadResult.record is NetSuiteLibrary.com.netsuite.webservices.Invoice))
                {
                    objReturn = (NetSuiteLibrary.com.netsuite.webservices.Invoice)objReadResult.record;
                }
                else
                {
                    throw new Exception("Can not find Invoice with Internal ID : " + NetSuiteInternalID);
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
            if (mImageSolutionsInvoice != null)
                return false;// CreateInvoice();
         
            else
                return false;
        }


        public bool CreateInvoice()
        {
            WriteResponse objWriteResponse = null;
            Invoice objInvoice = null;

            try
            {
                if (ImageSolutionsInvoice == null) throw new Exception("ImageSolutionsInvoice cannot be null");
                //if (SalesOrder == null) throw new Exception("Sales order is missing");
                //if (NetSuiteInvoice != null) throw new Exception("Invoice record already exists in NetSuite");
                //if (string.IsNullOrEmpty(ImageSolutionsInvoice.SalesOrderID)) throw new Exception("SalesOrderID is missing");
                if (ImageSolutionsInvoice.InvoiceLines == null || ImageSolutionsInvoice.InvoiceLines.Count() == 0) throw new Exception("Invoice lines are missing");
                //if (!ImageSolutionsInvoice.SalesOrder.IsCompleted) throw new Exception("Sales order is not yet completed, cannot generate invoice");
                //if (SalesOrder.NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._pendingBilling) throw new Exception("Unable to create NetSuite invoice, sales order stauts is not 'PendingBilling'");

                objInvoice = ObjectAlreadyExists();

                if (objInvoice != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsInvoice.InternalID = objInvoice.NetSuiteInvoice.internalId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteInvoice());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Invoice: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsInvoice.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                    }
                }

                foreach (NetSuiteLibrary.com.netsuite.webservices.InvoiceItem objInvoiceItem in NetSuiteInvoice.itemList.item)
                {
                    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objInvoiceItem, "custcol_api_external_id");
                    if (string.IsNullOrEmpty(strAPIExternalID) && (objInvoiceItem.item.internalId == "0"))
                    {
                        //This is item group end of group item, or general discount, ignore
                    }
                    else
                    {
                        if (ImageSolutionsInvoice.InvoiceLines.Exists(m => m.InvoiceLineID == strAPIExternalID))
                        {
                            ImageSolutionsInvoice.InvoiceLines.Find(m => m.InvoiceLineID == strAPIExternalID).LineID = Convert.ToString(objInvoiceItem.orderLine);
                        }
                        else
                        {
                            ImageSolutionsInvoice.InternalID = string.Empty;
                            throw new Exception("APIExternalID for invoice line " + strAPIExternalID + " did not get created, not found in NetSuite Invoice");
                        }
                    }
                }

                ImageSolutionsInvoice.ErrorMessage = string.Empty;
                ImageSolutionsInvoice.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsInvoice.ErrorMessage = "Invoice.cs - Create() - " + ex.Message;
                ImageSolutionsInvoice.Update();
            }
            finally
            {
                objWriteResponse = null;
            }
            return true;
        }

        public bool CreateBill()
        {
            WriteResponse objWriteResponse = null;
            Invoice objInvoice = null;

            try
            {
                if (ImageSolutionsInvoice == null) throw new Exception("ImageSolutionsInvoice cannot be null");
                //if (SalesOrder == null) throw new Exception("Sales order is missing");
                //if (NetSuiteInvoice != null) throw new Exception("Invoice record already exists in NetSuite");
                //if (string.IsNullOrEmpty(ImageSolutionsInvoice.SalesOrderID)) throw new Exception("SalesOrderID is missing");
                if (ImageSolutionsInvoice.InvoiceLines == null || ImageSolutionsInvoice.InvoiceLines.Count() == 0) throw new Exception("Invoice lines are missing");
                //if (!ImageSolutionsInvoice.SalesOrder.IsCompleted) throw new Exception("Sales order is not yet completed, cannot generate invoice");
                //if (SalesOrder.NetSuiteSalesOrder.orderStatus != SalesOrderOrderStatus._pendingBilling) throw new Exception("Unable to create NetSuite invoice, sales order stauts is not 'PendingBilling'");

                objInvoice = ObjectAlreadyExists();

                if (objInvoice != null)
                {
                    //NetSuite InternalID did not get updated, auto fix
                    ImageSolutionsInvoice.InternalID = objInvoice.NetSuiteInvoice.internalId;
                }
                else
                {
                    objWriteResponse = Service.add(CreateNetSuiteInvoice());
                    if (objWriteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to create Invoice: " + objWriteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        ImageSolutionsInvoice.InternalID = ((RecordRef)objWriteResponse.baseRef).internalId;
                    }
                }

                foreach (NetSuiteLibrary.com.netsuite.webservices.InvoiceItem objInvoiceItem in NetSuiteInvoice.itemList.item)
                {
                    string strAPIExternalID = NetSuiteLibrary.NetSuiteHelper.GetStringCustomFieldValue(objInvoiceItem, "custcol_api_external_id");
                    if (string.IsNullOrEmpty(strAPIExternalID) && (objInvoiceItem.item.internalId == "0"))
                    {
                        //This is item group end of group item, or general discount, ignore
                    }
                    else
                    {
                        if (ImageSolutionsInvoice.InvoiceLines.Exists(m => m.InvoiceLineID == strAPIExternalID))
                        {
                            ImageSolutionsInvoice.InvoiceLines.Find(m => m.InvoiceLineID == strAPIExternalID).LineID = Convert.ToString(objInvoiceItem.orderLine);
                        }
                        else
                        {
                            ImageSolutionsInvoice.InternalID = string.Empty;
                            throw new Exception("APIExternalID for invoice line " + strAPIExternalID + " did not get created, not found in NetSuite Invoice");
                        }
                    }
                }

                ImageSolutionsInvoice.ErrorMessage = string.Empty;
                ImageSolutionsInvoice.Update();
            }
            catch (Exception ex)
            {
                ImageSolutionsInvoice.ErrorMessage = "Invoice.cs - Create() - " + ex.Message;
                ImageSolutionsInvoice.Update();
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
        //        //Check to see if invoice total amount match with shipped total amount
        //        if (SalesOrder.NetSuiteSalesOrder == null) throw new Exception("Unable to load NetSuite sales order");

        //        //if (ImageSolutionsInvoice.CancelledFulfillmentLines != null && ImageSolutionsInvoice.CancelledFulfillmentLines.Count > 0)
        //        //{
        //        //    SalesOrder.Update();
        //        //    SalesOrder = null;
        //        //}

        //        if (SalesOrder.NetSuiteSalesOrder.orderStatus == SalesOrderOrderStatus._closed || SalesOrder.NetSuiteSalesOrder.orderStatus == SalesOrderOrderStatus._fullyBilled)
        //        {
        //            ImageSolutionsInvoice.IsReadyToSync = true;
        //        }
        //        else
        //        {
        //            throw new Exception("NetSuite sales order status is currently set to " + SalesOrder.NetSuiteSalesOrder.orderStatus.ToString() + ", it's supposed to be CLOSED or BILLED when invoicing");
        //        }

        //        ImageSolutionsInvoice.ErrorMessage = string.Empty;
        //        ImageSolutionsInvoice.Update();
        //    }
        //    catch (Exception ex)
        //    {
        //        ImageSolutionsInvoice.ErrorMessage = "Invoice.cs - Update() - " + ex.Message;
        //        ImageSolutionsInvoice.Update();
        //    }
        //    finally { }
        //    return true;
        //}

        public Invoice ObjectAlreadyExists()
        {
            //List<Invoice> objInvoices = null;
            //InvoiceFilter objFilter = null;
            //Invoice objReturn = null;

            //try
            //{
            //    objFilter = new InvoiceFilter();
            //    objFilter.APIExternalID = ImageSolutionsInvoice.ExternalID;
            //    objFilter.PurchaseOrderInternalID = ImageSolutionsInvoice.PurchaseOrder.InternalID;
            //    objFilter.SalesOrderInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;
            //    //objFilter.SalesOrderInternalID = ImageSolutionsInvoice.SalesOrder.NetSuiteInternalID;
            //    //objFilter.SalesOrderInternalIDOperator = SearchMultiSelectFieldOperator.anyOf;
            //    objInvoices = GetInvoices(Service, objFilter);
            //    if (objInvoices != null && objInvoices.Count() > 0)
            //    {
            //        if (objInvoices.Count > 1) throw new Exception("More than one Invoices with API External ID:" + ImageSolutionsInvoice.InvoiceID + " found in Netsuite with InternalIDs " + string.Join(", ", objInvoices.Select(m => m.NetSuiteInvoice.internalId)));
            //        objReturn = objInvoices[0];
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    objInvoices = null;
            //    objFilter = null;
            //}
            //return objReturn;
            return null;
        }

        //private Item.Item GetNetSuiteItem(string SKU)
        //{
        //    NetSuiteLibrary.Item.Item objNSItem = null;
        //    NetSuiteLibrary.Item.ItemFilter objNSFilter = null;
        //    ImageSolutions.Item.Item objItem = null;
        //    ImageSolutions.Item.ItemFilter objItemFilter = null;

        //    try
        //    {
        //        if (objNSItem == null)
        //        {
        //            objNSFilter = new Item.ItemFilter();
        //            objNSFilter.VendorCode1 = SKU;
        //            objNSItem = Item.Item.GetItem(Service, objNSFilter);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objNSFilter = null;
        //    }

        //    return objNSItem;
        //}

        private NetSuiteLibrary.com.netsuite.webservices.Invoice CreateNetSuiteInvoice()
        {
            NetSuiteLibrary.com.netsuite.webservices.Invoice objReturn = null;
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
                //if (ImageSolutionsInvoice.SalesOrder == null) throw new Exception("SalesOrder is missing");
                //if (string.IsNullOrEmpty(ImageSolutionsInvoice.SalesOrder.NetSuiteInternalID)) throw new Exception("SalesOrder.NetSuiteInternalID is missing");

                objReturn = new NetSuiteLibrary.com.netsuite.webservices.Invoice();
                objReturn.internalId = ImageSolutionsInvoice.InternalID;
                //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteInvoiceFormID, RecordType.invoice);
               
                //objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
                //objReturn.createdFrom.internalId = ImageSolutionsInvoice.PurchaseOrder.InternalID;
                //objReturn.createdFrom.type = RecordType.purchaseOrder;
               
                //objReturn.currency = NetSuiteHelper.GetRecordRef(GetCurrencyInternalID(ImageSolutionsInvoice.Currency), RecordType.currency);

                objReturn.tranDate = Convert.ToDateTime(ImageSolutionsInvoice.TransactionDate);
                objReturn.tranDateSpecified = true;

                objReturn.customFieldList = new CustomFieldRef[99];
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.InvoiceID, "custbody_api_external_id");
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.ShipToAddress.Phone, "custbody3");
                //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.Customer.EmailAddress, "custbody4");
                ////objReturn.nullFieldList = new string[99];
                //objReturn.nullFieldList[intNullFieldIndex++] = "salesrep";

                //objReturn.shippingCost = ImageSolutionsInvoice.ShippingAmount == null ? 0 : Convert.ToDouble(ImageSolutionsInvoice.ShippingAmount.Value);
                //objReturn.shippingCostSpecified = true;

                //objReturn.location = NetSuiteHelper.GetRecordRef("27", RecordType.location);

                //objReturn.otherRefNum = ImageSolutionsInvoice.PONumber;

                //dcmTotal += Convert.ToDecimal(objReturn.shippingCost);


                objReturn.itemList = new InvoiceItemList();
                objReturn.itemList.item = new InvoiceItem[ImageSolutionsInvoice.InvoiceLines.Count()];

                for (int i = 0; i < ImageSolutionsInvoice.InvoiceLines.Count; i++)
                {
                    intCustomFieldLineIndex = 0;
                    string strTaxRate = string.Empty;

                    //objItem = new Item.Item(ImageSolutionsInvoice.InvoiceLines[i].Item);

                    //objReturn.itemList.item[intItemIndex] = new InvoiceItem();
                    
                    //objReturn.itemList.item[intItemIndex].price = new RecordRef();
                    //objReturn.itemList.item[intItemIndex].price.internalId = "-1"; //Custom Pricing

                    //objReturn.itemList.item[intItemIndex].quantity = Convert.ToDouble(ImageSolutionsInvoice.InvoiceLines[i].Quantity);
                    //objReturn.itemList.item[intItemIndex].quantitySpecified = true;

                    //objReturn.itemList.item[intItemIndex].rate = ImageSolutionsInvoice.InvoiceLines[i].UnitPrice.ToString();

                    //objReturn.itemList.item[intItemIndex].orderLine = Convert.ToInt64(ImageSolutionsInvoice.InvoiceLines[i].PurchaseOrderLine.LineID);
                    //objReturn.itemList.item[intItemIndex].orderLineSpecified = true;

                    //objReturn.itemList.item[intItemIndex].customFieldList = new CustomFieldRef[99];
                    //objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.InvoiceLines[i].InvoiceLineID, "custcol_api_external_id");

                    intItemIndex++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }

        //private NetSuiteLibrary.com.netsuite.webservices.VendorBill CreateNetSuiteBill()
        //{
        //    NetSuiteLibrary.com.netsuite.webservices.VendorBill objReturn = null;
        //    int intCustomFieldIndex = 0;
        //    int intCustomFieldLineIndex = 0;
        //    int intNullFieldIndex = 0;
        //    NetSuiteLibrary.Item.Item objItem = null;
        //    string strItemType = string.Empty;
        //    string strItemVendorID = string.Empty;
        //    string str3PLVendorID = string.Empty;
        //    decimal dcmTotal = 0;
        //    int intItemIndex = 0;
        //    string strBinNumberInternalID = string.Empty;
        //    double dbLocationAvailablePrimary = 0;
        //    double dbLocationAvailableSecondary = 0;
        //    NetSuiteLibrary.Item.Item objKitMember = null;
        //    NetSuiteLibrary.Item.ItemFilter objKitMemberFilter = null;

        //    try
        //    {
        //        //if (ImageSolutionsInvoice.SalesOrder == null) throw new Exception("SalesOrder is missing");
        //        //if (string.IsNullOrEmpty(ImageSolutionsInvoice.SalesOrder.NetSuiteInternalID)) throw new Exception("SalesOrder.NetSuiteInternalID is missing");

        //        objReturn = new NetSuiteLibrary.com.netsuite.webservices.VendorBill();
        //        objReturn.internalId = ImageSolutionsInvoice.InternalID;
        //        //objReturn.customForm = NetSuiteLibrary.NetSuiteHelper.GetRecordRef(NetSuiteInvoiceFormID, RecordType.invoice);

        //        //objReturn 
        //        //objReturn.createdFrom = new com.netsuite.webservices.RecordRef();
        //        //objReturn.createdFrom.internalId = ImageSolutionsInvoice.PurchaseOrder.InternalID;
        //        //objReturn.createdFrom.type = RecordType.purchaseOrder;

        //        //objReturn.currency = NetSuiteHelper.GetRecordRef(GetCurrencyInternalID(ImageSolutionsInvoice.Currency), RecordType.currency);

        //        objReturn.tranDate = Convert.ToDateTime(ImageSolutionsInvoice.InvoiceDate);
        //        objReturn.tranDateSpecified = true;

        //        objReturn.customFieldList = new CustomFieldRef[99];
        //        //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.InvoiceID, "custbody_api_external_id");
        //        //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.ShipToAddress.Phone, "custbody3");
        //        //objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.Customer.EmailAddress, "custbody4");
        //        ////objReturn.nullFieldList = new string[99];
        //        //objReturn.nullFieldList[intNullFieldIndex++] = "salesrep";

        //        //objReturn.shippingCost = ImageSolutionsInvoice.ShippingAmount == null ? 0 : Convert.ToDouble(ImageSolutionsInvoice.ShippingAmount.Value);
        //        //objReturn.shippingCostSpecified = true;

        //        //objReturn.location = NetSuiteHelper.GetRecordRef("27", RecordType.location);

        //        //objReturn.otherRefNum = ImageSolutionsInvoice.PONumber;

        //        //dcmTotal += Convert.ToDecimal(objReturn.shippingCost);


        //        objReturn.itemList = new InvoiceItemList();
        //        objReturn.itemList.item = new InvoiceItem[ImageSolutionsInvoice.InvoiceLines.Count()];

        //        for (int i = 0; i < ImageSolutionsInvoice.InvoiceLines.Count; i++)
        //        {
        //            intCustomFieldLineIndex = 0;
        //            string strTaxRate = string.Empty;

        //            objItem = new Item.Item(ImageSolutionsInvoice.InvoiceLines[i].Item);

        //            objReturn.itemList.item[intItemIndex] = new InvoiceItem();

        //            objReturn.itemList.item[intItemIndex].price = new RecordRef();
        //            objReturn.itemList.item[intItemIndex].price.internalId = "-1"; //Custom Pricing

        //            objReturn.itemList.item[intItemIndex].quantity = ImageSolutionsInvoice.InvoiceLines[i].Quantity;
        //            objReturn.itemList.item[intItemIndex].quantitySpecified = true;

        //            objReturn.itemList.item[intItemIndex].rate = ImageSolutionsInvoice.InvoiceLines[i].UnitPrice.ToString();

        //            objReturn.itemList.item[intItemIndex].orderLine = Convert.ToInt64(ImageSolutionsInvoice.InvoiceLines[i].PurchaseOrderLine.LineID);
        //            objReturn.itemList.item[intItemIndex].orderLineSpecified = true;

        //            //objReturn.itemList.item[intItemIndex].customFieldList = new CustomFieldRef[99];
        //            //objReturn.itemList.item[intItemIndex].customFieldList[intCustomFieldLineIndex++] = NetSuiteHelper.CreateStringCustomField(ImageSolutionsInvoice.InvoiceLines[i].InvoiceLineID, "custcol_api_external_id");

        //            intItemIndex++;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //    return objReturn;
        //}

        private NetSuiteLibrary.com.netsuite.webservices.Invoice UpdateNetSuiteInvoice()
        {
            NetSuiteLibrary.com.netsuite.webservices.Invoice objReturn = null;

            //try
            //{
            //    objReturn = new NetSuiteLibrary.com.netsuite.webservices.Invoice.Invoice();
            //    objReturn.internalId = ImageSolutionsInvoice.NetSuiteInternalID;

            //    if (!string.IsNullOrEmpty(ImageSolutionsInvoice.TrackingNumber) && !string.IsNullOrEmpty(ImageSolutionsInvoice.PackingSlipPath))
            //    {
            //        objReturn.shipStatus = InvoiceShipStatus._packed;
            //        objReturn.shipStatusSpecified = true;

            //        objReturn.generateIntegratedShipperLabel = false;
            //        objReturn.generateIntegratedShipperLabelSpecified = true;

            //        objReturn.packageList = new com.netsuite.webservices.Invoice.InvoicePackageList();
            //        objReturn.packageList.replaceAll = true;

            //        objReturn.packageList.package = new com.netsuite.webservices.Invoice.InvoicePackage[1];
            //        objReturn.packageList.package[0] = new com.netsuite.webservices.Invoice.InvoicePackage();
            //        objReturn.packageList.package[0].packageWeight = 1;
            //        objReturn.packageList.package[0].packageWeightSpecified = true;
            //        objReturn.packageList.package[0].packageTrackingNumber = ImageSolutionsInvoice.TrackingNumber;
            //        objReturn.packageList.package[0].packageDescr = ImageSolutionsInvoice.SalesOrder.ShippingCode;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally { }
            return objReturn;
        }


        public bool UpdateInvoiceIsExported(string netsuiteinternalid)
        {
            WriteResponse objWriteResult = null;

            try
            {
                objWriteResult = Service.update(UpdateNetSuiteInvoiceIsExported(netsuiteinternalid));

                if (objWriteResult.status.isSuccess != true)
                {
                    throw new Exception("Invoice Update() : " + objWriteResult.status.statusDetail[0].message);
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

        private NetSuiteLibrary.com.netsuite.webservices.Invoice UpdateNetSuiteInvoiceIsExported(string netsuiteinternalid)
        {
            NetSuiteLibrary.com.netsuite.webservices.Invoice objReturn = null;
            int intCustomFieldIndex = 0;

            try
            {
                objReturn = new com.netsuite.webservices.Invoice();
                objReturn.internalId = netsuiteinternalid;

                objReturn.customFieldList = new CustomFieldRef[99];
                objReturn.customFieldList[intCustomFieldIndex++] = NetSuiteHelper.CreateBooleanCustomField(true, "custbody_invoice_exported");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
            return objReturn;
        }


        public bool Delete()
        {
            RecordRef objInvoiceOrderRef = null;
            WriteResponse objDeleteResponse = null;

            try
            {
                //if (ImageSolutionsInvoice == null) throw new Exception("ImageSolutionsInvoice cannot be null");

                if (NetSuiteInvoice != null)
                {
                    objInvoiceOrderRef = new RecordRef();
                    objInvoiceOrderRef.internalId = NetSuiteInvoice.internalId;
                    objInvoiceOrderRef.type = RecordType.invoice;
                    objInvoiceOrderRef.typeSpecified = true;
                    objDeleteResponse = Service.delete(objInvoiceOrderRef, null);

                    if (objDeleteResponse.status.isSuccess != true)
                    {
                        throw new Exception("Unable to delete invoice: " + objDeleteResponse.status.statusDetail[0].message);
                    }
                    else
                    {
                        mNetSuiteInvoice = null;
                    }
                }

                //ImageSolutionsInvoice.ErrorMessage = string.Empty;
                //ImageSolutionsInvoice.NetSuiteInternalID = string.Empty;
                //ImageSolutionsInvoice.NetSuiteDocumentNumber = string.Empty;
                //ImageSolutionsInvoice.Update();
            }
            catch (Exception ex)
            {
                throw ex;
                //if (ex.Message.Contains("Can not find Invoice with Internal ID"))
                //{
                //    ImageSolutionsInvoice.ErrorMessage = string.Empty;
                //    ImageSolutionsInvoice.NetSuiteInternalID = string.Empty;
                //    ImageSolutionsInvoice.NetSuiteDocumentNumber = string.Empty;
                //    ImageSolutionsInvoice.Update();
                //}
                //else
                //{
                //    ImageSolutionsInvoice.ErrorMessage = "Invoice.cs - Delete() - " + ex.Message;
                //    ImageSolutionsInvoice.Update();
                //}
            }
            finally
            {
                objInvoiceOrderRef = null;
                objDeleteResponse = null;
            }
            return true;
        }

        //use to find out replace all saleslines or not when update Invoice to netsuite
        //private bool isAllSalesLineNew()
        //{
        //    return !(InvoiceLines != null && InvoiceLines.Exists(sl => sl.LineID != null && sl.LineID > 0));
        //}

        //public bool IsRequireInvoice()
        //{
        //    List<InvoiceLine> Fulfillablelines = null;
        //    try
        //    {
        //        if (string.IsNullOrEmpty(Status) && OrderStatus == null) throw new Exception("Status and OrderStatus are not defined");
        //        //Fulfillablelines = GetFulfillableLines();
        //        if ((OrderStatus != null && (OrderStatus == NetSuiteLibrary.com.netsuite.webservices.Invoice.InvoiceOrderStatus._partiallyFulfilled || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.Invoice.InvoiceOrderStatus._pendingBillingPartFulfilled
        //                || OrderStatus == NetSuiteLibrary.com.netsuite.webservices.Invoice.InvoiceOrderStatus._pendingInvoice)) && Fulfillablelines != null && Fulfillablelines.Count > 0)
        //        {
        //            return true;
        //        }
        //        else if ((Status.ToLower().Contains("pending Invoice") || Status.ToLower().Contains("partially fulfilled") || Status.Contains("待完成") || Status.Contains("部分完成")) && Fulfillablelines != null && Fulfillablelines.Count > 0)
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

        //public List<InvoiceLine> GetFulfillableLines()
        //{
        //    List<InvoiceLine> lstReturn = null;
        //    if (this.Invoice.InvoiceLines != null && this.Invoice.InvoiceLines.Count() > 0)
        //    {
        //        lstReturn = this.Invoice.InvoiceLines;
        //        //when there are other Invoices exist for the same Invoice
        //        if (this.Invoice.Invoices != null && this.Invoice.Invoices.Count() > 0)
        //        {
        //            //Invoicelines check against all Invoicelines of all Invoice
        //            foreach (Invoice.Invoice.Invoice Invoice in this.Invoice.Invoices)
        //            {
        //                foreach (Invoice.Invoice.InvoiceLine InvoiceLine in Invoice.Invoice.InvoiceLines)
        //                {
        //                    if (lstReturn.Any(sl => sl.LineID == InvoiceLine.OrderLine))
        //                        lstReturn.Where(sl => sl.LineID == InvoiceLine.OrderLine).First().Qty -= InvoiceLine.Quantity;
        //                }
        //            }
        //            lstReturn = lstReturn.Any(sl => sl.Qty > 0) ? lstReturn = lstReturn.Where(sl => sl.Qty > 0).ToList() : null;
        //        }
        //    }
        //    return lstReturn;
        //}
        //public List<Invoice.Invoice.InvoiceLine> GetAllInvoiceLines()
        //{
        //    List<Invoice.Invoice.InvoiceLine> lstReturn = null;
        //    if (this.Invoice.Invoices != null && this.Invoice.Invoices.Count() > 0)
        //    {
        //        lstReturn = new List<Invoice.Invoice.InvoiceLine>();
        //        foreach (Invoice.Invoice.Invoice Invoice in this.Invoice.Invoices)
        //        {
        //            if (Invoice.Invoice.InvoiceLines != null && Invoice.Invoice.InvoiceLines.Count() > 0)
        //            {
        //                lstReturn = lstReturn.Concat(Invoice.Invoice.InvoiceLines).ToList();
        //            }
        //        }
        //    }
        //    return lstReturn;
        //}

        public static Invoice GetInvoice(NetSuiteService Service, InvoiceFilter Filter)
        {
            List<Invoice> objInvoices = null;
            Invoice objReturn = null;

            try
            {
                objInvoices = GetInvoices(Service, Filter);
                if (objInvoices != null && objInvoices.Count >= 1) objReturn = objInvoices[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objInvoices = null;
            }
            return objReturn;
        }

        private static List<Invoice> GetInvoices(NetSuiteService Service, InvoiceFilter Filter)
        {
            List<Invoice> objReturn = null;
            SearchResult objSearchResult = null;

            try
            {
                objReturn = new List<Invoice>();
                objSearchResult = GetNetSuiteTransactions(Service, Filter);
                if (objSearchResult != null && objSearchResult.totalRecords > 0)
                {
                    do
                    {
                        foreach (object objNetInvoice in objSearchResult.recordList)
                        {
                            if (objNetInvoice is NetSuiteLibrary.com.netsuite.webservices.Invoice)
                            {
                                objReturn.Add(new Invoice((NetSuiteLibrary.com.netsuite.webservices.Invoice)objNetInvoice));
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

        private static SearchResult GetNetSuiteTransactions(NetSuiteService Service, InvoiceFilter Filter)
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
                        objAPIExternalID.scriptId = "custbody_is_stv_txn_id";
                        objAPIExternalID.searchValue = Filter.APIExternalID;
                        objAPIExternalID.@operator = SearchStringFieldOperator.@is;
                        objAPIExternalID.operatorSpecified = true;
                        objTransacSearch.basic.customFieldList[0] = objAPIExternalID;
                    }

                    if (!string.IsNullOrEmpty(Filter.PONumber))
                    {
                        objTransacSearch.basic.otherRefNum = new SearchTextNumberField();
                        objTransacSearch.basic.otherRefNum.searchValue = Filter.PONumber;
                        objTransacSearch.basic.otherRefNum.@operator = SearchTextNumberFieldOperator.equalTo;
                        objTransacSearch.basic.otherRefNum.operatorSpecified = true;
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

                    if (!string.IsNullOrEmpty(Filter.PurchaseOrderInternalID))
                    {
                        if (Filter.SalesOrderInternalIDOperator == null) throw new Exception("SalesOrderInternalIDOperator must be specified");

                        SearchMultiSelectField objMultiSelectField = new SearchMultiSelectField();
                        objMultiSelectField.searchValue = new RecordRef[1];
                        objMultiSelectField.searchValue[0] = new RecordRef();
                        objMultiSelectField.searchValue[0].internalId = Filter.PurchaseOrderInternalID;
                        objMultiSelectField.@operator = Filter.SalesOrderInternalIDOperator.Value;
                        objMultiSelectField.operatorSpecified = true;

                        objTransacSearch.createdFromJoin = new TransactionSearchBasic();
                        objTransacSearch.createdFromJoin.internalId = objMultiSelectField;
                    }
                }

                objTransacSearch.basic.type = new SearchEnumMultiSelectField();
                objTransacSearch.basic.type.@operator = SearchEnumMultiSelectFieldOperator.anyOf;
                objTransacSearch.basic.type.operatorSpecified = true;
                objTransacSearch.basic.type.searchValue = new string[] { "_invoice" };

                objSearchPreferences = new SearchPreferences();
                objSearchPreferences.bodyFieldsOnly = false;
                objSearchPreferences.pageSize = 10;
                objSearchPreferences.pageSizeSpecified = true;

                Service.searchPreferences = objSearchPreferences;
                objSearchResult = Service.search(objTransacSearch);

                if (objSearchResult.status.isSuccess != true)  throw new Exception("Cannot find Invoice - " + objSearchResult.status.statusDetail[0].message);
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


