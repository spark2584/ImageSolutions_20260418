
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using SendGrid;
using ImageSolutions;

namespace EmailMarketing
{
    class SendSMS
    {
        public static bool Execute()
        {
            string strSQL = "update smsoutbox set isapproved=1 where issent=0 and isapproved=0";
            Database.ExecuteSQL(strSQL);

            List<ImageSolutions.Marketing.SMSOutbox> objSMSOutboxes = null;
            ImageSolutions.Marketing.SMSOutboxFilter objFilter = null;

            try
            {
                objFilter = new ImageSolutions.Marketing.SMSOutboxFilter();
                objFilter.IsApproved = true;
                objFilter.IsSent = false;
                objFilter.ErrorMessage = new Database.Filter.StringSearch.SearchFilter();
                objFilter.ErrorMessage.Operator = Database.Filter.StringSearch.SearchOperator.empty;
                objSMSOutboxes = ImageSolutions.Marketing.SMSOutbox.GetSMSOutboxes(objFilter);
                if (objSMSOutboxes != null)
                {
                    int intCount = 0;
                    foreach (ImageSolutions.Marketing.SMSOutbox objSMSOutbox in objSMSOutboxes)
                    {
                        intCount++;
                        Console.WriteLine(intCount.ToString() + objSMSOutbox.MarketingTemplate.MarketingCampaignName);

                        try
                        {
                            SMS.SendSMS(objSMSOutbox.SMSMobileNumber, objSMSOutbox.Message);
                            objSMSOutbox.IsSent = true;
                            objSMSOutbox.SentOn = DateTime.UtcNow;
                            objSMSOutbox.Update();
                        }
                        catch(Exception ex)
                        {
                            //objSMSOutbox.IsSent = true;
                            objSMSOutbox.SentOn = DateTime.UtcNow;
                            objSMSOutbox.ErrorMessage = ex.Message;
                            objSMSOutbox.Update();
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
                objSMSOutboxes = null;
                objFilter = null;
            }
            return true;
        }
    }
}
