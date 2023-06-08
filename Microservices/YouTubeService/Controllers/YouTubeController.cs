using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YouTubeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YouTubeController : ControllerBase
    {
        [HttpGet]
        [Route("YouTubeUrl")]
        public async Task<IActionResult> GetYouTubeUrl(string filmName, string timeCode)
        {
            string channelName = "FILMSTER";

            var youtubeService = new Google.Apis.YouTube.v3.YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBQqV3rOBonytms8vZDeoGLO5p2y63jGBM",
                ApplicationName = "69041"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            var channelid = GetChannelIdByName(youtubeService, channelName);
            if (channelid == null)
            {
                return Ok("Название канала не найдено");
            }
            searchListRequest.ChannelId = channelid;
            searchListRequest.Q = filmName;
            searchListRequest.MaxResults = 1;

            var searchListResponse = await searchListRequest.ExecuteAsync();


            if (searchListResponse.Items.Count > 0)
            {
                var videoId = searchListResponse.Items.First().Id.VideoId;
                var videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                var videoUrlWithTimeCode = AppendTimeCodeToUrl(videoUrl, GetTimeSpanFromString(timeCode));
                return Ok(videoUrlWithTimeCode);
            }
            else
            {
                return Ok("Видео не найдено");
            }
        }
        [NonAction]
        private static TimeSpan GetTimeSpanFromString(string timeCode)
        {
            string[] timeParts = timeCode.Split(new[] { " --> " }, StringSplitOptions.None);

            // Преобразование временных значений в TimeSpan
            return TimeSpan.Parse(timeParts[0]);
        }
        [NonAction]
        private static string GetChannelIdByName(Google.Apis.YouTube.v3.YouTubeService youtubeService, string channelName)
        {
            var channelsRequest = youtubeService.Search.List("id,snippet");
            channelsRequest.Type = "channel";
            channelsRequest.Q = channelName;
            var channelsResponse = channelsRequest.Execute();

            return channelsResponse.Items.FirstOrDefault()?.Snippet.ChannelId;
        }
        [NonAction]
        private static string AppendTimeCodeToUrl(string videoUrl, TimeSpan timeCode)
        {
            string timeCodeString = Convert.ToInt32(timeCode.TotalSeconds).ToString();
            string separator = videoUrl.Contains("?") ? "&" : "?";
            return $"{videoUrl}{separator}t={timeCodeString}";
        }
    }
}
