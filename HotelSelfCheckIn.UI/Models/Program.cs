using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HotelSelfCheckIn.UI.Models;

//Host : nu am idee ce fac dar sper ca merge
//generat initial de ai ca test, explicatii mai tarziu

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        // inregistrare Manager ca Singleton ( unul singur in aplicatie)
        services.AddSingleton<Manager>();
    })
    .Build();

// Extragem Managerul gata configurat (cu Loggerul deja injectat de sistem)
var manager = host.Services.GetRequiredService<Manager>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Aplicația Hotel Manager a pornit cu succes!");

// --- AICI ÎNCEPE LOGICA TA DE TEST SAU MENIUL ---

Console.WriteLine("=== Bun venit la Hotel Self Check-In ===");
// Exemplu: manager.AddRoom(admin, camera);

// La final, rulăm host-ul (opțional dacă e doar consolă, dar bine de pus)
await host.RunAsync();