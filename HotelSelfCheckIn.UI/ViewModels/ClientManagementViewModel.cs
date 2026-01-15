using System.Collections.ObjectModel;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class ClientManagementViewModel : ViewModelBase
{
    public ObservableCollection<User> Clients { get; set; }

    public ClientManagementViewModel(Manager manager)
    {
        // manager.GetAllClients() acum returneaza List<Client>
        var realClients = manager.GetAllClients();
        
        // Acum tipurile se potrivesc perfect
        Clients = new ObservableCollection<User>(realClients);
    }
}