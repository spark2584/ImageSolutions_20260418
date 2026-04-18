using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public delegate void SendMessageToThePageHandler(string message);
    public partial class UserWebsiteSearchModal : BaseControl
    {
        public string WebsiteID
        {
            get { return hfWebsiteID.Value; }
            set { hfWebsiteID.Value = value; }
        }

        public string UserWebsiteID
        {
            get { return hfUserWebsiteID.Value; }
            set { hfUserWebsiteID.Value = value; }
        }

        public event SendMessageToThePageHandler SendMessageToThePage;
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                this.Visible = false;

                if (ThisPage.CurrentWebsite != null && ThisPage.CurrentWebsite.HideEmail)
                {
                    lblEmail.Text = "Employee ID:";
                    txtEmail.Attributes.Add("placeholder", "employee id" );
                }
            }
        }
        public bool Show()
        {
            try
            {
                WebUtility.ClearForm(this);

                this.Visible = true;
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }

            return true;
        }

        protected void BindUserWebsites()
        {
            List<ImageSolutions.User.UserWebsite> objUserWebsite = null;
            ImageSolutions.User.UserWebsiteFilter objFilter = null;

            try
            {
                objFilter = new ImageSolutions.User.UserWebsiteFilter();

                objFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                objFilter.WebsiteID.SearchString = hfWebsiteID.Value;

                if (!string.IsNullOrEmpty(this.txtFirstName.Text.Trim()))
                {
                    objFilter.FirstName = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.FirstName.SearchString = this.txtFirstName.Text.Trim();
                }

                if (!string.IsNullOrEmpty(this.txtLastName.Text.Trim()))
                {
                    objFilter.LastName = new Database.Filter.StringSearch.SearchFilter();
                    objFilter.LastName.SearchString = this.txtLastName.Text.Trim();
                }


                if (!string.IsNullOrEmpty(this.txtEmail.Text.Trim()))
                {
                    if (ThisPage.CurrentWebsite != null && ThisPage.CurrentWebsite.HideEmail)
                    {
                        objFilter.EmployeeID = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.EmployeeID.SearchString = this.txtEmail.Text.Trim();
                    }
                    else
                    {
                        objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.EmailAddress.SearchString = this.txtEmail.Text.Trim();
                    }
                }

                objUserWebsite = ImageSolutions.User.UserWebsite.GetUserWebsites(objFilter);

                gvUserWebsites.DataSource = objUserWebsite;
                gvUserWebsites.DataBind();
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this.Page, ex.Message);
            }
            finally { }
        }

        protected void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            BindUserWebsites();
        }

        protected void txtLastName_TextChanged(object sender, EventArgs e)
        {
            BindUserWebsites();
        }

        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            BindUserWebsites();
        }

        protected void gvUserWebsites_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectLine")
            {
                string strCustomizationID = Convert.ToString(e.CommandArgument);
                SendMessageToThePage(strCustomizationID);

                gvUserWebsites.DataSource = null;
                gvUserWebsites.DataBind();

                this.Visible = false;
            }
        }

        public void Hide()
        {
            this.Visible = false;
        }

        protected void gvUserWebsites_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Hide Email if website is set to hide email and show employee id
            if (ThisPage.CurrentWebsite != null && ThisPage.CurrentWebsite.HideEmail)
            {
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = true;
            }
            else
            {
                e.Row.Cells[2].Visible = true;
                e.Row.Cells[3].Visible = false;
            }
        }
    }
}