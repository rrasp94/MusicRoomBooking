# Music Room Booking

Web aplikacija za rezervaciju glazbenih prostorija. Korisnici mogu pregledavati
prostorije i njihovu opremu, rezervirati slobodne termine, ostavljati recenzije
te čitati vijesti. Administratori upravljaju prostorijama, opremom, rezervacijama
i vijestima.

Projekt je izrađen kao seminarski rad.

## Tehnologije

- ASP.NET Core 8 – **Blazor Server** (Interactive Server render mode)
- **Entity Framework Core 8** (Code First) + **SQL Server LocalDB**
- **ASP.NET Core Identity** (autentifikacija i role)
- **Bootstrap 5** za izgled

Namjerno bez naprednih arhitektura (Clean Architecture, CQRS, MediatR,
repository pattern) – `DbContext` se koristi izravno u komponentama.

## Funkcionalnosti

### Korisnici
- Registracija, prijava i odjava (ASP.NET Identity)
- Dvije role: **User** i **Admin**

### Prostorije
- Naziv, opis, kapacitet, fotografija (URL), status (aktivna/neaktivna)
- Korisnici pregledavaju aktivne prostorije i njihove detalje
- Administrator dodaje, uređuje i briše prostorije

### Oprema
- Svaka prostorija ima popis opreme (npr. bubnjevi, pojačalo, klavir)
- Administrator upravlja opremom po prostoriji
- Oprema je vidljiva na stranici detalja prostorije

### Rezervacije
- Prijavljeni korisnik rezervira termin za prostoriju
- Sustav ne dopušta preklapanje termina (dvostruku rezervaciju)
- Korisnik vidi svoje rezervacije i može otkazati buduće
- Administrator vidi sve rezervacije, može ih obrisati i uređivati termine

### Recenzije i ocjene
- Recenziju (ocjena 1–5 + komentar) može ostaviti samo korisnik koji je imao
  završenu rezervaciju za tu prostoriju
- Na stranici prostorije prikazuju se prosječna ocjena i komentari

### Vijesti
- Administrator dodaje, uređuje i briše vijesti
- Korisnici čitaju vijesti

## Model baze podataka

Glavni entiteti (EF Core Code First):

- `ApplicationUser` – korisnik (proširuje `IdentityUser` s imenom i prezimenom)
- `Room` – prostorija
- `Equipment` – oprema (pripada prostoriji)
- `Reservation` – rezervacija (prostorija + korisnik + termin)
- `Review` – recenzija (prostorija + korisnik + ocjena + komentar)
- `News` – vijest

## Pokretanje

Preduvjeti: **.NET 8 SDK** i **SQL Server LocalDB** (dolazi uz Visual Studio;
connection string se nalazi u `appsettings.json`).

```bash
# 1. Primijeni migracije i kreiraj bazu
dotnet ef database update

# 2. Pokreni aplikaciju
dotnet run
```

Aplikacija se pokreće na `http://localhost:5024`.

Pri prvom pokretanju automatski se kreiraju role (**Admin**, **User**) i
demo administrator:

- **Email:** `admin@musicrooms.local`
- **Lozinka:** `Admin123!`

Novi korisnici se registriraju preko stranice **Register** i dobivaju rolu **User**.

## Struktura projekta

```
Components/
  Layout/            zajednički izgled (navigacija, glavni layout)
  Pages/             korisničke stranice (Home, Rooms, RoomDetails, Reserve, ...)
    Admin/           administratorske stranice (upravljanje prostorijama, opremom, ...)
  Account/           Identity stranice (login, register, upravljanje računom)
Data/
  ApplicationDbContext.cs   EF Core kontekst
  ApplicationUser.cs        korisnički model
  SeedData.cs               kreiranje rola i demo admina
  Migrations/               EF Core migracije
Models/              domenski modeli (Room, Equipment, Reservation, Review, News)
```
