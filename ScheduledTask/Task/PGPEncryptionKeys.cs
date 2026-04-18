using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ScheduledTask.Task
{
    public class PGPEncryptionKeys
    {
        public PgpPublicKey PublicKey { get; set; }
        public PgpPrivateKey PrivateKey { get; set; }
        public PgpSecretKey SecretKey { get; set; }

        public PGPEncryptionKeys(string publicKeyPath, string privateKeyPath, string passPhrase)
        {
            if (!File.Exists(publicKeyPath))
                throw new ArgumentException("Public key file not found", "publicKeyPath");
            if (!File.Exists(privateKeyPath))
                throw new ArgumentException("Private key file not found", "privateKeyPath");
            if (String.IsNullOrEmpty(passPhrase))
                throw new ArgumentException("passPhrase is null or empty.", "passPhrase");

            //PublicKey = ReadPublicKey(publicKeyPath);
            //PrivateKey = ReadPrivateKey(privateKeyPath);
            //SecretKey = ReadSecretKey(passPhrase);

            PublicKey = ReadPublicKey(publicKeyPath);
            SecretKey = ReadSecretKey(privateKeyPath);
            PrivateKey = ReadPrivateKey(passPhrase);
        }

        private PgpSecretKey ReadSecretKey(string privateKeyPath)
        {
            using (Stream keyIn = File.OpenRead(privateKeyPath))
            {
                using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
                {
                    PgpSecretKeyRingBundle secretKeyRingBundle = new PgpSecretKeyRingBundle(inputStream);
                    PgpSecretKey foundKey = GetFirstSecretKey(secretKeyRingBundle);
                    if (foundKey != null)
                        return foundKey;
                }
            }
            throw new ArgumentException("Can’t find signing key in key ring.");
        }

        private PgpSecretKey GetFirstSecretKey(PgpSecretKeyRingBundle secretKeyRingBundle)
        {
            foreach (PgpSecretKeyRing kRing in secretKeyRingBundle.GetKeyRings())
            {
                PgpSecretKey key = kRing.GetSecretKeys()
                .Cast<PgpSecretKey>()
                .Where(k => k.IsSigningKey)
                .FirstOrDefault();

                if (key != null)
                    return key;
            }

            return null;
        }

        private PgpPublicKey ReadPublicKey(string publicKeyPath)
        {
            using (Stream keyIn = File.OpenRead(publicKeyPath))
            {
                using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
                {
                    PgpPublicKeyRingBundle publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);
                    PgpPublicKey foundKey = GetFirstPublicKey(publicKeyRingBundle);
                    if (foundKey != null)
                        return foundKey;
                }
            }
            throw new ArgumentException("No encryption key found in public key ring.");
        }
        private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
        {
            foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
            {
                PgpPublicKey key = kRing.GetPublicKeys()
                .Cast<PgpPublicKey>()
                .Where(k => k.IsEncryptionKey)
                .FirstOrDefault();
                if (key != null)
                    return key;
            }
            return null;
        }

        private PgpPrivateKey ReadPrivateKey(string passPhrase)
        {
            PgpPrivateKey privateKey = SecretKey.ExtractPrivateKey(passPhrase.ToCharArray());
            if (privateKey != null)
                return privateKey;
            throw new ArgumentException("No private key found in secret key.");
        }
    }
}
