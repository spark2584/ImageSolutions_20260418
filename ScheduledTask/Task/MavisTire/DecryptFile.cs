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
    public class DecryptFile
    {
        public bool Execute()
        {
            List<ImageSolutions.MavisTire.MavisTireCustomerImport> CustomerImports = null;
            ImageSolutions.MavisTire.MavisTireCustomerImportFilter CustomerImportFilter = null;

            try
            {
                ImportGpgKey(Convert.ToString(ConfigurationManager.AppSettings["MavisGPGPublicKeyPath"]));
                ImportGpgKey(Convert.ToString(ConfigurationManager.AppSettings["MavisGPGPrivateKeyPath"]));

                CustomerImports = new List<ImageSolutions.MavisTire.MavisTireCustomerImport>();
                CustomerImportFilter = new ImageSolutions.MavisTire.MavisTireCustomerImportFilter();
                CustomerImportFilter.IsEncrypted = true;
                CustomerImportFilter.IsProcessed = false;
                CustomerImports = ImageSolutions.MavisTire.MavisTireCustomerImport.GetMavisTireCustomerImports(CustomerImportFilter);

                foreach (ImageSolutions.MavisTire.MavisTireCustomerImport _CustomerImport in CustomerImports)
                {
                    try
                    {
                        string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), _CustomerImport.FilePath); //Get path to saved file
                        string strExtension = Path.GetExtension(strFullPath);
                        string strOutPath = Path.Combine(Path.GetDirectoryName(strFullPath), "Decrypted", Path.GetFileNameWithoutExtension(strFullPath));

                        if (!Directory.Exists(Path.GetDirectoryName(strOutPath)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(strOutPath));
                        }

                        DecryptGpgFile(strFullPath, strOutPath, Convert.ToString(ConfigurationManager.AppSettings["MavisGPGPassPhrase"]));

                        _CustomerImport.FilePath = Path.Combine(Path.GetDirectoryName(_CustomerImport.FilePath), "Decrypted", Path.GetFileNameWithoutExtension(strFullPath));
                        _CustomerImport.FileName = Path.GetFileNameWithoutExtension(strFullPath);
                        _CustomerImport.IsEncrypted = false;
                        _CustomerImport.ErrorMessage = String.Empty;
                        _CustomerImport.Update();

                        List<string> objCCs = new List<string>();
                        objCCs.Add("alyssa@imageinc.com");
                        objCCs.Add("CYasgur@mavis.com");
                        objCCs.Add("CAllen@mavis.com");
                        objCCs.Add("lcontreras@mavis.com");
                        objCCs.Add("steve@imageinc.com");

                        string strBody = string.Empty;
                        strBody = string.Format(@"
Hello,
The Mavis User Data file {0} has been successfully received and processed. No further action is required at this time.
Thank you,

Image Solutions 
"
                            , _CustomerImport.FileName);

                        SendEmail("ashley@imageinc.com", string.Format("Mavis - Decryption Success"), strBody, objCCs).Wait();
                    }
                    catch (Exception ex)
                    {
                        bool blnSendEmail = string.IsNullOrEmpty(_CustomerImport.ErrorMessage);

                        _CustomerImport.ErrorMessage = ex.Message;
                        _CustomerImport.Update();

                        List<string> objCCs = new List<string>();
                        objCCs.Add("matthew.pendleton@imageinc.com");
                        objCCs.Add("shivali@imageinc.com");
                        objCCs.Add("CYasgur@mavis.com");
                        objCCs.Add("CAllen@mavis.com");
                        objCCs.Add("lcontreras@mavis.com");
                        objCCs.Add("steve@imageinc.com");

                        if (blnSendEmail)
                        {
                            SendEmail("it@imageinc.com", string.Format("Mavis - Decryption Failed"), string.Format("{0}", ex.Message), objCCs).Wait();
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
                CustomerImports = null;
                CustomerImportFilter = null;
            }

            return true;
        }

        public string DecryptGpgFile(string encryptedFilePath, string outputFilePath, string passphrase)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gpg",
                    Arguments = $"--batch --yes --passphrase \"{passphrase}\" --pinentry-mode loopback -o \"{outputFilePath}\" -d \"{encryptedFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"GPG decryption failed: {error}");
            }

            return output;
        }

        public string ImportGpgKey(string keyFilePath)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "gpg",
                    Arguments = $"--batch --yes --import \"{keyFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception($"GPG import failed: {error}");
            }

            return output;
        }

        public bool Test()
        {
            string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), "Uniformed Personnel - Full Workforce.txt.pgp");
            string strOutPath = Path.Combine(Path.GetDirectoryName(strFullPath), "Decrypted", "Uniformed Personnel - Full Workforce.txt");

            PGPEncryptDecrypt.Decrypt(strFullPath
                , Convert.ToString(ConfigurationManager.AppSettings["MavisGPGPrivateKeyPath"])
                , Convert.ToString(ConfigurationManager.AppSettings["MavisGPGPassPhras"])
                , strOutPath);

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
                    toemail = string.IsNullOrEmpty(Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"])) ? "steve@iamgeinc.com" : Convert.ToString(ConfigurationManager.AppSettings["DevelopmentEmail"]);
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
