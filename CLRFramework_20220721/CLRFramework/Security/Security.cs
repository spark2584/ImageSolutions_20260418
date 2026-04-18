using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Security.Cryptography;

public partial class Utility
{
    public partial class Security
    {
        public static string GetWindowsUserName()
        {
            System.Security.Principal.WindowsPrincipal objWindow = null;
            string strReturn = string.Empty;

            try
            {
                objWindow = System.Threading.Thread.CurrentPrincipal as System.Security.Principal.WindowsPrincipal;
                strReturn = objWindow.Identity.Name;
                int intStart = strReturn.IndexOf("\\");
                if (intStart != -1) strReturn = strReturn.Substring(intStart + 1, (strReturn.Length - intStart - 1));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                objWindow = null;
            }
            return strReturn;
        }

        public static string CreatePasswordHash(string Password, string PasswordSaltKey)
        {
            string strPasswordToHash = string.Empty;
            string strReturn = string.Empty;

            strPasswordToHash = String.Concat(Password, PasswordSaltKey);
            strReturn = FormsAuthentication.HashPasswordForStoringInConfigFile(strPasswordToHash, "SHA1");

            return strReturn;
        }

        public static string CreatePasswordSaltKey(int SaltKeyLength)
        {
            RNGCryptoServiceProvider objRNG = null;
            string strReturn = string.Empty;

            try
            {
                // Generate a cryptographic random number using the cryptographic service provider
                objRNG = new RNGCryptoServiceProvider();
                byte[] arrBuff = new byte[SaltKeyLength];
                objRNG.GetBytes(arrBuff);
                // Return a Base64 string representation of the random number
                strReturn = Convert.ToBase64String(arrBuff);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return strReturn;
        }
    }
}
