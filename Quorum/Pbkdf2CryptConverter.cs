using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public static class Pbkdf2CryptConverter
    {
        public static string Encrypt(string password)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, 16, 10000);
            var bytes = pbkdf2.GetBytes(20); // .NET uses SHA1, 160 bits

            string formatted = string.Format(
                "$pbkdf2${0}${1}${2}",
                pbkdf2.IterationCount,
                pbkdf2.Salt.ToBase64(),
                bytes.ToBase64());

            return formatted;
        }

        public static bool Compare(string crypt, string password)
        {
            var parts = crypt.Split(new[] { '$' }, StringSplitOptions.RemoveEmptyEntries);

            switch(parts[0])
            {
                case "pbkdf2":
                    var pbkdf2 = new Rfc2898DeriveBytes(password, parts[2].FromBase64(), int.Parse(parts[1]));
                    return pbkdf2.GetBytes(20).SequenceEqual(parts[3].FromBase64());
                default:
                    throw new Exception("Unknown cipher " + parts[0]);
            }
        }

        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] FromBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }
    }
}
