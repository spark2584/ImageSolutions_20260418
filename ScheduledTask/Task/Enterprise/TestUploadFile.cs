using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace ScheduledTask.Task.Enterprise
{
    public class TestUploadFile
    {
        public void Execute()
        {
            //C:\Import\Invoice\202410\Invoice_ImageSolutions_20241017072803.csv
            //C:\Import\Invoice\202410\Invoice_ImageSolutions_20241017072928.csv
            //C:\Import\Credit\202410\Credit_ImageSolutions_20241017074140.csv

            string strFilePath = string.Format(@"C:\Import\Credit\202410\Credit_ImageSolutions_20241017074140.csv");
            //Upload to SFTP
            using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPHost"]), (ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPLogin"]), (ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPPassword"])))
            {
                _SftpClient.Connect();
                UploadFile(_SftpClient, strFilePath);
            }
        }

        protected void UploadFile(SftpClient sftpclient, string filepath)
        {
            try
            {
                using (var filestream = System.IO.File.OpenRead(filepath))
                {
                    string strFileName = Path.GetFileName(filepath);
                    string strTest = string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPPath"]), strFileName);

                    sftpclient.UploadFile(filestream, string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["EnterpriseInvoiceSFTPPath"]), strFileName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    
}
