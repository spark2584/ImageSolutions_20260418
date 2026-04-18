using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class ReturnAuthorization : BasePageUserAccountAuth
    {
        protected string mSalesOrderID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mSalesOrderID = Request.QueryString.Get("SalesOrderID");

            if (string.IsNullOrEmpty(mSalesOrderID))
            {
                Response.Redirect("/myaccount/dashboard.aspx");
            }

            if (!Page.IsPostBack)
            {
                BindSalesOrder();
                BindRMAReason();
            }

            base.HideBreadcrumb = true;

            pnlUseMyOwn.Visible = pnlUseMyOwn.Visible = rbnUseMyOwn.Checked;
        }

        protected void BindSalesOrder()
        {
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;

            try
            {
                if (!string.IsNullOrEmpty(mSalesOrderID))
                {
                    objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);

                    if (objSalesOrder.SalesOrderLines.FindAll(x => x.RMAAvailableQuantity > 0) != null && objSalesOrder.SalesOrderLines.FindAll(x => x.RMAAvailableQuantity > 0).Count > 0)
                    {
                        gvSalesOrderLine.DataSource = objSalesOrder.SalesOrderLines.FindAll(x => x.RMAAvailableQuantity > 0);
                        gvSalesOrderLine.DataBind();

                        this.lblOrderNumber.Text = objSalesOrder.SalesOrderID;
                        this.lblOrderDate.Text = objSalesOrder.CreatedOn.ToShortDateString();
                        //this.lblSubtotal.Text = string.Format("{0:c}", objSalesOrder.LineTotal);


                        //if (objSalesOrder.BudgetShippingAmount == 0)
                        //{
                        //    this.litShipping.Text = string.Format(@"<span id=""spanShipping"">{0:c}</span>", objSalesOrder.ShippingAmount);
                        //}
                        //else
                        //{
                        //    this.litShipping.Text = string.Format(@"<span id=""spanShipping"">{0:c}</span><br>-Employee: {1:c}<br>-Employer: {2:c}", objSalesOrder.ShippingAmount, objSalesOrder.ShippingAmount + objSalesOrder.BudgetShippingAmount, objSalesOrder.BudgetShippingAmount * -1);
                        //}
                        //if (objSalesOrder.BudgetTaxAmount == 0)
                        //{
                        //    this.litTax.Text = string.Format(@"<span id=""spanTax"">{0:c}</span>", objSalesOrder.TaxAmount);
                        //}
                        //else
                        //{
                        //    this.litTax.Text = string.Format(@"<span id=""spanTax"">{0:c}</span><br>-Employee: {1:c}<br>-Employer: {2:c}", objSalesOrder.TaxAmount, objSalesOrder.TaxAmount + objSalesOrder.BudgetTaxAmount, objSalesOrder.BudgetTaxAmount * -1);
                        //}

                        //this.lblTotal.Text = string.Format("{0:c}", objSalesOrder.Total);
                        //this.lblStatus.Text = objSalesOrder.IsPendingApproval ? "Pending Approval" : objSalesOrder.Status; // "Approved";
                        //this.lblReferenceNumber.Text = objSalesOrder.SalesOrderNumber;

                        //if (!string.IsNullOrEmpty(objSalesOrder.TermPaymentPONumber))
                        //{
                        //    this.pnlPONumber.Visible = true;
                        //    this.lblPONumber.Text = objSalesOrder.TermPaymentPONumber;
                        //}
                        //else
                        //{
                        //    this.pnlPONumber.Visible = false;
                        //}

                        //if (objSalesOrder.Payments != null && objSalesOrder.Payments.Count > 0)
                        //{
                        //    foreach (ImageSolutions.Payment.Payment _Payment in objSalesOrder.Payments)
                        //    {
                        //        string strPayment = string.Empty;

                        //        if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.CreditCard)
                        //        {
                        //            strPayment = "Credit Card " + string.Format("{0:c}", _Payment.AmountPaid);
                        //        }
                        //        else if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.Budget
                        //            && _Payment.BudgetAssignment != null && _Payment.BudgetAssignment.Budget != null)
                        //        {
                        //            strPayment = "Budget: " + Convert.ToString(_Payment.BudgetAssignment.Budget.BudgetName) + string.Format("{0:c}", _Payment.AmountPaid);
                        //        }
                        //        else if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.Invoice)
                        //        {
                        //            strPayment = "Invoice: " + Convert.ToString(_Payment.PaymentTerm.Description) + string.Format("{0:c}", _Payment.AmountPaid);
                        //        }
                        //        else if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.Promotion)
                        //        {
                        //            strPayment = "Promotion: " + Convert.ToString(_Payment.Promotion.PromotionName) + string.Format("{0:c}", _Payment.AmountPaid);
                        //        }
                        //        this.lblPayment.Text = string.IsNullOrEmpty(lblPayment.Text) ? strPayment : string.Format("{0}<br />{1}", lblPayment.Text, strPayment);
                        //    }
                        //}

                        //if (this.gvSalesOrderLine.HeaderRow != null) this.gvSalesOrderLine.HeaderRow.TableSection = TableRowSection.TableHeader;

                        try
                        {
                            liShippingName.InnerText = objSalesOrder.DeliveryAddress.FullName; ;
                            liShippingAddress.InnerText = objSalesOrder.DeliveryAddress.AddressLine1;
                            liShippingAddress2.InnerText = objSalesOrder.DeliveryAddress.AddressLine2;
                            liShippingCityandState.InnerText = string.Format("{0}, {1} {2}", objSalesOrder.DeliveryAddress.City, objSalesOrder.DeliveryAddress.State, objSalesOrder.DeliveryAddress.PostalCode);
                            liShippingNumber.InnerText = objSalesOrder.DeliveryAddress.PhoneNumber;

                            //liBillingName.InnerText = objSalesOrder.BillingAddress.FullName;
                            //liBillingAddress.InnerText = objSalesOrder.BillingAddress.AddressLine1;
                            //liBillingCityandState.InnerText = string.Format("{0}, {1} {2}", objSalesOrder.BillingAddress.City, objSalesOrder.BillingAddress.State, objSalesOrder.BillingAddress.PostalCode);
                            //liBillingNumber.InnerText = objSalesOrder.BillingAddress.PhoneNumber;

                            //this.lblShippingMethod.Text = objSalesOrder.WebsiteShippingServiceID == null ? String.Empty : objSalesOrder.WebsiteShippingService.ShippingService.ServiceName;

                            //if (objSalesOrder.IsPendingApproval)
                            //{
                            //    litPaymentMessage.Text = "Payment is successfully processsed and your order is <b>pending approval</b>";
                            //}
                            //else if (objSalesOrder.IsPendingItemPersonalizationApproval)
                            //{
                            //    litPaymentMessage.Text = "Payment is successfully processsed and your order is <b>pending personalization approval</b>";
                            //}

                            //litTransactionID.Text = String.Format("Transaction ID: {0}", objSalesOrder.SalesOrderID);

                        }
                        catch { }
                    }
                    else
                    {
                        Response.Redirect("/myaccount/dashboard.aspx");
                    }
                }
                else
                {
                    Response.Redirect("/myaccount/dashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objSalesOrder = null;
            }
        }

        protected void BindRMAReason()
        {
            List<ImageSolutions.RMA.RMAReason> RMAReasons = null;

            try
            {
                RMAReasons = new List<ImageSolutions.RMA.RMAReason>();
                RMAReasons = ImageSolutions.RMA.RMAReason.GetRMAReasons();

                ddlReason.Items.Add(new ListItem("-- Please select the reason for return --", string.Empty));
                foreach (ImageSolutions.RMA.RMAReason _RMAReason in RMAReasons)
                {
                    ddlReason.Items.Add(new ListItem(_RMAReason.Reason, _RMAReason.RMAReasonID));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                RMAReasons = null;
            }
        }

        protected void gvSalesOrderLine_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            ImageSolutions.RMA.RMA RMA = null;
            ImageSolutions.SalesOrder.SalesOrder SalesOrder = null;

            try
            {
                if (string.IsNullOrEmpty(hfSubmitMessage.Value))
                {
                    hfSubmitMessage.Value = "Message Displayed";
                    throw new Exception("Please include your RMA number on your return label. Packages missing RMA numbers can not be processed.  Please Submit again to proceed.");
                }

                if (string.IsNullOrEmpty(ddlReason.SelectedValue))
                {
                    throw new Exception("Please specify the reason for return.");
                }

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);

                foreach (GridViewRow objRow in this.gvSalesOrderLine.Rows)
                {
                    ImageSolutions.SalesOrder.SalesOrderLine SalesOrderLine = new ImageSolutions.SalesOrder.SalesOrderLine(this.gvSalesOrderLine.DataKeys[objRow.RowIndex].Values["SalesOrderLineID"].ToString());
                    string strQuantity = String.IsNullOrEmpty(((TextBox)objRow.FindControl("txtReturnQuantity")).Text) ? "0" : ((TextBox)objRow.FindControl("txtReturnQuantity")).Text;

                    if (Convert.ToInt32(strQuantity) > SalesOrderLine.RMAAvailableQuantity)
                    {
                        throw new Exception("Return Quantity cannot be greater than Ordered Quantity");
                    }

                    if (!string.IsNullOrEmpty(strQuantity) && Convert.ToInt32(strQuantity) > 0)
                    {
                        if (RMA == null)
                        {
                            RMA = new ImageSolutions.RMA.RMA();
                            RMA.WebsiteID = SalesOrder.WebsiteID;
                            RMA.SalesOrderID = SalesOrder.SalesOrderID;
                            RMA.TransactionDate = DateTime.UtcNow;
                            RMA.RequestReturnLabel = rbnRequestReturnLabel.Checked;
                            RMA.Reason = ddlReason.SelectedItem.Text;
                            RMA.RMALines = new List<ImageSolutions.RMA.RMALine>();
                        }

                        ImageSolutions.RMA.RMALine RMALine = new ImageSolutions.RMA.RMALine();
                        RMALine.SalesOrderLineID = SalesOrderLine.SalesOrderLineID;
                        RMALine.ItemID = SalesOrderLine.ItemID;
                        RMALine.Quantity = Convert.ToInt32(strQuantity);
                        RMALine.UnitPrice = SalesOrderLine.UnitPrice;
                        RMA.RMALines.Add(RMALine);                       
                    }
                }

                if (RMA != null)
                {
                    if (rbnRequestReturnLabel.Checked)
                    {
                        // Generate Shipping Label 
                        ImageSolutions.Shipping.ShipHawkShipment ShipHawkShipment = CreateShipment(RMA);

                        Console.WriteLine("Sleeping for 10 seconds to let SHipHwk generate PDF");
                        System.Threading.Thread.Sleep(10000);

                        ImageSolutions.Shipping.ShipHawkFinalResponse ShipHawkFinalResponse = GetShipmentDocuments(ShipHawkShipment);

                        if (ShipHawkFinalResponse != null)
                        {
                            RMA.ReferenceNumber = ShipHawkFinalResponse.ShipHawkID;
                            RMA.ShippingLabelPath = ShipHawkFinalResponse.ShippingLabel;
                            RMA.TrackingNumber = ShipHawkFinalResponse.TrackingNumber;

                            ImageSolutions.RMA.RMAReason RMAReason = new ImageSolutions.RMA.RMAReason(ddlReason.SelectedValue);
                            if (RMAReason.IncludeShipping)
                            {
                                RMA.ShippingAmount = Convert.ToDouble(ShipHawkFinalResponse.Cost);
                            }
                        }
                        else
                        {
                            throw new Exception("Label cannot be created.");
                        }
                    }
                    RMA.Create(objConn, objTran);

                }
                else
                {
                    throw new Exception("Please specify Returned Quantity");
                }

                objTran.Commit();

                Response.Redirect(String.Format("/ReturnAuthorizationConfirmation.aspx?rmaid={0}", RMA.RMAID));
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                }

                rbnUseMyOwn.Checked = true;
                rbnRequestReturnLabel.Checked = false;
                pnlShippingCost.Visible = false;
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;

                if (RMA != null) RMA = null;
                if (SalesOrder != null) SalesOrder = null;
            }
        }

        public ImageSolutions.Shipping.ShipHawkFinalResponse GetShipmentDocuments(ImageSolutions.Shipping.ShipHawkShipment shiphawkshipment)
        {
            //hard code items for now
            ImageSolutions.Shipping.ShipHawkFinalResponse ret = null;
            RestSharp.Authenticators.OAuth1Authenticator objAuthenticator = null;
            RestClient objClient = null;
            RestRequest objRequest = null;
            string strResponse = string.Empty;
            string strPackingSlipURL = string.Empty;
            string strShippingLabel = string.Empty;
            try
            {

                objClient = new RestClient("https://shiphawk.com");

                objRequest = new RestRequest(string.Format("/api/v4/shipments/{0}/documents?api_key=93abde8691bbbae8c4960479d85465ff", shiphawkshipment.shid), Method.Get);

                RestResponse objResponse = objClient.Execute(objRequest);
                strResponse = objResponse.Content;

                List<ImageSolutions.Shipping.ShiphawkShipmentDocument > objResponseMessage = JsonSerializer.Deserialize<List<ImageSolutions.Shipping.ShiphawkShipmentDocument>>(objResponse.Content);

                if (objResponseMessage != null)
                {
                    strPackingSlipURL = objResponseMessage.ToList().Find(m => m.code == "packing_slip" && m.extension == "PDF").url;

                    strShippingLabel = objResponseMessage.ToList().Find(m => m.code == "package_labels_combined" && m.extension == "PDF").url;

                }

                ret = new ImageSolutions.Shipping.ShipHawkFinalResponse();
                ret.ShipHawkID = Convert.ToString(shiphawkshipment.shid);
                ret.ShippingLabel = strShippingLabel;
                ret.PackingSlip = strPackingSlipURL;
                ret.TrackingNumber = shiphawkshipment.tracking_number;
                ret.Cost = Convert.ToDecimal(shiphawkshipment.total_price);
            }
            catch (Exception ex)
            {
            }


            return ret;
        }

        public ImageSolutions.Shipping.ShipHawkShipment CreateShipment(ImageSolutions.RMA.RMA rma)
        {
            //hard code items for now
            ImageSolutions.Shipping.ShipHawkShipment ret = null;
            RestSharp.Authenticators.OAuth1Authenticator objAuthenticator = null;
            RestClient objClient = null;
            RestRequest objRequest = null;
            string strResponse = string.Empty;

            try
            {
                ImageSolutions.SalesOrder.SalesOrder Salesorder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);
                //get items
                List<ImageSolutions.Shipping.ShipHawkItem> ShipHawkItems = new List<ImageSolutions.Shipping.ShipHawkItem>();
                foreach (ImageSolutions.RMA.RMALine _RMALine in rma.RMALines)
                {
                    ImageSolutions.Shipping.ShipHawkItem ShipHawkItem = GetItems(_RMALine.Item.ItemNumber);
                    ShipHawkItem.items[0].quantity = _RMALine.Quantity;
                    ShipHawkItem.items[0].product_sku = _RMALine.Item.ItemNumber;
                    ShipHawkItems.Add(ShipHawkItem);
                }

                //get rateid
                ImageSolutions.Shipping.Rate ShipHawkRate = GetRates(ShipHawkItems);


                ImageSolutions.RMA.RMAReason RMAReason = new ImageSolutions.RMA.RMAReason(ddlReason.SelectedValue);
                if (RMAReason.IncludeShipping)
                {
                    if (ShipHawkRate.price != hfShippingCost.Value)
                    {
                        GetShippingCost();

                        rbnUseMyOwn.Checked = false;
                        rbnRequestReturnLabel.Checked = true;
                        pnlShippingCost.Visible = true;

                        throw new Exception("Shopping Cost has been updated.");
                    }
                }

                ImageSolutions.Shipping.ShipHawkCreateShipmentBody shipHawkCreateShipmentBody = new ImageSolutions.Shipping.ShipHawkCreateShipmentBody();
                shipHawkCreateShipmentBody.rate_id = ShipHawkRate.id;
                shipHawkCreateShipmentBody.order_number = rma.SalesOrderID;

                shipHawkCreateShipmentBody.destination_address = new ImageSolutions.Shipping.Destination_Address();
                shipHawkCreateShipmentBody.destination_address.name = CurrentWebsite.DeliveryAddress.CompanyName ;
                shipHawkCreateShipmentBody.destination_address.street1 = CurrentWebsite.DeliveryAddress.AddressLine1;
                shipHawkCreateShipmentBody.destination_address.city = CurrentWebsite.DeliveryAddress.City;
                shipHawkCreateShipmentBody.destination_address.state = CurrentWebsite.DeliveryAddress.State;
                shipHawkCreateShipmentBody.destination_address.zip = CurrentWebsite.DeliveryAddress.PostalCode;
                shipHawkCreateShipmentBody.destination_address.country = CurrentWebsite.DeliveryAddress.CountryCode;
                shipHawkCreateShipmentBody.destination_address.phone_number = CurrentWebsite.DeliveryAddress.PhoneNumber;

                shipHawkCreateShipmentBody.origin_address = new ImageSolutions.Shipping.Origin_Address();
                shipHawkCreateShipmentBody.origin_address.name = String.IsNullOrEmpty(Salesorder.DeliveryAddress.AddressLabel) ? Salesorder.DeliveryAddress.FirstName + " " + Salesorder.DeliveryAddress.LastName : Salesorder.DeliveryAddress.AddressLabel;
                shipHawkCreateShipmentBody.origin_address.street1 = Salesorder.DeliveryAddress.AddressLine1;
                shipHawkCreateShipmentBody.origin_address.street2 = Salesorder.DeliveryAddress.AddressLine2;
                shipHawkCreateShipmentBody.origin_address.city = Salesorder.DeliveryAddress.City;
                shipHawkCreateShipmentBody.origin_address.state = Salesorder.DeliveryAddress.State;
                shipHawkCreateShipmentBody.origin_address.zip = Salesorder.DeliveryAddress.PostalCode;
                shipHawkCreateShipmentBody.origin_address.country = Salesorder.DeliveryAddress.CountryCode;
                shipHawkCreateShipmentBody.origin_address.phone_number = Salesorder.DeliveryAddress.PhoneNumber;

                shipHawkCreateShipmentBody.packages = new ImageSolutions.Shipping.package[1];
                shipHawkCreateShipmentBody.packages[0] = new ImageSolutions.Shipping.package();
                shipHawkCreateShipmentBody.packages[0].length = "10";
                shipHawkCreateShipmentBody.packages[0].weight = "10";
                shipHawkCreateShipmentBody.packages[0].height = "10";
                shipHawkCreateShipmentBody.packages[0].packing_type = "boxed";
                //shipHawkCreateShipmentBody.packages[0].value = 10;
                //shipHawkCreateShipmentBody.packages[0].package_items = new ImageSolutions.Shipping.package_item[1];
                //shipHawkCreateShipmentBody.packages[0].package_items[0] = new ImageSolutions.Shipping.package_item();
                //shipHawkCreateShipmentBody.packages[0].package_items[0].product_sku = "# Mouse-Pad-8";
                //shipHawkCreateShipmentBody.packages[0].package_items[0].country_of_origin = "US";
                //shipHawkCreateShipmentBody.packages[0].package_items[0].hs_code = "6109.10.0012";
                //shipHawkCreateShipmentBody.packages[0].package_items[0].value = 10;
                //shipHawkCreateShipmentBody.packages[0].package_items[0].quantity = 1;
                //shipHawkCreateShipmentBody.packages[0].package_items[0].description = "mouse pad";
                //shipHawkCreateShipmentBody.packages[0].package_items[0].item_name = "# Mouse-Pad-8";
                //shipHawkCreateShipmentBody.packages[0].package_items[0].weight = 10;

                shipHawkCreateShipmentBody.packages[0].package_items = new ImageSolutions.Shipping.package_item[rma.RMALines.Count];

                int counter = 0;

                foreach (ImageSolutions.Shipping.ShipHawkItem _ShipHawkItem in ShipHawkItems)
                {
                    shipHawkCreateShipmentBody.packages[0].package_items[counter] = new ImageSolutions.Shipping.package_item();
                    shipHawkCreateShipmentBody.packages[0].package_items[counter].product_sku = _ShipHawkItem.items[0].product_sku;
                    //shipHawkCreateShipmentBody.packages[0].package_items[0].country_of_origin = "US";
                    //shipHawkCreateShipmentBody.packages[0].package_items[0].hs_code = "6109.10.0012";
                    //shipHawkCreateShipmentBody.packages[0].package_items[0].value = 10;
                    shipHawkCreateShipmentBody.packages[0].package_items[counter].quantity = _ShipHawkItem.items[0].quantity;
                    //shipHawkCreateShipmentBody.packages[0].package_items[0].description = "mouse pad";
                    //shipHawkCreateShipmentBody.packages[0].package_items[0].item_name = "# Mouse-Pad-8";
                    //shipHawkCreateShipmentBody.packages[0].package_items[0].weight = 10;
                    counter++;
                }

                //ImageSolutions.Shipping.ShipHawkCreateProposedShipment ShipHawkCreateProposedShipment = new ImageSolutions.Shipping.ShipHawkCreateProposedShipment();
                //ShipHawkCreateProposedShipment.destination_address = new ImageSolutions.Shipping.Destination_Address();
                //ShipHawkCreateProposedShipment.destination_address.name = CurrentWebsite.DeliveryAddress.CompanyName;
                //ShipHawkCreateProposedShipment.destination_address.street1 = CurrentWebsite.DeliveryAddress.AddressLine1;
                //ShipHawkCreateProposedShipment.destination_address.city = CurrentWebsite.DeliveryAddress.City;
                //ShipHawkCreateProposedShipment.destination_address.state = CurrentWebsite.DeliveryAddress.State;
                //ShipHawkCreateProposedShipment.destination_address.zip = CurrentWebsite.DeliveryAddress.PostalCode;
                //ShipHawkCreateProposedShipment.destination_address.country = CurrentWebsite.DeliveryAddress.CountryCode;
                //ShipHawkCreateProposedShipment.destination_address.phone_number = CurrentWebsite.DeliveryAddress.PhoneNumber;

                //ShipHawkCreateProposedShipment.origin_address = new ImageSolutions.Shipping.Origin_Address();
                //ShipHawkCreateProposedShipment.origin_address.name = String.IsNullOrEmpty(Salesorder.DeliveryAddress.AddressLabel) ? Salesorder.DeliveryAddress.FirstName + " " + Salesorder.DeliveryAddress.LastName : Salesorder.DeliveryAddress.AddressLabel;
                //ShipHawkCreateProposedShipment.origin_address.street1 = Salesorder.DeliveryAddress.AddressLine1;
                //ShipHawkCreateProposedShipment.origin_address.street2 = Salesorder.DeliveryAddress.AddressLine2;
                //ShipHawkCreateProposedShipment.origin_address.city = Salesorder.DeliveryAddress.City;
                //ShipHawkCreateProposedShipment.origin_address.state = Salesorder.DeliveryAddress.State;
                //ShipHawkCreateProposedShipment.origin_address.zip = Salesorder.DeliveryAddress.PostalCode;
                //ShipHawkCreateProposedShipment.origin_address.country = Salesorder.DeliveryAddress.CountryCode;
                //ShipHawkCreateProposedShipment.origin_address.phone_number = Salesorder.DeliveryAddress.PhoneNumber;

                //ShipHawkCreateProposedShipment.packages = new List<ImageSolutions.Shipping.Package>();
                //ImageSolutions.Shipping.Package Package = new ImageSolutions.Shipping.Package();
                //Package.packing_type = "parcel";
                //Package.number_of_units = 1;
                //Package.value = 20;
                //Package.weight = 2;
                ////Package.weight_uom
                //ShipHawkCreateProposedShipment.packages.Add(Package);

                //ShipHawkCreateProposedShipment.shipment_line_items = new List<ImageSolutions.Shipping.Shipment_Line_Items>();
                //ImageSolutions.Shipping.Shipment_Line_Items Shipment_Line_Items = new ImageSolutions.Shipping.Shipment_Line_Items();

                //Shipment_Line_Items.hs_code = "3926.90.9050";
                //Shipment_Line_Items.country_of_origin = "VI";

                //ShipHawkCreateProposedShipment.shipment_line_items.Add(Shipment_Line_Items);

                //objClient = new RestClient("https://shiphawk.com");

                //objRequest = new RestRequest("/api/v4/shipments?api_key=93abde8691bbbae8c4960479d85465ff", Method.Post);
                //objRequest.AddBody(shipHawkCreateShipmentBody);

                //Get SKU
                //https://docs.shiphawk.com/?shell#get-sku
                objClient = new RestClient("https://shiphawk.com");

                objRequest = new RestRequest("/api/v4/shipments?api_key=93abde8691bbbae8c4960479d85465ff", Method.Post);
                objRequest.AddBody(shipHawkCreateShipmentBody);

                RestResponse objResponse = objClient.Execute(objRequest);
                strResponse = objResponse.Content;

                ImageSolutions.Shipping.ShipHawkShipment objResponseMessage = JsonSerializer.Deserialize<ImageSolutions.Shipping.ShipHawkShipment>(objResponse.Content);

                if (objResponseMessage != null)
                {
                    Console.WriteLine(string.Format("{0}", objResponseMessage.id));
                }

                ret = objResponseMessage;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }
        public ImageSolutions.Shipping.Rate GetRates(List<ImageSolutions.Shipping.ShipHawkItem> shiphawkitems)
        {
            ImageSolutions.Shipping.Rate ret = null;
            RestSharp.Authenticators.OAuth1Authenticator objAuthenticator = null;
            RestClient objClient = null;
            RestRequest objRequest = null;
            string strResponse = string.Empty;

            try
            {
                ImageSolutions.SalesOrder.SalesOrder SalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);

                //Box size and weight
                //need to figure out how to get dimensions size for rate
                ImageSolutions.Shipping.ShipHawkGetRateBody shipHawkGetRateBody = new ImageSolutions.Shipping.ShipHawkGetRateBody();

                shipHawkGetRateBody.items = new ImageSolutions.Shipping.ShiphawkParcel[shiphawkitems.Count];

                decimal decTotalWeight = 0;

                for (int i = 0; i < shiphawkitems.Count; i++)
                {
                    decTotalWeight += Convert.ToDecimal(shiphawkitems[i].items[0].weight);

                    shipHawkGetRateBody.items[i] = new ImageSolutions.Shipping.ShiphawkParcel();
                    shipHawkGetRateBody.items[i].weight = Convert.ToString(shiphawkitems[i].items[0].weight);//just hard coding the one item for now
                    shipHawkGetRateBody.items[i].weight_uom = "lbs";
                    //shipHawkGetRateBody.items[i].type = "parcel";
                    //shipHawkGetRateBody.items[i].length = "10";
                    //shipHawkGetRateBody.items[i].width = "10";
                    //shipHawkGetRateBody.items[i].height = "10";
                    //shipHawkGetRateBody.items[i].item_type = "unpacked";
                    //shipHawkGetRateBody.items[i].length = "10";
                    //shipHawkGetRateBody.items[i].width = "10";
                    //shipHawkGetRateBody.items[i].height = "10";
                    shipHawkGetRateBody.items[i].quantity = shiphawkitems[i].items[0].quantity;
                    shipHawkGetRateBody.items[i].product_sku = shiphawkitems[i].items[0].product_sku; //string.IsNullOrEmpty(shiphawkitems[i].items[0].product_sku) ? "na" : shiphawkitems[i].items[0].product_sku;
                }

                //shipHawkGetRateBody.items = new ImageSolutions.Shipping.ShiphawkParcel[1];
                //shipHawkGetRateBody.items[0] = new ImageSolutions.Shipping.ShiphawkParcel();
                ////shipHawkGetRateBody.items[0].weight = Convert.ToString(shiphawkitems[0].items[0].weight);//just hard coding the one item for now
                //shipHawkGetRateBody.items[0].weight = Convert.ToString(decTotalWeight);//just hard coding the one item for now
                //shipHawkGetRateBody.items[0].weight_uom = "lbs";//just hard coding the one item for now
                //shipHawkGetRateBody.items[0].type = "parcel";
                ////shipHawkGetRateBody.items[0].product_sku = "na";
                //shipHawkGetRateBody.items[0].length = "10";
                //shipHawkGetRateBody.items[0].width = "10";
                //shipHawkGetRateBody.items[0].height = "10";
                ////shipHawkGetRateBody.items[0].item_type = "unpacked";
                //shipHawkGetRateBody.items[0].quantity = 1;

                //shipHawkGetRateBody.items[0].hs_code = "3926.90.9050";
                //shipHawkGetRateBody.items[0].country_of_origin = "VI";
                //shipHawkGetRateBody.items[0].value = 20;

                //shipHawkGetRateBody.items[0].product_sku = shiphawkitems[0].items[0].product_sku; //"# Mouse-Pad-8";

                //ImageSolutions.Shipping.HarmonizedCodeMapping HarmonizedCodeMapping = new ImageSolutions.Shipping.HarmonizedCodeMapping();
                //HarmonizedCodeMapping.harmonized_code = "3926.90.9050";
                //shipHawkGetRateBody.items[0].harmonized_codes = HarmonizedCodeMapping;

                shipHawkGetRateBody.origin_address = new ImageSolutions.Shipping.Origin_Address();
                shipHawkGetRateBody.origin_address.zip = SalesOrder.DeliveryAddress.PostalCode; //"93101"; //need logic to insert zip code her
                shipHawkGetRateBody.origin_address.country = SalesOrder.DeliveryAddress.CountryCode;

                shipHawkGetRateBody.destination_address = new ImageSolutions.Shipping.Destination_Address();
                shipHawkGetRateBody.destination_address.zip = CurrentWebsite.DeliveryAddress.PostalCode; //"45011-3558"; //OH Zip Code
                shipHawkGetRateBody.destination_address.country = CurrentWebsite.DeliveryAddress.CountryCode; //"45011-3558"; //OH Zip Code

                //Get SKU
                //https://docs.shiphawk.com/?shell#get-sku
                objClient = new RestClient("https://shiphawk.com");

                objRequest = new RestRequest("/api/v4/rates?api_key=93abde8691bbbae8c4960479d85465ff", Method.Post);
                objRequest.AddBody(shipHawkGetRateBody);

                RestResponse objResponse = objClient.Execute(objRequest);
                strResponse = objResponse.Content;

                ImageSolutions.Shipping.Rates objResponseMessage = JsonSerializer.Deserialize<ImageSolutions.Shipping.Rates>(objResponse.Content);

                if (objResponseMessage != null && objResponseMessage.rates != null)
                {
                    //need to find logic on how which shipping method to use
                    //cheapest or what
                    //for now always get fedex Ground
                    
                    if (SalesOrder.DeliveryAddress.CountryCode == "US" || string.IsNullOrEmpty(SalesOrder.DeliveryAddress.CountryCode))
                    {
                        ret = objResponseMessage.rates.ToList().Find(m => m.service_code == "FEDEX_GROUND");
                    }
                    else
                    {
                        ret = objResponseMessage.rates.ToList().Find(m => m.service_code == "INTERNATIONAL_GROUND");
                    }


                    //Console.WriteLine(string.Format("{0} - {1}", ret.service_code, ret.price));
                }

                if (ret == null)
                {
                    throw new Exception("Shipping not available");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        public ImageSolutions.Shipping.ShipHawkItem GetItems(string sku)
        {
            ImageSolutions.Shipping.ShipHawkItem ret = null;
            RestSharp.Authenticators.OAuth1Authenticator objAuthenticator = null;
            RestClient objClient = null;
            RestRequest objRequest = null;
            string strResponse = string.Empty;

            try
            {
                //set items to search here
                //string strSKU = "SK702-RD-L";

                //Get SKU
                //https://docs.shiphawk.com/?shell#get-sku
                objClient = new RestClient("https://shiphawk.com");
                objRequest = new RestRequest("/api/v4/skus/detect?api_key=93abde8691bbbae8c4960479d85465ff&sku=" + sku, Method.Get);


                RestResponse objResponse = objClient.Execute(objRequest);
                strResponse = objResponse.Content;

                ImageSolutions.Shipping.ShipHawkItem objResponseMessage = JsonSerializer.Deserialize<ImageSolutions.Shipping.ShipHawkItem>(objResponse.Content);

                if (objResponseMessage != null)
                {
                    Console.WriteLine(string.Format("{0} - {1} - {2}", objResponseMessage.sku, objResponseMessage.items[0].weight, objResponseMessage.items[0].harmonized_codes));
                }

                ret = objResponseMessage;
            }
            catch (Exception ex)
            {
            }

            return ret;
        }


        protected void rbnRequestReturnLabel_CheckedChanged(object sender, EventArgs e)
        {
            //pnlRequestReturnLabel.Visible = true;
            pnlUseMyOwn.Visible = pnlUseMyOwn.Visible = rbnUseMyOwn.Checked;

            //btnSubmit.Visible = false;

            //foreach (GridViewRow objRow in this.gvSalesOrderLine.Rows)
            //{
            //    TextBox txtReturnQuantity = (TextBox)objRow.FindControl("txtReturnQuantity");
            //    txtReturnQuantity.Enabled = false;
            //}

            try
            {
                if (string.IsNullOrEmpty(ddlReason.SelectedValue))
                {
                    throw new Exception("Please select the reason for return");
                }

                pnlShippingCost.Visible = true;
                GetShippingCost();
            }
            catch (Exception ex)
            {
                rbnUseMyOwn.Checked = true;
                rbnRequestReturnLabel.Checked = false;
                pnlShippingCost.Visible = false;
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void GetShippingCost()
        {
            ImageSolutions.RMA.RMAReason RMAReason = new ImageSolutions.RMA.RMAReason(ddlReason.SelectedValue);

            if (rbnRequestReturnLabel.Checked && !RMAReason.IncludeShipping)
             {
                ddlReason.SelectedValue = String.Empty;
                throw new Exception("Please contact customer service.");
            }

            if (rbnUseMyOwn.Checked || !RMAReason.IncludeShipping)
            {
                litShippingCost.Text = String.Format(@"<br />Shipping Cost: {0:c}", 0);
                hfShippingCost.Value = Convert.ToString(0);

                pnlShippingCost.Visible = rbnRequestReturnLabel.Checked;
            }
            else
            {
                List<ImageSolutions.Shipping.ShipHawkItem> ShipHawkItems = new List<ImageSolutions.Shipping.ShipHawkItem>();

                foreach (GridViewRow objRow in this.gvSalesOrderLine.Rows)
                {
                    TextBox txtReturnQuantity = (TextBox)objRow.FindControl("txtReturnQuantity");
                    Label lblItemNumber = (Label)objRow.FindControl("lblItemNumber");
                    if (!string.IsNullOrEmpty(txtReturnQuantity.Text) && Convert.ToInt32(txtReturnQuantity.Text) > 0)
                    {
                        ImageSolutions.Shipping.ShipHawkItem ShipHawkItem = GetItems(lblItemNumber.Text);
                        ShipHawkItem.items[0].quantity = Convert.ToInt32(txtReturnQuantity.Text);
                        ShipHawkItem.items[0].product_sku = lblItemNumber.Text;
                        ShipHawkItems.Add(ShipHawkItem);

                        //ShipHawkItems.Add(GetItems(lblItemNumber.Text));
                    }
                }

                if (ShipHawkItems != null && ShipHawkItems.Count > 0)
                {
                    ImageSolutions.Shipping.Rate ShipHawkRate = GetRates(ShipHawkItems);

                    litShippingCost.Text = String.Format(@"<br />Shipping Cost: {0:c}", ShipHawkRate.price);
                    hfShippingCost.Value = ShipHawkRate.price;
                }
                else
                {
                    litShippingCost.Text = String.Format(@"<br />Shipping Cost: {0:c}", 0);
                    hfShippingCost.Value = Convert.ToString(0);
                }
            }
        }

        protected void rbnUseMyOwn_CheckedChanged(object sender, EventArgs e)
        {
            //pnlRequestReturnLabel.Visible = false;
            pnlUseMyOwn.Visible = rbnUseMyOwn.Checked;

            //foreach (GridViewRow objRow in this.gvSalesOrderLine.Rows)
            //{
            //    TextBox txtReturnQuantity = (TextBox)objRow.FindControl("txtReturnQuantity");
            //    txtReturnQuantity.Enabled = true;
            //}

            //btnRequestShippingLabel.Visible = true;
            //hlnkRequestShippingLabel.NavigateUrl = String.Empty;
            //hlnkRequestShippingLabel.Visible = false;

            //btnSubmit.Visible = true;

            pnlShippingCost.Visible = false;
            litShippingCost.Text = String.Format(@"<br />Shipping Cost: {0:c}", 0);
            hfShippingCost.Value = Convert.ToString(0);
        }

        protected void btnRequestShippingLabel_Click(object sender, EventArgs e)
        {
            int Quantity = 0;
            try
            {
                foreach (GridViewRow objRow in this.gvSalesOrderLine.Rows)
                {
                    TextBox txtReturnQuantity = (TextBox)objRow.FindControl("txtReturnQuantity");
                    txtReturnQuantity.Enabled = false;

                    Quantity += String.IsNullOrEmpty(((TextBox)objRow.FindControl("txtReturnQuantity")).Text) ? 0 : Convert.ToInt32(((TextBox)objRow.FindControl("txtReturnQuantity")).Text);
                }

                if (Quantity == 0)
                {
                    foreach (GridViewRow objRow in this.gvSalesOrderLine.Rows)
                    {
                        TextBox txtReturnQuantity = (TextBox)objRow.FindControl("txtReturnQuantity");
                        txtReturnQuantity.Enabled = true;
                    }

                    throw new Exception("Please specify Returned Quantity");
                }

                //Download return label

                //btnRequestShippingLabel.Visible = false;
                //hlnkRequestShippingLabel.Text = "click here to download";
                //hlnkRequestShippingLabel.NavigateUrl = "https://www.google.com";
                //hlnkRequestShippingLabel.Visible = true;

                btnSubmit.Visible = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void ddlReason_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetShippingCost();
            }
            catch (Exception ex)
            {
                rbnUseMyOwn.Checked = true;
                rbnRequestReturnLabel.Checked = false;
                pnlShippingCost.Visible = false;
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}