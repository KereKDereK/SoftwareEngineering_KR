using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Crypto
    {
        internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public Dictionary<string, string> ipToKey = new Dictionary<string, string>();


        public Crypto(string path, Schedule currentSchedule, string HostIp)
        {
            int check = ParseKeysFromJson(path + "Keys.json");
            if (check == 1)
            {
                GenerateKeyPairs(currentSchedule, HostIp);
                ParseKeysToJson();
            }
        }

        private void GenerateKeyPairs(Schedule currentSchedule, string hostIp)
        {
            foreach (ConnectionPair pair in currentSchedule.ConnectionPairs)
            {
                if (pair.FirstClient == hostIp)
                    ipToKey[pair.SecondClient] = GetUniqueKey(8);
                else
                    ipToKey[pair.FirstClient] = GetUniqueKey(8);
                Console.WriteLine("[LOG] Keys successfully generated");
            }
        }

        private void ParseKeysToJson()
        {
            string fileName = "Keys.json";
            string jsonString = JsonConvert.SerializeObject(ipToKey);
            File.WriteAllText(fileName, jsonString);
            Console.WriteLine("[LOG] Keys successfully parsed to json file");
        }

        private int ParseKeysFromJson(string path)
        {
            Dictionary<string, string> tmpDict = new Dictionary<string, string>();
            try
            {
                tmpDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
            }
            catch (Exception)
            {
                Console.WriteLine("[ERROR] An error occured. Keys file does not exist");
                return 1;
            }
            Console.WriteLine("[LOG] Keys successfully parsed from json file");
            ipToKey = tmpDict;
            return 0;
        }

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string Encrypt(string message, string key)
        {
            if (key.Length != 8)
            {
                Console.WriteLine("[ERROR] Wrong key length.");
                return "[ERROR]";
            }

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            MemoryStream memStream = new MemoryStream();
            try
            {
                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                ICryptoTransform transform = provider.CreateEncryptor(passwordBytes, passwordBytes);
                CryptoStreamMode mode = CryptoStreamMode.Write;

                CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
                cryptoStream.Write(messageBytes, 0, messageBytes.Length);
                cryptoStream.FlushFinalBlock();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] An error occured while encrypting the message. Please check your keys file.");

            }

            byte[] encryptedMessageBytes = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(encryptedMessageBytes, 0, encryptedMessageBytes.Length);

            string encryptedMessage = Convert.ToBase64String(encryptedMessageBytes);

            return encryptedMessage;
        }

        public static string Decrypt(string encryptedMessage, string key)
        {
            if (key.Length != 8)
            {
                Console.WriteLine("[ERROR] Wrong key length.");
                return "[ERROR]";
            }

            encryptedMessage = encryptedMessage.Replace(" <EOF>", String.Empty);
            string newmsg = "";
            for (int i = 0; i < encryptedMessage.Length; i++)
                if (encryptedMessage[i] != '\0')
                    newmsg += encryptedMessage[i];
                else
                    break;
            MemoryStream memStream = new MemoryStream();
            try
            {
                encryptedMessage = newmsg;
                byte[] encryptedMessageBytes = Convert.FromBase64String(encryptedMessage);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

                DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
                ICryptoTransform transform = provider.CreateDecryptor(passwordBytes, passwordBytes);
                CryptoStreamMode mode = CryptoStreamMode.Write;

                CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
                cryptoStream.Write(encryptedMessageBytes, 0, encryptedMessageBytes.Length);
                cryptoStream.FlushFinalBlock();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] An error occured while decrypting the message. Please check your keys file.");
            }

            byte[] decryptedMessageBytes = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(decryptedMessageBytes, 0, decryptedMessageBytes.Length);

            string message = Encoding.UTF8.GetString(decryptedMessageBytes);

            return message;
        }
    }
}
