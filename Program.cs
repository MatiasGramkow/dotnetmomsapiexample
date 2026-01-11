using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UFSTWSSecuritySample
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            Settings settings = configuration.GetSection("Settings").Get<Settings>();
            Endpoints endpoints = configuration.GetSection("Endpoints").Get<Endpoints>();

            Console.WriteLine($"Path to PCKS#12 file = {settings.PathPKCS12}");
            Console.WriteLine($"Path to PEM file = {settings.PathPEM}");
            Console.WriteLine($"VirksomhedKalenderHent = {endpoints.VirksomhedKalenderHent}");

            if (!File.Exists(settings.PathPKCS12))
            {
                Console.WriteLine("Cannot find " + settings.PathPKCS12);
                Console.WriteLine("Aborting run...");
                return;
            }

            if (!File.Exists(settings.PathPEM))
            {
                Console.WriteLine("Cannot find " + settings.PathPEM);
                Console.WriteLine("Aborting run...");
                return;
            }

            if (args.Length == 0)
            {
                Console.WriteLine("Invalid args");
                return;
            }

            var command = args[0];

            IApiClient client = new ApiClient(settings);
            switch (command)
            {
                case "VirksomhedKalenderHent":
                    await client.CallService(new VirksomhedKalenderHentWriter("41250313", "2025-01-01", "2026-12-31"), endpoints.VirksomhedKalenderHent);
                    Console.WriteLine("Finished");
                    break;
                case "ModtagMomsangivelseForeloebig":
                    // Kræver TransaktionIdentifikator fra VirksomhedKalenderHent response
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Fejl: TransaktionIdentifikator mangler!");
                        Console.WriteLine("Brug: dotnet run ModtagMomsangivelseForeloebig <transaktionId>");
                        Console.WriteLine("Hent først transaktionId fra: dotnet run VirksomhedKalenderHent");
                        break;
                    }
                    var kalenderTransaktionId = args[1];
                    // Test momsangivelse for Q1 2026 (current quarter)
                    // afgiftTilsvar = salgsMoms - koebsMoms = 2000 - 500 = 1500
                    await client.CallService(new ModtagMomsangivelseForeloebigWriter(
                        transaktionIdentifikator: kalenderTransaktionId,
                        seNummer: "41250313",
                        periodeFraDato: "2026-01-01",
                        periodeTilDato: "2026-03-31",
                        afgiftTilsvarBeloeb: 1500,
                        salgsMomsBeloeb: 2000,
                        koebsMomsBeloeb: 500
                    ), endpoints.ModtagMomsangivelseForeloebig);
                    Console.WriteLine("Finished");
                    break;
                case "MomsangivelseKvitteringHent":
                    // Hent kvittering for en tidligere indsendt momsangivelse
                    // transaktionIdentifier skal være ID fra en tidligere ModtagMomsangivelseForeloebig
                    var transaktionId = args.Length > 1 ? args[1] : "00000000-0000-0000-0000-000000000000";
                    await client.CallService(new MomsangivelseKvitteringHentWriter(
                        seNummer: "41250313",
                        transaktionIdentifier: transaktionId
                    ), endpoints.MomsangivelseKvitteringHent, "getMomsangivelseKvitteringHent");
                    Console.WriteLine("Finished");
                    break;
                default:
                    Console.WriteLine("Invalid command");
                    Console.WriteLine("Brug:");
                    Console.WriteLine("  dotnet run VirksomhedKalenderHent");
                    Console.WriteLine("  dotnet run ModtagMomsangivelseForeloebig <transaktionId fra VirksomhedKalenderHent>");
                    Console.WriteLine("  dotnet run MomsangivelseKvitteringHent <transaktionId fra ModtagMomsangivelseForeloebig>");
                    break;
            }
        }
    }
}
