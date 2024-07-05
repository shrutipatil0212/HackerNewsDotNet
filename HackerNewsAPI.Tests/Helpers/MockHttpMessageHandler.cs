using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HackerNewsApi.Tests.Helpers
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var clonedResponse = new HttpResponseMessage(_response.StatusCode)
            {
                Content = new StringContent(await _response.Content.ReadAsStringAsync()),
                RequestMessage = request,
                ReasonPhrase = _response.ReasonPhrase
            };

            foreach (var header in _response.Headers)
            {
                clonedResponse.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return await Task.FromResult(clonedResponse);
        }
    }
}
