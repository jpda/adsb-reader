// using adsb = ADSB.Interpreter.Adsb;
// using Microsoft.Azure.Devices.Client;
// using System;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
//
// namespace Adsb.Reader.Observers
// {
//     public class IotHubObserver : ObserverBase<adsb.Message>
//     {
//         private readonly DeviceClient _client;
//
//         public IotHubObserver(string connectionString, TransportType type, ILog logger) : this(DeviceClient.CreateFromConnectionString(connectionString, type), logger) { }
//
//         public IotHubObserver(DeviceClient client, ILog logger) : base(logger)
//         {
//             _client = client;
//             Task.Run(ReceiveCommands);
//         }
//
//         public override void OnNext(adsb.Message value)
//         {
//             OnNextAsync(value).Wait();
//         }
//
//         public override async Task OnNextAsync(adsb.Message value)
//         {
//             var val = JsonConvert.SerializeObject(value);
//             var msg = new Message(Encoding.UTF8.GetBytes(val));
//             await _client.OpenAsync();
//             await _client.SendEventAsync(msg);
//         }
//
//         private async Task ReceiveCommands()
//         {
//             Console.WriteLine("\nDevice waiting for commands from IoTHub...\n");
//             Message receivedMessage = null;
//
//             while (true)
//             {
//                 try
//                 {
//                     receivedMessage = await _client.ReceiveAsync(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
//
//                     if (receivedMessage != null)
//                     {
//                         string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
//                         Console.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);
//
//                         int propCount = 0;
//                         foreach (var prop in receivedMessage.Properties)
//                         {
//                             Console.WriteLine("\t\tProperty[{0}> Key={1} : Value={2}", propCount++, prop.Key, prop.Value);
//                         }
//
//                         await _client.CompleteAsync(receivedMessage).ConfigureAwait(false);
//                     }
//                 }
//                 finally
//                 {
//                     if (receivedMessage != null)
//                     {
//                         receivedMessage.Dispose();
//                     }
//                 }
//             }
//         }
//     }
// }