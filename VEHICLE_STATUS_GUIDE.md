# Guide de Suivi de l'État des Véhicules 🚗

## Vue d'ensemble

Le système de gestion de location de voitures dispose maintenant de **5 statuts** pour un suivi complet du cycle de vie des véhicules.

## Statuts Disponibles

### 1. 🟢 **Available**
- **Couleur**: Badge vert (`bg-success`)
- **Signification**: Le véhicule est disponible à la location
- **Actions possibles**: 
  - Les clients peuvent réserver ce véhicule
  - Le bouton "Book This Vehicle" est actif

### 2. 🔵 **Reserved**
- **Couleur**: Badge bleu clair (`bg-info`)
- **Signification**: Le véhicule est réservé mais pas encore récupéré
- **Actions possibles**: 
  - En attente de récupération par le client
  - Message affiché: "This vehicle is currently reserved. It will be available soon."

### 3. 🟡 **Rented**
- **Couleur**: Badge jaune (`bg-warning text-dark`)
- **Signification**: Le véhicule est en cours de location
- **Actions possibles**: 
  - Véhicule utilisé par un client
  - Message affiché: "This vehicle is currently rented out."

### 4. ⚫ **Maintenance**
- **Couleur**: Badge gris (`bg-secondary`)
- **Signification**: Le véhicule est en réparation/entretien
- **Actions possibles**: 
  - Véhicule temporairement indisponible
  - Message affiché: "This vehicle is under maintenance."

### 5. 🔴 **Retired**
- **Couleur**: Badge rouge (`bg-danger`)
- **Signification**: Le véhicule n'est plus utilisable
- **Actions possibles**: 
  - Véhicule retiré de la flotte
  - Message affiché: "This vehicle is no longer in service."

## Cycle de Vie Typique d'un Véhicule

```
Available → Reserved → Rented → Available
    ↓           ↓         ↓
Maintenance → Available
    ↓
Retired (fin de vie)
```

## Modifications Apportées

### Frontend

#### 1. `Frontend/Models/VehicleDtos.cs`
```csharp
public enum VehicleStatus
{
    Available,      // Disponible - le véhicule est disponible à la location
    Reserved,       // Réservé - le véhicule est réservé mais pas encore récupéré
    Rented,         // Loué actuellement - le véhicule est en cours de location
    Maintenance,    // En maintenance - le véhicule est en réparation/entretien
    Retired         // Hors service - le véhicule n'est plus utilisable
}
```

#### 2. `Frontend/Pages/Vehicles.razor`
- ✅ Affichage direct des statuts en anglais (`@vehicle.Status`)
- ✅ Mise à jour de `GetStatusBadgeClass()` pour inclure le statut Reserved
- ✅ Couleurs intuitives pour chaque statut

#### 3. `Frontend/Pages/VehicleDetails.razor`
- ✅ Messages spécifiques pour chaque statut en anglais
- ✅ Icônes appropriées pour chaque état
- ✅ Seuls les véhicules "Available" peuvent être réservés
- ✅ Badge de statut affiché en anglais

#### 4. `Frontend/Pages/ManageVehicles.razor`
- ✅ Support du nouveau statut Reserved dans les filtres
- ✅ Couleurs cohérentes dans la page de gestion

### Backend

#### 1. `Backend/Core/Entities/Vehicle.cs`
```csharp
public enum VehicleStatus
{
    Available,      // Disponible - le véhicule est disponible à la location
    Reserved,       // Réservé - le véhicule est réservé mais pas encore récupéré
    Rented,         // Loué actuellement - le véhicule est en cours de location
    Maintenance,    // En maintenance - le véhicule est en réparation/entretien
    Retired         // Hors service - le véhicule n'est plus utilisable
}
```

## Status Display

All status badges are displayed in **English**:
- Available
- Reserved
- Rented
- Maintenance
- Retired

## Utilisation pour les Administrateurs

### Ajouter un Nouveau Véhicule
1. Cliquez sur le bouton **"Add Vehicle"** (vert) sur la page Vehicles
2. Remplissez le formulaire
3. Par défaut, le statut sera "Available"

### Modifier le Statut d'un Véhicule
1. Accédez à la page **"Manage Vehicles"**
2. Cliquez sur **"Edit"** pour le véhicule concerné
3. Changez le statut dans la liste déroulante
4. Sauvegardez les modifications

### Filtrer par Statut
Sur la page "Manage Vehicles", vous pouvez filtrer les véhicules par statut :
- All (X vehicles)
- Available (X)
- Reserved (X)
- Rented (X)
- Maintenance (X)
- Retired (X)

## Recommandations

### Workflow de Location
1. **Available** → Client fait une réservation
2. **Reserved** → Client vient récupérer le véhicule
3. **Rented** → Location en cours
4. **Available** → Véhicule retourné et inspecté

### Maintenance Planifiée
- Passez le statut à **"Maintenance"** avant d'envoyer un véhicule en réparation
- Une fois la maintenance terminée, remettez le statut à **"Available"**

### Retrait de la Flotte
- Utilisez le statut **"Retired"** pour les véhicules qui ne sont plus en service
- Ne supprimez pas les véhicules pour conserver l'historique

## Tests Recommandés

1. ✅ Vérifier que seuls les véhicules "Available" peuvent être réservés
2. ✅ Vérifier que les couleurs des badges correspondent aux statuts
3. ✅ Vérifier que les messages d'alerte sont appropriés pour chaque statut
4. ✅ Tester le filtrage par statut dans la page de gestion
5. ✅ Vérifier la cohérence entre Frontend et Backend
6. ✅ Vérifier que tous les statuts sont affichés en anglais

## Support

Pour toute question sur les statuts des véhicules, consultez ce guide ou contactez l'administrateur système.

---
**Dernière mise à jour**: Aujourd'hui
**Version**: 2.1 avec statuts en anglais
