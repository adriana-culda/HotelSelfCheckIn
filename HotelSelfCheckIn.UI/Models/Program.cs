using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using HotelSelfCheckIn.UI.Models;


//Configurare host-----------------------------------
// using IHost host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices((_, services) =>
//     {
//         // inregistrare Manager si Memoria ca Singleton(?)
//         services.AddSingleton<Manager>();
//         services.AddSingleton<FileService>();
//     })
//     .Build();

//NOU HOST CA SA NU SE MAI INCURCE CU INPUTUL IN CONSOLA 
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
    })
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

manager.RegisterUser(new Admin("admin", "1234"));

//incarcare datele salvate daca exista------------------------------
var (camere, rezervari) = fileService.Load(); 
manager.LoadData(camere, rezervari);

logger.LogInformation("Aplicatia a pornit cu succes!");
bool autentificare = false;
while (!autentificare)
{
    //Ecran pornire
    Console.WriteLine("1. Login");
    Console.WriteLine("2. Register [Client only]");
    Console.WriteLine("0. Iesire");

    Console.WriteLine("Alege optiunea: ");
    string choice = Console.ReadLine() ?? "";
    if (string.IsNullOrWhiteSpace(choice)) continue;
    Console.WriteLine($"Ai ales optiunea {choice}.");
    
    if (choice == "1")
    {
            Console.WriteLine("Username: ");
            string user = Console.ReadLine() ?? "";
            Console.WriteLine("Parola: ");
            string paro = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(paro))
            {
                var ultilizatorLogat = manager.Authenticate(user, paro);
                Console.WriteLine($"DEBUG: Am primit User: '{user}' si Parola: '{paro}'");

                if (ultilizatorLogat is Admin admin)
                {
                    autentificare = true;
                    RunAdminMenu(manager, admin, fileService);
                }
                else if (ultilizatorLogat is Client client)
                {
                    autentificare = true;
                    RunClientMenu(manager, client, fileService);
                }
                else
                {
                    Console.WriteLine("Date incorecte!");
                }
            }
            else Console.WriteLine("Date invalide!");
    }
    else if (choice == "2")
    {
            Console.WriteLine("\n CREARE CONT CLIENT ");
            Console.WriteLine("Alege Username: "); //de verificat daca nu exista un user deja existent : NU E FACUT 
            string username = Console.ReadLine() ?? "";
            Console.WriteLine("Alege Parola: ");
            string parola = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(parola))
            {
                manager.RegisterUser(new Client(username, parola));
                Console.WriteLine($"DEBUG: Am primit User: '{username}' si Parola: '{parola}'");
                Console.WriteLine("Contul a fost creat cu succes!");
                Console.ReadKey();
            }
            else Console.WriteLine("Date invalide!");
    }
    else if (choice == "0") return;
    
}
void RunAdminMenu(Manager mgr, Admin admin, FileService fs)
{
    bool logout = false;
    while (!logout)
    {
        Console.WriteLine($"\n--- MENIU ADMIN: {admin.Username} ---");
        Console.WriteLine("1. Administrare Camere");
        Console.WriteLine("2. Administrare Rezervari");
        Console.WriteLine("3. Setare Reguli Check in/out");
        Console.WriteLine("0. Logout");
        Console.Write("\nAlege: ");
        
        string opt = Console.ReadLine()?? "";
        Console.WriteLine($"Ai ales optiunea {opt}.");
        
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
                Console.WriteLine("5. Vizualizare Lista Camere");
                Console.WriteLine("0. Logout");
                
                string? opt1 = Console.ReadLine();
                if (opt1 == "1")
                {
                    Console.Write("Introduceti numarul camerei de creat: ");
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
                       if (cameraNoua != null)
                       {
                           mgr.AddRoom(admin, cameraNoua);
                           fs.Save(mgr.GetAllRooms(admin).ToList(),mgr.GetActiveReservations(admin).ToList());
                           Console.WriteLine($"Camera {nr} [{cameraNoua.Type}] a fost adaugata");
                       }
                       else Console.WriteLine("Tip invalid");
                    }
                }
                else if (opt1 == "2")
                {
                    Console.WriteLine("Introduceti nr Camerei de modificat: ");
                    //nu am mai vrut sa ma complic asadar stergem camera si o scriem pe cea noua, deoarece tipul camerei influenteaza pretul si 
                    //nu vreau sa avem bug-uri
                    if (int.TryParse(Console.ReadLine(), out int nr))
                    {
                        mgr.DeleteRoom(admin, nr);
                        Console.WriteLine("Introduceti noile detalii [ca la creare]:\n");
                        Console.WriteLine($"Tip camera:\n1.Single, 2.Double, 3.Triple, 4.Family"); 
                        string tip = Console.ReadLine()?? "";
                        Room? cameraNoua = tip switch
                        {
                            "1" => new SingleRoom(nr),
                            "2" => new DoubleRoom(nr),
                            "3" => new TripleRoom(nr),
                            "4" => new FamilyRoom(nr),
                            _ => null
                        };
                        if (cameraNoua != null)
                        {
                            mgr.AddRoom(admin, cameraNoua);
                            fs.Save(mgr.GetAllRooms(admin).ToList(),mgr.GetActiveReservations(admin).ToList());
                            Console.WriteLine($"Camera {nr} [{cameraNoua.Type}] a fost actualizata");
                        }
                        else Console.WriteLine("Tip invalid");
                    }
                }
                else if (opt1 == "3")
                {
                    Console.Write("Introduceti numarul camerei de sters: ");
                    if (int.TryParse(Console.ReadLine(), out int nr))
                    {
                        mgr.DeleteRoom(admin, nr);
                        fs.Save(mgr.GetAllRooms(admin).ToList(), mgr.GetActiveReservations(admin).ToList());
                        Console.WriteLine($"Camera {nr} a fost stearsa");
                    }
                }
                else if (opt1 == "4")
                {
                    Console.Write("Introduceti numarul camerei pentru a seta status: ");
                    if (int.TryParse(Console.ReadLine(), out int nr))
                    {
                        Console.WriteLine("Status nou: 0.Free | 1.Occupied | 2.Cleaning | 3.Unavailable");
                        if (int.TryParse(Console.ReadLine(), out int st))
                        {
                            mgr.SetRoomStatus(admin, nr, (RoomStatus)st);
                            fs.Save(mgr.GetAllRooms(admin).ToList(), mgr.GetActiveReservations(admin).ToList());
                            Console.WriteLine("Status actualizat");
                        }
                    }
                }
                else if (opt1 == "5")
                {
                    var cam = mgr.GetAllRooms(admin);
                    Console.WriteLine("\nLista Camere:");
                    foreach (var c in cam)
                    {
                        Console.WriteLine($"Nr.: {c.Number}| Tip: {c.GetType().Name} | Status: {c.Status}");
                    }
                }
                else if (opt1 == "0")
                {
                    Console.WriteLine("Ai iesit din meniul de gestionare a camerelor");
                    logout1 = true;
                }
            }
        }
        else if (opt == "2")
        {
            bool logout1 = false;
            while (!logout1)
            {
                Console.WriteLine("\n--- Meniu Administrare Rezervari ---");
                Console.WriteLine("1. Vezi Rezervari Active");
                Console.WriteLine("2. Vezi Rezervari Istorice");
                Console.WriteLine("3. Setare Status Rezervare");
                Console.WriteLine("4. Aprobare rezervari");
                Console.WriteLine("5. Finalizare manuala a rezervarii");
                Console.WriteLine("0. Logout");

                string? opt1 = Console.ReadLine();
                if (opt1 == "1")
                {
                    var activeRes = mgr.GetActiveReservations(admin).ToList();
                    Console.WriteLine("\nREZERVARI ACTIVE");
                    if(!activeRes.Any()) Console.WriteLine("Nu exista rezervari active");
                    foreach (var rez in activeRes)
                    {
                        Console.WriteLine($"ID: {rez.ReservationID} | Client: {rez.Username} | Camera: {rez.RoomNumber} | Status: {rez.Status}");
                    }
                }
                else if (opt1 == "2")
                {
                    var pastRes = mgr.GetHistoryReservations(admin).ToList();
                    Console.WriteLine("\nREZERVARI ISTORICE");
                    if(!pastRes.Any()) Console.WriteLine("Nu exista rezervari active");
                    foreach (var rez in pastRes)
                    {
                        Console.WriteLine($"ID: {rez.ReservationID} | Client: {rez.Username} | Camera: {rez.RoomNumber} | Status: {rez.Status}");
                    }
                }
                else if (opt1 == "3")
                {
                    Console.WriteLine("Introdu ID-ul Rezervarii");
                    string id = Console.ReadLine() ?? "";
                    if (Guid.TryParse(id, out Guid guid))
                    {
                        Console.WriteLine("Status Nou: 0. Pending | 1. Active | 2. Cancelled | 3. Completed");
                        if (int.TryParse(Console.ReadLine(), out int st))
                        {
                            try
                            {
                                mgr.ForceChangeStatus(admin, guid, (ReservationStatus)st);
                                fs.Save(mgr.GetAllRooms(admin).ToList(), mgr.GetActiveReservations(admin).ToList());
                                Console.WriteLine("Statusul rezervarii a fost actualizat");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Eroare");
                                //throw;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID-ul introdus nu este un format GUID valabil");
                    }
                    Console.WriteLine("Apasa orice tasta pentru a continua....");
                    Console.ReadKey();
                }
                else if (opt1 == "4")
                {
                    Console.WriteLine("Introdu GUID-ul Rezervarii pt confirmare");
                    if (Guid.TryParse(Console.ReadLine(), out Guid id))
                    {
                        mgr.AdminConfirmReservation(admin, id);
                        fs.Save(mgr.GetAllRooms(admin).ToList(), mgr.GetActiveReservations(admin).ToList());
                        Console.WriteLine("Rezervare confirmata. Camera ocupata");
                    }
                }
                else if (opt1 == "5")
                {
                    Console.WriteLine("Introdu GUID-ul Rezervarii pt finalizare");
                    if (Guid.TryParse(Console.ReadLine(), out Guid id))
                    {
                        mgr.AdminFinalizeReservation(admin, id);
                        fs.Save(mgr.GetAllRooms(admin).ToList(), mgr.GetActiveReservations(admin).ToList());
                        Console.WriteLine("Sejur finalizat manual. Camera este libera");
                    }
                }
                else if (opt1 == "0")
                {
                    Console.WriteLine("Ai iesit din meniul de gestionare a rezervarilor");
                    logout1 = true;
                }
            }
        }
        else if (opt == "3")
        {
            Console.WriteLine("CONFIGURARE REGULI HOTEL [CHECK IN/OUT]");
            Console.WriteLine("Ora check in [hh:mm]: ");
            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan checkin))
            {
                Console.WriteLine("Ora check out [hh:mm]: ");
                if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan checkout))
                {
                    mgr.UpdateCheckInRules(admin, checkin, checkout);
                    Console.WriteLine("Regulile au fost actualizate");
                }
            }
            Console.ReadKey();
        }
        else if (opt == "0")
        {
            Console.WriteLine("Ai iesit din aplicatie ");
            logout = true;
        }
    }
}

void RunClientMenu(Manager mgr, Client client, FileService fs)
{
    //aici la fel
    bool logout = false;
    while (!logout)
    {
        Console.WriteLine($"\n--- MENIU CLIENT: {client.Username} ---");
        Console.WriteLine("1. Cauta Camere disponibile");
        Console.WriteLine("2. Creaza o cerere pentru rezervare");// nu uita de metoda de admin
        Console.WriteLine("3. Gestionare rezervari personale");
        Console.WriteLine("4. Istoric Sejururi");
        Console.WriteLine("0. Logout");
        Console.Write("Alege: ");

        string? opt = Console.ReadLine();
        if (opt == "1")
        {
            //var disp = mgr.GetAllRooms(new Admin("system", "system"))
            //    .Where(r => r.Status == RoomStatus.Free).ToList();
            var disp = mgr.GetRoomsForClient();
            Console.WriteLine("    CAMERE DISPONIBILE    ");
            if(!disp.Any()) Console.WriteLine("Nu sunt camere disponibile");
            foreach (var r in disp)
            {
                Console.WriteLine($"Nr.: {r.Number} | Tip: {r.Type} | Pret: {r.PricePerNight} | Facilitati: {r.Facilities};");
            }
            Console.WriteLine("\n Apasa orice tasta pt a reveni....");
            Console.ReadKey();
        }
        else if (opt == "2")
        {
            Console.WriteLine("Introdu nr. camerei dorite: ");
            int nr = int.Parse(Console.ReadLine()?? "");
            
            Console.WriteLine("Data start (yyyy-mm-dd): ");
            DateTime sd = DateTime.Parse(Console.ReadLine() ?? "");
            
            Console.WriteLine("Data end (yyyy-mm-dd): ");
            DateTime ed = DateTime.Parse(Console.ReadLine() ?? "");
            try
            {
                mgr.ClientRequestReservation(client, nr, sd, ed);
                fs.Save(mgr.GetAllRooms( new Admin("sys","sys")).ToList(),
                    mgr.GetActiveReservations(new Admin("sys","sys")).ToList());
                Console.WriteLine("Cererea a fost trimisa! Asteapta confirmarea adminului");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //throw;
            }
        }
        else if (opt == "3")
        {
            Console.Clear();
            var mele = mgr.GetMyReservations(client).ToList();
            Console.WriteLine("REZERVARILE TALE");
            foreach (var r in mele)
                Console.WriteLine($"ID: {r.ReservationID.ToString()} | Camera: {r.RoomNumber} | Status: {r.Status}");
            Console.WriteLine("\n1. Check in");
            Console.WriteLine("2. Check out");
            Console.WriteLine("3. Anulare rezervare");
            Console.WriteLine("\nAlege: ");
            string subopt = Console.ReadLine() ?? "";
            
            Console.WriteLine("Introdu ID-ul rezervarii [guid]: ");
            string inputId = Console.ReadLine() ?? "";

            if (Guid.TryParse(inputId, out Guid guid))
            {
                try
                {
                    if (subopt == "1")
                    {
                        mgr.PerformCheckin(client, guid);
                        Console.WriteLine("Check in reusit");
                    }
                    else if (subopt == "2")
                    {
                        mgr.PerformCheckout(client, guid);
                        Console.WriteLine("Check out reusit");
                    }
                    else if (subopt == "3")
                    {
                        mgr.CancelReservation(client, guid);
                        Console.WriteLine("Rezervare anulata");
                    }
                    fs.Save(mgr.GetAllRooms( new Admin("sys","sys")).ToList(),mgr.GetActiveReservations(new Admin("sys","sys")).ToList());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //throw;
                }
            }
            else
            {
                Console.WriteLine("ID invalid");
            }
            Console.WriteLine("\n apasa orice tasta pt a continua...");
            Console.ReadKey();
        }
        else if (opt == "4")
        {
            var istoric = mgr.GetMyHistory(client).ToList();
            Console.WriteLine("\n--- ISTORIC REZERVARI ---");
            if (!istoric.Any()) Console.WriteLine("Nu ai sejururi finalizate.");
    
            foreach (var s in istoric)
            {
                Console.WriteLine($"Cazare: {s.StartDate:dd/MM} - {s.EndDate:dd/MM} | Camera: {s.RoomNumber}");
            }
            Console.WriteLine("\nApasa orice tasta pentru a reveni...");
            Console.ReadKey();
        }
        else if (opt == "0") logout = true;
    }
}
