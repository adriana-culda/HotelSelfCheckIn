using System.Collections.Generic;
using System.Linq;
using HotelSelfCheckIn.UI.Models;
using HotelSelfCheckIn.UI.Data;

namespace HotelSelfCheckIn.Services
{
    public class HotelManager
    {
        private readonly DataService _dataService; 
        private List<Camera> _toateCamerele;
        private const string CAMERE_FILE = "camere.json";

        
        public HotelManager(DataService dataService)
        {
            _dataService = dataService;
            
           
            _toateCamerele = _dataService.Load<List<Camera>>(CAMERE_FILE);
            
            // Daca lista e goala (fisierul nu exista), cream datele
            if (_toateCamerele == null || !_toateCamerele.Any())
            {
                _toateCamerele = new List<Camera>(); // Initializam lista daca e null
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

            // Modificam statusul
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
            
            _dataService.Save(CAMERE_FILE, _toateCamerele);
        }

        private void SeedData()
        {
            // Aici se creeaza datele initiale
            _toateCamerele.Add(new CameraSingle(101, new List<string> { "TV", "Wi-Fi" }) 
            { 
                Status = StatusCamera.Libera 
            });

            _toateCamerele.Add(new CameraDubla(102, new List<string> { "TV", "Wi-Fi", "MiniBar" }) 
            { 
                Status = StatusCamera.Ocupata 
            });

            // AICI se creeaza efectiv fisierul 'camere.json' pe disc prima data
            SaveChanges();
        }
    }
}