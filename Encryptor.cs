using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace TestProj
{
    public class Encryptor
    {
        public static string Hash(string password)
        { 
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(password))
                );
        }
    }
}