using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CMS.IO;
using Newtonsoft.Json;
using Directory = System.IO.Directory;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static partial class HtmlHelperExtensions
    {
        public const string DistPath = "~/dist";
        public const string ManifestJsonPath = DistPath + "/manifest.json";
        public const string MonolithManifestJsonPath = DistPath + "/monolith.manifest.json";

        private static readonly object ManifestLock = new object();
        private static Dictionary<string, string> _manifest;

        public static string ManifestHash { get; private set; } = string.Empty;
        public static Dictionary<string, string> Manifest
        {
            get
            {
#if !_NOCACHE
                if (_manifest == null)
#endif
                {
                    lock (ManifestLock)
                    {
                        _manifest = new Dictionary<string, string>();
                        try
                        {
                            var strBuilder = new StringBuilder();
                            foreach (var fileName in new List<string> { ManifestJsonPath, MonolithManifestJsonPath })
                            {
                                var path = HttpContext.Current.Server.MapPath(fileName);
                                if (!File.Exists(path)) continue;
                                var fileContent = File.ReadAllText(path);
                                strBuilder.Append(fileContent);
                                var tempManifest =
                                    JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent)
                                    ?? new Dictionary<string, string>();

                                foreach (var entry in tempManifest)
                                {
                                    if (!_manifest.ContainsKey(entry.Key))
                                    {
                                        _manifest.Add(entry.Key, entry.Value);
                                    }
                                }
                            }

                            using (var hash = SHA1.Create())
                            {
                                var bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(strBuilder.ToString()));
                                ManifestHash = Convert.ToBase64String(bytes);
                            }
                        }
                        finally
                        {
                            _manifest = _manifest ?? new Dictionary<string, string>();
                        }
                    }
                }

                return _manifest;
            }
        }

        public static string GetWebPath(string manifestKey)
        {
            return GetFullPathsFromManifest(new[] { manifestKey }).FirstOrDefault();
        }

        public static string GetWebPath(this HtmlHelper htmlHelper, string manifestKey)
        {
            return GetWebPath(manifestKey);
        }

        private static IEnumerable<string> GetFullPathsFromManifest(string[] manifestKeys)
        {
            return manifestKeys.Select(GetFullPathFromManifest).ToArray();
        }

        private static string GetFullPathFromManifest(string manifestKey)
        {
            return Manifest.GetValueOrDefault(manifestKey).OrDefault(manifestKey);
        }


        //ToDo: cache
        public static string GetContent(string manifestKey, string prefix = "")
        {

            var distServerPath = HttpContext.Current.Server.MapPath(DistPath);
            if (string.IsNullOrEmpty(distServerPath) || !Directory.Exists(distServerPath))
                return null;

            var fullPath = GetFullPathFromManifest(manifestKey);
            if (!string.IsNullOrEmpty(fullPath))
            {
                var processedValue = fullPath.Replace($"{DistPath}/", string.Empty);
                var fullFilePath = distServerPath + "\\" + prefix + processedValue;
                if (System.IO.File.Exists(fullFilePath))
                {
                    return System.IO.File.ReadAllText(fullFilePath);
                }
            }

            return string.Empty;
        }

    }
}
