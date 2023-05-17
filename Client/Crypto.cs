﻿using Newtonsoft.Json;
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
            int check = ParseKeysFromJson(path);
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

            // Encode message and password

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            // Set encryption settings -- Use password for both key and init. vector
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            ICryptoTransform transform = provider.CreateEncryptor(passwordBytes, passwordBytes);
            CryptoStreamMode mode = CryptoStreamMode.Write;

            // Set up streams and encrypt
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
            cryptoStream.Write(messageBytes, 0, messageBytes.Length);
            cryptoStream.FlushFinalBlock();

            // Read the encrypted message from the memory stream
            byte[] encryptedMessageBytes = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(encryptedMessageBytes, 0, encryptedMessageBytes.Length);

            // Encode the encrypted message as base64 string
            string encryptedMessage = Encoding.UTF8.GetString(encryptedMessageBytes);

            return encryptedMessage;
        }

        public static string Decrypt(string encryptedMessage, string key)
        {
            string newmsg = "";
            for (int i = 0; i < encryptedMessage.Length; i++)
                if (encryptedMessage[i] != '\0')
                    newmsg += encryptedMessage[i];
                else
                    break;
            while (newmsg.Length % 8 != 0)
                newmsg += "=";
            encryptedMessage = newmsg + '\0';
            byte[] encryptedMessageBytes = Encoding.UTF8.GetBytes(encryptedMessage);
            //byte[] encryptedMessageBytes = Convert.FromBase64String(encryptedMessage);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);

            // Set encryption settings -- Use password for both key and init. vector
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            ICryptoTransform transform = provider.CreateDecryptor(passwordBytes, passwordBytes);
            CryptoStreamMode mode = CryptoStreamMode.Write;

            // Set up streams and decrypt
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
            cryptoStream.Write(encryptedMessageBytes, 0, encryptedMessageBytes.Length);
            cryptoStream.FlushFinalBlock();

            // Read decrypted message from memory stream
            byte[] decryptedMessageBytes = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(decryptedMessageBytes, 0, decryptedMessageBytes.Length);

            // Encode deencrypted binary data to base64 string
            string message = Encoding.UTF8.GetString(decryptedMessageBytes);

            return message;
        }
    }
}
