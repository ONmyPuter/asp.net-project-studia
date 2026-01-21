# Dokumentacja Systemu Rezerwacji Samochodów

## 1. Wymagania Systemowe

Aby uruchomić aplikację, wymagane jest zainstalowane środowisko uruchomieniowe oraz SDK dla platformy .NET.

*   **System operacyjny**: Windows, macOS lub Linux.
*   **Platforma .NET**: Wersja 10.0 (zgodnie z konfiguracją projektu).
*   **Baza danych**: SQLite (wbudowana, nie wymaga osobnej instalacji serwera).

## 2. Instalacja i Konfiguracja

### Krok 1: Pobranie kodu
Pobierz kod źródłowy aplikacji do wybranego katalogu na dysku.

### Krok 2: Konfiguracja połączenia z bazą danych
Aplikacja korzysta z bazy danych SQLite. Łańcuch połączenia (Connection String) znajduje się w pliku `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=carreservation.db"
}
```

Plik bazy danych `carreservation.db` zostanie utworzony automatycznie w głównym katalogu aplikacji przy pierwszym uruchomieniu, jeśli nie istnieje.

### Krok 3: Uruchomienie aplikacji
Otwórz terminal w katalogu głównym projektu (tam, gdzie znajduje się plik `CarReservationSystemApp.csproj`) i wykonaj następujące polecenia:

1.  Przywrócenie zależności:
    ```bash
    dotnet restore
    ```
2.  Budowanie projektu:
    ```bash
    dotnet build
    ```
3.  Uruchomienie:
    ```bash
    dotnet run
    ```

Po uruchomieniu aplikacja będzie dostępna pod adresem: `http://localhost:5xxx` (port zostanie wyświetlony w konsoli).

## 3. Użytkownicy i Bezpieczeństwo

System posiada wbudowany mechanizm "Seedowania" danych, który przy pierwszym uruchomieniu tworzy domyślne role oraz konto administratora.

### Domyślny Administrator
*   **Email (Login)**: `admin@test.com`
*   **Hasło**: `Admin123!`

### Rejestracja Użytkowników
Nowi użytkownicy mogą zakładać konta samodzielnie poprzez formularz rejestracji dostępny w aplikacji. Domyślnie nowe konta otrzymują rolę "User" (Użytkownik standardowy).

### Role w systemie
*   **Admin**: Pełny dostęp do zarządzania flotą samochodów, lokalizacjami oraz podgląd i edycja wszystkich rezerwacji.
*   **User**: Możliwość przeglądania oferty, tworzenia własnych rezerwacji oraz podglądu historii swoich rezerwacji.

## 4. Opis Działania Aplikacji (Podręcznik Użytkownika)

### Strona Główna
Po wejściu na stronę główną użytkownik widzi powitanie oraz nawigację do głównych sekcji serwisu.

### Rezerwacja Samochodu
Proces rezerwacji składa się z następujących kroków:
1.  **Wybór Samochodu**: Użytkownik może przeglądać listę dostępnych pojazdów w zakładce "Samochody".
2.  **Szczegóły Rezerwacji**: Po kliknięciu "Zarezerwuj" (lub "Book Now"), użytkownik zostaje przeniesiony do formularza rezerwacji.
3.  **Formularz**: Należy uzupełnić:
    *   Datę odbioru i zwrotu (system waliduje daty).
    *   Lokalizację odbioru i zwrotu.
    *   Rodzaj ubezpieczenia.
4.  **Zatwierdzenie**: Po kliknięciu "Zarezerwuj", system sprawdza dostępność wybranego auta w zadanym terminie. Jeśli auto jest dostępne, rezerwacja zostaje zapisana.

### Panel Administratora
Zalogowany administrator ma dostęp do dodatkowych funkcji (widocznych w menu lub po wejściu w odpowiednie sekcje):
*   **Zarządzanie Samochodami**: Dodawanie, edycja i usuwanie pojazdów z floty.
*   **Zarządzanie Lokalizacjami**: Definiowanie miast i punktów odbioru.
*   **Zarządzanie Rezerwacjami**: Podgląd listy wszystkich rezerwacji w systemie, możliwość ich edycji lub anulowania.

### Moje Rezerwacje (Użytkownik)
Zalogowany użytkownik w zakładce "Rezerwacje" (Index) widzi tylko i wyłącznie swoje rezerwacje. Może sprawdzić ich status oraz szczegóły.
