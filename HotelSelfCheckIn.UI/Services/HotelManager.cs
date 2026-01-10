using System.Collections.Generic;
using System.Linq;
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.Data; // <--- Importăm namespace-ul unde ai pus DataService

namespace HotelSelfCheckIn.Services
{
    public class HotelManager
    {
        private readonly DataService _dataService; // Folosim clasa ta DataService
        private List<Camera> _toateCamerele;
        private const string CAMERE_FILE = "camere.json";

        // Constructorul primește DataService-ul tău
        public HotelManager(DataService dataService)
        {
            _dataService = dataService;
            
            // Folosim metoda ta .Load()
            _toateCamerele = _dataService.Load<List<Camera>>(CAMERE_FILE);
            
            // Dacă lista e goală (fișierul nu exista), creăm datele
            if (_toateCamerele == null || !_toateCamerele.Any())
            {
                _toateCamerele = new List<Camera>(); // Inițializăm lista dacă e null
                SeedData();
            }
        }

        public IEnumerable<Camera> GetToateCamerele()
        {
            return _toateCamerele;
        }

        public bool CheckInCamera(int numarCamera)
        {
            var index = _toateCamerele.FindIndex(c => c.Numar == numarCamera);
            if (index == -1) return false;

            var camera = _toateCamerele[index];
            if (camera.Status != StatusCamera.Libera) return false;

            // Modificăm statusul
            var cameraOcupata = camera with { Status = StatusCamera.Ocupata };
            _toateCamerele[index] = cameraOcupata;
            
            SaveChanges();
            return true;
        }

        public void CheckOutCamera(int numarCamera)
        {
            var index = _toateCamerele.FindIndex(c => c.Numar == numarCamera);
            if (index != -1)
            {
                var camera = _toateCamerele[index];
                var cameraInCuratenie = camera with { Status = StatusCamera.InCuratenie };
                _toateCamerele[index] = cameraInCuratenie;
                
                SaveChanges();
            }
        }

        private void SaveChanges()
        {
            // Folosim metoda ta .Save()
            _dataService.Save(CAMERE_FILE, _toateCamerele);
        }

        private void SeedData()
        {
            // Aici se creează datele inițiale
            _toateCamerele.Add(new CameraSingle(101, new List<string> { "TV", "Wi-Fi" }) 
            { 
                Status = StatusCamera.Libera 
            });

            _toateCamerele.Add(new CameraDubla(102, new List<string> { "TV", "Wi-Fi", "MiniBar" }) 
            { 
                Status = StatusCamera.Ocupata 
            });

            // AICI se creează efectiv fișierul 'camere.json' pe disc prima dată!
            SaveChanges();
        }
    }
}