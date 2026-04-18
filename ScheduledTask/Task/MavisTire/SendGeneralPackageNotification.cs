using ImageSolutions.Address;
using ImageSolutions.Customer;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ScheduledTask.Task.MavisTire
{
    public class SendGeneralPackageNotification
    {
        public bool Execute()
        {
            SqlDataReader objRead = null;
            string strSQL = string.Empty;

            try
            {
                strSQL = string.Format(@"
SELECT u.FirstName, u.EmailAddress --, uw.PackageAvailableDate
FROM UserWebsite (NOLOCK) uw
Inner Join UserInfo (NOLOCK) u on u.UserInfoID = uw.UserInfoID
WHERE uw.WebsiteID = 25
and uw.InActive = 0
and u.EmailAddress not like '%@imageinc.com'
and (
uw.PackageAvailableDate is null
or 
uw.PackageAvailableDate <= GETUTCDATE()
)

"
                    , Convert.ToString(ConfigurationManager.AppSettings["MavisTireWebsiteID"])
                );

                objRead = Database.GetDataReader(strSQL, Database.DefaultConnectionString);

                while (objRead.Read())
                {
                    try
                    {
                        string strFirstName = Convert.ToString(objRead["FirstName"]);
                        string strEmail = Convert.ToString(objRead["EmailAddress"]);

                        string strBody = string.Empty;

                        strBody = string.Format(@"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=windows-1252"">
<meta name=""Generator"" content=""Microsoft Word 15 (filtered)"">
<style>
<!--
 /* Font Definitions */
 @font-face
	{{font-family:Wingdings;
    panose-1:5 0 0 0 0 0 0 0 0 0;}}
@font-face
	{{font-family:""Cambria Math"";
	panose-1:2 4 5 3 5 4 6 3 2 4;}}
@font-face
	{{font-family:Aptos;}}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:0in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}

a:link, span.MsoHyperlink
	{{color:#0563C1;
	text-decoration:underline;}}
p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpFirst, li.MsoListParagraphCxSpFirst, div.MsoListParagraphCxSpFirst
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:0in;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpMiddle, li.MsoListParagraphCxSpMiddle, div.MsoListParagraphCxSpMiddle
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:0in;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
p.MsoListParagraphCxSpLast, li.MsoListParagraphCxSpLast, div.MsoListParagraphCxSpLast
	{{margin-top:0in;
	margin-right:0in;
	margin-bottom:8.0pt;
	margin-left:.5in;
	line-height:115%;
	font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
.MsoChpDefault
	{{font-size:12.0pt;
	font-family:""Aptos"",sans-serif;}}
.MsoPapDefault
	{{margin-bottom:8.0pt;
	line-height:115%;}}
@page WordSection1
	{{size:8.5in 11.0in;
	margin:1.0in 1.0in 1.0in 1.0in;}}
div.WordSection1
	{{page:WordSection1;}}
 /* List Definitions */
 ol
	{{margin-bottom:0in;}}

ul
	{{margin-bottom:0in;}}
-->
</style>
</head>
<body style='word-wrap:break-word'>

<div>

<p align=""center"">
<img height=""125"" src=""https://portal.imageinc.com/assets/company/mavis/2024_Mavis_Tire_Logo.png"">
</p>

<p align=""center""><b>Welcome to the Mavis Gear E-Store!</b></p>

<p>&nbsp;</p>

<p>Hello, {0}!</p>
<br />

<p>Good News! Your <b>Annual Mavis Uniform Allotment</b> is now available and ready to use.</p>

<br />
<p class=MsoNormal><b>What Should You Do Today? </b></p>

<p class=MsoListParagraphCxSpFirst style='text-indent:-.25in'>
<b>1.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></b>
<b>Log in</b> to the <b> Mavis Gear E-Store </b> website via the Company Intranet on the store computer. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span>
<b>• Username</b>: Use your personal email address listed in ADP. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;text-indent:-.25in;line-height:normal'>
<span style='font-family:Symbol'> <span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span>
<b>• Password:</b> Use the password you created during the initial login. 
<i>(If you forgot your password, password reset is an option.) </i>
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-top:0in;margin-right:0in;margin-bottom:0in;margin-left:.75in;line-height:normal'><i>&nbsp;</i></p>

<p class=MsoListParagraphCxSpMiddle style='margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b>2.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></b>
<b>Select </b>your preferred uniform package. 
</p>

<p class=MsoListParagraphCxSpMiddle style='margin-bottom:0in;line-height:normal'>&nbsp;</p>

<p class=MsoListParagraphCxSpLast style='margin-bottom:0in;text-indent:-.25in;line-height:normal'>
<b>3.<span style='font:7.0pt ""Times New Roman""'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></b>
<b>Submit </b>your Order. 
</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>&nbsp;</p>

<p class=MsoNormal style='margin-bottom:0in;line-height:normal'>Orders should be placed by Team Members during working hours. </p>

<br />
<p align=""center"">Thank you for being a valued member of the Mavis Team!</p>
<br />

<p align=""center"">
Image Solutions Customer Service: 
<a href=""mailto:cs@imageinc.com"">cs@imageinc.com</a> | (800)805-3090<br/>
Monday - Friday, 9:00 am - 6:00 pm EST
</p>

</div>
</body>
</html>
"
                        , strFirstName);

                        SendEmail(strEmail, string.Format("Your Annual Mavis Uniform Allotment is Available!"), strBody).Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("{0}", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (objRead != null) objRead.Dispose();
                objRead = null;
            }

            return true;
        }

        static async Task<Response> SendEmail(string toemail, string subject, string htmlcontent, List<string> ccs = null)
        {
            try
            {
                if (String.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["Environment"]))
                    || Convert.ToString(ConfigurationManager.AppSettings["Environment"]).ToLower() != "production"
                )
                {
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@imageinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
                }

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

                return await Client.SendEmailAsync(SendGridMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }
    }
}
