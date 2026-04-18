using ImageSolutions.Item;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace ImageSolutionsWebsite
{
    public partial class ItemList : BasePageUserAuth
    {
        protected string mWebSiteTabID = string.Empty;
        protected string mItemID = string.Empty;
        protected string mSearch = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");
            mItemID = Request.QueryString.Get("ItemID");
            mSearch = Request.QueryString.Get("Search");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }
        protected void Initialize()
        {
            if (!string.IsNullOrEmpty(mWebSiteTabID) || !string.IsNullOrEmpty(mItemID) || !string.IsNullOrEmpty(mSearch)) BindItems();

            SetQuantityField();

            if (CurrentWebsite.EnablePackagePayment)
            {
                phAddToCart.Visible = false;
                phPackageTop.Visible = true;
                phPackageBottom.Visible = true;

                if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines != null && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count > 0)
                {
                    foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                    {
                        _ShoppingCartLine.Delete();
                    }
                }

                BindPackage();

                divList.Visible = false;
                btnAddToCartPackage.Visible = false;

                litSelectPackageHeader.Visible = true;
                litSelectPackageHeader.Text = String.Format(@"<p style=""color: blue; font-size: 24px; font-family: 'Lato';"">Select one of the packages below.</p>");

                if (CurrentUser.CurrentUserWebSite.PackageAvailableDate != null && CurrentUser.CurrentUserWebSite.PackageAvailableDate > DateTime.UtcNow)
                {
                    litSelectPackageHeader.Text = String.Format(@"<p style=""color: red; font-size: 24px; font-family: 'Lato'; text-decoration: underline;"">The next available date to place your annual order is  <b>{0}</b></p>", Convert.ToDateTime(CurrentUser.CurrentUserWebSite.PackageAvailableDate).ToString("MM/dd/yyyy") );
                    rblPackage.Visible = false;
                }
            }
        }

        protected void BindPackage()
        {
            List<ImageSolutions.Package.Package> Packages = new List<ImageSolutions.Package.Package>();
            ImageSolutions.Package.PackageFilter PackageFilter = new ImageSolutions.Package.PackageFilter();
            PackageFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
            PackageFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
            PackageFilter.InActive = false;
            Packages = ImageSolutions.Package.Package.GetPackages(PackageFilter);

            List<ImageSolutions.Package.Package> _Packages = new List<ImageSolutions.Package.Package>();

            //ddlPackage.Items.Clear();
            //ddlPackage.Items.Add(new ListItem(string.Empty, string.Empty));
            foreach (ImageSolutions.Package.Package _Package in Packages)
            {
                ImageSolutions.Website.WebsiteGroupPackage WebsiteGroupPackage = new ImageSolutions.Website.WebsiteGroupPackage();

                foreach (ImageSolutions.User.UserAccount _UserAccount in CurrentUser.CurrentUserWebSite.UserAccounts)
                {
                    ImageSolutions.Website.WebsiteGroupPackageFilter WebsiteGroupPackageFilter = new ImageSolutions.Website.WebsiteGroupPackageFilter();
                    WebsiteGroupPackageFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupPackageFilter.WebsiteID.SearchString = CurrentWebsite.WebsiteID;
                    WebsiteGroupPackageFilter.PackageID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupPackageFilter.PackageID.SearchString = _Package.PackageID;
                    WebsiteGroupPackageFilter.WebsiteGroupID = new Database.Filter.StringSearch.SearchFilter();
                    WebsiteGroupPackageFilter.WebsiteGroupID.SearchString = _UserAccount.WebsiteGroupID;
                    WebsiteGroupPackage = ImageSolutions.Website.WebsiteGroupPackage.GetWebsiteGroupPackage(WebsiteGroupPackageFilter);

                    if (WebsiteGroupPackage != null) break;
                }

                if (WebsiteGroupPackage != null)
                {
                    _Packages.Add(_Package);
                    //ddlPackage.Items.Add(new ListItem(_Package.Name, _Package.PackageID));
                }
            }

            rblPackage.DataSource = _Packages;
            rblPackage.DataTextField = "DescriptionHTML";
            rblPackage.DataValueField = "PackageID";
            rblPackage.DataBind();
        }

        protected void BindItems()
        {
            List<ImageSolutions.Website.WebsiteTabItem> objWebsiteTabItems = null;
            ImageSolutions.Website.WebsiteTabItemFilter objFilter = null;
            ImageSolutions.Item.Item objItem = null;

            try
            {
                List<ImageSolutions.Item.MyGroupItem> MyGroupItems = new List<ImageSolutions.Item.MyGroupItem>();

                if (!string.IsNullOrEmpty(mSearch))
                {
                    //foreach (ImageSolutions.Item.ItemWebsite _ItemWebsite in CurrentWebsite.ItemWebsites)
                    //{
                    //    if(!_ItemWebsite.Item.InActive 
                    //        && _ItemWebsite.Item.IsOnline 
                    //        && string.IsNullOrEmpty(_ItemWebsite.Item.ParentID)
                    //        && (_ItemWebsite.Item.ItemName.Contains(mSearch) 
                    //            || _ItemWebsite.Item.ItemNumber.Contains(mSearch) 
                    //            || _ItemWebsite.Item.StoreDisplayName.Contains(mSearch)
                    //            || _ItemWebsite.Item.SalesDescription.Contains(mSearch)   
                    //        )
                    //    )
                    //    {
                    //        MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, _ItemWebsite.ItemID));
                    //    }
                    //}

                    SqlDataReader objRead = null;
                    string strSQL = string.Empty;

                    try
                    {
                        strSQL = string.Format(@"
SELECT i.ItemID
FROM Item (NOLOCK) i
Inner Join ItemWebsite (NOLOCK) iw on iw.ItemID = i.ItemID
Outer Apply
(
	SELECT Top 1 ItemID FROM Item (NOLOCK) i2
	WHERE i2.ParentID = i.ItemID
	AND i2.IsOnline = 1
	AND i2.InActive = 0
	AND I.InternalID not like '%_inactive'
) child
WHERE i.InActive = 0
and i.IsOnline = 1
and isnull(i.ParentID,0) = 0
AND child.ItemID is not null
and iw.WebsiteID = {0}
and (
	i.ItemName like '%' + {1} + '%'
	or i.ItemNumber like '%' + {1} + '%'
	or i.StoreDisplayName like '%' + {1} + '%'
	or i.SalesDescription like '%' + {1} + '%'
) "
                        , Database.HandleQuote(CurrentWebsite.WebsiteID)
                        , IsNumberic(mSearch) ? String.Format("'{0}'", mSearch) : Database.HandleQuote(mSearch));

                        objRead = Database.GetDataReader(strSQL);

                        while (objRead.Read())
                        {
                            MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, Convert.ToString(objRead["ItemID"])));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (objRead != null) objRead.Dispose();
                        objRead = null;
                    }
                }
                else if (!string.IsNullOrEmpty(mWebSiteTabID))
                {
                    objFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                    objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.WebsiteTabID.SearchString = mWebSiteTabID;
                    objWebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(objFilter);
                    //rptItems.DataSource = objWebsiteTabItems;

                    foreach (ImageSolutions.Website.WebsiteTabItem _WebsiteTabItem in objWebsiteTabItems)
                    {
                        MyGroupItems.Add(new ImageSolutions.Item.MyGroupItem(CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroupID, _WebsiteTabItem.ItemID));
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
                        }
                    }
                }

                if (rblPackage != null && !string.IsNullOrEmpty(rblPackage.SelectedValue))
                {
                    List<ImageSolutions.Item.MyGroupItem> MyGroupPackageItems = new List<ImageSolutions.Item.MyGroupItem>();

                    ImageSolutions.Package.Package Package = new ImageSolutions.Package.Package(rblPackage.SelectedValue);
                    foreach (MyGroupItem _MyGroupItem in MyGroupItems)
                    {
                        Item Item = new Item(_MyGroupItem.ItemID);                        
                        if (Package.PackageLines.Exists(x => x.PackageGroupID == Item.PackageGroupID))
                        {
                            MyGroupPackageItems.Add(_MyGroupItem);
                        }
                    }

                    rptItems.DataSource = MyGroupPackageItems;
                }
                else
                {
                    rptItems.DataSource = MyGroupItems;
                }


                rptItems.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally
            {
                objWebsiteTabItems = null;
                objFilter = null;
            }
        }
        public bool IsNumberic(string stringValue)
        {
            var pattern = @"^-?\d+(?:\.\d+)?$";
            var regex = new Regex(pattern);
            return regex.IsMatch(stringValue);
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
                                Panel pnlGroupSingleAttribute = (Panel)e.Item.FindControl("pnlGroupSingleAttribute");
                                pnlGroupSingleAttribute.Visible = true;

                                if (!CurrentWebsite.EnablePackagePayment)
                                {
                                    Repeater rptGroupSingleAttributeValue = (Repeater)e.Item.FindControl("rptGroupSingleAttributeValue");
                                    rptGroupSingleAttributeValue.DataSource = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues;
                                    rptGroupSingleAttributeValue.DataBind();
                                }
                                else
                                {
                                    HtmlGenericControl divMobile = (HtmlGenericControl)e.Item.FindControl("divMobile");
                                    divMobile.Attributes["class"] = "d-block d-md-block";
                                }

                                Repeater rptGroupSingleAttributeValue2 = (Repeater)e.Item.FindControl("rptGroupSingleAttributeValue2");
                                rptGroupSingleAttributeValue2.DataSource = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues;
                                rptGroupSingleAttributeValue2.DataBind();


                            }
                            else
                            {
                                Panel pnlGroupMultipleAttribute = (Panel)e.Item.FindControl("pnlGroupMultipleAttribute");
                                pnlGroupMultipleAttribute.Visible = true;

                                Repeater rptMultipleGroupAttributes = (Repeater)e.Item.FindControl("rptMultipleGroupAttributes");
                                rptMultipleGroupAttributes.DataSource = Attributes.FindAll(x => x.AttributeID != Item.GroupByAttributeID);
                                rptMultipleGroupAttributes.DataBind();

                                List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                                ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                                AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                                AttributeValueFilter.AttributeID.SearchString = Item.GroupByAttributeID;
                                AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                                Repeater rptMutlipleGroupAttributeValue = (Repeater)e.Item.FindControl("rptMutlipleGroupAttributeValue");
                                rptMutlipleGroupAttributeValue.DataSource = AttributeValues;
                                rptMutlipleGroupAttributeValue.DataBind();
                            }
                        }
                        else
                        {
                            Repeater rptNoGroupAttributes = (Repeater)e.Item.FindControl("rptNoGroupAttributes");
                            rptNoGroupAttributes.DataSource = Attributes.FindAll(x => x.AttributeID != Item.GroupByAttributeID);  //DropDownAttributes;
                            rptNoGroupAttributes.DataBind();

                            Panel pnlNoGroup = (Panel)e.Item.FindControl("pnlNoGroup");
                            pnlNoGroup.Visible = true;
                        }

                        HyperLink hlnkProductDetail = (HyperLink)e.Item.FindControl("hlnkProductDetail");
                        HyperLink hlnkProductDetailImage = (HyperLink)e.Item.FindControl("hlnkProductDetailImage");
                        Literal litStoreDisplayName = (Literal)e.Item.FindControl("litStoreDisplayName");

                        litStoreDisplayName.Text = Item.StoreDisplayNameHTML;

                        if (CurrentWebsite.EnablePackagePayment)
                        {
                            hlnkProductDetail.NavigateUrl = string.Empty;
                            hlnkProductDetailImage.NavigateUrl = string.Empty;

                            if (Item.ItemDetails != null && Item.ItemDetails.Count > 0)
                            {
                                string strProductDetailInfo = string.Empty;

                                foreach (ImageSolutions.Item.ItemDetail _ItemDetail in Item.ItemDetails)
                                {
                                    if(!string.IsNullOrEmpty(strProductDetailInfo))
                                    {
                                        strProductDetailInfo = string.Format(@"{0}&#013;&#013;", strProductDetailInfo);
                                    }

                                    strProductDetailInfo = string.Format(@"{0}{1}", strProductDetailInfo, _ItemDetail.Attribute);
                                    foreach (ImageSolutions.Item.ItemDetailValue _ItemDetailValue in _ItemDetail.ItemDetailValues)
                                    {
                                        strProductDetailInfo = string.Format(@"{0}&#013; - {1}", strProductDetailInfo, _ItemDetailValue.Value);
                                    }
                                }

                                //strProductDetailInfo = "Test";

                                Literal litProductDetailInfo = (Literal)e.Item.FindControl("litProductDetailInfo");
                                litProductDetailInfo.Text = String.Format(string.Format(@"&nbsp;&nbsp;&nbsp;<span style='font-size: 24px;' title='{0}' class='custom-tooltip'><i class='ti-info-alt' style='color: blue;'></i></span>", strProductDetailInfo));
                                //litProductDetailInfo.Text = String.Format(string.Format(@"&nbsp;&nbsp;&nbsp;<span class='custom-tooltip'><i class='ti-info-alt' style='color: blue;'></i><span class='custom-tooltip-text'>{0}</span></span>", strProductDetailInfo));
                            }
                        }
                        else
                        {
                            hlnkProductDetail.Text = Item.StoreDisplayNameHTML;
                            hlnkProductDetail.NavigateUrl = string.Format(@"ProductDetail.aspx?id={0}&WebsiteTabID={1}", Item.ItemID, mWebSiteTabID);
                            hlnkProductDetailImage.NavigateUrl = string.Format(@"ProductDetail.aspx?id={0}&WebsiteTabID={1}", Item.ItemID, mWebSiteTabID);
                        }
                    }
                    else
                    {
                        TextBox txtQuantity = (TextBox)e.Item.FindControl("txtQuantity");
                    }
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
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
        protected void ddlAttributeValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList DropDownList = (DropDownList)sender;
            string strAttributeValueID = Convert.ToString(DropDownList.SelectedValue);

            ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(strAttributeValueID);
            string strItemID = AttributeValue.Attribute.ItemID;
            SetQuantityField();
        }


        protected void BindGroupSingleAttributeValue(Repeater rptGroupSingleAttributeValue)
        {
            foreach (RepeaterItem _RepeaterItem in rptGroupSingleAttributeValue.Items)
            {
                List<string> AttributeValueIDs = new List<string>();

                HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                AttributeValueIDs.Add(hfAttributeValueID.Value);

                Repeater rptGroupAttribute = (Repeater)_RepeaterItem.FindControl("rptGroupAttribute");
                foreach (RepeaterItem _ListRepeaterItem in rptGroupAttribute.Items)
                {
                    TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                    HiddenField hfListAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfListAttributeValueID");
                    AttributeValueIDs.Add(hfListAttributeValueID.Value);

                    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                    
                    //txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.QuantityAvailable > 0;
                    txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && ValidateBackOrder(Item, 1);

                    Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                    //lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;

                    //MyGroupItem objMyGroupItem = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID);

                    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID).Price) : string.Empty;

                    Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.QuantityAvailable + " Available" : "0 Available";
                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;


                    HtmlGenericControl spanBackOrderMessage = (HtmlGenericControl)_ListRepeaterItem.FindControl("spanBackOrderMessage");
                    if (
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "production" && CurrentWebsite.WebsiteID == "2")
                        ||
                        (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging" && CurrentWebsite.WebsiteID == "26")
                    )
                    {
                        spanBackOrderMessage.Visible = txtGroupAttributeQuantity.Enabled && ((Item.IsNonInventory && Item.VendorInventory == 0) || (!Item.IsNonInventory && Item.QuantityAvailable == 0));
                    }
                    else
                    {
                        spanBackOrderMessage.Visible = false;
                    }

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

        protected void SetQuantityField()
        {
            foreach (RepeaterItem _Item in rptItems.Items)
            {
                Panel pnlNoAttribute = (Panel)_Item.FindControl("pnlNoAttribute");
                Panel pnlNoGroup = (Panel)_Item.FindControl("pnlNoGroup");
                Panel pnlGroupSingleAttribute = (Panel)_Item.FindControl("pnlGroupSingleAttribute");
                Panel pnlGroupMultipleAttribute = (Panel)_Item.FindControl("pnlGroupMultipleAttribute");

                if (pnlNoAttribute.Visible)
                {
                    string strItemID = ((HiddenField)_Item.FindControl("hfItemID")).Value;
                    ImageSolutions.Item.Item objItem = new ImageSolutions.Item.Item(strItemID);

                    TextBox txtQuantity = (TextBox)_Item.FindControl("txtQuantity");
                    //txtQuantity.Enabled = objItem.AvailableInventory > 0;
                    txtQuantity.Enabled = ValidateBackOrder(objItem, 1);

                    Label lblUnitPrice = (Label)_Item.FindControl("lblUnitPrice");
                    //lblUnitPrice.Text = String.Format("{0:c}", objItem.BasePrice);
                    lblUnitPrice.Text = String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == strItemID).Price);


                    Label lblQuantityAvailable = (Label)_Item.FindControl("lblQuantityAvailable");
                    lblQuantityAvailable.Text = objItem.QuantityAvailable.ToString() + " Available";
                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;
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

                    //txtNoGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.AvailableInventory > 0;
                    txtNoGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && ValidateBackOrder(Item, 1);

                    Label lblUnitPrice = (Label)_Item.FindControl("lblNoGroupAttributeUnitPrice");
                    //lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;
                    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID).Price) : string.Empty;

                    Label lblQuantityAvailable = (Label)_Item.FindControl("lblNoGroupAttributeQuantityAvailable");
                    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.QuantityAvailable + " Available" : "0 Available";
                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;
                }
                else
                {
                    if (pnlGroupSingleAttribute.Visible)
                    {
                        Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");
                        Repeater rptGroupSingleAttributeValue2 = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue2");

                        BindGroupSingleAttributeValue(rptGroupSingleAttributeValue);
                        BindGroupSingleAttributeValue(rptGroupSingleAttributeValue2);

                        //foreach (RepeaterItem _RepeaterItem in rptGroupSingleAttributeValue.Items)
                        //{
                        //    List<string> AttributeValueIDs = new List<string>();

                        //    HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                        //    AttributeValueIDs.Add(hfAttributeValueID.Value);

                        //    Repeater rptGroupAttribute = (Repeater)_RepeaterItem.FindControl("rptGroupAttribute");
                        //    foreach (RepeaterItem _ListRepeaterItem in rptGroupAttribute.Items)
                        //    {
                        //        TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                        //        HiddenField hfListAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfListAttributeValueID");
                        //        AttributeValueIDs.Add(hfListAttributeValueID.Value);

                        //        ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                        //        txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.AvailableInventory > 0;

                        //        Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                        //        //lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;
                        //        lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID).Price) : string.Empty;

                        //        Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                        //        lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.QuantityAvailable + " Available" : "0 Available";
                        //        lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;

                        //        LinkButton lbnGroupAttributeSuperceedingItem = (LinkButton)_ListRepeaterItem.FindControl("lbnGroupAttributeSuperceedingItem");
                        //        lbnGroupAttributeSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                        //        lbnGroupAttributeSuperceedingItem.CommandArgument = Item != null ? Item.ItemID : String.Empty;

                        //        //HtmlAnchor aSuperceedingItem = (HtmlAnchor)_ListRepeaterItem.FindControl("aGroupAttributeSuperceedingItem");
                        //        //aSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                        //        //if (Item != null)
                        //        //{
                        //        //    aSuperceedingItem.HRef = "/ItemList.aspx?itemid=" + (Item.ParentItem != null ? Item.ParentItem.ItemID : Item.ItemID);
                        //        //}
                        //        AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                        //    }
                        //}
                    }
                    else
                    {
                        List<string> AttributeValueIDs = new List<string>();
                        Repeater rptMultipleGroupAttributes = (Repeater)_Item.FindControl("rptMultipleGroupAttributes");
                        foreach (RepeaterItem _RepeaterAttributeItem in rptMultipleGroupAttributes.Items)
                        {
                            DropDownList ddlAttributeValue = (DropDownList)_RepeaterAttributeItem.FindControl("ddlAttributeValue");
                            AttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                        }
                        Repeater rptMutlipleGroupAttributeValue = (Repeater)_Item.FindControl("rptMutlipleGroupAttributeValue");
                        foreach (RepeaterItem _ListRepeaterItem in rptMutlipleGroupAttributeValue.Items)
                        {
                            TextBox txtGroupAttributeQuantity = (TextBox)_ListRepeaterItem.FindControl("txtGroupAttributeQuantity");

                            HiddenField hfAttributeValueID = (HiddenField)_ListRepeaterItem.FindControl("hfAttributeValueID");
                            AttributeValueIDs.Add(hfAttributeValueID.Value);

                            //Add to Cart
                            ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                            //txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.AvailableInventory > 0;
                            txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && ValidateBackOrder(Item, 1);

                            Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                            //lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;
                            lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID).Price) : string.Empty;

                            Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                            lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.QuantityAvailable + " Available" : "0 Available";
                            lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;

                            LinkButton lbnGroupAttributeSuperceedingItem = (LinkButton)_ListRepeaterItem.FindControl("lbnGroupAttributeSuperceedingItem");
                            lbnGroupAttributeSuperceedingItem.Visible = lblQuantityAvailable.Text == "0 Available" && (Item != null && Item.SuperceedingItems != null && Item.SuperceedingItems.Count > 0);
                            lbnGroupAttributeSuperceedingItem.CommandArgument = Item != null ? Item.ItemID : String.Empty;


                            AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                        }

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
                    //List<ImageSolutions.Item.ItemAttributeValue> ItemAttributeValues = new List<ImageSolutions.Item.ItemAttributeValue>();
                    //ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                    //ItemAttributeValueFilter.AttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                    //ItemAttributeValueFilter.AttributeValueID.SearchString = _AttributeValueID;
                    //ItemAttributeValues = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValues(ItemAttributeValueFilter);

                    List<string> AttributeItemIDs = GetItemAttributeValues(_AttributeValueID);

                    if (AttributeItemIDs != null && AttributeItemIDs.Count > 0)
                    //if (ItemAttributeValues != null && ItemAttributeValues.Count > 0)
                    {
                        if (ItemIDs.Count == 0)
                        {
                            foreach (string _ItemID in AttributeItemIDs)
                            //foreach (ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                            {
                                //ItemIDs.Add(_ItemAttributeValue.ItemID);
                                ItemIDs.Add(_ItemID);
                            }
                        }
                        else
                        {
                            List<string> UpdateItemIds = new List<string>();

                            foreach (string _ItemID in AttributeItemIDs)
                            //foreach (ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                            {
                                //if (ItemIDs.Contains(_ItemAttributeValue.ItemID))
                                //{
                                //    UpdateItemIds.Add(_ItemAttributeValue.ItemID);
                                //}
                                if (ItemIDs.Contains(_ItemID))
                                {
                                    UpdateItemIds.Add(_ItemID);
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
                Response.Redirect("Error.aspx");
                throw ex;
            }
            finally
            {
            }
        }


        protected List<string> GetItemAttributeValues(string attributevalueid)
        {
            List<string> blnReturn = null;
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                blnReturn = new List<string>();
                strSQL = string.Format(@"
SELECT ItemID FROM ItemAttributeValue (NOLOCK) WHERE 1=1 AND AttributeValueID = {0}
"
                    , Database.HandleQuote(attributevalueid));

                objRead = Database.GetDataReader(strSQL);
                while (objRead.Read())
                {
                    blnReturn.Add(Convert.ToString(objRead["ItemID"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return blnReturn;
        }

        protected void AddToCartRepeater(Repeater rptGroupSingleAttributeValue, SqlConnection conn, SqlTransaction tran)
        {
            foreach (RepeaterItem _AttributeItem in rptGroupSingleAttributeValue.Items)
            {
                List<string> GroupSingleAttributeValueIDs = new List<string>();

                HiddenField hfAttributeValueID = (HiddenField)_AttributeItem.FindControl("hfAttributeValueID");
                if (hfAttributeValueID != null && !String.IsNullOrEmpty(hfAttributeValueID.Value))
                {
                    GroupSingleAttributeValueIDs.Add(hfAttributeValueID.Value);
                }

                HiddenField hfAttributeValueID2 = (HiddenField)_AttributeItem.FindControl("hfAttributeValueID2");
                if (hfAttributeValueID2 != null && !String.IsNullOrEmpty(hfAttributeValueID2.Value))
                {
                    GroupSingleAttributeValueIDs.Add(hfAttributeValueID2.Value);
                }

                Repeater rptGroupAttribute = (Repeater)_AttributeItem.FindControl("rptGroupAttribute");
                foreach (RepeaterItem _AttributeValueItem in rptGroupAttribute.Items)
                {
                    TextBox txtGroupAttributeQuantity = (TextBox)_AttributeValueItem.FindControl("txtGroupAttributeQuantity");
                    if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
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
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = GroupItem.ItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                        newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;

                        if (!ValidateBackOrder(newShoppingCartLine.Item, newShoppingCartLine.Quantity))
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", newShoppingCartLine.Item.SalesDescription, newShoppingCartLine.Item.IsNonInventory ? newShoppingCartLine.Item.VendorInventory : newShoppingCartLine.Item.QuantityAvailable));
                        }
                            
                        newShoppingCartLine.Create(conn, tran);

                        if (newShoppingCartLine.Item.ParentItem.DefaultSelectableLogo)
                        {
                            ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo();
                            ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                            ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                            ItemSelectableLogoFilter.ItemID.SearchString = newShoppingCartLine.Item.ParentID;
                            ItemSelectableLogo = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogo(ItemSelectableLogoFilter);

                            if (ItemSelectableLogo != null && !string.IsNullOrEmpty(ItemSelectableLogo.ItemSelectableLogoID))
                            {
                                ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo ShoppingCartLineSelectableLogo = new ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo();
                                ShoppingCartLineSelectableLogo.ShoppingCartLineID = newShoppingCartLine.ShoppingCartLineID;
                                ShoppingCartLineSelectableLogo.SelectableLogoID = Convert.ToString(ItemSelectableLogo.SelectableLogoID);
                                ShoppingCartLineSelectableLogo.BasePrice = ItemSelectableLogo.SelectableLogo.BasePrice;
                                ShoppingCartLineSelectableLogo.Create(conn, tran);
                            }
                        }

                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update(conn, tran);

                        //txtGroupAttributeQuantity.Text = String.Empty;

                        GroupSingleAttributeValueIDs.RemoveAt(GroupSingleAttributeValueIDs.Count - 1);
                    }
                }
            }
        }

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (AddToCart())
            {
                Response.Redirect("/ShoppingCart.aspx");
            }
        }

        protected bool AddToCart()
        {
            bool blnReturn = true;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                foreach (RepeaterItem _Item in this.rptItems.Items)
                {
                    string strItemID = ((HiddenField)_Item.FindControl("hfItemID")).Value;

                    //No Attribute
                    TextBox txtQuantity = (TextBox)_Item.FindControl("txtQuantity");
                    if (txtQuantity != null && !string.IsNullOrEmpty(txtQuantity.Text))
                    {
                        //ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);
                        //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        //newShoppingCartLine.ItemID = strItemID;
                        //newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                        //newShoppingCartLine.UnitPrice = Item.BasePrice == null ? (double)0 : Item.BasePrice.Value;
                        //newShoppingCartLine.Create();

                        ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == strItemID);
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = strItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                        newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;

                        if (!ValidateBackOrder(newShoppingCartLine.Item, newShoppingCartLine.Quantity))
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", newShoppingCartLine.Item.SalesDescription, newShoppingCartLine.Item.IsNonInventory ? newShoppingCartLine.Item.VendorInventory : newShoppingCartLine.Item.QuantityAvailable));
                        }

                        newShoppingCartLine.Create(objConn, objTran);

                        if (newShoppingCartLine.Item.ParentItem.DefaultSelectableLogo)
                        {
                            ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo();
                            ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                            ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                            ItemSelectableLogoFilter.ItemID.SearchString = newShoppingCartLine.Item.ParentID;
                            ItemSelectableLogo = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogo(ItemSelectableLogoFilter);

                            if (ItemSelectableLogo != null && !string.IsNullOrEmpty(ItemSelectableLogo.ItemSelectableLogoID))
                            {
                                ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo ShoppingCartLineSelectableLogo = new ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo();
                                ShoppingCartLineSelectableLogo.ShoppingCartLineID = newShoppingCartLine.ShoppingCartLineID;
                                ShoppingCartLineSelectableLogo.SelectableLogoID = Convert.ToString(ItemSelectableLogo.SelectableLogoID);
                                ShoppingCartLineSelectableLogo.BasePrice = ItemSelectableLogo.SelectableLogo.BasePrice;
                                ShoppingCartLineSelectableLogo.Create(objConn, objTran);
                            }
                        }
                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update(objConn, objTran);

                        //txtQuantity.Text = String.Empty;
                    }

                    //Attribute - No Group
                    Repeater rptNoGroupAttributes = (Repeater)_Item.FindControl("rptNoGroupAttributes");

                    TextBox txtNoGroupAttributeQuantity = (TextBox)_Item.FindControl("txtNoGroupAttributeQuantity");

                    if (txtNoGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtNoGroupAttributeQuantity.Text))
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
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = NoGroupItem.ItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                        newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;

                        if (!ValidateBackOrder(newShoppingCartLine.Item, newShoppingCartLine.Quantity))
                        {
                            throw new Exception(String.Format("Not enough inventory available {0} ({1})", newShoppingCartLine.Item.SalesDescription, newShoppingCartLine.Item.IsNonInventory ? newShoppingCartLine.Item.VendorInventory : newShoppingCartLine.Item.QuantityAvailable));
                        }

                        newShoppingCartLine.Create(objConn, objTran);

                        if (newShoppingCartLine.Item.ParentItem.DefaultSelectableLogo)
                        {
                            ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo();
                            ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                            ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                            ItemSelectableLogoFilter.ItemID.SearchString = newShoppingCartLine.Item.ParentID;
                            ItemSelectableLogo = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogo(ItemSelectableLogoFilter);

                            if (ItemSelectableLogo != null && !string.IsNullOrEmpty(ItemSelectableLogo.ItemSelectableLogoID))
                            {
                                ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo ShoppingCartLineSelectableLogo = new ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo();
                                ShoppingCartLineSelectableLogo.ShoppingCartLineID = newShoppingCartLine.ShoppingCartLineID;
                                ShoppingCartLineSelectableLogo.SelectableLogoID = Convert.ToString(ItemSelectableLogo.SelectableLogoID);
                                ShoppingCartLineSelectableLogo.BasePrice = ItemSelectableLogo.SelectableLogo.BasePrice;
                                ShoppingCartLineSelectableLogo.Create(objConn, objTran);
                            }
                        }

                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update(objConn, objTran);

                        txtNoGroupAttributeQuantity.Text = String.Empty;
                    }

                    //Attribute - Group - Single
                    Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");
                    Repeater rptGroupSingleAttributeValue2 = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue2");

                    AddToCartRepeater(rptGroupSingleAttributeValue, objConn, objTran);
                    AddToCartRepeater(rptGroupSingleAttributeValue2, objConn, objTran);
                    //foreach (RepeaterItem _AttributeItem in rptGroupSingleAttributeValue.Items)
                    //{
                    //    List<string> GroupSingleAttributeValueIDs = new List<string>();

                    //    HiddenField hfAttributeValueID = (HiddenField)_AttributeItem.FindControl("hfAttributeValueID");
                    //    GroupSingleAttributeValueIDs.Add(hfAttributeValueID.Value);

                    //    Repeater rptGroupAttribute = (Repeater)_AttributeItem.FindControl("rptGroupAttribute");
                    //    foreach (RepeaterItem _AttributeValueItem in rptGroupAttribute.Items)
                    //    {
                    //        TextBox txtGroupAttributeQuantity = (TextBox)_AttributeValueItem.FindControl("txtGroupAttributeQuantity");
                    //        if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
                    //        {
                    //            HiddenField hfListAttributeValueID = (HiddenField)_AttributeValueItem.FindControl("hfListAttributeValueID");
                    //            GroupSingleAttributeValueIDs.Add(hfListAttributeValueID.Value);
                    //            ImageSolutions.Item.Item GroupItem = FindVariationItem(GroupSingleAttributeValueIDs);

                    //            //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    //            //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    //            //newShoppingCartLine.ItemID = GroupItem.ItemID;
                    //            //newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                    //            //newShoppingCartLine.UnitPrice = GroupItem.BasePrice == null ? (double)0 : GroupItem.BasePrice.Value;
                    //            //newShoppingCartLine.Create();

                    //            ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == GroupItem.ItemID);
                    //            ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    //            newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    //            newShoppingCartLine.ItemID = GroupItem.ItemID;
                    //            newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                    //            newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;
                    //            newShoppingCartLine.Create();
                    //            CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                    //            txtGroupAttributeQuantity.Text = String.Empty;

                    //            GroupSingleAttributeValueIDs.RemoveAt(GroupSingleAttributeValueIDs.Count - 1);
                    //        }                                           
                    //    }
                    //}

                    //Attribute - Group - Multiple
                    Repeater rptMultipleGroupAttributes = (Repeater)_Item.FindControl("rptMultipleGroupAttributes");
                    Repeater rptMutlipleGroupAttributeValue = (Repeater)_Item.FindControl("rptMutlipleGroupAttributeValue");

                    foreach (RepeaterItem _RepeaterItem in rptMutlipleGroupAttributeValue.Items)
                    {
                        TextBox txtGroupAttributeQuantity = (TextBox)_RepeaterItem.FindControl("txtGroupAttributeQuantity");

                        if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
                        {
                            List<string> MultipleGroupAttributeValueIDs = new List<string>();
                            foreach (RepeaterItem _AttributeValueItem in rptMultipleGroupAttributes.Items)
                            {
                                DropDownList ddlAttributeValue = (DropDownList)_AttributeValueItem.FindControl("ddlAttributeValue");
                                MultipleGroupAttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                            }

                            HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                            MultipleGroupAttributeValueIDs.Add(hfAttributeValueID.Value);
                            ImageSolutions.Item.Item GroupItem = FindVariationItem(MultipleGroupAttributeValueIDs);

                            //ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                            //newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                            //newShoppingCartLine.ItemID = GroupItem.ItemID;
                            //newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                            //newShoppingCartLine.UnitPrice = GroupItem.BasePrice == null ? (double)0 : GroupItem.BasePrice.Value;
                            //newShoppingCartLine.Create();

                            ImageSolutions.Item.MyGroupItem MyGroupItemWebsite = CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == GroupItem.ItemID);
                            ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                            newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                            newShoppingCartLine.ItemID = GroupItem.ItemID;
                            newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                            newShoppingCartLine.UnitPrice = MyGroupItemWebsite.Price;

                            if (!ValidateBackOrder(newShoppingCartLine.Item, newShoppingCartLine.Quantity))
                            {
                                throw new Exception(String.Format("Not enough inventory available {0} ({1})", newShoppingCartLine.Item.SalesDescription, newShoppingCartLine.Item.IsNonInventory ? newShoppingCartLine.Item.VendorInventory : newShoppingCartLine.Item.QuantityAvailable));
                            }

                            newShoppingCartLine.Create(objConn, objTran);

                            if (newShoppingCartLine.Item.ParentItem.DefaultSelectableLogo)
                            {
                                ImageSolutions.Item.ItemSelectableLogo ItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo();
                                ImageSolutions.Item.ItemSelectableLogoFilter ItemSelectableLogoFilter = new ImageSolutions.Item.ItemSelectableLogoFilter();
                                ItemSelectableLogoFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                                ItemSelectableLogoFilter.ItemID.SearchString = newShoppingCartLine.Item.ParentID;
                                ItemSelectableLogo = ImageSolutions.Item.ItemSelectableLogo.GetItemSelectableLogo(ItemSelectableLogoFilter);

                                if (ItemSelectableLogo != null && !string.IsNullOrEmpty(ItemSelectableLogo.ItemSelectableLogoID))
                                {
                                    ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo ShoppingCartLineSelectableLogo = new ImageSolutions.ShoppingCart.ShoppingCartLineSelectableLogo();
                                    ShoppingCartLineSelectableLogo.ShoppingCartLineID = newShoppingCartLine.ShoppingCartLineID;
                                    ShoppingCartLineSelectableLogo.SelectableLogoID = Convert.ToString(ItemSelectableLogo.SelectableLogoID);
                                    ShoppingCartLineSelectableLogo.BasePrice = ItemSelectableLogo.SelectableLogo.BasePrice;
                                    ShoppingCartLineSelectableLogo.Create(objConn, objTran);
                                }
                            }

                            CurrentUser.CurrentUserWebSite.ShoppingCart.Update(objConn, objTran);

                            //txtGroupAttributeQuantity.Text = String.Empty;

                            MultipleGroupAttributeValueIDs.RemoveAt(MultipleGroupAttributeValueIDs.Count - 1);
                        }
                    }
                }

                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                }

                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                blnReturn = false;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

            return blnReturn;
        }

        protected bool ValidateBackOrder(ImageSolutions.Item.Item item, int quantity)
        {
            bool blnReturn = true;

            if(
                (!item.AllowBackOrder || CurrentWebsite.DisallowBackOrder)
                &&
                (
                    (item.IsNonInventory && quantity > item.VendorInventory)
                    ||
                    (!item.IsNonInventory && quantity > item.QuantityAvailable)
                )
            )
            {
                blnReturn = false;
            }

            return blnReturn;
        }

        protected void rptGroupSingleAttributeValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hfAttributeValueID = (HiddenField)e.Item.FindControl("hfAttributeValueID");
                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(hfAttributeValueID.Value);

                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = AttributeValue.Attribute.Item.GroupByAttributeID;
                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                    Repeater rptGroupAttribute = (Repeater)e.Item.FindControl("rptGroupAttribute");
                    rptGroupAttribute.DataSource = AttributeValues;
                    rptGroupAttribute.DataBind();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void rptGroupSingleAttributeValue2_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hfAttributeValueID2 = (HiddenField)e.Item.FindControl("hfAttributeValueID2");
                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(hfAttributeValueID2.Value);

                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = AttributeValue.Attribute.Item.GroupByAttributeID;
                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                    Repeater rptGroupAttribute = (Repeater)e.Item.FindControl("rptGroupAttribute");
                    rptGroupAttribute.DataSource = AttributeValues;
                    rptGroupAttribute.DataBind();
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

        protected bool ValidatePacakge()
        {
            bool blnReturn = true;
            List<PackageCount> PackageCounts = new List<PackageCount>();
            try
            {
                foreach (RepeaterItem _Item in this.rptItems.Items)
                {
                    string strItemID = ((HiddenField)_Item.FindControl("hfItemID")).Value;
                    int Quantity = 0;

                    //No Attribute
                    TextBox txtQuantity = (TextBox)_Item.FindControl("txtQuantity");
                    if (txtQuantity != null && !string.IsNullOrEmpty(txtQuantity.Text))
                    {
                        Item Item = new Item(strItemID);
                        Item ParentItem = new Item(Item.ParentID);

                        AddPackageCount(ParentItem, Convert.ToInt32(txtQuantity.Text), ref PackageCounts);
                    }

                    //Attribute - No Group
                    Repeater rptNoGroupAttributes = (Repeater)_Item.FindControl("rptNoGroupAttributes");

                    TextBox txtNoGroupAttributeQuantity = (TextBox)_Item.FindControl("txtNoGroupAttributeQuantity");

                    if (txtNoGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtNoGroupAttributeQuantity.Text))
                    {
                        List<string> NoGroupAttributeValueIDs = new List<string>();
                        foreach (RepeaterItem _AttributeItem in rptNoGroupAttributes.Items)
                        {
                            DropDownList ddlAttributeValue = (DropDownList)_AttributeItem.FindControl("ddlAttributeValue");
                            NoGroupAttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                        }
                        ImageSolutions.Item.Item NoGroupItem = FindVariationItem(NoGroupAttributeValueIDs);
                        
                        string strSize = string.Empty;
                        foreach(string _attributevalue in NoGroupAttributeValueIDs)
                        {
                            ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(_attributevalue);
                            if (AttributeValue != null && AttributeValue.Attribute.AttributeName == "Size")
                            {
                                strSize = AttributeValue.Value;
                            }
                        }

                        Item ParentItem = new Item(NoGroupItem.ParentID);
                        AddPackageCount(ParentItem, Convert.ToInt32(txtQuantity.Text), ref PackageCounts, strSize);
                    }

                    //Attribute - Group - Single
                    Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");
                    foreach (RepeaterItem _AttributeItem in rptGroupSingleAttributeValue.Items)
                    {
                        List<string> GroupSingleAttributeValueIDs = new List<string>();

                        HiddenField hfAttributeValueID = (HiddenField)_AttributeItem.FindControl("hfAttributeValueID");
                        GroupSingleAttributeValueIDs.Add(hfAttributeValueID.Value);

                        Repeater rptGroupAttribute = (Repeater)_AttributeItem.FindControl("rptGroupAttribute");
                        foreach (RepeaterItem _AttributeValueItem in rptGroupAttribute.Items)
                        {
                            TextBox txtGroupAttributeQuantity = (TextBox)_AttributeValueItem.FindControl("txtGroupAttributeQuantity");
                            if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
                            {
                                HiddenField hfListAttributeValueID = (HiddenField)_AttributeValueItem.FindControl("hfListAttributeValueID");
                                GroupSingleAttributeValueIDs.Add(hfListAttributeValueID.Value);
                                ImageSolutions.Item.Item GroupItem = FindVariationItem(GroupSingleAttributeValueIDs);

                                string strSize = string.Empty;
                                foreach (string _attributevalue in GroupSingleAttributeValueIDs)
                                {
                                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(_attributevalue);
                                    if (AttributeValue != null && AttributeValue.Attribute.AttributeName == "Size")
                                    {
                                        strSize = AttributeValue.Value;
                                    }
                                }

                                Item ParentItem = new Item(GroupItem.ParentID);
                                AddPackageCount(ParentItem, Convert.ToInt32(txtGroupAttributeQuantity.Text), ref PackageCounts, strSize);

                                GroupSingleAttributeValueIDs.RemoveAt(GroupSingleAttributeValueIDs.Count - 1);
                            }
                        }
                    }

                    Repeater rptGroupSingleAttributeValue2 = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue2");
                    foreach (RepeaterItem _AttributeItem in rptGroupSingleAttributeValue2.Items)
                    {
                        List<string> GroupSingleAttributeValueIDs = new List<string>();

                        HiddenField hfAttributeValueID2 = (HiddenField)_AttributeItem.FindControl("hfAttributeValueID2");
                        GroupSingleAttributeValueIDs.Add(hfAttributeValueID2.Value);

                        Repeater rptGroupAttribute = (Repeater)_AttributeItem.FindControl("rptGroupAttribute");
                        foreach (RepeaterItem _AttributeValueItem in rptGroupAttribute.Items)
                        {
                            TextBox txtGroupAttributeQuantity = (TextBox)_AttributeValueItem.FindControl("txtGroupAttributeQuantity");
                            if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
                            {
                                HiddenField hfListAttributeValueID = (HiddenField)_AttributeValueItem.FindControl("hfListAttributeValueID");
                                GroupSingleAttributeValueIDs.Add(hfListAttributeValueID.Value);
                                ImageSolutions.Item.Item GroupItem = FindVariationItem(GroupSingleAttributeValueIDs);

                                string strSize = string.Empty;
                                foreach (string _attributevalue in GroupSingleAttributeValueIDs)
                                {
                                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(_attributevalue);
                                    if (AttributeValue != null && AttributeValue.Attribute.AttributeName == "Size")
                                    {
                                        strSize = AttributeValue.Value;
                                    }
                                }

                                Item ParentItem = new Item(GroupItem.ParentID);
                                AddPackageCount(ParentItem, Convert.ToInt32(txtGroupAttributeQuantity.Text), ref PackageCounts, strSize);

                                GroupSingleAttributeValueIDs.RemoveAt(GroupSingleAttributeValueIDs.Count - 1);
                            }
                        }
                    }


                    //Attribute - Group - Multiple
                    Repeater rptMultipleGroupAttributes = (Repeater)_Item.FindControl("rptMultipleGroupAttributes");
                    Repeater rptMutlipleGroupAttributeValue = (Repeater)_Item.FindControl("rptMutlipleGroupAttributeValue");

                    foreach (RepeaterItem _RepeaterItem in rptMutlipleGroupAttributeValue.Items)
                    {
                        TextBox txtGroupAttributeQuantity = (TextBox)_RepeaterItem.FindControl("txtGroupAttributeQuantity");

                        if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
                        {
                            List<string> MultipleGroupAttributeValueIDs = new List<string>();
                            foreach (RepeaterItem _AttributeValueItem in rptMultipleGroupAttributes.Items)
                            {
                                DropDownList ddlAttributeValue = (DropDownList)_AttributeValueItem.FindControl("ddlAttributeValue");
                                MultipleGroupAttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                            }

                            HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                            MultipleGroupAttributeValueIDs.Add(hfAttributeValueID.Value);
                            ImageSolutions.Item.Item GroupItem = FindVariationItem(MultipleGroupAttributeValueIDs);

                            string strSize = string.Empty;
                            foreach (string _attributevalue in MultipleGroupAttributeValueIDs)
                            {
                                ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(_attributevalue);
                                if (AttributeValue != null && AttributeValue.Attribute.AttributeName == "Size")
                                {
                                    strSize = AttributeValue.Value;
                                }
                            }

                            Item ParentItem = new Item(GroupItem.ParentID);
                            AddPackageCount(ParentItem, Convert.ToInt32(txtGroupAttributeQuantity.Text), ref PackageCounts, strSize);

                            MultipleGroupAttributeValueIDs.RemoveAt(MultipleGroupAttributeValueIDs.Count - 1);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                blnReturn = false;
            }

            if (blnReturn)
            {
                string strPackageMessage = string.Empty;

                //ImageSolutions.Package.Package Package = new ImageSolutions.Package.Package(Convert.ToString(ddlPackage.SelectedValue));
                ImageSolutions.Package.Package Package = new ImageSolutions.Package.Package(Convert.ToString(rblPackage.SelectedValue));
                foreach (ImageSolutions.Package.PackageLine _PacakgeLine in Package.PackageLines)
                {
                    if (PackageCounts.Exists(x => x.PackageGroupID == _PacakgeLine.PackageGroupID))
                    {
                        PackageCount PackageCount = PackageCounts.Find(x => x.PackageGroupID == _PacakgeLine.PackageGroupID);
                        ImageSolutions.Package.PackageGroup PackageGroup = new ImageSolutions.Package.PackageGroup(_PacakgeLine.PackageGroupID);

                        if (blnReturn && _PacakgeLine.IsOptional && chkOptionalField.Checked)
                        {
                            strPackageMessage = string.Format(@"{0}\nToo Many '{1}' - Quantity To Remove {2}", strPackageMessage, PackageGroup.Name, PackageCount.Quantity);
                            blnReturn = false;
                        }

                        if (_PacakgeLine.ValidateSingleSize && !PackageCount.IsSingleAttribute)
                        {
                            strPackageMessage = string.Format(@"{0}\nMultiple sizes are not allowed for '{1}'", strPackageMessage, PackageGroup.Name);
                            blnReturn = false;
                        }

                        if (blnReturn && PackageCount.Quantity != _PacakgeLine.Quantity)
                        {
                            if (PackageCount.Quantity > _PacakgeLine.Quantity)
                            {
                                strPackageMessage = string.Format(@"{0}\nToo Many '{1}' - Quantity To Remove {2}", strPackageMessage, PackageGroup.Name, PackageCount.Quantity - _PacakgeLine.Quantity);
                            }
                            else
                            {
                                strPackageMessage = string.Format(@"{0}\nMissing '{1}' - Quantity To Add {2}", strPackageMessage, PackageGroup.Name, _PacakgeLine.Quantity - PackageCount.Quantity);
                            }
                            blnReturn = false;
                        }
                    }
                    else
                    {
                        if (!_PacakgeLine.IsOptional || !chkOptionalField.Checked)
                        {
                            ImageSolutions.Package.PackageGroup PackageGroup = new ImageSolutions.Package.PackageGroup(_PacakgeLine.PackageGroupID);
                            strPackageMessage = string.Format(@"{0}\nMissing '{1}' - Quantity To Add: {2}", strPackageMessage, PackageGroup.Name, _PacakgeLine.Quantity);
                            blnReturn = false;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(strPackageMessage))
                {
                    WebUtility.DisplayJavascriptMessage(this, strPackageMessage);
                }
            }

            return blnReturn;
        }

        protected void AddPackageCount(Item item, int quantity, ref List<PackageCount> packagecounts, string attribute = "")
        {
            if (!string.IsNullOrEmpty(item.PackageGroupID))
            {
                if (packagecounts.Exists(x => x.PackageGroupID == item.PackageGroupID))
                {
                    PackageCount PackageCount = packagecounts.Find(x => x.PackageGroupID == item.PackageGroupID);
                    PackageCount.Quantity += quantity;
                    if (PackageCount.IsSingleAttribute)
                    {
                        PackageCount.IsSingleAttribute = PackageCount.Attribute == attribute;
                    }
                }
                else
                {
                    PackageCount PackageCount = new PackageCount();
                    PackageCount.PackageGroupID = item.PackageGroupID;
                    PackageCount.Quantity = quantity;
                    PackageCount.Attribute = attribute;
                    PackageCount.IsSingleAttribute = true;
                    packagecounts.Add(PackageCount);
                }
            }
            else
            {
                throw new Exception(string.Format("Item {0} not available for package", item.ItemName));
            }            
        }


        protected void btnAddToCartPackage_Click(object sender, EventArgs e)
        {
            //if(!string.IsNullOrEmpty(ddlPackage.SelectedValue))
            //{
            //    if (ValidatePacakge())
            //    {
            //        //Add to Cart
            //        AddToCart();

            //        CurrentUser.CurrentUserWebSite.ShoppingCart.PackageID = ddlPackage.SelectedValue;
            //        CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

            //        Response.Redirect("/Checkout.aspx");
            //    }
            //}

            if (CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines != null && CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines.Count > 0)
            {
                foreach (ImageSolutions.ShoppingCart.ShoppingCartLine _ShoppingCartLine in CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartLines)
                {
                    _ShoppingCartLine.Delete();
                }
            }

            if (!string.IsNullOrEmpty(rblPackage.SelectedValue))
            {
                if (ValidatePacakge())
                {
                    //Add to Cart
                    
                    if (AddToCart())
                    {
                        CurrentUser.CurrentUserWebSite.ShoppingCart.PackageID = rblPackage.SelectedValue;
                        CurrentUser.CurrentUserWebSite.ShoppingCart.ExcludeOptional = chkOptionalField.Checked;
                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                        Response.Redirect("/Checkout.aspx");
                    }
                }
            }
            else
            {
                WebUtility.DisplayJavascriptMessage(this, "Package must be selected.");
            }
        }

        public class PackageCount
        {
            public string PackageGroupID { get; set; }
            public int Quantity { get; set; }
            public string Attribute { get; set; }
            public bool IsSingleAttribute { get; set; }
        }

        protected void ddlPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            litPackageDetail.Text = String.Empty;

            ImageSolutions.Package.Package Package = new ImageSolutions.Package.Package(ddlPackage.SelectedValue);

            if (Package != null)
            {
                litPackageDetail.Text = string.Format(@"<b>{0}</b><br>This package includes:", string.IsNullOrEmpty(Package.DisplayName) ? Package.Name : Package.DisplayName);
                foreach (ImageSolutions.Package.PackageLine _PackageLine in Package.PackageLines)
                {
                    if (_PackageLine.IsOptional)
                    {
                        litPackageDetail.Text = string.Format(@"{0}<br>- {1} : qty {2} (Optional)", litPackageDetail.Text, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
                    }
                    else
                    {
                        litPackageDetail.Text = string.Format(@"{0}<br>- {1} : qty {2}", litPackageDetail.Text, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
                    }
                }
            }
        }

        protected void rblPackage_SelectedIndexChanged(object sender, EventArgs e)
        {

            litPackageDetail.Text = String.Empty;

            ImageSolutions.Package.Package Package = new ImageSolutions.Package.Package(rblPackage.SelectedValue);

            //if (Package != null)
            //{
            //    foreach (ImageSolutions.Package.PackageLine _PackageLine in Package.PackageLines)
            //    {
            //        litPackageDetail.Text = string.Format(@"{0}<br>- {1} : {2}", litPackageDetail.Text, _PackageLine.PackageGroup.Name, _PackageLine.Quantity);
            //    }
            //}

            litPackageDetail.Text = Package.DescriptionHTMLSelected;

            rblPackage.Visible = false;

            litSelectPackageHeader.Text = string.Format(@"<br><p style=""color: blue; text-transform: unset; font-size: 24px; line-height: 1.5;"">Following the package guidelines below, enter the specific quantity of tops and bottoms as well as select a vest or fleece jacket.<br>Full package allotment must be used at once.</p><br>");
            
            if (!string.IsNullOrEmpty(CurrentWebsite.DefaultSizeChartPath))
            {
                litSelectPackageHeader.Text = String.Format(@"{0}<p style=""color: blue; text-transform: unset; font-size: 24px;""><a href=""{1}"" target=""_blank""><u>Size Chart</u></a></p>", litSelectPackageHeader.Text, CurrentWebsite.DefaultSizeChartPath);
            }

            divList.Visible = true;
            btnAddToCartPackage.Visible = true;
            btnPackageBack.Visible = true;

            bool blnHasOptionalField = false;
            foreach (ImageSolutions.Package.PackageLine _PackageLine in Package.PackageLines)
            {
                if (_PackageLine.IsOptional)
                {
                    blnHasOptionalField = true;
                }
            }

            if (blnHasOptionalField && !string.IsNullOrEmpty(rblPackage.SelectedValue) && !string.IsNullOrEmpty(Package.OptionalFieldText))
            {
                pnlOptionalField.Visible = true;
                chkOptionalField.Text = Package.OptionalFieldText;
                litOptionalFieldMessage.Text = Package.OptionalFieldMessage;
            }
            else
            {
                chkOptionalField.Checked = false;
                pnlOptionalField.Visible = false;
            }

            BindItems();
        }

        protected void btnPackageBack_Click(object sender, EventArgs e)
        {
            litPackageDetail.Text = String.Empty;

            rblPackage.Visible = true;
            litSelectPackageHeader.Text = string.Format(@"<p style=""color: blue; font-size: 24px; font-family: 'Lato';"">Select one of the packages below.</p><br>");

            divList.Visible = false;
            btnAddToCartPackage.Visible = false;
            btnPackageBack.Visible = false;
        }
    }
}