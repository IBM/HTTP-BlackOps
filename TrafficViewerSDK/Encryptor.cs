using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Management;

namespace TrafficViewerSDK
{
	/// <summary>
	/// Obfuscates sensitive data
	/// </summary>
	public class Encryptor
	{
        
        private static byte[] SALT = GetHashSha256Bytes(Environment.UserName);
        private static string SHARED_SECRET = GetHashSha256String(Environment.MachineName);
        private static int READ_BUFFER_SIZE = 1024 * 10;
        private static string RegistryCryptoGuid = Utils.GetRegistryKeyValueString(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Cryptography","MachineGuid");

      

        private static ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            

        public static byte[] GetHashSha256Bytes(string text)
        {
            byte[] bytes = Constants.DefaultEncoding.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);

            return hash;
        }

        public static string GetHashSha256String(string text)
        {

            byte[] hash = GetHashSha256Bytes(text);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }



		private static RijndaelManaged InitRijndael()
		{
            disk.Get();
            string volIdString = disk["VolumeSerialNumber"].ToString();
            int iterations;
            if (volIdString != null)
            {
                int volId = Convert.ToInt32(volIdString, 16);
                iterations = volId % 1000 + 1000;
            }
            else
            {
                volIdString = "XX_XX_XX_XX";
                iterations = 1234;
            }
			RijndaelManaged rijndael = new RijndaelManaged();
			Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(volIdString + SHARED_SECRET + RegistryCryptoGuid , SALT, iterations);
			rijndael.Key = key.GetBytes(rijndael.KeySize / 8);
			rijndael.IV = key.GetBytes(rijndael.BlockSize / 8);
			return rijndael;
		}

		/// <summary>
		/// Encrypts a byte array
		/// </summary>
		/// <param name="textBytes"></param>
		/// <returns></returns>
		public static string EncryptToString(byte[] textBytes)
		{
			string result = String.Empty;
            result = Convert.ToBase64String(Encrypt(textBytes));
			return result;
		}

        /// <summary>
        /// Encrypts a byte array
        /// </summary>
        /// <param name="textBytes"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] textBytes)
        {
            byte[] result = new byte[0];

            RijndaelManaged rijndael = InitRijndael();

            ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(textBytes);
                    }
                }
                result = msEncrypt.ToArray();
            }

            rijndael.Clear();

            return result;
        }


        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptToString(string text)
        {
            return EncryptToString(Constants.DefaultEncoding.GetBytes(text));
        }


        /// <summary>
        /// Decrypts to a string
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public static string DecryptToString(string encryptedString)
        {
            byte[] result = Decrypt(encryptedString);
            if (result == null)
            { 
                return String.Empty;
            }
            return Constants.DefaultEncoding.GetString(result);
        }

        /// <summary>
        /// Decrypts a from encrypted bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] bytes)
        {

            ByteArrayBuilder result = new ByteArrayBuilder();

            try
            {

                RijndaelManaged rijndael = InitRijndael();

                ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader srDecrypt = new BinaryReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            int readBytes = 0;
                            do
                            {
                                byte[] buffer = new byte[READ_BUFFER_SIZE];
                                readBytes = srDecrypt.Read(buffer, 0, READ_BUFFER_SIZE);
                                if (readBytes > 0)
                                {
                                    result.AddChunkReference(buffer, readBytes);
                                }
                            }
                            while (readBytes != 0);
                        }
                    }


                    rijndael.Clear();
                }
            }
            catch { };

            return result.ToArray();
        }


		/// <summary>
		/// Decrypts a string
		/// </summary>
		/// <param name="encryptedString"></param>
		/// <returns></returns>
		public static byte[] Decrypt(string encryptedString)
		{
            byte[] result = new byte[0];
            
            if (!String.IsNullOrEmpty(encryptedString))
            {
                result = Decrypt(Convert.FromBase64String(encryptedString));   
            }
            return result;
		}

	}
}
