using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using Newtonsoft.Json;

namespace DeleteBoilerplate.Infrastructure.Extensions
{
    public static partial class HtmlHelperExtensions
    {
        public static string DistPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_distPath))
                    _distPath = HttpContext.Current.Server.MapPath(Path.Combine(HostingEnvironment.ApplicationVirtualPath, "dist"));

                return _distPath;
            }
        }

        private static string _distPath;

        public static bool ApplicationHasAlias => HostingEnvironment.ApplicationVirtualPath != null && HostingEnvironment.ApplicationVirtualPath != "/";

        public static string ApplicationAlias => HostingEnvironment.ApplicationVirtualPath;

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
                            foreach (var fileName in new List<string> { "manifest.json", "monolith.manifest.json" })
                            {
                                var path = Path.Combine(DistPath, fileName);

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
                                        _manifest.Add(entry.Key, ApplicationHasAlias ? $"{ApplicationAlias}{entry.Value}" : entry.Value);
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

        public static string GetContent(string manifestKey)
        {
            var result = string.Empty;

            using (var cs = new CachedSection<string>(ref result, CacheHelper.CacheMinutes(SiteContext.CurrentSiteName), true,
                $"AssetsCache_{manifestKey}_{ManifestHash}"))
            {
                if (cs.LoadData)
                {
                    var relativePath = ApplicationHasAlias
                        ? new Regex($"{ApplicationAlias}/dist/").Replace(GetFullPathFromManifest(manifestKey), string.Empty, 1)
                        : new Regex("/dist/").Replace(GetFullPathFromManifest(manifestKey), string.Empty, 1);

                    var fullPath = Path.Combine(DistPath, relativePath);

                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        if (File.Exists(fullPath))
                        {
                            result = File.ReadAllText(fullPath);
                        }

                        cs.Data = result;
                    }
                }
            }

            return result;
        }
    }
}
