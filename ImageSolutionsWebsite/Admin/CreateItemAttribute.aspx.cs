using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Admin
{
    public partial class CreateItemAttribute : BasePageAdminUserWebSiteAuth
    {
        private string mItemID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString.Get("itemid")))
            {
                mItemID = Request.QueryString.Get("itemid");
            }
            else
            {
                Response.Redirect(String.Format("/Admin/ItemOverview.aspx"));
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/Admin/Item.aspx?id={0}", mItemID));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                ImageSolutions.Attribute.Attribute Attribute = new ImageSolutions.Attribute.Attribute();
                Attribute.ItemID = Convert.ToString(mItemID);
                Attribute.AttributeName = txtName.Text;
                Attribute.Sort = Convert.ToInt32(txtSort.Text);
                Attribute.Create();

                Response.Redirect(String.Format("/Admin/Item.aspx?id={0}", mItemID));
            }
            catch (Exception ex)
            {
                WebUtility.DisplayJavascriptMessage(this, ex.Message);
            }
            finally { }
        }
    }
}