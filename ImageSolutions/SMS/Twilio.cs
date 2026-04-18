using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ImageSolutions
{
    public class SMS
    {
        public static bool SendSMS(string ToPhoneNumber, string Message)
        {
            try
            {
                var accountSid = "ACc1ad8efae5ea8b55bd5f1295ae182b78";
                var authToken = "de613b702bd95ffac0bc7f6b3746bf2b";
                TwilioClient.Init(accountSid, authToken);
                if (!ToPhoneNumber.StartsWith("+1)")) ToPhoneNumber = "+1" + ToPhoneNumber;
                var messageOptions = new CreateMessageOptions(new PhoneNumber(ToPhoneNumber));
                //var messageOptions = new CreateMessageOptions(new PhoneNumber("+16269751375"));
                messageOptions.From = new PhoneNumber("+18556424786");
                messageOptions.Body = Message;
                var message = MessageResource.Create(messageOptions);
                Console.WriteLine(message.Body);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return true;
        }
    }
}
