using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Downloader.DummyHttpServer;

public static class HttpServer
{
    private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private static IWebHost Server;
    public static int Port { get; set; } = 3333;
    public static CancellationTokenSource CancellationToken { get; set; }

    public static async Task Main()
    {
        Run(Port);
        Console.ReadKey();
        await Stop();
    }

    public static void Run(int port)
    {
        CancellationToken ??= new CancellationTokenSource();
        if (CancellationToken.IsCancellationRequested)
            return;

        Server ??= _cache.GetOrCreate("DownloaderWebHost", e => {
            var host = CreateHostBuilder(port);
            host.RunAsync(CancellationToken.Token).ConfigureAwait(false);
            return host;
        });

        if (port == 0) // dynamic port
            SetPort();
    }

    private static void SetPort()
    {
        var feature = Server.ServerFeatures.Get<IServerAddressesFeature>();
        if (feature.Addresses.Any())
        {
            var address = feature.Addresses.First();
            Port = new Uri(address).Port;
        }
    }

    public static async Task Stop()
    {
        if (Server is not null)
        {
            CancellationToken?.Cancel();
            await Server?.StopAsync();
            Server?.Dispose();
            Server = null;
        }
    }

    public static IWebHost CreateHostBuilder(int port)
    {
        var host = WebHost.CreateDefaultBuilder()
                      .UseStartup<Startup>();

        if (port > 0)
        {
            host = host.UseUrls($"http://localhost:{port}");
        }

        return host.Build();
    }

    public static IWebHost CreateKestrelBuilder(int port)
    {
        IWebHost webHost = new WebHostBuilder()
            .UseKestrel(options => options.Listen(IPAddress.Loopback, port)) // dynamic port
            .UseStartup<Startup>()
            .Build();

        return webHost;
    }
}
