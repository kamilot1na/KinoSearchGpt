using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.OpenSubtitles;
using MovieCollection.OpenSubtitles.Models;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using SubtitleService.Models;

namespace SubtitleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubtitleController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public static readonly OpenSubtitlesOptions options = new OpenSubtitlesOptions
        {
            ApiKey = "4jolZUk6HWzxuIfuN7jiWSJs3AYQ6SUE",
            ProductInformation = new ProductHeaderValue("kino-search", "0.1")
        };
        public static readonly OpenSubtitlesService service = new OpenSubtitlesService(_httpClient, options);

        [HttpGet]
        [Route("Subtitle")]
        public async Task<IActionResult> GetSubtitle(string filmName, string phrase)
        {
            var fileId = await getFileIdAsync(filmName);
            var link = await getSubDownloadLinkAsync(fileId);
            if (link == null)
            {
                return Ok("По вашему запросу ничего не найдено");
            }
            var timeCodes = await FindPhraseTimeAsync(link, phrase);
            var result = "";
            timeCodes.ForEach(x =>
            {
                result += x + ";\n";
            });
            return Ok(result);
        }

        [NonAction]
        public async Task<Uri?> getSubDownloadLinkAsync(int fileId)
        {
            var download = new NewDownload
            {
                FileId = fileId,
            };
            var result = await service.GetSubtitleForDownloadAsync(download);
            var link = result.Link;
            return link;
        }

        [NonAction]
        public async Task<int> getFileIdAsync(string filmName)
        {
            var search = new NewSubtitleSearch
            {
                Query = filmName,
                Languages = new[] { "ru" }
            };


            var result = await service.SearchSubtitlesAsync(search);
            var data = result.Data?.FirstOrDefault()?.Attributes?.Files?.FirstOrDefault();
            return (data != null ? data.FileId : 0);
        }

        [NonAction]
        public async Task<List<string>> FindPhraseTimeAsync(Uri fileUri, string phrase)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(fileUri);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();

                var timeCodes = new List<string>();

                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                //O(N)
                for (int i = 1; i < lines.Length - 1; i++)
                {
                    if (lines[i].Contains(phrase))
                    {
                        int j = i - 1;
                        //O(1)
                        while (!IsTimeCode(lines[j]))
                        {
                            j--;
                        }
                        timeCodes.Add(lines[j]);
                    }
                }

                return timeCodes;
            }
            bool IsTimeCode(string input)
            {
                string pattern = @"\d{2}:\d{2}:\d{2},\d{3}\s-->\s\d{2}:\d{2}:\d{2},\d{3}";
                return Regex.IsMatch(input, pattern);
            }
        }
    }
}
