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

    public (List<Room>, List<Reservation>) Load()
    {
        
        string pathCamere = Path.Combine(FolderName, NumeFisierCamere);
        string pathRezervari = Path.Combine(FolderName, NumeFisierRezervari);

        var rooms = LoadData<Room>(pathCamere);
        var reservations = LoadData<Reservation>(pathRezervari);
        
        return (rooms, reservations);
    }

   
    
    
    public HotelSettings LoadSettings()
    {
        string path = Path.Combine(FolderName, NumeFisierSetari);
        var list = LoadData<HotelSettings>(path);
    
        
        return list.Count > 0 ? list[0] : new HotelSettings();
    }

// 3. Metoda de Save pentru Setări
    public void SaveSettings(HotelSettings settings)
    {
        // Salvăm ca listă (deși e un singur obiect) pentru a refolosi metoda generică Save<T>
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
    // -----------------------------------------------

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

            // Dacă am găsit folderul proiectului
            if (directoryInfo != null)
            {
                string projectPath = directoryInfo.FullName;
                
                // Construim calea către folderul SavedData din sursă
                string sourceFolderPath = Path.Combine(projectPath, FolderName);

                // Ne asigurăm că folderul există și în sursă (dacă l-ai șters din greșeală)
                if (!Directory.Exists(sourceFolderPath))
                {
                    Directory.CreateDirectory(sourceFolderPath);
                }

                string sourceFilePath = Path.Combine(sourceFolderPath, fileName);
                
                // Scriem fișierul fizic în proiect
                File.WriteAllText(sourceFilePath, json);
            }
        }
        catch 
        {
            // Ignorăm erorile de sincronizare ca să nu crăpăm aplicația
        }
#endif
    }
    public void SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("users.json", json);
    }
}