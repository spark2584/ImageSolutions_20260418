using ImageSolutions.SalesOrder;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Order : BasePageAdminUserWebSiteAuth
    {
        protected string mSalesOrderID = string.Empty;

        private ImageSolutions.SalesOrder.SalesOrder mSalesOrder = null;
        protected ImageSolutions.SalesOrder.SalesOrder _SalesOrder
        {
            get
            {
                if (mSalesOrder == null)
                {
                    mSalesOrder = new ImageSolutions.SalesOrder.SalesOrder(mSalesOrderID);
                }
                return mSalesOrder;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/OrderOverview.aspx";
                else
                    return ViewState["ReturnURL"].ToString();
            }
            set
            {
                ViewState["ReturnURL"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mSalesOrderID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                BindCustomField();

                if (!_SalesOrder.IsNew)
                {
                    gvSalesOrderLine.DataSource = _SalesOrder.SalesOrderLines;
                    gvSalesOrderLine.DataBind();
                    gvSalesOrderLine.Columns[4].Visible = _SalesOrder.DisplayTariffCharge;


                    if (!string.IsNullOrEmpty(_SalesOrder.PackageID))
                    {
                        phPackage.Visible = true;
                        txtPackage.Text = _SalesOrder.Package.Name;

                        if (!string.IsNullOrEmpty(_SalesOrder.Package.OptionalFieldText))
                        {
                            chkExcludeOptional.Visible = true;
                            chkExcludeOptional.Text = _SalesOrder.Package.OptionalFieldText;
                            chkExcludeOptional.Checked = _SalesOrder.ExcludeOptional;
                        }

                    }
                    else
                    {
                        phPackage.Visible = false;
                    }

                    this.txtUser.Text = _SalesOrder.UserWebsite.Description;

                    List<ImageSolutions.Custom.CustomField> CustomFields = new List<ImageSolutions.Custom.CustomField>();
                    ImageSolutions.Custom.CustomFieldFilter CustomFieldFilter = new ImageSolutions.Custom.CustomFieldFilter();
                    CustomFieldFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    CustomFieldFilter.WebsiteID.SearchString = _SalesOrder.WebsiteID;
                    CustomFieldFilter.Location = new Database.Filter.StringSearch.SearchFilter();
                    CustomFieldFilter.Location.SearchString = "user";
                    CustomFieldFilter.Inactive = false;
                    CustomFields = ImageSolutions.Custom.CustomField.GetCustomFields(CustomFieldFilter);

                    foreach (ImageSolutions.Custom.CustomField _CustomField in CustomFields)
                    {
                        ImageSolutions.Custom.CustomValue CustomValue = new ImageSolutions.Custom.CustomValue();
                        ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                        CustomValueFilter.CustomFieldID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueFilter.CustomFieldID.SearchString = _CustomField.CustomFieldID;
                        CustomValueFilter.UserWebsiteID = new Database.Filter.StringSearch.SearchFilter();
                        CustomValueFilter.UserWebsiteID.SearchString = _SalesOrder.UserWebsiteID;
                        CustomValue = ImageSolutions.Custom.CustomValue.GetCustomValue(CustomValueFilter);

                        if (CustomValue != null && !string.IsNullOrEmpty(CustomValue.Value))
                            this.txtUser.Text = String.Format("{0} | {1}: {2}", txtUser.Text, _CustomField.Name, CustomValue.Value);
                    }

                    this.txtOrderNumber.Text = _SalesOrder.SalesOrderID;

                    if (!string.IsNullOrEmpty(_SalesOrder.TermPaymentPONumber))
                    {
                        pnlPONumber.Visible = true;
                        this.txtPONumber.Text = _SalesOrder.TermPaymentPONumber;
                    }
                    else
                    {
                        pnlPONumber.Visible = false;
                    }

                    this.txtOrderDate.Text = _SalesOrder.TransactionDate == null ? String.Empty : Convert.ToDateTime(_SalesOrder.TransactionDate).ToShortDateString();
                    this.txtSubtotal.Text = string.Format("{0:c}", _SalesOrder.LineTotal);
                    this.txtShipping.Text = string.Format("{0:c}", _SalesOrder.ShippingAmount);
                    this.txtTax.Text = string.Format("{0:c}", _SalesOrder.TaxAmount + _SalesOrder.IPDDutiesAndTaxesAmount);
                    this.txtTotal.Text = string.Format("{0:c}", _SalesOrder.Total);

                    this.txtBudgetUsed.Text = String.Format("{0:c}", _SalesOrder.BudgetApplied);
                    this.pnlBudgetUsed.Visible = _SalesOrder.BudgetApplied > 0;

                    this.chkIsPendingApproval.Checked = _SalesOrder.IsPendingApproval;
                    this.chkIsPendingPersonalziationApproval.Checked = _SalesOrder.IsPendingItemPersonalizationApproval;

                    if (_SalesOrder.Payments != null && _SalesOrder.Payments.Count > 0)
                    {
                        if (_SalesOrder.Payments[0].BudgetAssignment != null)
                        {
                            this.txtPayment.Text = "Budget: " + _SalesOrder.Payments[0].BudgetAssignment.Budget.BudgetName;
                        }
                        else if (_SalesOrder.Payments[0].CreditCardTransactionLog != null)
                        {
                            this.txtPayment.Text = "Credit Card: " + _SalesOrder.Payments[0].CreditCardTransactionLog.CCType + " XXXXXXXXXXXX" + _SalesOrder.Payments[0].CreditCardTransactionLog.CCLastFourNumber;
                        }
                        else if (_SalesOrder.Payments[0].PaymentTerm != null)
                        {
                            this.txtPayment.Text = "Payment Term: " + _SalesOrder.Payments[0].PaymentTerm.Description;
                        }
                        else if (_SalesOrder.Payments[0].Promotion != null)
                        {
                            this.txtPayment.Text = "Promotion: " + _SalesOrder.Payments[0].Promotion.PromotionName;
                        }
                    }

                    if (!string.IsNullOrEmpty(_SalesOrder.WebsiteShippingServiceID))
                        txtShippingMethod.Text = _SalesOrder.WebsiteShippingService.ShippingService.ServiceName;

                    txtShippingAddress.Text = string.Format(@"{0}
{1} {2}
{3}
{4}
{5}
{6}
"
                        , _SalesOrder.DeliveryAddress.AddressLabel
                        , _SalesOrder.DeliveryAddress.FirstName
                        , _SalesOrder.DeliveryAddress.LastName
                        , _SalesOrder.DeliveryAddress.AddressLine1
                        , _SalesOrder.DeliveryAddress.AddressLine2
                        , !string.IsNullOrEmpty(_SalesOrder.DeliveryAddress.State)
                            ? string.Format("{0}, {1} {2}", _SalesOrder.DeliveryAddress.City, _SalesOrder.DeliveryAddress.State, _SalesOrder.DeliveryAddress.PostalCode)
                            : string.Format("{0} {1} {2}", _SalesOrder.DeliveryAddress.City, _SalesOrder.DeliveryAddress.State, _SalesOrder.DeliveryAddress.PostalCode)
                        , _SalesOrder.DeliveryAddress.PhoneNumber
                    );

                    txtRejectionReason.Text = _SalesOrder.RejectionReason;

                    btnSave.Text = "Approve";
                    btnSave.Visible = !_SalesOrder.IsClosed && (_SalesOrder.IsPendingApproval || _SalesOrder.IsPendingItemPersonalizationApproval) && (CurrentWebsite.OrderApprovalRequired || CanApprove() || CanApprovePersonalization() );
                    btnReject.Visible = !_SalesOrder.IsClosed && (_SalesOrder.IsPendingApproval || _SalesOrder.IsPendingItemPersonalizationApproval) && (CurrentWebsite.OrderApprovalRequired || CanApprove() || CanApprovePersonalization() );
                    btnDelete.Visible = false;
                }
                else
                {
                    btnSave.Text = "Create";
                    btnReject.Visible = false;
                    btnDelete.Visible = false;
                }

                //txtRejectionReason.Visible = btnReject.Visible;
                txtRejectionReason.Enabled = btnReject.Visible;

                aCancel.HRef = ReturnURL;

                BindRMA();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindCustomField()
        {
            try
            {
                List<ImageSolutions.Custom.CustomValue> CustomValues = new List<ImageSolutions.Custom.CustomValue>();
                ImageSolutions.Custom.CustomValueFilter CustomValueFilter = new ImageSolutions.Custom.CustomValueFilter();
                CustomValueFilter.SalesOrderID = new Database.Filter.StringSearch.SearchFilter();
                CustomValueFilter.SalesOrderID.SearchString = _SalesOrder.SalesOrderID;
                CustomValues = ImageSolutions.Custom.CustomValue.GetCustomValues(CustomValueFilter);

                if(CustomValues != null && CustomValues.Count > 0)
                {
                    rptCustomField.DataSource = CustomValues;
                    rptCustomField.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }

        protected bool CanApprove()
        {
            bool _return = false;

            if (CurrentWebsite.OrderApprovalRequired)
            {
                _return = true;
            }            
            if(!_return)
            {
                List<ImageSolutions.Account.AccountOrderApproval> AccountOrderApprovals = new List<ImageSolutions.Account.AccountOrderApproval>();
                ImageSolutions.Account.AccountOrderApprovalFilter AccountOrderApprovalFilter = new ImageSolutions.Account.AccountOrderApprovalFilter();

                AccountOrderApprovalFilter.AccountID = new Database.Filter.StringSearch.SearchFilter();
                AccountOrderApprovalFilter.AccountID.SearchString = _SalesOrder.AccountID;
                AccountOrderApprovals = ImageSolutions.Account.AccountOrderApproval.GetAccountOrderApprovals(AccountOrderApprovalFilter);

                if (AccountOrderApprovals != null && AccountOrderApprovals.Count > 0)
                {
                    AccountOrderApprovals = AccountOrderApprovals.Where(m => m.Amount <= Convert.ToDecimal(_SalesOrder.Total)).ToList();

                    if (AccountOrderApprovals != null && AccountOrderApprovals.Count > 0)
                    {
                        //ImageSolutions.Account.AccountOrderApproval AccountOrderApproval = new ImageSolutions.Account.AccountOrderApproval();
                        //AccountOrderApproval = AccountOrderApprovals.OrderByDescending(m => m.Amount).First();

                        //if (CurrentUser.UserInfoID == AccountOrderApproval.UserWebsite.UserInfoID)
                        //{
                        //    _return = true;
                        //}

                        foreach (ImageSolutions.Account.AccountOrderApproval _AccountOrderApproval in AccountOrderApprovals.Where(x => x.Amount == AccountOrderApprovals.Max(y => y.Amount)))
                        {
                            if (CurrentUser.UserInfoID == _AccountOrderApproval.UserWebsite.UserInfoID)
                            {
                                _return = true;
                            }
                        }


                    }
                }
                //else
                //{
                //    _return = true;
                //}
            }
            if (!_return)
            {
                foreach (ImageSolutions.Payment.Payment _Payment in _SalesOrder.Payments)
                {
                    if (!string.IsNullOrEmpty(_Payment.BudgetAssignmentID)
                        && _Payment.AmountPaid != _SalesOrder.PaymentTotal)
                    {
                        if (_Payment.BudgetAssignment.Budget.ApproverUserWebsite != null && CurrentUser.UserInfoID == _Payment.BudgetAssignment.Budget.ApproverUserWebsite.UserInfoID)
                        {
                            _return = true;
                        }
                    }
                }
            }

            return _return;
        }

        protected bool CanApprovePersonalization()
        {
            bool blnReturn = false;
            ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
            List<ImageSolutions.User.UserWebsite> UserWebsites = null;

            try
            {
                UserWebsites = new List<ImageSolutions.User.UserWebsite>();

                UserWebsite = GetPersonalizationAppover(_SalesOrder.AccountID);

                if (UserWebsite != null && UserWebsite.UserInfoID == CurrentUser.UserInfoID)
                {
                    blnReturn = true;
                }

                if (!blnReturn)
                {
                    UserWebsite = GetPersonalizationAppover2(_SalesOrder.AccountID);

                    if (UserWebsite != null && UserWebsite.UserInfoID == CurrentUser.UserInfoID)
                    {
                        blnReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                UserWebsite = null;
            }
            return blnReturn;
        }
        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApproverUserWebsiteID);
                }
                else if (!string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }

        protected ImageSolutions.User.UserWebsite GetPersonalizationAppover2(string accountid)
        {
            ImageSolutions.User.UserWebsite UserWebsite = null;
            ImageSolutions.Account.Account Account = null;

            try
            {
                Account = new ImageSolutions.Account.Account(accountid);

                if (!string.IsNullOrEmpty(Account.PersonalizationApprover2UserWebsiteID))
                {
                    UserWebsite = new ImageSolutions.User.UserWebsite(Account.PersonalizationApprover2UserWebsiteID);
                }
                else if (string.IsNullOrEmpty(Account.PersonalizationApproverUserWebsiteID) && !string.IsNullOrEmpty(Account.ParentID))
                {
                    UserWebsite = GetPersonalizationAppover2(Account.ParentID);
                }
            }
            catch (Exception ex)
            {

            }

            return UserWebsite;
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                if(CanApprove())
                {
                    _SalesOrder.IsPendingApproval = false;

                    _SalesOrder.ApprovedBy = CurrentUser.UserInfoID;
                    _SalesOrder.ApprovedOn = DateTime.UtcNow;
                }
                
                if(CanApprovePersonalization())
                {
                    _SalesOrder.IsPendingItemPersonalizationApproval = false;

                    _SalesOrder.ApprovedBy = CurrentUser.UserInfoID;
                    _SalesOrder.ApprovedOn = DateTime.UtcNow;

                    if (CurrentWebsite.WebsiteID == "9" || CurrentWebsite.WebsiteID == "48")
                    {
                        foreach (ImageSolutions.SalesOrder.SalesOrderLine _SalesOrderLine in _SalesOrder.SalesOrderLines)
                        {
                            foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in _SalesOrderLine.ItemPersonalizationValues)
                            {
                                if (_ItemPersonalizationValue.ItemPersonalization.Name.ToLower() == "nickname")
                                {
                                    ImageSolutions.Item.ItemPersonalizationValueApproved ItemPersonalizationValueApproved = new ImageSolutions.Item.ItemPersonalizationValueApproved();
                                    ImageSolutions.Item.ItemPersonalizationValueApprovedFilter ItemPersonalizationValueApprovedFilter = new ImageSolutions.Item.ItemPersonalizationValueApprovedFilter();
                                    ItemPersonalizationValueApprovedFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                    ItemPersonalizationValueApprovedFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                    ItemPersonalizationValueApprovedFilter.ItemPersonalizationName = new Database.Filter.StringSearch.SearchFilter();
                                    ItemPersonalizationValueApprovedFilter.ItemPersonalizationName.SearchString = "Nickname";
                                    ItemPersonalizationValueApprovedFilter.ItemPersonalizationApprovedValue = new Database.Filter.StringSearch.SearchFilter();
                                    ItemPersonalizationValueApprovedFilter.ItemPersonalizationApprovedValue.SearchString = _ItemPersonalizationValue.Value;
                                    ItemPersonalizationValueApproved = ImageSolutions.Item.ItemPersonalizationValueApproved.GetItemPersonalizationValueApproved(ItemPersonalizationValueApprovedFilter);

                                    if (ItemPersonalizationValueApproved == null || string.IsNullOrEmpty(ItemPersonalizationValueApproved.ItemPersonalizationValueApprovedID))
                                    {
                                        ItemPersonalizationValueApproved = new ImageSolutions.Item.ItemPersonalizationValueApproved();
                                        ItemPersonalizationValueApproved.WebsiteID = CurrentWebsite.WebsiteID;
                                        ItemPersonalizationValueApproved.ItemPersonalizationName = "Nickname";
                                        ItemPersonalizationValueApproved.ItemPersonalizationApprovedValue = _ItemPersonalizationValue.Value;
                                        ItemPersonalizationValueApproved.CreatedBy = CurrentUser.UserInfoID;
                                        ItemPersonalizationValueApproved.Create();
                                    }
                                }
                            }
                        }
                    }                   
                }

                blnReturn = _SalesOrder.Update();


                if(!_SalesOrder.IsPendingApproval && !_SalesOrder.IsPendingItemPersonalizationApproval)
                {
                    SendOrderApproval(_SalesOrder);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }


            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
            else
            {
                Response.Redirect("/Admin/OrderOverview.aspx");
            }
        }

        protected bool SendOrderApproval(SalesOrder SalesOrder)
        {
            string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>Your order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been approved and is being processed.</p>
                                                        <p>Items stocked in our warehouse will ship same day if your order is placed by 2:30pm PST and next business day if after. Custom items will ship within 10-12 business days.</p>
                                                    </div>
                                                    <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <thead>
                                                            <tr>
                                                                <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                                <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            ${SalesOrderLine}
                                                        </tbody>
                                                    </table>
                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Ship To:</b>
                                                                                    <br />
                                                                                    ${ShipTo}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                                <td align='right'>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align='right'><b>Subtotal</b></td>
                                                                                            <td align='right'>${Subtotal}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Shipping & Handling</b></td>
                                                                                            <td align='right'>${Shipping}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Tax</b></td>
                                                                                            <td align='right'>${Tax}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right' style='font-weight:bold;'><b>TOTAL</b></td>
                                                                                            <td align='right' style='font-weight:bold;'>${Total}</td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>

                                                    <div style='margin-top:30px;text-align:center;'>
                                                        We appreciate your business! Providing you with a great experience is very important to us. <br />
                                                        - Image Solutions Team
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";

            try
            {
                string strSalesOrderLine = string.Empty;

                foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in SalesOrder.SalesOrderLines)
                {
                    string strOptions = string.Empty;

                    strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";
                    if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
                    {
                        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
                        {
                            strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
                        }
                    }

                    if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
                    {
                        foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
                        {
                            strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
                        }
                    }

                    if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
                    {
                        strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
                    }

                    strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                                                    </tr>", objSalesOrderLine.Item.ItemName, objSalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", objSalesOrderLine.UnitPrice), objSalesOrderLine.Quantity, string.Format("{0:c}", objSalesOrderLine.LineSubTotal));
                }

                strHTMLContent = strHTMLContent.Replace("${Logo}", SalesOrder.Website.LogoPath);
                strHTMLContent = strHTMLContent.Replace("${FirstName}", SalesOrder.UserInfo.FirstName);
                strHTMLContent = strHTMLContent.Replace("${OrderNumber}", SalesOrder.SalesOrderID);
                strHTMLContent = strHTMLContent.Replace("${ShipTo}", SalesOrder.DisplayDeliveryAddress());
                strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", SalesOrder.LineTotal));
                strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", SalesOrder.ShippingAmount));
                strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", SalesOrder.TaxAmount));
                strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", SalesOrder.Total));
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));


                SendEmail(SalesOrder.UserInfo.EmailAddress, CurrentWebsite.Name + " Order Confirmation #" + SalesOrder.SalesOrderID, strHTMLContent);
            }
            catch { }
            finally { }
            return true;
        }

        protected bool SendOrderRejected(SalesOrder SalesOrder)
        {
            string strHTMLContent = @"<!DOCTYPE html>
                                        <html>
                                            <head></head>
                                            <body>
                                                <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                    <div style='float:right;width:45%; font-weight:bold; font-size:20px; text-align:right; padding-top:20px;'>Thank you for your order!</div>

                                                    <div style='float:left; width:45%;'><img src='${Logo}' style='width:2.5in; height:0.76in ' /></div>

                                                    <div style='clear:left;padding-top:40px;'>
                                                        <div style='font-size:16px; font-weight:bold;'>Hi ${FirstName},</div>
                                                        <p>Your order #<span style='color:blue; font-size:14px;'>${OrderNumber}</span> has been rejected</p>                                             

                                                        ${RejectionReason}

                                                    </div>                                                    

                                                    <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <thead>
                                                            <tr>
                                                                <th align='left' colspan='12' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Item</th>
                                                                <th colspan='3' align='center' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Options</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Rate</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Qty</th>
                                                                <th align='center' colspan='4' style='background-color:#ddd;text-transform:uppercase;padding:10px 6px;border:1px solid #333;border-left:none;border-right:none;'>Amount</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            ${SalesOrderLine}
                                                        </tbody>
                                                    </table>
                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                        <tbody>
                                                            <tr>
                                                                <td>
                                                                    <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <b>Ship To:</b>
                                                                                    <br />
                                                                                    ${ShipTo}
                                                                                    <br />
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                    </table>
                                                                </td>
                                                                <td align='right'>
                                                                    <table>
                                                                        <tr>
                                                                            <td>
                                                                                <table style='width:100%;border-collapse:collapse;margin:10px 0;'>
                                                                                    <tbody>
                                                                                        <tr>
                                                                                            <td align='right'><b>Subtotal</b></td>
                                                                                            <td align='right'>${Subtotal}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Shipping & Handling</b></td>
                                                                                            <td align='right'>${Shipping}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right'><b>Tax</b></td>
                                                                                            <td align='right'>${Tax}</td>
                                                                                        </tr>
                                                                                        <tr>
                                                                                            <td align='right' style='font-weight:bold;'><b>TOTAL</b></td>
                                                                                            <td align='right' style='font-weight:bold;'>${Total}</td>
                                                                                        </tr>
                                                                                    </tbody>
                                                                                </table>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </body>
                                        </html>";

            try
            {
                string strSalesOrderLine = string.Empty;

                foreach (ImageSolutions.SalesOrder.SalesOrderLine objSalesOrderLine in SalesOrder.SalesOrderLines)
                {
                    string strOptions = string.Empty;

                    strOptions += objSalesOrderLine.UserInfo == null ? string.Empty : "User: " + objSalesOrderLine.UserInfo.FullName + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_1 == null ? string.Empty : objSalesOrderLine.CustomListValue_1.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_1.ListValue + "<br />";
                    strOptions += objSalesOrderLine.CustomListValue_2 == null ? string.Empty : objSalesOrderLine.CustomListValue_2.CustomList.ListName + ": " + objSalesOrderLine.CustomListValue_2.ListValue + "<br />";
                    if (objSalesOrderLine.ItemPersonalizationValues != null && objSalesOrderLine.ItemPersonalizationValues.Count > 0)
                    {
                        foreach (ImageSolutions.Item.ItemPersonalizationValue _ItemPersonalizationValue in objSalesOrderLine.ItemPersonalizationValues)
                        {
                            strOptions += _ItemPersonalizationValue.ItemPersonalization.Name + ": " + _ItemPersonalizationValue.Value + "<br />";
                        }
                    }

                    if (objSalesOrderLine.SalesOrderLineSelectableLogos != null && objSalesOrderLine.SalesOrderLineSelectableLogos.Count > 0)
                    {
                        foreach (ImageSolutions.SalesOrder.SalesOrderLineSelectableLogo _SalesOrderLineSelectableLogo in objSalesOrderLine.SalesOrderLineSelectableLogos)
                        {
                            strOptions += _SalesOrderLineSelectableLogo.SelectableLogo.Name + "<br />";
                        }
                    }

                    if (objSalesOrderLine.Item.IsNonInventory || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_1) || !string.IsNullOrEmpty(objSalesOrderLine.CustomListValueID_2))
                    {
                        strOptions += "<font style='color:red;'>(This is a custom item. You will receive it in 12 business days.)</font>";
                    }

                    strSalesOrderLine += string.Format(@"<tr>
                                                        <td colspan='12' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>
                                                            <span style='font-weight:bold; color:#333;'>{0}</span><br />
                                                            <span style='font-style:italic;'>Item #{1}</span>
                                                        </td>
                                                        <td colspan='3' align='center' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{2}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{3}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{4}</td>
                                                        <td align='center' colspan='4' style='padding:6px 2px;border-bottom:1px solid #333;' valign='top'>{5}</td>
                                                    </tr>", objSalesOrderLine.Item.ItemName, objSalesOrderLine.Item.ItemNumber, strOptions, string.Format("{0:c}", objSalesOrderLine.UnitPrice), objSalesOrderLine.Quantity, string.Format("{0:c}", objSalesOrderLine.LineSubTotal));
                }

                strHTMLContent = strHTMLContent.Replace("${Logo}", SalesOrder.Website.LogoPath);
                strHTMLContent = strHTMLContent.Replace("${FirstName}", SalesOrder.UserInfo.FirstName);
                strHTMLContent = strHTMLContent.Replace("${OrderNumber}", SalesOrder.SalesOrderID);
                strHTMLContent = strHTMLContent.Replace("${ShipTo}", SalesOrder.DisplayDeliveryAddress());
                strHTMLContent = strHTMLContent.Replace("${Subtotal}", string.Format("{0:c}", SalesOrder.LineTotal));
                strHTMLContent = strHTMLContent.Replace("${Shipping}", string.Format("{0:c}", SalesOrder.ShippingAmount));
                strHTMLContent = strHTMLContent.Replace("${Tax}", string.Format("{0:c}", SalesOrder.TaxAmount));
                strHTMLContent = strHTMLContent.Replace("${Total}", string.Format("{0:c}", SalesOrder.Total));
                strHTMLContent = strHTMLContent.Replace("${SalesOrderLine}", string.Format("{0:c}", strSalesOrderLine));

                strHTMLContent = strHTMLContent.Replace("${RejectionReason}"
                    , string.IsNullOrEmpty(SalesOrder.RejectionReason) 
                        ? string.Empty 
                        : string.Format(@"
<br/>
<p>Reason for rejection:</p>
<p>{0}</p>
<br/>
"
                            , SalesOrder.RejectionReason)
                );

                SendEmail(SalesOrder.UserInfo.EmailAddress, CurrentWebsite.Name + " Order Rejected #" + SalesOrder.SalesOrderID, strHTMLContent);
            }
            catch { }
            finally { }
            return true;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = _SalesOrder.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
            else
            {
                Response.Redirect("/Admin/OrderOverview.aspx");
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {

                if (string.IsNullOrEmpty(txtRejectionReason.Text.Trim()))
                {
                    throw new Exception("Rejection Reason is required");
                }

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                _SalesOrder.IsClosed = true;
                _SalesOrder.Status = "Rejected";
                _SalesOrder.RejectionReason = txtRejectionReason.Text;
                blnReturn = _SalesOrder.Update(objConn, objTran);

                //if payment exists - issue refund
                foreach(ImageSolutions.Payment.Payment _Payment in _SalesOrder.Payments)
                {
                    if(!string.IsNullOrEmpty(_Payment.CreditCardTransactionLogID) && !string.IsNullOrEmpty(_Payment.CreditCardTransactionLog.TransactionID))
                    {
                        ImageSolutions.Refund.Refund Refund = new ImageSolutions.Refund.Refund();
                        Refund.SalesOrderID = _SalesOrder.SalesOrderID;
                        Refund.TransactionNumber = _Payment.CreditCardTransactionLog.TransactionID;
                        Refund.Amount = Convert.ToDecimal(_Payment.AmountPaid);
                        Refund.Create(objConn, objTran);
                    }
                }
                objTran.Commit();

                SendOrderRejected(_SalesOrder);

                if (blnReturn)
                {
                    Response.Redirect(ReturnURL);
                }
                else
                {
                    Response.Redirect("/Admin/OrderOverview.aspx");
                }
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                }

                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

        }

        protected void gvSalesOrderLine_DataBound(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void gvSalesOrderLine_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditPersonalization")
            {
                string strSalesOrderLineID = Convert.ToString(e.CommandArgument);
                ImageSolutions.SalesOrder.SalesOrderLine SalesOrderLine = new SalesOrderLine(strSalesOrderLineID);

                lblSelectedItem.Text = SalesOrderLine.Item.Description;

                rptItemPersonalization.DataSource = SalesOrderLine.ItemPersonalizationValues;
                rptItemPersonalization.DataBind();

                hfSalesOrderLineID.Value = Convert.ToString(SalesOrderLine.SalesOrderLineID);
                pnlEditPersonalization.Visible = true;
            }
        }

        protected void btnUpdatePersonalization_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem _Item in this.rptItemPersonalization.Items)
            {
                string strItemPersonalizationValueID = ((HiddenField)_Item.FindControl("hfItemPersonalizationValueID")).Value;
                TextBox txtValue = (TextBox)_Item.FindControl("txtValue");

                ImageSolutions.Item.ItemPersonalizationValue ItemPersonalizationValue = new ImageSolutions.Item.ItemPersonalizationValue(strItemPersonalizationValueID);
                ItemPersonalizationValue.Value = txtValue.Text;
                ItemPersonalizationValue.Update();
            }

            lblSelectedItem.Text = String.Empty;
            hfSalesOrderLineID.Value = String.Empty;
            pnlEditPersonalization.Visible = false;

            gvSalesOrderLine.DataSource = _SalesOrder.SalesOrderLines;
            gvSalesOrderLine.DataBind();
        }

        protected void btnCancelPersonalization_Click(object sender, EventArgs e)
        {
            lblSelectedItem.Text = String.Empty;
            hfSalesOrderLineID.Value = String.Empty;
            pnlEditPersonalization.Visible = false;
        }

        protected void gvSalesOrderLine_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton btnEditPersonalization = (LinkButton)e.Row.FindControl("btnEditPersonalization");

                btnEditPersonalization.Visible = CanApprovePersonalization();
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
                    gvRMA.Columns[4].Visible = CurrentUser.IsSuperAdmin;

                    lblReturns.Visible = objSalesOrder.RMAs != null && objSalesOrder.RMAs.Count > 0;
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

                        Response.Redirect(String.Format("/Admin/Order.aspx?id={0}", mSalesOrderID));
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