using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveWearIntegration.Task
{
    public class SyncInvoice
    {
        public static bool Start()
        {
            ActiveWearAPI.Request request = new ActiveWearAPI.Request();
            request.GET_Orders();
            return true;
        }

        public static bool ExecuteByInvoiceDate()
        {
            ImageSolutions.PurchaseOrder.PurchaseOrderFilter objFilter = null;
            ImageSolutions.PurchaseOrder.PurchaseOrder ISPurchaseOrder = null;
            ImageSolutions.Invoice.InvoiceFilter objInvoiceFilter = null;
            ImageSolutions.Invoice.Invoice ISInvoice = null;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                ActiveWearAPI.Request request = new ActiveWearAPI.Request();
                List<ActiveWearAPI.OrderHistory.Header> orders = null;//request.GetOrdersByInvoiceDate(DateTime.UtcNow);

                foreach (ActiveWearAPI.OrderHistory.Header _order in orders)
                {
                    try
                    {
                        objFilter = new ImageSolutions.PurchaseOrder.PurchaseOrderFilter();
                        objFilter.PurchaseOrderNumber = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PurchaseOrderNumber.SearchString = _order.poNumber;

                        ISPurchaseOrder = ImageSolutions.PurchaseOrder.PurchaseOrder.GetPurchaseOrder(objFilter);

                        if (ISPurchaseOrder == null)
                        {
                            throw new Exception("Purchase Order not in database");
                        }

                        objInvoiceFilter = new ImageSolutions.Invoice.InvoiceFilter();
                        objInvoiceFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                        objInvoiceFilter.ExternalID.SearchString = _order.invoiceNumber;
                        objInvoiceFilter.OrderNumber = new Database.Filter.StringSearch.SearchFilter();
                        objInvoiceFilter.OrderNumber.SearchString = _order.orderNumber;

                        ISInvoice = ImageSolutions.Invoice.Invoice.GetInvoice(objInvoiceFilter);

                        if(ISInvoice == null)
                        {
                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();

                            ISInvoice = new ImageSolutions.Invoice.Invoice();
                            ISInvoice.ExternalID = _order.invoiceNumber;
                            ISInvoice.OrderNumber = _order.orderNumber;

                            ISInvoice.PurchaseOrderID = ISPurchaseOrder.PurchaseOrderID;
                            ISInvoice.InvoiceDate = _order.invoiceDate;

                            ISInvoice.InvoiceLines = new List<ImageSolutions.Invoice.InvoiceLine>();

                            foreach (ActiveWearAPI.OrderHistory.Header.Line _line in _order.lines)
                            {
                                ImageSolutions.Invoice.InvoiceLine ISInvoiceLine = new ImageSolutions.Invoice.InvoiceLine();

                                ISInvoiceLine.PurchaseOrderLineID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).PurchaseOrderLineID;
                                ISInvoiceLine.ItemID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).ItemID;
                                //ISInvoiceLine.ItemNumber = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).Item.ItemNumber;
                                ISInvoiceLine.Quantity = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).Quantity;
                                ISInvoiceLine.UnitPrice = (decimal)_line.price;
                                ISInvoice.InvoiceLines.Add(ISInvoiceLine);
                            }

                            ISInvoice.Create(objConn, objTran);
                            objTran.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        if(ISPurchaseOrder == null)
                        {
                            Console.WriteLine(string.Format("{0}", ex.Message));
                        }
                        else
                        {
                            ISPurchaseOrder.ErrorMessage = ex.Message;
                            ISPurchaseOrder.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));
            }
            finally
            {
                objFilter = null;
                ISPurchaseOrder = null;
            }

            return true;
        }

        public static bool ExecuteByPONumber()
        {
            ImageSolutions.PurchaseOrder.PurchaseOrderFilter objFilter = null;
            ImageSolutions.PurchaseOrder.PurchaseOrder ISPurchaseOrder = null;
            ImageSolutions.Invoice.InvoiceFilter objInvoiceFilter = null;
            ImageSolutions.Invoice.Invoice ISInvoice = null;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                string strPoNumber = "1786309";

                ActiveWearAPI.Request request = new ActiveWearAPI.Request();
                List<ActiveWearAPI.OrderHistory.Header> orders = request.GetOrdersByPoNumber(strPoNumber, true);

                foreach (ActiveWearAPI.OrderHistory.Header _order in orders)
                {
                    try
                    {
                        objFilter = new ImageSolutions.PurchaseOrder.PurchaseOrderFilter();
                        objFilter.PurchaseOrderNumber = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.PurchaseOrderNumber.SearchString = _order.poNumber;

                        ISPurchaseOrder = ImageSolutions.PurchaseOrder.PurchaseOrder.GetPurchaseOrder(objFilter);

                        if (ISPurchaseOrder == null)
                        {
                            throw new Exception("Purchase Order not in database");
                        }

                        objInvoiceFilter = new ImageSolutions.Invoice.InvoiceFilter();
                        objInvoiceFilter.ExternalID = new Database.Filter.StringSearch.SearchFilter();
                        objInvoiceFilter.ExternalID.SearchString = _order.invoiceNumber;
                        objInvoiceFilter.OrderNumber = new Database.Filter.StringSearch.SearchFilter();
                        objInvoiceFilter.OrderNumber.SearchString = _order.orderNumber;

                        ISInvoice = ImageSolutions.Invoice.Invoice.GetInvoice(objInvoiceFilter);

                        if (ISInvoice == null)
                        {
                            objConn = new SqlConnection(Database.DefaultConnectionString);
                            objConn.Open();
                            objTran = objConn.BeginTransaction();

                            ISInvoice = new ImageSolutions.Invoice.Invoice();
                            ISInvoice.ExternalID = _order.invoiceNumber;
                            ISInvoice.OrderNumber = _order.orderNumber;

                            ISInvoice.PurchaseOrderID = ISPurchaseOrder.PurchaseOrderID;
                            ISInvoice.InvoiceDate = _order.invoiceDate;

                            ISInvoice.InvoiceLines = new List<ImageSolutions.Invoice.InvoiceLine>();

                            foreach (ActiveWearAPI.OrderHistory.Header.Line _line in _order.lines)
                            {
                                ImageSolutions.Invoice.InvoiceLine ISInvoiceLine = new ImageSolutions.Invoice.InvoiceLine();

                                ISInvoiceLine.PurchaseOrderLineID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).PurchaseOrderLineID;
                                ISInvoiceLine.ItemID = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).ItemID;
                                //ISInvoiceLine.ItemNumber = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).Item.ItemNumber;
                                ISInvoiceLine.Quantity = ISPurchaseOrder.PurchaseOrderLines.Find(m => m.Item.ExternalID == _line.sku).Quantity;
                                ISInvoiceLine.UnitPrice = (decimal)_line.price;
                                ISInvoice.InvoiceLines.Add(ISInvoiceLine);
                            }

                            ISInvoice.Create(objConn, objTran);
                            objTran.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ISPurchaseOrder == null)
                        {
                            Console.WriteLine(string.Format("{0}", ex.Message));
                        }
                        else
                        {
                            ISPurchaseOrder.ErrorMessage = ex.Message;
                            ISPurchaseOrder.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("{0}", ex.Message));
            }
            finally
            {
                objFilter = null;
                ISPurchaseOrder = null;
            }

            return true;
        }
    }
}
