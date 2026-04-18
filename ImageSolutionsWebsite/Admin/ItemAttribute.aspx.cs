using ImageSolutions.Attribute;
using ImageSolutions.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemAttribute : BasePageAdminUserWebSiteAuth
    {
        protected string mAttributeID = string.Empty;
        protected string mItemID = string.Empty;

        private ImageSolutions.Attribute.Attribute mAttribute = null;
        protected ImageSolutions.Attribute.Attribute _Attribute
        {
            get
            {
                if (mAttribute == null)
                {
                    if (string.IsNullOrEmpty(mAttributeID))
                        mAttribute = new ImageSolutions.Attribute.Attribute();
                    else
                        mAttribute = new ImageSolutions.Attribute.Attribute(mAttributeID);
                }
                return mAttribute;
            }
            set
            {
                mAttribute = value;
            }
        }

        protected string ReturnURL
        {
            get
            {
                if (ViewState["ReturnURL"] == null)
                    return "/Admin/Item.aspx?id=" + (_Attribute.IsNew ? mItemID : _Attribute.ItemID) + "&tab=2";
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
            mAttributeID = Request.QueryString.Get("id");
            mItemID = Request.QueryString.Get("itemid");

            if (!Page.IsPostBack)
            {
                //if (Request.UrlReferrer != null) ReturnURL = Request.UrlReferrer.PathAndQuery;

                InitializePage();
            }
        }
        public void InitializePage()
        {
            try
            {
                InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, null, null, null, null);

                if (!_Attribute.IsNew)
                {
                    txtAttribute.Text = _Attribute.AttributeName;
                    txtSort.Text = Convert.ToString(_Attribute.Sort);
                    btnSave.Text = "Save";

                    BindItemAttributeValue();
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                    top_2_tab.Visible = false;
                }
                aCancel.HRef = ReturnURL;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemAttributeValue()
        {
            try
            {
                gvAttributeValue.DataSource = _Attribute.AttributeValues.OrderBy(m => m.Sort).ThenBy(m => m.Value);
                gvAttributeValue.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                _Attribute.AttributeName = txtAttribute.Text;
                _Attribute.Sort = Convert.ToInt32(txtSort.Text);

                if (_Attribute.IsNew)
                {
                    _Attribute.ItemID = mItemID;
                    _Attribute.CreatedBy = CurrentUser.UserInfoID;
                    blnReturn = _Attribute.Create();
                    ReturnURL = "/admin/itemattribute.aspx?id=" + _Attribute.AttributeID;
                }
                else
                {
                    blnReturn = _Attribute.Update();
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
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = _Attribute.Delete();
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
            if (blnReturn)
            {
                Response.Redirect(ReturnURL);
            }
        }

        protected void gvAttributeValue_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    string strAttributeValueID = gvAttributeValue.DataKeys[e.Row.RowIndex].Value.ToString();

                    Button btnUp = (Button)e.Row.FindControl("btnUp");
                    Button btnDown = (Button)e.Row.FindControl("btnDown");

                    AttributeValue objAttributeValue = new AttributeValue(strAttributeValueID);

                    List<ImageSolutions.Attribute.AttributeValue> objAttributeValues = new List<AttributeValue>();
                    ImageSolutions.Attribute.AttributeValueFilter objAttributeValuesFilter = new AttributeValueFilter();
                    objAttributeValuesFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                    objAttributeValuesFilter.AttributeID.SearchString = mAttributeID;
                    objAttributeValues = ImageSolutions.Attribute.AttributeValue.GetAttributeValues(objAttributeValuesFilter);

                    objAttributeValue.Sort = e.Row.RowIndex + 1;
                    objAttributeValue.Update();

                    btnUp.Visible = Convert.ToInt32(objAttributeValue.Sort) != 1;
                    btnDown.Visible = objAttributeValues.Count != Convert.ToInt32(objAttributeValue.Sort);

                    TextBox txtSort = (TextBox)e.Row.FindControl("txtSort");
                    txtSort.Text = Convert.ToString(objAttributeValue.Sort);
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
        }

        protected void btnUpdateSort_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow _Row in gvAttributeValue.Rows)
            {
                string strAttributeValueID = gvAttributeValue.DataKeys[_Row.RowIndex].Value.ToString();

                TextBox txtSort = (TextBox)_Row.FindControl("txtSort");

                AttributeValue objAttributeValue = new AttributeValue(strAttributeValueID);
                objAttributeValue.Sort = Convert.ToInt32(txtSort.Text);
                objAttributeValue.Update();
            }

            BindItemAttributeValue();
        }

        protected void gvAttributeValue_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "MoveUp")
            {
                string strAttributeValueID = Convert.ToString(e.CommandArgument);
                AttributeValue AttributeValue = new AttributeValue(strAttributeValueID);

                AttributeValue UpAttributeValue = new AttributeValue();
                AttributeValueFilter AttributeValueFilter = new AttributeValueFilter();
                AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                AttributeValueFilter.AttributeID.SearchString = _Attribute.AttributeID;
                AttributeValueFilter.Sort = AttributeValue.Sort - 1;
                UpAttributeValue = AttributeValue.GetAttributeValue(AttributeValueFilter);

                UpAttributeValue.Sort = UpAttributeValue.Sort + 1;
                UpAttributeValue.Update();

                AttributeValue.Sort = AttributeValue.Sort - 1;
                AttributeValue.Update();

                BindItemAttributeValue();
            }
            else if (e.CommandName == "MoveDown")
            {
                string strAttributeValueID = Convert.ToString(e.CommandArgument);
                AttributeValue AttributeValue = new AttributeValue(strAttributeValueID);

                AttributeValue DownAttributeValue = new AttributeValue();
                AttributeValueFilter AttributeValueFilter = new AttributeValueFilter();
                AttributeValueFilter.AttributeID = new Database.Filter.StringSearch.SearchFilter();
                AttributeValueFilter.AttributeID.SearchString = _Attribute.AttributeID;
                AttributeValueFilter.Sort = AttributeValue.Sort + 1;
                DownAttributeValue = AttributeValue.GetAttributeValue(AttributeValueFilter);

                DownAttributeValue.Sort = DownAttributeValue.Sort - 1;
                DownAttributeValue.Update();

                AttributeValue.Sort = AttributeValue.Sort + 1;
                AttributeValue.Update();

                BindItemAttributeValue();
            }
        }
    }
}