using System;
using System.Linq;
using System.Web.UI;
using Sustainsys.Saml2;
using Sustainsys.Saml2.WebSso;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Sustainsys.Saml2.Saml2P;
using Sustainsys.Saml2.Owin;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImageSolutionsWebsite
{
    public partial class SSO : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 检查是否为SAML响应
            if (Context.User.Identity.IsAuthenticated)
            {
                var identity = User.Identity as ClaimsIdentity;

                if (identity == null)
                {
                    Debug.WriteLine("No ClaimsIdentity found.");
                    return;
                }

                string issuerFromClaims = identity.Claims.FirstOrDefault()?.Issuer;
                string issuerFromQuery = Request.QueryString["source"];

                Debug.WriteLine($"QueryString Source: {issuerFromQuery}");
                Debug.WriteLine($"Claims Issuer: {issuerFromClaims}");

                string userName = identity.FindFirst(ClaimTypes.Name)?.Value;
                string userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;

                Debug.WriteLine($"UserName: {userName}");
                Debug.WriteLine($"UserEmail: {userEmail}");

                ImageSolutions.SSO.SSOLog SSOLog = new ImageSolutions.SSO.SSOLog();
                SSOLog.AuthenticateResult = issuerFromClaims;
                SSOLog.Create();

                if (issuerFromQuery == "pingone" ||
                    (issuerFromQuery == null && issuerFromClaims?.ToLowerInvariant().Contains("pingidentity") == true))
                {
                    Debug.WriteLine("User logged in via PingOne");

                }
                else
                {
                    Debug.WriteLine("User logged in via Old IdP");

                }


                if (!string.IsNullOrEmpty(userEmail))
                {

                }
                else
                {
                    Response.Redirect("/Login.aspx?error=missingemail");
                }
            }
            else
            {
                Debug.WriteLine("User not authenticated");
                Response.Redirect("/Login.aspx?error=unauthenticated");
            }


        }
    }
}