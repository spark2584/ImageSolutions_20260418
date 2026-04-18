using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateUser : BasePageAdminUserWebSiteAuth
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Admin/UserOverview.aspx");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                UserInfo.FirstName = txtFirstName.Text;
                UserInfo.LastName = txtLastName.Text;
                UserInfo.EmailAddress = txtEmail.Text;
                UserInfo.Password = txtPassword.Text;
                UserInfo.IsAdmin = cbIsAdmin.Checked;
                UserInfo.Create();

                //Response.Redirect(String.Format("/Admin/Website.aspx?id={0}", Website.WebsiteID));
                Response.Redirect("/Admin/UserOverview.aspx");
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}