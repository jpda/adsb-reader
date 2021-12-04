using adsb = jpda.Adsb.Model;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Adsb.Reader.Observers
{
    public class ConsoleObserver : IAsyncObserver<adsb.Message>
    {
        public void OnCompleted()
        {
            Console.WriteLine("Console observer completed");
        }

        public void OnError(Exception error)
        {
            var def = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error.Message);
            Console.ForegroundColor = def;
        }

        public void OnNext(adsb.Message value)
        {
            System.Text.Json.JsonSerializer.Serialize(value, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            var val = JsonSerializer.Serialize(value);
            Console.WriteLine(val);
        }

        public Task OnNextAsync(adsb.Message value)
        {
            OnNext(value);
            return Task.CompletedTask;
        }
    }
}