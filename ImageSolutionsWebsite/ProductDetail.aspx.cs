using ImageSolutions.Attribute;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace ImageSolutionsWebsite
{
    public partial class ProductDetail : BasePageUserAccountAuth
    {
        protected string mWebSiteTabID = string.Empty;
        protected string mItemID = string.Empty;
        protected int mGroupByAttributeValueCount = 0;
        protected string mSelectedGroupByAttributeValueID = string.Empty;
        //protected string mSelectedItemSelectableLogoID = string.Empty;

        private AttributeValue mSelectedGroupAttributeValue = null;
        public AttributeValue SelectedGroupAttributeValue
        {
            get
            {
                if (mSelectedGroupAttributeValue == null && !string.IsNullOrEmpty(mSelectedGroupByAttributeValueID))
                {
                    mSelectedGroupAttributeValue = new AttributeValue(mSelectedGroupByAttributeValueID);
                }
                return mSelectedGroupAttributeValue;
            }
        }

        private ImageSolutions.Item.Item mItem = null;
        protected ImageSolutions.Item.Item _Item
        {
            get
            {
                if (mItem == null)
                {
                    if (string.IsNullOrEmpty(mItemID))
                        mItem = new ImageSolutions.Item.Item();
                    else
                        mItem = new ImageSolutions.Item.Item(mItemID);
                }
                return mItem;
            }
            set
            {
                mItem = value;
            }
        }
        private ImageSolutions.Item.MyGroupItem mMyGroupItem = null;
        protected ImageSolutions.Item.MyGroupItem _MyGroupItem
        {
            get
            {
                if (mMyGroupItem == null)
                {
                    if (!string.IsNullOrEmpty(mItemID)) mMyGroupItem = new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, mItemID);
                }
                return mMyGroupItem;
            }           
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");
            mItemID = Request.QueryString.Get("id");

            //if (CurrentWebsite.Name == "Discount Tire Corporate" || CurrentWebsite.Name == "Securitas")
            //{
                Response.Redirect("/ProductDetailDT.aspx?id=" + mItemID + "&WebSiteTabID=" + mWebSiteTabID);
            //}

            if (string.IsNullOrEmpty(mWebSiteTabID))
            {
                try
                {
                    mWebSiteTabID = new ImageSolutions.Item.Item(mItemID).WebsiteTabItems[0].WebsiteTabID;
                }
                catch { }
            }

            UpdateAccount();

            if (string.IsNullOrEmpty(hfAccountID.Value))
            {
                if (HttpContext.Current.Request.Cookies["AccountUpdate"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["AccountUpdate"].Value))
                {
                    hfAccountID.Value = Convert.ToString(HttpContext.Current.Request.Cookies["AccountUpdate"].Value);
                }
            }

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        protected void UpdateAccount()
        {
            ucAccountSearchModal.SendMessageToThePage += delegate (string message)
            {
                try
                {
                    hfAccountID.Value = message;

                    if (!string.IsNullOrEmpty(hfAccountID.Value))
                    {
                        ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);

                        if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                        {
                            txtAccount.Text = Account.AccountName;
                            BindUserInfo();
                        }
                    }
                    else
                    {
                        txtAccount.Text = string.Empty;
                    }

                    //btnAccountRemove.Visible = !string.IsNullOrEmpty(hfAccountID.Value);
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                }
            };

        }
        public void InitializePage()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString.Get("WebSiteTabID")))
                {
                    pnlCategoryBreadCrumb.Visible = true;
                    ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebSiteTabID);
                    litCategoryBreadCrumb.Text = WebsiteTab.TabPathBreadCrumb;
                }
                else
                {
                    pnlCategoryBreadCrumb.Visible = false;
                }

                lblHeader.Text = _MyGroupItem.Item.StoreDisplayName;
                litItemNumber.Text = _MyGroupItem.Item.ItemNumber;
                litSalesDescription.Text = _MyGroupItem.Item.SalesDescription;
                litSalesDescription.Visible = CurrentWebsite.ShowSalesDescription;

                if(CurrentWebsite.ShowDetailedDescription && !_MyGroupItem.Item.HideDetailedDescription && !string.IsNullOrEmpty(_MyGroupItem.Item.DetailedDescription))
                {
                    litDetailedDescription.Text = _MyGroupItem.Item.DetailedDescription;
                    List <ImageSolutions.Website.WebsiteTag> WebsiteTags = new List<ImageSolutions.Website.WebsiteTag>();
                    WebsiteTags = ImageSolutions.Website.WebsiteTag.GetWebsiteTags();
                    
                    foreach (ImageSolutions.Website.WebsiteTag _WebsiteTag in WebsiteTags)
                    {
                        string strTag = string.Format("<{0}>", _WebsiteTag.Name);
                        litDetailedDescription.Text = Regex.Replace(litDetailedDescription.Text, strTag, "<br>" + _WebsiteTag.Value, RegexOptions.IgnoreCase);
                    }


                    int intULFrom = litDetailedDescription.Text.IndexOf("<ul");
                    while (intULFrom >= 0)
                    {
                        int intULTo = litDetailedDescription.Text.IndexOf(">", intULFrom);
                        string strUL = litDetailedDescription.Text.Substring(intULFrom, intULTo - intULFrom + 1);
                        litDetailedDescription.Text = litDetailedDescription.Text.Replace(strUL, "<ul>");

                        intULFrom = litDetailedDescription.Text.IndexOf("<ul", intULFrom + 1);
                    }

                    int intLIFrom = litDetailedDescription.Text.IndexOf("<li");
                    while (intLIFrom >= 0)
                    {
                        int intLITo = litDetailedDescription.Text.IndexOf(">", intLIFrom);
                        string strLI = litDetailedDescription.Text.Substring(intLIFrom, intLITo - intLIFrom + 1);
                        litDetailedDescription.Text = litDetailedDescription.Text.Replace(strLI, "<li>");

                        intLIFrom = litDetailedDescription.Text.IndexOf("<li", intLIFrom + 1);
                    }

                    if(CurrentWebsite.WebsiteID == "15" || CurrentWebsite.WebsiteID == "17" || CurrentWebsite.WebsiteID == "18" || CurrentWebsite.WebsiteID == "19")
                    {
                        int intSPANFrom = litDetailedDescription.Text.IndexOf("<span");
                        while (intSPANFrom >= 0)
                        {
                            int intSPANTo = litDetailedDescription.Text.IndexOf(">", intSPANFrom);
                            string strSPAN = litDetailedDescription.Text.Substring(intSPANFrom, intSPANTo - intSPANFrom + 1);
                            litDetailedDescription.Text = litDetailedDescription.Text.Replace(strSPAN, "<span>");

                            intSPANFrom = litDetailedDescription.Text.IndexOf("<span", intSPANFrom + 1);
                        }
                    }

                    int intPFrom = litDetailedDescription.Text.IndexOf("<p");
                    while (intPFrom >= 0)
                    {
                        int intPTo = litDetailedDescription.Text.IndexOf(">", intPFrom);
                        string strP = litDetailedDescription.Text.Substring(intPFrom, intPTo - intPFrom + 1);

                        if(strP.Substring(0,3) == "<p ")
                        {
                            litDetailedDescription.Text = litDetailedDescription.Text.Replace(strP, "<p>");
                        }

                        intPFrom = litDetailedDescription.Text.IndexOf("<p", intPFrom + 1);
                    }

                    litDetailedDescription.Text = litDetailedDescription.Text.Replace("</p><br>", "</p>");
                    litDetailedDescription.Text = litDetailedDescription.Text.Replace("<ul>", string.Format(@"<ul style=""list-style-position: inside;"">"));
                    litDetailedDescription.Text = litDetailedDescription.Text.Replace("<li>", string.Format(@"<li style=""display:revert;"">"));
                    //litDetailedDescription.Text = _MyGroupItem.Item.DetailedDescription.Replace("<ul>", string.Empty).Replace("</ul>", string.Empty).Replace("<li>", " - ").Replace("</li>", "<br>");
                    litDetailedDescription.Text = string.Format(@"<br>{0}", litDetailedDescription.Text);
                }
                litBasePrice.Text = Convert.ToString(_MyGroupItem.Price);

                if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert) 
                    && CurrentWebsite.CurrentyConvertPercentage != null 
                    && CurrentWebsite.CurrentyConvertPercentage > 0)
                {
                    litBasePrice.Text = string.Format(@"{0:c} USD <span style=""font-size: small; "" title=""est. {1:c} {2}""><i class=""ti-info-alt"" ></i> </span>"
                        , Convert.ToString(_MyGroupItem.Price)
                        , Convert.ToDecimal(_MyGroupItem.Price) * CurrentWebsite.CurrentyConvertPercentage
                        , CurrentWebsite.CurrencyConvert
                        );
                }

                if (!string.IsNullOrEmpty(_MyGroupItem.Item.DisplayImageURL))
                {
                    imgItem.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                    imgItem2.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                }
                else
                {
                    imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
                    imgItem2.ImageUrl = "../assets/images/pro3/2.jpg";
                }

                if (!string.IsNullOrEmpty(mWebSiteTabID) || !string.IsNullOrEmpty(mItemID)) BindItems();
                SetQuantityField();

                this.rptProductDetail.DataSource = _MyGroupItem.Item.ItemDetails;
                this.rptProductDetail.DataBind();

                //if (_mygroupitem.item.itemnumber == "is700n-123456")
                //{
                //    this.phcustomization.visible = true;
                //}

                if(!string.IsNullOrEmpty(_Item.UnitItemID))
                {
                    pnlSingleUnit.Visible = true;
                    btnSingleUnit.NavigateUrl = string.Format(@"../ProductDetail.aspx?id={0}",_Item.UnitItemID);
                }
                else
                {
                    pnlSingleUnit.Visible = false;
                }

                if (!CurrentWebsite.DisplayLeftNavigation)
                {
                    divLeftPanel.Visible = false;
                }

                btnSizeChart.Visible = !_Item.HideSizeChart && (!string.IsNullOrEmpty(CurrentWebsite.DefaultSizeChartPath) || !string.IsNullOrEmpty(_Item.SizeChartURL));

                List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();
                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                ItemFilter.ParentID.SearchString = _MyGroupItem.ItemID;
                Items = ImageSolutions.Item.Item.GetItems(ItemFilter);

                divNonInventoryUnavailableMessage.Visible = false;
                
                if (!_Item.DoNotDisplayNIMessage)
                {
                    foreach (ImageSolutions.Item.Item _Item2 in Items)
                    {
                        if (_Item2.IsNonInventory && (_Item2.VendorInventory == 0 || CurrentWebsite.DisplayNonInventoryMessage))
                        {
                            divNonInventoryUnavailableMessage.Visible = true;
                        }
                    }
                }

                if (IsDisplayAccountUserItem(mItemID))
                {
                    phEmployee.Visible = true;
                    BindUserInfo();
                }
                else
                {
                    phEmployee.Visible = false;
                }

                BindRelatedItem();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindUserInfo()
        {
            List<ImageSolutions.User.UserInfo> UserInfos = new List<ImageSolutions.User.UserInfo>();

            if (!string.IsNullOrEmpty(ddlAccount.SelectedValue) || !string.IsNullOrEmpty(hfAccountID.Value))
            {
                ImageSolutions.Account.Account Account = null;
                
                if(!string.IsNullOrEmpty(ddlAccount.SelectedValue))
                {
                    Account = new ImageSolutions.Account.Account(Convert.ToString(ddlAccount.SelectedValue));
                }
                else
                {
                    Account = new ImageSolutions.Account.Account(Convert.ToString(hfAccountID.Value));
                }

                foreach (ImageSolutions.User.UserInfo _UserInfo in Account.UserInfos)
                {
                    ImageSolutions.User.UserWebsite UserWebSite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.UserInfoID.SearchString = _UserInfo.UserInfoID;
                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    UserWebSite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                    if (UserWebSite != null && !UserWebSite.InActive)
                    {
                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo(_UserInfo.UserInfoID);
                        if (!string.IsNullOrEmpty(UserWebSite.EmployeeID))
                        {
                            UserInfo.LastName = String.Format("{0} - {1}", UserInfo.LastName, UserWebSite.EmployeeID);
                        }

                        UserInfos.Add(UserInfo);
                    }
                }

                //Update cookie with selected store
                if(Account != null && !string.IsNullOrEmpty(Account.AccountID))
                {
                    SaveAccountUpdate(Account.AccountID);
                }
            }
            else
            {
                foreach (ImageSolutions.User.UserInfo _UserInfo in CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.UserInfos)
                {
                    ImageSolutions.User.UserWebsite UserWebSite = new ImageSolutions.User.UserWebsite();
                    ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                    UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.UserInfoID.SearchString = _UserInfo.UserInfoID;
                    UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    UserWebSite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                    if (UserWebSite != null && !UserWebSite.InActive)
                    {
                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo(_UserInfo.UserInfoID);
                        if (!string.IsNullOrEmpty(UserWebSite.EmployeeID))
                        {
                            UserInfo.LastName = String.Format("{0} - {1}", UserInfo.LastName, UserWebSite.EmployeeID);
                        }

                        UserInfos.Add(UserInfo);
                    }
                }
            }


            ddlUserInfo.DataSource = UserInfos.OrderBy(x => x.FullName);
            ddlUserInfo.DataBind();

            //ddlUserInfo.Items.Insert(0, new ListItem("", ""));

            if(UserInfos.Exists(x => x.UserInfoID == CurrentUser.UserInfoID))
            {
                ddlUserInfo.SelectedValue = CurrentUser.UserInfoID;
            }
        }

        protected void SaveAccountUpdate(string accountid)
        {
            //HttpContext.Current.Response.Cookies["AccountUpdate"].Value = null;
            //HttpContext.Current.Response.Cookies["AccountUpdate"].Expires = System.DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies["AccountUpdate"].Value = accountid;
        }

        protected bool IsDisplayAccountUserItem(string itemid)
        {
            bool blnReturn = false;

            if(!_Item.ExcludeDisplayUser)
            {
                if (CurrentWebsite.DisplayUserPermission == "All")
                {
                    blnReturn = true;
                }
                else if (CurrentWebsite.DisplayUserPermission == "All With Account Change")
                {
                    blnReturn = true;

                    phEmployeeAccount.Visible = true;


                    if (CurrentWebsite.Accounts.Count <= 100)
                    {
                        ddlAccount.Visible = true;
                        ddlAccount.DataSource = CurrentWebsite.Accounts.OrderBy(x => x.AccountName);
                        ddlAccount.DataBind();
                        
                        if (string.IsNullOrEmpty(hfAccountID.Value))
                        {
                            ddlAccount.SelectedValue = CurrentUser.CurrentUserWebSite.CurrentUserAccount.AccountID;
                        }

                        txtAccount.Visible = false;
                    }
                    else
                    {
                        ddlAccount.Visible = false;
                        txtAccount.Visible = true;

                        if (string.IsNullOrEmpty(hfAccountID.Value))
                        {
                            txtAccount.Text = Convert.ToString(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountName);
                            hfAccountID.Value = Convert.ToString(CurrentUser.CurrentUserWebSite.CurrentUserAccount.Account.AccountID);
                        }
                        else
                        {
                            ImageSolutions.Account.Account Account = new ImageSolutions.Account.Account(hfAccountID.Value);
                            if (Account != null && !string.IsNullOrEmpty(Account.AccountID))
                            {
                                txtAccount.Text = Account.AccountName;
                            }
                        }
                    }

                }
                else if (CurrentWebsite.DisplayUserPermission == "Store Only")
                {
                    if (CurrentUser.CurrentUserWebSite.IsStore)
                    {
                        blnReturn = true;
                    }
                }

                if (!blnReturn)
                {
                    List<ImageSolutions.Website.WebsiteTabItem> WebsiteTabItems = new List<ImageSolutions.Website.WebsiteTabItem>();
                    ImageSolutions.Website.WebsiteTabItemFilter WebsiteTabItemFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                    WebsiteTabItemFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteTabItemFilter.ItemID.SearchString = itemid;
                    WebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(WebsiteTabItemFilter);

                    foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in WebsiteTabItems)
                    {
                        if (!string.IsNullOrEmpty(_WebsiteTabItem.WebsiteTab.DisplayUserPermission))
                        {
                            if (_WebsiteTabItem.WebsiteTab.DisplayUserPermission == "All")
                            {
                                blnReturn = true;
                            }
                            else if (_WebsiteTabItem.WebsiteTab.DisplayUserPermission == "Store Only")
                            {
                                if (CurrentUser.CurrentUserWebSite.IsStore)
                                {
                                    blnReturn = true;
                                }
                            }
                        }
                    }
                }
            }
            
            return blnReturn;
        }

        protected void DisplayCustomization()
        {
            if(btnAddToCart.Visible)
            {
                if (
                    (_Item.EnableSelectableLogo && _Item.ItemSelectableLogos != null && _Item.ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive).Count > 0)
                    ||
                    (_Item.EnablePersonalization && _Item.ItemPersonalizations != null && _Item.ItemPersonalizations.FindAll(x => !x.InActive).Count > 0)
                )
                {
                    this.phCustomization.Visible = true;

                    if (_Item.ItemSelectableLogos != null && _Item.EnableSelectableLogo && _Item.ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive).Count > 0)
                    {
                        phLogo.Visible = true;
                        chkNoLogo.Visible = !_Item.RequireLogoSelection;

                        //BindItemSelectableLogo();
                        BindItemSelectableLogoImage();
                    }
                    else
                    {
                        phLogo.Visible = false;
                    }

                    if (_Item.ItemPersonalizations != null && _Item.EnablePersonalization && _Item.ItemPersonalizations.FindAll(x => !x.InActive).Count > 0)
                    {
                        phPersonalization.Visible = true;
                        BindItemPersonalization();
                    }
                    else
                    {
                        phPersonalization.Visible = false;
                    }
                }

                btnAddMore.Visible = phPersonalization.Visible || phLogo.Visible;
            }
            else
            {
                phLogo.Visible = false;
                phPersonalization.Visible = false;
                btnAddMore.Visible = false;
            }
        }

        //protected void BindItemSelectableLogo()
        //{
        //    try
        //    {
        //        ddlItemSelectableLogo.DataSource = _Item.ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive).OrderBy(x => x.SelectableLogo.Name);
        //        ddlItemSelectableLogo.DataBind();

        //        ddlItemSelectableLogo.Items.Insert(0, new ListItem(string.Empty, string.Empty));
        //        ddlItemSelectableLogo.SelectedIndex = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally { }
        //}
        protected void BindItemSelectableLogoImage()
        {
            try
            {
                List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = new List<ImageSolutions.Item.ItemSelectableLogo>();

                if (!string.IsNullOrEmpty(hfSelectedGroupByAttributeValueID.Value))
                {
                    foreach (ImageSolutions.Item.ItemSelectableLogo _ItemSelectableLogo in _Item.ItemSelectableLogos)
                    {
                        ImageSolutions.Item.ItemSelectableLogoExcludeAttribute ItemSelectableLogoExcludeAttribute = new ImageSolutions.Item.ItemSelectableLogoExcludeAttribute();
                        ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter ItemSelectableLogoExcludeAttributeFilter = new ImageSolutions.Item.ItemSelectableLogoExcludeAttributeFilter();
                        ItemSelectableLogoExcludeAttributeFilter.ItemSelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
                        ItemSelectableLogoExcludeAttributeFilter.ItemSelectableLogoID.SearchString = _ItemSelectableLogo.ItemSelectableLogoID;
                        ItemSelectableLogoExcludeAttributeFilter.AttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                        ItemSelectableLogoExcludeAttributeFilter.AttributeValueID.SearchString = Convert.ToString(hfSelectedGroupByAttributeValueID.Value);
                        ItemSelectableLogoExcludeAttribute = ImageSolutions.Item.ItemSelectableLogoExcludeAttribute.GetItemSelectableLogoExcludeAttribute(ItemSelectableLogoExcludeAttributeFilter);

                        if (ItemSelectableLogoExcludeAttribute == null)
                        {
                            ItemSelectableLogos.Add(_ItemSelectableLogo);
                        }
                    }
                }
                else
                {
                    ItemSelectableLogos = _Item.ItemSelectableLogos;
                }

                rptSelectableLogo.DataSource = ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive && !x.SelectableLogo.IsPersonalization).OrderBy(x => x.SelectableLogo.Name);
                rptSelectableLogo.DataBind();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void BindItemPersonalization()
        {
            try
            {
                rptItemPersonalization.DataSource = _Item.ItemPersonalizations.FindAll(x => !x.InActive);
                rptItemPersonalization.DataBind();

                SetPersonalizationBasePrice();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        protected void BindItems()
        {
            List<ImageSolutions.Website.WebsiteTabItem> objWebsiteTabItems = null;
            ImageSolutions.Website.WebsiteTabItemFilter objFilter = null;
            ImageSolutions.Item.Item objItem = null;

            try
            {
                List<ImageSolutions.Item.MyGroupItem> MyGroupItems = new List<ImageSolutions.Item.MyGroupItem>();

                if (!string.IsNullOrEmpty(mWebSiteTabID))
                {
                    objFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                    objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.WebsiteTabID.SearchString = mWebSiteTabID;
                    objWebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(objFilter);
                    //rptItems.DataSource = objWebsiteTabItems;

                    foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in objWebsiteTabItems)
                    {
                        //MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, _WebsiteTabItem.ItemID));
                        MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, mItemID));
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(mItemID))
                {
                    objItem = new ImageSolutions.Item.Item(mItemID);
                    if (objItem.SuperceedingItems != null)
                    {
                        foreach (ImageSolutions.Item.SuperceedingItem objSuperceedingItem in objItem.SuperceedingItems)
                        {
                            MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, objSuperceedingItem.ReplacementItemID));
                            break;
                        }
                    }
                }

                rptItems.DataSource = MyGroupItems;
                rptItems.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteTabItems = null;
                objFilter = null;
            }
        }

        protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    //ImageSolutions.Website.WebsiteTabItem objWebsiteTabItem = (ImageSolutions.Website.WebsiteTabItem)e.Item.DataItem;
                    ImageSolutions.Item.MyGroupItem MyGroupItem = (ImageSolutions.Item.MyGroupItem)e.Item.DataItem;
                    ImageSolutions.Item.Item Item = MyGroupItem.Item;

                    List<ImageSolutions.Attribute.Attribute> Attributes = new List<ImageSolutions.Attribute.Attribute>();
                    ImageSolutions.Attribute.AttributeFilter AttributeFilter = new ImageSolutions.Attribute.AttributeFilter();
                    AttributeFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeFilter.ItemID.SearchString = Item.ItemID;
                    Attributes = ImageSolutions.Attribute.Attribute.GetAttributes(AttributeFilter);

                    Panel pnlAttribute = (Panel)e.Item.FindControl("pnlAttribute");
                    pnlAttribute.Visible = Attributes != null && Attributes.Count > 0;

                    Panel pnlNoAttribute = (Panel)e.Item.FindControl("pnlNoAttribute");
                    pnlNoAttribute.Visible = !pnlAttribute.Visible;

                    if (pnlAttribute.Visible)
                    {
                        if (!string.IsNullOrEmpty(Item.GroupByAttributeID))
                        {
                            Panel pnlGroup = (Panel)e.Item.FindControl("pnlGroup");
                            pnlGroup.Visible = true;

                            int AttributeCount = Attributes.Count(x => x.AttributeID != Item.GroupByAttributeID);

                            if (AttributeCount == 1)
                            {
                                Repeater rptGroupByAttribute = (Repeater)e.Item.FindControl("rptGroupByAttribute");
                                List<ImageSolutions.Attribute.AttributeValue> objColors = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues;

                                if (string.IsNullOrEmpty(hfSelectedGroupByAttributeValueID.Value) && objColors != null && objColors.Count > 1)
                                {
                                    mSelectedGroupByAttributeValueID = objColors.FirstOrDefault().AttributeValueID;
                                    hfSelectedGroupByAttributeValueID.Value = mSelectedGroupByAttributeValueID;
                                }

                                rptGroupByAttribute.DataSource = objColors;
                                rptGroupByAttribute.DataBind();

                                mGroupByAttributeValueCount = rptGroupByAttribute.Items == null ? 0 : rptGroupByAttribute.Items.Count;
                                if (rptGroupByAttribute.Items == null || rptGroupByAttribute.Items.Count == 1)
                                {
                                    if (rptGroupByAttribute.Items != null && rptGroupByAttribute.Items.Count == 1) mSelectedGroupByAttributeValueID = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues[0].AttributeValueID;
                                    rptGroupByAttribute.Visible = false;
                                    hfSelectedGroupByAttributeValueID.Value = mSelectedGroupByAttributeValueID;
                                }

                                if(Item.UseLengthAndWidth)
                                {
                                    Panel pnlLengthWidthAttribute = (Panel)e.Item.FindControl("pnlLengthWidthAttribute");
                                    pnlLengthWidthAttribute.Visible = true;

                                    if (!string.IsNullOrEmpty(mSelectedGroupByAttributeValueID))
                                    {
                                        DropDownList ddlLengthWidthAttribute = (DropDownList)e.Item.FindControl("ddlLengthWidthAttribute");
                                        ddlLengthWidthAttribute.DataSource = Attributes.Find(x => x.AttributeID == Item.GroupByAttributeID).AttributeValues.OrderBy(x => x.Sort);
                                        ddlLengthWidthAttribute.DataBind();
                                        btnAddToCart.Visible = true;
                                    }
                                    else
                                    {
                                        btnAddToCart.Visible = false;
                                    }
                                }
                                else
                                {
                                    Panel pnlGroupSingleAttribute = (Panel)e.Item.FindControl("pnlGroupSingleAttribute");
                                    pnlGroupSingleAttribute.Visible = true;

                                    if (!string.IsNullOrEmpty(mSelectedGroupByAttributeValueID))
                                    {
                                        Repeater rptGroupSingleAttributeValue = (Repeater)e.Item.FindControl("rptGroupSingleAttributeValue");
                                        rptGroupSingleAttributeValue.DataSource = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues.FindAll(m => m.AttributeValueID == mSelectedGroupByAttributeValueID);
                                        rptGroupSingleAttributeValue.DataBind();
                                        btnAddToCart.Visible = true;
                                    }
                                    else
                                    {
                                        //Repeater rptGroupSingleAttributeValue = (Repeater)e.Item.FindControl("rptGroupSingleAttributeValue");
                                        //rptGroupSingleAttributeValue.DataSource = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues;
                                        //rptGroupSingleAttributeValue.DataBind();
                                        btnAddToCart.Visible = false;
                                    }
                                }
                            }
                            else
                            {
                                //Panel pnlGroupMultipleAttribute = (Panel)e.Item.FindControl("pnlGroupMultipleAttribute");
                                //pnlGroupMultipleAttribute.Visible = true;

                                //Repeater rptMultipleGroupAttributes = (Repeater)e.Item.FindControl("rptMultipleGroupAttributes");
                                //rptMultipleGroupAttributes.DataSource = Attributes.FindAll(x => x.AttributeID != Item.GroupByAttributeID);
                                //rptMultipleGroupAttributes.DataBind();

                                //List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                                //ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                                //AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                                //AttributeValueFilter.AttributeID.SearchString = Item.GroupByAttributeID;
                                //AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                                //Repeater rptMutlipleGroupAttributeValue = (Repeater)e.Item.FindControl("rptMutlipleGroupAttributeValue");
                                //rptMutlipleGroupAttributeValue.DataSource = AttributeValues;
                                //rptMutlipleGroupAttributeValue.DataBind();
                            }
                        }
                        else
                        {
                            Repeater rptNoGroupAttributes = (Repeater)e.Item.FindControl("rptNoGroupAttributes");
                            rptNoGroupAttributes.DataSource = Attributes.FindAll(x => x.AttributeID != Item.GroupByAttributeID);  //DropDownAttributes;
                            rptNoGroupAttributes.DataBind();

                            Panel pnlNoGroup = (Panel)e.Item.FindControl("pnlNoGroup");
                            pnlNoGroup.Visible = true;
                            btnAddToCart.Visible = true;
                        }
                    }
                    else
                    {
                        TextBox txtQuantity = (TextBox)e.Item.FindControl("txtQuantity");
                    }
                }

                DisplayCustomization();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }


        protected void ddlAttributeValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList DropDownList = (DropDownList)sender;
            string strAttributeValueID = Convert.ToString(DropDownList.SelectedValue);

            ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(strAttributeValueID);
            string strItemID = AttributeValue.Attribute.ItemID;
            SetQuantityField();
        }

        protected void SetQuantityField()
        {
            foreach (RepeaterItem _Item in rptItems.Items)
            {
                Panel pnlNoAttribute = (Panel)_Item.FindControl("pnlNoAttribute");
                Panel pnlNoGroup = (Panel)_Item.FindControl("pnlNoGroup");
                Panel pnlGroupSingleAttribute = (Panel)_Item.FindControl("pnlGroupSingleAttribute");
                Panel pnlGroupMultipleAttribute = (Panel)_Item.FindControl("pnlGroupMultipleAttribute");

                Panel pnlLengthWidthAttribute = (Panel)_Item.FindControl("pnlLengthWidthAttribute");

                if (pnlLengthWidthAttribute.Visible)
                {
                    DropDownList ddlLengthWidthAttribute = (DropDownList)_Item.FindControl("ddlLengthWidthAttribute");                    
                    Label lblLengthWidthAttributeUnitPrice = (Label)_Item.FindControl("lblLengthWidthAttributeUnitPrice");

                    if(!string.IsNullOrEmpty(mSelectedGroupByAttributeValueID))
                    {
                        List<string> AttributeValueIDs = new List<string>();
                        AttributeValueIDs.Add(mSelectedGroupByAttributeValueID);
                        AttributeValueIDs.Add(ddlLengthWidthAttribute.SelectedValue);
                        ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);

                        lblLengthWidthAttributeUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;

                        if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                            && CurrentWebsite.CurrentyConvertPercentage != null
                            && CurrentWebsite.CurrentyConvertPercentage > 0
                            && !string.IsNullOrEmpty(lblLengthWidthAttributeUnitPrice.Text))
                        {
                            lblLengthWidthAttributeUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.BasePrice != null)
                                ? string.Format(@"{0:c} USD <span style=""font-size: small; "" title=""est. {1:c} {2}""><i class=""ti-info-alt"" ></i> </span>"
                                    , Convert.ToDecimal(Item.BasePrice)
                                    , Convert.ToDecimal(Item.BasePrice) * CurrentWebsite.CurrentyConvertPercentage
                                    , CurrentWebsite.CurrencyConvert
                                    )
                                : string.Empty;
                        }


                        Label lblLengthWidthAttributeQuantityAvailable = (Label)_Item.FindControl("lblLengthWidthAttributeQuantityAvailable");
                        lblLengthWidthAttributeQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.AvailableInventory + " Available" : "0 Available";
                        lblLengthWidthAttributeQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory
                             && Item != null
                             && (
                                !Item.AllowBackOrder
                                ||
                                ( Item.AllowBackOrder && Item.AvailableInventory > 0 )
                            )
                            && (
                                !Item.IsNonInventory
                            );

                        TextBox txtLengthWidthAttributeQuantity = (TextBox)_Item.FindControl("txtLengthWidthAttributeQuantity");
                        txtLengthWidthAttributeQuantity.Enabled = Item != null && (Item.AvailableInventory > 0 || Item.AllowBackOrder || CurrentWebsite.AllowBackOrderForAllItems);

                        //LinkButton lbnLengthWidthAttributeSuperceedingItem = (LinkButton)_RepeaterItem.FindControl("lbnLengthWidthAttributeSuperceedingItem");
                    }
                    else
                    {
                        pnlLengthWidthAttribute.Visible = false;
                    }

                }
                else if (pnlNoAttribute.Visible)
                {
                    string strItemID = ((HiddenField)_Item.FindControl("hfItemID")).Value;
                    ImageSolutions.Item.Item objItem = new ImageSolutions.Item.Item(strItemID);

                    imgItem.ImageUrl = objItem.DisplayImageURL;
                    imgItem2.ImageUrl = objItem.DisplayImageURL;

                    TextBox txtQuantity = (TextBox)_Item.FindControl("txtQuantity");
                    txtQuantity.Enabled = objItem != null && (objItem.AvailableInventory > 0 || objItem.AllowBackOrder || CurrentWebsite.AllowBackOrderForAllItems);

                    Label lblUnitPrice = (Label)_Item.FindControl("lblUnitPrice");
                    lblUnitPrice.Text = String.Format("{0:c}", objItem.BasePrice);

                    if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                        && CurrentWebsite.CurrentyConvertPercentage != null
                        && CurrentWebsite.CurrentyConvertPercentage > 0
                        && !string.IsNullOrEmpty(lblUnitPrice.Text))
                    {
                        lblUnitPrice.Text = string.Format(@"{0:c} USD <span style=""font-size: small; "" title=""est. {1:c} {2}""><i class=""ti-info-alt"" ></i> </span>"
                            , Convert.ToDecimal(objItem.BasePrice)
                            , Convert.ToDecimal(objItem.BasePrice) * CurrentWebsite.CurrentyConvertPercentage
                            , CurrentWebsite.CurrencyConvert
                            );
                    }

                    Label lblQuantityAvailable = (Label)_Item.FindControl("lblQuantityAvailable");
                    lblQuantityAvailable.Text = objItem.AvailableInventory.ToString() + " Available";
                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory 
                        && objItem != null
                        && (
                        !objItem.AllowBackOrder
                        ||
                        objItem.AllowBackOrder && objItem.AvailableInventory > 0
                        )
                        && (
                            !objItem.IsNonInventory
                        );
                }
                else if (pnlNoGroup.Visible)
                {
                    TextBox txtNoGroupAttributeQuantity = (TextBox)_Item.FindControl("txtNoGroupAttributeQuantity");

                    List<string> AttributeValueIDs = new List<string>();
                    Repeater rptNoGroupAttributes = (Repeater)_Item.FindControl("rptNoGroupAttributes");
                    foreach (RepeaterItem _RepeaterItem in rptNoGroupAttributes.Items)
                    {
                        DropDownList ddlAttributeValue = (DropDownList)_RepeaterItem.FindControl("ddlAttributeValue");
                        AttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                    }

                    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                    
                    if (Item != null)
                    {
                        imgItem.ImageUrl = Item.DisplayImageURL;
                        imgItem2.ImageUrl = Item.DisplayImageURL;
                    }

                    txtNoGroupAttributeQuantity.Enabled = Item != null && (Item.AvailableInventory > 0 || Item.AllowBackOrder || CurrentWebsite.AllowBackOrderForAllItems); 

                    Label lblUnitPrice = (Label)_Item.FindControl("lblNoGroupAttributeUnitPrice");
                    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;

                    if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                        && CurrentWebsite.CurrentyConvertPercentage != null
                        && CurrentWebsite.CurrentyConvertPercentage > 0
                        && !string.IsNullOrEmpty(lblUnitPrice.Text))
                    {
                        lblUnitPrice.Text = string.Format(@"{0:c} USD <span style=""font-size: small; "" title=""est. {1:c} {2}""><i class=""ti-info-alt"" ></i> </span>"
                            , Convert.ToDecimal(Item.BasePrice)
                            , Convert.ToDecimal(Item.BasePrice) * CurrentWebsite.CurrentyConvertPercentage
                            , CurrentWebsite.CurrencyConvert
                            );
                    }


                    Label lblQuantityAvailable = (Label)_Item.FindControl("lblNoGroupAttributeQuantityAvailable");
                    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.AvailableInventory + " Available" : "0 Available";
                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory
                        && Item != null
                        && (
                            !Item.AllowBackOrder
                            ||
                            Item.AllowBackOrder && Item.AvailableInventory > 0
                        )
                        && (
                            !Item.IsNonInventory
                        );
                }
                else
                {
                    if (pnlGroupSingleAttribute.Visible)
                    {
                        Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");

                        foreach (RepeaterItem _RepeaterItem in rptGroupSingleAttributeValue.Items)
                        {
                            List<string> AttributeValueIDs = new List<string>();

                            HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                            AttributeValueIDs.Add(hfAttributeValueID.Value);

                            if (CurrentWebsite.ProductDetailDisplayType == "Grid")
                            {
                                Repeater rptGroupAttribute = (Repeater)_RepeaterItem.FindControl("rptGroupAttribute");
                                rptGroupAttribute.Visible = true;
                                foreach (RepeaterItem _ListRepeaterItem in rptGroupAttribute.Items)
                                {
                                    TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                                    HiddenField hfListAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfListAttributeValueID");
                                    AttributeValueIDs.Add(hfListAttributeValueID.Value);

                                    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                                    if (Item != null)
                                    {
                                        imgItem.ImageUrl = Item.DisplayImageURL;
                                        imgItem2.ImageUrl = Item.DisplayImageURL;
                                    }
                                    txtGroupAttributeQuantity.Enabled = Item != null && (Item.AvailableInventory > 0 || Item.AllowBackOrder || CurrentWebsite.AllowBackOrderForAllItems);

                                    Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                                    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;

                                    if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                                        && CurrentWebsite.CurrentyConvertPercentage != null
                                        && CurrentWebsite.CurrentyConvertPercentage > 0
                                        && !string.IsNullOrEmpty(lblUnitPrice.Text))
                                    {
                                        lblUnitPrice.Text = string.Format(@"{0:c} USD <span style=""font-size: small; "" title=""est. {1:c} {2}""><i class=""ti-info-alt"" ></i> </span>"
                                            , Convert.ToDecimal(Item.BasePrice)
                                            , Convert.ToDecimal(Item.BasePrice) * CurrentWebsite.CurrentyConvertPercentage
                                            , CurrentWebsite.CurrencyConvert
                                            );
                                    }

                                    Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                                    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.AvailableInventory + " Available" : "0 Available";
                                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory
                                         && Item != null
                                         && (
                                            !Item.AllowBackOrder
                                            ||
                                            Item.AllowBackOrder && Item.AvailableInventory > 0
                                        )
                                        && (
                                            !Item.IsNonInventory
                                        );

                                    LinkButton lbnGroupAttributeSuperceedingItem = (LinkButton)_ListRepeaterItem.FindControl("lbnGroupAttributeSuperceedingItem");
                                    lbnGroupAttributeSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                                    lbnGroupAttributeSuperceedingItem.CommandArgument = Item != null ? Item.ItemID : String.Empty;

                                    //HtmlAnchor aSuperceedingItem = (HtmlAnchor)_ListRepeaterItem.FindControl("aGroupAttributeSuperceedingItem");
                                    //aSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                                    //if (Item != null)
                                    //{
                                    //    aSuperceedingItem.HRef = "/ItemList.aspx?itemid=" + (Item.ParentItem != null ? Item.ParentItem.ItemID : Item.ItemID);
                                    //}
                                    AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                                }
                            }
                            else if (CurrentWebsite.ProductDetailDisplayType == "List")
                            {
                                GridView gvGroupAttribute = (GridView)_RepeaterItem.FindControl("gvGroupAttribute");
                                gvGroupAttribute.Visible = true;
                                foreach (GridViewRow _ListRepeaterItem in gvGroupAttribute.Rows)
                                {
                                    TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                                    HiddenField hfListAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfListAttributeValueID");
                                    AttributeValueIDs.Add(hfListAttributeValueID.Value);

                                    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                                    if (Item != null)
                                    {
                                        imgItem.ImageUrl = Item.DisplayImageURL;
                                        imgItem2.ImageUrl = Item.DisplayImageURL;
                                    }
                                    txtGroupAttributeQuantity.Enabled = Item != null && (Item.AvailableInventory > 0 || Item.AllowBackOrder || CurrentWebsite.AllowBackOrderForAllItems);

                                    Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                                    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;

                                    if (!string.IsNullOrEmpty(CurrentWebsite.CurrencyConvert)
                                        && CurrentWebsite.CurrentyConvertPercentage != null
                                        && CurrentWebsite.CurrentyConvertPercentage > 0
                                        && !string.IsNullOrEmpty(lblUnitPrice.Text))
                                    {
                                        lblUnitPrice.Text = string.Format(@"{0:c} USD <span style=""font-size: small; "" title=""est. {1:c} {2}""><i class=""ti-info-alt"" ></i> </span>"
                                            , Convert.ToDecimal(Item.BasePrice)
                                            , Convert.ToDecimal(Item.BasePrice) * CurrentWebsite.CurrentyConvertPercentage
                                            , CurrentWebsite.CurrencyConvert
                                            );
                                    }

                                    Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                                    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.AvailableInventory + " Available" : "0 Available";
                                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory
                                        && Item != null
                                        && (
                                            !Item.AllowBackOrder
                                            ||
                                            Item.AllowBackOrder && Item.AvailableInventory > 0
                                        )
                                        && (
                                            !Item.IsNonInventory
                                        )
                                        ;

                                    LinkButton lbnGroupAttributeSuperceedingItem = (LinkButton)_ListRepeaterItem.FindControl("lbnGroupAttributeSuperceedingItem");
                                    lbnGroupAttributeSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                                    lbnGroupAttributeSuperceedingItem.CommandArgument = Item != null ? Item.ItemID : String.Empty;

                                    //HtmlAnchor aSuperceedingItem = (HtmlAnchor)_ListRepeaterItem.FindControl("aGroupAttributeSuperceedingItem");
                                    //aSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                                    //if (Item != null)
                                    //{
                                    //    aSuperceedingItem.HRef = "/ItemList.aspx?itemid=" + (Item.ParentItem != null ? Item.ParentItem.ItemID : Item.ItemID);
                                    //}
                                    AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        //List<string> AttributeValueIDs = new List<string>();
                        //Repeater rptMultipleGroupAttributes = (Repeater)_Item.FindControl("rptMultipleGroupAttributes");
                        //foreach (RepeaterItem _RepeaterAttributeItem in rptMultipleGroupAttributes.Items)
                        //{
                        //    DropDownList ddlAttributeValue = (DropDownList)_RepeaterAttributeItem.FindControl("ddlAttributeValue");
                        //    AttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                        //}
                        //Repeater rptMutlipleGroupAttributeValue = (Repeater)_Item.FindControl("rptMutlipleGroupAttributeValue");
                        //foreach (RepeaterItem _ListRepeaterItem in rptMutlipleGroupAttributeValue.Items)
                        //{
                        //    TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                        //    HiddenField hfAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfAttributeValueID");
                        //    AttributeValueIDs.Add(hfAttributeValueID.Value);

                        //    //Add to Cart
                        //    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                        //    txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.AvailableInventory > 0;

                        //    Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                        //    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;

                        //    Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                        //    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.AvailableInventory + " Available" : "0 Available";
                        //    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;

                        //    LinkButton lbnGroupAttributeSuperceedingItem = (LinkButton)_ListRepeaterItem.FindControl("lbnGroupAttributeSuperceedingItem");
                        //    lbnGroupAttributeSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                        //    lbnGroupAttributeSuperceedingItem.CommandArgument = Item != null ? Item.ItemID : String.Empty;


                        //    AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                        //}

                    }
                }
            }
        }

        protected ImageSolutions.Item.Item FindVariationItem(List<string> attributevalueids)
        {
            try
            {
                List<string> ItemIDs = new List<string>();

                foreach (string _AttributeValueID in attributevalueids)
                {
                    List<ImageSolutions.Item.ItemAttributeValue> ItemAttributeValues = new List<ImageSolutions.Item.ItemAttributeValue>();
                    ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                    ItemAttributeValueFilter.AttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                    ItemAttributeValueFilter.AttributeValueID.SearchString = _AttributeValueID;
                    ItemAttributeValues = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValues(ItemAttributeValueFilter);

                    if (ItemAttributeValues != null && ItemAttributeValues.Count > 0)
                    {
                        if (ItemIDs.Count == 0)
                        {
                            foreach (ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                            {
                                ItemIDs.Add(_ItemAttributeValue.ItemID);
                            }
                        }
                        else
                        {
                            List<string> UpdateItemIds = new List<string>();
                            foreach (ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                            {
                                if (ItemIDs.Contains(_ItemAttributeValue.ItemID))
                                {
                                    UpdateItemIds.Add(_ItemAttributeValue.ItemID);
                                }
                            }

                            ItemIDs = UpdateItemIds;

                            if (ItemIDs.Count == 0)
                            {
                                return null;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                if (ItemIDs.Count == 1)
                {
                    return new ImageSolutions.Item.Item(Convert.ToString(ItemIDs[0]));
                }
                else if (ItemIDs.Count == 0)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Multiple items assigned");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        protected void rptAttributes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hfAttributeID = (HiddenField)e.Item.FindControl("hfAttributeID");
                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = Convert.ToString(hfAttributeID.Value);
                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                    DropDownList ddlAttributeValue = (DropDownList)e.Item.FindControl("ddlAttributeValue");
                    ddlAttributeValue.DataSource = AttributeValues;
                    ddlAttributeValue.DataTextField = "Value";
                    ddlAttributeValue.DataValueField = "AttributeValueID";
                    ddlAttributeValue.DataBind();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void rptGroupAttribute_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string strItemID = e.CommandArgument.ToString();
            ucSuperceedingItem.ItemID = strItemID;
            ucSuperceedingItem.Show();
        }

        protected void rptGroupSingleAttributeValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Header)
                {
                    HtmlContainerControl hHeader = (HtmlContainerControl)e.Item.FindControl("hHeader");
                    hHeader.InnerHtml = "select size";
                    hHeader.Visible = mGroupByAttributeValueCount > 1;
                }
                else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hfAttributeValueID = (HiddenField)e.Item.FindControl("hfAttributeValueID");
                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(hfAttributeValueID.Value);

                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = AttributeValue.Attribute.Item.GroupByAttributeID;
                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);


                    if (_Item.UseLengthAndWidth)
                    {
                        DropDownList ddlLengthWidthAttribute = (DropDownList)e.Item.FindControl("ddlLengthWidthAttribute");
                        ddlLengthWidthAttribute.DataSource = AttributeValues.OrderBy(m => m.Sort);
                        ddlLengthWidthAttribute.DataBind();
                    }
                    else
                    if (CurrentWebsite.ProductDetailDisplayType == "Grid")
                    {
                        Repeater rptGroupAttribute = (Repeater)e.Item.FindControl("rptGroupAttribute");
                        rptGroupAttribute.DataSource = AttributeValues.OrderBy(m => m.Sort);
                        rptGroupAttribute.DataBind();
                    }
                    else if (CurrentWebsite.ProductDetailDisplayType == "List")
                    {
                        GridView gvGroupAttribute = (GridView)e.Item.FindControl("gvGroupAttribute");
                        gvGroupAttribute.DataSource = AttributeValues.OrderBy(m => m.Sort);
                        gvGroupAttribute.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            AddToCart(true);            
        }

        public void AddToCart(bool gotoshoppingcart)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                bool HasCustomizedItem = !string.IsNullOrEmpty(hfSelectedLogo.Value) || (rptItemPersonalization.Items != null && rptItemPersonalization.Items.Count > 0);
                foreach (RepeaterItem _Item in this.rptItems.Items)
                {
                    string strItemID = ((HiddenField)_Item.FindControl("hfItemID")).Value;

                    //No Attribute
                    TextBox txtQuantity = (TextBox)_Item.FindControl("txtQuantity");
                    if (txtQuantity != null && !string.IsNullOrEmpty(txtQuantity.Text) && Convert.ToInt32(txtQuantity.Text) > 0)
                    {
                        //ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);
                        //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        //newShoppingCartLine.ItemID = strItemID;
                        //newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                        //newShoppingCartLine.UnitPrice = Item.BasePrice == null ? (double)0 : Item.BasePrice.Value;
                        //newShoppingCartLine.Create();

                        ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == strItemID);
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(HasCustomizedItem);
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = strItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                        newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                        //if (_MyGroupItem.Item.ItemNumber == "IS700N-123456")
                        //{
                        //    newShoppingCartLine.CustomDesignImagePath = imgItem.ImageUrl;
                        //    newShoppingCartLine.CustomDesignName = this.txtCustomDesignName.Text.Trim();
                        //}
                        if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
                        {
                            newShoppingCartLine.CustomDesignImagePath = imgItem.ImageUrl;
                        }

                        decimal decAvailable = MyGroupItemWebsite.Item.IsNonInventory ? Convert.ToDecimal(MyGroupItemWebsite.Item.VendorInventory) : Convert.ToDecimal(MyGroupItemWebsite.Item.QuantityAvailable);
                        if (!MyGroupItemWebsite.Item.AllowBackOrder && newShoppingCartLine.Quantity > decAvailable && !CurrentWebsite.AllowBackOrderForAllItems)
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", MyGroupItemWebsite.Item.ItemName, MyGroupItemWebsite.Item.IsNonInventory ? MyGroupItemWebsite.Item.VendorInventory : MyGroupItemWebsite.Item.QuantityAvailable));
                        }

                        if(phEmployee.Visible)
                        {
                            if(string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                            {
                                throw new Exception("Employee must be selected");
                            }

                            newShoppingCartLine.UserInfoID = ddlUserInfo.SelectedValue;
                        }

                        newShoppingCartLine.Create(objConn, objTran);

                        SaveSelectableLogo(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);
                        SavePersonalization(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);

                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update(objConn, objTran);

                        txtQuantity.Text = String.Empty;
                    }

                    //Attribute - No Group
                    Repeater rptNoGroupAttributes = (Repeater)_Item.FindControl("rptNoGroupAttributes");

                    TextBox txtNoGroupAttributeQuantity = (TextBox)_Item.FindControl("txtNoGroupAttributeQuantity");

                    if (txtNoGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtNoGroupAttributeQuantity.Text) && Convert.ToInt32(txtNoGroupAttributeQuantity.Text) > 0)
                    {
                        List<string> NoGroupAttributeValueIDs = new List<string>();
                        foreach (RepeaterItem _AttributeItem in rptNoGroupAttributes.Items)
                        {
                            DropDownList ddlAttributeValue = (DropDownList)_AttributeItem.FindControl("ddlAttributeValue");
                            NoGroupAttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                        }
                        ImageSolutions.Item.Item NoGroupItem = FindVariationItem(NoGroupAttributeValueIDs);

                        //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        //newShoppingCartLine.ItemID = NoGroupItem.ItemID;
                        //newShoppingCartLine.Quantity = Convert.ToInt32(txtNoGroupAttributeQuantity.Text);
                        //newShoppingCartLine.UnitPrice = NoGroupItem.BasePrice == null ? (double)0 : NoGroupItem.BasePrice.Value;
                        //newShoppingCartLine.Create();

                        ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == NoGroupItem.ItemID);
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(HasCustomizedItem);
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = NoGroupItem.ItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtNoGroupAttributeQuantity.Text); //Convert.ToInt32(txtQuantity.Text);
                        newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                        if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
                        {
                            newShoppingCartLine.CustomDesignImagePath = imgItem.ImageUrl;
                        }

                        decimal decAvailable = MyGroupItemWebsite.Item.IsNonInventory ? Convert.ToDecimal(MyGroupItemWebsite.Item.VendorInventory) : Convert.ToDecimal(MyGroupItemWebsite.Item.QuantityAvailable);
                        if (!MyGroupItemWebsite.Item.AllowBackOrder && newShoppingCartLine.Quantity > decAvailable && !CurrentWebsite.AllowBackOrderForAllItems)
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", MyGroupItemWebsite.Item.ItemName, MyGroupItemWebsite.Item.IsNonInventory ? MyGroupItemWebsite.Item.VendorInventory : MyGroupItemWebsite.Item.QuantityAvailable));
                        }

                        if (phEmployee.Visible)
                        {
                            if (string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                            {
                                throw new Exception("Employee must be selected");
                            }

                            newShoppingCartLine.UserInfoID = ddlUserInfo.SelectedValue;
                        }

                        newShoppingCartLine.Create(objConn, objTran);

                        SaveSelectableLogo(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);
                        SavePersonalization(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);

                        txtNoGroupAttributeQuantity.Text = String.Empty;
                    }

                    //Attribute - Group - Single
                    Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");
                    foreach (RepeaterItem _AttributeItem in rptGroupSingleAttributeValue.Items)
                    {
                        List<string> GroupSingleAttributeValueIDs = new List<string>();

                        HiddenField hfAttributeValueID = (HiddenField)_AttributeItem.FindControl("hfAttributeValueID");
                        GroupSingleAttributeValueIDs.Add(hfAttributeValueID.Value);


                        if (CurrentWebsite.ProductDetailDisplayType == "Grid")
                        {
                            Repeater rptGroupAttribute = (Repeater)_AttributeItem.FindControl("rptGroupAttribute");
                            foreach (RepeaterItem _AttributeValueItem in rptGroupAttribute.Items)
                            {
                                TextBox txtGroupAttributeQuantity = (TextBox)_AttributeValueItem.FindControl("txtGroupAttributeQuantity");
                                if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text) && Convert.ToInt32(txtGroupAttributeQuantity.Text) > 0)
                                {
                                    HiddenField hfListAttributeValueID = (HiddenField)_AttributeValueItem.FindControl("hfListAttributeValueID");
                                    GroupSingleAttributeValueIDs.Add(hfListAttributeValueID.Value);
                                    ImageSolutions.Item.Item GroupItem = FindVariationItem(GroupSingleAttributeValueIDs);

                                    //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                                    //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                                    //newShoppingCartLine.ItemID = GroupItem.ItemID;
                                    //newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                                    //newShoppingCartLine.UnitPrice = GroupItem.BasePrice == null ? (double)0 : GroupItem.BasePrice.Value;
                                    //newShoppingCartLine.Create();

                                    ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == GroupItem.ItemID);
                                    ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(HasCustomizedItem);
                                    newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                                    newShoppingCartLine.ItemID = GroupItem.ItemID;
                                    newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                                    newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                                    if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
                                    {
                                        newShoppingCartLine.CustomDesignImagePath = imgItem.ImageUrl;
                                    }

                                    decimal decAvailable = MyGroupItemWebsite.Item.IsNonInventory ? Convert.ToDecimal(MyGroupItemWebsite.Item.VendorInventory) : Convert.ToDecimal(MyGroupItemWebsite.Item.QuantityAvailable);
                                    if (!MyGroupItemWebsite.Item.AllowBackOrder && newShoppingCartLine.Quantity > decAvailable && !CurrentWebsite.AllowBackOrderForAllItems)
                                    {
                                        throw new Exception(String.Format("Not enough inventory available {0} ({1})", MyGroupItemWebsite.Item.ItemName, MyGroupItemWebsite.Item.IsNonInventory ? MyGroupItemWebsite.Item.VendorInventory : MyGroupItemWebsite.Item.QuantityAvailable));
                                    }

                                    if (phEmployee.Visible)
                                    {
                                        if (string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                                        {
                                            throw new Exception("Employee must be selected");
                                        }

                                        newShoppingCartLine.UserInfoID = ddlUserInfo.SelectedValue;
                                    }

                                    newShoppingCartLine.Create(objConn, objTran);

                                    SaveSelectableLogo(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);
                                    SavePersonalization(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);

                                    txtGroupAttributeQuantity.Text = String.Empty;

                                    GroupSingleAttributeValueIDs.RemoveAt(GroupSingleAttributeValueIDs.Count - 1);
                                }
                            }
                        }
                        else if (CurrentWebsite.ProductDetailDisplayType == "List")
                        {
                            GridView gvGroupAttribute = (GridView)_AttributeItem.FindControl("gvGroupAttribute");
                            foreach (GridViewRow _AttributeValueItem in gvGroupAttribute.Rows)
                            {
                                TextBox txtGroupAttributeQuantity = (TextBox)_AttributeValueItem.FindControl("txtGroupAttributeQuantity");
                                if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text) && Convert.ToInt32(txtGroupAttributeQuantity.Text) > 0)
                                {
                                    HiddenField hfListAttributeValueID = (HiddenField)_AttributeValueItem.FindControl("hfListAttributeValueID");
                                    GroupSingleAttributeValueIDs.Add(hfListAttributeValueID.Value);
                                    ImageSolutions.Item.Item GroupItem = FindVariationItem(GroupSingleAttributeValueIDs);

                                    //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                                    //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                                    //newShoppingCartLine.ItemID = GroupItem.ItemID;
                                    //newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                                    //newShoppingCartLine.UnitPrice = GroupItem.BasePrice == null ? (double)0 : GroupItem.BasePrice.Value;
                                    //newShoppingCartLine.Create();

                                    ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == GroupItem.ItemID);
                                    ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(HasCustomizedItem);
                                    newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                                    newShoppingCartLine.ItemID = GroupItem.ItemID;
                                    newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                                    newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                                    if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
                                    {
                                        newShoppingCartLine.CustomDesignImagePath = imgItem.ImageUrl;
                                    }

                                    decimal decAvailable = MyGroupItemWebsite.Item.IsNonInventory ? Convert.ToDecimal(MyGroupItemWebsite.Item.VendorInventory) : Convert.ToDecimal(MyGroupItemWebsite.Item.QuantityAvailable);
                                    if (!MyGroupItemWebsite.Item.AllowBackOrder && newShoppingCartLine.Quantity > decAvailable && !CurrentWebsite.AllowBackOrderForAllItems)
                                    {
                                        throw new Exception(String.Format("Not enough inventory available {0} ({1})", MyGroupItemWebsite.Item.ItemName, MyGroupItemWebsite.Item.IsNonInventory ? MyGroupItemWebsite.Item.VendorInventory : MyGroupItemWebsite.Item.QuantityAvailable));
                                    }

                                    if (phEmployee.Visible)
                                    {
                                        if (string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                                        {
                                            throw new Exception("Employee must be selected");
                                        }

                                        newShoppingCartLine.UserInfoID = ddlUserInfo.SelectedValue;
                                    }

                                    newShoppingCartLine.Create(objConn, objTran);

                                    SaveSelectableLogo(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);
                                    SavePersonalization(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);

                                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update(objConn, objTran);

                                    txtGroupAttributeQuantity.Text = String.Empty;

                                    GroupSingleAttributeValueIDs.RemoveAt(GroupSingleAttributeValueIDs.Count - 1);
                                }
                            }
                        }
                    }

                    if (this._Item.UseLengthAndWidth)
                    {
                        DropDownList ddlLengthWidthAttribute = (DropDownList)_Item.FindControl("ddlLengthWidthAttribute");
                        TextBox txtLengthWidthAttributeQuantity = (TextBox)_Item.FindControl("txtLengthWidthAttributeQuantity");

                        if(string.IsNullOrEmpty(txtLengthWidthAttributeQuantity.Text) || Convert.ToInt32(txtLengthWidthAttributeQuantity.Text) == 0)
                        {
                            throw new Exception("Quantity required");
                        }
                        List<string> AttributeValueIDs = new List<string>();
                        AttributeValueIDs.Add(mSelectedGroupByAttributeValueID);
                        AttributeValueIDs.Add(ddlLengthWidthAttribute.SelectedValue);
                        ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);

                        ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID);
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine(HasCustomizedItem);
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = Item.ItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtLengthWidthAttributeQuantity.Text);
                        newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                        if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
                        {
                            newShoppingCartLine.CustomDesignImagePath = imgItem.ImageUrl;
                        }

                        decimal decAvailable = MyGroupItemWebsite.Item.IsNonInventory ? Convert.ToDecimal(MyGroupItemWebsite.Item.VendorInventory) : Convert.ToDecimal(MyGroupItemWebsite.Item.QuantityAvailable);
                        if (!MyGroupItemWebsite.Item.AllowBackOrder && newShoppingCartLine.Quantity > decAvailable && !CurrentWebsite.AllowBackOrderForAllItems)
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", MyGroupItemWebsite.Item.ItemName, MyGroupItemWebsite.Item.IsNonInventory ? MyGroupItemWebsite.Item.VendorInventory : MyGroupItemWebsite.Item.QuantityAvailable));
                        }

                        if (phEmployee.Visible)
                        {
                            if (string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                            {
                                throw new Exception("Employee must be selected");
                            }

                            newShoppingCartLine.UserInfoID = ddlUserInfo.SelectedValue;
                        }

                        newShoppingCartLine.Create(objConn, objTran);

                        SaveSelectableLogo(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);
                        SavePersonalization(newShoppingCartLine.ShoppingCartLineID, objConn, objTran);

                        txtLengthWidthAttributeQuantity.Text = String.Empty;
                    }
                }

                objTran.Commit();
                if(gotoshoppingcart)
                {
                    Response.Redirect("/ShoppingCart.aspx");
                }
                else
                {
                    InitializePage();
                    DisplayCustomization();
                    //Response.Redirect(String.Format("/ProductDetail.aspx?id={0}",_Item.ItemID));
                }
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                    WebUtility.DisplayJavascriptMessage(this, string.Format(@"{0}", ex.Message));
                }
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
        }

        protected void rptGroupByAttribute_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            mSelectedGroupByAttributeValueID = e.CommandArgument.ToString();
            hfSelectedGroupByAttributeValueID.Value = mSelectedGroupByAttributeValueID;
            BindItems();
            SetQuantityField();
            if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
            {
                hfSelectedLogo.Value = String.Empty;
            }
        }

        protected void rptGroupByAttribute_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton lbnAttributeValue = (LinkButton)e.Item.FindControl("lbnAttributeValue");
                HtmlControl liSelected = (HtmlControl)e.Item.FindControl("liSelected");
                string strBackgroundColor = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "BackgroundColor"));
                liSelected.Style.Add("background-color", "#" + strBackgroundColor);
                liSelected.Style.Add("border-color", "grey");

                lbnAttributeValue.ToolTip = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Value"));

                if (lbnAttributeValue.CommandArgument == mSelectedGroupByAttributeValueID)
                {
                    liSelected.Style.Add("border", "5px solid red");
                }
                else
                {
                    if (string.IsNullOrEmpty(strBackgroundColor))
                    {
                        string strAbbreviation = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Abbreviation"));
                        if (string.IsNullOrEmpty(strAbbreviation))
                        {
                            strAbbreviation = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "Value"));
                        }

                        lbnAttributeValue.Text = strAbbreviation.Substring(0, 1);
                    }
                    else
                    {
                        //lbnAttributeValue.Text = "&nbsp;";
                    }
                }
            }
        }

        //Item Customization
        //protected void btnGenerate_Click(object sender, EventArgs e)
        //{
        //    int left = 0;
        //    int top = 0;

        //    string strPath = Server.MapPath("\\DragAndDrop\\");
        //    //System.Drawing.Image imgLogo = System.Drawing.Image.FromFile(strPath + filLogo.FileName);

        //    System.Drawing.Image imgLogo = Resize();

        //    if (imgLogo != null)
        //    {
        //        Bitmap imgSource = null;

        //        if (ddlPosition.SelectedValue == "back")
        //            imgSource = new Bitmap(strPath + "ImageSolutions-Back.jpg");
        //        else
        //            imgSource = new Bitmap(strPath + "ImageSolutions-Front.jpg");
        //        Graphics graphicFinal = Graphics.FromImage(imgSource);

        //        int intLeftMargin = Convert.ToInt32(imgLogo.Width * Convert.ToDouble(ddlLeftMargin.SelectedValue)) - imgLogo.Width;
        //        int intTopMargin = Convert.ToInt32(imgLogo.Height * Convert.ToDouble(ddlTopMargin.SelectedValue)) - imgLogo.Height;

        //        switch (ddlPosition.SelectedValue)
        //        {
        //            case "left-chest":
        //                left = (imgSource.Width / 3) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            case "right-chest":
        //                left = (imgSource.Width / 3 * 2) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            case "left-sleeve":
        //                left = (imgSource.Width / 8) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            case "right-sleeve":
        //                left = (imgSource.Width / 8 * 7) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            case "left-waist":
        //                left = (imgSource.Width / 3) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = Convert.ToInt32((imgSource.Height / 5 * 4.5)) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            case "right-waist":
        //                left = (imgSource.Width / 3 * 2) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = Convert.ToInt32((imgSource.Height / 5 * 4.5)) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            case "center":
        //            case "back":
        //                left = (imgSource.Width / 2) - ((imgLogo.Width + intLeftMargin) / 2);
        //                top = (imgSource.Height / 5) - ((imgLogo.Height + intTopMargin) / 2);
        //                break;
        //            default:
        //                break;
        //        }

        //        graphicFinal.DrawImage(imgLogo, new Point(left, top));

        //        string strCustomDesignImageName = Guid.NewGuid().ToString() + ".jpg";
        //        string strCustomDesignImagePath = strPath + strCustomDesignImageName;

        //        imgSource.Save(strCustomDesignImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

        //        imgItem.ImageUrl = "/DragAndDrop/" + strCustomDesignImageName;
        //        imgItem2.ImageUrl = imgItem.ImageUrl;
        //    }
        //}

        //protected System.Drawing.Image Resize()
        //{
        //    System.Drawing.Image imgReturn = null;

        //    if ((filLogo.PostedFile != null) && (filLogo.PostedFile.ContentLength > 0))
        //    {
        //        Guid uid = Guid.NewGuid();
        //        string fn = System.IO.Path.GetFileName(filLogo.PostedFile.FileName);
        //        string SaveLocation = Server.MapPath("\\DragAndDrop\\Logo\\") + "" + uid + fn;
        //        try
        //        {
        //            string fileExtention = filLogo.PostedFile.ContentType;
        //            int fileLenght = filLogo.PostedFile.ContentLength;
        //            if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png")
        //            {
        //                if (fileLenght <= 3048576)
        //                {
        //                    System.Drawing.Bitmap bmpPostedImage = new System.Drawing.Bitmap(filLogo.PostedFile.InputStream);
        //                    imgReturn = ScaleImage(bmpPostedImage, Convert.ToInt32(100 * Convert.ToDouble(this.ddlRatio.SelectedValue)));
        //                    // Saving image in png format
        //                    imgReturn.Save(SaveLocation, ImageFormat.Png);
        //                    imgUploadedLogo.ImageUrl = "/DragAndDrop/logo/" + uid + fn;
        //                    //WebUtility.DisplayJavascriptMessage(this, "The file has been uploaded.");
        //                }
        //                else
        //                {
        //                    throw new Exception("Image file cannot be more than 3MB");
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("Invalid File Format");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(imgUploadedLogo.ImageUrl))
        //    {
        //        imgReturn = System.Drawing.Image.FromFile(Server.MapPath(imgUploadedLogo.ImageUrl));
        //        imgReturn = ScaleImage(imgReturn, Convert.ToInt32(100 * Convert.ToDouble(this.ddlRatio.SelectedValue)));
        //    }
        //    return imgReturn;
        //}

        //public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxHeight)
        //{
        //    var ratio = (double)maxHeight / image.Height;
        //    var newWidth = (int)(image.Width * ratio);
        //    var newHeight = (int)(image.Height * ratio);
        //    var newImage = new Bitmap(newWidth, newHeight);
        //    using (var g = Graphics.FromImage(newImage))
        //    {
        //        g.DrawImage(image, 0, 0, newWidth, newHeight);
        //    }
        //    return newImage;
        //}

        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int width)
        {
            var ratio = (double)width / image.Width;
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        //protected void ddlPosition_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    btnGenerate_Click(null, null);
        //}

        //protected void btnRatio_Click(object sender, EventArgs e)
        //{
        //    if (((Button)sender).CommandArgument == "Minus")
        //    {
        //        this.ddlRatio.SelectedIndex = this.ddlRatio.SelectedIndex - 1;
        //    }
        //    else if (((Button)sender).CommandArgument == "Plus")
        //    {
        //        this.ddlRatio.SelectedIndex = this.ddlRatio.SelectedIndex + 1;
        //    }
        //    ddlRatio_SelectedIndexChanged(null, null);
        //}

        //protected void ddlRatio_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    this.btnRatioMinus.Enabled = this.ddlRatio.SelectedIndex != 0;
        //    this.btnRatioPlus.Enabled = this.ddlRatio.SelectedIndex < this.ddlRatio.Items.Count - 1;
        //    btnGenerate_Click(null, null);
        //}

        //protected void btnLeftMargin_Click(object sender, EventArgs e)
        //{
        //    if (((Button)sender).CommandArgument == "Minus")
        //    {
        //        this.ddlLeftMargin.SelectedIndex = this.ddlLeftMargin.SelectedIndex - 1;
        //    }
        //    else if (((Button)sender).CommandArgument == "Plus")
        //    {
        //        this.ddlLeftMargin.SelectedIndex = this.ddlLeftMargin.SelectedIndex + 1;
        //    }
        //    ddlLeftMargin_SelectedIndexChanged(null, null);
        //}

        //protected void ddlLeftMargin_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    this.btnLeftMarginMinus.Enabled = this.ddlLeftMargin.SelectedIndex != 0;
        //    this.btnLeftMarginPlus.Enabled = this.ddlLeftMargin.SelectedIndex < this.ddlLeftMargin.Items.Count - 1;
        //    btnGenerate_Click(null, null);
        //}

        //protected void btnTopMargin_Click(object sender, EventArgs e)
        //{
        //    if (((Button)sender).CommandArgument == "Minus")
        //    {
        //        this.ddlTopMargin.SelectedIndex = this.ddlTopMargin.SelectedIndex - 1;
        //    }
        //    else if (((Button)sender).CommandArgument == "Plus")
        //    {
        //        this.ddlTopMargin.SelectedIndex = this.ddlTopMargin.SelectedIndex + 1;
        //    }
        //    ddlTopMargin_SelectedIndexChanged(null, null);
        //}

        //protected void ddlTopMargin_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    this.btnTopMarginMinus.Enabled = this.ddlTopMargin.SelectedIndex != 0;
        //    this.btnTopMarginPlus.Enabled = this.ddlTopMargin.SelectedIndex < this.ddlTopMargin.Items.Count - 1;
        //    btnGenerate_Click(null, null);
        //}

        //protected void ddlLogo_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (ddlLogo.SelectedValue == "Logo1")
        //    {
        //        imgUploadedLogo.ImageUrl = "/DragAndDrop/LogoUpload/OneLove.png";
        //    }
        //    else if (ddlLogo.SelectedValue == "Logo2")
        //    {
        //        imgUploadedLogo.ImageUrl = "/DragAndDrop/LogoUpload/CanesCrew.png";
        //    }
        //    else if (ddlLogo.SelectedValue == "Logo3")
        //    {
        //        imgUploadedLogo.ImageUrl = "/DragAndDrop/LogoUpload/Swirl.png";
        //    }
        //    else if (ddlLogo.SelectedValue == "Logo4")
        //    {
        //        imgUploadedLogo.ImageUrl = "/DragAndDrop/LogoUpload/IS-CircleLogo.png";
        //    }
        //    else if (ddlLogo.SelectedValue == "Logo5")
        //    {
        //        imgUploadedLogo.ImageUrl = "/DragAndDrop/LogoUpload/IS-Logo_CMYK.png";
        //    }
        //    btnGenerate_Click(null, null);
        //}

        //protected void ddlItemSelectableLogo_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(ddlItemSelectableLogo.SelectedValue))
        //    {
        //        ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(ddlItemSelectableLogo.SelectedValue);
        //        //ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
        //        //ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
        //        //ItemSelectableLogoFilter.ItemID.SearchString = _Item.ItemID;
        //        //ItemSelectableLogoFilter.SelectableLogoID = new Database.Filter.StringSearch.SearchFilter();
        //        //ItemSelectableLogoFilter.SelectableLogoID.SearchString = Convert.ToString(ddlItemSelectableLogo.SelectedValue);
        //        //ItemSelectableLogo = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogo(ItemSelectableLogoFilter);

        //        //if (!string.IsNullOrEmpty(ItemSelectableLogo.ImageURL))
        //        //{
        //        //    imgItem.ImageUrl = ItemSelectableLogo.ImageURL;
        //        //    imgItem2.ImageUrl = ItemSelectableLogo.ImageURL;
        //        //}
        //        //else
        //        //{
        //        //    //ImageSolutions.SelectableLogo.SelectableLogo SelectableLogo = new ImageSolutions.SelectableLogo.SelectableLogo(ddlItemSelectableLogo.SelectedValue);
        //        //    GenerateImage(ItemSelectableLogo);
        //        //}

        //        GenerateImage(ItemSelectableLogo);
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(_MyGroupItem.Item.DisplayImageURL))
        //        {
        //            imgItem.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
        //            imgItem2.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
        //        }
        //        else
        //        {
        //            imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
        //            imgItem2.ImageUrl = "../assets/images/pro3/2.jpg";
        //        }
        //    }
        //}

        protected void GenerateImage(ImageSolutions.Item.ItemSelectableLogo itemselectedlogo)
        {
            int left = 0;
            int top = 0;

            string strMonth = DateTime.UtcNow.ToString("yyyyMM");
            string strPath = Server.MapPath("\\DragAndDrop\\" + strMonth + "\\");
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }

            if(CheckUrlStatus(itemselectedlogo.SelectableLogo.ImagePath))
            {
                imgUploadedLogo.ImageUrl = itemselectedlogo.SelectableLogo.ImagePath;
                System.Drawing.Image imgLogo = GetImageFromURL(itemselectedlogo.SelectableLogo.ImagePath);

                if (itemselectedlogo.Width != null && itemselectedlogo.Width != 0)
                {
                    imgLogo = ScaleImage(imgLogo, Convert.ToInt32(itemselectedlogo.Width)); //* Convert.ToDouble(0.2)));
                }

                if (imgLogo != null)
                {

                    //string strBackImageURL = !string.IsNullOrEmpty(_Item.LogoImageURL) ? _Item.LogoImageURL : _Item.ImageURL;
                    string strBackImageURL = !string.IsNullOrEmpty(_MyGroupItem.Item.DisplayLogoImageURL) ? _MyGroupItem.Item.DisplayLogoImageURL : _MyGroupItem.Item.DisplayImageURL;

                    foreach (RepeaterItem _Item in rptItems.Items)
                    {
                        string strItemID = ((HiddenField)_Item.FindControl("hfItemID")).Value;
                        ImageSolutions.Item.Item objItem = new ImageSolutions.Item.Item(strItemID);

                        Panel pnlNoAttribute = (Panel)_Item.FindControl("pnlNoAttribute");
                        Panel pnlNoGroup = (Panel)_Item.FindControl("pnlNoGroup");
                        Panel pnlGroupSingleAttribute = (Panel)_Item.FindControl("pnlGroupSingleAttribute");
                        Panel pnlGroupMultipleAttribute = (Panel)_Item.FindControl("pnlGroupMultipleAttribute");

                        if (!pnlNoAttribute.Visible && !pnlNoGroup.Visible)
                        {
                            if (pnlGroupSingleAttribute.Visible)
                            {
                                Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");

                                foreach (RepeaterItem _RepeaterItem in rptGroupSingleAttributeValue.Items)
                                {
                                    List<string> AttributeValueIDs = new List<string>();

                                    HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                                    AttributeValueIDs.Add(hfAttributeValueID.Value);

                                    if (CurrentWebsite.ProductDetailDisplayType == "Grid")
                                    {
                                        Repeater rptGroupAttribute = (Repeater)_RepeaterItem.FindControl("rptGroupAttribute");
                                        foreach (RepeaterItem _ListRepeaterItem in rptGroupAttribute.Items)
                                        {
                                            TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                                            HiddenField hfListAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfListAttributeValueID");
                                            AttributeValueIDs.Add(hfListAttributeValueID.Value);

                                            ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                                            if (Item != null && (!string.IsNullOrEmpty(Item.LogoImageURL) || !string.IsNullOrEmpty(Item.ImageURL)))
                                            {
                                                strBackImageURL = !string.IsNullOrEmpty(Item.LogoImageURL) ? Item.LogoImageURL : Item.ImageURL;
                                            }

                                            AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                                        }
                                    }
                                    else if (CurrentWebsite.ProductDetailDisplayType == "List")
                                    {
                                        GridView gvGroupAttribute = (GridView)_RepeaterItem.FindControl("gvGroupAttribute");
                                        foreach (GridViewRow _ListRepeaterItem in gvGroupAttribute.Rows)
                                        {
                                            HiddenField hfListAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfListAttributeValueID");
                                            AttributeValueIDs.Add(hfListAttributeValueID.Value);

                                            ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                                            if (Item != null && (!string.IsNullOrEmpty(Item.LogoImageURL) || !string.IsNullOrEmpty(Item.ImageURL))) 
                                            {
                                                strBackImageURL = !string.IsNullOrEmpty(Item.LogoImageURL) ? Item.LogoImageURL : Item.ImageURL;
                                            }
                                            AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                                        }
                                    }
                                }
                            }
                        }                                                                                                                                                                     
                    }                    

                    if(!string.IsNullOrEmpty(strBackImageURL))
                    {
                        System.Drawing.Image imgSource = GetImageFromURL(strBackImageURL);

                        //imgSource = ScaleImage(imgSource, Convert.ToInt32(300));

                        if (imgSource.PixelFormat == PixelFormat.Format1bppIndexed ||
                            imgSource.PixelFormat == PixelFormat.Format4bppIndexed ||
                            imgSource.PixelFormat == PixelFormat.Format8bppIndexed)
                        {
                            Bitmap originalBmp = new Bitmap(imgSource);
                            imgSource = (System.Drawing.Image) originalBmp;
                        }                        

                        Graphics graphicFinal = Graphics.FromImage(imgSource);

                        int intXPercent = itemselectedlogo.PositionXPercent == null ? 0 : Convert.ToInt32(itemselectedlogo.PositionXPercent);
                        int intYPercent = itemselectedlogo.PositionYPercent == null ? 0 : Convert.ToInt32(itemselectedlogo.PositionYPercent);

                        //if (!string.IsNullOrEmpty(itemselectedlogo.SelectableLogo.LogoPosition))
                        //{
                        //    string[] arrPosition = itemselectedlogo.SelectableLogo.LogoPosition.Split(',');
                        //    if(arrPosition.Length == 2)
                        //    {
                        //        string strXPercent = arrPosition[0];
                        //        string strYPercent = arrPosition[1];

                        //        intXPercent = Convert.ToInt32(Convert.ToDecimal(strXPercent.Substring(0, strXPercent.IndexOf("%"))));
                        //        intYPercent = Convert.ToInt32(Convert.ToDecimal(strYPercent.Substring(0, strYPercent.IndexOf("%"))));
                        //    }
                        //}

                        left = (imgSource.Width * intXPercent / 100) - (imgLogo.Width / 2);
                        top = (imgSource.Height * intYPercent / 100) - (imgLogo.Height / 2);

                        graphicFinal.DrawImage(imgLogo, new Point(left, top));

                        string strCustomDesignImageName = Guid.NewGuid().ToString() + ".jpg";
                        string strCustomDesignImagePath = strPath + strCustomDesignImageName;

                        imgSource.Save(strCustomDesignImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                        imgItem.ImageUrl = "/DragAndDrop/" + strMonth + "/" + strCustomDesignImageName;
                        imgItem2.ImageUrl = imgItem.ImageUrl;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_MyGroupItem.Item.DisplayImageURL))
                {
                    imgItem.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                    imgItem2.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                }
                else
                {
                    imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
                    imgItem2.ImageUrl = "../assets/images/pro3/2.jpg";
                }
            }            
        }
        public System.Drawing.Image GetImageFromURL(string url)
        {
            WebClient WebClient = new WebClient();
            byte[] bytes = WebClient.DownloadData(url);
            MemoryStream ms = new MemoryStream(bytes);
            return System.Drawing.Image.FromStream(ms);
        }
        protected bool CheckUrlStatus(string Website)
        {
            try
            {
                var request = WebRequest.Create(Website) as HttpWebRequest;
                request.Method = "GET";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }
        protected void SaveSelectableLogo(string shoppingcartlineid, SqlConnection conn, SqlTransaction trans)
        {
            if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
            {
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(Convert.ToString(hfSelectedLogo.Value));

                ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo ShoppingCartLineSelectableLogo = new ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo();
                ShoppingCartLineSelectableLogo.ShoppingCartLineID = shoppingcartlineid;
                ShoppingCartLineSelectableLogo.SelectableLogoID = Convert.ToString(ItemSelectableLogo.SelectableLogoID);
                ShoppingCartLineSelectableLogo.BasePrice = ItemSelectableLogo.SelectableLogo.BasePrice;
                if(pnlSelectableLogoYear.Visible && !string.IsNullOrEmpty(ddlSelectableLogoYear.SelectedValue))
                {
                    ShoppingCartLineSelectableLogo.SelectYear = Convert.ToString(ddlSelectableLogoYear.SelectedValue);
                }
                ShoppingCartLineSelectableLogo.Create(conn, trans);
            }
            //SP
            else if (chkNoLogo.Checked)
            {
                ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo ShoppingCartLineSelectableLogo = new ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo();
                ShoppingCartLineSelectableLogo.ShoppingCartLineID = shoppingcartlineid;
                ShoppingCartLineSelectableLogo.HasNoLogo = true;
                ShoppingCartLineSelectableLogo.Create(conn, trans);
            }
            else
            {
                if (phLogo.Visible && !chkNoLogo.Checked)
                {
                    throw new Exception("No Logo selected");
                }
            }
        }

        protected void rptItemPersonalization_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ImageSolutions.Item.ItemPersonalization ItemPersonalization = (ImageSolutions.Item.ItemPersonalization)e.Item.DataItem;

                    Label lblLabel = (Label)e.Item.FindControl("lblLabel");
                    TextBox txtValue = (TextBox)e.Item.FindControl("txtValue");
                    DropDownList ddlValueList = (DropDownList)e.Item.FindControl("ddlValueList");
                    CheckBox chkBlank = (CheckBox)e.Item.FindControl("chkBlank");
                    Label lblVerifyLabel = (Label)e.Item.FindControl("lblVerifyLabel");
                    TextBox txtVerifyValue = (TextBox)e.Item.FindControl("txtVerifyValue");

                    DropDownList ddlTextOption = (DropDownList)e.Item.FindControl("ddlTextOption");
                    TextBox txtTextOption = (TextBox)e.Item.FindControl("txtTextOption");

                    lblLabel.Text = string.IsNullOrEmpty(ItemPersonalization.Label) ? ItemPersonalization.Name : ItemPersonalization.Label;

                    if (ItemPersonalization.Type == "dropdown")
                    {
                        txtValue.Visible = false;
                        ddlValueList.Visible = true;
                        chkBlank.Visible = false;
                        lblVerifyLabel.Visible = false;
                        txtVerifyValue.Visible = false;
                        ddlTextOption.Visible = false;
                        txtTextOption.Visible = false;

                        List<ImageSolutions.Item.ItemPersonalizationValueList> ItemPersonalizationValueLists = new List<ImageSolutions.Item.ItemPersonalizationValueList>();
                        ImageSolutions.Item.ItemPersonalizationValueListFilter ItemPersonalizationValueListFilter = new ImageSolutions.Item.ItemPersonalizationValueListFilter();
                        ItemPersonalizationValueListFilter.ItemPersonalizationID = new Database.Filter.StringSearch.SearchFilter();
                        ItemPersonalizationValueListFilter.ItemPersonalizationID.SearchString = ItemPersonalization.ItemPersonalizationID;
                        ItemPersonalizationValueLists = ImageSolutions.Item.ItemPersonalizationValueList.GetItemPersonalizationValueLists(ItemPersonalizationValueListFilter);

                        ddlValueList.DataSource = ItemPersonalizationValueLists; //.OrderBy(x => x.Value);
                        ddlValueList.DataBind();
                        ddlValueList.Items.Insert(0, new ListItem(String.Empty, string.Empty));

                    }
                    else if (ItemPersonalization.Type == "textoption")
                    {
                        txtValue.Visible = false;
                        ddlValueList.Visible = false;
                        chkBlank.Visible = false;
                        lblVerifyLabel.Visible = false;
                        txtVerifyValue.Visible = false;
                        ddlTextOption.Visible = true;
                        txtTextOption.Visible = true;
                        txtTextOption.Enabled = false;

                        if (ItemPersonalization.DefaultValue == "First Name")
                        {
                            if (phEmployee.Visible && !string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                            {
                                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo(ddlUserInfo.SelectedValue);

                                ImageSolutions.User.UserWebsite UserWebsite = new ImageSolutions.User.UserWebsite();
                                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                                UserWebsiteFilter.UserInfoID = new Database.Filter.StringSearch.SearchFilter();
                                UserWebsiteFilter.UserInfoID.SearchString = UserInfo.UserInfoID; 
                                UserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsite(UserWebsiteFilter);

                                if(UserWebsite.IsStore)
                                {
                                    ddlTextOption.Items.Insert(0, new ListItem(string.IsNullOrEmpty(ItemPersonalization.FreeTextValueLabel) ? "Free Text" : ItemPersonalization.FreeTextValueLabel, "FreeText"));
                                    ddlTextOption.Items.Insert(1, new ListItem("No Embroidery", "NoEmbroidery"));

                                    txtTextOption.Enabled = true;
                                }
                                else
                                {
                                    ddlTextOption.Items.Insert(0, new ListItem(string.IsNullOrEmpty(ItemPersonalization.DefaultValueLabel) ? "Default" : ItemPersonalization.DefaultValueLabel, "Default"));
                                    ddlTextOption.Items.Insert(1, new ListItem(string.IsNullOrEmpty(ItemPersonalization.FreeTextValueLabel) ? "Free Text" : ItemPersonalization.FreeTextValueLabel, "FreeText"));
                                    ddlTextOption.Items.Insert(2, new ListItem("No Embroidery", "NoEmbroidery"));

                                    txtTextOption.Text = UserInfo.FirstName;
                                }

                            }
                            else
                            {
                              
                                if(CurrentUser.CurrentUserWebSite.IsStore)
                                {
                                    ddlTextOption.Items.Insert(0, new ListItem(string.IsNullOrEmpty(ItemPersonalization.FreeTextValueLabel) ? "Free Text" : ItemPersonalization.FreeTextValueLabel, "FreeText"));
                                    ddlTextOption.Items.Insert(1, new ListItem("No Embroidery", "NoEmbroidery"));

                                    txtTextOption.Enabled = true;
                                }
                                else
                                {
                                    ddlTextOption.Items.Insert(0, new ListItem(string.IsNullOrEmpty(ItemPersonalization.DefaultValueLabel) ? "Default" : ItemPersonalization.DefaultValueLabel, "Default"));
                                    ddlTextOption.Items.Insert(1, new ListItem(string.IsNullOrEmpty(ItemPersonalization.FreeTextValueLabel) ? "Free Text" : ItemPersonalization.FreeTextValueLabel, "FreeText"));
                                    ddlTextOption.Items.Insert(2, new ListItem("No Embroidery", "NoEmbroidery"));

                                    txtTextOption.Text = CurrentUser.FirstName;
                                }
                            }
                        }
                    }
                    else //"text"
                    {
                        txtValue.Visible = true;
                        ddlValueList.Visible = false;
                        chkBlank.Visible = true;
                        lblVerifyLabel.Visible = false;
                        txtVerifyValue.Visible = false;
                        ddlTextOption.Visible = false;
                        txtTextOption.Visible = false;

                        if(ItemPersonalization.RequireVerification)
                        {
                            lblVerifyLabel.Visible = true;
                            lblVerifyLabel.Text = String.Format("{0} (Verify):", lblLabel.Text);
                            txtVerifyValue.Visible = true;
                        }

                        if(ItemPersonalization.DefaultValue == "First Name")
                        {
                            if(phEmployee.Visible && !string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                            {
                                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo(ddlUserInfo.SelectedValue);
                                txtValue.Text = UserInfo.FirstName;
                                if (txtVerifyValue.Visible)
                                {
                                    txtVerifyValue.Text = UserInfo.FirstName;
                                }
                            }
                            else
                            {
                                txtValue.Text = CurrentUser.FirstName;
                                if (txtVerifyValue.Visible)
                                {
                                    txtVerifyValue.Text = CurrentUser.FirstName;
                                }
                            }
                        }

                        chkBlank.Visible = ItemPersonalization.AllowBlank;
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void SavePersonalization(string shoppingcartlineid, SqlConnection conn, SqlTransaction trans)
        {
            foreach (RepeaterItem _Item in this.rptItemPersonalization.Items)
            {
                string strItemPersonalizationID = ((HiddenField)_Item.FindControl("hfItemPersonalizationID")).Value;

                ImageSolutions.Item.ItemPersonalizationValue ItemPersonalizationValue = new ImageSolutions.Item.ItemPersonalizationValue();
                ItemPersonalizationValue.ItemPersonalizationID = strItemPersonalizationID;
                ItemPersonalizationValue.ShoppingCartLineID = shoppingcartlineid;

                ImageSolutions.Item.ItemPersonalization ItemPersonalization = new ImageSolutions.Item.ItemPersonalization(strItemPersonalizationID);

                if (ItemPersonalization.Type == "dropdown")
                {
                    DropDownList ddlValueList = (DropDownList)_Item.FindControl("ddlValueList");
                    ItemPersonalizationValue.Value = ddlValueList.SelectedValue;

                    if (!ItemPersonalization.AllowBlank && string.IsNullOrEmpty(ItemPersonalizationValue.Value))
                    {
                        throw new Exception(String.Format("Missing: {0}", string.IsNullOrEmpty(ItemPersonalizationValue.ItemPersonalization.Label) ? ItemPersonalizationValue.ItemPersonalization.Name : ItemPersonalizationValue.ItemPersonalization.Label));
                    }
                }
                else if (ItemPersonalization.Type == "textoption")
                {
                    DropDownList ddlTextOption = (DropDownList)_Item.FindControl("ddlTextOption");
                    TextBox txtTextOption = (TextBox)_Item.FindControl("txtTextOption");

                    if (ddlTextOption.SelectedValue == "Default" || ddlTextOption.SelectedValue == "FreeText")
                    {
                        if (string.IsNullOrEmpty(txtTextOption.Text))
                        {
                            throw new Exception(String.Format("Missing Text for {0}", string.IsNullOrEmpty(ItemPersonalization.Label) ? ItemPersonalization.Name : ItemPersonalization.Label));
                        }

                        ItemPersonalizationValue.Value = txtTextOption.Text;

                        if (ddlTextOption.SelectedValue == "Default")
                        {
                            ItemPersonalizationValue.TextOption = string.IsNullOrEmpty(ItemPersonalization.DefaultValueLabel) ? "Default" : ItemPersonalization.DefaultValueLabel;
                        }
                        else if (ddlTextOption.SelectedValue == "FreeText")
                        {
                            ItemPersonalizationValue.TextOption = string.IsNullOrEmpty(ItemPersonalization.FreeTextValueLabel) ? "Free" : ItemPersonalization.FreeTextValueLabel;
                        }
                    }
                    else if (ddlTextOption.SelectedValue == "NoEmbroidery")
                    {
                        ItemPersonalizationValue.Value = String.Empty;
                        ItemPersonalizationValue.TextOption = "No Embroidery";
                    }
                }
                else
                {
                    TextBox txtValue = (TextBox)_Item.FindControl("txtValue");
                    ItemPersonalizationValue.Value = txtValue.Text;

                    if(ItemPersonalization.RequireVerification)
                    {
                        TextBox txtVerifyValue = (TextBox)_Item.FindControl("txtVerifyValue");
                        if (txtVerifyValue.Text != txtValue.Text)
                        {
                            throw new Exception(String.Format("{0}: Verify does not match", ItemPersonalization.Name));
                        }
                    }
                }

                //if (!string.IsNullOrEmpty(ItemPersonalizationValue.Value) && !string.IsNullOrEmpty(ItemPersonalizationValue.Value.Trim()))
                //SP
                if (
                    (!string.IsNullOrEmpty(ItemPersonalizationValue.Value) && !string.IsNullOrEmpty(ItemPersonalizationValue.Value.Trim()))
                    ||
                    (!string.IsNullOrEmpty(ItemPersonalizationValue.TextOption))
                )
                {
                    ItemPersonalizationValue.Create(conn, trans);
                }
                else if (ItemPersonalization.Type == "text" && !ItemPersonalization.AllowBlank)
                {
                    throw new Exception(String.Format("Missing: {0}", ItemPersonalizationValue.ItemPersonalization.Name));
                }

            }
        }

        protected void chkNoLogo_CheckedChanged(object sender, EventArgs e)
        {
            if(chkNoLogo.Checked)
            {
                hfSelectedLogo.Value = String.Empty;
                pnlSelectableLogoYear.Visible = false;
                ddlSelectableLogoYear.Items.Clear();

                pnlSelectableBasePrice.Visible = false;
                txtSelectableBasePrice.Text = String.Empty;
                //ddlItemSelectableLogo.SelectedValue = string.Empty;

                if (!string.IsNullOrEmpty(_MyGroupItem.Item.DisplayImageURL))
                {
                    imgItem.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                    imgItem2.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                }
                else
                {
                    imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
                    imgItem2.ImageUrl = "../assets/images/pro3/2.jpg";
                }

                //ddlItemSelectableLogo.Enabled = false;
                pnlSelectableLogoImage.Visible = false;
            }
            else
            {
                //ddlItemSelectableLogo.Enabled = true;
                pnlSelectableLogoImage.Visible = true;
            }
        }

        protected void btnAddMore_Click(object sender, EventArgs e)
        {
            AddToCart(false);
        }

        protected void rptSelectableLogo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LinkButton lbnSelectableLogo = (LinkButton)e.Item.FindControl("lbnSelectableLogo");
                HtmlControl liSelected = (HtmlControl)e.Item.FindControl("liSelected");
                System.Web.UI.WebControls.Image imgLogo = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgLogo");
                string strItemSelectableLogoID = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "ItemSelectableLogoID"));
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(strItemSelectableLogoID);
                imgLogo.ImageUrl = ItemSelectableLogo.SelectableLogo.ImagePath;
                imgLogo.ToolTip = ItemSelectableLogo.SelectableLogo.Name;
                //liSelected.Style.Add("border", "5px solid blue");
                imgLogo.Style.Add("width", "100px");
                imgLogo.Style.Add("height", "50px");
                imgLogo.Style.Add("margin", "8px");

                if (lbnSelectableLogo.CommandArgument == hfSelectedLogo.Value)
                {
                    liSelected.Style.Add("border", "5px solid red");
                }
            }          
        }

        protected void rptSelectableLogo_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            hfSelectedLogo.Value = e.CommandArgument.ToString();
            BindItemSelectableLogoImage();

            pnlSelectableLogoYear.Visible = false;
            ddlSelectableLogoYear.Items.Clear();

            pnlSelectableBasePrice.Visible = false;
            txtSelectableBasePrice.Text = String.Empty;

            if (!string.IsNullOrEmpty(hfSelectedLogo.Value))
            {
                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(hfSelectedLogo.Value);
                GenerateImage(ItemSelectableLogo);

                if (!string.IsNullOrEmpty(ItemSelectableLogo.SelectableLogo.SelectYears))
                {
                    List<string> DropDownYears = new List<string>();
                    string[] years = ItemSelectableLogo.SelectableLogo.SelectYears.Split('-');

                    if (years.Length == 2)
                    {
                        string strMinYear = years[0];
                        string strMaxYear = years[1];
                        int intMinYear;
                        int intMaxYear;

                        if (int.TryParse(strMinYear, out intMinYear) && int.TryParse(strMaxYear, out intMaxYear))
                        {
                            while (intMinYear <= intMaxYear)
                            {
                                DropDownYears.Add(Convert.ToString(intMinYear));
                                intMinYear++;
                            }
                        }
                    }

                    if(DropDownYears != null && DropDownYears.Count > 0)
                    {
                        pnlSelectableLogoYear.Visible = true;
                        lblSelectableLogoYear.Text = string.IsNullOrEmpty(ItemSelectableLogo.SelectableLogo.SelectYearsLabel) ? "Year" : ItemSelectableLogo.SelectableLogo.SelectYearsLabel;

                        ddlSelectableLogoYear.DataSource = DropDownYears;
                        ddlSelectableLogoYear.DataBind();
                    }
                }

                if(ItemSelectableLogo.SelectableLogo.BasePrice != null && ItemSelectableLogo.SelectableLogo.BasePrice > 0)
                {
                    pnlSelectableBasePrice.Visible = true;
                    txtSelectableBasePrice.Text = string.Format("{0:C}", ItemSelectableLogo.SelectableLogo.BasePrice);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_MyGroupItem.Item.DisplayImageURL))
                {
                    imgItem.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                    imgItem2.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
                }
                else
                {
                    imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
                    imgItem2.ImageUrl = "../assets/images/pro3/2.jpg";
                }
            }

        }

        protected void btnSizeChart_Click(object sender, EventArgs e)
        {
            if(_Item.SizeChartURL.Contains("=.pdf"))
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + _Item.SizeChartURL + "');", true);
            }
            else
            {
                ucImageModal.DialogImageURL = string.IsNullOrEmpty(_Item.SizeChartURL) ? CurrentWebsite.DefaultSizeChartPath : _Item.SizeChartURL;
                ucImageModal.Show();
            }
        }

        protected void ddlLengthWidthAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            mSelectedGroupByAttributeValueID = hfSelectedGroupByAttributeValueID.Value;
            
            SetQuantityField();
        }

        protected void chkBlank_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBlank = (CheckBox)sender;
            RepeaterItem item = (RepeaterItem)chkBlank.NamingContainer;
            TextBox txtValue = (TextBox)item.FindControl("txtValue");
            TextBox txtVerifyValue = (TextBox)item.FindControl("txtVerifyValue");
            //HiddenField hfItemPersonalizationID = (HiddenField)item.FindControl("hfItemPersonalizationID");
            if (chkBlank.Checked)
            {
                txtValue.Text = String.Empty;
                txtValue.Enabled = false;
                txtVerifyValue.Text = String.Empty;
                txtVerifyValue.Enabled = false;
                //hfItemPersonalizationID.Value = String.Empty;
            }
            else
            {
                txtValue.Enabled = true;
                txtVerifyValue.Enabled = true;
            }

            SetPersonalizationBasePrice();
            //if (chkNoLogo.Checked)
            //{
            //    hfSelectedLogo.Value = String.Empty;
            //    //ddlItemSelectableLogo.SelectedValue = string.Empty;

            //    if (!string.IsNullOrEmpty(_MyGroupItem.Item.DisplayImageURL))
            //    {
            //        imgItem.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
            //        imgItem2.ImageUrl = _MyGroupItem.Item.DisplayImageURL;
            //    }
            //    else
            //    {
            //        imgItem.ImageUrl = "../assets/images/pro3/2.jpg";
            //        imgItem2.ImageUrl = "../assets/images/pro3/2.jpg";
            //    }

            //    //ddlItemSelectableLogo.Enabled = false;
            //    pnlSelectableLogoImage.Visible = false;
            //}
            //else
            //{
            //    //ddlItemSelectableLogo.Enabled = true;
            //    pnlSelectableLogoImage.Visible = true;
            //}
        }

        protected void ddlTextOption_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlTextOption = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddlTextOption.NamingContainer;
            TextBox txtTextOption = (TextBox)item.FindControl("txtTextOption");
            HiddenField hfItemPersonalizationID = (HiddenField)item.FindControl("hfItemPersonalizationID");

            if (ddlTextOption.SelectedValue == "Default")
            {
                txtTextOption.Visible = true;
                txtTextOption.Enabled = false;

                ImageSolutions.Item.ItemPersonalization ItemPersonalization = new ImageSolutions.Item.ItemPersonalization(hfItemPersonalizationID.Value);

                if (ItemPersonalization.DefaultValue == "First Name")
                {
                    if (phEmployee.Visible && !string.IsNullOrEmpty(ddlUserInfo.SelectedValue))
                    {
                        ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo(ddlUserInfo.SelectedValue);

                        txtTextOption.Text = UserInfo.FirstName;
                    }
                    else
                    {
                        txtTextOption.Text = CurrentUser.FirstName;
                    }
                }
            }
            else if (ddlTextOption.SelectedValue == "FreeText")
            {
                txtTextOption.Visible = true;
                txtTextOption.Enabled = true;
                txtTextOption.Text = String.Empty;
            }
            else
            {
                txtTextOption.Visible = false;
                txtTextOption.Enabled = false;
                txtTextOption.Text = String.Empty;
            }

            SetPersonalizationBasePrice();
        }

        protected void ddlUserInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayCustomization();
        }

        protected void ddlValueList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPersonalizationBasePrice();
        }

        public void SetPersonalizationBasePrice()
        {
            bool blnHasPersonalization = false;

            foreach (RepeaterItem _Item in this.rptItemPersonalization.Items)
            {
                string strItemPersonalizationID = ((HiddenField)_Item.FindControl("hfItemPersonalizationID")).Value;

                ImageSolutions.Item.ItemPersonalization ItemPersonalization = new ImageSolutions.Item.ItemPersonalization(strItemPersonalizationID);

                if (ItemPersonalization.Type == "dropdown")
                {
                    DropDownList ddlValueList = (DropDownList)_Item.FindControl("ddlValueList");
                    if (!string.IsNullOrEmpty(ddlValueList.SelectedValue))
                    {
                        blnHasPersonalization = true;
                    }
                }
                else if (ItemPersonalization.Type == "textoption")
                {
                    DropDownList ddlTextOption = (DropDownList)_Item.FindControl("ddlTextOption");
                    TextBox txtTextOption = (TextBox)_Item.FindControl("txtTextOption");

                    if (ddlTextOption.SelectedValue == "Default" || ddlTextOption.SelectedValue == "FreeText")
                    {
                        blnHasPersonalization = true;
                    }
                }
                else
                {
                    TextBox txtValue = (TextBox)_Item.FindControl("txtValue");
                    CheckBox chkBlank = (CheckBox)_Item.FindControl("chkBlank");

                    if (!chkBlank.Checked)
                    {
                        blnHasPersonalization = true;
                    }
                }
            }


            pnlPersonalizationBasePrice.Visible = false;
            txtPersonalizationBasePrice.Text = String.Empty;

            if (blnHasPersonalization)
            {
                ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(mItemID);

                List<ImageSolutions.Item.ItemSelectableLogo> ItemSelectableLogos = Item.ItemSelectableLogos.FindAll(x => !x.SelectableLogo.Inactive && x.SelectableLogo.IsPersonalization);

                if (ItemSelectableLogos != null && ItemSelectableLogos.Count > 0)
                {
                    double dblPersonalizationBasePrice = Math.Round(Convert.ToDouble(ItemSelectableLogos.Sum(y => y.SelectableLogo.BasePrice == null ? 0 : y.SelectableLogo.BasePrice)), 2);
                    pnlPersonalizationBasePrice.Visible = true;
                    txtPersonalizationBasePrice.Text = string.Format("{0:C}", dblPersonalizationBasePrice);
                }
            }
        }

        protected void ddlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindUserInfo();
        }

        protected void btnAccountSearch_Click(object sender, EventArgs e)
        {
            ucAccountSearchModal.WebsiteID = CurrentWebsite.WebsiteID;
            ucAccountSearchModal.Show();
        }

        protected void btnAccountRemove_Click(object sender, EventArgs e)
        {
            ddlAccount.SelectedValue = String.Empty;
            txtAccount.Text = String.Empty;
            hfAccountID.Value = String.Empty;

            //btnAccountRemove.Visible = !string.IsNullOrEmpty(hfAccountID.Value);
        }

        protected void rptRelatedItem_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "")
            {
            }
        }

        protected void rptRelatedItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    //ImageSolutions.Item.MyGroupItem MyGroupItem = (ImageSolutions.Item.MyGroupItem)e.Item.DataItem;

                    //ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(MyGroupItem.ItemID);

                    //ImageSolutions.Attribute.Attribute Attribute = Item.Attributes.Find(x => x.AttributeName == "Color");

                    //if (Attribute != null)
                    //{
                    //    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    //    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    //    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    //    AttributeValueFilter.AttributeID.SearchString = Attribute.AttributeID;
                    //    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                    //    Repeater rptColors = (Repeater)e.Item.FindControl("rptColors");
                    //    List<ImageSolutions.Attribute.AttributeValue> objColors = AttributeValues;

                    //    rptColors.DataSource = objColors;
                    //    rptColors.DataBind();
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void BindRelatedItem()
        {
            List<ImageSolutions.Website.WebsiteTabItem> objWebsiteTabItems = null;
            ImageSolutions.Website.WebsiteTabItemFilter objFilter = null;
            int intTotalRecord = 0;
            List<string> objColors = new List<string>();
            List<string> objSizes = new List<string>();

            try
            {
                objFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteTabID.SearchString = mWebSiteTabID;

                objFilter.IsOnline = true;
                objFilter.Inactive = false;

                objWebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(objFilter, "Sort", true, ucPager.PageSize, ucPager.CurrentPageNumber, out intTotalRecord); ;

                List<ImageSolutions.Item.MyGroupItem> MyGroupItems = new List<ImageSolutions.Item.MyGroupItem>();



                foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in objWebsiteTabItems)
                {
                    MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, _WebsiteTabItem.ItemID));
                }

                pnlCategoryBreadCrumb.Visible = true;
                ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(mWebSiteTabID);
                litCategoryBreadCrumb.Text = WebsiteTab.TabPathBreadCrumb;

                List<ImageSolutions.Item.MyGroupItem> RemoveMyGroupItems = new List<ImageSolutions.Item.MyGroupItem>();

                foreach (ImageSolutions.Item.MyGroupItem _Item in MyGroupItems)
                {
                    bool blnValid = true;

                    if (_Item.Item.WebsiteGroupItems != null
                        && _Item.Item.WebsiteGroupItems.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID) != null
                        && _Item.Item.WebsiteGroupItems.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID).Count > 0)
                    {
                        blnValid = false;

                        foreach (ImageSolutions.Website.WebsiteGroupItem _WebsiteGroupItem in _Item.Item.WebsiteGroupItems)
                        {
                            if (CurrentWebsite.CombineWebsiteGroup)
                            {
                                if (CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == _WebsiteGroupItem.WebsiteGroupID))
                                {
                                    blnValid = true;
                                }
                            }
                            else
                            {
                                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == _WebsiteGroupItem.WebsiteGroupID)
                                {
                                    blnValid = true;
                                }
                            }
                        }

                        if (!blnValid)
                        {
                            RemoveMyGroupItems.Add(_Item);
                        }
                    }

                    if (_Item.Item.WebsiteGroupItemExcludes != null
                        && _Item.Item.WebsiteGroupItemExcludes.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID) != null
                        && _Item.Item.WebsiteGroupItemExcludes.FindAll(x => x.WebsiteID == CurrentWebsite.WebsiteID).Count > 0)
                    {
                        blnValid = true;

                        foreach (ImageSolutions.Website.WebsiteGroupItemExclude _WebsiteGroupItemExclude in _Item.Item.WebsiteGroupItemExcludes)
                        {
                            if (CurrentWebsite.CombineWebsiteGroup)
                            {
                                if (CurrentUser.CurrentUserWebSite.UserAccounts.Exists(x => x.WebsiteGroupID == _WebsiteGroupItemExclude.WebsiteGroupID))
                                {
                                    blnValid = false;
                                }
                            }
                            else
                            {
                                if (CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID == _WebsiteGroupItemExclude.WebsiteGroupID)
                                {
                                    blnValid = false;
                                }
                            }
                        }

                        if (!blnValid)
                        {
                            RemoveMyGroupItems.Add(_Item);
                        }
                    }

                    if (blnValid)
                    {
                        if (_Item.Item.Attributes != null)
                        {
                            foreach (ImageSolutions.Attribute.Attribute objAttribute in _Item.Item.Attributes)
                            {
                                if (objAttribute.AttributeValues != null)
                                {
                                    foreach (ImageSolutions.Attribute.AttributeValue objAttributeValue in objAttribute.AttributeValues)
                                    {
                                        if (objAttribute.AttributeName.ToLower() == "color")
                                        {
                                            if (!objColors.Exists(m => m.ToLower().Equals(objAttributeValue.Value.ToLower())))
                                            {
                                                objColors.Add(objAttributeValue.Value);
                                            }
                                        }

                                        if (objAttribute.AttributeName.ToLower() == "size")
                                        {
                                            if (!objSizes.Exists(m => m.ToLower().Equals(objAttributeValue.Value.ToLower())))
                                            {
                                                objSizes.Add(objAttributeValue.Value);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (RemoveMyGroupItems != null && RemoveMyGroupItems.Count > 0)
                {
                    foreach (ImageSolutions.Item.MyGroupItem _Item in RemoveMyGroupItems)
                    {
                        MyGroupItems.Remove(_Item);
                    }
                }

                if (MyGroupItems.Exists(x => x.ItemID == mItemID))
                {
                    MyGroupItems.Remove(MyGroupItems.Find(x => x.ItemID == mItemID));
                }

                if (MyGroupItems != null && MyGroupItems.Count > 0)
                {
                    pnlRelatedItems.Visible = true;
                    this.rptRelatedItem.DataSource = MyGroupItems;
                    this.rptRelatedItem.DataBind();

                    ucPager.TotalRecord = intTotalRecord;
                }
                else
                {
                    pnlRelatedItems.Visible = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebsiteTabItems = null;
                objFilter = null;
            }
        }

    }
}