using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HotelSelfCheckIn.UI.Models;

//Host : nu am idee ce fac dar sper ca merge
//Configurare host-----------------------------------
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        // inregistrare Manager si Memoria ca Singleton(?)
        services.AddSingleton<Manager>();
        services.AddSingleton<FileService>();
    })
    .Build();

// Extragem Managerul configurat (de Loggerul deja injectat de sistem)
var manager = host.Services.GetRequiredService<Manager>();
var fileService = host.Services.GetRequiredService<FileService>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();



//incarcare datele salvate daca exista------------------------------
var (camere, rezervari) = fileService.Load(); 
manager.LoadData(camere, rezervari);

logger.LogInformation("Aplicatia a pornit cu succes!");
//Ecran pornire
Console.WriteLine("1. Login");
Console.WriteLine("2. Register [Client only]");
Console.WriteLine("0. Iesire");
string? choice = Console.ReadLine();
if (choice == "2")
{
    Console.WriteLine("Alege Username: "); //de verificat daca nu exista un user deja existent
    string ? username = Console.ReadLine();
    Console.WriteLine("Alege Parola: ");
    string ? parola = Console.ReadLine();
    manager.RegisterUser(new Client(username, parola));
    Console.WriteLine("Contul a fost creat cu succes!");
}

if (choice == "0") return;
if (choice == "1")
{
    Console.WriteLine("Username: "); 
    string ? user = Console.ReadLine();
    Console.WriteLine("Parola: ");
    string ? paro = Console.ReadLine();
    var ultilizatorLogat = manager.Authenticate(user, paro);

    if (ultilizatorLogat is Admin admin)
    {
        RunAdminMenu(manager, admin,fileService);
    }
    else if (ultilizatorLogat is Client client)
    {
        RunClientMenu(manager, client, fileService); 
    }
    else
    {
        Console.WriteLine("Date incorecte!");
    }
}
void RunAdminMenu(Manager mgr, Admin admin, FileService fs)
{
    bool logout = false;
    while (!logout)
    {
        Console.WriteLine($"\n--- MENIU ADMIN: {admin.Username} ---");
        Console.WriteLine("1. Administrare Camere");
        Console.WriteLine("2. Administrare Rezervari");
        Console.WriteLine("0. Logout");
        Console.Write("\nAlege: ");
        
        string? opt = Console.ReadLine();
        if (opt == "1")
        {
            bool logout1 = false;
            while (!logout1)
            {
                Console.WriteLine("\n--- Meniu Administrare Camere ---");
                Console.WriteLine("1. Creare Camera");
                Console.WriteLine("2. Modificare Camera");
                Console.WriteLine("3. Stergere Camera");
                Console.WriteLine("4. Setare Status Camera");
                Console.WriteLine("0. Logout");
                
                string? opt1 = Console.ReadLine();
                if (opt1 == "1")
                {
                    Console.Write("Introduceți numărul camerei (int): ");
                    if (int.TryParse(Console.ReadLine(), out int nr))
                    {
                       Console.WriteLine($"Tip camera:\n1.Single, 2.Double, 3.Triple, 4.Family"); 
                       string? tip = Console.ReadLine();
                       Room? cameraNoua = tip switch
                       {
                           "1" => new SingleRoom(nr),
                           "2" => new DoubleRoom(nr),
                           "3" => new TripleRoom(nr),
                           "4" => new FamilyRoom(nr),
                           _ => null
                       };
                    }
                }
                else if (opt1 == "0") logout1 = true;
            }
        }
        else if (opt == "0") logout = true;
    }
}

void RunClientMenu(Manager mgr, Client client, FileService fs)
{
    //aici la fel
    bool logout = false;
    while (!logout)
    {
        Console.WriteLine($"\n--- MENIU CLIENT: {client.Username} ---");
        Console.WriteLine("1. Rezervă Cameră");
        Console.WriteLine("0. Logout");
        Console.Write("Alege: ");

        string? opt = Console.ReadLine();
        if (opt == "0") logout = true;
    }
}
