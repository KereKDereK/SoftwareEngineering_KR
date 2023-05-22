using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class AESDecryptor
    {
        private static byte[] GetIV(string ivSecret)
        {
            using MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(ivSecret));
        }
        private static byte[] GetKey(string key)
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }
        private static void DecryptFile(string sourceFile, string destFile, byte[] key, byte[] iv)
        {
            using FileStream fileStream = new(sourceFile, FileMode.Open);
            using Aes aes = Aes.Create();

            aes.IV = iv;

            using CryptoStream cryptoStream = new(fileStream,
                                       aes.CreateDecryptor(key, iv),
                                                  CryptoStreamMode.Read); //создаем поток для чтения (расшифровки) данных
            using FileStream outStream = new FileStream(destFile, FileMode.Create);//создаем поток для расшифрованных данных

            using BinaryReader decryptReader = new(cryptoStream);
            int tempSize = 10;  //размер временного хранилища
            byte[] data;        //временное хранилище для зашифрованной информации
            while (true)
            {
                data = decryptReader.ReadBytes(tempSize);
                if (data.Length == 0)
                    break;
                outStream.Write(data, 0, data.Length);
            }
        }

        public void DecryptShedule() 
        {
            string outputFileName = "Schedule.enc"; //файл, который будет содержать зашифрованные данные
            string key = "секретный ключ"; //ключ для шифрования
            string ivSecret = "вектор"; //вектор инициализации
            DecryptFile(outputFileName, "Schedule.json", GetKey(key), GetIV(ivSecret));
        }
    }
}
