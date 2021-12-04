using System;
using System.Threading.Tasks;
using Adsb.Actors.Interfaces;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Communication;
using jpda.Adsb.Model;
using adsb = jpda.Adsb.Model;

namespace Adsb.Reader.Observers
{
    public class DaprActorObserver : IAsyncObserver<adsb.Message>
    {
        private readonly ActorProxyFactory _proxyFactory;
        private readonly ILogger<DaprActorObserver> _logger;

        public DaprActorObserver(ActorProxyFactory proxyFactory, ILoggerFactory loggerFactory)
        {
            _proxyFactory = proxyFactory;
            _logger = loggerFactory.CreateLogger<DaprActorObserver>();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Message value)
        {
            throw new NotImplementedException();
        }

        public async Task OnNextAsync(Message value)
        {
            _logger.LogInformation("Connecting to actor proxy id {ProxyId}", value.Hexident);
            var vehicle = _proxyFactory.CreateActorProxy<IVehicleActor>(
                new ActorId(value.Hexident), "PlaneVehicleActor");
            await vehicle.SetPositionAsync(new Position(value.Latitude, value.Longitude));
        }
    }
}