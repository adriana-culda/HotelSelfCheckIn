using System.Collections.ObjectModel;
using HotelSelfCheckIn.UI.Models;

namespace HotelSelfCheckIn.UI.ViewModels;

public class ClientManagementViewModel : ViewModelBase
{
    public ObservableCollection<User> Clients { get; set; }

    public ClientManagementViewModel(Manager manager)
    {
        // Managerul tău ar trebui să aibă o metodă GetClients()
        // Dacă nu are, adaug-o în Manager.cs: public IEnumerable<User> GetClients() => _users.Where(u => u is Client);
        
        // Mockup pentru exemplu dacă nu ai metoda încă:
        Clients = new ObservableCollection<User>();
        Clients.Add(new Client("client1", "pass") { Name = "Ion Popescu", Email = "ion@test.com" ,Phone ="+40719459130"});
    }
}