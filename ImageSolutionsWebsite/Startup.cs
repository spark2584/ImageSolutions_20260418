using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Sustainsys.Saml2.Owin;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;

[assembly: OwinStartup(typeof(ImageSolutionsWebsite.Startup))]
namespace ImageSolutionsWebsite
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType
            });

            var spOptions = new SPOptions
            {
                //EntityId = new EntityId("willinventory")
                EntityId = new EntityId(String.Format("{0}", ConfigurationManager.AppSettings["SSOEntityID"]))
            };

            //var idp = new IdentityProvider(new EntityId("https://sts.windows.net/1b8aeafe-aee8-4bcd-a358-b14799b53c7f/"), spOptions)
            var idp = new IdentityProvider(new EntityId(String.Format("{0}", ConfigurationManager.AppSettings["SSOIdentityProvider"])), spOptions)
            {
                LoadMetadata = true,
                //MetadataLocation = "https://login.microsoftonline.com/5a9bb941-ba53-48d3-b086-2927fea7bf01/federationmetadata/2007-06/federationmetadata.xml"
                MetadataLocation = String.Format("{0}", ConfigurationManager.AppSettings["SSOMetaDataLocation"])
            };

            if (Convert.ToString(ConfigurationManager.AppSettings["SSOType"]) == "EntraID")
            {

            }
            else
            {
                if (Convert.ToString(ConfigurationManager.AppSettings["SSOMetaDataLocation"]) == "ADFS")
                {
                    idp.SigningKeys.AddConfiguredKey(
                        new X509Certificate2(
                            new X509Certificate2(
                                Convert.FromBase64String("MIIC0DCCAbigAwIBAgIQE+A7jDGPto9DZ41l3+QyQDANBgkqhkiG9w0BAQsFADAkMSIwIAYDVQQDExlBREZTIFNpZ25pbmcgLSBmcy5laGkuY29tMB4XDTIyMDUyNjAxMjU0N1oXDTI1MDUyNTAxMjU0N1owJDEiMCAGA1UEAxMZQURGUyBTaWduaW5nIC0gZnMuZWhpLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALF9ZrxMvp2PBWvDsX/oXIWru1o8csGKV55P0DGajMtH/9uoT0es/QzTKjGHVX7kw6dMTe5V/J/DDnUJUDPXa2D3QcDkolrTmPRsWkk73tJj+iYBMLbUVRzqRTGLobe7IbsdFaexY7aMqOK3p4ZS4nJMkgh842Et6uKFAwFZHlhPSijqwslz1QQ6UuqkOwICLiI8VC1X8JZGL2hvxI5tL5dNkRCnPUEDpltuowX+L4h0DivbFzqxrtA5CFwbyAdkoa/Da9BdFooFsngOs0xDETnHTyrx8Ozo6yiUlbFnlOHxsqHkqs31IUEHutuSMbvp2Ubh8xC/daP2hjQcZbSC648CAwEAATANBgkqhkiG9w0BAQsFAAOCAQEArKNPC3GxTCAFGn8xdifINH9AeVeGGHm7A/GlbS1Su3a85OxCZT0LSuzPcNJFfDTY2zkzuk5H2DhbEpgPJWKCFX4i/QuFNFjFX7wIJVdqUavPl7LXtc5RaIg+PjE3KaHeah6JvyAEPmTWKLIgwCNN8WIrHaH/rn3ErPP1i5/K6m55P8Wkje2/20lNROOTDamQUQY/vYO0HDjY49isgx+iS+KDvg730AtpHbrcU62Xjk4NzPzlmIlNjGD2vX0gA0ktDeJbkrc1QR2mN6J0ZhL1qn6vLsMtT1Jdxv+9ezv0/Q3VQ7t2I3rGRiKGaiFGmAiwUcn5HFV/ukOlnklgvLvt4w==")
                            )
                        )
                    );
                }
                else
                {
                    //var idpCertificate = new X509Certificate2("C:\\xampp\\htdocs\\NET\\Image\\ImageSolutionsWebsite\\TEST.cer");
                    var idpCertificate = new X509Certificate2(String.Format("{0}", ConfigurationManager.AppSettings["SSOCertificatePath"]));
                    idp.SigningKeys.AddConfiguredKey(new X509Certificate2(idpCertificate));
                }
            }

            var pingOneIdp = new IdentityProvider(
                //new EntityId("https://sso.connect.pingidentity.com/sso/idp/SSO.saml2?idpid=<your-idpid>"),
                new EntityId(String.Format("{0}", ConfigurationManager.AppSettings["SSOIdentityProviderPingOne"])),
                spOptions)
            {
                LoadMetadata = true,
                MetadataLocation = String.Format("{0}", ConfigurationManager.AppSettings["SSOMetaDataLocationPingOne"]) //"https://sso.connect.pingidentity.com/sso/idp/SSO.saml2?idpid=<your-idpid>"
            };

            var saml2Options = new Saml2AuthenticationOptions(false)
            {
                SPOptions = spOptions,
                SignInAsAuthenticationType = CookieAuthenticationDefaults.AuthenticationType
            };

            saml2Options.Notifications = new Saml2Notifications
            {
                AcsCommandResultCreated = (commandResult, response) =>
                {
                    // set reirect url
                    commandResult.Location = new Uri(String.Format("{0}", ConfigurationManager.AppSettings["SSOURI"]));

                    var issuer = response.Issuer?.Id?.ToLowerInvariant();

                    if (!string.IsNullOrEmpty(issuer))
                    {
                        if (issuer.Contains("pingidentity"))
                        {
                            commandResult.Location = new Uri(String.Format("{0}default.aspx?source=pingone", ConfigurationManager.AppSettings["SSOURI"]), UriKind.Relative); //new Uri("/SSO.aspx?source=pingone", UriKind.Relative);
                        }
                        //else
                        //{
                        //    commandResult.Location = new Uri(String.Format("{0}", ConfigurationManager.AppSettings["SSOURI"]));
                        //}
                    }
                }
            };

            saml2Options.IdentityProviders.Add(idp);
            saml2Options.IdentityProviders.Add(pingOneIdp);

            app.UseSaml2Authentication(saml2Options);
        }
    }
}