using ImageSolutions.Brightspeed;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledTask.Task.Brightspeed
{
    public class DownloadCustomer
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

                using (SftpClient _SftpClient = new SftpClient(Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCustomerSFTPHost"])
                    , Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCustomerSFTPLogin"])
                    , Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCustomerSFTPPassword"])
                    )
                )
                {
                    _SftpClient.Connect();
                    SftpFiles = _SftpClient.ListDirectory(Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCustomerSFTPath"]));
                    foreach (SftpFile _SftpFile in SftpFiles)
                    {
                        if (_SftpFile.FullName.Contains(".xls"))
                        {
                            BrightspeedCustomerImport BrightspeedCustomerImport = new BrightspeedCustomerImport();

                            BrightspeedCustomerImportFilter BrightspeedCustomerImportFilter = new BrightspeedCustomerImportFilter();
                            BrightspeedCustomerImportFilter.FileName = new Database.Filter.StringSearch.SearchFilter();
                            BrightspeedCustomerImportFilter.FileName.SearchString = _SftpFile.Name;
                            BrightspeedCustomerImport = BrightspeedCustomerImport.GetBrightspeedCustomerImport(BrightspeedCustomerImportFilter);

                            if (BrightspeedCustomerImport == null)
                            {
                                using (Stream _Stream = File.OpenWrite(string.Format(@"{0}\{1}", strLocalFolder, _SftpFile.Name)))
                                {
                                    _SftpClient.DownloadFile(_SftpFile.FullName, _Stream);
                                    //Store in Database
                                    BrightspeedCustomerImport NewBrightspeedCustomerImport = new BrightspeedCustomerImport();
                                    NewBrightspeedCustomerImport.FilePath = string.Format(@"{0}\{1}", DateTime.UtcNow.ToString("yyyyMM"), _SftpFile.Name);
                                    NewBrightspeedCustomerImport.FileName = _SftpFile.Name;
                                    NewBrightspeedCustomerImport.Create();

                                    //Delete file in sFTP - need to test
                                    if (!string.IsNullOrEmpty(NewBrightspeedCustomerImport.BrightspeedCustomerImportID))
                                    {
                                        _SftpClient.DeleteFile(string.Format(@"{0}/{1}", Convert.ToString(ConfigurationManager.AppSettings["BrightspeedCustomerSFTPath"]), _SftpFile.Name));
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
    }
}
