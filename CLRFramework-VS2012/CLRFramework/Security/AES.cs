using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public partial class Utility
{
    public partial class Security
    {
        public class AES
        {
            public static void Main()
            {
                string original = "Here is some data to encrypt!";

                string strEncrypted = EncryptToString(original, "InventoryStudio", "Password$1");
                string strDecrypted = DecryptFromString(strEncrypted, "InventoryStudio", "Password$1");


                // Create a new instance of the AesCryptoServiceProvider
                // class.  This generates a new key and initialization 
                // vector (IV).
                using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptToBytes(original, myAes.Key, myAes.IV);

                    // Decrypt the bytes to a string.
                    string decripted = DecryptFromBytes(encrypted, myAes.Key, myAes.IV);

                    //Display the original data and the decrypted data.
                    Console.WriteLine("Original:   {0}", original);
                    Console.WriteLine("Round Trip: {0}", decripted);
                }
            }

            public static string EncryptToString(string TextToEncode, string Password, string PasswordSalt)
            {
                byte[] key = new byte[32];
                byte[] iv = new byte[16];
                GetEncryptionKeyAndIV(Password, PasswordSalt, ref key, ref iv);
                return EncryptToString(TextToEncode, key, iv);
            }

            public static string EncryptToString(string TextToEncode, byte[] Key, byte[] IV)
            {
                return Convert.ToBase64String(EncryptToBytes(TextToEncode, Key, IV));
            }

            public static byte[] EncryptToBytes(string TextToEncode, string Password, string PasswordSalt)
            {
                byte[] key = new byte[32];
                byte[] iv = new byte[16];
                GetEncryptionKeyAndIV(Password, PasswordSalt, ref key, ref iv);
                return EncryptToBytes(TextToEncode, key, iv);
            }

            public static byte[] EncryptToBytes(string TextToEncode, byte[] Key, byte[] IV)
            {
                // Check arguments.
                if (TextToEncode == null || TextToEncode.Length <= 0)
                    throw new ArgumentNullException("TextToEncode");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");
                byte[] encrypted;
                // Create an AesCryptoServiceProvider object
                // with the specified key and IV.
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(TextToEncode);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }
                }
                // Return the encrypted bytes from the memory stream.
                return encrypted;
            }

            public static string DecryptFromString(string StringToDecode, string Password, string PasswordSalt)
            {
                if (string.IsNullOrEmpty(StringToDecode)) return string.Empty;

                byte[] key = new byte[32];
                byte[] iv = new byte[16];
                GetEncryptionKeyAndIV(Password, PasswordSalt, ref key, ref iv);
                return DecryptFromString(StringToDecode, key, iv);
            }

            public static string DecryptFromString(string StringToDecode, byte[] Key, byte[] IV)
            {
                return DecryptFromBytes(Convert.FromBase64String(StringToDecode), Key, IV);
            }

            public static string DecryptFromBytes(byte[] BytesToDecode, string Password, string PasswordSalt)
            {
                byte[] key = new byte[32];
                byte[] iv = new byte[16];
                GetEncryptionKeyAndIV(Password, PasswordSalt, ref key, ref iv);
                return DecryptFromBytes(BytesToDecode, key, iv);
            }

            public static string DecryptFromBytes(byte[] BytesToDecode, byte[] Key, byte[] IV)
            {
                // Check arguments.
                if (BytesToDecode == null || BytesToDecode.Length <= 0)
                    throw new ArgumentNullException("cipherText");
                if (Key == null || Key.Length <= 0)
                    throw new ArgumentNullException("Key");
                if (IV == null || IV.Length <= 0)
                    throw new ArgumentNullException("IV");

                // Declare the string used to hold
                // the decrypted text.
                string plaintext = null;

                // Create an AesCryptoServiceProvider object
                // with the specified key and IV.
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(BytesToDecode))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {

                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }

            private static bool GetEncryptionKeyAndIV(string Password, string PasswordSalt, ref byte[] Key, ref byte[] IV)
            {
                byte[] salt = Encoding.Default.GetBytes(PasswordSalt);
                Rfc2898DeriveBytes keyGenerator = new Rfc2898DeriveBytes(Password, salt);
                Key = keyGenerator.GetBytes(32);
                IV = keyGenerator.GetBytes(16);
                return true;
            }
        }
    }
}