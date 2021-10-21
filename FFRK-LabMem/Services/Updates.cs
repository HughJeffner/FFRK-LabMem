using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FFRK_Machines;
using Newtonsoft.Json.Linq;
using Semver;

namespace FFRK_LabMem.Services
{
    class Updates
    {

        private String Endpoint { get; set; }
        private Boolean IncludePreRelease { get; set; }
        private HttpClient httpClient;
        private const String API_URL = "https://api.github.com/repos/{0}/{1}/releases";
        private const String WEB_URL = "https://github.com/{0}/{1}/releases";

        public Updates(String user, String repo, bool includePreRelease)
        {

            this.Endpoint = string.Format(API_URL, user, repo);
            this.IncludePreRelease = includePreRelease;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", String.Format("{0} {1}", GetName(), GetVersionCode("")));

        }

        public static String GetVersionCode(String preRelease)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var suffix = (String.IsNullOrEmpty(preRelease))?"":"-" + preRelease;
            return string.Format("v{0}.{1}.{2}{3}", version.Major, version.Minor, version.Build, suffix);
        }

        public static String GetName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }

        public static void Check(String user, String repo, bool includePreRelease, String versionCode)
        {
            
            ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Checking for newer releases...");
            var checkerTask = Task.Run(async () =>
            {
                var checker = new Updates(user, repo, includePreRelease);
                try
                {
                    if (await checker.IsReleaseAvailable(versionCode))
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkYellow, "A new version of FFRK-LabMem has been released. Go to " + WEB_URL + " or press [Alt+U] to get it!", user, repo);
                    }
                }
                catch (Exception e)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Failed to check for new version: {0}", e.Message);
                }
            });

        }

        public static void OpenReleasesInBrowser(string user, string repo)
        {
            var url = String.Format(WEB_URL, user, repo);
            System.Diagnostics.Process.Start("explorer", url);
        }

        public static void DownloadInstallerAndRun(string user, string repo, bool includePreRelease)
        {

            ColorConsole.Write(ConsoleColor.DarkYellow, "You are about to download and install from an external website, are you sure (Y/N):");
            var key = Console.ReadKey().Key;
            ColorConsole.WriteLine("");
            if (key != ConsoleKey.Y) return;

            var updaterTask = Task.Run(async () =>
            {
                var checker = new Updates(user, repo, includePreRelease);
                try
                {
                    var latestRelease = await checker.GetLatestRelease();

                    if (String.IsNullOrEmpty(latestRelease.InstallerUrl))
                    {
                        ColorConsole.Write(ConsoleColor.DarkYellow, "Latest release has no installer!");
                        return;
                    }

                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Downloading installer: {0}", latestRelease.InstallerName);
                    WebClient client = new WebClient();
                    client.DownloadFile(latestRelease.InstallerUrl, latestRelease.InstallerName);
                    
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Starting installer and exiting");
                    System.Diagnostics.Process.Start(latestRelease.InstallerName, "/SILENT");
                    Environment.Exit(0);

                }
                catch (Exception e)
                {
                    ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Failed to download new version: {0}", e.Message);
                }
            });


        }

        public async Task<bool> IsReleaseAvailable(string version)
        {
            SemVersion semVersion;
            try
            {
                semVersion = SemVersion.Parse(CleanVersion(version));
            }
            catch (Exception)
            {
                throw new Exception("Invalid version.");
            }

            var latestRelease = await GetLatestRelease();
            if (latestRelease.Version == null) return false;
            return semVersion < latestRelease.Version;
        }

        private static string CleanVersion(string version)
        {
            var cleanedVersion = version;
            cleanedVersion = cleanedVersion.StartsWith("v") ? cleanedVersion.Substring(1) : cleanedVersion;
            var buildDelimiterIndex = cleanedVersion.LastIndexOf("+", StringComparison.Ordinal);
            cleanedVersion = buildDelimiterIndex > 0
                ? cleanedVersion.Substring(0, buildDelimiterIndex)
                : cleanedVersion;
            return cleanedVersion;
        }

        private async Task<ReleaseInfo> GetLatestRelease()
        {
            var releases = await GetReleasesAsync();
            var latestRelease = releases.FirstOrDefault();

            foreach (var release in releases)
                if (SemVersion.Compare(release.Version, latestRelease.Version) > 0)
                    latestRelease = release;

            return latestRelease;
        }

        private async Task<List<ReleaseInfo>> GetReleasesAsync()
        {
            var pageNumber = "1";
            var releases = new List<ReleaseInfo>();
            while (pageNumber != null)
            {
                var response = await httpClient.GetAsync(new Uri(this.Endpoint + "?page=" + pageNumber));
                var contentJson = await response.Content.ReadAsStringAsync();
                VerifyGitHubAPIResponse(response.StatusCode, contentJson);
                var releasesJson = JArray.Parse(contentJson);
                foreach (var releaseJson in releasesJson)
                {
                    bool preRelease = (bool)releaseJson["prerelease"];
                    if (!this.IncludePreRelease && preRelease) continue;
                    var releaseId = releaseJson["id"].ToString();
                    try
                    {
                        string tagName = releaseJson["tag_name"].ToString();
                        var version = CleanVersion(tagName);
                        var semVersion = SemVersion.Parse(version);
                        var url = "";
                        var name = "";
                        foreach (var asset in releaseJson["assets"])
                        {
                            if (asset["name"].ToString().EndsWith("-Installer.exe"))
                            {
                                name = asset["name"].ToString();
                                url = asset["browser_download_url"].ToString();
                            }
                        }

                        releases.Add(new ReleaseInfo() { Id = releaseId, Version = semVersion, InstallerUrl = url, InstallerName = name});
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                pageNumber = GetNextPageNumber(response.Headers);
            }

            return releases;
        }

        private static void VerifyGitHubAPIResponse(HttpStatusCode statusCode, string content)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Forbidden:
                    if (content.Contains("API rate limit exceeded")) throw new Exception("GitHub API rate limit exceeded.");
                    break;
                case HttpStatusCode.NotFound:
                    if (content.Contains("Not Found")) throw new Exception("GitHub Repo not found.");
                    break;
                default:
                {
                    if (statusCode != HttpStatusCode.OK) throw new Exception("GitHub API call failed.");
                    break;
                }
            }
        }

        private static string GetNextPageNumber(HttpResponseHeaders headers)
        {
            string linkHeader;
            try
            {
                linkHeader = headers.GetValues("Link").FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(linkHeader)) return null;
            var links = linkHeader.Split(',');
            return !links.Any()
                ? null
                : (
                    from link in links
                    where link.Contains(@"rel=""next""")
                    select Regex.Match(link, "(?<=page=)(.*)(?=>;)").Value).FirstOrDefault();
        }

        private class ReleaseInfo
        {
            public string Id { get; set; }
            public SemVersion Version { get; set; }
            public string InstallerUrl { get; set; }
            public string InstallerName { get; set; }
        }

    }
}
