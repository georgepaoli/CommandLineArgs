using CommandLineArgs;
using System;
using System.IO;
using System.Net.Http;

namespace Download
{
    [DefaultCommand("Download")]
    public class Downloader
    {
        // TODO: Fix usage so that PopArg is displayed nicer and aliases are not so verbosse
        [Required]
        [PopArg]
        public string Url;

        [Required]
        [PopArg]
        [Alias("-o")]
        public string OutputPath;

        public void Download()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(Url).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Download failed.");
                }

                File.WriteAllBytes(OutputPath, response.Content.ReadAsByteArrayAsync().Result);
            }
        }
    }
}
