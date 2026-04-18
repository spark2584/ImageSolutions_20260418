using ImageSolutions.User;
using ImageSolutionsWebsite.Admin;
using ImageSolutionsWebsite.Library;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;

namespace ImageSolutionsWebsite
{
    public class BasePage : System.Web.UI.Page
    {
        private CurrentUser mCurrentUser = null;
        public CurrentUser CurrentUser
        {
            get
            {
                if (mCurrentUser == null)
                {
                    mCurrentUser = new CurrentUser();
                }
                return mCurrentUser;
            }
        }

        private CurrentWebsite mCurrentWebsite = null;
        public CurrentWebsite CurrentWebsite
        {
            get
            {
                if (mCurrentWebsite == null)
                {
                    mCurrentWebsite = new CurrentWebsite();
                }
                return mCurrentWebsite;
            }
        }

        public bool HideBreadcrumb { get; set; }


        public static TimeSpan GetOffSetTime(DateTime datetime)
        {
            TimeZone zone = TimeZone.CurrentTimeZone;
            TimeSpan local = zone.GetUtcOffset(datetime);
            return local;
        }

        public bool UserAuth()
        {

            if (!CurrentUser.IsLoggedIn) Response.Redirect("/login.aspx?target=" + Server.UrlEncode(Request.Url.ToString()));

            else
            {
                HttpContext.Current.Response.Cookies["UserGUID"].Value = CurrentUser.GUID;
                //DateTime ExpireDateTime = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 1);

                if (CurrentWebsite != null && CurrentWebsite.WebsiteID == "25" || CurrentWebsite.WebsiteID == "26")
                {
                    HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 10);
                }
                else
                {
                    HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 30);
                }

                if (CurrentUser.IsSuperAdmin)
                {
                    if (HttpContext.Current.Request.Cookies["ISAdminUserGUID"] == null || string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value))
                    {
                        HttpContext.Current.Response.Cookies["ISAdminUserGUID"].Value = HttpContext.Current.Response.Cookies["UserGUID"].Value;
                    }
                }
            }

            return true;
        }
        public bool UserAdminAuth()
        {
            
            if (!CurrentUser.IsLoggedIn) Response.Redirect("/login.aspx?target=" + Server.UrlEncode(Request.Url.ToString()));
            else
            {            
                HttpContext.Current.Response.Cookies["UserGUID"].Value = CurrentUser.GUID;
                //DateTime ExpireDateTime = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 1);
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 30);

                if (CurrentUser.IsSuperAdmin)
                {
                    if (HttpContext.Current.Request.Cookies["ISAdminUserGUID"] == null || string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["ISAdminUserGUID"].Value))
                    {
                        HttpContext.Current.Response.Cookies["ISAdminUserGUID"].Value = HttpContext.Current.Response.Cookies["UserGUID"].Value;
                    }
                }
            }


            if (CurrentUser.UserWebsites == null || !CurrentUser.UserWebsites.Exists(m => m.IsAdmin)) Response.Redirect("/myaccount/dashboard.aspx?login=t");
            return true;
        }

        public bool UserNoAuth()
        {
            if (CurrentUser.IsLoggedIn) Response.Redirect("/myaccount/dashboard.aspx?login=t");
            return true;
        }

        public bool UserWebSiteAuth()
        {
            UserAuth();
            if (!CurrentUser.CurrentUserWebSite.IsLoggedIn) Response.Redirect("/userwebsite.aspx?target=" + Server.UrlEncode(Request.Url.ToString()));

            return true;
        }

        public bool UserAdminWebSiteAuth()
        {
            UserAuth();
            if (!CurrentUser.CurrentUserWebSite.CurrentUserAccount.IsLoggedIn) Response.Redirect("/UserAccount.aspx?target=" + Server.UrlEncode(Request.Url.ToString()));
            if (!CurrentUser.CurrentUserWebSite.IsLoggedIn) Response.Redirect("/admin/websiteoverview.aspx?target=" + Server.UrlEncode(Request.Url.ToString()));
            if (!CurrentUser.CurrentUserWebSite.IsAdmin) Response.Redirect("/admin/websiteoverview.aspx");
            return true;
        }

        public bool UserAccountAuth()
        {
            UserAuth();
            UserWebSiteAuth();
            if (!CurrentUser.CurrentUserWebSite.CurrentUserAccount.IsLoggedIn) Response.Redirect("/UserAccount.aspx?target=" + Server.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri));

            return true;
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1)
        {
            return InitializeTabs(top_1_tab, top_1, null, null, null, null, null, null, null, null);
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1, HtmlAnchor top_2_tab, HtmlGenericControl top_2)
        {
            return InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, null, null, null, null, null, null);
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1, HtmlAnchor top_2_tab, HtmlGenericControl top_2, HtmlAnchor top_3_tab, HtmlGenericControl top_3)
        {
            return InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, null, null, null, null);
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1, HtmlAnchor top_2_tab, HtmlGenericControl top_2, HtmlAnchor top_3_tab, HtmlGenericControl top_3, HtmlAnchor top_4_tab, HtmlGenericControl top_4)
        {
            return InitializeTabs(top_1_tab, top_1, top_2_tab, top_2, top_3_tab, top_3, top_4_tab, top_4, null, null);
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1, HtmlAnchor top_2_tab, HtmlGenericControl top_2, HtmlAnchor top_3_tab, HtmlGenericControl top_3, HtmlAnchor top_4_tab, HtmlGenericControl top_4, HtmlAnchor top_5_tab, HtmlGenericControl top_5)
        {
            switch (Request.QueryString.Get("tab"))
            {
                case "2":
                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link active");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "3":
                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link active");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "4":
                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link active");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "5":
                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link active");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                default:
                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link active");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
            }
            return true;
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1, HtmlAnchor top_2_tab, HtmlGenericControl top_2, HtmlAnchor top_3_tab, HtmlGenericControl top_3, HtmlAnchor top_4_tab, HtmlGenericControl top_4, HtmlAnchor top_5_tab, HtmlGenericControl top_5, HtmlAnchor top_6_tab, HtmlGenericControl top_6)
        {
            switch (Request.QueryString.Get("tab"))
            {
                case "2":
                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link active");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "3":
                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link active");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "4":
                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link active");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "5":
                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link active");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }
                    break;

                case "6":
                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link active");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                default:
                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link active");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
            }
            return true;
        }

        protected bool InitializeTabs(HtmlAnchor top_1_tab, HtmlGenericControl top_1, HtmlAnchor top_2_tab, HtmlGenericControl top_2, HtmlAnchor top_3_tab, HtmlGenericControl top_3, HtmlAnchor top_4_tab, HtmlGenericControl top_4, HtmlAnchor top_5_tab, HtmlGenericControl top_5, HtmlAnchor top_6_tab, HtmlGenericControl top_6, HtmlAnchor top_7_tab, HtmlGenericControl top_7)
        {
            switch (Request.QueryString.Get("tab"))
            {
                case "2":
                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link active");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "3":
                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link active");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "4":
                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link active");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "5":
                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link active");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                case "6":
                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link active");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade");
                    }
                    break;

                case "7":
                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link active");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
                default:
                    if (top_1_tab != null)
                    {
                        top_1_tab.Attributes.Remove("class");
                        top_1_tab.Attributes.Add("class", "nav-link active");
                        top_1.Attributes.Remove("class");
                        top_1.Attributes.Add("class", "tab-pane fade show active");
                    }

                    if (top_2_tab != null)
                    {
                        top_2_tab.Attributes.Remove("class");
                        top_2_tab.Attributes.Add("class", "nav-link");
                        top_2.Attributes.Remove("class");
                        top_2.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_3_tab != null)
                    {
                        top_3_tab.Attributes.Remove("class");
                        top_3_tab.Attributes.Add("class", "nav-link");
                        top_3.Attributes.Remove("class");
                        top_3.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_4_tab != null)
                    {
                        top_4_tab.Attributes.Remove("class");
                        top_4_tab.Attributes.Add("class", "nav-link");
                        top_4.Attributes.Remove("class");
                        top_4.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_5_tab != null)
                    {
                        top_5_tab.Attributes.Remove("class");
                        top_5_tab.Attributes.Add("class", "nav-link");
                        top_5.Attributes.Remove("class");
                        top_5.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_6_tab != null)
                    {
                        top_6_tab.Attributes.Remove("class");
                        top_6_tab.Attributes.Add("class", "nav-link");
                        top_6.Attributes.Remove("class");
                        top_6.Attributes.Add("class", "tab-pane fade");
                    }

                    if (top_7_tab != null)
                    {
                        top_7_tab.Attributes.Remove("class");
                        top_7_tab.Attributes.Add("class", "nav-link");
                        top_7.Attributes.Remove("class");
                        top_7.Attributes.Add("class", "tab-pane fade");
                    }
                    break;
            }
            return true;
        }
        public bool SortAscending(string SortExpression)
        {
            if (!string.IsNullOrEmpty(SortExpression) && ViewState["SortExpression"] != null && ViewState["SortAscending"] != null && ViewState["SortExpression"].ToString() == SortExpression)
            {
                if (Convert.ToBoolean(ViewState["SortAscending"]) == true)
                {
                    ViewState["SortAscending"] = false;
                }
                else
                {
                    ViewState["SortAscending"] = true;
                }
            }
            else
            {
                ViewState["SortExpression"] = SortExpression;
                ViewState["SortAscending"] = true;
            }
            return Convert.ToBoolean(ViewState["SortAscending"]);
        }
        public void SendEmail(string toemail, string subject, string htmlcontent, List<string>ccs = null)
        {
            UserInfoFilter objFilter = null;
            UserInfo objUserInfo = null;

            try
            {
                //if ( !(CurrentWebsite.WebsiteID == "42" && toemail.ToLower().Contains("@imageinc.com")) )//Test email from staging
                //{
                    //If the environment is sandbox, default it to steve@imageinc.com or the web.config application variable unless the user email address is whitelisted
                    if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                        || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) != "production"
                    )
                    {
                        toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);

                        objFilter = new UserInfoFilter();
                        objFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                        objFilter.EmailAddress.SearchString = toemail;
                        objUserInfo = UserInfo.GetUserInfo(objFilter);
                        if (objUserInfo != null)
                        {
                            if (objUserInfo.EmailWhiteListed) toemail = objUserInfo.EmailAddress;
                        }
                    }
                //}


                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(toemail);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, subject, null, htmlcontent);

                if (ccs != null)
                {
                    foreach (string _cc in ccs)
                        SendGridMessage.AddCc(_cc);
                }

                Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objFilter = null;
                objUserInfo = null;
            }
        }
    }
}