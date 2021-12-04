using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Adsb.Reader.Observers
{
    public abstract class ObserverBase<T> : IAsyncObserver<T>
    {
        private readonly ILogger _log;

        protected ObserverBase(ILogger logger)
        {
            _log = logger;
        }
        public virtual void OnCompleted()
        {
            _log.LogInformation($"Observer completed");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:The logging message template should not vary between calls", Justification = "OmniSharp bug")]
        public virtual void OnError(Exception error)
        {

            _log.LogError(error, error.Message);
        }

        public virtual void OnNext(T value)
        {
            //_log.Event("MessageReceived");
        }

        public virtual Task OnNextAsync(T value)
        {
            OnNext(value);
            return Task.CompletedTask;
        }
    }
}