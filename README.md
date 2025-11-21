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

---

### 📬 API-Schnittstellen
Eine Anforderung für das Projekt ist es mindestens eine öffentlich verfügbare API zu verwenden. Diese Anforderung nutzen wir, um die Forschungsdokumentation der Wissenschaftler mit Umweltdaten aus den Everglades zu versorgen. Wasserbezogene Daten werden per HTTPS vom **USGS** (United States Geological Survery) und Luftdaten von **NOAA** (National Oceanic and Atmospheric Administration).

- NOAA (Lufttemperatur, Luftfeuchtigkeit)
- USGS (Salzgehalt, Wassertemperatur)

Da die Station, von der die Daten bezogen wird, leider nicht über einen funktionsfähigen pH-Wert Sensor verfügt, wird dieser Wert gemockt indem aus einem für diese Region typischen pH-Wert Bereich (7.2 - 8.4 in 0.1 Schritten) ein zufälliger Wert generiert und den Daten hinzugefügt wird.

#### Endpoints:
```
https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00095&period=PT2H
```

#### Parameter:
| Parameter              | Wert             | Beschreibung                                |
| :--------------------- | :----------------| ------------------------------------------: |
| Format                 |json              |    Ausgabe Format                           |
| Sites                  |251457080395802   |    ID der Messstation                       |
| parameterCd            |00010,00095       |    Wassertemperatur, Salzgehalt (Salinität) |
| period                 |PT2H              |    Daten der letzten beiden Stunden         |

```
https://api.weather.gov/points/25.25255556,-80.6662611
```
| Parameter              | Wert             | Beschreibung                                |
| :--------------------- | :----------------| ------------------------------------------: |
| lat                    |25.25255556       |    Breitengrad                              |
| lon                    |-80.6662611       |    Längengrad                               |


#### Docs:
- USGS: https://waterservices.usgs.gov/docs/instantaneous-values/instantaneous-values-details/
- NOAA: https://www.weather.gov/documentation/api/point

Die Messstation befindet sich in einer Übergangszone zwischen Süß- und Salzwasser, wo Krokodile und Alligatoren auf natürliche weise miteinander leben können.

### 💻 Installation

In Bearbeitung

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

---


### 📬 API Interfaces

One requirement for the project is to use at least one publicly available API. We use this requirement to provide the scientists' research documentation with environmental data from the Everglades. Water-related data is obtained via HTTPS from the **USGS** (United States Geological Survey), and air data from **NOAA** (National Oceanic and Atmospheric Administration).

- NOAA (air temperature, humidity)
- USGS (salinity, water temperature)

Since the station from which the data is obtained unfortunately does not have a functioning pH sensor, this value is mocked by generating a random value from a pH range (7.2 - 8.4 in 0.1 increments) typical for this region and adding it to the data.

#### Endpoints:
```
https://waterservices.usgs.gov/nwis/iv/?format=json&sites=251457080395802&parameterCd=00010,00095&period=PT2H
```

#### Parameter:
| Parameter              | Value            | Description                                 |
| :--------------------- | :----------------| ------------------------------------------: |
| Format                 |json              |    result format                            |
| Sites                  |251457080395802   |    station ID                               |
| parameterCd            |00010,00095       |    watertemperature, salinity               |
| period                 |PT2H              |    data from the last two hours             |

```
https://api.weather.gov/points/25.25255556,-80.6662611
```
| Parameter              | Value            | Description                                 |
| :--------------------- | :----------------| ------------------------------------------: |
| lat                    |25.25255556       |    latitude                                 |
| lon                    |-80.6662611       |    longitude                                |


#### Docs:
- USGS: https://waterservices.usgs.gov/docs/instantaneous-values/instantaneous-values-details/
- NOAA: https://www.weather.gov/documentation/api/point

### 💻 Installation

In progress