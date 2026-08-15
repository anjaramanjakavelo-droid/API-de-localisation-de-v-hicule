# Vehicle Tracking API

API REST développée avec ASP.NET Core permettant de simuler le suivi GPS de véhicules.

L'API reçoit des positions GPS, les enregistre dans une base de données MySQL et permet de consulter l'historique des déplacements, de calculer les distances parcourues et d'identifier les zones géographiques traversées.

## Technologies utilisées

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- MySQL
- Pomelo.EntityFrameworkCore.MySql
- REST API
- JSON
- Postman
- Blazor pour l'interface web

---

## Architecture

Le projet suit une architecture simple basée sur la séparation des responsabilités :

```text
Client HTTP / Blazor / Postman
              |
              | HTTP / JSON
              v
      ASP.NET Core Web API
              |
       +------+------+
       |             |
 Controllers      Services
       |             |
       +------+------+
              |
       Entity Framework Core
              |
              v
            MySQL

Rôle des composants
Controllers : reçoivent les requêtes HTTP et retournent les réponses JSON.
Services : contiennent la logique métier, notamment le calcul des distances et la détermination des zones.
Entity Framework Core : assure la communication entre l'API et MySQL.
MySQL : stocke les véhicules et leurs positions GPS.
Fonctionnement

Dans un système réel, un boîtier GPS installé dans un véhicule récupérerait régulièrement sa position et enverrait une requête HTTP à l'API.

Dans ce projet, l'envoi GPS est simulé avec Postman.

Le fonctionnement est donc :
Boîtier GPS simulé avec Postman
              |
              | POST /api/positions
              v
        ASP.NET Core API
              |
              v
       Entity Framework Core
              |
              v
            MySQL

L'interface Blazor permet ensuite de consulter les données enregistrées via l'API REST.

Base de données

La base de données utilisée est :
vehicle_tracking
Elle contient deux tables principales :
vehicle_tracking
│
├── vehicule
│   ├── id
│   └── immatriculation
│
└── position_gps
    ├── id
    ├── vehicule_id
    ├── latitude
    ├── longitude
    └── date_position

Table vehicule

Cette table contient les véhicules suivis.

Colonne	Type	Description
id	INT	Identifiant unique du véhicule
immatriculation	VARCHAR(20)	Immatriculation du véhicule

L'immatriculation est unique.

Table position_gps

Cette table contient les positions GPS reçues.

Colonne	Type	Description
id	INT	Identifiant unique de la position
vehicule_id	INT	Identifiant du véhicule
latitude	DECIMAL	Latitude GPS
longitude	DECIMAL	Longitude GPS
date_position	DATETIME	Date et heure de la position

Relation
vehicule (1) ──────────── (N) position_gps

Un véhicule peut avoir plusieurs positions GPS.

Une position GPS appartient à un seul véhicule.

La suppression d'un véhicule entraîne également la suppression de ses positions GPS associées.

API REST

L'API est accessible localement à :

http://localhost:5041
Véhicules
Récupérer tous les véhicules
GET /api/vehicules
Récupérer un véhicule
GET /api/vehicules/{id}
Ajouter un véhicule
POST /api/vehicules

Exemple :

{
  "immatriculation": "1234TAA"
}
Modifier un véhicule
PUT /api/vehicules/{id}
Supprimer un véhicule
DELETE /api/vehicules/{id}
Positions GPS
Ajouter une position GPS
POST /api/positions

Exemple :

{
  "vehiculeId": 2,
  "latitude": -18.8792,
  "longitude": 47.5079,
  "datePosition": "2026-07-09T10:30:00"
}
Récupérer toutes les positions
GET /api/positions
Historique des déplacements
Consulter l'historique d'un véhicule
GET /api/vehicules/{id}/historique
Consulter l'historique sur une période
GET /api/vehicules/{id}/historique?dateDebut=...&dateFin=...

Exemple :

GET /api/vehicules/2/historique?dateDebut=2026-07-09T09:00:00&dateFin=2026-07-09T18:00:00

L'API retourne uniquement les positions GPS correspondant au véhicule et à la période demandée.

Statistiques kilométriques
Calculer la distance parcourue
GET /api/vehicules/{id}/distance

La distance est calculée à partir des positions GPS successives du véhicule.

La formule de Haversine est utilisée pour calculer la distance entre deux coordonnées GPS.

Position 1 → Position 2 → Position 3 → Position 4
       ↓           ↓           ↓
    Distance    Distance    Distance


Distance totale = somme des distances successives
Zones de passage
Consulter les zones traversées
GET /api/vehicules/{id}/zones

Les coordonnées GPS sont comparées à des zones géographiques prédéfinies.

Les zones correspondent à différentes villes ou régions de Madagascar.

Si une position se trouve dans une zone définie, le nom de cette zone est associé au véhicule.

Exemple :

Antananarivo
Toamasina
Mahajanga
Fianarantsoa
Antsiranana
Toliara

Si aucune zone ne correspond :

Hors zone définie
Interface Blazor

Une application Blazor consomme l'API REST.

Blazor
   |
   | HTTP / JSON
   v
ASP.NET Core API
   |
   v
MySQL

L'interface permet notamment de :

consulter les véhicules ;
ajouter, modifier et supprimer des véhicules ;
consulter l'historique GPS ;
filtrer l'historique par véhicule et période ;
consulter les distances parcourues ;
consulter les zones traversées ;
visualiser les positions GPS sur une carte.

Blazor n'accède jamais directement à MySQL.

Toutes les communications passent par l'API REST.

Tests

Les requêtes HTTP peuvent être testées avec Postman.

Postman est principalement utilisé dans le projet pour simuler l'envoi des positions GPS par un boîtier GPS.

Exemple :

Postman
   |
   | POST /api/positions
   v
API REST
   |
   v
MySQL
   |
   v
Blazor
Objectif du projet

Ce projet permet de mettre en pratique :

la conception d'une API REST ;
le développement avec ASP.NET Core ;
l'utilisation d'Entity Framework Core ;
la communication avec une base MySQL ;
la réception de données GPS ;
la consultation d'historiques ;
le calcul de distances ;
la détermination de zones de passage ;
la communication entre un backend REST et un frontend Blazor.

Le projet simule ainsi une solution de suivi de véhicules dans laquelle les positions GPS sont transmises au serveur par des requêtes HTTP.