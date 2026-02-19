# CrocoFeeding Manager 🐊

[Deutsch](#deutsch) | [English](#english)
## Deutsch

### 📘 Projektbeschreibung  
Dieses Projekt entsteht im Rahmen einer Schulaufgabe. Ziel ist es, eine vollständige Softwareentwicklung von der Planung über die Umsetzung bis hin zum Testen und Dokumentieren durchzuführen.

---

### 🌍 Szenario  
In den Everglades, Florida, USA, gibt es ein Naturschutzreservat mit einer Wildtierstation für Krokodile und Alligatoren. Ranger versorgen Tiere in Gebieten mit wenig natürlicher Nahrung, während Wissenschaftler aus umliegenden Universitäten das Fressverhalten der Tiere überwachen und protokollieren.  
Bisher wurde diese Arbeit auf Papier durchgeführt.

---

### 🎯 Zielsetzung  
Mit dem **CrocoFeeding Manager** soll die Fütterungsplanung digitalisiert und die Arbeit der Wissenschaftler effizient unterstützt werden. Die Anwendung soll plattformübergreifend (Windows, Android) nutzbar sein.

---

### ⚙️ Funktionen / Features  
- Verwaltung von Tieren (Art, Alter, Gewicht, Gesundheitszustand)  
- Planung von Fütterungen inkl. Zeitplan und Mengenberechnung  
- Dokumentation wissenschaftlicher Beobachtungen  
- Exportfunktionen für Forschungsberichte  
- Benutzerfreundliche Oberfläche für Ranger und Wissenschaftler  
- Unterstützung mehrerer Plattformen

---

### 🧰 Technologien  
- **Programmiersprache:** C#
- **GUI Framework:** .NET MAUI  
- **Datenbank:** PostgreSQL via Supabase
- **Unit Tests** XUnit

---

### 📬 API-Schnittstellen
Eine Anforderung für das Projekt ist es mindestens eine öffentlich verfügbare API zu verwenden. Diese Anforderung nutzen wir, um die Forschungsdokumentation der Wissenschaftler mit Umweltdaten aus den Everglades zu versorgen. Wasserbezogene Daten werden per HTTPS vom **USGS** (United States Geological Survey) und Luftdaten von **NOAA** (National Oceanic and Atmospheric Administration).

- NOAA (Lufttemperatur, Luftfeuchtigkeit)
- USGS (Salzgehalt, Wassertemperatur)

Da die Station, von der die Daten bezogen wird, leider nicht über einen funktionsfähigen pH-Wert Sensor verfügt, wird dieser Wert gemockt indem aus einem für diese Region typischen pH-Wert Bereich (7.2 - 8.4 in 0.1 Schritten) ein zufälliger Wert generiert und den Daten hinzugefügt wird.

#### Endpoints:
```
https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00480&period=PT2H
```

#### Parameter:
| Parameter              | Wert             | Beschreibung                                |
| :--------------------- | :----------------| :------------------------------------------ |
| Format                 |json              |    Ausgabe Format                           |
| Sites                  |251457080395802   |    ID der Messstation                       |
| parameterCd            |00010,00480       |    Wassertemperatur, Salzgehalt (Salinität) |
| period                 |PT2H              |    Daten der letzten beiden Stunden         |

```
https://api.weather.gov/stations/KHST/observations/latest
```
| Parameter              | Wert             | Beschreibung                                |
| :--------------------- | :----------------| :------------------------------------------ |
| stations                    |KHST       |    Homestead Air Reserve Base                              |


#### Docs:
- USGS: https://waterservices.usgs.gov/docs/instantaneous-values/instantaneous-values-details/
- NOAA: https://www.weather.gov/documentation/services-web-api#/default/station_observation_list

Die Messstation befindet sich in einer Übergangszone zwischen Süß- und Salzwasser, wo Krokodile und Alligatoren auf natürliche weise miteinander leben können. Wobei die NOAA Wetter Station ca. 39km entfernt von der Wassermesstation ist, da NOAA seit vergangenen Hurrikans und Budget Kürzungen keine nähere Station mehr unterhält.

### 🏗️ Architektur & Design Patterns

Die Anwendung folgt dem **MVVM-Entwurfsmuster (Model-View-ViewModel)**. Zur Reduzierung von Boilerplate-Code (wie `INotifyPropertyChanged`) wird das **CommunityToolkit.Mvvm** eingesetzt, welches durch Source Generator Attribute wie `[ObservableProperty]` und `[RelayCommand]` die Entwicklung beschleunigt.

#### Schichtentrennung (Core & UI)
Um die Testbarkeit zu gewährleisten, wurde eine strikte Trennung zwischen UI-Logik und Geschäftslogik vorgenommen:
- **CrocoManager.Core (.NET Library):** Enthält plattformunabhängigen Code, Interfaces, Modelle und Services. Da Unit-Tests für MAUI-spezifischen Code (UI-Komponenten, Plattform-APIs) äußerst komplex und oft unmöglich sind, wurde die gesamte Geschäftslogik hierhin extrahiert.
- **CrocoManager (MAUI Project):** Enthält die Views (XAML) und ViewModels, die auf die Core-Bibliothek zugreifen.

---

### 📊 Visualisierung & Dokumentation

In diesem Projekt wurde viel Wert auf eine saubere Planung gelegt. Die folgenden Diagramme visualisieren die Struktur:

| Typ | Diagramm |
| :--- | :--- |
| **Klassendiagramm** | ![Klassendiagramm](./Diagrams/Klassendiagramm%20CrocoManager.webp) |
| **Datenbank (ERD)** | [Entity Relationship Diagram](./Diagrams/erd.md) |
| **Use Case** | [Use Case Diagram](./Diagrams/UseCase-Diagramm-CrocoManager.drawio.webp) |
| **Abläufe** | [Beobachtung dokumentieren](./Diagrams/Aktivitätsdiagramm%20Beobachtung%20dokumentieren.webp) \| [Whitelist Prozess](./Diagrams/Aktivitätsdiagramm%20Whitelist%20Eintrag.webp) |

---

### 🧪 Qualitätssicherung (Testing)

Die Geschäftslogik wird durch automatisierte Unit-Tests in einem separaten Test-Projekt (`CrocoManager.Core.Tests`) abgesichert. Dies stellt sicher, dass Kernfunktionen wie Mappings und Berechnungen auch bei Änderungen stabil bleiben.

**Tests ausführen:**
```bash
dotnet test CrocoManager.Core.Tests/CrocoManager.Core.Tests.csproj
```

---

### 💻 Installation & Build

#### Voraussetzungen
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [.NET MAUI Workload](https://learn.microsoft.com/de-de/dotnet/maui/get-started/installation)

#### Vorbereitung
1. Repository klonen:
   ```bash
   git clone git@github.com:tobias-hstbrg/CrocoManager.git
   cd CrocoManager
   ```
2. Konfiguration erstellen:
  `CrocoManager/appsettings.example.json` umbenennen zu `CrocoManager/appsettings.json` und dann deine Supabase-Zugangsdaten eingeben.

#### Build & Run
- **Windows:**
  ```bash
  dotnet build CrocoManager/CrocoManager.csproj -f net9.0-windows10.0.19041.0
  ```
- **Android:**
  ```bash
  dotnet build CrocoManager/CrocoManager.csproj -f net9.0-android
  ```

#### GitHub Actions Artefakte
Die aktuellsten Build-Artefakte (APK für Android, ZIP für Windows) können direkt aus den GitHub Actions bezogen werden:
1. Gehe zum Tab [Actions](https://github.com/tobias-hstbrg/CrocoManager/actions).
2. Wähle den aktuellsten erfolgreichen Workflow-Lauf aus.
3. Scrolle nach unten zum Bereich **Artifacts** und lade das gewünschte Paket herunter.

## English

### 📘 Project description
This project is part of a school assignment. The goal is to carry out the entire software development process, from planning and implementation to testing and documentation.

---

### 🌍 Scenario
In the Everglades, Florida, USA, there is a nature reserve with a wildlife station for crocodiles and alligators. Rangers care for animals in areas with little natural food, while scientists from surrounding universities monitor and record the animals' feeding behavior.
Until now, this work has been done on paper.

---

### 🎯 Objective
The **CrocoFeeding Manager** is designed to digitize feeding planning and efficiently support the work of scientists. The application should be usable across platforms (Windows, Android).

---

### ⚙️ Functions / Features
- Management of animals (species, age, weight, health status)
- Feeding planning, including schedule and quantity calculation
- Documentation of scientific observations
- Export functions for research reports
- User-friendly interface for rangers and scientists
- Support for multiple platforms

---

### 🧰 Technologies
- **Programming language:** C#
- **GUI framework:** .NET MAUI
- **Database:** PostgreSQL via Supabase
- **Unit Tests:** XUnit

---

### 📬 API Interfaces

One requirement for the project is to use at least one publicly available API. We use this requirement to provide the scientists' research documentation with environmental data from the Everglades. Water-related data is obtained via HTTPS from the **USGS** (United States Geological Survey), and air data from **NOAA** (National Oceanic and Atmospheric Administration).

- NOAA (air temperature, humidity)
- USGS (salinity, water temperature)

Since the station from which the data is obtained does not have a functioning pH sensor, this value is mocked by generating a random value from a pH range typical for this region (7.2–8.4 in 0.1 increments) and adding it to the data.

#### Endpoints:
```
https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00480&period=PT2H
```

| Parameter   | Value          | Description                          |
| :---------- | :------------- | :----------------------------------- |
| format      | json           | Result format                        |
| sites       | 251457080395802 | Station ID                          |
| parameterCd | 00010,00480    | Water temperature, salinity          |
| period      | PT2H           | Data from the last two hours         |

```
https://api.weather.gov/stations/KHST/observations/latest
```

| Parameter | Value | Description              |
| :-------- | :---- | :----------------------- |
| stations  | KHST  | Homestead Air Reserve Base |

#### Docs:
- USGS: https://waterservices.usgs.gov/docs/instantaneous-values/instantaneous-values-details/
- NOAA: https://www.weather.gov/documentation/services-web-api#/default/station_observation_list

The measurement station is located in a transition zone between fresh and saltwater, where crocodiles and alligators can naturally coexist. The NOAA weather station is approximately 39 km away from the water measurement station, as NOAA no longer maintains a closer station due to past hurricane damage and budget cuts.

### 🏗️ Architecture & Design Patterns

The application follows the **MVVM (Model-View-ViewModel)** design pattern. To minimize boilerplate code (such as `INotifyPropertyChanged`), the **CommunityToolkit.Mvvm** is utilized. Its source generator attributes like `[ObservableProperty]` and `[RelayCommand]` significantly accelerate development.

#### Layer Separation (Core & UI)
To ensure testability, a strict separation between UI logic and business logic was implemented:
- **CrocoManager.Core (.NET Library):** Contains platform-agnostic code, interfaces, models, and services. Since unit tests for MAUI-specific code (UI components, platform APIs) are extremely complex and often impossible, all business logic has been extracted into this library.
- **CrocoManager (MAUI Project):** Contains the views (XAML) and ViewModels, which interact with the Core library.

---

### 📊 Visualization & Documentation

Great care was taken in the planning phase of this project. The following diagrams visualize the structure:

| Type | Diagram |
| :--- | :--- |
| **Class Diagram** | ![Class Diagram](./Diagrams/Klassendiagramm%20CrocoManager.webp) |
| **Database (ERD)** | [Entity Relationship Diagram](./Diagrams/erd.md) |
| **Use Case** | [Use Case Diagram](./Diagrams/UseCase-Diagramm-CrocoManager.drawio.webp) |
| **Workflows** | [Document Observation](./Diagrams/Aktivitätsdiagramm%20Beobachtung%20dokumentieren.png) \| [Whitelist Process](./Diagrams/Aktivitätsdiagramm%20Whitelist%20Eintrag.png) |

---

### 🧪 Quality Assurance (Testing)

The business logic is protected by automated unit tests in a separate test project (`CrocoManager.Core.Tests`). This ensures that core functionalities like mappings and calculations remain stable through changes.

### 💻 Installation & Build

#### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [.NET MAUI Workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)

#### Setup
1. Clone the repository:
   ```bash
   git clone git@github.com:tobias-hstbrg/CrocoManager.git
   cd CrocoManager
   ```
2. Create Configuration:
   Rename `CrocoManager/appsettings.example.json` to `CrocoManager/appsettings.json` and fill in your Supabase credentials.

#### Build & Run
- **Windows:**
  ```bash
  dotnet build CrocoManager/CrocoManager.csproj -f net9.0-windows10.0.19041.0
  ```
- **Android:**
  ```bash
  dotnet build CrocoManager/CrocoManager.csproj -f net9.0-android
  ```

#### GitHub Actions Artifacts
The latest build artifacts (APK for Android, ZIP for Windows) can be obtained directly from GitHub Actions:
1. Go to the [Actions](https://github.com/tobias-hstbrg/CrocoManager/actions) tab.
2. Select the latest successful workflow run.
3. Scroll down to the **Artifacts** section and download the desired package.