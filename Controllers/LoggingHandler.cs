using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AirportDemo.Controllers
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Ýstek bilgilerini konsola yazdýrýn
            Console.WriteLine("=== HTTP Request ===");
            Console.WriteLine($"{request.Method} {request.RequestUri}");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
                string requestContent = await request.Content.ReadAsStringAsync();
                Console.WriteLine("Request Content: " + requestContent);
            }
            Console.WriteLine("====================");

            // Ýsteði gönder
            var response = await base.SendAsync(request, cancellationToken);

            // Yanýt bilgilerini konsola yazdýrýn
            Console.WriteLine("=== HTTP Response ===");
            Console.WriteLine($"Status Code: {response.StatusCode}");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Content: " + responseContent);
            }
            Console.WriteLine("====================");

            return response;
        }
    }
}
