using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite.Library
{
    public class CurrentUserWebSite : ImageSolutions.User.UserWebsite
    {
        public bool IsLoggedIn { get { return !string.IsNullOrEmpty(GUID); } }

        private CurrentUserAccount mCurrentUserAccount = null;
        public CurrentUserAccount CurrentUserAccount
        {
            get
            {
                if (mCurrentUserAccount == null)
                {
                    mCurrentUserAccount = new CurrentUserAccount();
                }
                return mCurrentUserAccount;
            }
        }

        public CurrentUserWebSite() : base()
        {
            if (HttpContext.Current.Request.Cookies["UserWebSiteGUID"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["UserWebSiteGUID"].Value))
            {
                try
                {
                    Load(HttpContext.Current.Request.Cookies["UserWebSiteGUID"].Value);
                }
                catch
                {
                    Logout();
                }
            }
        }

        public bool Login(string GUID)
        {
            try
            {
                Load(GUID);
                HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Value = GUID;
                if (HttpContext.Current.Request.Cookies["UserAccountGUID"] != null) HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
                throw ex;
            }
            return true;
        }

        public bool Logout()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["UserWebSiteGUID"] != null) HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}