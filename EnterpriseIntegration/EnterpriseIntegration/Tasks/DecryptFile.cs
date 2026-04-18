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

namespace EnterpriseIntegration.Tasks
{
    public class DecryptFile
    {
        public bool Execute()
        {
            List<CustomerImport> CustomerImports = null;
            CustomerImportFilter CustomerImportFilter = null;

            try
            {
                CustomerImports = new List<CustomerImport>();
                CustomerImportFilter = new CustomerImportFilter();
                CustomerImportFilter.IsEncrypted = true;
                CustomerImportFilter.IsProcessed = false;
                CustomerImports = CustomerImport.GetCustomerImports(CustomerImportFilter);

                foreach (CustomerImport _CustomerImport in CustomerImports)
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
    }
}
