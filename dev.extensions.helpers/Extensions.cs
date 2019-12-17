
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace connectjs.dev.extensions
{
    public static class Extensions
    {
        public static string ToDDMMYYYY(this DateTime dt)
        {
            return dt.ToString("dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string ToMMDDYYYY(this DateTime dt)
        {
            return dt.ToString("MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static bool IsNullOrEmpty(this String s)
        {
            return String.IsNullOrEmpty(s);
        }

        public static bool IsNotNullAndWhiteSpace(this string str, int minLength = 1)
        {
            return !string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str) && (str ?? "").Length >= minLength;
        }

        public static string IsNotNull(this string str, int minLength = 1)
        {
            if (str.IsNotNullAndWhiteSpace(minLength)) return str;
            return null;
        }

        public static string Join(this string[] data, string separater)
        {
            return string.Join(separater, data);
        }

        public static string Join(this int[] data, string separater)
        {
            return string.Join(separater, data);
        }

        public static string Join(this IEnumerable<string> data, string separater)
        {
            return string.Join(separater, data);
        }

        public static string Join(this IEnumerable<int> data, string separater)
        {
            return string.Join(separater, data);
        }

        public static byte[] GetBytes(this string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        public static byte[] GetBytes(this object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static byte[] GetBytesFromBase64(this string data)
        {
            return Convert.FromBase64String(data);
        }

        public static string GetBase64(this string data)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public static string GetString(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static DateTime ToDateTime(this string dt, string format = null)
        {
            if (dt.IsNotNullAndWhiteSpace())
            {
                if (format == null)
                {
                    if (DateTime.TryParse(dt, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var _dt)) return _dt;
                    else if (DateTime.TryParseExact(dt, "MM/dd/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var __dt)) return __dt;
                    else
                        throw new Exception("Invalid Date Time format - " + dt);
                }


                if (format != null && DateTime.TryParseExact(dt, format,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var output))
                    return output;
            }

            return DateTime.MinValue;
        }

        public static long ToEpoch(this string dt, string format)
        {
            return (long)ToDateTime(dt, format).Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long ToEpoch(this DateTime dt)
        {
            return (long)dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }



        public static string Compress(this string uncompressedString)
        {
            byte[] compressedBytes;

            using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
            {
                using (var compressedStream = new MemoryStream())
                {
                    using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
                    {
                        uncompressedStream.CopyTo(compressorStream);
                    }
                    compressedBytes = compressedStream.ToArray();
                }
            }

            return Convert.ToBase64String(compressedBytes);
        }

        public static string Decompress(this string compressedString)
        {
            byte[] decompressedBytes;

            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    decompressorStream.CopyTo(decompressedStream);

                    decompressedBytes = decompressedStream.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }

        public static JObject ToJson(this Dictionary<string, string> data)
        {
            return JObject.Parse(Serialize(data));
        }

        public static JObject ToJson<T>(this Dictionary<string, T> data)
        {
            return JObject.Parse(Serialize(data));
        }


        public static T ToObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        }

        public static T ParseObject<T>(this JObject value)
        {
            return value.ToObject<T>(new JsonSerializer { DateParseHandling = DateParseHandling.None });
        }

        public static object GetPropertyValue<T>(this T input, string property)
        {
            var prop = input.GetType().GetProperties().FirstOrDefault(m => m.Name == property);
            if (prop != null)
                return prop.GetValue(input);
            return null;
        }

        public static object GetPropertyValue<T>(this object input, string property)
        {
            var prop = input.GetType().GetProperties().FirstOrDefault(m => m.Name == property);
            if (prop != null)
                return prop.GetValue(input);
            return null;
        }

        public static DateTime ToIndianTime(this System.DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(5).AddMinutes(30);
        }

        public static string AddVariableValue(this string input, string variable, string value)
        {
            return input.Replace("{" + variable + "}", value);
        }

        public static string ClipSegments(this string input, string start, string end)
        {
            var res = Regex.Replace(input, $"{start}(.|\n)*?{end}", string.Empty);
            res = Regex.Replace(input, $"{start.ToUpper()}(.|\n)*?{end.ToUpper()}", string.Empty);
            return res;
        }

        public static string Serialize(this object input)
        {
            return JsonConvert.SerializeObject(input);
        }

        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T Deserialize<T>(this string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static T DeserializeXml<T>(this string input) where T : class
        {
            var reader = XmlReader.Create(input.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }

        public static T DeserializeUrlEncoded<T>(this string input)
        {
            var items = input.Split('&').ToDictionary(m => m.Split('=')[0].Trim(), m => HttpUtility.UrlDecode(m.Split('=')[1]));
            return ToJson(items).ToObject<T>();
        }

        public static bool IsBool(this PropertyInfo prop)
        {
            return prop.PropertyType.Equals(typeof(bool)) || prop.PropertyType.Equals(typeof(bool?));
        }

        public static bool IsString(this PropertyInfo prop)
        {
            return prop.PropertyType.Equals(typeof(string));
        }

        public static bool IsDecimal(this PropertyInfo prop)
        {
            return prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(decimal?));
        }

        public static bool IsInt(this PropertyInfo prop)
        {
            return prop.PropertyType.Equals(typeof(int)) || prop.PropertyType.Equals(typeof(int?));
        }

        public static bool IsDouble(this PropertyInfo prop)
        {
            return prop.PropertyType.Equals(typeof(double)) || prop.PropertyType.Equals(typeof(double?));
        }

        public static TDestination Map<TSource, TDestination>(this TSource source, bool excludeNulls = true)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => excludeNulls && srcMember != null));
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<TDestination>(source);
        }

        public static TDestination NativeMap<TSource, TDestination>(this TSource source)
        {
            var obj = (TDestination)Activator.CreateInstance(typeof(TDestination));
            if (obj != null)
            {
                source.GetType().GetProperties().Where(m => m.CanRead).ToList().ForEach(prop =>
                {
                    var prop2 = obj.GetType().GetProperty(prop.Name);
                    if (prop2 != null && prop2.CanWrite && prop2.PropertyType == prop.PropertyType)
                        prop2.SetValue(obj, prop.GetValue(source));
                });
            }
            return obj;
        }

        public static void ForEachItem<T>(this List<T> source, Action<T, int> predicate)
        {
            int count = 0;
            source.ForEach(m =>
            {
                predicate.Invoke(m, count);
                count++;
            });
        }

        public static void ForEachItem<T>(this IEnumerable<T> source, Action<T, int> predicate)
        {
            ForEachItem(source.ToList(), predicate);
        }

        public static List<T> AddItem<T>(this List<T> @this, T item)
        {
            @this.Add(item);
            return @this;
        }

        public static IEnumerable<T> AddItem<T>(this IEnumerable<T> @this, T item)
        {
            return @this.ToList().AddItem(item);
        }

        public static T[] AddItem<T>(this T[] @this, T item)
        {
            return @this.ToList().AddItem(item).ToArray();
        }

        public static int ToInt(this string value)
        {
            if (int.TryParse(value, out var output)) return output;
            throw new Exception("String invalid for conversion");
        }

        public static double ToDouble(this string value)
        {
            if (double.TryParse(value, out var output)) return output;
            throw new Exception("String invalid for conversion");
        }
    }
}
