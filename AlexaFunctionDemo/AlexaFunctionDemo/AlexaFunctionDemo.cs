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
using System.Collections.Generic;

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
            Session session = skillRequest.Session;

            SkillResponse response = null;
            response.Response.ShouldEndSession = false;

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Welcome to Gorilla Logic!");
            }
            else if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                switch (intentRequest.Intent.Name.ToLower())
                {
                    case "gorillalocation":
                        response = ResponseBuilder.Tell("Gorilla Logic is located in Ruta N medellin Colombia oficina 2020");
                        var speech = new SsmlOutputSpeech();
                        speech.Ssml = "<speak>Gorilla Logic is located in <lang xml:lang=\"es-ES\">Ruta Ene Medellin Colombia oficina 2020</lang></speak>";
                        //response = ResponseBuilder.Tell(speech);
                        break;
                    case "gorillamusic":
                    case "AMAZON.ResumeIntent":
                        string audioUrl = "https://audiodemosmp3.s3.amazonaws.com/Gorillaz_-_19-2000_lyrics.mp3";
                        string audioToken = "Gorillaz song 19 - 20000";
                        var speechMusic = new SsmlOutputSpeech();
                        //speech.Ssml = $"<speak>{audioToken}<audio src=\"{audioUrl}\"/></speak>";
                        //response = ResponseBuilder.Tell(speechMusic);
                        response = ResponseBuilder.AudioPlayerPlay(PlayBehavior.ReplaceAll, audioUrl, audioToken, (int)skillRequest.Context.AudioPlayer.OffsetInMilliseconds);
                        break;
                    case "gorillainvitation":
                        var speechInivtation = new SsmlOutputSpeech();
                        speechInivtation.Ssml = "<speak><lang xml:lang=\"es-ES\"><voice name=\"Enrique\">Estan todos invitados al meetup del 25 de julio donde yo alexa seré la protagonista. Los esperamos en Ruta N</voice></lang></speak>";
                        response = ResponseBuilder.Tell(speechInivtation);
                        break;
                    case "gorillacalculation":
                        Reprompt reprompt = new Reprompt("you said {year}");
                        if (intentRequest.Intent.Slots.Count > 0)
                        {
                            if (intentRequest.Intent.Slots["year"] != null &&
                                intentRequest.Intent.Slots["year"].Value != null &&
                                intentRequest.Intent.Slots["date"] != null &&
                                intentRequest.Intent.Slots["date"].Value != null)
                            {
                                DateTime dateValue = DateTime.Parse(intentRequest.Intent.Slots["date"].Value.ToString());

                                dateValue.AddYears(int.Parse(intentRequest.Intent.Slots["year"].Value.ToString()));

                                int result = (DateTime.Now - dateValue).Days / 365;

                                response = ResponseBuilder.Tell($"you are {result} years old");
                                response.Response.ShouldEndSession = true;

                            }
                            else
                            {
                                response = ResponseBuilder.Ask("Please tell me, when were you born?", reprompt);
                            }
                        }
                        else
                        {
                            response = ResponseBuilder.Ask("Please tell me, when were you born?", reprompt);
                        }
                        break;
                    case "amazon.pauseintent":
                        response = ResponseBuilder.AudioPlayerStop();
                        break;
                    default:
                        break;
                }

            }
            else if (requestType == typeof(SessionEndedRequest))
            {
                response = ResponseBuilder.Tell("bye");
                response.Response.ShouldEndSession = true;
            }
            else if (requestType == typeof(AudioPlayerRequest))
            {
                // do some audio response stuff
                var audioRequest = skillRequest.Request as AudioPlayerRequest;

                //
                //if (audioRequest.AudioRequestType == AudioRequestType.PlaybackStopped)

                //
                //if (audioRequest.AudioRequestType == AudioRequestType.PlaybackNearlyFinished)
            }
            else
            {
                response = ResponseBuilder.Empty();
            }

            return new OkObjectResult(response);
        }
        
    }
}
