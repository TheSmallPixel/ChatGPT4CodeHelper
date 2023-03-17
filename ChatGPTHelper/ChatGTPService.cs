using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;


namespace ChatGPTHelper
{
    public class ChatGTPService
    {


        public async Task GetCodeSuggestionAsync(string request, Action<int, CompletionResult> resultHandler)
        {
            OpenAIAPI api = new OpenAIAPI("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");
            // var completions = await _client.Completions.CreateAsync("text-davinci-002", prompt, maxTokens: 50, n: 1, stop: new string[] { "\n" });


            CompletionRequest completionRequest = new CompletionRequest(request, Model.DavinciText, temperature: 0.7d, top_p: 1d, frequencyPenalty: 0d, presencePenalty: 0d, numOutputs: 1, max_tokens: 256);

            await api.Completions.StreamCompletionAsync(completionRequest, resultHandler);

        }
    }
}
