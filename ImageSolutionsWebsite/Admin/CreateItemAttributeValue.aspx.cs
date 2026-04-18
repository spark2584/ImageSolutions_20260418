using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateItemAttributeValue : BasePageAdminUserWebSiteAuth
    {
        private string mAttributeID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("attributeid")))
            {
                mAttributeID = Request.QueryString.Get("attributeid");
            }
            else
            {
                Response.Redirect(String.Format("/Admin/ItemOverview.aspx"));
            }

            if (!Page.IsPostBack)
            {
                Init();   
            }
        }
        protected void Init()
        {
            ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue();
            ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
            AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
            AttributeValueFilter.AttributeID.SearchString = mAttributeID;
            AttributeValue = ImageSolutions.Attribute.AttributeValue.GetAttributeValue(AttributeValueFilter);

            if(AttributeValue == null || string.IsNullOrEmpty(AttributeValue.AttributeID))
            {
                cbIsDefault.Checked = true;
                cbIsDefault.Enabled = false;

                //Validate for any items with existing attribute
                ImageSolutions.Attribute.Attribute Attribute = new ImageSolutions.Attribute.Attribute(mAttributeID);
                List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();
                ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                ItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                ItemFilter.ParentID.SearchString = Attribute.ItemID;
                Items = ImageSolutions.Item.Item.GetItems(ItemFilter);
                foreach (ImageSolutions.Item.Item _Item in Items)
                {
                    ImageSolutions.Item.ItemAttributeValue ItemAttributeValue = new ImageSolutions.Item.ItemAttributeValue();
                    ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                    ItemAttributeValueFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                    ItemAttributeValueFilter.ItemID.SearchString = _Item.ItemID;
                    ItemAttributeValue = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValue(ItemAttributeValueFilter);

                    if (ItemAttributeValue != null && !string.IsNullOrEmpty(ItemAttributeValue.ItemAttributeValueID))
                    {
                        lblExistingItemMessage.Text = "There are item(s) existing with the existing attribute.  Input attribute will be assigned to the existing item(s).";
                        break;
                    }
                }
            }
            else
            {
                cbIsDefault.Enabled = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/ItemAttribute.aspx?id={0}", mAttributeID));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                ImageSolutions.Attribute.AttributeValue AttributeValue = new ImageSolutions.Attribute.AttributeValue();
                AttributeValue.AttributeID = Convert.ToString(mAttributeID);
                AttributeValue.Value = txtAttributeValue.Text.Trim();
                AttributeValue.Abbreviation = txtAbbreviation.Text.Trim();
                AttributeValue.IsDefault = cbIsDefault.Checked;
                AttributeValue.Create(objConn, objTran);

                if(AttributeValue.IsDefault)
                {
                    List<ImageSolutions.Attribute.AttributeValue> AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = new ImageSolutions.Attribute.AttributeValueFilter();
                    AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    AttributeValueFilter.AttributeID.SearchString = AttributeValue.AttributeID;
                    AttributeValueFilter.IsDefault = true;
                    AttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(AttributeValueFilter);

                    foreach (ImageSolutions.Attribute.AttributeValue _AttributeValue in AttributeValues)
                    {
                        if (_AttributeValue.AttributeValueID != AttributeValue.AttributeValueID)
                        {
                            _AttributeValue.IsDefault = false;
                            _AttributeValue.Update(objConn, objTran);
                        }
                    }
                }

                //Assign attribute to existing items
                if (!string.IsNullOrEmpty(lblExistingItemMessage.Text))
                {
                    //Validate for any items with existing attribute
                    ImageSolutions.Attribute.Attribute Attribute = new ImageSolutions.Attribute.Attribute(AttributeValue.AttributeID);
                    List<ImageSolutions.Item.Item> Items = new List<ImageSolutions.Item.Item>();
                    ImageSolutions.Item.ItemFilter ItemFilter = new ImageSolutions.Item.ItemFilter();
                    ItemFilter.ParentID = new Database.Filter.StringSearch.SearchFilter();
                    ItemFilter.ParentID.SearchString = Attribute.ItemID;
                    Items = ImageSolutions.Item.Item.GetItems(ItemFilter);
                    foreach (ImageSolutions.Item.Item _Item in Items)
                    {
                        ImageSolutions.Item.ItemAttributeValue ItemAttributeValue = new ImageSolutions.Item.ItemAttributeValue();
                        ImageSolutions.Item.ItemAttributeValueFilter ItemAttributeValueFilter = new ImageSolutions.Item.ItemAttributeValueFilter();
                        ItemAttributeValueFilter.ItemID = new Database.Filter.StringSearch.SearchFilter();
                        ItemAttributeValueFilter.ItemID.SearchString = _Item.ItemID;
                        ItemAttributeValue = ImageSolutions.Item.ItemAttributeValue.GetItemAttributeValue(ItemAttributeValueFilter);

                        if (ItemAttributeValue != null && !string.IsNullOrEmpty(ItemAttributeValue.ItemAttributeValueID))
                        {
                            ImageSolutions.Item.ItemAttributeValue NewItemAttributeValue = new ImageSolutions.Item.ItemAttributeValue();
                            NewItemAttributeValue.ItemID = _Item.ItemID;
                            NewItemAttributeValue.AttributeValueID = Convert.ToString(AttributeValue.AttributeValueID);
                            NewItemAttributeValue.Create(objConn, objTran);
                        }
                    }
                }

                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null) objTran.Rollback();
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }

            Response.Redirect(String.Format("/Admin/ItemAttribute.aspx?id={0}", mAttributeID));
        }
    }
}