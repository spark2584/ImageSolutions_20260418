using ImageSolutions.User;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Albertsons
{
    public class SendWelcomeEmail
    {
        public bool Execute()
        {
            try
            {
                List<ImageSolutions.User.UserWebsite> UserWebsites = new List<ImageSolutions.User.UserWebsite>();
                ImageSolutions.User.UserWebsiteFilter UserWebsiteFilter = new ImageSolutions.User.UserWebsiteFilter();
                UserWebsiteFilter.WebsiteID = new Database.Filter.StringSearch.SearchFilter();
                UserWebsiteFilter.WebsiteID.SearchString = Convert.ToString(ConfigurationManager.AppSettings["AlbertsonsGearWebsiteID"]);
                UserWebsiteFilter.SendWelcomeEmail = true;
                UserWebsiteFilter.Inactive = false;
                UserWebsites = ImageSolutions.User.UserWebsite.GetUserWebsites(UserWebsiteFilter);

                int counter = 0;

                foreach(ImageSolutions.User.UserWebsite _UserWebsite in UserWebsites)
                {
                    counter++;

                    //5F75A788-9BC7-4E57-8984-FC95CA945F99	ABS Companies Uniforms - 7
                    //A4971F0B-B598-4D36-9B49-BDA96EC85D76	Albertsons Gear - 8

                    string strHTMLContent = @"<!DOCTYPE html>
                                    <html>
                                        <head></head>
                                        <body>
                                            <div style='font-size:14px;padding-bottom:30px;height:100%;position:relative;'>
                                                <table style='font-size:0.9em;width:100%;border-collapse:collapse;margin:10px 0;'>
                                                    <tr>
                                                        <td><a href=""https://portal.imageinc.com/website.aspx?website=A4971F0B-B598-4D36-9B49-BDA96EC85D76""><img src=""https://portal.imageinc.com/assets/company/Albertsons/NewLogin.png"" /></a></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <table border=""0"" cellspacing=""0"" cellpadding=""0"">
                                                                <tbody>
                                                                    <tr>
                                                                        <td valign=""top"" style=""padding:0 1.5pt 0 0;"">
                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><a href=""https://www.eimagesolutions.com/"" target=""_blank"" rel=""noopener noreferrer"" data-auth=""NotApplicable"" data-linkindex=""1""><span style=""color: blue !important; font-family: Times New Roman, serif; text-decoration: none;""><img data-imagetype=""AttachmentByCid"" src=""https://portal.imageinc.com/assets/company/ImageSolutions/ImageSolutionsEmailLogo.png"" border=""0"" id=""x_x_Picture_x0020_1"" data-outlook-trace=""F:1|T:1"" style=""width: 78.49pt; height: 82.99pt; min-height: auto; min-width: auto;"" crossorigin=""use-credentials"" fetchpriority=""high"" class=""Do8Zj""></span></a><span style=""font-family:Times New Roman,serif;""></span></p>
                                                                        </td>
                                                                        <td style=""padding:0 0 0 9pt;"">
                                                                            <table border=""0"" cellspacing=""0"" cellpadding=""0"">
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td colspan=""2"" style=""padding:0 0 0.75pt 0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><b><span style=""color: rgb(8, 153, 255) !important; font-size: 12pt; font-family: Helvetica, sans-serif;"">Image Solutions Customer Service</span></b><span style=""color: rgb(8, 153, 255) !important; font-size: 12pt; font-family: Helvetica, sans-serif;""></span></p>
                                                                                        </td>
                                                                                        <td style=""width:7.5pt;padding:0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;"" aria-hidden=""true"">&nbsp;</p>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan=""2"" style=""padding:0 0 3pt 0;""></td>
                                                                                        <td style=""width:7.5pt;padding:0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;"" aria-hidden=""true"">&nbsp;</p>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td colspan=""2"" style=""padding:0 0 0.75pt 0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><span style=""color: rgb(87, 86, 86) !important; font-size: 8.5pt; font-family: Helvetica, sans-serif;""><a href=""https://www.imageinc.com/"" target=""_blank"" rel=""noopener noreferrer"" data-auth=""NotApplicable"" title=""https://www.imageinc.com/"" data-linkindex=""2""><span style=""color: blue !important;"">Image Solutions</span></a></span></p>
                                                                                        </td>
                                                                                        <td style=""width:7.5pt;padding:0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;"" aria-hidden=""true"">&nbsp;</p>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td valign=""top"" colspan=""2"" style=""padding:0 0 0.75pt 0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><span style=""color: rgb(87, 86, 86) !important; font-size: 8.5pt; font-family: Helvetica, sans-serif;"">4692 Brate Drive • Suite #300</span></p>
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><span style=""color: rgb(87, 86, 86) !important; font-size: 8.5pt; font-family: Helvetica, sans-serif;"">West Chester • OH • 45011</span></p>
                                                                                        </td>
                                                                                        <td style=""width:7.5pt;padding:0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;"" aria-hidden=""true"">&nbsp;</p>
                                                                                        </td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td valign=""top"" colspan=""2"" style=""width:198.1pt;padding:0 0 0.75pt 0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><span style=""color: rgb(87, 86, 86) !important; font-size: 8.5pt; font-family: Helvetica, sans-serif;"">Direct:&nbsp;888.756.9898</span></p>
                                                                                        </td>
                                                                                        <td valign=""top"" style=""padding:0 0 1.5pt 0;""></td>
                                                                                    </tr>
                                                                                    <tr>
                                                                                        <td valign=""top"" style=""padding:0 0 0.75pt 0;"">
                                                                                            <p style=""font-size:11pt;font-family:Calibri,sans-serif;margin:0;""><u><span style=""color: blue !important; font-size: 8.5pt; font-family: Helvetica, sans-serif;""><a href=""mailto:cs@imageinc.com"" data-linkindex=""3""><span style=""color: blue !important;"">cs@imageinc.com</span></a></span></u><span style=""color: rgb(87, 86, 86) !important; font-size: 8.5pt; font-family: Helvetica, sans-serif;""></span></p>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </table>
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </body>
                                    </html>";

                    Console.WriteLine(String.Format("{1}. Sending to: {0}", _UserWebsite.UserInfo.EmailAddress, counter));
                    SendEmail(_UserWebsite.UserInfo.EmailAddress, "Image Solutions - Albertsons Uniform Website Password Reset", strHTMLContent);
                    //SendEmail("steve@imageinc.com", "Image Solutions - Albertsons Uniform Website Password Reset", strHTMLContent);

                    _UserWebsite.SendWelcomeEmail = false;
                    _UserWebsite.Update();

                    //if (counter == 2)
                    //{
                    //    Console.WriteLine("pause");
                    //    break;
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public void SendEmail(string toemail, string subject, string htmlcontent, List<string> ccs = null)
        {
            UserInfoFilter objFilter = null;
            UserInfo objUserInfo = null;

            try
            {
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
