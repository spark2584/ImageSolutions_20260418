using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetSuiteLibrary.com.netsuite.webservices;
using System.Configuration;
using System.Security.Cryptography;

namespace NetSuiteLibrary
{
    public class User : NetSuiteBase
    {
        public User()
        {
        }
        public User(ImageSolutions.Employee.Employee Employee)
        {
        }

        public Passport Passport()
        {
            Passport objPassport = null;
            RecordRef objRole = new RecordRef();

            try
            {
                objPassport = new Passport();
                objPassport.account = NetSuiteAccountNumber;
                objPassport.email = NetSuiteAccountEmail;
                objRole = new RecordRef();
                objRole.internalId = NetSuiteAccountRoleID;
                objPassport.role = objRole;
                objPassport.password = NetSuiteAccountPassword;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }

            return objPassport;
        }

        public TokenPassport TokenPassport()
        {
            //string scompid = "ACCT88641";//account id

            //string consumerkey = "01099c3ecb51fa1578e5cf1e71d98ff117421f35ba5fd6727910227367962158"; //application consumer key
            //string consumersecret = "57a73a0586c4e8af83d4d6cfe289907ab85c83169760a87197270aab1fbe1833";

            ////string tokenid = "1795e63eb8bfb13cd4d3704abad3c6eaa0c13b82e2a9f696b6e706a81ab323d5";//user access tokenid
            ////string tokensecret = "db0832f1b1c22e54d09b1fbc59cd6fb2c0d120616a86c7cbed395e02747767aa";
            ////raj
            ////string tokenid = "478c3edae1e185ddc035963ef3054d5d9a07b214eb5b4ff3169612d2509fb9e9";//user access tokenid
            ////string tokensecret = "3953e6ebbd3ac61079212461bd9d872fdd19de57dbc89cad5048525d727ead71";
            //string tokenid = "487eba5a361b25f56893c83f351841e30ebc8edb5d5dabe30a311b074f62368e";//user access tokenid
            //string tokensecret = "9e1d79d460cf265d8de6e87212f0c19fc7dca054bd9643b51c563ef70e939496";

            string scompid = NetSuiteAccountNumber;//account id
            string consumerkey = AppConsumerKey; //application consumer key
            string tokenid = TokenID;//user access tokenid
            string consumersecret = AppConsumerSecret;
            string tokensecret = TokenSecret;

            // Create Passport and assign it to _service.passport
            NetSuiteLibrary.com.netsuite.webservices.TokenPassport passport = new NetSuiteLibrary.com.netsuite.webservices.TokenPassport();
            passport.account = scompid;
            passport.consumerKey = consumerkey;
            passport.token = tokenid;

            //Computing for nonce
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] data = new byte[20];
            rng.GetBytes(data);
            int value = Math.Abs(BitConverter.ToInt32(data, 0));
            string nonce = value.ToString();
            passport.nonce = nonce;

            //computing for timestamp
            long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string timestamp = unixTimestamp.ToString();
            passport.timestamp = unixTimestamp;

            //Computing for Signature
            string baseString = scompid + "&" + consumerkey + "&" + tokenid + "&" + nonce + "&" + timestamp;
            string key = consumersecret + "&" + tokensecret;
            string signature = "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(baseString);
            using (var myhmacsha1 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = myhmacsha1.ComputeHash(messageBytes);
                signature = Convert.ToBase64String(hashmessage);
            }
            // Console.WriteLine("Computed Signature is " + signature);
            NetSuiteLibrary.com.netsuite.webservices.TokenPassportSignature sign = new NetSuiteLibrary.com.netsuite.webservices.TokenPassportSignature();
            sign.algorithm = "HMAC-SHA256";
            sign.Value = signature;
            passport.signature = sign;

            return passport;
        }
    }
}
