using connectjs.dev.extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace connectjs.dev.helpers
{
    public enum PhoneCountry
    {
        India,
        US,
        Canada
    }

    public class Utils
    {
        public static void LogMessage(object message, string ident = null, string ext = "txt")
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", (ident ?? "log") + "." + ext);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.AppendAllText(path, JsonConvert.SerializeObject(message));
            File.AppendAllText(path, Environment.NewLine);
        }

        static DateTime _epoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string Serialize<T>(T data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        public static T DeSerialize<T>(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
        }

        public static long Epoch => ((long)DateTime.UtcNow.Subtract(_epoch).TotalSeconds);

        public static string TimeSnapshot => DateTime.UtcNow.ToString("yyyy-dd-MM HHmmss", CultureInfo.InvariantCulture);

        public static DateTime FromEpoch(long epochSeconds)
        {
            return _epoch.AddSeconds(epochSeconds);
        }

        public static double NumberMap(double h1, double h2, double l1, double l2, double HValue)
        {
            return l1 + ((l2 - l1) * ((HValue - h1) / (h2 - h1)));
        }

        public static DateTime IndianTime => DateTime.UtcNow.AddHours(5).AddMinutes(30);
    }

    public class TSEqualityComparer<T> : IEqualityComparer<T>
    {
        string property;
        public TSEqualityComparer(string property)
        {
            this.property = property;
        }
        public bool Equals(T x, T y)
        {
            return x.GetPropertyValue(property) == y.GetPropertyValue(property);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetPropertyValue(property).GetHashCode();
        }
    }

    public static class Json
    {
        public static JObject Convert(object schema)
        {
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(schema));
        }

        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string Serialize(object schema)
        {
            if (!schema.GetType().Equals(typeof(string)))
                return JsonConvert.SerializeObject(schema);
            else
                return schema.ToString();
        }
    }


    public class Validators
    {
        public static bool ValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return Regex.IsMatch(email, "^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+[.])+[a-z]{2,5}$");
        }

        public static string ParsePhoneNo(string phone)
        {
            return string.IsNullOrEmpty(phone) || (!string.IsNullOrEmpty(phone) && phone.Length < 10) ? null :
                phone.Substring(phone.Length - 10, 10);
        }

        public static bool ValidPhone(string phone, PhoneCountry country)
        {
            if (string.IsNullOrEmpty(phone)) return false;

            switch (country)
            {
                case PhoneCountry.India:
                    phone = ParsePhoneNo(phone);
                    phone = "+91" + phone;
                    return Regex.IsMatch(phone, @"^(0|\+91)?[56789]\d{9}$");
            }
            return false;
        }

        public static bool IsEmailOrPhone(string username)
        {
            return ValidEmail(username) || ValidPhone(username, PhoneCountry.India);
        }
    }

    public class Encryption
    {
        public static string EncryptString(string Input, string passphrase)
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });

            try
            {
                using (var aes = new AesManaged())
                {
                    var encrypted = Encrypt(Input, pdb.GetBytes(32), pdb.GetBytes(16));
                    return encrypted.GetString();
                }
            }
            catch (Exception exp)
            {

            }
            return string.Empty;
        }
        public static string DecryptString(string Input, string passphrase)
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passphrase, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });

            try
            {
                using (var aes = new AesManaged())
                {
                    var encrypted = Decrypt(Input.GetBytesFromBase64(), pdb.GetBytes(32), pdb.GetBytes(16));
                    return encrypted;
                }
            }
            catch (Exception exp)
            {

            }
            return string.Empty;
        }
        private static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            using (var aes = new AesManaged())
            {
                var encryptor = aes.CreateEncryptor(Key, IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            return encrypted;
        }
        private static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            using (var aes = new AesManaged())
            {
                var decryptor = aes.CreateDecryptor(Key, IV);
                using (var ms = new MemoryStream(cipherText))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
