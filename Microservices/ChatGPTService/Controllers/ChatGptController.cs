using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatGPTService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatGptController : ControllerBase
    {

        [HttpGet]
        [Route("GetFilmName")]
        public async Task<IActionResult> GetFilmName(string phrase)
        {
            string OutPutResult = "";
            var openAi = new OpenAIAPI("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");
            CompletionRequest completionRequest = new CompletionRequest();
            completionRequest.Prompt = phrase;
            completionRequest.Model = OpenAI_API.Models.Model.DavinciText;

            var completions = openAi.Completions.CreateCompletionAsync(completionRequest);

            foreach (var completion in completions.Result.Completions)
            {
                OutPutResult += completion.Text;
            }
            return Ok(OutPutResult);
        }

    }
}
