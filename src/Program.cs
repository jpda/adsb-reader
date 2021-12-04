using System.Net.Sockets;
using jpda.Adsb.Model;
using Adsb.Reader;
using Adsb.Reader.Observers;
using Dapr.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<AdsbConnectorConfiguration>().Configure(opts =>
        {
            context.Configuration.Bind("AdsbConnector", opts);
        });

        services.AddSingleton(x => new TcpClient());

        //services.AddSingleton<IAsyncObserver<Message>, ConsoleObserver>();
        services.AddSingleton(x => new DaprClientBuilder().Build());
        services.AddSingleton<Dapr.Actors.Client.ActorProxyFactory>();
        services.AddSingleton<IAsyncObserver<Message>, DaprActorObserver>();
        services.AddHostedService<AdsbConnector>();
    })
    .Build();

await host.RunAsync();