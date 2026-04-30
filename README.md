E-Commerce Cart & Checkout API – PC Shop

Acest proiect a fost realizat ca exercițiu tehnic pentru Quest Global, cu scopul de a simula un flux real de e-commerce pentru componente PC. Dincolo de cerințele funcționale, am urmărit să înțeleg cum circulă datele într-o aplicație fullstack și unde apar riscuri reale în zona de checkout și manipulare a datelor din client.

Cum am abordat proiectul

Am tratat aplicația ca pe un sistem mic, dar real: un magazin online în care cel mai important aspect nu este doar afișarea produselor, ci corectitudinea datelor și controlul logicii de business.

Am pus accent pe:

separarea clară între frontend și backend
controlul complet al logicii de preț pe server
simplitate în frontend, dar reactivitate bună în interfață
Backend (.NET C# Web API)

Pe backend am ales o abordare mai apropiată de nivelul de bază al comunicării cu baza de date:

Am evitat Entity Framework intenționat
Am folosit ADO.NET și query-uri SQL directe pentru a înțelege mai bine cum funcționează accesul la date “raw” și ce presupune controlul manual al query-urilor.
Logica de preț rămâne exclusiv pe server
Clientul trimite doar ID-urile produselor. Backend-ul este responsabil de calculul final al comenzii, citind prețurile din baza de date.
Am făcut asta pentru a elimina complet orice risc de manipulare din partea clientului (ex: modificarea prețurilor din browser).
Dependency Injection
Am separat logica în servicii pentru a evita coupling-ul între controllere și business logic. Asta face codul mai ușor de extins și testat.
Frontend (Angular)

Pe frontend am urmărit mai mult experiența utilizatorului și modul în care se propagă starea în aplicație:

State management cu RxJS (BehaviorSubject)
Coșul de cumpărături este centralizat într-un service, ceea ce permite actualizări în timp real în întreaga aplicație (badge, total, UI) fără refresh.
Structură modulară
Aplicația este împărțită în componente clare: Product List, Cart, Checkout, Login și Register. Am încercat să păstrez fiecare responsabilitate izolată.
UI simplu și funcțional
Am folosit Bootstrap pentru a menține interfața curată și responsive, fără a complica partea de design.
Tehnologii și setup
Frontend
Angular CLI – structură și build al aplicației
RxJS – gestionarea stării și a fluxului de date
Bootstrap – UI rapid și consistent
Backend
ASP.NET Core Web API
ADO.NET (Microsoft.Data.SqlClient) – acces direct la SQL Server
CORS configurat manual pentru comunicarea cu frontend-ul
JSON serialization pentru schimb corect de date între client și server
Cum rulezi proiectul local
1. Baza de date
Deschide folderul /Database
Rulează script.sql în SQL Server Management Studio
(creează tabelele și adaugă date de test)
2. Backend
Deschide /Backend în Visual Studio
Verifică connection string-ul din appsettings.json
Rulează aplicația (F5)
3. Frontend
Deschide /Frontend

Rulează:

npm install
ng serve

Accesează: http://localhost:4200
Ce am învățat din proiect

Acest proiect m-a ajutat să înțeleg mai bine:

cum se separă corect responsabilitățile într-o aplicație fullstack
de ce logica de business nu trebuie să stea niciodată în frontend
cum funcționează efectiv comunicarea frontend–backend fără “abstracții magice” precum ORM-uri
importanța stării aplicației într-un UI modern
Zone de îmbunătățire

Dacă aș continua proiectul într-un context mai apropiat de producție, aș adăuga:

teste unitare (xUnit) pentru servicii și logica de business
validări mai robuste pe autentificare și formulare
storage extern pentru imagini 
logging și eventual un layer de monitoring pentru backend



<img width="1907" height="992" alt="image" src="https://github.com/user-attachments/assets/cca749e8-43a7-42e3-a644-c1eda8f97dc1" />



<img width="1896" height="639" alt="image" src="https://github.com/user-attachments/assets/725d3d6d-3a94-42be-800c-c87c8ad81203" />

