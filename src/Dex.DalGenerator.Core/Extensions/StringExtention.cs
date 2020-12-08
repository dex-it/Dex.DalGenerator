using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dex.DalGenerator.Core.Extensions
{
    public static class StringExtention
    {
        public static string ReplaceRegex(this string source, string pattern, string replacement)
        {
            var regex = new Regex(pattern);
            return regex.Replace(source, replacement);
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        public static byte[] GetMd5Hash(this string value, Encoding encoding)
        {
            var md5 = MD5.Create();

            return md5.ComputeHash(encoding.GetBytes(value));
        }

        public static byte[] GetSha256Hash(this string value, Encoding encoding)
        {
            var sha256 = SHA256.Create();

            return sha256.ComputeHash(encoding.GetBytes(value));
        }

        public static byte[] GetSha1Hash(this string value, Encoding encoding)
        {
            var sha1 = SHA1.Create();

            return sha1.ComputeHash(encoding.GetBytes(value));
        }

        private static string GetStringFromHash(IEnumerable<byte> hash)
        {
            var sb = new StringBuilder();

            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string GetMd5HashString(this string value, Encoding encoding)
        {
            var hash = GetMd5Hash(value, encoding);

            return GetStringFromHash(hash);
        }

        public static string GetSha256HashString(this string value, Encoding encoding)
        {
            var hash = GetSha256Hash(value, encoding);

            return GetStringFromHash(hash);
        }

        public static string GetSha1HashString(this string value, Encoding encoding)
        {
            var hash = GetSha1Hash(value, encoding);

            return GetStringFromHash(hash);
        }

        public static string GetMd5HashString(this string value)
        {
            return GetMd5HashString(value, Encoding.UTF8);
        }

        public static string GetSha256HashString(this string value)
        {
            return GetSha256HashString(value, Encoding.UTF8);
        }

        public static string GetSha1HashString(this string value)
        {
            return GetSha1HashString(value, Encoding.UTF8);
        }

        public static Type ResolveTypeFromDomain(this string typeName, string assembltFileName = null)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => x.GetType(typeName))
                .FirstOrDefault(x => x != null);

            if (type == null && !string.IsNullOrEmpty(assembltFileName))
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembltFileName);
                var assembly = Assembly.LoadFile(path);
                if (assembly == null) throw new DllNotFoundException("ResolveTypeFromDomain: " + path);

                type = assembly.GetType(typeName);
                if (type != null) return type;
            }

            if (type == null)
                throw new TypeLoadException(string.Format("Can't resolve type '{0}' from domain", typeName));

            return type;
        }
    }
}