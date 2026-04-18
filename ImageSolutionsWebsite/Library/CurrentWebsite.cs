using ImageSolutionsWebsite.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class CurrentWebsite : ImageSolutions.Website.Website
    {
        public bool IsLoggedIn { get { return !string.IsNullOrEmpty(GUID); } }

        //private CurrentWebsite mCurrentWebsite = null;
        //public CurrentWebsite CurrentWebsite
        //{
        //    get
        //    {
        //        if (mCurrentUserWebSite == null)
        //        {
        //            mCurrentUserWebSite = new CurrentUserWebSite();
        //        }
        //        return mCurrentUserWebSite;
        //    }
        //}

        public CurrentWebsite() : base()
        {
            if (HttpContext.Current.Request.Cookies["WebsiteGUID"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["WebsiteGUID"].Value))
            {
                try
                {
                    Load(HttpContext.Current.Request.Cookies["WebsiteGUID"].Value);
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
                HttpContext.Current.Response.Cookies["WebsiteGUID"].Value = GUID;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["WebsiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
                throw ex;
            }
            return true;
        }

        public bool Logout()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["WebsiteGUID"] != null)
                {
                    HttpContext.Current.Response.Cookies["WebsiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}