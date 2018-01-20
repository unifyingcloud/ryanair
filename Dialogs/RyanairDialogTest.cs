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

        public static string obtenerVuelosBaratos(string departureAirportIataCode, string outboundDepartureDateFrom, string outboundDepartureDateTo, string apikey){
            string response;
            string url = "http://apigateway.ryanair.com/pub/v1/farefinder/3/oneWayFares?departureAirportIataCode=" + departureAirportIataCode + "&outboundDepartureDateFrom=" + outboundDepartureDateFrom + "&outboundDepartureDateTo=" + outboundDepartureDateTo + "&apikey=" + apikey;
            
            //Llamada a API Ryanair
            WebRequest WebRequest = WebRequest.Create(url);
            
            WebResponse WebResponse = WebRequest.GetResponse();
            
            using (var sr = new StreamReader(WebResponse.GetResponseStream()))
                    {
                        response = sr.ReadToEnd();
                    }
            return response;
        }
        

        public static string obtenerInfoVuelo(String number, String apikey){
		string response;
		string url = "http://apigateway.ryanair.com/pub/v1/flightinfo/3/flights?number=" + number + "&apikey=" + apikey;
		
		//Llamada a API Ryanair
		WebRequest WebRequest = WebRequest.Create(url);
        
		WebResponse WebResponse = WebRequest.GetResponse();
		
		using (var sr = new StreamReader(WebResponse.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }
		return response;
	}


public static string parsearJSONInfoVuelo (JToken token){
		string response = "";
		string status = token.SelectToken("flights[0].status.message").ToString();
			
		response += "Flight " + token.SelectToken("flights[0].number").ToString() + " is " + status +".\n\n";	

		return response;
	}
	
        public static string parsearJSON(JToken token)
        {
            string response = "";
            Object[] fares = token.SelectToken("fares").ToArray();
            for (int i = 0; i < fares.Length; i++)
            {

                response += "Flight number " + i + 1 + " goes from " + token.SelectToken("fares[" + i + "].outbound.departureAirport.name").ToString() + " to  " + token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + " and it costs " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString() + ".\n\n";

            }

            return response;
        }

    public static string parsearJSONVuelos (JToken token){
		string response = "";
            try{ 

                String status = "";//token.SelectToken("flights[0].status.message").ToString();
			
                response += "Flight " + token.SelectToken("flights[0].number").ToString() + ":   " + status +".\n\n" + token.ToString();	

                Object[] fares = token.SelectToken("fares").ToArray();
                for (int i = 0; i < fares.Length; i++)
                {

                    response += "Flight number " + token.SelectToken("flights["+ i +"].number")  ;// goes from " + token.SelectToken("fares[" + i + "].outbound.departureAirport.name").ToString() + " to  " + token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + " and it costs " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString() + ".\n\n";

                }


            }
            catch(Exception ex){

                response = ex.Message ;
            }
		return response;
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
            BotOptions.Add("OPO-Porto");/*
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
            BotOptions.Add("ACE-Lanzarote"); */


            if (message.Text.ToUpper() == "CHEAP FLIGHTS FROM HERE")
            {
                List<string> BotMonthOptions = new List<string>();
                BotMonthOptions.Add("January");
                BotMonthOptions.Add("February");
                BotMonthOptions.Add("March");
                BotMonthOptions.Add("April");
                BotMonthOptions.Add("May");
                BotMonthOptions.Add("June");
                BotMonthOptions.Add("July");
                BotMonthOptions.Add("Augost");
                BotMonthOptions.Add("September");


                PromptDialog.Choice(context, 
                                    AfterCheapFlightsAsync,BotMonthOptions,
                    "Your closest airport is Madrid, Barajas. Please select a Month when you want to fly", 
                    "Didn't get that", 
                    1, 
                    PromptStyle.Auto);
            }
             else if (message.Text.ToUpper() == "FLIGHTS FROM HERE")
            {
             
                PromptDialog.Choice(context, 
                    AfterFlightsAsync,BotOptions,
                    "Your closest airport is Madrid, Barajas. Please choose your destination", 
                    "Didn't get that", 
                    1, 
                    PromptStyle.Auto);
            }else if (message.Text.ToUpper() == "FLIGHT STATUS")
            {
               await context.PostAsync("Please type your flight number");

                context.Wait(this.flightNumberAsync);
                
            }
            else
            {
                
               // await context.PostAsync("Result");
                 
                WebRequest request = WebRequest.Create("https://westeurope.api.cognitive.microsoft.com/luis/v2.0/apps/1703b0e2-e00d-466e-8f99-710cfc850299?subscription-key=7cadeb2e13cf4cd3803cc832b6dfcd15&verbose=true&timezoneOffset=0&q" + message.Text);
                WebResponse response = request.GetResponse();

                String json;

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }


                JToken token = JToken.Parse(json);

               // await context.PostAsync(json);
                 
                string TopIntent = token.SelectToken("topScoringIntent.intent").ToString();
             //   string Entity1 =token.SelectToken("entities[0].entity").ToString();
               // string Entity2 =token.SelectToken("entities[1].entity").ToString();

                                
                    switch (TopIntent)
                {
                    case "ry.sales.search":
                            PromptDialog.Choice(context,
                            AfterFlightsAsync, BotOptions,
                            "Your closest airport is Madrid, Barajas. Please choose your destination",
                            "Didn't get that",
                            1,
                            PromptStyle.Auto);
                        
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
              //  context.Wait(MessageReceivedAsync);

 
            }
        }


   public async Task AfterMovieAsync(IDialogContext context, IAwaitable<bool> argument)
        {

             await context.PostAsync("Movie search ");
            
        }
	
        public async Task AfterFlightsAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var resultValue = await argument;
            String confirmedDestination = resultValue.ToString().Substring(0,3) ;
            if (confirmedDestination!="")
            {
              
                String resultJSON=  obtenerVuelos("MAD", confirmedDestination, "axQgeITSziRuQSDAG765w1M3iXnkTAET");
                 JToken  token = JToken.Parse(resultJSON);

                String resp = "";
                try{

                    resp=parsearJSONVuelos(token);
                }
                catch(Exception ex){
                    resp = ex.Message + resultJSON;

                }
                await context.PostAsync(resp); 

        		


            }
            else
            {
                await context.PostAsync("Did not search for flights.");
            }
            context.Wait(MessageReceivedAsync);
        }




	public static string obtenerVuelos(string departureAirportIataCode, string arrivalAirportIataCode, string apikey){
		string response;
		string url = "http://apigateway.ryanair.com/pub/v1/flightinfo/3/flights?departureAirportIataCode=" + departureAirportIataCode + "&arrivalAirportIataCode=" + arrivalAirportIataCode + "&apikey=" + apikey;
		
		//Llamada a API Ryanair
		WebRequest WebRequest = WebRequest.Create(url);
        
		WebResponse WebResponse = WebRequest.GetResponse();
		
		using (var sr = new StreamReader(WebResponse.GetResponseStream()))
                {
                    response = sr.ReadToEnd();
                }
		return response;
	}

    private async Task flightNumberAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            

             String json= obtenerInfoVuelo(message.ToString(),"axQgeITSziRuQSDAG765w1M3iXnkTAET");

              JToken token = JToken.Parse(json);

              await context.PostAsync(parsearJSONInfoVuelo(token));
            
            
            }
    public async Task AfterCheapFlightsAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var resultValue = await argument;
            String confirm = resultValue.ToString() ;
            String DateFrom = "";
            String DateTo = "";

            switch (confirm)
            {
                case "January":
                    DateFrom = "2018-01-01";
                    DateTo="2018-01-31";
                    break;
                case "February":
                    DateFrom = "2018-02-01";
                    DateTo = "2018-02-31";
                    break;
                case "March":
                    DateFrom = "2018-03-01";
                    DateTo = "2018-03-31";
                    break;
                case "April":
                    DateFrom = "2018-04-01";
                    DateTo = "2018-04-31";
                    break;
                case "May":
                    DateFrom = "2018-05-01";
                    DateTo = "2018-05-31";
                    break;
                case "June":
                    DateFrom = "2018-06-01";
                    DateTo = "2018-06-31";
                    break;
                case "July":
                    DateFrom = "2018-07-01";
                    DateTo = "2018-07-31";
                    break;
                case "Augost":
                    DateFrom = "2018-08-01";
                    DateTo = "2018-08-31";
                    break;

            }


            if (confirm!="")
            {
              
                String resultJSON= obtenerVuelosBaratos("MAD",DateFrom, DateTo, "axQgeITSziRuQSDAG765w1M3iXnkTAET");
                 JToken  token = JToken.Parse(resultJSON);
	             await context.PostAsync(parsearJSON(token)); 

		 
            }
            else
            {
                await context.PostAsync("Did not search for flights.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}