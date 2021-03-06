﻿using System;
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


        private static Attachment GetThumbnailCard()
        {
            var heroCard = new ThumbnailCard
            {
                Title = "Baggage help",
                Subtitle = "Follow the link to obtain information about baggage help",
                Text = "Checked baggage fees are fees due on purchased checked bags.",
                Images = new List<CardImage> { new CardImage("https://cdn-03.independent.ie/incoming/article35396046.ece/5653d/AUTOCROP/w620/5%20NEWS%20Ryanair%20bag%20te.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Open Q&A", value: "https://www.ryanair.com/gb/en/useful-info/help-centre/faq-overview/Baggage#0-0") }
            };

            return heroCard.ToAttachment();
        }

        private static Attachment GetHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Search results",
                Subtitle = "These are your selected flights",
                Text = "Click on each card to book a flight.",
                Images = new List<CardImage> { new CardImage("https://i.gocollette.com/img/destination-page/europe/europe-continent/europe-ms3.jpg?h=720&w=1280&la=en"),new CardImage("http://cdn.bootsnall.com/locations/Europe-thumb.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Book", value: "https://www.ryanair.com/gb/en/booking/home") }
            };

            return heroCard.ToAttachment();
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


        public static Attachment parsearJSONInfoVuelo (String token){
		/*string response = "";
		string status = token.SelectToken("flights[0].status.message").ToString();
			
		response += "Flight " + token.SelectToken("flights[0].number").ToString() + " is " + status +".nn";	

		return response;*/


            var heroCard = new HeroCard
            {
                Title = "Flight info",
                Subtitle = "Click below to see your flight info",
                Text = "",
                Images = new List<CardImage> { new CardImage("http://www.airlinequality.com/wp-content/uploads/2015/07/RYANAIR_JET.jpg") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Your Flight", value: "https://www.flightstats.com/v2/flight-tracker/FR/" + token) }
            };

            return heroCard.ToAttachment();
	}
	
        public static Attachment parsearJSON(JToken token)
        {
            var heroCard = new HeroCard
            {
                Title = "Search results",
                Subtitle = "These are your selected flights",
                Text = "Click on each card to book a flight.",
                Images = new List<CardImage> { new CardImage("https://i.gocollette.com/img/destination-page/europe/europe-continent/europe-ms3.jpg?h=720&w=1280&la=en"), new CardImage("http://cdn.bootsnall.com/locations/Europe-thumb.jpg") },
                Buttons = new List<CardAction> {  }
            };

           
            Object[] fares = token.SelectToken("fares").ToArray();
            for (int i = 0; i < 10; i++)
            {
                try{
                    heroCard.Buttons.Add(new CardAction(ActionTypes.OpenUrl, token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + ": " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + " " + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString(), value: "https://www.ryanair.com/gb/en/booking/home"));
               // response += "From " + token.SelectToken("fares[" + i + "].outbound.departureAirport.name").ToString() + " to  " + token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + " and it costs " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString() + ".nn";
                }
                catch(Exception ex){}
            }

            return  heroCard.ToAttachment();
        }


        public static Attachment parseJSONToCards(JToken token)
        {
            var heroCard = new HeroCard
            {
                Title = "Search results",
                Subtitle = "These are your selected flights",
                Text = "Click on each card to book a flight.",
                Images = new List<CardImage> { new CardImage("https://i.gocollette.com/img/destination-page/europe/europe-continent/europe-ms3.jpg?h=720&w=1280&la=en"), new CardImage("http://cdn.bootsnall.com/locations/Europe-thumb.jpg") },
                Buttons = new List<CardAction> { }
            };


            Object[] fares = token.SelectToken("flights").ToArray();
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    heroCard.Buttons.Add(new CardAction(ActionTypes.OpenUrl, token.SelectToken("flights[" + i + "].number").ToString() , value: "http://www.ryanair.com"));
                    // response += "From " + token.SelectToken("fares[" + i + "].outbound.departureAirport.name").ToString() + " to  " + token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + " and it costs " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString() + ".nn";
                }
                catch (Exception ex) { }
            }

            return heroCard.ToAttachment();
        }


    public static string parsearJSONVuelos (JToken token){
		string response = "";
            try{


                //response = token.ToString();

                Object[] fares = token.SelectToken("flights").ToArray();
                for (int i = 0; i < fares.Length; i++)
                {

                    response += "Flight number " + token.SelectToken("flights[" + i + "].number");// goes from " + token.SelectToken("fares[" + i + "].outbound.departureAirport.name").ToString() + " to  " + token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + " and it costs " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString() + ".nn";

                }

            /*    String status = "";//token.SelectToken("flights[0].status.message").ToString();
			
               // response += "Flight " + token.SelectToken("flights[0].number").ToString() + ":   " + status +".nn" + token.ToString();	

                Object[] fares = token.SelectToken("fares").ToArray();
                for (int i = 0; i < fares.Length; i++)
                {

                    response += "Flight number " + token.SelectToken("flights["+ i +"].number")  ;// goes from " + token.SelectToken("fares[" + i + "].outbound.departureAirport.name").ToString() + " to  " + token.SelectToken("fares[" + i + "].outbound.arrivalAirport.name").ToString() + " and it costs " + token.SelectToken("fares[" + i + "].outbound.price.value").ToString() + token.SelectToken("fares[" + i + "].outbound.price.currencySymbol").ToString() + ".nn";

                }*/


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
                BotMonthOptions.Add("August");
                BotMonthOptions.Add("September");


                PromptDialog.Choice(context, 
                                    AfterCheapFlightsAsync,BotMonthOptions,
                    "Your closest airport is Madrid, Barajas. Please select a Month when you want to fly", 
                    "Didn't get that", 
                    3, 
                    PromptStyle.Auto);
            }
            else if (message.Text.ToUpper() == ":)")
            {
                List<string> BotMonthOptions = new List<string>();
                //BotMonthOptions.Add("January");
                BotMonthOptions.Add("February");
                BotMonthOptions.Add("March");
                BotMonthOptions.Add("April");
                BotMonthOptions.Add("May");
                BotMonthOptions.Add("June");
                BotMonthOptions.Add("July");
                BotMonthOptions.Add("August");
                BotMonthOptions.Add("September");

                BotMonthOptions.Add("October");
                BotMonthOptions.Add("November");
                BotMonthOptions.Add("December");

                PromptDialog.Choice(context,
                                    AfterCheapFlightsAsync, BotMonthOptions,
                    "Your closest airport is Madrid, Barajas. Please select a Month when you want to fly",
                    "What month would you like to fly",
                    3,
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


                

                
            }else if (message.Text.ToUpper() == "HELLO" || message.Text.ToUpper() == "HI" || message.Text.ToUpper() == "HOLA" )
            {
                
                await context.PostAsync("Hello, how can I can help you? You can type -whats my flight status? or.. Type a smily face to get cheap flights from your city!... Type 'Flights from here to search for flights... or Type help if you want to know about your baggage info' ");

                //  context.Wait(this.flightNumberAsync);

            }else if (message.Text.ToUpper() == "HELP")
            {
                var messageReturn = context.MakeMessage();

                var attachment = GetThumbnailCard();
                messageReturn.Attachments.Add(attachment);

                await context.PostAsync(messageReturn);

              //  context.Wait(this.flightNumberAsync);

            }
            else if (message.Text.ToUpper() == "BAGGAGE HELP")
            {
              //  await context.PostAsync("If you need help go to https://www.ryanair.com/gb/en/useful-info/help-centre/faq-overview/Baggage/What-cabin-baggage-can-I-carry");


                 var messageReturn = context.MakeMessage();

                var attachment = GetThumbnailCard();
                messageReturn.Attachments.Add(attachment);

                await context.PostAsync(messageReturn);

               // context.Wait(this.MessageReceivedAsync);

                //  context.Wait(this.flightNumberAsync);

            }
            else if (message.Text.ToUpper() == "BOOK")
            {
                //  await context.PostAsync("If you need help go to https://www.ryanair.com/gb/en/useful-info/help-centre/faq-overview/Baggage/What-cabin-baggage-can-I-carry");


                var messageReturn = context.MakeMessage();

                var attachment = GetHeroCard();
                messageReturn.Attachments.Add(attachment);

                await context.PostAsync(messageReturn);

                // context.Wait(this.MessageReceivedAsync);

                //  context.Wait(this.flightNumberAsync);

            } else
            {
                
                //await context.PostAsync("Result");


                String json="";

                try{

                    WebRequest request = WebRequest.Create("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/08ac4c18-40c9-4fee-a606-98e882e4e15e?subscription-key=1d4fc55c3a4a4402b6647f2f0db35bec&verbose=true&timezoneOffset=0&q=" + message.Text);
                WebResponse response = request.GetResponse();





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
                  
                        case "ry.arrival.stats":


                            await context.PostAsync("Please type your flight number");

                            context.Wait(this.flightNumberAsync); 

                            break;
                        case "None":
                        Console.WriteLine("Are you interested in flying?");
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }

                }
                catch (Exception ex)
                {
                    await context.PostAsync("Can you repeat that? I found an issue with LUIS: " + ex.Message +json  );

                }
                     
             //   await context.PostAsync("Your intent is: "+   TopIntent + " and your entities: " + Entity1+ " " + Entity2);

                // await context.PostAsync(json);
                //await context.PostAsync($"{this.count++}: You said {message.Text}");
              //  context.Wait(MessageReceivedAsync);

             }
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

                    //resp=parsearJSONVuelos(token);

                    var messageReturn = context.MakeMessage();

                    var attachment = parseJSONToCards(token);
                    messageReturn.Attachments.Add(attachment);

                    await context.PostAsync(messageReturn);

//                    
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

                  var messageReturn = context.MakeMessage();

            var attachment = GetHeroCard();
            messageReturn.Attachments.Add(parsearJSONInfoVuelo(message.Text));

            await context.PostAsync(messageReturn);
            context.Done<string>(null);
           
            
            
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
                    DateTo = "2018-03-01";
                    break;
                case "March":
                    DateFrom = "2018-03-01";
                    DateTo = "2018-04-01";
                    break;
                case "April":
                    DateFrom = "2018-04-01";
                    DateTo = "2018-05-01";
                    break;
                case "May":
                    DateFrom = "2018-05-01";
                    DateTo = "2018-06-01";
                    break;
                case "June":
                    DateFrom = "2018-06-01";
                    DateTo = "2018-07-01";
                    break;
                case "July":
                    DateFrom = "2018-07-01";
                    DateTo = "2018-07-08";
                    break;
                case "August":
                    DateFrom = "2018-08-01";
                    DateTo = "2018-09-01";
                    break;
                case "September":
                    DateFrom = "2018-09-01";
                    DateTo = "2018-10-01";
                    break;
                case "October":
                    DateFrom = "2018-10-01";
                    DateTo = "2018-11-01";
                    break;
                case "November":
                    DateFrom = "2018-11-01";
                    DateTo = "2018-12-01";
                    break;
                case "December":
                    DateFrom = "2018-12-01";
                    DateTo = "2019-01-01";
                    break;
                 default:
                    DateFrom = "2018-12-01";
                    DateTo = "2019-01-01";
                    break;
            }


            if (confirm!="")
            {
                String resultJSON="";
                try{   

                    resultJSON  = obtenerVuelosBaratos("MAD",DateFrom, DateTo, "axQgeITSziRuQSDAG765w1M3iXnkTAET");
                 JToken  token = JToken.Parse(resultJSON);

                var messageReturn = context.MakeMessage();

                var attachment = parsearJSON(token);
                messageReturn.Attachments.Add(attachment);

                await context.PostAsync(messageReturn);
                 
                }

                catch(Exception ex){
                    await context.PostAsync(ex.Message + " " + resultJSON);
                }


            }
            else
            {
                await context.PostAsync("Did not search for flights.");
            }
            context.Wait(MessageReceivedAsync);
        
        
        
        
        
        }

         
    }

   
}