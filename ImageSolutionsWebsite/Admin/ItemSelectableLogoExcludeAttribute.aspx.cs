using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class ItemSelectableLogoExcludeAttribute : BasePageAdminUserWebSiteAuth
    {
        private string mItemSelectableLogoExcludeAttributeID = string.Empty;
        private string mItemSelectableLogoID = string.Empty;

        private ImageSolutions.Item.ItemSelectableLogoExcludeAttribute mItemSelectableLogoExcludeAttribute = null;
        protected ImageSolutions.Item.ItemSelectableLogoExcludeAttribute _ItemSelectableLogoExcludeAttribute
        {
            get
            {
                if (mItemSelectableLogoExcludeAttribute == null)
                {
                    if (string.IsNullOrEmpty(mItemSelectableLogoExcludeAttributeID))
                        mItemSelectableLogoExcludeAttribute = new ImageSolutions.Item.ItemSelectableLogoExcludeAttribute();
                    else
                        mItemSelectableLogoExcludeAttribute = new ImageSolutions.Item.ItemSelectableLogoExcludeAttribute(mItemSelectableLogoExcludeAttributeID);
                }
                return mItemSelectableLogoExcludeAttribute;
            }
            set
            {
                mItemSelectableLogoExcludeAttribute = value;
            }
        }

        private ImageSolutions.Item.ItemSelectableLogo mItemSelectableLogo = null;
        protected ImageSolutions.Item.ItemSelectableLogo _ItemSelectableLogo
        {
            get
            {
                if (mItemSelectableLogo == null)
                {
                    if (string.IsNullOrEmpty(mItemSelectableLogoID))
                        mItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo();
                    else
                        mItemSelectableLogo = new ImageSolutions.Item.ItemSelectableLogo(mItemSelectableLogoID);
                }
                return mItemSelectableLogo;
            }
            set
            {
                mItemSelectableLogo = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            mItemSelectableLogoExcludeAttributeID = Request.QueryString.Get("id");
            mItemSelectableLogoID = Request.QueryString.Get("itemselectablelogoid");

            if (!Page.IsPostBack)
            {
                InitializePage();
            }
        }

        public void InitializePage()
        {
            try
            {
                BindItemAttribute();

                if (!_ItemSelectableLogoExcludeAttribute.IsNew)
                {
                    ddlAttributeValue.SelectedValue = _ItemSelectableLogoExcludeAttribute.AttributeValueID;
                    btnSave.Text = "Save";
                }
                else
                {
                    btnSave.Text = "Create";
                    btnDelete.Visible = false;
                }

                if (!string.IsNullOrEmpty(mItemSelectableLogoID))
                {
                    btnCancel.NavigateUrl = String.Format(@"/Admin/ItemSelectableLogo.aspx?id={0}&itemid={1}", mItemSelectableLogoID, _ItemSelectableLogo.ItemID);
                }
                else
                {
                    btnCancel.NavigateUrl = String.Format(@"/Admin/ItemOverview.aspx");
                }

            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindItemAttribute()
        {
            List<ImageSolutions.Attribute.AttributeValue> AttributeValues = null;
            ImageSolutions.Attribute.AttributeValueFilter AttributeValueFilter = null;

            try
            {
                if (!string.IsNullOrEmpty(_ItemSelectableLogo.ItemID))
                {

                    ImageSolutions.Item.Item ParentItem = new ImageSolutions.Item.Item(_ItemSelectableLogo.ItemID);

                    if (ParentItem.Attributes != null && ParentItem.Attributes.Count > 0)
                    {
                        AttributeValues = new List<ImageSolutions.Attribute.AttributeValue>();

                        foreach (ImageSolutions.Attribute.Attribute _Attribute in ParentItem.Attributes)
                        {
                            if(_Attribute.AttributeValues != null && _Attribute.AttributeValues.Count > 0)
                            {
                                foreach (ImageSolutions.Attribute.AttributeValue _AttributeValue in _Attribute.AttributeValues)
                                {
                                    AttributeValues.Add(_AttributeValue);
                                }
                            }
                        }
                    }

                    ddlAttributeValue.DataSource = AttributeValues.OrderBy(x => x.Sort);
                    ddlAttributeValue.DataBind();
                }
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
                _ItemSelectableLogoExcludeAttribute.AttributeValueID = ddlAttributeValue.SelectedValue;

                if (_ItemSelectableLogoExcludeAttribute.IsNew)
                {
                    _ItemSelectableLogoExcludeAttribute.ItemSelectableLogoID = mItemSelectableLogoID;
                    _ItemSelectableLogoExcludeAttribute.CreatedBy = CurrentUser.UserInfoID;

                    blnReturn = _ItemSelectableLogoExcludeAttribute.Create();
                }
                else
                {
                    blnReturn = _ItemSelectableLogoExcludeAttribute.Update();
                }
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }


            if (blnReturn)
            {
                if(!string.IsNullOrEmpty(mItemSelectableLogoID))
                {
                    Response.Redirect(String.Format(@"/Admin/ItemSelectableLogo.aspx?id={0}&itemid={1}", mItemSelectableLogoID, _ItemSelectableLogo.ItemID));
                }
                else
                {
                    Response.Redirect(String.Format(@"/Admin/ItemOverview.aspx"));
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            bool blnReturn = false;

            try
            {
                blnReturn = _ItemSelectableLogoExcludeAttribute.Delete();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }

            if (blnReturn)
            {
                if (!string.IsNullOrEmpty(mItemSelectableLogoID))
                {
                    Response.Redirect(String.Format(@"/Admin/ItemSelectableLogo.aspx?id={0}&itemid={1}", mItemSelectableLogoID, _ItemSelectableLogo.ItemID));
                }
                else
                {
                    Response.Redirect(String.Format(@"/Admin/ItemOverview.aspx"));
                }
            }
        }
    }
}