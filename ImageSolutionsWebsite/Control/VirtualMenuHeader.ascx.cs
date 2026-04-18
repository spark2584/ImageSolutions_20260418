using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ImageSolutionsWebsite.Control
{
    public partial class VirtualMenuHeader : BaseControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                        strHTML += GetSubTabHTML(objWebsiteTab, objWebSiteTabs);
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

        protected string GetSubTabHTML(ImageSolutions.Website.WebsiteTab WebsiteTab, List<ImageSolutions.Website.WebsiteTab> WebsiteTabs)
        {
            string strReturn = string.Empty;

            string strDisplayName = string.IsNullOrEmpty(Convert.ToString(WebsiteTab.DisplayName).Trim()) ? WebsiteTab.TabName : WebsiteTab.DisplayName;

            if (ThisPage.CurrentUser.CurrentUserWebSite.WebSite.ItemDisplayType == "List")
            {
                if (
                    !ExistsWebsiteTabItem(WebsiteTab.WebsiteTabID)
                    //(WebsiteTab.WebsiteTabItems == null || WebsiteTab.WebsiteTabItems.FindAll(m => !m.Item.InActive && m.Item.IsOnline).Count == 0)
                    &&
                    (!ThisPage.CurrentWebsite.DisplaySubCategory || WebsiteTab.ChildWebsiteTabs == null || WebsiteTab.ChildWebsiteTabs.Count == 0)
                )
                {
                    if (WebsiteTab.ChildWebsiteTabs == null || WebsiteTab.ChildWebsiteTabs.Count == 0)
                        strReturn += "<li>";
                    else
                    {
                        if (WebsiteTab.ChildWebsiteTabs.FindAll(x => x.AllowAllGroups || WebsiteTabs.Exists(y => y.WebsiteTabID == x.WebsiteTabID)).Count == 1)
                        {
                            ImageSolutions.Website.WebsiteTab ChildWebsiteTab = WebsiteTab.ChildWebsiteTabs.FindAll(x => x.AllowAllGroups || WebsiteTabs.Exists(y => y.WebsiteTabID == x.WebsiteTabID)).First();
                            
                            if (
                                (ChildWebsiteTab.ChildWebsiteTabs == null || ChildWebsiteTab.ChildWebsiteTabs.FindAll(x => x.AllowAllGroups || WebsiteTabs.Exists(y => y.WebsiteTabID == x.WebsiteTabID)).Count == 0)
                                && 
                                ExistsWebsiteTabItem(ChildWebsiteTab.WebsiteTabID)
                            )
                            //if (ChildWebsiteTab.ChildWebsiteTabs.Count == 0 && ExistsWebsiteTabItem(ChildWebsiteTab.WebsiteTabID))
                            {
                                strReturn += "<li><a href='/itemlist.aspx?WebsiteTabID=" + ChildWebsiteTab.WebsiteTabID + "'>" + strDisplayName + "</a>";
                            }
                            else
                            {
                                strReturn += "<li><a href='#'>" + strDisplayName + "</a>";
                            }
                        }
                        else
                        {
                            strReturn += "<li><a href='#'>" + strDisplayName + "</a>";
                        }
                    }
                }
                else
                {
                    strReturn += "<li><a href='/itemlist.aspx?WebsiteTabID=" + WebsiteTab.WebsiteTabID + "'>" + strDisplayName + "</a>";
                }
            }
            else
            {
                if (
                    !ExistsWebsiteTabItem(WebsiteTab.WebsiteTabID)
                    //(WebsiteTab.WebsiteTabItems == null || WebsiteTab.WebsiteTabItems.FindAll(m => !m.Item.InActive && m.Item.IsOnline).Count == 0)
                    &&
                    (!ThisPage.CurrentWebsite.DisplaySubCategory || WebsiteTab.ChildWebsiteTabs == null || WebsiteTab.ChildWebsiteTabs.Count == 0)
                )
                {
                    if (WebsiteTab.ChildWebsiteTabs == null || WebsiteTab.ChildWebsiteTabs.Count == 0)
                        strReturn += "<li>";
                    else
                    {
                        strReturn += "<li><a href='#'>" + strDisplayName + "</a>";
                    }
                }
                else
                {
                    if(WebsiteTab.ParentWebsiteTab != null && WebsiteTab.ParentWebsiteTab.WebsiteTabID == "1365")
                    {
                        string strTabName = WebsiteTab.TabName.Replace("Employee Purchase", "<br/>Employee Purchase");


                        strReturn += "<li><a href='/items.aspx?WebsiteTabID=" + WebsiteTab.WebsiteTabID + "' style='padding: 5px!important; font-size:11px;'>" + strTabName + "</a>";
                    }
                    else
                    {
                        strReturn += "<li><a href='/items.aspx?WebsiteTabID=" + WebsiteTab.WebsiteTabID + "'>" + strDisplayName + "</a>";
                    }
                }
            }

            if (WebsiteTabs.Exists(m => m.ParentID == WebsiteTab.WebsiteTabID))
            {
                if(WebsiteTab.WebsiteTabID == "1365")
                {
                    strReturn += "<ul>";
                }
                else
                {
                    strReturn += "<ul>";
                }

                foreach (ImageSolutions.Website.WebsiteTab objWebsiteTab in WebsiteTabs.FindAll(m => m.ParentID == WebsiteTab.WebsiteTabID))
                {
                    strReturn += GetSubTabHTML(objWebsiteTab, WebsiteTabs);
                }
                strReturn += "</ul>";
            }
            strReturn += "</li>";

            return strReturn;
        }

        protected bool ExistsWebsiteTabItem(string websitetabid)
        {
            bool blnReturn = false;
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT WebsiteTabItemID 
FROM WebsiteTabItem (NOLOCK) wi
Inner Join Item (NOLOCK) i on i.ItemID = wi.ItemID
WHERE i.InActive = 0
and i.IsOnline = 1
and WebsiteTabID = {0}
"
                    , Database.HandleQuote(websitetabid));

                objRead = Database.GetDataReader(strSQL);
                if (objRead.Read())
                {
                    blnReturn = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return blnReturn;
        }
    }
}