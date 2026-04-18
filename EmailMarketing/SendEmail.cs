
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace EmailMarketing
{
    class SendEmail
    {
        public static async Task Execute()
        {
            string strSQL = "update emailoutbox set isapproved=1 where isemailed=0 and isapproved=0";
            Database.ExecuteSQL(strSQL);


            List<ImageSolutions.Marketing.EmailOutbox> objEmailOutboxes = null;
            ImageSolutions.Marketing.EmailOutboxFilter objFilter = null;

            try
            {
                objFilter = new ImageSolutions.Marketing.EmailOutboxFilter();
                objFilter.IsApproved = true;
                objFilter.IsEmailed = false;
                objFilter.ErrorMessage = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ErrorMessage.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                objEmailOutboxes = ImageSolutions.Marketing.EmailOutbox.GetEmailOutboxes(objFilter);
                if (objEmailOutboxes != null)
                {
                    int intCount = 0;
                    foreach (ImageSolutions.Marketing.EmailOutbox objEmailOutbox in objEmailOutboxes)
                    {
                        intCount++;
                        Console.WriteLine(intCount.ToString() + objEmailOutbox.MarketingTemplate.MarketingCampaignName);
                        
                        try
                        {
                            await SendGrid(objEmailOutbox.ToEmail, objEmailOutbox.Subject, objEmailOutbox.HTMLContent, objEmailOutbox.MarketingTemplate.MarketingCampaignName);
                            objEmailOutbox.IsEmailed = true;
                            objEmailOutbox.EmailedOn = DateTime.UtcNow;
                            objEmailOutbox.Update();
                        }
                        catch (Exception ex)
                        {
                            //objSMSOutbox.IsSent = true;
                            objEmailOutbox.EmailedOn = DateTime.UtcNow;
                            objEmailOutbox.ErrorMessage = ex.Message;
                            objEmailOutbox.Update();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objEmailOutboxes = null;
                objFilter = null;
            }
        }

        public static async Task<Response> SendGrid(string ToEmail, string Subject, string HTMLContent, string MarketingCampaignName)
        {
            try
            {
                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"])) || Convert.ToString(ConfigurationManager.AppSettings["Environment"]) != "production")
                {
                    ToEmail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "vincent@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
                }

                string strAPIKey = Convert.ToString(ConfigurationManager.AppSettings["SendGridApiKey"]);
                SendGridClient Client = new SendGridClient(strAPIKey);
                EmailAddress FromEmailAddress = new EmailAddress(Convert.ToString(ConfigurationManager.AppSettings["EmailFrom"]));
                EmailAddress ToEmailAddress = new EmailAddress(ToEmail);
                SendGridMessage SendGridMessage = MailHelper.CreateSingleEmail(FromEmailAddress, ToEmailAddress, Subject, null, HTMLContent);

                if (!string.IsNullOrEmpty(MarketingCampaignName))
                {
                    SendGridMessage.Categories = new List<string>();
                    SendGridMessage.AddCategory(MarketingCampaignName);
                }

                if (ConfigurationManager.AppSettings["EmailCC"] != null)
                {
                    SendGridMessage.AddCc(Convert.ToString(ConfigurationManager.AppSettings["EmailCC"]));
                }

                return await Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
    }
}
