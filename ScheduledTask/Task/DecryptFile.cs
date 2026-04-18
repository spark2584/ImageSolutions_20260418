using ImageSolutions.Address;
using ImageSolutions.Customer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScheduledTask.Task
{
    public class DecryptFile
    {
        public bool Execute()
        {
            List<ImageSolutions.Enterprise.EnterpriseCustomerImport> CustomerImports = null;
            ImageSolutions.Enterprise.EnterpriseCustomerImportFilter CustomerImportFilter = null;

            try
            {
                CustomerImports = new List<ImageSolutions.Enterprise.EnterpriseCustomerImport>();
                CustomerImportFilter = new ImageSolutions.Enterprise.EnterpriseCustomerImportFilter();
                CustomerImportFilter.IsEncrypted = true;
                CustomerImportFilter.IsProcessed = false;
                CustomerImports = ImageSolutions.Enterprise.EnterpriseCustomerImport.GetEnterpriseCustomerImports(CustomerImportFilter);

                foreach (ImageSolutions.Enterprise.EnterpriseCustomerImport _CustomerImport in CustomerImports)
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

                        PGPEncryptDecrypt.Decrypt(strFullPath
                            , Convert.ToString(ConfigurationManager.AppSettings["PGPPrivateKeyPath"])
                            , Convert.ToString(ConfigurationManager.AppSettings["PGPPassPhrase"])
                            , strOutPath);

                        _CustomerImport.FilePath = Path.Combine(Path.GetDirectoryName(_CustomerImport.FilePath), "Decrypted", Path.GetFileNameWithoutExtension(strFullPath));
                        _CustomerImport.FileName = Path.GetFileNameWithoutExtension(strFullPath);
                        _CustomerImport.IsEncrypted = false;
                        _CustomerImport.ErrorMessage = String.Empty;
                        _CustomerImport.Update();
                    }
                    catch (Exception ex)
                    {
                        _CustomerImport.ErrorMessage = ex.Message;
                        _CustomerImport.Update();
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

        public bool Test()
        {
            string strFullPath = String.Format("{0}{1}", Convert.ToString(ConfigurationManager.AppSettings["CustomerLocalPath"]), "em_preemployee_20240228_14354_TEST.tsv.pgp");
            string strOutPath = Path.Combine(Path.GetDirectoryName(strFullPath), "Decrypted", "em_preemployee_20240228_14354_TEST.tsv");

            PGPEncryptDecrypt.Decrypt(strFullPath
                , Convert.ToString(ConfigurationManager.AppSettings["PGPPrivateKeyPath"])
                , Convert.ToString(ConfigurationManager.AppSettings["PGPPassPhrase"])
                , strOutPath);

            return true;
        }
    }
}
