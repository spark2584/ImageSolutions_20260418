using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class Customization : BasePageAdminUserWebSiteAuth
    {
        private string mCustomizationID = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("id")))
            {
                mCustomizationID = Request.QueryString.Get("id");
            }

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(mCustomizationID))
                {
                    GetCustomizationData();
                }
            }
        }

        public void GetCustomizationData()
        {
            try
            {
                ImageSolutions.Customization.Customization Customization = new ImageSolutions.Customization.Customization(Convert.ToString(mCustomizationID));
                txtName.Text = Customization.CustomizationName;

                lblHeader.Text = Customization.CustomizationName;

                BindCustomizationField();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void BindCustomizationField()
        {
            try
            {
                List<ImageSolutions.Customization.CustomizationField> CustomizationFields = new List<ImageSolutions.Customization.CustomizationField>();
                ImageSolutions.Customization.CustomizationFieldFilter CustomizationFieldFilter = new ImageSolutions.Customization.CustomizationFieldFilter();
                CustomizationFieldFilter.CustomizationID = new Database.Filter.StringSearch.SearchFilter();
                CustomizationFieldFilter.CustomizationID.SearchString = mCustomizationID;
                CustomizationFields = ImageSolutions.Customization.CustomizationField.GetCustomizationFields(CustomizationFieldFilter);

                gvCustomizationField.DataSource = CustomizationFields;
                gvCustomizationField.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
                //Response.Redirect(String.Format("/Admin/CreateGroup.aspx?websiteid={0}", ddlWebsite.SelectedValue));
            }
            finally { }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("/Admin/CustomizationOverview.aspx"));
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.Customization Customization = new ImageSolutions.Customization.Customization(mCustomizationID);
                Customization.Delete();

                Response.Redirect(String.Format("/Admin/CustomizationOverview.aspx"));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }

        protected void btnAddField_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/CreateCustomizationField.aspx?customizationid={0}", mCustomizationID));
        }

        protected void gvCustomizationField_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "LineUpdate")
            {
                string strCustomizationFieldID = Convert.ToString(e.CommandArgument);
                Response.Redirect(String.Format("/Admin/CustomizationField.aspx?id={0}", strCustomizationFieldID));
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Customization.Customization Customization = new ImageSolutions.Customization.Customization(mCustomizationID);
                Customization.CustomizationName = Convert.ToString(txtName.Text);
                Customization.Update();

                WebUtility.DisplayJavascriptMessage(this, "Update completed");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}