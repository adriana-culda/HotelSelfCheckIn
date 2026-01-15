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

        // Daca folderul nu exista, il cream

        if (!Directory.Exists(_dataFolder))
        {
            Directory.CreateDirectory(_dataFolder);
        }
    }
    
    public void Save<T>(string fileName, T data)
    {
        string fullPath = Path.Combine(_dataFolder, fileName);
        
      
        var options = new JsonSerializerOptions { WriteIndented = true };
        
       
        string json = JsonSerializer.Serialize(data, options);
        
     
        File.WriteAllText(fullPath, json);
    }
    
    public T Load<T>(string fileName)
    {
        string fullPath = Path.Combine(_dataFolder, fileName);

        // Daca fisierul nu exista, returnam o valoare goala
        if (!File.Exists(fullPath))
        {
            return Activator.CreateInstance<T>();
        }

        // Citim textul si Il transformam Inapoi In obiecte
        
        string json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<T>(json);
    }
}

