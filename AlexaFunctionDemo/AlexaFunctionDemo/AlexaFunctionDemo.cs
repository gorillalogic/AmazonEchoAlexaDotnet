using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Response;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET;
using Alexa.NET.Response.Directive;

namespace AlexaFunctionDemo
{
    public static class AlexaFunctionDemo
    {
        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            string json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            var requestType = skillRequest.GetRequestType();

            SkillResponse response = null;

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Welcome to Gorilla Logic!");
                response.Response.ShouldEndSession = false;
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                if (intentRequest.Intent.Name.ToLower() == "gorillalocation")
                {
                    response = ResponseBuilder.Tell("Gorilla Logic is located in Medellin RutaN building");
                    response.Response.ShouldEndSession = false;
                }

                if (intentRequest.Intent.Name.ToLower() == "gorillamusic")
                {
                    string audioUrl = "https://files.fm/down.php?cf&i=n523dd2d&n=Gorillaz+-+19-2000+lyrics.mp3";
                    string audioToken = "Gorillaz song 19 - 20000";
                    response = ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, audioUrl, audioToken);
                    response.Response.ShouldEndSession = true;

                }
            }
            else
            {
                response = ResponseBuilder.Empty();
            }

            return new OkObjectResult(response);
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="skillRequest">The skill request.</param>
        /// <returns></returns>
        private static async Task<bool> ValidateRequest(HttpRequest request, SkillRequest skillRequest)
        {
            request.Headers.TryGetValue("SignatureCertChainUrl", out var signatureChainUrl);
            if (string.IsNullOrWhiteSpace(signatureChainUrl))
            {
                return false;
            }

            Uri certUrl;
            try
            {
                certUrl = new Uri(signatureChainUrl);
            }
            catch
            {
                return false;
            }

            request.Headers.TryGetValue("Signature", out var signature);
            if (string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            request.Body.Position = 0;
            var body = await request.ReadAsStringAsync();
            request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
            {
                return false;
            }

            bool valid = await RequestVerification.Verify(signature, certUrl, body);
            bool isTimestampValid = RequestVerification.RequestTimestampWithinTolerance(skillRequest);

            if (!isTimestampValid)
            {
                valid = false;
            }

            return valid;
        }
    }
}
