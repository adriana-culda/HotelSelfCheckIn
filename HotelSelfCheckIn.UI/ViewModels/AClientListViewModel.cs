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
        
        // Acum _manager.GetAllClients() returneaza List<Client>, 
        // deci constructorul ObservableCollection(IEnumerable<Client>) este gasit
        Clients = new ObservableCollection<Client>(_manager.GetAllClients());
    }
}