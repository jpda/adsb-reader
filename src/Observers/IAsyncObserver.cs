using System;
using System.Threading.Tasks;

namespace Adsb.Reader.Observers
{
    public interface IAsyncObserver<in T> : IObserver<T>
    {
        Task OnNextAsync(T value);
    }
}