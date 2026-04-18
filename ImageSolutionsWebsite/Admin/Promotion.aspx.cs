using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Promotion : BasePageAdminUserWebSiteAuth
    {
        private string mPromotionID = string.Empty;
        private ImageSolutions.Promotion.Promotion mPromotion = null;

        private ImageSolutions.Promotion.Promotion _Promotion
        {
            get
            {
                if (mPromotion == null)
                {
                    if (!string.IsNullOrEmpty(mPromotionID))
                        mPromotion = new ImageSolutions.Promotion.Promotion(CurrentWebsite.WebsiteID, mPromotionID);
                    else
                        mPromotion = new ImageSolutions.Promotion.Promotion(CurrentWebsite.WebsiteID);
                }
                return mPromotion;
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            mPromotion = null;
            base.OnUnload(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //base.PromotionPermission = true;

            mPromotionID = Request.QueryString.Get("id");

            if (!Page.IsPostBack)
            {
                //WebUtility.AssignValidationGroup(this.gvPromotionBuy, "Promotion_Promotion_PromotionBuy");
                //WebUtility.AssignValidationGroup(this.gvPromotionGet, "Promotion_Promotion_PromotionGet");
                Initialize();
            }            
        }
        protected void Initialize()
        {
            InitializeTabs(top_1_tab, top_1, top_2_tab, top_2);

            BindWebsiteShippingService();

            this.txtPromotionCode.Text = _Promotion.PromotionCode;
            //this.txtCustomerNumber.Text = _Promotion.Customer == null ? string.Empty : _Promotion.Customer.CustomerNumber;
            //if (_Promotion.Customer != null)
            //{
            //    this.hypCustomer.NavigateUrl = WebUtility.PageSURL("Customer/Customer.aspx?id=" + _Promotion.Customer.CustomerID);
            //    this.hypCustomer.Visible = true;
            //}
            //else
            //{
            //    this.hypCustomer.Visible = false;
            //}
            this.txtPromotionName.Text = _Promotion.PromotionName;
            this.txtDiscountPercent.Text = _Promotion.DiscountPercent == null ? string.Empty : (_Promotion.DiscountPercent.Value * 100).ToString();
            this.txtDiscountAmount.Text = _Promotion.DiscountAmount == null ? string.Empty : _Promotion.DiscountAmount.Value.ToString();
            this.txtMinOrderAmount.Text = _Promotion.MinOrderAmount == null ? string.Empty : _Promotion.MinOrderAmount.Value.ToString();
            this.txtMaxOrderAmount.Text = _Promotion.MaxOrderAmount == null ? string.Empty : _Promotion.MaxOrderAmount.Value.ToString();
            this.txtFromDate.Text = _Promotion.FromDate == null ? string.Empty : _Promotion.FromDate.Value.ToShortDateString();
            this.txtToDate.Text = _Promotion.ToDate == null ? string.Empty : _Promotion.ToDate.Value.ToShortDateString();
            this.txtMaxUsageCount.Text = _Promotion.MaxUsageCount == null ? string.Empty : _Promotion.MaxUsageCount.Value.ToString();
            this.lblUsageCount.Text = _Promotion.UsageCount.ToString();
            //this.chkCanBeCombined.Checked = _Promotion.CanBeCombined;
            //this.chkExcludeSaleItem.Checked = _Promotion.ExcludeSaleItem;
            //this.chkIsSalesTaxExempt.Checked = _Promotion.IsSalesTaxExempt;
            //this.chkIsOrPromotionBuy.Checked = _Promotion.IsOrPromotionBuy;
            //this.chkIsOrPromotionGet.Checked = _Promotion.IsOrPromotionGet;

            this.ddlWebsiteShippingService.SelectedValue = _Promotion.FreeShippingServiceID;

            this.chkIsActive.Checked = _Promotion.IsActive;

            //this.lblPromotionBuyDescription.Text = _Promotion.PromotionBuyDescription;
            //this.lblPromotionGetDescription.Text = _Promotion.PromotionGetDescription;
            this.lblPromotionDiscount.Text = _Promotion.PromotionDiscountDescription;

            //this.gvPromotionBuy.EditIndex = -1;
            //BindPromotionBuy();

            //this.gvPromotionGet.EditIndex = -1;
            //BindPromotionGet();

            BindPayments();
        }
        protected void BindPayments()
        {
            int intTotalRecord = 0;
            try
            {
                List<ImageSolutions.Payment.Payment> Payments = new List<ImageSolutions.Payment.Payment>();
                ImageSolutions.Payment.PaymentFilter PaymentFilter = new ImageSolutions.Payment.PaymentFilter();
                PaymentFilter.PromotionID = new Database.Filter.StringSearch.SearchFilter();
                PaymentFilter.PromotionID.SearchString = _Promotion.PromotionID;
                Payments = ImageSolutions.Payment.Payment.GetPayments(PaymentFilter, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord);

                intTotalRecord = _Promotion.Payments == null ? 0 : _Promotion.Payments.Count();
                gvPayments.DataSource = _Promotion.Payments;
                gvPayments.DataBind();

                ucPager.TotalRecord = intTotalRecord;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
        protected void BindWebsiteShippingService()
        {
            try
            {
                List<ImageSolutions.Website.WebsiteShippingService> WebsiteShippingServices = new List<ImageSolutions.Website.WebsiteShippingService>();
                ImageSolutions.Website.WebsiteShippingServiceFilter WebsiteShippingServiceFilter = new ImageSolutions.Website.WebsiteShippingServiceFilter();
                WebsiteShippingServiceFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                WebsiteShippingServiceFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebsiteID;
                WebsiteShippingServices = ImageSolutions.Website.WebsiteShippingService.GetWebsiteShippingServices(WebsiteShippingServiceFilter);

                ddlWebsiteShippingService.DataSource = WebsiteShippingServices;
                ddlWebsiteShippingService.DataBind();

                ddlWebsiteShippingService.Items.Insert(0, new ListItem(string.Empty, string.Empty));
                ddlWebsiteShippingService.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        //protected void BindPromotionBuy()
        //{
        //    this.gvPromotionBuy.DataSource = _Promotion.PromotionBuys;
        //    this.gvPromotionBuy.DataBind();
        //    if (gvPromotionBuy.Rows.Count == 0) WebUtility.ShowNoResultFound(new List<ImageSolutions.Promotion.PromotionBuy>(), this.gvPromotionBuy);
        //    this.gvPromotionBuy.Enabled = !_Promotion.IsNew;
        //}
        //protected void BindPromotionGet()
        //{
        //    this.gvPromotionGet.DataSource = _Promotion.PromotionGets;
        //    this.gvPromotionGet.DataBind();
        //    if (gvPromotionGet.Rows.Count == 0) WebUtility.ShowNoResultFound(new List<ImageSolutions.Promotion.PromotionGet>(), this.gvPromotionGet);
        //    this.gvPromotionGet.Enabled = !_Promotion.IsNew;
        //}
        //protected void ddlSubcategories_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    GridViewRow objRow = (GridViewRow)(((Control)sender).NamingContainer);

        //    string strItemCategoryID = ((DropDownList)sender).SelectedValue;
        //    Repeater rptItemCategory = (Repeater)objRow.FindControl("rptItemCategory");
        //    Repeater rptSubcategories = (Repeater)objRow.FindControl("rptSubcategories");
        //    DropDownList ddlSubcategories = (DropDownList)objRow.FindControl("ddlSubcategories");
        //    BindItemCategory(strItemCategoryID, rptItemCategory);
        //    BindSubcategory(strItemCategoryID, rptSubcategories);
        //    BindSubcategory(strItemCategoryID, ddlSubcategories);
        //}
        //protected void lkBtnItemCategory_Click(object sender, EventArgs e)
        //{
        //    GridViewRow objRow = (GridViewRow)(((((Control)sender).NamingContainer)).NamingContainer).NamingContainer;

        //    string strItemCategoryID = ((LinkButton)sender).CommandArgument;
        //    Repeater rptItemCategory = (Repeater)objRow.FindControl("rptItemCategory");
        //    Repeater rptSubcategories = (Repeater)objRow.FindControl("rptSubcategories");
        //    DropDownList ddlSubcategories = (DropDownList)objRow.FindControl("ddlSubcategories");
        //    BindItemCategory(strItemCategoryID, rptItemCategory);
        //    BindSubcategory(strItemCategoryID, rptSubcategories);
        //    BindSubcategory(strItemCategoryID, ddlSubcategories);
        //}
        //protected void BindItemCategory(string ItemCategoryID, Repeater rptItemCategory)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(ItemCategoryID))
        //            rptItemCategory.DataSource = new ImageSolutions.ItemCategory(CurrentBusiness.BusinessID, ItemCategoryID).Breadcrumb;
        //        else
        //            rptItemCategory.DataSource = new List<ImageSolutions.ItemCategory>();
        //        rptItemCategory.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //}
        //protected void BindSubcategory(string ItemCategoryID, Repeater rptSubcategories)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(ItemCategoryID)
        //            rptSubcategories.DataSource = new ImageSolutions.ItemCategory(CurrentBusiness.BusinessID, ItemCategoryID).SubCategories;
        //        else
        //            rptSubcategories.DataSource = new List<ImageSolutions.ItemCategory>();
        //        rptSubcategories.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //}
        //protected void BindSubcategory(string ItemCategoryID, DropDownList ddlSubcategories)
        //{
        //    ImageSolutions.ItemCategoryFilter objFilter = null;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(ItemCategoryID))
        //            ddlSubcategories.DataSource = new ImageSolutions.ItemCategory(CurrentWebsite.WebsiteID, ItemCategoryID).SubCategories;
        //        else
        //        {
        //            objFilter = new ImageSolutions.ItemCategoryFilter();
        //            ddlSubcategories.DataSource = ImageSolutions.ItemCategory.GetItemCategories(CurrentWebsite.WebsiteID);
        //        }
        //        ddlSubcategories.DataBind();
        //        ddlSubcategories.Items.Insert(0, new ListItem());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        objFilter = null;
        //    }
        //}
        //protected void BindItem(string ItemID, DropDownList ddlItem)
        //{
        //    ddlItem.DataSource = ImageSolutions.Item.Item.GetItems(CurrentWebsite.WebsiteID);
        //    ddlItem.DataBind();
        //    ddlItem.Items.Insert(0, new ListItem());
        //    ddlItem.SelectedIndex = ddlItem.Items.IndexOf(ddlItem.Items.FindByValue(ItemID));
        //}
        //protected void gvPromotionBuy_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
        //        {
        //            string strItemCategoryID = Convert.ToString(gvPromotionBuy.DataKeys[e.Row.RowIndex]["ItemCategoryID"]);
        //            Repeater rptItemCategory = (Repeater)e.Row.FindControl("rptItemCategory");
        //            BindItemCategory(strItemCategoryID, rptItemCategory);
        //        }
        //        else if (e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Normal) || e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Alternate))
        //        {
        //            string strItemCategoryID = Convert.ToString(gvPromotionBuy.DataKeys[e.Row.RowIndex]["ItemCategoryID"]);
        //            Repeater rptItemCategory = (Repeater)e.Row.FindControl("rptItemCategory");
        //            Repeater rptSubcategories = (Repeater)e.Row.FindControl("rptSubcategories");
        //            DropDownList ddlSubcategories = (DropDownList)e.Row.FindControl("ddlSubcategories");
        //            BindItemCategory(strItemCategoryID, rptItemCategory);
        //            BindSubcategory(strItemCategoryID, rptSubcategories);
        //            BindSubcategory(strItemCategoryID, ddlSubcategories);

        //            string strItemID = Convert.ToString(gvPromotionBuy.DataKeys[e.Row.RowIndex]["ItemID"]);
        //            DropDownList ddlItem = (DropDownList)e.Row.FindControl("ddlItem");
        //            BindItem(strItemID, ddlItem);
        //        }
        //    }
        //    else if (e.Row.RowType == DataControlRowType.Footer)
        //    {
        //        Repeater rptItemCategory = (Repeater)e.Row.FindControl("rptItemCategory");
        //        Repeater rptSubcategories = (Repeater)e.Row.FindControl("rptSubcategories");
        //        DropDownList ddlSubcategories = (DropDownList)e.Row.FindControl("ddlSubcategories");
        //        BindItemCategory(string.Empty, rptItemCategory);
        //        BindSubcategory(string.Empty, rptSubcategories);
        //        BindSubcategory(string.Empty, ddlSubcategories);

        //        DropDownList ddlItem = (DropDownList)e.Row.FindControl("ddlItem");
        //        BindItem(string.Empty, ddlItem);
        //    }
        //}
        //protected void gvPromotionBuy_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName.ToLower() == "add")
        //    {
        //        ImageSolutions.Promotion.PromotionBuy objPromotionBuy = null;
        //        string strItemCategoryID = string.Empty;
        //        string strItemID = string.Empty;

        //        try
        //        {
        //            Repeater rptItemCategory = (Repeater)gvPromotionBuy.FooterRow.FindControl("rptItemCategory");
        //            TextBox txtQuantity = (TextBox)gvPromotionBuy.FooterRow.FindControl("txtQuantity");
        //            if (rptItemCategory.Items.Count > 0) strItemCategoryID = ((LinkButton)rptItemCategory.Items[rptItemCategory.Items.Count - 1].FindControl("lkBtnItemCategory")).CommandArgument.ToString();

        //            DropDownList ddlItem = (DropDownList)gvPromotionBuy.FooterRow.FindControl("ddlItem");
        //            strItemID = ddlItem.SelectedValue;

        //            //if (string.IsNullOrEmpty(strItemCategoryID) && string.IsNullOrEmpty(strItemID)) throw new Exception("Please select a category/item");

        //            objPromotionBuy = new ImageSolutions.Promotion.PromotionBuy(CurrentBusiness.BusinessID);
        //            objPromotionBuy.PromotionID = Promotion.PromotionID;
        //            objPromotionBuy.ItemCategoryID = strItemCategoryID;
        //            objPromotionBuy.ItemID = strItemID;
        //            objPromotionBuy.Quantity = string.IsNullOrEmpty(txtQuantity.Text.Trim()) ? (int?)null : Convert.ToInt32(txtQuantity.Text.Trim());
        //            objPromotionBuy.CreatedBy = CurrentUser.BusinessUserID;
        //            objPromotionBuy.Create();

        //            Promotion.PromotionBuys = null;
        //            this.gvPromotionBuy.EditIndex = -1;
        //            Initialize();
        //        }
        //        catch (Exception ex)
        //        {
        //            WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
        //        }
        //        finally
        //        {
        //            objPromotionBuy = null;
        //        }
        //    }
        //}
        //protected void gvPromotionBuy_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    this.gvPromotionBuy.EditIndex = e.NewEditIndex;
        //    BindPromotionBuy();
        //}

        //protected void gvPromotionBuy_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    this.gvPromotionBuy.EditIndex = -1;
        //    BindPromotionBuy();
        //}
        //protected void gvPromotionBuy_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    ImageSolutions.Promotion.PromotionBuy objPromotionBuy = null;
        //    string strPromotionBuyID = string.Empty;
        //    string strItemCategoryID = string.Empty;
        //    string strItemID = string.Empty;

        //    try
        //    {
        //        Repeater rptItemCategory = (Repeater)gvPromotionBuy.Rows[e.RowIndex].FindControl("rptItemCategory");
        //        TextBox txtQuantity = (TextBox)gvPromotionBuy.Rows[e.RowIndex].FindControl("txtQuantity");
        //        strPromotionBuyID = gvPromotionBuy.DataKeys[e.RowIndex]["PromotionBuyID"].ToString();
        //        if (rptItemCategory.Items.Count > 0) strItemCategoryID = ((LinkButton)rptItemCategory.Items[rptItemCategory.Items.Count - 1].FindControl("lkBtnItemCategory")).CommandArgument.ToString();

        //        DropDownList ddlItem = (DropDownList)gvPromotionBuy.Rows[e.RowIndex].FindControl("ddlItem");
        //        strItemID = ddlItem.SelectedValue;

        //        //if (string.IsNullOrEmpty(strItemCategoryID) && string.IsNullOrEmpty(strItemID)) throw new Exception("Please select a category/item");

        //        objPromotionBuy = new ImageSolutions.Promotion.PromotionBuy(CurrentBusiness.BusinessID, strPromotionBuyID);
        //        objPromotionBuy.ItemCategoryID = strItemCategoryID;
        //        objPromotionBuy.ItemID = strItemID;
        //        objPromotionBuy.Quantity = string.IsNullOrEmpty(txtQuantity.Text.Trim()) ? (int?)null : Convert.ToInt32(txtQuantity.Text.Trim());
        //        objPromotionBuy.UpdatedBy = CurrentUser.BusinessUserID;
        //        objPromotionBuy.Update();
        //        gvPromotionBuy.EditIndex = -1;
        //        Initialize();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
        //    }
        //    finally
        //    {
        //        objPromotionBuy = null;
        //    }
        //}

        //protected void gvPromotionBuy_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    ImageSolutions.Promotion.PromotionBuy objPromotionBuy = null;
        //    string strPromotionBuyID = string.Empty;

        //    try
        //    {
        //        Repeater rptItemCategory = (Repeater)gvPromotionBuy.Rows[e.RowIndex].FindControl("rptItemCategory");
        //        strPromotionBuyID = gvPromotionBuy.DataKeys[e.RowIndex]["PromotionBuyID"].ToString();

        //        objPromotionBuy = new ImageSolutions.Promotion.PromotionBuy(CurrentBusiness.BusinessID, strPromotionBuyID);
        //        objPromotionBuy.IsActive = false;
        //        objPromotionBuy.UpdatedBy = CurrentUser.BusinessUserID;
        //        objPromotionBuy.Delete();
        //        gvPromotionBuy.EditIndex = -1;
        //        Initialize();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
        //    }
        //    finally
        //    {
        //        objPromotionBuy = null;
        //    }
        //}

        //protected void gvPromotionGet_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    if (e.Row.RowType == DataControlRowType.DataRow)
        //    {
        //        if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
        //        {
        //            string strItemCategoryID = Convert.ToString(gvPromotionGet.DataKeys[e.Row.RowIndex]["ItemCategoryID"]);
        //            Repeater rptItemCategory = (Repeater)e.Row.FindControl("rptItemCategory");
        //            BindItemCategory(strItemCategoryID, rptItemCategory);
        //        }
        //        else if (e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Normal) || e.Row.RowState == (DataControlRowState.Edit | DataControlRowState.Alternate))
        //        {
        //            string strItemCategoryID = Convert.ToString(gvPromotionGet.DataKeys[e.Row.RowIndex]["ItemCategoryID"]);
        //            Repeater rptItemCategory = (Repeater)e.Row.FindControl("rptItemCategory");
        //            Repeater rptSubcategories = (Repeater)e.Row.FindControl("rptSubcategories");
        //            DropDownList ddlSubcategories = (DropDownList)e.Row.FindControl("ddlSubcategories");
        //            BindItemCategory(strItemCategoryID, rptItemCategory);
        //            BindSubcategory(strItemCategoryID, rptSubcategories);
        //            BindSubcategory(strItemCategoryID, ddlSubcategories);

        //            string strItemID = Convert.ToString(gvPromotionBuy.DataKeys[e.Row.RowIndex]["ItemID"]);
        //            DropDownList ddlItem = (DropDownList)e.Row.FindControl("ddlItem");
        //            BindItem(strItemID, ddlItem);
        //        }
        //    }
        //    else if (e.Row.RowType == DataControlRowType.Footer)
        //    {
        //        Repeater rptItemCategory = (Repeater)e.Row.FindControl("rptItemCategory");
        //        Repeater rptSubcategories = (Repeater)e.Row.FindControl("rptSubcategories");
        //        DropDownList ddlSubcategories = (DropDownList)e.Row.FindControl("ddlSubcategories");
        //        BindItemCategory(string.Empty, rptItemCategory);
        //        BindSubcategory(string.Empty, rptSubcategories);
        //        BindSubcategory(string.Empty, ddlSubcategories);

        //        DropDownList ddlItem = (DropDownList)e.Row.FindControl("ddlItem");
        //        BindItem(string.Empty, ddlItem);
        //    }
        //}

        //protected void gvPromotionGet_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName.ToLower() == "add")
        //    {
        //        ImageSolutions.Promotion.PromotionGet objPromotionGet = null;
        //        string strItemCategoryID = string.Empty;
        //        string strItemID = string.Empty;

        //        try
        //        {
        //            Repeater rptItemCategory = (Repeater)gvPromotionGet.FooterRow.FindControl("rptItemCategory");
        //            TextBox txtQuantity = (TextBox)gvPromotionGet.FooterRow.FindControl("txtQuantity");
        //            if (rptItemCategory.Items.Count > 0) strItemCategoryID = ((LinkButton)rptItemCategory.Items[rptItemCategory.Items.Count - 1].FindControl("lkBtnItemCategory")).CommandArgument.ToString();

        //            DropDownList ddlItem = (DropDownList)gvPromotionGet.FooterRow.FindControl("ddlItem");
        //            strItemID = ddlItem.SelectedValue;

        //            //if (string.IsNullOrEmpty(strItemCategoryID) && string.IsNullOrEmpty(strItemID)) throw new Exception("Please select a category/item");

        //            objPromotionGet = new ImageSolutions.Promotion.PromotionGet(CurrentBusiness.BusinessID);
        //            objPromotionGet.PromotionID = Promotion.PromotionID;
        //            objPromotionGet.ItemCategoryID = strItemCategoryID;
        //            objPromotionGet.ItemID = strItemID;
        //            objPromotionGet.Quantity = string.IsNullOrEmpty(txtQuantity.Text.Trim()) ? (int?)null : Convert.ToInt32(txtQuantity.Text.Trim());
        //            objPromotionGet.CreatedBy = CurrentUser.BusinessUserID;
        //            objPromotionGet.Create();

        //            Promotion.PromotionGets = null;
        //            this.gvPromotionGet.EditIndex = -1;
        //            Initialize();
        //        }
        //        catch (Exception ex)
        //        {
        //            WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
        //        }
        //        finally
        //        {
        //            objPromotionGet = null;
        //        }
        //    }
        //}

        //protected void gvPromotionGet_RowEditing(object sender, GridViewEditEventArgs e)
        //{
        //    this.gvPromotionGet.EditIndex = e.NewEditIndex;
        //    BindPromotionGet();
        //}

        //protected void gvPromotionGet_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        //{
        //    this.gvPromotionGet.EditIndex = -1;
        //    BindPromotionGet();
        //}

        //protected void gvPromotionGet_RowUpdating(object sender, GridViewUpdateEventArgs e)
        //{
        //    ImageSolutions.Promotion.PromotionGet objPromotionGet = null;
        //    string strPromotionGetID = string.Empty;
        //    string strItemCategoryID = string.Empty;
        //    string strItemID = string.Empty;

        //    try
        //    {
        //        Repeater rptItemCategory = (Repeater)gvPromotionGet.Rows[e.RowIndex].FindControl("rptItemCategory");
        //        TextBox txtQuantity = (TextBox)gvPromotionGet.Rows[e.RowIndex].FindControl("txtQuantity");
        //        strPromotionGetID = gvPromotionGet.DataKeys[e.RowIndex]["PromotionGetID"].ToString();
        //        if (rptItemCategory.Items.Count > 0) strItemCategoryID = ((LinkButton)rptItemCategory.Items[rptItemCategory.Items.Count - 1].FindControl("lkBtnItemCategory")).CommandArgument.ToString();

        //        DropDownList ddlItem = (DropDownList)gvPromotionGet.Rows[e.RowIndex].FindControl("ddlItem");
        //        strItemID = ddlItem.SelectedValue;

        //        //if (string.IsNullOrEmpty(strItemCategoryID) && string.IsNullOrEmpty(strItemID)) throw new Exception("Please select a category/item");

        //        objPromotionGet = new ImageSolutions.Promotion.PromotionGet(CurrentBusiness.BusinessID, strPromotionGetID);
        //        objPromotionGet.ItemCategoryID = strItemCategoryID;
        //        objPromotionGet.ItemID = strItemID;
        //        objPromotionGet.Quantity = string.IsNullOrEmpty(txtQuantity.Text.Trim()) ? (int?)null : Convert.ToInt32(txtQuantity.Text.Trim());
        //        objPromotionGet.UpdatedBy = CurrentUser.BusinessUserID;
        //        objPromotionGet.Update();
        //        gvPromotionGet.EditIndex = -1;
        //        Initialize();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
        //    }
        //    finally
        //    {
        //        objPromotionGet = null;
        //    }
        //}

        //protected void gvPromotionGet_RowDeleting(object sender, GridViewDeleteEventArgs e)
        //{
        //    ImageSolutions.Promotion.PromotionGet objPromotionGet = null;
        //    string strPromotionGetID = string.Empty;

        //    try
        //    {
        //        Repeater rptItemCategory = (Repeater)gvPromotionGet.Rows[e.RowIndex].FindControl("rptItemCategory");
        //        strPromotionGetID = gvPromotionGet.DataKeys[e.RowIndex]["PromotionGetID"].ToString();

        //        objPromotionGet = new ImageSolutions.Promotion.PromotionGet(CurrentBusiness.BusinessID, strPromotionGetID);
        //        objPromotionGet.IsActive = false;
        //        objPromotionGet.UpdatedBy = CurrentUser.BusinessUserID;
        //        objPromotionGet.Delete();
        //        gvPromotionGet.EditIndex = -1;
        //        Initialize();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
        //    }
        //    finally
        //    {
        //        objPromotionGet = null;
        //    }
        //}

        protected void UpdatePromotion()
        {
            //ImageSolutions.Customer objCustomer = null;

            try
            {
                //if (!string.IsNullOrEmpty(this.txtCustomerNumber.Text.Trim()))
                //{
                //    objCustomer = ImageSolutions.Customer.GetCustomerByCustomerNumber(CurrentBusiness.BusinessID, this.txtCustomerNumber.Text.Trim());
                //    if (objCustomer == null) throw new Exception("Customer number not found");
                //    _Promotion.CustomerID = objCustomer.CustomerID;
                //}
                //else
                //{
                //    _Promotion.CustomerID = string.Empty;
                //}
                _Promotion.PromotionCode = this.txtPromotionCode.Text.Trim();
                _Promotion.PromotionName = this.txtPromotionName.Text.Trim();
                _Promotion.DiscountPercent = string.IsNullOrEmpty(this.txtDiscountPercent.Text.Trim()) ? (decimal?)null : Convert.ToDecimal(this.txtDiscountPercent.Text.Trim()) / 100;
                _Promotion.DiscountAmount = string.IsNullOrEmpty(this.txtDiscountAmount.Text.Trim()) ? (decimal?)null : Convert.ToDecimal(this.txtDiscountAmount.Text.Trim());
                _Promotion.MinOrderAmount = string.IsNullOrEmpty(this.txtMinOrderAmount.Text.Trim()) ? (decimal?)null : Convert.ToDecimal(this.txtMinOrderAmount.Text.Trim());
                _Promotion.MaxOrderAmount = string.IsNullOrEmpty(this.txtMaxOrderAmount.Text.Trim()) ? (decimal?)null : Convert.ToDecimal(this.txtMaxOrderAmount.Text.Trim());
                _Promotion.FromDate = string.IsNullOrEmpty(this.txtFromDate.Text.Trim()) ? (DateTime?)null : Convert.ToDateTime(this.txtFromDate.Text.Trim());
                _Promotion.ToDate = string.IsNullOrEmpty(this.txtToDate.Text.Trim()) ? (DateTime?)null : Convert.ToDateTime(this.txtToDate.Text.Trim());
                _Promotion.MaxUsageCount = string.IsNullOrEmpty(this.txtMaxUsageCount.Text.Trim()) ? (int?)null : Convert.ToInt32(this.txtMaxUsageCount.Text.Trim());
                //_Promotion.CanBeCombined = this.chkCanBeCombined.Checked;
                //_Promotion.ExcludeSaleItem = this.chkExcludeSaleItem.Checked;
                //_Promotion.IsSalesTaxExempt = this.chkIsSalesTaxExempt.Checked;
                //_Promotion.IsOrPromotionBuy = this.chkIsOr_PromotionBuy.Checked;
                //_Promotion.IsOrPromotionGet = this.chkIsOr_PromotionGet.Checked;
                _Promotion.FreeShippingServiceID = ddlWebsiteShippingService.SelectedValue;
                _Promotion.IsActive = this.chkIsActive.Checked;
                Initialize();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //objCustomer = null;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    UpdatePromotion();
                    if (_Promotion.IsNew)
                    {
                        _Promotion.CreatedBy = CurrentUser.CurrentUserWebSite.UserInfoID;
                        _Promotion.Create();
                        Response.Redirect(WebUtility.PageSURL("Admin/Promotion.aspx?id=" + _Promotion.PromotionID));
                    }
                    else
                    {
                        _Promotion.UpdatedBy = CurrentUser.CurrentUserWebSite.UserInfoID;
                        _Promotion.Update();
                        this.lblMessage.Text = "Promotion saved successfully";
                    }
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            }
        }

        protected void chkIsOrPromotionGet_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePromotion();
        }

        protected void chkIsOrPromotionBuy_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePromotion();
        }
    }
}