using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using jpda.Adsb.Model;
using Adsb.Reader.Observers;

namespace Adsb.Reader
{
    public class AdsbConnectorConfiguration
    {
        public string? Ip { get; set; }
        public int Port { get; set; }
    }

    public class AdsbConnector : BackgroundService
    {
        private readonly string _ip;
        private readonly int _port;

        private readonly TcpClient _client;

        //private readonly List<IAsyncObserver<Adsb.Model.Message>> _observers;
        private readonly IAsyncObserver<Message> _observer;
        private readonly ILogger<AdsbConnector> _logger;

        public AdsbConnector(IOptions<AdsbConnectorConfiguration> opt, IAsyncObserver<Message> observer,
            ILoggerFactory loggerFactory, TcpClient client)
        {
            _client = client;
            _ip = opt.Value.Ip ?? throw new ArgumentNullException(nameof(opt), "IP is missing");
            _port = opt.Value.Port;
            _logger = loggerFactory.CreateLogger<AdsbConnector>();
            _observer = observer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Start(_ip, _port);
            }
        }

        private async Task Start(string server, int port)
        {
            do
            {
                var connected = await Connect(server, port);
                if (connected)
                {
                    await ReadStream();
                }

                await Task.Delay(10000);
            } while (true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:The logging message template should not vary between calls", Justification = "OmniSharp bug, see 7.x milestone")]
        private async Task<bool> Connect(string server, int port)
        {
            try
            {
                await _client.ConnectAsync(server, port);
                var prop = new Dictionary<string, string>() { { "Server", server }, { "Port", port.ToString() } };
                _logger.LogInformation("TcpClient-Connected", prop);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        private async Task ReadStream()
        {
            var data = new byte[256];
            var sbuffer = new StringBuilder(256);
            var mb = new MessageBuffer();
            var contCount = 0;

            try
            {
                while (_client.Connected)
                {
                    var stream = _client.GetStream();
                    if (!stream.DataAvailable)
                    {
                        if (contCount > 15)
                        {
                            EndSession();
                            return;
                        }

                        await Task.Delay(1000);
                        contCount++;
                        continue;
                    }

                    contCount = 0;
                    var bytes = stream.Read(data, 0, data.Length);

                    sbuffer.Clear();
                    sbuffer.Append(Encoding.ASCII.GetString(data, 0, bytes));

                    mb.AddData(sbuffer.ToString()).ForEach(m =>
                    {
                        sbuffer.Clear().Append(m.Trim());

                        var a = Message.Parse(sbuffer.ToString());
                        _observer.OnNextAsync(a);

                        // todo: figure out a pattern for multi-observer
                        // todo: should it be single-queue out of here? and downstream rebroadcast?
                        // todo: or should we just broadcast from here?
                        // todo: i dunno

                        //_observers.Select(x => x.OnNextAsync(a));
                        // Parallel.ForEach(_observers,
                        //     async (x, i) => await x.OnNextAsync(Message.Parse(sbuffer.ToString())));
                        //async x => { await x.OnNextAsync(Message.Parse(sbuffer.ToString())); });
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(2), "Stream error", ex.Message);
                EndSession();
            }
        }

        private void EndSession()
        {
            _client.Dispose();
        }
    }
}