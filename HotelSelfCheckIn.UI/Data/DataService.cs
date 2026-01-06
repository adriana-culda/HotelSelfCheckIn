using System;
using System.IO;
using System.Text.Json;



namespace HotelSelfCheckIn.UI.Data;

public class DataService
{
    // Folderul unde se vor salva toate fisierele

    private readonly string _dataFolder;

    public DataService(string dataFolder)
    {
        string appPath = AppDomain.CurrentDomain.BaseDirectory;
        _dataFolder = Path.Combine(appPath, "SavedData");

        // Dacă folderul nu exista, îl cream

        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }
    }
    
    public void Save<T>(string fileName, T data)
    {
        string fullPath = Path.Combine(_dataFolder, fileName);
        
        //  Trebuie sa fie citibil de oameni, cu spatii si randuri noi
        var options = new JsonSerializerOptions { WriteIndented = true };
        
        // Transformam obiectul în text
        string json = JsonSerializer.Serialize(data, options);
        
        // Scriem textul în fisier
        File.WriteAllText(fullPath, json);
    }
    
    public T Load<T>(string fileName)
    {
        string fullPath = Path.Combine(_dataFolder, fileName);

        // Daca fisierul nu exista, returnăm o valoare goala
        if (!File.Exists(fullPath))
        {
            return Activator.CreateInstance<T>();
        }

        // Citim textul și îl transformăm înapoi în obiecte
        
        string json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<T>(json);
    }
}

