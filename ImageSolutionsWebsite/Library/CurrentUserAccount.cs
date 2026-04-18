using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite.Library
{
    public class CurrentUserAccount : ImageSolutions.User.UserAccount
    {
        public bool IsLoggedIn { get { return !string.IsNullOrEmpty(GUID); } }

        public CurrentUserAccount() : base()
        {
            if (HttpContext.Current.Request.Cookies["UserAccountGUID"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["UserAccountGUID"].Value))
            {
                try
                {
                    Load(HttpContext.Current.Request.Cookies["UserAccountGUID"].Value);
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
                HttpContext.Current.Response.Cookies["UserAccountGUID"].Value = GUID;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);
                throw ex;
            }
            return true;
        }

        public bool Logout()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["UserAccountGUID"] != null) HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}