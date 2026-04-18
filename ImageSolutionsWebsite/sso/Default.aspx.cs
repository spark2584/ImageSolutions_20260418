using System;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using Newtonsoft.Json;
using Sustainsys.Saml2.WebSso;

namespace ImageSolutionsWebsite.sso
{
    public partial class Default : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strAuthenticationResult = string.Empty;
            
            //if (Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() == "staging")
            //{
            //    var identity = HttpContext.Current?.User?.Identity;
            //    var identity2 = Context.User?.Identity;

            //    ImageSolutions.SSO.SSOLog SSOLog = new ImageSolutions.SSO.SSOLog();
            //    SSOLog.AuthenticateResult = String.Format("SSO Connected - {0} - {1}", identity.Name, identity2.Name);
            //    SSOLog.Create();
            //}

            if (Context.User.Identity.IsAuthenticated)
            {
                ClaimsIdentity identity = User.Identity as ClaimsIdentity;

                ImageSolutions.SSO.SSOLog SSOLog = new ImageSolutions.SSO.SSOLog();

                if (identity != null)
                {
                    try
                    {
                        string strClaims = string.Empty;
                        if(identity.Claims != null)
                        {
                            foreach (Claim _Claim in identity.Claims)
                            {
                                if (string.IsNullOrEmpty(strClaims))
                                {
                                    strClaims = string.Format("{0} ({1}: {2})", _Claim.ToString(), Convert.ToString(_Claim.Type), Convert.ToString(_Claim.Value));
                                }
                                else
                                {
                                    strClaims = string.Format(@"{0} | {1} ({2}: {3})", strClaims, _Claim.ToString(), Convert.ToString(_Claim.Type), Convert.ToString(_Claim.Value));
                                }
                            }

                        }


                        //strAuthenticationResult = JsonConvert.SerializeObject(identity);
                        //SSOLog.AuthenticateResult = strAuthenticationResult;

                        SSOLog.AuthenticateResult = strClaims;
                        SSOLog.Create();

                        string userName = identity.FindFirst(ClaimTypes.Name)?.Value;
                        string userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;
                        string strNameIdentifier = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        //string strUserData = identity.FindFirst(ClaimTypes.UserData)?.Value;

                        if(string.IsNullOrEmpty(strNameIdentifier))
                        {
                            foreach (Claim _Claim in identity.Claims)
                            {
                                if (Convert.ToString(_Claim.Type) == "nameidentifier")
                                {
                                    strNameIdentifier = _Claim.Value;
                                }
                            }
                        }

                        //if (!string.IsNullOrEmpty(userEmail))
                        if (!string.IsNullOrEmpty(strNameIdentifier) && strNameIdentifier.Length > 3)
                        {
                            ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                            ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            UserInfoFilter.UserName = new Database.Filter.StringSearch.SearchFilter();
                            UserInfoFilter.UserName.SearchString = strNameIdentifier;
                            UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                            //ImageSolutions.User.UserInfo UserInfo = new ImageSolutions.User.UserInfo();
                            //ImageSolutions.User.UserInfoFilter UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                            //UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                            //UserInfoFilter.EmailAddress.SearchString = userEmail;
                            //UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                            if (UserInfo == null)
                            {
                                UserInfo = new ImageSolutions.User.UserInfo();
                                UserInfoFilter = new ImageSolutions.User.UserInfoFilter();
                                UserInfoFilter.EmailAddress = new Database.Filter.StringSearch.SearchFilter();
                                UserInfoFilter.EmailAddress.SearchString = strNameIdentifier;
                                UserInfo = ImageSolutions.User.UserInfo.GetUserInfo(UserInfoFilter);

                                if (UserInfo == null)
                                {
                                    throw new Exception(string.Format("Email not found: {0} - {1} - {2}"
                                        , Convert.ToString(userEmail)
                                        , Convert.ToString(userName)
                                        , Convert.ToString(strNameIdentifier)
                                    ));
                                }
                            }

                            CurrentUser.Login(UserInfo.EmailAddress, UserInfo.Password);
                            //CurrentUser.LoginAs(userEmail);

                            if (CurrentWebsite != null && !string.IsNullOrEmpty(CurrentWebsite.StartingPath))
                            {
                                Response.Redirect(CurrentWebsite.StartingPath, false);
                                Context.ApplicationInstance.CompleteRequest();
                            }
                            else
                            {
                                Response.Redirect("/myaccount/dashboard.aspx?login=t", false);
                                Context.ApplicationInstance.CompleteRequest();
                            }
                        }
                        else
                        {
                            throw new Exception("Missing Email");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (SSOLog != null && !string.IsNullOrEmpty(SSOLog.SSOLogID))
                        {
                            SSOLog.ErrorMessage = String.Format("authentication_error - {0}", ex.Message);
                            SSOLog.Update();
                        }

                        Response.Redirect("/login.aspx?error=authentication_error");                    
                    }
                }
                else
                {
                    // 处理认证失败的情况
                    Response.Redirect("/login.aspx?error=authentication_failed");
                }
            }
            else
            {
                // 处理认证失败的情况
                Response.Redirect("/login.aspx?error=authentication_failed");
            }
        }        
    }
}