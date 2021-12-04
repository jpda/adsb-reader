using adsb = jpda.Adsb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adsb.Reader.Observers
{
    public class StatsObserver : ConsoleObserver, IAsyncObserver<adsb.Message>
    {
        private readonly List<adsb.Message> _cache;
        private DateTime _windowStart;

        public StatsObserver()
        {
            _cache = new List<adsb.Message>();
            _windowStart = DateTime.UtcNow;
        }

        public new void OnNext(adsb.Message value)
        {
            _cache.Add(value);
            if (_cache.Count > 1000)
            {
                var time = DateTime.UtcNow - _windowStart;
                Console.WriteLine($" === last {_cache.Count} message stats - { time.TotalSeconds }s");
                var types = _cache.GroupBy(x => x.GetType().Name).Select(y => new { y.Key, Count = y.Count() }).OrderByDescending(z => z.Count);
                foreach (var t in types)
                {
                    Console.WriteLine($"{t.Key}: {t.Count}");
                }
                _cache.Clear();
                _windowStart = DateTime.UtcNow;
            }
        }

        public new Task OnNextAsync(adsb.Message value)
        {
            OnNext(value);
            return Task.CompletedTask;
        }
    }
}