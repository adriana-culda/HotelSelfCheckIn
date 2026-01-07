using System;
using System.Collections.Generic;
using System.IO; 
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelSelfCheckIn.UI.Models;

//File service este memoria aplicatiei!! (Sa nu se stearga datele cand se inchide aplicatia)
namespace HotelSelfCheckIn.UI.Models
{
    public class FileService
    {
        private const string RoomsFile = "camere.json";
        private const string ResFile = "rezervari.json";

        //optiune pt salvare/recunoastere tipuri camere
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true, //<- pentru a fi mai usor de citit
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public void Save(List<Room> rooms, List<Reservation> res)
        {
            try
            {
                string roomsJson = JsonSerializer.Serialize(rooms, _options);
                File.WriteAllText(RoomsFile, roomsJson);

                string resJson = JsonSerializer.Serialize(res, _options);
                File.WriteAllText(ResFile, resJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la salvare: {ex.Message}");
            }
        }

        public (List<Room>, List<Reservation>) Load()
        {
            try
            {
                List<Room> rooms = new();
                List<Reservation> res = new();

                if (File.Exists(RoomsFile))
                {
                    string json = File.ReadAllText(RoomsFile);
                    rooms = JsonSerializer.Deserialize<List<Room>>(json, _options) ?? new();
                }

                if (File.Exists(ResFile))
                {
                    string json = File.ReadAllText(ResFile);
                    res = JsonSerializer.Deserialize<List<Reservation>>(json, _options) ?? new();
                }

                return (rooms, res);
            }
            catch
            {
                // Se returneaza o lista goala daca fisierul e corrupt
                return (new List<Room>(), new List<Reservation>());
            }
        }
    }
}