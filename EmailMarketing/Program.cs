using ImageSolutions;
using ImageSolutions.Marketing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailMarketing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //SMS.SendSMS("3102914629", "Thanks for your Image Solutions order! We'll notify you when it ships. Track Your Order bit.ly/img-inc");

            //SMS.SendSMS("3102914629", "Discount Tire Gear: Chris, thanks for your purchase. Order #XYZ123 is confirmed. We’ll send tracking once it ships");

            //SMS.SendSMS("3102914629", "Discount Tire Gear: Good news! Chris, your order's on the way! Track it here: bit.ly/img-dt Need more uniforms? bit.ly/img-dt");

            //FedExSample.Execute();


            //try { WelcomeEmail.Execute(); } catch { }

            try { await SendEmail.Execute(); } catch { }
            try { SendSMS.Execute(); } catch { }

        }
    }
}
