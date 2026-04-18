using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class LeftPanelNavigation : BaseControl
    {
        protected string mWebSiteTabID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            mWebSiteTabID = Request.QueryString.Get("WebSiteTabID");
            if (!Page.IsPostBack)
            {
                BindWebsiteTab();
            }
        }

        protected void BindWebsiteTab()
        {
            List<ImageSolutions.Website.WebsiteTab> objWebSiteTabs = null;

            try
            {
                if (ThisPage.CurrentUser.IsLoggedIn && ThisPage.CurrentUser.CurrentUserWebSite != null && ThisPage.CurrentUser.CurrentUserWebSite.WebSite != null && ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount != null && ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroup != null)
                {
                    objWebSiteTabs = new List<ImageSolutions.Website.WebsiteTab>();

                    if (ThisPage.CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs != null)
                    {
                        objWebSiteTabs.AddRange(ThisPage.CurrentUser.CurrentUserWebSite.WebSite.WebsiteTabs.FindAll(m => m.AllowAllGroups == true));
                    }

                    if (ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroup.WebsiteGroupTabs != null)
                    {
                        if (ThisPage.CurrentWebsite.CombineWebsiteGroup)
                        {
                            foreach (ImageSolutions.User.UserAccount _UserAccount in ThisPage.CurrentUser.CurrentUserWebSite.UserAccounts)
                            {
                                foreach (ImageSolutions.Website.WebsiteGroupTab objWebSiteGroupTab in _UserAccount.WebsiteGroup.WebsiteGroupTabs)
                                {
                                    if (!objWebSiteTabs.Exists(m => m.WebsiteTabID == objWebSiteGroupTab.WebsiteTab.WebsiteTabID))
                                    {
                                        objWebSiteTabs.Add(objWebSiteGroupTab.WebsiteTab);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (ImageSolutions.Website.WebsiteGroupTab objWebSiteGroupTab in ThisPage.CurrentUser.CurrentUserWebSite.CurrentUserAccount.WebsiteGroup.WebsiteGroupTabs)
                            {
                                if (!objWebSiteTabs.Exists(m => m.WebsiteTabID == objWebSiteGroupTab.WebsiteTab.WebsiteTabID))
                                {
                                    objWebSiteTabs.Add(objWebSiteGroupTab.WebsiteTab);
                                }
                            }
                        }
                    }

                    if (ThisPage.CurrentUser.CurrentUserWebSite.UserWebsiteTabs != null)
                    {
                        foreach (ImageSolutions.User.UserWebsiteTab _UserWebsiteTab in ThisPage.CurrentUser.CurrentUserWebSite.UserWebsiteTabs)
                        {
                            if (!objWebSiteTabs.Exists(m => m.WebsiteTabID == _UserWebsiteTab.WebsiteTab.WebsiteTabID))
                            {
                                objWebSiteTabs.Add(_UserWebsiteTab.WebsiteTab);
                            }
                        }
                    }

                    objWebSiteTabs = objWebSiteTabs.OrderBy(x => x.Sort == null ? 9999 : x.Sort).ToList();

                    string strHTML = string.Empty;
                    foreach (ImageSolutions.Website.WebsiteTab objWebsiteTab in objWebSiteTabs.FindAll(m => string.IsNullOrEmpty(m.ParentID)))
                    {
                        strHTML += GetSubTabHTML(objWebsiteTab, objWebSiteTabs, 1, WebsiteTabExists(objWebsiteTab.WebsiteTabID));
                    }
                    this.litWebsiteTab.Text = strHTML;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWebSiteTabs = null;
            }
        }

        protected string GetSubTabHTML(ImageSolutions.Website.WebsiteTab websitetab, List<ImageSolutions.Website.WebsiteTab> websitetabs, int level, bool expand)
        {
            string strDisplayName = string.IsNullOrEmpty(Convert.ToString(websitetab.DisplayName).Trim()) ? websitetab.TabName : websitetab.DisplayName;

            string strReturn = string.Empty;

            if (ThisPage.CurrentUser.CurrentUserWebSite.WebSite.ItemDisplayType == "List")
            {
                strReturn += String.Format(@"<div class='form-check collection-filter-checkbox'><a href='/itemlist.aspx?WebsiteTabID=" + websitetab.WebsiteTabID + "'>&#183; " + strDisplayName + "</a></div>"); //"<li><a href='/itemlist.aspx?WebsiteTabID=" + websitetab.WebsiteTabID + "'>" + websitetab.TabName + "</a>";
            }
            else
            {
                if (
                    (websitetab.WebsiteTabItems != null && websitetab.WebsiteTabItems.FindAll(m => !m.Item.InActive && m.Item.IsOnline).Count > 0)
                    ||
                    (websitetab.ChildWebsiteTabs != null && websitetab.ChildWebsiteTabs.Count > 0)
                )
                {
                    if(websitetab.WebsiteTabID != mWebSiteTabID)
                    {
                        strReturn += String.Format(@"<div class='form-check collection-filter-checkbox' style='margin-left:" + Convert.ToString((level - 1) * 20) + "px; '><a href='/items.aspx?WebsiteTabID=" + websitetab.WebsiteTabID + "'><label class='form-check-label'>&#183; " + strDisplayName + "</label></a></div>"); //"<li><a href='/itemlist.aspx?WebsiteTabID=" + websitetab.WebsiteTabID + "'>" + websitetab.TabName + "</a>";
                    }
                    else
                    {
                        strReturn += String.Format(@"<div class='form-check collection-filter-checkbox' style='margin-left:" + Convert.ToString((level - 1) * 20) + "px; '><a href='/items.aspx?WebsiteTabID=" + websitetab.WebsiteTabID + "'><label class='form-check-label' style='color: red;'>&#183; " + strDisplayName + "</label></a></div>"); //"<li><a href='/itemlist.aspx?WebsiteTabID=" + websitetab.WebsiteTabID + "'>" + websitetab.TabName + "</a>";
                    }
                }
            }

            if (expand && websitetabs.Exists(m => m.ParentID == websitetab.WebsiteTabID)
            )
            {
                level = level + 1;

                foreach (ImageSolutions.Website.WebsiteTab objWebsiteTab in websitetabs.FindAll(m => m.ParentID == websitetab.WebsiteTabID))
                {
                    strReturn += GetSubTabHTML(objWebsiteTab, websitetabs, level, WebsiteTabExists(objWebsiteTab.WebsiteTabID));
                }
            }

            return strReturn;
        }

        protected bool WebsiteTabExists(string websitetabid)
        {
            bool blnReturn = false;

            if(websitetabid == mWebSiteTabID)
            {
                blnReturn = true;
            }
            else
            {
                ImageSolutions.Website.WebsiteTab WebsiteTab = new ImageSolutions.Website.WebsiteTab(websitetabid);

                foreach (ImageSolutions.Website.WebsiteTab _WebsiteTab in WebsiteTab.ChildWebsiteTabs)
                {
                    if(!blnReturn)
                    {
                        blnReturn = WebsiteTabExists(_WebsiteTab.WebsiteTabID);
                    }
                }
            }

            return blnReturn;
        }
    }
}