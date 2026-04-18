using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemVariation : BasePageAdminUserWebSiteAuth
    {
        protected string mItemID = string.Empty;

        List<ItemVariation> mItemVariations = new List<ItemVariation>();

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Item.aspx?id=" + mItemID + "&tab=3";
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
            mItemID = Request.QueryString.Get("itemid");

            if (!Page.IsPostBack)
            {
                BindItemVariation();
            }
        }

        protected void BindItemVariation()
        {
            List<ImageSolutions.Attribute.Attribute> Attributes = new List<ImageSolutions.Attribute.Attribute>();
            ImageSolutions.Attribute.AttributeFilter AttributeFilter = new ImageSolutions.Attribute.AttributeFilter();
            AttributeFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
            AttributeFilter.ItemID.SearchString = mItemID;
            Attributes = ImageSolutions.Attribute.Attribute.GetAttributes(AttributeFilter);

            Dictionary<string, List<string>> objAttributes = new Dictionary<string, List<string>>();

            foreach (ImageSolutions.Attribute.Attribute _Attribute in Attributes)
            {
                List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                AttributeValueFilter.AttributeID.SearchString = _Attribute.AttributeID;
                AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                List<string> values = new List<string>();
                foreach (ImageSolutions.Attribute.AttributeValue _AttributeValue in AttributeValues)
                {
                    values.Add(_AttributeValue.Value);
                }                
                if(values.Count > 0)
                {
                    objAttributes.Add(_Attribute.AttributeID, values);
                }
            }

            mItemVariations = GetCombos(objAttributes);

            int intCounter = 0;
            foreach(ItemVariation _ItemVariation in mItemVariations)
            {
                intCounter++;
                _ItemVariation.VariationID = Convert.ToString(intCounter);
                ImageSolutions.Item.Item Item = FindVariationItem(_ItemVariation.AttributeValues);
                
                if(Item != null)
                {
                    _ItemVariation.ItemID = Item.ItemID;
                    _ItemVariation.ItemNumber = Item.ItemNumber;
                    _ItemVariation.ItemName = Item.ItemName;
                    _ItemVariation.BasePrice = Item.BasePrice;
                    _ItemVariation.PurchasePrice = Item.PurchasePrice;
                }
            }

            gvItemVariation.DataSource = mItemVariations;
            gvItemVariation.DataBind();
        }

        ImageSolutions.Item.Item FindVariationItem(List<ImageSolutions.Attribute.AttributeValue> attributevalues)
        {
            try
            {
                List<string> ItemIDs = new List<string>();

                foreach(ImageSolutions.Attribute.AttributeValue _AttributeValue in attributevalues)
                {
                    List<ImageSolutions.Item.ItemAttributeValue> ItemAttributeValues = new List<ImageSolutions.Item.ItemAttributeValue>();
                    ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                    ItemAttributeValueFilter.AttributeValueID = new Database.Filter.StringSearch.SearchFilter();
                    ItemAttributeValueFilter.AttributeValueID.SearchString = _AttributeValue.AttributeValueID;
                    ItemAttributeValues = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValues(ItemAttributeValueFilter);
                    
                    if(ItemAttributeValues != null && ItemAttributeValues.Count > 0)
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
                                if(ItemIDs.Contains(_ItemAttributeValue.ItemID))
                                {
                                    UpdateItemIds.Add(_ItemAttributeValue.ItemID);
                                }
                            }

                            ItemIDs = UpdateItemIds;

                            if(ItemIDs.Count == 0)
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

                if(ItemIDs.Count == 1)
                {
                    return new ImageSolutions.Item.Item(Convert.ToString(ItemIDs[0]));
                }
                else if(ItemIDs.Count == 0)
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

        List<ItemVariation> GetCombos(IEnumerable<KeyValuePair<string, List<string>>> attributes)
        {
            KeyValuePair<string, List<string>> Current = attributes.First();
            List<ItemVariation> ItemVariations = new List<ItemVariation>();

            if (attributes.Count() == 1)
            {

                foreach(string _Value in attributes.First().Value)
                {
                    ItemVariation ItemVariation = new ItemVariation();
                  
                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = Convert.ToString(Current.Key);
                    AttributeValueFilter.Value = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.Value.SearchString = Convert.ToString(_Value);
                    AttributeValue = ImageSolutions.Attribute.AttributeValue.GetAttributeValue(AttributeValueFilter);                    

                    ItemVariation.AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ItemVariation.AttributeValues.Add(AttributeValue);
                    ItemVariations.Add(ItemVariation);
                }
            }
            else
            {
                List<ItemVariation> ComboItemVariations = GetCombos(attributes.Where(attribute => attribute.Key != Current.Key));

                foreach (string _Value in Current.Value)
                {
                    ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = Convert.ToString(Current.Key);
                    AttributeValueFilter.Value = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.Value.SearchString = Convert.ToString(_Value);
                    AttributeValue = ImageSolutions.Attribute.AttributeValue.GetAttributeValue(AttributeValueFilter);

                    foreach (ItemVariation _ItemVariation in ComboItemVariations)
                    {
                        ItemVariation ItemVariation = new ItemVariation();

                        List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                        AttributeValues.Add(AttributeValue);
                        foreach (ImageSolutions.Attribute.AttributeValue _AttributeValue in _ItemVariation.AttributeValues)
                        {
                            AttributeValues.Add(_AttributeValue);
                        }
                        ItemVariation.AttributeValues = AttributeValues;

                        ItemVariations.Add(ItemVariation);
                    }
                }
            }

            return ItemVariations;
        }

        protected void gvItemVariation_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CreateItem")
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;

                string strVariationID = Convert.ToString(e.CommandArgument);

                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    ImageSolutions.Item.Item ParentItem = new ImageSolutions.Item.Item(mItemID);

                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item();
                    Item.ParentID = ParentItem.ItemID;
                    Item.ItemNumber = ParentItem.ItemNumber + RandomString(6);
                    Item.ItemName = ParentItem.ItemName;
                    Item.ItemType = ParentItem.ItemType;
                    Item.StoreDisplayName = ParentItem.StoreDisplayName;
                    Item.SalesDescription = ParentItem.SalesDescription;
                    Item.IsOnline = ParentItem.IsOnline;
                    Item.InActive = ParentItem.InActive;
                    Item.VendorInventory = 0;
                    Item.CreatedBy = CurrentUser.UserInfoID;
                    Item.Create(objConn, objTran);

                    ImageSolutions.Item.ItemWebsite ItemWebsite = new ImageSolutions.Item.ItemWebsite();
                    ItemWebsite.WebsiteID = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    ItemWebsite.ItemID = Item.ItemID;
                    ItemWebsite.CreatedBy = CurrentUser.UserInfoID;
                    ItemWebsite.Create(objConn, objTran);

                    GridViewRow row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
                    GridView gvAttributeList = (GridView)row.FindControl("gvAttributeList");

                    int counter = 0;
                    foreach (GridViewRow _GridViewRow in gvAttributeList.Rows)
                    {
                        counter++;
                        HiddenField hfAttributeValueID = _GridViewRow.FindControl("hfAttributeValueID") as HiddenField;
                        ImageSolutions.Item.ItemAttributeValue ItemAttributeValue = new ImageSolutions.Item.ItemAttributeValue();
                        ItemAttributeValue.ItemID = Item.ItemID;
                        ItemAttributeValue.AttributeValueID = Convert.ToString(hfAttributeValueID.Value);
                        ItemAttributeValue.CreatedBy = CurrentUser.UserInfoID;
                        ItemAttributeValue.Create(objConn, objTran);
                                               
                        if (!string.IsNullOrEmpty(ItemAttributeValue.AttributeValue.Abbreviation))
                        {
                            if(counter==1)
                            {
                                Item.ItemNumber = String.Format("{0}-{1}", ParentItem.ItemNumber, ItemAttributeValue.AttributeValue.Abbreviation);
                            }
                            else
                            {
                                Item.ItemNumber = String.Format("{0}-{1}", Item.ItemNumber, ItemAttributeValue.AttributeValue.Abbreviation);
                            }
                        }
                    }

                    if (Item.ItemNumber != ParentItem.ItemNumber)
                    {
                        Item.Update(objConn, objTran);
                    }

                    objTran.Commit();

                    //WebUtility.DisplayJavascriptMessage(this, "Item Created");

                    BindItemVariation();
                }
                catch (Exception ex)
                {
                    if (objTran != null) objTran.Rollback();
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
            if (e.CommandName == "Assign")
            {
                string strItemID = Convert.ToString(e.CommandArgument);

                GridViewRow row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
                DropDownList ddlAssignItem = (DropDownList)row.FindControl("ddlAssignItem");
                Button btnAssignItem = (Button)row.FindControl("btnAssignItem");
                Button btnAssign = (Button)row.FindControl("btnAssign");
                try
                {
                    List<ImageSolutions.Item.ItemWebsite> ItemWebsites = new List<ImageSolutions.Item.ItemWebsite>();
                    ImageSolutions.Item.ItemWebsiteFilter ItemWebsiteFilter = new ImageSolutions.Item.ItemWebsiteFilter();
                    ItemWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                    ItemWebsiteFilter.WebsiteID.SearchString = CurrentUser.CurrentUserWebSite.WebSite.WebsiteID;
                    ItemWebsites = ImageSolutions.Item.ItemWebsite.GetItemWebsites(ItemWebsiteFilter);

                    ddlAssignItem.DataSource = ItemWebsites.OrderBy(x => x.ItemNumber);
                    ddlAssignItem.DataTextField = "ItemNumber";
                    ddlAssignItem.DataValueField = "ItemID";
                    ddlAssignItem.DataBind();
                    ddlAssignItem.Items.Insert(0, new ListItem(String.Empty, string.Empty));

                    ddlAssignItem.Visible = true;
                    btnAssignItem.Visible = true;
                    btnAssign.Visible = false;
                }
                catch (Exception ex)
                {
                    WebUtility.DisplayJavascriptMessage(this, ex.Message);
                    //Response.Redirect(string.Format("/Admin/TabOverview.aspx"));
                }
                finally { }
            }
            if (e.CommandName == "AssignItem")
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;

                string strItemID = Convert.ToString(e.CommandArgument);

                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    GridViewRow row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
                    GridView gvAttributeList = (GridView)row.FindControl("gvAttributeList");
                    DropDownList ddlAssignItem = (DropDownList)row.FindControl("ddlAssignItem");
                    Button btnAssignItem = (Button)row.FindControl("btnAssignItem");

                    ImageSolutions.Item.Item ParentItem = new ImageSolutions.Item.Item(mItemID);
                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(Convert.ToString(ddlAssignItem.SelectedValue));

                    Item.ParentID = ParentItem.ItemID;
                    Item.Update(objConn, objTran);

                    //Delete all existing attributes
                    List<ImageSolutions.Item.ItemAttributeValue> ItemAttributeValues = new List<ImageSolutions.Item.ItemAttributeValue>();
                    ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                    ItemAttributeValueFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    ItemAttributeValueFilter.ItemID.SearchString = Item.ItemID;
                    ItemAttributeValues = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValues(ItemAttributeValueFilter);
                    foreach (ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                    {
                        _ItemAttributeValue.Delete(objConn, objTran);
                    }

                    foreach (GridViewRow _GridViewRow in gvAttributeList.Rows)
                    {
                        HiddenField hfItemAttributeValueID = _GridViewRow.FindControl("hfAttributeValueID") as HiddenField;
                        ImageSolutions.Item.ItemAttributeValue ItemAttributeValue = new ImageSolutions.Item.ItemAttributeValue();
                        ItemAttributeValue.ItemID = Item.ItemID;
                        ItemAttributeValue.AttributeValueID = Convert.ToString(hfItemAttributeValueID.Value);
                        ItemAttributeValue.CreatedBy = CurrentUser.UserInfoID;
                        ItemAttributeValue.Create(objConn, objTran);
                    }
                    objTran.Commit();

                    //WebUtility.DisplayJavascriptMessage(this, "Item Assigned");

                    BindItemVariation();
                }
                catch (Exception ex)
                {
                    if (objTran != null) objTran.Rollback();
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
            if (e.CommandName == "LineUpdate")
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;

                string strItemID = Convert.ToString(e.CommandArgument);

                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    GridViewRow row = (GridViewRow)(((Button)e.CommandSource).NamingContainer);
                    TextBox txtBasePrice = (TextBox)row.FindControl("txtBasePrice");
                    TextBox txtPurchasePrice = (TextBox)row.FindControl("txtPurchasePrice");
                    TextBox txtItemNumber = (TextBox)row.FindControl("txtItemNumber");

                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);
                    Item.ItemNumber = Convert.ToString(txtItemNumber.Text);
                    if (!string.IsNullOrEmpty(txtBasePrice.Text))
                    {
                        Item.BasePrice = Convert.ToDouble(txtBasePrice.Text);
                    }
                    if (!string.IsNullOrEmpty(txtPurchasePrice.Text))
                    {
                        Item.PurchasePrice = Convert.ToDouble(txtPurchasePrice.Text);
                    }
                    Item.Update(objConn, objTran);

                    objTran.Commit();
                    //WebUtility.DisplayJavascriptMessage(this, "Update completed");
                }
                catch (Exception ex)
                {
                    if (objTran != null) objTran.Rollback();
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
            if (e.CommandName == "Remove")
            {
                SqlConnection objConn = null;
                SqlTransaction objTran = null;

                string strItemID = Convert.ToString(e.CommandArgument);

                try
                {
                    objConn = new SqlConnection(Database.DefaultConnectionString);
                    objConn.Open();
                    objTran = objConn.BeginTransaction();

                    ImageSolutions.Item.Item Item = new ImageSolutions.Item.Item(strItemID);
                    Item.ParentID = null;

                    int intCounter = 0;

                    ImageSolutions.Item.Item ExistItem = null;
                    ImageSolutions.Item.ItemFilter ExistItemFilter = null;
                    string updateItemNumber = Item.ItemNumber;

                    ExistItemFilter = new ImageSolutions.Item.ItemFilter();
                    ExistItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                    ExistItemFilter.ItemNumber.SearchString = updateItemNumber;
                    ExistItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                    ExistItemFilter.ParentID.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                    ExistItem = ImageSolutions.Item.Item.GetItem(ExistItemFilter);

                    while (ExistItem != null)
                    {
                        updateItemNumber = String.Format("{0}_Inactive{1}"
                            , Item.ItemNumber
                            , intCounter > 0 ? string.Format("_{0}", intCounter) : string.Empty);

                        ExistItemFilter = new ImageSolutions.Item.ItemFilter();
                        ExistItemFilter.ItemNumber = new Database.Filter.StringSearch.SearchFilter();
                        ExistItemFilter.ItemNumber.SearchString = updateItemNumber;
                        ExistItem = ImageSolutions.Item.Item.GetItem(ExistItemFilter);

                        intCounter++;
                    }
                    Item.ItemNumber = updateItemNumber;
                    Item.InternalID = String.Format("{0}_Inactive", Item.InternalID);

                    Item.Update(objConn, objTran);

                    //remove all attribute
                    List<ImageSolutions.Item.ItemAttributeValue> ItemAttributeValues = new List<ImageSolutions.Item.ItemAttributeValue>();
                    ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                    ItemAttributeValueFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    ItemAttributeValueFilter.ItemID.SearchString = Item.ItemID;
                    ItemAttributeValues = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValues(ItemAttributeValueFilter);

                    foreach(ImageSolutions.Item.ItemAttributeValue _ItemAttributeValue in ItemAttributeValues)
                    {
                        _ItemAttributeValue.Delete(objConn, objTran);
                    }

                    objTran.Commit();
                    WebUtility.DisplayJavascriptMessage(this, "Item removed from variation");
                    BindItemVariation();
                }
                catch (Exception ex)
                {
                    if (objTran != null) objTran.Rollback();
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
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/Item.aspx?id={0}", mItemID));
        }

        protected void gvItemVariation_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {                
                string strVariationID = gvItemVariation.DataKeys[e.Row.RowIndex].Value.ToString();

                GridView gvAttributeList = (GridView)e.Row.FindControl("gvAttributeList");
                Button btnNewItem = (Button)e.Row.FindControl("btnNewItem");
                TextBox txtBasePrice = (TextBox)e.Row.FindControl("txtBasePrice");
                TextBox txtPurchasePrice = (TextBox)e.Row.FindControl("txtPurchasePrice");
                TextBox txtItemNumber = (TextBox)e.Row.FindControl("txtItemNumber");
                Button btnUpdate = (Button)e.Row.FindControl("btnUpdate");
                Button btnAssign = (Button)e.Row.FindControl("btnAssign");

                List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                ItemVariation ItemVariation = mItemVariations.Find(x => x.VariationID == strVariationID);
                AttributeValues = ItemVariation.AttributeValues;
                gvAttributeList.DataSource = AttributeValues;
                gvAttributeList.DataBind();

                if (!string.IsNullOrEmpty(ItemVariation.ItemID))
                {
                    btnAssign.Enabled = false;
                    btnNewItem.Enabled = false;
                    txtBasePrice.Text = Convert.ToString(ItemVariation.BasePrice);
                    txtPurchasePrice.Text = Convert.ToString(ItemVariation.PurchasePrice);
                    txtItemNumber.Text = Convert.ToString(ItemVariation.ItemNumber);
                    btnUpdate.Enabled = true;
                }
                else
                {
                    btnAssign.Enabled = true;
                    btnNewItem.Enabled = true;
                    txtBasePrice.Visible = false;
                    txtPurchasePrice.Visible = false;
                    txtItemNumber.Visible = false;
                    btnUpdate.Enabled = false;
                }
            }
        }
        protected string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public partial class ItemVariation
    {
        public string VariationID { get; set; }
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        public List<ImageSolutions.Attribute.AttributeValue> AttributeValues { get; set; }
        public double? BasePrice { get; set; }
        public double? PurchasePrice { get; set; }
        public string ItemID { get; set; }
    }
}