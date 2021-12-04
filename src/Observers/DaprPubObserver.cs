using System;
using System.Threading.Tasks;
using Dapr.Client;
using adsb = jpda.Adsb.Model;

namespace Adsb.Reader.Observers
{
    public class DaprPubObserver : IAsyncObserver<adsb.Message>
    {
        private readonly DaprClient _client;

        public DaprPubObserver(DaprClient client)
        {
            _client = client;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(adsb.Message value)
        {
            OnNextAsync(value).Wait();
        }

        public async Task OnNextAsync(adsb.Message value)
        {
            await _client.PublishEventAsync("currentLocation", "locations", value);
        }
    }
}