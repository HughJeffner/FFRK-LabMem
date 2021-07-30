using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            httpClient.DefaultRequestHeaders.Add("User-Agent", "FFRK-LabMem UpdateChecker 1.0");

        }

        public static void Check(String user, String repo, bool includePreRelease, String versionCode)
        {
            
            ColorConsole.WriteLine(ConsoleColor.DarkYellow, "Checking for newer releases... disable in app.config");
            var checkerTask = Task.Run(async () =>
            {
                var checker = new Updates(user, repo, includePreRelease);
                try
                {
                    if (await checker.IsLatestRelease(versionCode))
                    {
                        ColorConsole.WriteLine(ConsoleColor.DarkYellow, "A new version of FFRK-LabMem has been released. Go to " + WEB_URL + " by pressing [Alt+U] to get it!", user, repo);
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

        public async Task<bool> IsLatestRelease(string version)
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
            return semVersion < latestRelease.Value;
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

        private async Task<KeyValuePair<string, SemVersion>> GetLatestRelease()
        {
            var releases = await GetReleasesAsync();
            var latestRelease = releases.First();

            foreach (var release in releases)
                if (SemVersion.Compare(release.Value, latestRelease.Value) > 0)
                    latestRelease = release;

            return latestRelease;
        }

        private async Task<Dictionary<string, SemVersion>> GetReleasesAsync()
        {
            var pageNumber = "1";
            var releases = new Dictionary<string, SemVersion>();
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
                        releases.Add(releaseId, semVersion);
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

    }
}
