using ApiGateway.Dto;
using BrotliSharpLib;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ocelot.Configuration;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Ocelot.RequestId;
using System.Net;
using System.Net.Http.Headers;

namespace ApiGateway.Aggregator
{
    public class DetailsFlightsPassengersInvoiceAggregator : IDefinedAggregator
    {

       public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
            {
                List<FlighttDto> flights = new List<FlighttDto>();
                List<PassengerDto> passengers = new List<PassengerDto>();
                List<InvoiceDto> invoices = new List<InvoiceDto>();

                foreach (var response in responses)
                {
                    string downStreamRouteKey = ((DownstreamRoute)response.Items["DownstreamRoute"]).Key;
                    DownstreamResponse downstreamResponse = (DownstreamResponse)response.Items["DownstreamResponse"];
                    var  downstreamResponseContent = await downstreamResponse.Content.ReadAsStringAsync();

                if (downStreamRouteKey == "passengers")
                {
                    Console.WriteLine("Request: {0}", "Ingresando a PasajerosUsuarios");
                   // var articuloInventarioObj = JsonConvert.DeserializeObject<PassengerDto>(productoStr);

                    passengers = JsonConvert.DeserializeObject<List<PassengerDto>>(downstreamResponseContent);
                    Console.WriteLine("Request: {0}", passengers.ToString());

                }
                if (downStreamRouteKey == "flights")
                {
                    flights = JsonConvert.DeserializeObject<List<FlighttDto>>(downstreamResponseContent);
                }
                if (downStreamRouteKey == "invoices")
                    {
                        Console.WriteLine("Request: {0}", "Ingresando a Invoices");
                        invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(downstreamResponseContent);
                        Console.WriteLine("Request: {0}", passengers.ToString());

                    }
                }

                return DetailsFlightsPassengersInvoice(flights,passengers,invoices);

            }
            public DownstreamResponse DetailsFlightsPassengersInvoice(List<FlighttDto> flights, List<PassengerDto> passengers, List<InvoiceDto> invoices)
            {

                var arrayUsers = new JArray();            
                var contador = 0;

                foreach (var invoice in invoices)
                {
                    Console.WriteLine(invoice.id);
                    var invoiceId = invoice.id;
                    //Console.WriteLine(invoice.title);

                    var postvuelos = new JArray();                

                    foreach (var flight in flights)
                    {

                        var flightId = flight.id;
                        //Console.WriteLine(post.id);
                        //Console.WriteLine(post.title);
                        //Console.WriteLine(post.body);
                         if (invoice.flight == flightId)
                         {
                             var ObjPost = new JObject(
                                            new JProperty("id", flight.id),
                                            new JProperty("source_airport_code", flight.source_airport_code),
                                            new JProperty("destiny_airport_code", flight.destiny_airport_code));
                             postvuelos.Add(ObjPost);

                         }


                    }
                    var postpasajeros = new JArray();
                    foreach (var passenger in passengers)
                    {
                        var passengerId = passenger.id;
                        //Console.WriteLine(post.id);
                        //Console.WriteLine(post.title);
                        //Console.WriteLine(post.body);
                         if (invoice.passanger == passengerId)
                         {
                            var ObjPost = new JObject(
                                    new JProperty("id", passenger.id),
                                    new JProperty("name", passenger.name),
                                    new JProperty("lastName", passenger.lastName),
                                    new JProperty("passport", passenger.passport)
                                    );
                             postpasajeros.Add(ObjPost);

                         }


                    }

                    var ObjUser = new JObject(
                                   new JProperty("id", invoice.id),
                                   new JProperty("reservationNumber", invoice.reservationNumber),
                                   new JProperty("reservationStatus", invoice.reservationStatus),
                                   new JProperty("date", invoice.date),
                                   new JProperty("Pasajero", postpasajeros),
                                   new JProperty("Vuelo", postvuelos)
                        );


                    arrayUsers.Add(ObjUser);

                    contador++; 
                    Console.WriteLine(contador + " -------");

                }            

                var objectPostsUsersString = JsonConvert.SerializeObject(arrayUsers);

                var stringContent = new StringContent(objectPostsUsersString)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                };

                return new DownstreamResponse(stringContent, HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "OK");
            }
            private string DeCompressBrotli(byte[] xResponseContent)
            {
                return System.Text.Encoding.UTF8.GetString(Brotli.DecompressBuffer(xResponseContent, 0, xResponseContent.Length, null));
            }
    }
}
