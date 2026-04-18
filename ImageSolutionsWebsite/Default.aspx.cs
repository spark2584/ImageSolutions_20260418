using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ImageSolutions;

namespace ImageSolutionsWebsite
{
    public partial class Default : BasePage
    {
        //https://jqueryui.com/droppable/
        protected void Page_Load(object sender, EventArgs e)
        {
            HideBreadcrumb = true;
            if (CurrentWebsite != null && !string.IsNullOrEmpty(CurrentWebsite.StartingPath))
            {
                Response.Redirect(CurrentWebsite.StartingPath);
            }
            else
            {
                string strDomainName = HttpContext.Current.Request.Url.Host;

                ImageSolutions.Website.Website Website = new ImageSolutions.Website.Website();
                ImageSolutions.Website.WebsiteFilter WebsiteFilter = new ImageSolutions.Website.WebsiteFilter();
                WebsiteFilter.Domain = new Database.Filter.StringSearch.SearchFilter();
                WebsiteFilter.Domain.SearchString = strDomainName;
                Website = ImageSolutions.Website.Website.GetWebsite(WebsiteFilter);

                if (Website != null && Website.UseDomain)
                {
                    Response.Redirect(String.Format("website.aspx?website={0}", Website.GUID));
                }
                else
                {
                    Response.Redirect("/myaccount/dashboard.aspx?login=t");
                }
            }
        }
    }
}