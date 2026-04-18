using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ImageSolutionsWebsite
{
    public partial class ItemsListView : BasePageUserAuth
    {
        protected string mWebSiteTabID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");

            if (!Page.IsPostBack)
            {
                Initialize();
            }
        }
        protected void Initialize()
        {
            BindItems();
        }
        protected void BindItems()
        {
            List<ImageSolutions.Website.WebsiteTabItem> objWebsiteTabItems = null;
            ImageSolutions.Website.WebsiteTabItemFilter objFilter = null;
            int intTotalRecord = 0;

            try
            {
                objFilter = new ImageSolutions.Website.WebsiteTabItemFilter();
                objFilter.WebsiteTabID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteTabID.SearchString = mWebSiteTabID;
                objWebsiteTabItems = ImageSolutions.Website.WebsiteTabItem.GetWebsiteTabItems(objFilter);
                //gvItem.DataSource = objWebsiteTabItems;
                //gvItem.DataBind();

                rptItems.DataSource = objWebsiteTabItems;
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

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            GridViewRow row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);

            if (e.CommandName == "AddNoAttributeItem")
            {
                string strItemID = Convert.ToString(e.CommandArgument);
                ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);

                TextBox txtQuantity = (TextBox)row.FindControl("txtQuantity");

                if (Item != null)
                {
                    ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    newShoppingCartLine.ItemID = Item.ItemID;
                    newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                    newShoppingCartLine.UnitPrice = Item.BasePrice == null ? (double)0 : Item.BasePrice.Value;
                    newShoppingCartLine.Create();

                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update();
                }

                txtQuantity.Text = String.Empty;
            }
            else if (e.CommandName == "AddAttributeItem")
            {
                string strItemID = Convert.ToString(e.CommandArgument);
                List<string> AttributeValueIDs = new List<string>();

                GridView gvAttributes = (GridView)row.FindControl("gvAttributes");
                foreach (GridViewRow _Row in gvAttributes.Rows)
                {
                    DropDownList ddlAttributeValue = (DropDownList)_Row.FindControl("ddlAttributeValue");

                    AttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                }

                TextBox txtNoGroupAttributeQuantity = (TextBox)row.FindControl("txtNoGroupAttributeQuantity");
                if (!string.IsNullOrEmpty(txtNoGroupAttributeQuantity.Text) && Convert.ToInt32(txtNoGroupAttributeQuantity.Text) > 0)
                {
                    //Add to Cart
                    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);

                    if (Item != null)
                    {
                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = Item.ItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtNoGroupAttributeQuantity.Text);
                        newShoppingCartLine.UnitPrice = Item.BasePrice == null ? (double)0 : Item.BasePrice.Value;
                        newShoppingCartLine.Create();

                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                        txtNoGroupAttributeQuantity.Text = String.Empty;
                    }
                }
                else
                {
                    Repeater rptGroupAttribute = (Repeater)row.FindControl("rptGroupAttribute");
                    foreach (RepeaterItem _Item in rptGroupAttribute.Items)
                    {
                        TextBox txtGroupAttributeQuantity = (TextBox)_Item.FindControl("txtGroupAttributeQuantity");

                        if (!string.IsNullOrEmpty(txtGroupAttributeQuantity.Text) && Convert.ToInt32(txtGroupAttributeQuantity.Text) > 0)
                        {
                            HiddenField hfAttributeValueID = (HiddenField)_Item.FindControl("hfAttributeValueID");
                            AttributeValueIDs.Add(hfAttributeValueID.Value);

                            //Add to Cart
                            ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);

                            if (Item != null)
                            {
                                ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                                newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                                newShoppingCartLine.ItemID = Item.ItemID;
                                newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                                newShoppingCartLine.UnitPrice = Item.BasePrice == null ? (double)0 : Item.BasePrice.Value;
                                newShoppingCartLine.Create();

                                CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                                txtGroupAttributeQuantity.Text = String.Empty;
                            }

                            //Remove last AttributeValue added
                            AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                        }
                    }
                }
            }
        }

        //protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            string strItemID = gvItem.DataKeys[e.Row.RowIndex].Value.ToString();
        //            ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);

        //            Image imgItem = (Image)e.Row.FindControl("imgItem");
        //            imgItem.ImageUrl = Item.ImageURL;

        //            List<ImageSolutions.Attribute.Attribute> Attributes = new List<ImageSolutions.Attribute.Attribute>();
        //            ImageSolutions.Attribute.AttributeFilter AttributeFilter = new ImageSolutions.Attribute.AttributeFilter();
        //            AttributeFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
        //            AttributeFilter.ItemID.SearchString = Item.ItemID;
        //            Attributes = ImageSolutions.Attribute.Attribute.GetAttributes(AttributeFilter);

        //            Panel pnlAttribute = (Panel)e.Row.FindControl("pnlAttribute");
        //            pnlAttribute.Visible = Attributes != null && Attributes.Count > 0;

        //            Panel pnlNoAttribute = (Panel)e.Row.FindControl("pnlNoAttribute");
        //            pnlNoAttribute.Visible = !pnlAttribute.Visible;

        //            if (pnlAttribute.Visible)
        //            {
        //                List<ImageSolutions.Attribute.Attribute> DropDownAttributes = new List<ImageSolutions.Attribute.Attribute>();
        //                foreach (ImageSolutions.Attribute.Attribute _Attribute in Attributes)
        //                {
        //                    if (_Attribute.AttributeID != Item.GroupByAttributeID)
        //                    {
        //                        DropDownAttributes.Add(_Attribute);
        //                    }
        //                }
        //                GridView gvAttributes = (GridView)e.Row.FindControl("gvAttributes");
        //                gvAttributes.DataSource = DropDownAttributes;
        //                gvAttributes.DataBind();

        //                if (!string.IsNullOrEmpty(Item.GroupByAttributeID))
        //                {
        //                    Panel pnlGroup = (Panel)e.Row.FindControl("pnlGroup");
        //                    pnlGroup.Visible = true;

        //                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
        //                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
        //                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
        //                    AttributeValueFilter.AttributeID.SearchString = Item.GroupByAttributeID;
        //                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

        //                    Repeater rptGroupAttribute = (Repeater)e.Row.FindControl("rptGroupAttribute");
        //                    rptGroupAttribute.DataSource = AttributeValues;
        //                    rptGroupAttribute.DataBind();
        //                }
        //                else
        //                {
        //                    Panel pnlNoGroup = (Panel)e.Row.FindControl("pnlNoGroup");
        //                    pnlNoGroup.Visible = true;
        //                }
        //            }
        //            else
        //            {
        //                TextBox txtQuantity = (TextBox)e.Row.FindControl("txtQuantity");
        //            }

        //            SetQuantityField(strItemID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WebUtility.DisplayJavascriptMessage(this, ex.Message);
        //    }
        //}

        protected void btnAddToCart_Click(object sender, EventArgs e)
        {
            foreach (RepeaterItem _Row in this.rptItems.Items)
            {
                string strItemID = ((HiddenField)_Row.FindControl("hfItemID")).Value;

                //No Attribute
                TextBox txtQuantity = (TextBox)_Row.FindControl("txtQuantity");
                if (txtQuantity != null && !string.IsNullOrEmpty(txtQuantity.Text))
                {
                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);

                    ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    newShoppingCartLine.ItemID = strItemID;
                    newShoppingCartLine.Quantity = Convert.ToInt32(txtQuantity.Text);
                    newShoppingCartLine.UnitPrice = Item.BasePrice == null ? (double)0 : Item.BasePrice.Value;
                    newShoppingCartLine.Create();

                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                    txtQuantity.Text = String.Empty;
                }

                //Attribute - No Group
                GridView gvAttributes = (GridView)_Row.FindControl("gvAttributes");

                TextBox txtNoGroupAttributeQuantity = (TextBox)_Row.FindControl("txtNoGroupAttributeQuantity");

                if (txtNoGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtNoGroupAttributeQuantity.Text))
                {
                    List<string> NoGroupAttributeValueIDs = new List<string>();
                    foreach (GridViewRow _AttributeRow in gvAttributes.Rows)
                    {
                        DropDownList ddlAttributeValue = (DropDownList)_AttributeRow.FindControl("ddlAttributeValue");
                        NoGroupAttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                    }
                    ImageSolutions.Item.Item NoGroupItem = FindVariationItem(NoGroupAttributeValueIDs);

                    ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                    newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                    newShoppingCartLine.ItemID = NoGroupItem.ItemID;
                    newShoppingCartLine.Quantity = Convert.ToInt32(txtNoGroupAttributeQuantity.Text);
                    newShoppingCartLine.UnitPrice = NoGroupItem.BasePrice == null ? (double)0 : NoGroupItem.BasePrice.Value;
                    newShoppingCartLine.Create();

                    CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                    txtNoGroupAttributeQuantity.Text = String.Empty;
                }

                //Attribute - Group
                Repeater rptGroupAttribute = (Repeater)_Row.FindControl("rptGroupAttribute");

                foreach (RepeaterItem _RepeaterItem in rptGroupAttribute.Items)
                {
                    TextBox txtGroupAttributeQuantity = (TextBox)_RepeaterItem.FindControl("txtGroupAttributeQuantity");

                    if (txtGroupAttributeQuantity != null && !string.IsNullOrEmpty(txtGroupAttributeQuantity.Text))
                    {
                        List<string> GroupAttributeValueIDs = new List<string>();
                        foreach (GridViewRow _AttributeRow in gvAttributes.Rows)
                        {
                            DropDownList ddlAttributeValue = (DropDownList)_AttributeRow.FindControl("ddlAttributeValue");
                            GroupAttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                        }

                        HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                        GroupAttributeValueIDs.Add(hfAttributeValueID.Value);
                        ImageSolutions.Item.Item GroupItem = FindVariationItem(GroupAttributeValueIDs);

                        ImageSolutions.ShoppingCart.ShoppingCartLine newShoppingCartLine = new ImageSolutions.ShoppingCart.ShoppingCartLine();
                        newShoppingCartLine.ShoppingCartID = CurrentUser.CurrentUserWebSite.ShoppingCart.ShoppingCartID;
                        newShoppingCartLine.ItemID = GroupItem.ItemID;
                        newShoppingCartLine.Quantity = Convert.ToInt32(txtGroupAttributeQuantity.Text);
                        newShoppingCartLine.UnitPrice = GroupItem.BasePrice == null ? (double)0 : GroupItem.BasePrice.Value;
                        newShoppingCartLine.Create();

                        CurrentUser.CurrentUserWebSite.ShoppingCart.Update();

                        txtGroupAttributeQuantity.Text = String.Empty;

                        GroupAttributeValueIDs.RemoveAt(GroupAttributeValueIDs.Count - 1);
                    }
                }
            }
        }

        protected void gvAttributes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    HiddenField hfAttributeID = (HiddenField)e.Row.FindControl("hfAttributeID");
                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = Convert.ToString(hfAttributeID.Value);
                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                    DropDownList ddlAttributeValue = (DropDownList)e.Row.FindControl("ddlAttributeValue");
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

        protected void ddlAttributeValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList DropDownList = (DropDownList)sender;
            string strAttributeValueID = Convert.ToString(DropDownList.SelectedValue);

            ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue(strAttributeValueID);
            string strItemID = AttributeValue.Attribute.ItemID;
            SetQuantityField(strItemID);
        }
        protected void SetQuantityField(string itemid)
        {
            foreach (RepeaterItem _Item in rptItems.Items)
            {
                Panel pnlNoAttribute = (Panel)_Item.FindControl("pnlNoAttribute");
                Panel pnlNoGroup = (Panel)_Item.FindControl("pnlNoGroup");

                if (pnlNoAttribute.Visible)
                {
                    TextBox txtQuantity = (TextBox)_Item.FindControl("txtQuantity");
                }
                else if (pnlNoGroup.Visible)
                {
                    TextBox txtNoGroupAttributeQuantity = (TextBox)_Item.FindControl("txtNoGroupAttributeQuantity");

                    List<string> AttributeValueIDs = new List<string>();
                    GridView gvAttributes = (GridView)_Item.FindControl("gvAttributes");
                    foreach (GridViewRow _AttributeRow in gvAttributes.Rows)
                    {
                        DropDownList ddlAttributeValue = (DropDownList)_AttributeRow.FindControl("ddlAttributeValue");
                        AttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                    }

                    ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);

                    txtNoGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID);
                }
                else
                {
                    Repeater rptGroupAttribute = (Repeater)_Item.FindControl("rptGroupAttribute");

                    foreach (RepeaterItem _RepeaterItem in rptGroupAttribute.Items)
                    {
                        TextBox txtGroupAttributeQuantity = (TextBox)_RepeaterItem.FindControl("txtGroupAttributeQuantity");

                        List<string> AttributeValueIDs = new List<string>();
                        GridView gvAttributes = (GridView)_Item.FindControl("gvAttributes");
                        foreach (GridViewRow _AttributeRow in gvAttributes.Rows)
                        {
                            DropDownList ddlAttributeValue = (DropDownList)_AttributeRow.FindControl("ddlAttributeValue");
                            AttributeValueIDs.Add(ddlAttributeValue.SelectedValue);
                        }

                        HiddenField hfAttributeValueID = (HiddenField)_RepeaterItem.FindControl("hfAttributeValueID");
                        AttributeValueIDs.Add(hfAttributeValueID.Value);

                        //Add to Cart
                        ImageSolutions.Item.Item Item = FindVariationItem(AttributeValueIDs);
                        txtGroupAttributeQuantity.Enabled = Item != null && !string.IsNullOrEmpty(Item.ItemID);

                        AttributeValueIDs.RemoveAt(AttributeValueIDs.Count - 1);
                    }
                }
            }
        }

        protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    ImageSolutions.Website.WebsiteTabItem objWebsiteTabItem = (ImageSolutions.Website.WebsiteTabItem)e.Item.DataItem;
                    ImageSolutions.Item.Item Item = objWebsiteTabItem.Item;

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
                        List<ImageSolutions.Attribute.Attribute> DropDownAttributes = new List<ImageSolutions.Attribute.Attribute>();
                        foreach (ImageSolutions.Attribute.Attribute _Attribute in Attributes)
                        {
                            if (_Attribute.AttributeID != Item.GroupByAttributeID)
                            {
                                DropDownAttributes.Add(_Attribute);
                            }
                        }
                        GridView gvAttributes = (GridView)e.Item.FindControl("gvAttributes");
                        gvAttributes.DataSource = DropDownAttributes;
                        gvAttributes.DataBind();

                        if (!string.IsNullOrEmpty(Item.GroupByAttributeID))
                        {
                            Panel pnlGroup = (Panel)e.Item.FindControl("pnlGroup");
                            pnlGroup.Visible = true;

                            List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                            ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                            AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                            AttributeValueFilter.AttributeID.SearchString = Item.GroupByAttributeID;
                            AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                            Repeater rptGroupAttribute = (Repeater)e.Item.FindControl("rptGroupAttribute");
                            rptGroupAttribute.DataSource = AttributeValues;
                            rptGroupAttribute.DataBind();
                        }
                        else
                        {
                            Panel pnlNoGroup = (Panel)e.Item.FindControl("pnlNoGroup");
                            pnlNoGroup.Visible = true;
                        }
                    }
                    else
                    {
                        TextBox txtQuantity = (TextBox)e.Item.FindControl("txtQuantity");
                    }

                    SetQuantityField(Item.ItemID);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }
    }
}