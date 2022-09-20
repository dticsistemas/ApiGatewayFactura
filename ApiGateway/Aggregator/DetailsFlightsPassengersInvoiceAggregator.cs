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
                byte[] downstreamResponseContent = await downstreamResponse.Content.ReadAsByteArrayAsync();

                if (downStreamRouteKey == "flights")
                {
                    flights = JsonConvert.DeserializeObject<List<FlighttDto>>(DeCompressBrotli(downstreamResponseContent));
                }

                if (downStreamRouteKey == "passengers")
                {
                    Console.WriteLine("Request: {0}", "Ingresando a PasajerosUsuarios");
                    passengers = JsonConvert.DeserializeObject<List<PassengerDto>>(DeCompressBrotli(downstreamResponseContent));
                    Console.WriteLine("Request: {0}", passengers.ToString());

                }
                if (downStreamRouteKey == "invoices")
                {
                    Console.WriteLine("Request: {0}", "Ingresando a Invoices");
                    invoices = JsonConvert.DeserializeObject<List<InvoiceDto>>(DeCompressBrotli(downstreamResponseContent));
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
                Console.WriteLine(invoice.title);
                //Console.WriteLine(invoice.title);
                /*
                var postusers = new JArray();
                var userId = usuario.id;
                */
                foreach (var flight in flights)
                {

                    //Console.WriteLine(post.id);
                    //Console.WriteLine(post.title);
                    //Console.WriteLine(post.body);
                   /* if (post.userId == userId)
                    {
                        var ObjPost = new JObject(
                                       new JProperty("id", post.id),
                                       new JProperty("title", post.title),
                                       new JProperty("body", post.body));
                        postusers.Add(ObjPost);
                    }*/


                }
                foreach (var passenger in passengers)
                {

                    //Console.WriteLine(post.id);
                    //Console.WriteLine(post.title);
                    //Console.WriteLine(post.body);
                    /* if (post.userId == userId)
                     {
                         var ObjPost = new JObject(
                                        new JProperty("id", post.id),
                                        new JProperty("title", post.title),
                                        new JProperty("body", post.body));
                         postusers.Add(ObjPost);
                     }*/


                }
                /*
                var ObjUser = new JObject(
                               new JProperty("id", usuario.id),
                               new JProperty("name", usuario.name),
                               new JProperty("username", usuario.username),
                               new JProperty("email", usuario.email),
                               new JProperty("post", postusers));

                
                arrayUsers.Add(ObjUser);
                */
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
