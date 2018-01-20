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
 using System.Collections.Generic;

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
            List<string> BotOptions = new List<string>();
                    
            BotOptions.Add("PRG-Prague");
            BotOptions.Add("SXF-Berlin Schönefeld");
            BotOptions.Add("STN-London Stansted");
            BotOptions.Add("TLS-Toulouse");
           BotOptions.Add("MLA-Malta");
            BotOptions.Add("CIA-Rome Ciampino");
            BotOptions.Add("LPA-Gran Canaria");
            BotOptions.Add("TFN-Tenerife North");
            BotOptions.Add("BUD-Budapest");
            BotOptions.Add("GLA-Glasgow");
            BotOptions.Add("IBZ-Ibiza");
            BotOptions.Add("BRI-Bari");
            BotOptions.Add("KRK-Krakow");
            BotOptions.Add("PMI-Mallorca");
            BotOptions.Add("SCQ-Santiago");
            BotOptions.Add("OPO-Porto");
            BotOptions.Add("BLQ-Bologna");
            BotOptions.Add("BGY-Milan Bergamo");
            BotOptions.Add("BRU-Brussels");
            BotOptions.Add("HAM-Hamburg");
            BotOptions.Add("DUB-Dublin");
            BotOptions.Add("PSA-Pisa");
            BotOptions.Add("STN-London Stansted");
            BotOptions.Add("FUE-Fuerteventura");
            BotOptions.Add("STN-London Stansted");
            BotOptions.Add("CAG-Cagliari");
            BotOptions.Add("CIA-Rome Ciampino");
            BotOptions.Add("PMI-Mallorca");
            BotOptions.Add("EIN-Eindhoven");
            BotOptions.Add("BLQ-Bologna");
            BotOptions.Add("OTP-Bucharest");
            BotOptions.Add("FEZ-Fez");
            BotOptions.Add("RAK-Marrakesh");
            BotOptions.Add("WRO-Wroclaw");
            BotOptions.Add("WMI-Warsaw Modlin");
            BotOptions.Add("MAN-Manchester");
            BotOptions.Add("CIA-Rome Ciampino");
            BotOptions.Add("STN-London Stansted");
            BotOptions.Add("PMI-Mallorca");
            BotOptions.Add("TLS-Toulouse");
            BotOptions.Add("OPO-Porto");
            BotOptions.Add("IBZ-Ibiza");
            BotOptions.Add("FRA-Frankfurt International");
            BotOptions.Add("PMI-Mallorca");
            BotOptions.Add("BGY-Milan Bergamo");
            BotOptions.Add("DUB-Dublin");
            BotOptions.Add("LPA-Gran Canaria");
            BotOptions.Add("ACE-Lanzarote"); 





            if (message.Text.ToUpper() == "FLIGHTS FROM HERE")
            {
              
                PromptDialog.Choice(context, 
                    AfterFlightsAsync,BotOptions,
                    "Your closest airport is Madrid, Barajas. Please choose your destination", 
                    "Didn't get that", 
                    1, 
                    PromptStyle.Auto);
              }  else
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

                                
                    switch (TopIntent)
                {
                    case "tef.int.es_ES.mp.tv.search":
                        PromptDialog.Confirm(
                        context,
                        AfterMovieAsync,
                        "I can see that you would like to see a movie, right?",
                        "Didn't get that!",
                        promptStyle: PromptStyle.Auto);
                        break;
                    case "":
                        Console.WriteLine("Case 2");
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
                     
             //   await context.PostAsync("Your intent is: "+   TopIntent + " and your entities: " + Entity1+ " " + Entity2);

                // await context.PostAsync(json);
                //await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
        }


   public async Task AfterMovieAsync(IDialogContext context, IAwaitable<bool> argument)
        {

             await context.PostAsync("Movie search ");
            
        }

        public async Task AfterFlightsAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var confirm = await argument;
            if (confirm!="")
            {
                this.count = 1;
               

                WebRequest request = WebRequest.Create("http://apigateway.ryanair.com/pub/v1/flightinfo/3/flights?departureAirportIataCode=MAD&arrivalAirportIataCode="+  confirm.ToString().Substring(0,3)  +"&apikey=axQgeITSziRuQSDAG765w1M3iXnkTAET");
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
