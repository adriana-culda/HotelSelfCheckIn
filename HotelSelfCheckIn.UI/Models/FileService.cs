using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.Models;

public class FileService
{

    private const string FolderName = "SavedData";
    private const string NumeFisierCamere = "camere.json";
    private const string NumeFisierRezervari = "rezervari.json";
    private const string NumeFisierSetari = "settings.json";

    public FileService()
    {

        if (!Directory.Exists(FolderName))
        {
            Directory.CreateDirectory(FolderName);
        }
    }

    public (List<Room>, List<Reservation>,List<User>) Load()
    {

        string pathCamere = Path.Combine(FolderName, NumeFisierCamere);
        string pathRezervari = Path.Combine(FolderName, NumeFisierRezervari);
        string pathUsers = Path.Combine(FolderName, "users.json");

        var rooms = LoadData<Room>(pathCamere);
        var reservations = LoadData<Reservation>(pathRezervari);
        var users = LoadData<User>(pathUsers);

        return (rooms, reservations,users);
    }




    public HotelSettings LoadSettings()
    {
        string path = Path.Combine(FolderName, NumeFisierSetari);
        var list = LoadData<HotelSettings>(path);


        return list.Count > 0 ? list[0] : new HotelSettings();
    }

// 3. Metoda de Save pentru Setari
    public void SaveSettings(HotelSettings settings)
    {
        // Salvam ca lista (desi e un singur obiect) pentru a refolosi metoda generica Save<T>
        Save(NumeFisierSetari, new List<HotelSettings> { settings });
    }

    public void SaveRooms(List<Room> rooms)
    {
        Save(NumeFisierCamere, rooms);
    }

    public void SaveReservations(List<Reservation> reservations)
    {
        Save(NumeFisierRezervari, reservations);
    }
    
    private List<T> LoadData<T>(string path)
    {
        if (!File.Exists(path))
        {
            return new List<T>();
        }

        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
        catch
        {
            return new List<T>();
        }
    }


    private void Save<T>(string fileName, List<T> data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(data, options);


        string pathBin = Path.Combine(FolderName, fileName);
        File.WriteAllText(pathBin, json);


#if DEBUG
        try
        {

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo directoryInfo = new DirectoryInfo(baseDir);



            while (directoryInfo != null && !directoryInfo.GetFiles("*.csproj").Any())
            {
                directoryInfo = directoryInfo.Parent;
            }

            // Daca am gasit folderul proiectului
            if (directoryInfo != null)
            {
                string projectPath = directoryInfo.FullName;

                // Construim calea catre folderul SavedData din sursa
                string sourceFolderPath = Path.Combine(projectPath, FolderName);

                // Ne asiguram ca folderul exista si In sursa (daca l-ai sters din greseala)
                if (!Directory.Exists(sourceFolderPath))
                {
                    Directory.CreateDirectory(sourceFolderPath);
                }

                string sourceFilePath = Path.Combine(sourceFolderPath, fileName);

                // Scriem fisierul fizic In proiect
                File.WriteAllText(sourceFilePath, json);
            }
        }
        catch
        {
            // Ignoram erorile de sincronizare ca sa nu crapam aplicatia
        }
#endif
    }

    public void SaveUsers(List<User> users)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var dataToSave=users.Cast<object>().ToList();
        // Salvam ca List<object> pentru ca JSON sa includa si campurile de Client (Name, Email, Phone)
        string json = JsonSerializer.Serialize<List<object>>(dataToSave, options);

        string path = Path.Combine(FolderName, "users.json");
        File.WriteAllText(path, json);
    }
}