using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        protected void Page_Load(object sender, EventArgs e)
        {
            mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");
            mItemID = Request.QueryString.Get("ItemID");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }
        protected void Initialize()
        {
            if (!string.IsNullOrEmpty(mWebSiteTabID) || !string.IsNullOrEmpty(mItemID)) BindItems();

            SetQuantityField();
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

                            if(AttributeCount == 1)
                            {
                                Panel pnlGroupSingleAttribute = (Panel)e.Item.FindControl("pnlGroupSingleAttribute");
                                pnlGroupSingleAttribute.Visible = true;

                                Repeater rptGroupSingleAttributeValue = (Repeater)e.Item.FindControl("rptGroupSingleAttributeValue");
                                rptGroupSingleAttributeValue.DataSource = Attributes.Find(x => x.AttributeID != Item.GroupByAttributeID).AttributeValues;
                                rptGroupSingleAttributeValue.DataBind();
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
                    txtQuantity.Enabled = objItem.QuantityAvailable > 0;

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

                    txtNoGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.QuantityAvailable > 0;

                    Label lblUnitPrice = (Label)_Item.FindControl("lblNoGroupAttributeUnitPrice");
                    //lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;
                    lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID).Price) : string.Empty;

                    Label lblQuantityAvailable = (Label)_Item.FindControl("lblNoGroupAttributeQuantityAvailable");
                    lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.QuantityAvailable + " Available" : "0 Available";
                    lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;
                }
                else
                {
                    if(pnlGroupSingleAttribute.Visible)
                    {
                        Repeater rptGroupSingleAttributeValue = (Repeater)_Item.FindControl("rptGroupSingleAttributeValue");

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
                                txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.QuantityAvailable > 0;

                                Label lblUnitPrice = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeUnitPrice");
                                //lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", Item.BasePrice) : string.Empty;
                                lblUnitPrice.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? String.Format("{0:c}", CurrentUser.CurrentUserWebSite.CurrentUserAccount.MyGroupItems.Find(x => x.ItemID == Item.ItemID).Price) : string.Empty;

                                Label lblQuantityAvailable = (Label)_ListRepeaterItem.FindControl("lblGroupAttributeQuantityAvailable");
                                lblQuantityAvailable.Text = (Item != null && !string.IsNullOrEmpty(Item.ItemID)) ? Item.QuantityAvailable + " Available" : "0 Available";
                                lblQuantityAvailable.Visible = CurrentWebsite.ShowAvailableInventory;

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
                            txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID) && Item.QuantityAvailable > 0;

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

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
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
                    newShoppingCartLine.Create();
                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                    txtQuantity.Text = String.Empty;
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
                    newShoppingCartLine.Create();
                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                    txtNoGroupAttributeQuantity.Text = String.Empty;
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
                            newShoppingCartLine.Create();
                            CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                            txtGroupAttributeQuantity.Text = String.Empty;

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
                        newShoppingCartLine.Create();
                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                        txtGroupAttributeQuantity.Text = String.Empty;

                        MultipleGroupAttributeValueIDs.RemoveAt(MultipleGroupAttributeValueIDs.Count - 1);
                    }
                }
            }
            Response.Redirect("/ShoppingCart.aspx");
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

        protected void rptGroupAttribute_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string strItemID = e.CommandArgument.ToString();
            ucSuperceedingItem.ItemID = strItemID;
            ucSuperceedingItem.Show();
        }
    }
}