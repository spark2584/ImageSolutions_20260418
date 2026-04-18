using ImageSolutions.Marketing;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageSolutions.Website;
using ImageSolutions.User;
using System.Data.SqlClient;
using ImageSolutions;

namespace EmailMarketing
{
    class WelcomeEmail
    {
        public static bool Execute()
        {
            List<MarketingTemplate> objMarketingTemplates = null;
            MarketingTemplateFilter objFilter = null;
            List<UserWebsite> objUserWebsites = null;
            int intCount = 0;

            try
            {
                objFilter = new MarketingTemplateFilter();
                objFilter.MarketingCampaigns = new List<MarketingTemplate.enumMarketingCampaign>();
                objFilter.MarketingCampaigns.Add(MarketingTemplate.enumMarketingCampaign.Welcome);
                objFilter.IsEnabled = true;
                objMarketingTemplates = MarketingTemplate.GetMarketingTemplates(objFilter);
                foreach (MarketingTemplate objMarketingTemplate in objMarketingTemplates)
                {
                    if (objMarketingTemplate.Website.UserWebsites != null)
                    {
                        // && m.UserWebsiteID == "658964" = Vincent Mavis
                        // && m.UserWebsiteID == "334067" = Vincent EBA
                        objUserWebsites = objMarketingTemplate.Website.UserWebsites.FindAll(m => !m.InActive && m.OptInForNotification && !m.MarketingWelcome);

                        objUserWebsites = objMarketingTemplate.Website.UserWebsites.FindAll(m => m.UserWebsiteID == "334067");

                        if (objMarketingTemplate.Website.EnableEmployeeCredit)
                        {
                            objUserWebsites = objUserWebsites.FindAll(m => m.TotalActiveBudgetAmount > 0);
                        }

                        if (objUserWebsites != null)
                        {
                            Console.WriteLine(objUserWebsites.Count());

                            foreach (UserWebsite objUserWebsite in objUserWebsites)
                            {
                                intCount++;

                                Console.Write("Processing " + intCount + " ");

                                if (objUserWebsite.SalesOrders != null && objUserWebsite.SalesOrders.Count > 0)
                                {
                                    Console.WriteLine("UserWebsite already has order placed");
                                    objUserWebsite.MarketingWelcome = true;
                                    objUserWebsite.Update();
                                }
                                else
                                {
                                    Console.WriteLine("Generating welcome email");
                                    CreateWelcomeEmail(objMarketingTemplate, objUserWebsite);
                                    //SMS.SendSMS(objUserWebsite.SMSMobileNumber, objMarketingTemplate.SMSContent)
                                }
                            }
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
                objMarketingTemplates = null;
                objFilter = null;
            }
            return true;
        }

        public static bool CreateWelcomeEmail(MarketingTemplate MarketingTemplate, ImageSolutions.User.UserWebsite UserWebsite)
        {
            EmailOutbox objEmailOutbox = null;
            SqlConnection objConn = null;
            SqlTransaction objTran = null;

            try
            {
                objConn = new SqlConnection(Database.DefaultConnectionString);
                objConn.Open();
                objTran = objConn.BeginTransaction();

                objEmailOutbox = new EmailOutbox();
                objEmailOutbox.MarketingTemplateID = MarketingTemplate.MarketingTemplateID;
                objEmailOutbox.UserWebsiteID = UserWebsite.UserWebsiteID;
                objEmailOutbox.Subject = MarketingTemplate.EmailSubject.Replace("{$FirstName}", UserWebsite.UserInfo.FirstName);
                objEmailOutbox.HTMLContent = MarketingTemplate.EmailContent.Replace("{$FirstName}", UserWebsite.UserInfo.FirstName)
                                                                           .Replace("{$Budget}", UserWebsite.TotalActiveBudgetAmount.ToString("C"));

                if (!string.IsNullOrEmpty(UserWebsite.NotificationEmail))
                    objEmailOutbox.ToEmail = UserWebsite.NotificationEmail;
                else
                    objEmailOutbox.ToEmail = UserWebsite.UserInfo.EmailAddress;

                objEmailOutbox.IsApproved = false;
                objEmailOutbox.IsEmailed = false;
                objEmailOutbox.Create(objConn, objTran);

                UserWebsite.MarketingWelcome = true;
                UserWebsite.Update(objConn, objTran);

                objTran.Commit();
            }
            catch (Exception ex)
            {
                if (objTran != null && objTran.Connection != null) objTran.Rollback();
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
    }
}
