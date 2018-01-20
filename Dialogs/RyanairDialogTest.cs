using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class RyanairTest : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "Flights")
            {
                PromptDialog.Confirm(
                    context,
                    AfterFlightsAsync,
                    "Do you want to start looking for flights?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            
            else
            {
                


                  WebRequest request = WebRequest.Create("https://westeurope.api.cognitive.microsoft.com/luis/v2.0/apps/c22412bb-2bb6-48f0-aba6-52d4783853b5?subscription-key=7cadeb2e13cf4cd3803cc832b6dfcd15&verbose=true&timezoneOffset=0&q=" + message.Text);
                WebResponse response = request.GetResponse();

                string json;

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }


                JToken token = JToken.Parse(json);
                    
                string TopIntent =token.SelectToken("topScoringIntent.intent").ToString();

                string Entity1 =token.SelectToken("entities[0].entity").ToString();
                string Entity2 =token.SelectToken("entities[1].entity").ToString();

                
                     
                await context.PostAsync("Your intent is : "   TopIntent + " and your first Entity" + Entity1);

                // await context.PostAsync(json);
                //await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }




        public async Task AfterFlightsAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
               

                WebRequest request = WebRequest.Create("http://apigateway.ryanair.com/pub/v1/farefinder/3/oneWayFares?departureAirportIataCode=MAD&outboundDepartureDateFrom=2018-01-22&outboundDepartureDateTo=2018-01-31&apikey=axQgeITSziRuQSDAG765w1M3iXnkTAET");
                WebResponse response = request.GetResponse();

                string json;

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }

                            
                    await context.PostAsync(json);




            }
            else
            {
                await context.PostAsync("Did not search for flights.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}
