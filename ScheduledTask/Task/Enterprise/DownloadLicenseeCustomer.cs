using ImageSolutions.Enterprise;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Enterprise
{
    public class DownloadLicenseeCustomer
    {
        public bool Execute()
        {
            IEnumerable<ISftpFile> SftpFiles = null;

            try
            {
                string strLocalFolder = (ConfigurationManager.AppSettings["CustomerLocalPath"]) + DateTime.UtcNow.ToString("yyyyMM");

                if (!Directory.Exists(strLocalFolder))
                {
                    Directory.CreateDirectory(strLocalFolder);
                }

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    SftpFiles = _SftpClient.ListDirectory(Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPath"]));
                    foreach (SftpFile _SftpFile in SftpFiles)
                    {
                        if (_SftpFile.FullName.Contains(".xls"))
                        {
                            EnterpriseCustomerImport EnterpriseCustomerImport = new EnterpriseCustomerImport();

                            EnterpriseCustomerImportFilter EnterpriseCustomerImportFilter = new EnterpriseCustomerImportFilter();
                            EnterpriseCustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                            EnterpriseCustomerImportFilter.FileName.SearchString = _SftpFile.Name;
                            EnterpriseCustomerImport = EnterpriseCustomerImport.GetEnterpriseCustomerImport(EnterpriseCustomerImportFilter);

                            if (EnterpriseCustomerImport == null)
                            {
                                using (Stream _Stream = File.OpenWrite(string.Format(@"{0}\{1}", strLocalFolder, _SftpFile.Name)))
                                {
                                    _SftpClient.DownloadFile(_SftpFile.FullName, _Stream);
                                    //Store in Database
                                    EnterpriseCustomerImport NewEnterpriseCustomerImport = new EnterpriseCustomerImport();
                                    NewEnterpriseCustomerImport.FilePath = string.Format(@"{0}\{1}", DateTime.UtcNow.ToString("yyyyMM"), _SftpFile.Name);
                                    NewEnterpriseCustomerImport.FileName = _SftpFile.Name;
                                    NewEnterpriseCustomerImport.IsStore = true;
                                    NewEnterpriseCustomerImport.Create();

                                    //Delete file in sFTP - need to test
                                    if (!string.IsNullOrEmpty(NewEnterpriseCustomerImport.EnterpriseCustomerImportID))
                                    {
                                        _SftpClient.DeleteFile(string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPath"]), _SftpFile.Name));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return true;
        }

        public bool TestUpload()
        {
            try
            {
                string strLocalFilePath = string.Format(@"C:\Import\Customer\202212\ehi_location_all20221208_test.xls");
                string strRemoteFilePath = string.Format(@"/Outbound/ehi_location_all20221208_test.xls");


                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();

                    using (Stream localFile = File.Create(strLocalFilePath))
                    {
                        _SftpClient.UploadFile(localFile, strRemoteFilePath);
                    }
                }
            }
            catch
            {

            }
            return true;
        }

        public bool TestDelete()
        {
            try
            {
                string strRemoteFilePath = string.Format(@"/Outbound/ehi_location_all20221208.xls");

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["CustomerLicenseeSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    _SftpClient.DeleteFile(strRemoteFilePath);
                    _SftpClient.Disconnect();
                }
            }
            catch
            {

            }
            return true;
        }
    }
}
