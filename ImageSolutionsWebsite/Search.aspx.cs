using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite
{
    public partial class Search : BasePageUserAccountAuth
    {
        protected string mSearchText = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            mSearchText = Request.QueryString.Get("txt");

            if (!string.IsNullOrEmpty(mSearchText))
            {
                this.txtSearchText.Text = mSearchText;
                BindSearchResult();
            }
        }
        
        protected void BindSearchResult()
        {

        }
    }
}