using ImageSolutionsWebsite.Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ImageSolutionsWebsite
{
    public class CurrentUser : ImageSolutions.User.UserInfo
    {
        public bool IsLoggedIn { get { return !string.IsNullOrEmpty(GUID); } }

        private CurrentUserWebSite mCurrentUserWebSite = null;
        public CurrentUserWebSite CurrentUserWebSite
        {
            get
            {
                if (mCurrentUserWebSite == null)
                {
                    mCurrentUserWebSite = new CurrentUserWebSite();
                }
                return mCurrentUserWebSite;
            }
        }

        public CurrentUser() : base()
        {
            if (HttpContext.Current.Request.Cookies["UserGUID"] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Cookies["UserGUID"].Value))
            {
                try
                {
                    Load(HttpContext.Current.Request.Cookies["UserGUID"].Value);
                }
                catch
                {
                    Logout();
                }
            }
        }



        public static TimeSpan GetOffSetTime(DateTime datetime)
        {
            TimeZone zone = TimeZone.CurrentTimeZone;
            TimeSpan local = zone.GetUtcOffset(datetime);
            return local;
        }

        public new bool Login(string Email, string Password, bool ispunchout = false)
        {
            try
            {
                base.Login(Email, Password);

                if(ispunchout)
                {
                    if (InActive || !UserWebsites.Exists(x => !x.InActive && x.WebSite.IsPunchout))
                    {
                        throw new Exception("Invalid User");
                    }
                }
                else
                {
                    if (InActive || !UserWebsites.Exists(x => !x.InActive && !x.WebSite.IsPunchout))
                    {
                        throw new Exception("Invalid User");
                    }
                }

                if (HttpContext.Current.Request.Cookies["WebsiteMessage"] != null) HttpContext.Current.Response.Cookies["WebsiteMessage"].Expires = System.DateTime.Now.AddDays(-1);
                if (HttpContext.Current.Request.Cookies["UserWebSiteGUID"] != null) HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
                if (HttpContext.Current.Request.Cookies["UserAccountGUID"] != null) HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);

                HttpContext.Current.Response.Cookies["UserGUID"].Value = GUID;
                //DateTime ExpireDateTime = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 1);
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 30);

                //HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddMinutes(1);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddDays(-1);
                throw ex;
            }
            return true;
        }

        public bool LoginAsGuest(CurrentWebsite CurrentWebsite)
        {
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                if (CurrentWebsite == null) throw new Exception("CurrentWebsite is missing");
                if (CurrentWebsite.DefaultGuestAccount == null) throw new Exception("DefaultGuestAccount is required for guest checkout");
                if (CurrentWebsite.DefaultWebsiteGroup == null) throw new Exception("DefaultWebsiteGroup is required for guest checkout");

                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                //Password = Guid.NewGuid().ToString();
                //IsGuest = true;
                //base.Create(objConn, objTran);

                //base.LoginByUserInfoID(objConn, objTran);

                ImageSolutions.User.UserInfo objUserInfo = new ImageSolutions.User.UserInfo();
                objUserInfo.Password = Guid.NewGuid().ToString();
                objUserInfo.IsGuest = true;
                objUserInfo.Create(objConn, objTran);

                ImageSolutions.User.UserWebsite objUserWebsite = new ImageSolutions.User.UserWebsite();
                objUserWebsite.UserInfoID = objUserInfo.UserInfoID;
                objUserWebsite.WebsiteID = CurrentWebsite.WebsiteID;
                objUserWebsite.CreatedBy = CurrentWebsite.CreatedBy;
                objUserWebsite.PaymentTermID = CurrentWebsite.DefaultPaymentTermID;
                objUserWebsite.Create(objConn, objTran);

                ImageSolutions.User.UserAccount objUserAccount = new ImageSolutions.User.UserAccount();
                objUserAccount.UserWebsiteID = objUserWebsite.UserWebsiteID;
                objUserAccount.AccountID = CurrentWebsite.DefaultGuestAccountID;
                objUserAccount.WebsiteGroupID = CurrentWebsite.DefaultWebsiteGroupID;
                objUserAccount.CreatedBy = CurrentWebsite.CreatedBy;
                objUserAccount.Create(objConn, objTran);

                objTran.Commit();

                base.LoginByUserInfoID(objUserInfo.UserInfoID);

                HttpContext.Current.Response.Cookies["UserGUID"].Value = GUID;
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddMinutes(GetOffSetTime(System.DateTime.Now).Minutes + 30);
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddDays(-1);
                if (objTran != null && objTran.Connection != null)
                {
                    objTran.Rollback();
                }
                throw ex;
            }
            finally
            {
                if (objTran != null) objTran.Dispose();
                objTran = null;
                if (objConn != null) objConn.Dispose();
                objConn = null;
            }
            return true;
        }

        public bool LoginAs(string Email)
        {
            try
            {
                
                //if (!IsSuperAdmin) throw new Exception("You are not allowed to use this feature, please contact web administrator.");

                HttpContext.Current.Response.Cookies["UserAccountGUID"].Value = null;
                HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Value = null;
                HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
                base.Login(Email, String.Empty, true);
                HttpContext.Current.Response.Cookies["UserGUID"].Value = GUID;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddDays(-1);
                throw ex;
            }
            return true;
        }

        public new bool LoginPasscode(string Email, string Passcode)
        {
            try
            {
                base.LoginPasscode(Email, Passcode);

                if(InActive || !UserWebsites.Exists(x => !x.InActive))
                {
                    throw new Exception("Invalid User");
                }
                
                HttpContext.Current.Response.Cookies["UserGUID"].Value = GUID;
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddDays(-1);
                throw ex;
            }
            return true;
        }
        //public override bool Register(string CompanyName)
        //{
        //    try
        //    {
        //        base.Register(CompanyName);
        //        HttpContext.Current.Response.Cookies["UserGUID"].Value = BusinessUserGuid;
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddDays(-1);
        //        throw ex;
        //    }
        //    return true;
        //}

        public bool Logout()
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["UserGUID"] != null)
                {
                    HttpContext.Current.Response.Cookies["UserGUID"].Expires = System.DateTime.Now.AddDays(-1);
                }
                //if (HttpContext.Current.Request.Cookies["UserWebSiteGUID"] != null)
                //{
                //    HttpContext.Current.Response.Cookies["UserWebSiteGUID"].Expires = System.DateTime.Now.AddDays(-1);
                //}
                //if (HttpContext.Current.Request.Cookies["UserAccountGUID"] != null)
                //{
                //    HttpContext.Current.Response.Cookies["UserAccountGUID"].Expires = System.DateTime.Now.AddDays(-1);
                //}
                if (HttpContext.Current.Request.Cookies["WebsiteMessage"] != null)
                {
                    HttpContext.Current.Response.Cookies["WebsiteMessage"].Expires = System.DateTime.Now.AddDays(-1);
                }
                if (HttpContext.Current.Request.Cookies["OptIn"] != null)
                {
                    HttpContext.Current.Response.Cookies["OptIn"].Expires = System.DateTime.Now.AddDays(-1);
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