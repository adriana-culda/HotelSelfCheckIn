# Hotel Self Check-In System (C# Desktop App)

Acest proiect reprezintă un **sistem integrat pentru gestiunea unui micro-hotel**, facilitând procesul de rezervare și check-in/out automatizat. Aplicația este dezvoltată ca **proiect pentru facultate**, vizând implementarea unor concepte avansate de **Programare Orientată Obiect** și **Arhitectură Software**.

###  Arhitectură și Concepte Tehnice
Pentru a asigura un cod curat și scalabil, am implementat următoarele principii:

* **Arhitectură pe Layere / MVVM:** Separarea clară între interfața grafică și logica de business.
* **Domain-Driven Design (DDD) Style:** Utilizarea unui **Aggregate Root** pentru gestionarea coerentă a rezervărilor și camerelor, fără setteri publici.
* **Imutabilitate:** Entitățile esențiale sunt proiectate folosind **tipuri imutabile**.
* **Dependency Injection:** Gestionarea serviciilor prin **.NET Core GenericHost**.
* **Logging:** Monitorizarea acțiunilor și erorilor folosind **ILogger**.
* **Persistența Datelor:** Salvare și încărcare automată din **fișiere** (cu tratarea excepțiilor pentru date invalide sau lipsă).

### Roluri și Funcționalități

####  Administrator Hotel
* **Gestiune Camere:** Operații **CRUD** pe entitățile de tip cameră și setarea stării (*Liber, Ocupat, Curățenie, Indisponibil*).
* **Monitorizare Rezervări:** Vizualizarea istoricului și a rezervărilor active.
* **Configurare:** Definirea regulilor de check-in/out și a limitărilor de utilizare.

####  Client
* **Booking Engine:** Căutare camere după perioadă sau facilități și **creare rezervări**.
* **Self Service:** Efectuarea digitală a procesului de **check-in și check-out**.
* **Personal Dashboard:** Gestionarea, anularea și vizualizarea istoricului sejururilor proprii.

###  Persistența Datelor
Aplicația pune accent pe **robustetea datelor**:
* **Salvare Automată:** Toate modificările asupra utilizatorilor și camerelor sunt salvate în fișiere.
* **Error Handling:** Gestionarea scenariilor în care fișierele sunt corupte sau lipsesc prin inițializare controlată.

###  Tehnologii Utilizate
* **Limbaj:** **C# (.NET Core)**
* **IDE principal:** **JetBrains Rider** 
* **UI Framework:** **WPF**
* **Biblioteci:** `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.Logging`
* **Version Control:** Git
