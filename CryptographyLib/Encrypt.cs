using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptographyLib
{
    public class Encrypt
    {
        public static string EncMD5(string password)
        {
            //From https://www.c-sharpcorner.com/forums/how-to-create-md5-password-encryption-in-mvc

            return BitConverter.ToString(new MD5CryptoServiceProvider()
                .ComputeHash(new UTF8Encoding().GetBytes(password)))
                .Replace("-", "")
                .ToLower();  //32chars
        }
    }
}
