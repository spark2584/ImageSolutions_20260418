using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class OrderConfirmation : BasePageUserAccountAuth
    {
        protected string mSalesOrderID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mSalesOrderID = Request.QueryString.Get("SalesOrderID");

            if (!Page.IsPostBack)
            {
                BindSalesOrder();
                BindFulfillment();
                BindRMA();
            }

            base.HideBreadcrumb = true;
        }

        protected void BindSalesOrder()
        {
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;

            try
            {
                if (!string.IsNullOrEmpty(mSalesOrderID))
                {
                    objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);
                    gvSalesOrderLine.DataSource = objSalesOrder.SalesOrderLines;
                    gvSalesOrderLine.DataBind();
                    gvSalesOrderLine.Columns[4].Visible = objSalesOrder.DisplayTariffCharge;

                    this.lblOrderNumber.Text = objSalesOrder.SalesOrderID;
                    this.lblOrderDate.Text = objSalesOrder.CreatedOn.ToShortDateString();
                    this.lblSubtotal.Text = string.Format("{0:c}", objSalesOrder.LineTotal);
                    this.lblDiscountAmount.Text = string.Format("{0:c}", objSalesOrder.DiscountAmount * -1);
                    //this.lblShipping.Text = string.Format("{0:c}", objSalesOrder.ShippingAmount);
                    //this.lblTax.Text = string.Format("{0:c}", objSalesOrder.TaxAmount);

                    if (objSalesOrder.BudgetShippingAmount == 0)
                    {
                        this.litShipping.Text = string.Format(@"<span id=""spanShipping"">{0:c}</span>", objSalesOrder.ShippingAmount);
                    }
                    else
                    {
                        this.litShipping.Text = string.Format(@"<span id=""spanShipping"">{0:c}</span><br>-Employee: {1:c}<br>-Employer: {2:c}", objSalesOrder.ShippingAmount, objSalesOrder.ShippingAmount + objSalesOrder.BudgetShippingAmount, objSalesOrder.BudgetShippingAmount *-1);
                    }
                    if (objSalesOrder.BudgetTaxAmount == 0)
                    {
                        this.litTax.Text = string.Format(@"<span id=""spanTax"">{0:c}</span>", objSalesOrder.TaxAmount + objSalesOrder.IPDDutiesAndTaxesAmount);
                    }
                    else
                    {
                        this.litTax.Text = string.Format(@"<span id=""spanTax"">{0:c}</span><br>-Employee: {1:c}<br>-Employer: {2:c}"
                            , objSalesOrder.TaxAmount + objSalesOrder.IPDDutiesAndTaxesAmount
                            , objSalesOrder.TaxAmount + objSalesOrder.IPDDutiesAndTaxesAmount + objSalesOrder.BudgetTaxAmount
                            , objSalesOrder.BudgetTaxAmount *-1);
                    }

                    this.lblTotal.Text = string.Format("{0:c}", objSalesOrder.Total);
                    this.lblStatus.Text = objSalesOrder.IsPendingApproval ? "Pending Approval" : objSalesOrder.Status; // "Approved";
                    this.lblReferenceNumber.Text = objSalesOrder.SalesOrderNumber;

                    if (!string.IsNullOrEmpty(objSalesOrder.TermPaymentPONumber))
                    {
                        this.pnlPONumber.Visible = true;
                        this.lblPONumber.Text = objSalesOrder.TermPaymentPONumber;
                    }
                    else
                    {
                        this.pnlPONumber.Visible = false;
                    }

                    if (objSalesOrder.Payments != null && objSalesOrder.Payments.Count > 0)
                    {
                        //if (objSalesOrder.Payments[0].BudgetAssignment != null)
                        //{
                        //    this.lblPayment.Text = "Budget: " + objSalesOrder.Payments[0].BudgetAssignment.Budget.BudgetName;
                        //}
                        //else if (objSalesOrder.Payments[0].CreditCardTransactionLog != null)
                        //{
                        //    //this.lblPayment.Text = "Credit Card: " + objSalesOrder.Payments[0].CreditCardTransactionLog.CCType + " XXXXXXXXXXXX" + objSalesOrder.Payments[0].CreditCardTransactionLog.CCLastFourNumber;
                        //    this.lblPayment.Text = "Credit Card";
                        //}

                        foreach (ImageSolutions.Payment.Payment _Payment in objSalesOrder.Payments)
                        {
                            string strPayment = string.Empty;

                            if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.CreditCard)
                            {
                                strPayment = "Credit Card " + string.Format("{0:c}", _Payment.AmountPaid);
                            }
                            else if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.Budget
                                && _Payment.BudgetAssignment != null && _Payment.BudgetAssignment.Budget != null)
                            {
                                strPayment = "Budget: " + Convert.ToString(_Payment.BudgetAssignment.Budget.BudgetName) + string.Format("{0:c}", _Payment.AmountPaid);
                            }
                            else if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.Invoice)
                            {
                                strPayment = "Invoice: " + Convert.ToString(_Payment.PaymentTerm.Description) + string.Format("{0:c}", _Payment.AmountPaid);
                            }
                            else if (_Payment.PaymentSource == ImageSolutions.Payment.Payment.enumPaymentSource.Promotion)
                            {
                                strPayment = "Promotion: " + Convert.ToString(_Payment.Promotion.PromotionName) + string.Format("{0:c}", _Payment.AmountPaid);
                            }
                            this.lblPayment.Text = string.IsNullOrEmpty(lblPayment.Text) ? strPayment : string.Format("{0}<br />{1}", lblPayment.Text, strPayment);
                        }
                    }

                    if (this.gvSalesOrderLine.HeaderRow != null) this.gvSalesOrderLine.HeaderRow.TableSection = TableRowSection.TableHeader;

                    try
                    {
                        liShippingName.InnerText = objSalesOrder.DeliveryAddress.FullName; ;
                        liShippingAddress.InnerText = objSalesOrder.DeliveryAddress.AddressLine1;
                        liShippingAddress2.InnerText = objSalesOrder.DeliveryAddress.AddressLine2;
                        liShippingCityandState.InnerText = string.Format("{0}, {1} {2}", objSalesOrder.DeliveryAddress.City, objSalesOrder.DeliveryAddress.State, objSalesOrder.DeliveryAddress.PostalCode);
                        liShippingNumber.InnerText = objSalesOrder.DeliveryAddress.PhoneNumber;

                        liBillingName.InnerText = objSalesOrder.BillingAddress.FullName;
                        liBillingAddress.InnerText = objSalesOrder.BillingAddress.AddressLine1;
                        liBillingCityandState.InnerText = string.Format("{0}, {1} {2}", objSalesOrder.BillingAddress.City, objSalesOrder.BillingAddress.State, objSalesOrder.BillingAddress.PostalCode);
                        liBillingNumber.InnerText = objSalesOrder.BillingAddress.PhoneNumber;

                        this.lblShippingMethod.Text = string.IsNullOrEmpty(objSalesOrder.WebsiteShippingServiceID) ? String.Empty : objSalesOrder.WebsiteShippingService.ShippingService.ServiceName;

                        if(objSalesOrder.IsPendingApproval)
                        {
                            litPaymentMessage.Text = "Payment is successfully processsed and your order is <b>pending approval</b>";
                        }
                        else if(objSalesOrder.IsPendingItemPersonalizationApproval)
                        {
                            litPaymentMessage.Text = "Payment is successfully processsed and your order is <b>pending personalization approval</b>";
                        }

                        litTransactionID.Text = String.Format("Transaction ID: {0}", objSalesOrder.SalesOrderID);


                        if (string.IsNullOrEmpty(objSalesOrder.RejectionReason))
                        {
                            pnlRejectionReason.Visible = false;
                        }
                        else
                        {
                            pnlRejectionReason.Visible = true;
                            lblRejectionReason.Text = objSalesOrder.RejectionReason;
                        }
                    }
                    catch { }
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

        protected void BindFulfillment()
        {
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;

            try
            {
                if (!string.IsNullOrEmpty(mSalesOrderID))
                {
                    objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);
                    gvFulfillment.DataSource = objSalesOrder.Fulfillments;
                    gvFulfillment.DataBind();
                    
                    if (this.gvFulfillment.HeaderRow != null) this.gvFulfillment.HeaderRow.TableSection = TableRowSection.TableHeader;
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

        protected void BindRMA()
        {
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;

            try
            {
                if (!string.IsNullOrEmpty(mSalesOrderID))
                {
                    objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);
                    gvRMA.DataSource = objSalesOrder.RMAs;
                    gvRMA.DataBind();

                    if (this.gvRMA.HeaderRow != null) this.gvRMA.HeaderRow.TableSection = TableRowSection.TableHeader;

                    gvRMA.Columns[3].Visible = CurrentUser.IsSuperAdmin;
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
        protected void gvSalesOrderLine_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ImageSolutions.SalesOrder.SalesOrder objSalesOrder = null;
            string strSalesOrderID = string.Empty;

            try
            {
                strSalesOrderID = Convert.ToString(e.CommandArgument);
                objSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(strSalesOrderID);

                if (objSalesOrder != null)
                {
                    //if (e.CommandName == "ViewOrder")
                    //{
                    //    Response.Redirect(string.Format("/OrderConfirmation.aspx?SalesOrderID={0}", strSalesOrderID));
                    //    //objAddressBook.Update();
                    //}
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

            Response.Redirect(string.Format("/OrderConfirmation.aspx?SalesOrderID={0}", mSalesOrderID));
        }

        protected void gvRMA_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strRMAID = gvRMA.DataKeys[e.Row.RowIndex].Value.ToString();
                    ImageSolutions.RMA.RMA RMA = new ImageSolutions.RMA.RMA(strRMAID);

                    Panel pnlShippingLabel = (Panel)e.Row.FindControl("pnlShippingLabel");

                    pnlShippingLabel.Visible = !string.IsNullOrEmpty(RMA.ShippingLabelPath);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void gvRMA_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            ImageSolutions.RMA.RMA RMA = null;
            string strRMAID = string.Empty;

            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                strRMAID = Convert.ToString(e.CommandArgument);
                RMA = new ImageSolutions.RMA.RMA(strRMAID);

                if (RMA != null)
                {
                    if (e.CommandName == "DeleteRMA")
                    {
                        if (string.IsNullOrEmpty(hfDeleteMessage.Value))
                        {
                            hfDeleteMessage.Value = "Message Displayed";
                            throw new Exception("Please note that deleted RMA must be closed in NetSuite.  Delete again to proceed.");
                        }

                        objConn = new SqlConnection(Database.DefaultConnectionString);
                        objConn.Open();
                        objTran = objConn.BeginTransaction();

                        foreach (ImageSolutions.RMA.RMALine _RMALine in RMA.RMALines)
                        {
                            _RMALine.Delete(objConn, objTran);
                        }

                        RMA.Delete(objConn, objTran);

                        objTran.Commit();

                        Response.Redirect(String.Format("/OrderConfirmation.aspx?SalesOrderID={0}", mSalesOrderID));
                    }
                }
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                }

                WebUtility.DisplayJavascriptMessage(this, string.Format(@"{0}", ex.Message));
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;

                RMA = null;
            }
        }
    }
}