using System.Net.Sockets;
using jpda.Adsb.Model;
using Adsb.Reader;
using Adsb.Reader.Observers;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dapr.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<AdsbConnectorConfiguration>().Configure(opts =>
        {
            context.Configuration.Bind("AdsbConnector", opts);
        });

        services.AddSingleton<TcpClient>(x => new TcpClient());

        //services.AddSingleton<IAsyncObserver<Message>, ConsoleObserver>();
        services.AddSingleton<DaprClient>(x => new DaprClientBuilder().Build());
        services.AddSingleton<IAsyncObserver<Message>, DaprPubObserver>();
        services.AddHostedService<AdsbConnector>();
    })
    .Build();

await host.RunAsync();