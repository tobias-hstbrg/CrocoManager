# ERD

Um mit der Applikation sinnvoll arbeiten zu können, wird auf eine PostgreSQL-Datenbank gesetzt, die von unserem Backend-as-a-Service Supabase bereitgestellt wird. Auf dieser läuft das unten ausgearbeitete Datenbankschema. 

Als Primary Keys werden UUIDs verwendet, was dem PostgreSQL-Standard entspricht. C# bietet mit dem Datentyp `Guid` eine native Unterstützung für UUIDs, sodass die Arbeit mit diesen Werten problemlos möglich ist. Ein weiterer Vorteil von UUIDs ist ihre Eignung für verteilte Systeme, da sie global eindeutig sind und keine Kollisionen verursachen. Dies macht die Wahl dieses Datentyps besonders zukunftssicher, falls das System später skaliert oder erweitert werden soll.

```mermaid
 ---
title: CrocoManager
---
erDiagram
	direction TB
	ANIMALS {
		uuid id PK ""  
		string name  ""  
		string species  ""  
		string gender  ""  
		int age_years  ""  
		string enclosure  ""  
		text description  ""  
		timestamp created_at  ""  
		timestamp updated_at  ""  
	}

	FEEDING_ANIMALS {
		uuid id PK ""  
		uuid feeding_id FK ""  
		uuid animal_id FK ""  
		boolean was_fed  ""  
		timestamp created_at  ""  
	}

	FEEDING_PLAN {
		uuid id PK ""  
		string name  ""  
		string food_type  ""  
		decimal amount_kg  ""  
		int frequency_per_week  ""  
		string weekdays  ""  
		text description  ""  
		boolean is_active  ""  
		timestamp created_at  ""  
		timestamp updated_at  ""  
	}

	FEEDINGS {
		uuid id PK ""  
		uuid feeding_plan_id FK ""  
		date feeding_date  ""  
		string performed_by_email  ""  
		timestamp created_at  ""
	}

	ENVIRONMENTAL_DATA {
		uuid id PK ""  
		date measurement_date  ""  
		time measurement_time  ""  
		decimal air_temperature_celsius  ""  
		decimal humidity_percent  ""  
		decimal water_temperature_celsius  ""  
		decimal ph_value  ""  
		decimal salinity_ppt  ""  
		timestamp created_at  ""
	}

	OBSERVATIONS {
		uuid id PK ""  
		uuid animal_id FK ""  
		uuid feeding_id FK ""  
		uuid environmental_data_id FK ""  
		string feeding_behavior  ""  
		text notes  ""  
		string researcher_email  ""  
		timestamp created_at  "" 
	}

	ANIMALS||--o{FEEDING_ANIMALS:"wird gefüttert in"
	ANIMALS||--o{OBSERVATIONS:"hat"
	FEEDING_PLAN||--o{FEEDINGS:"wird verwendet in"
	FEEDINGS||--o{FEEDING_ANIMALS:"enthält"
	FEEDINGS||--o{OBSERVATIONS:"wird dokumentiert in"
	ENVIRONMENTAL_DATA||--o{OBSERVATIONS:"gemessen bei"

```

| Tabelle            | Beschreibung                                                                       |
|:-------------------| :----------------------------------------------------------------------------------|
| ANIMALS            | Verwaltung aller Tiere im Reservat mit Stammdaten wie Name, Art, Alter und Gehege  |
| FEEDING_PLANS      | Definition von Fütterungsplänen mit Futterart, Menge und Frequenz                  |
| FEEDINGS           | Dokumentation durchgeführter Fütterungen mit Datum und verwendetem Plan            |
| FEEDING_ANIMALS    | Zuordnungstabelle, welche Tiere bei welcher Fütterung gefüttert wurden             |
| OBSERVATIONS       | Wissenschaftliche Beobachtungen zum Fressverhalten der Tiere                       |
| ENVIRONMENTAL_DATA | Umweltdaten (Wetter, Wasserqualität) zum Zeitpunkt der Beobachtungen               |
