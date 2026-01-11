using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.Models;

public class FileService
{
    // DEFINIM CĂILE CORECT
    private const string FolderName = "SavedData";
    private const string NumeFisierCamere = "camere.json";
    private const string NumeFisierRezervari = "rezervari.json";

    public FileService()
    {
        // Asigurăm că folderul există la pornire
        if (!Directory.Exists(FolderName))
        {
            Directory.CreateDirectory(FolderName);
        }
    }

    public (List<Room>, List<Reservation>) Load()
    {
        // Construim calea completă: SavedData/camere.json
        string pathCamere = Path.Combine(FolderName, NumeFisierCamere);
        string pathRezervari = Path.Combine(FolderName, NumeFisierRezervari);

        var rooms = LoadData<Room>(pathCamere);
        var reservations = LoadData<Reservation>(pathRezervari);
        
        return (rooms, reservations);
    }

    // --- METODELE NOI PENTRU SALVARE (SEED DATA) ---
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
    
    // Metoda generică de salvare
    private void Save<T>(string fileName, List<T> data)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(data, options);

        // 1. SALVARE STANDARD (în bin/Debug/...)
        // Asta face ca aplicația să funcționeze pe moment
        string pathBin = Path.Combine(FolderName, fileName);
        File.WriteAllText(pathBin, json);

        // 2. SALVARE ÎN PROIECT (SYNC CU RIDER)
        // Acest bloc de cod rulează doar cât timp programăm (Debug mode)
#if DEBUG
        try 
        {
            // Calea de bază e: .../bin/Debug/net9.0-windows/
            // Trebuie să urcăm 3 nivele (../) ca să ajungem la folderul proiectului
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Urcăm 3 nivele: net9.0 -> Debug -> bin -> Proiect
            string projectPath = Directory.GetParent(baseDir).Parent.Parent.FullName;
            
            // Construim calea către folderul SavedData din sursă
            string sourcePath = Path.Combine(projectPath, FolderName, fileName);

            // Scriem fișierul și aici!
            File.WriteAllText(sourcePath, json);
        }
        catch 
        {
            // Dacă nu reușește să găsească calea, nu crăpăm aplicația, doar ignorăm
        }
#endif
    }
}