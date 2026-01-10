using System.Collections.Generic;
using HotelSelfCheckIn.UI.Data;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.Services;

public class HotelService
{
    private readonly DataService _dataService;
    private const string FileName = "camere.json";

    public HotelService(DataService dataService)
    {
        _dataService = dataService;
        EnsureDefaultRoomsExist();
    }

    public List<Camera> GetToateCamerele()
    {
        return _dataService.Load<List<Camera>>(FileName) ?? new List<Camera>();
    }

    public void SalveazaListaCamere(List<Camera> camereModificate)
    {
        _dataService.Save(FileName, camereModificate);
    }

    private void EnsureDefaultRoomsExist()
    {
        var camere = _dataService.Load<List<Camera>>(FileName);

        if (camere == null || camere.Count == 0)
        {
            camere = new List<Camera>
            {
                // Etajul 1 - Single
                new CameraSingle(101),
                new CameraSingle(102),
                new CameraSingle(103) with { Status = StatusCamera.Ocupata },

                // Etajul 2 - Duble
                new CameraDubla(201),
                new CameraDubla(202),

                // Etajul 3 - Triple si Familiale
                new CameraTripla(301),
                new CameraFamiliala(401)
            };

            _dataService.Save(FileName, camere);
        }
    }
}