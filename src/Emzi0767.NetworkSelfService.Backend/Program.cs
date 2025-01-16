using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Emzi0767.NetworkSelfService.Backend.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Emzi0767.NetworkSelfService.Backend;

public static class Program
{
    public static void Main(string[] args)
        => CreateHostBuilder(args).Build().Run();

    private static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
                webBuilder.UseStartup<Startup>()
                    .UseKestrel(ConfigureKestrel));

    private static void ConfigureKestrel(KestrelServerOptions kestrel)
    {
        kestrel.AddServerHeader = false;
        var konfig = kestrel.ApplicationServices.GetRequiredService<IOptions<HttpConfiguration>>().Value;

        if (konfig is null || konfig.Endpoints?.Any() != true)
        {
            kestrel.ListenAnyIP(5000, listen => listen.Protocols = HttpProtocols.Http2);
            return;
        }

        foreach (var endpoint in konfig.Endpoints)
        {
            if (!endpoint.EnableTls)
            {
                if (endpoint.Address.StartsWith("unix://"))
                    kestrel.ListenUnixSocket(
                        endpoint.Address.Substring(7),
                        ConfigureHttp);
                else
                    kestrel.Listen(
                        new IPEndPoint(
                            IPAddress.Parse(endpoint.Address),
                            endpoint.Port),
                        ConfigureHttp);
            }
            else
            {
                var serverCertFile = new FileInfo(endpoint.ServerCertificate);
                var serverKeyFile = endpoint.ServerKey is null
                    ? null
                    : new FileInfo(endpoint.ServerKey);

                using var serverCertTemp = endpoint.ServerKeyPassword is null
                ? X509Certificate2.CreateFromPemFile(serverCertFile.FullName, serverKeyFile?.FullName)
                    : X509Certificate2.CreateFromEncryptedPemFile(serverCertFile.FullName, Extensions.UTF8.GetString(endpoint.ServerKeyPassword), serverKeyFile?.FullName);

                // Workaround for Windows Schannel oddities
                // https://github.com/dotnet/runtime/issues/23749#issuecomment-388231655
                var serverCert = X509CertificateLoader.LoadCertificate(serverCertTemp.Export(X509ContentType.Pkcs12));

                var caCertsFile = endpoint.MutualTls
                    ? new FileInfo(endpoint.CaCertificate)
                    : null;

                var caCerts = caCertsFile is not null
                    ? new X509Certificate2Collection()
                    : null;

                caCerts?.ImportFromPemFile(caCertsFile?.FullName);

                if (endpoint.Address.StartsWith("unix://"))
                    kestrel.ListenUnixSocket(
                        endpoint.Address.Substring(7),
                        ConfigureHttps(endpoint.MutualTls, serverCert, caCerts));
                else
                    kestrel.Listen(
                        new IPEndPoint(
                            IPAddress.Parse(endpoint.Address),
                            endpoint.Port),
                        ConfigureHttps(endpoint.MutualTls, serverCert, caCerts));
            }

            static void ConfigureHttp(ListenOptions listen)
                => listen.Protocols = HttpProtocols.Http2;

            static Action<ListenOptions> ConfigureHttps(bool mutualTls, X509Certificate2 server, X509Certificate2Collection ca)
            {
                return ConfigureHttpsInner;

                void ConfigureHttpsInner(ListenOptions listen)
                {
                    listen.Protocols = HttpProtocols.Http1AndHttp2;
                    listen.UseHttps(server, https =>
                    {
                        https.ClientCertificateMode = mutualTls
                            ? ClientCertificateMode.RequireCertificate
                            : ClientCertificateMode.NoCertificate;

                        https.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                        https.ClientCertificateValidation = (clientCert, chain, errors) =>
                        {
                            chain.ChainPolicy.ExtraStore.Clear();
                            chain.ChainPolicy.ExtraStore.AddRange(ca);
                            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

                            chain.Build(clientCert);
                            foreach (var chainStatus in chain.ChainStatus)
                                if (chainStatus.Status != X509ChainStatusFlags.NoError && chainStatus.Status != X509ChainStatusFlags.UntrustedRoot)
                                    return false;

                            return true;
                        };
                    });
                }
            }
        }
    }
}
