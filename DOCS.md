# Projektdokumentation: CrocoFeeding Manager 🐊

## 1. Projektbeschreibung
Dieses Projekt wurde als Schulaufgabe entwickelt. Ziel ist es, die Verwaltung und Fütterung von Krokodilen in einem Naturschutzreservat (Everglades, Florida) digital abzubilden. Die App hilft Rangern bei der täglichen Fütterung und Wissenschaftlern beim Protokollieren von Forschungsdaten unter Einbeziehung von echten Wetter- und Wasserdaten.

---

## 2. Architektur & Designentscheidungen

### 2.1 Projektstruktur
Um die Software einfach testen zu können, wurde das Projekt in drei Teile getrennt:

1.  **CrocoManager.Core:** Hier liegt die gesamte "Intelligenz" der App (Logik, Datenmodelle, Berechnungen). Da dieser Teil keine grafische Oberfläche hat, können wir ihn blitzschnell automatisch testen. Sämtliche ViewModels wurden hierhin verschoben.
2.  **CrocoManager (MAUI App):** Dieser Teil ist nur für das Aussehen (UI) zuständig. Er greift auf die Logik im Core-Teil zu.
3.  **CrocoManager.Core.Tests:** Ein eigenständiges Projekt, das prüft, ob die Logik im Core-Teil fehlerfrei funktioniert.

### 2.2 Weniger Code durch Vererbung
Um Zeit zu sparen und Fehler durch doppelte Programmierung zu vermeiden, nutzt das Projekt Basisklassen:
*   **BaseService:** Erledigt die Standard-Aufgaben mit der Datenbank (Speichern, Löschen, Laden). Die speziellen Services für Tiere oder Futterpläne müssen diese Dinge nicht neu erfinden.
*   **BaseViewModel:** Enthält Funktionen, die auf fast jeder Seite gebraucht werden, wie zum Beispiel den Logout-Button oder die Prüfung, ob ein Nutzer überhaupt die Berechtigung für eine Aktion hat.

### 2.3 Warum so viele Interfaces?
Interfaces wirken wie ein Vertrag. Ein ViewModel weiß zum Beispiel nur, *dass* es Daten speichern kann, aber nicht *wie* die Datenbank dahinter genau funktioniert.
*   **Vorteil beim Testen:** Wir können für die Tests so genannte "Mocks" (Platzhalter) einsetzen. So können wir den Login testen, ohne dass wir tatsächlich eine Internetverbindung zum Datenbank-Server benötigen.

---

## 3. Technologien & Schnittstellen

### 3.1 Technologie-Stack
*   **Sprache/Framework:** C# mit .NET MAUI.
*   **Datenbank:** PostgreSQL via Supabase.
*   **Testing:** xUnit (Test-Framework) und Moq (für die Platzhalter-Services).

### 3.2 Schnittstellen (APIs)
Die App bezieht Live-Daten aus den Everglades über zwei öffentliche Schnittstellen:
*   **NOAA:** Lufttemperatur und Feuchtigkeit.
*   **USGS:** Wassertemperatur und Salzgehalt.
*   **Hinweis:** pH-Werte werden mangels Sensoren vor Ort durch einen Zufallsgenerator innerhalb realistischer Grenzen simuliert.

---

## 4. Qualitätssicherung (Tests)

Die Tests basieren direkt auf den Anforderungen, die nach dem Rupp-Schema definiert wurden. Jeder wichtige Prozess in der App wird durch einen automatisierten Test abgesichert.

### 4.1 Zuordnung: Welche Anforderung wurde wie getestet?

| ID | Anforderungs-Name | Testmethode / Testklasse |
| :--- | :--- | :--- |
| **FA-01** | E-Mail Whitelist verwalten | Manuell (Admin-Oberfläche) |
| **FA-02** | Registrierung | Manuell (Bestätigungs-Mail Flow) |
| **FA-03** | Whitelist-Validierung | Manuell (Prüfung ungültiger Mails) |
| **FA-04** | Anmeldung | Automatisiert (`LoginViewModelTests`) |
| **FA-05** | Passwort zurücksetzen | Manuell (E-Mail Versand) |
| **FA-06** | Tiere anzeigen | Manuell (Listen-Darstellung) |
| **FA-07** | Tiere anlegen | Automatisiert (`AnimalViewModelTests`) |
| **FA-08** | Tiere bearbeiten | Manuell (Eingabemaske) |
| **FA-09** | Tier löschen | Manuell (Datenbank-Sync) |
| **FA-10** | Futterpläne anzeigen | Manuell (Listen-Darstellung) |
| **FA-11** | Futterplan erstellen | Manuell (Eingabemaske) |
| **FA-12** | Futterplan bearbeiten | Manuell (Eingabemaske) |
| **FA-13** | Plan löschen | Automatisiert (`FeedingPlanViewModelTests`) |
| **FA-14** | Plan aktivieren | Automatisiert (`FeedingPlanViewModelTests`) |
| **FA-15** | Fütterung vorbereiten | Manuell (Checkbox-Liste) |
| **FA-16** | Tiere markieren | Manuell (UI-Interaktion) |
| **FA-17** | Fütterung beenden | Automatisiert (`FeedingViewModelTests`) |
| **FA-18** | Fütterungshistorie anzeigen | Manuell (Chronologische Liste) |
| **FA-19** | Umweltdaten abrufen | Automatisiert (`ObservationServiceTests`) |
| **FA-20** | Umweltdaten anzeigen | Manuell (UI-Dashboard) |
| **FA-21** | Beobachtung speichern | Automatisiert (`ObservationViewModelTests`) |
| **FA-22** | Beobachtungen anzeigen | Manuell (Chronologische Liste) |
| **FA-23** | Dashboard anzeigen | Manuell (Statistik-Abgleich) |
| **NFA-01** | Ladezeit Hauptansichten | Manuell (Messung < 5 Sek.) |
| **NFA-02** | API-Antwortzeit | Automatisiert (`ObservationServiceTests`) |
| **NFA-03** | Fehlerbehandlung | Automatisiert (`AnimalViewModelTests`) |
| **NFA-04** | MVVM-Struktur | Architektur-Review (✅ erfüllt) |
| **NFA-05** | Service Pattern | Architektur-Review (✅ erfüllt) |
| **NFA-06** | Windows-Kompatibilität | Manuell (Lauffähigkeit Win 11) |
| **NFA-07** | Android-Kompatibilität | Manuell (Lauffähigkeit Android 8+) |

### 4.2 Code-Abdeckung (Coverage)

Die folgende Tabelle zeigt, wie viel Prozent der Logik in den jeweiligen Bereichen durch automatisierte Tests abgedeckt sind:

| Bereich | Abdeckung (ca. %) | Bemerkung |
| :--- | :---: | :--- |
| **Authentifizierung** | 100 % | Alle Login-Szenarien und Rollen-Mappings sind abgedeckt. |
| **Tierverwaltung** | 100 % | Anlegen, Löschen und Validierung sind vollautomatisch geprüft. |
| **Futterplan-Verwaltung** | 100 % | Logik für Aktivierung und Löschschutz ist abgesichert. |
| **Fütterungsdurchführung** | 100 % | Auswahlprüfung und Speichervorgang sind getestet. |
| **Forschungsdokumentation** | 85 % | Validierung der Eingaben ist getestet; API-Anbindung ist im Core getestet. |
| **Daten-Mapping** | 100 % | 6 Mapper-Tests stellen sicher, dass Daten korrekt umgewandelt werden. |
| **Gesamtprojekt** | **~94 %** | Die gesamte kritische Geschäftslogik ist automatisiert abgesichert. |

---

## 5. Deployment
Die App kann auf **Windows 11** und **Android (ab Version 8.0)** genutzt werden. Voraussetzung für die Ausführung ist eine gültige Konfiguration in der `appsettings.json`, um eine Verbindung zur Datenbank herstellen zu können.
