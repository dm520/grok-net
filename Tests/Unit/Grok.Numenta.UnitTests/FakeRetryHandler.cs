using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Grok.Numenta.UnitTests
{
    /// <summary>
    /// HTTP Handler used to simulate Server error 
    /// </summary>
    class FakeRetryHandler : FakeHandler
    {
        public int Retries { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
        public HttpResponseMessage BadResponse { get; set; }
  
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken  cancellationToken)
        {
            if (Response == null)
            {
                return base.SendAsync(request, cancellationToken);
            }
            Retries--;
            if (Retries > 0)
                return Task.Factory.StartNew(() => BadResponse);
             return Task.Factory.StartNew(() => Response);
        }
    }
}
