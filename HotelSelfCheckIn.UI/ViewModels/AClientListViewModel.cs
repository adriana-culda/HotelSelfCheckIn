using System.Collections.ObjectModel;
using System.Linq;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class AClientListViewModel : ViewModelBase
{
    private readonly Manager _manager;

    public ObservableCollection<Client> Clients { get; set; }

    public AClientListViewModel(Manager manager)
    {
        _manager = manager;
        var clientData = _manager.GetAllClients().OfType<Client>();
    
        Clients = new ObservableCollection<Client>(clientData);
    }
}