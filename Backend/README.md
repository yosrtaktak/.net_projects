# Car Rental Management System 🚗

Un système complet de gestion de location de voitures développé avec ASP.NET Core 9, Entity Framework Core, et implémentant plusieurs Design Patterns.

## 📋 Table des Matières

- [Caractéristiques](#caractéristiques)
- [Design Patterns Implémentés](#design-patterns-implémentés)
- [Architecture](#architecture)
- [Technologies Utilisées](#technologies-utilisées)
- [Installation](#installation)
- [Configuration](#configuration)
- [Utilisation de l'API](#utilisation-de-lapi)
- [Endpoints](#endpoints)

## ✨ Caractéristiques

- **Authentification JWT** : Système de connexion sécurisé avec tokens JWT
- **Gestion des Véhicules** : CRUD complet pour les véhicules
- **Gestion des Clients** : Système de gestion des clients avec niveaux de fidélité
- **Gestion des Locations** : Réservation, activation, et clôture des locations
- **Calcul de Prix Dynamique** : Plusieurs stratégies de tarification
- **Swagger UI** : Documentation interactive de l'API
- **Base de données SQL Server** : Avec Entity Framework Core (Code First)

## 🎨 Design Patterns Implémentés

### 1. Repository Pattern
Abstraction de la couche d'accès aux données pour faciliter les tests et la maintenance.

**Fichiers:** 
- `Core/Interfaces/IRepository.cs`
- `Infrastructure/Repositories/Repository.cs`
- `Infrastructure/Repositories/VehicleRepository.cs`
- `Infrastructure/Repositories/RentalRepository.cs`

### 2. Unit of Work Pattern
Coordonne les transactions entre plusieurs repositories.

**Fichiers:**
- `Core/Interfaces/IUnitOfWork.cs`
- `Infrastructure/UnitOfWork/UnitOfWork.cs`

### 3. Strategy Pattern
Permet de changer dynamiquement l'algorithme de calcul de prix.

**Stratégies disponibles:**
- **Standard Pricing**: Prix de base par jour
- **Loyalty Pricing**: Réductions basées sur le niveau du client (Silver: 5%, Gold: 10%, Platinum: 15%)
- **Seasonal Pricing**: Augmentation de 25% pendant la haute saison (Juin-Août, Décembre)
- **Weekend Pricing**: Surcharge de 15% pour les weekends

**Fichiers:**
- `Core/Interfaces/IPricingStrategy.cs`
- `Application/Services/PricingStrategies/`

### 4. Factory Pattern
Création d'objets de stratégie de tarification.

**Fichiers:**
- `Application/Factories/IPricingStrategyFactory.cs`
- `Application/Factories/PricingStrategyFactory.cs`

### 5. Singleton Pattern
Service JWT partagé dans toute l'application.

**Fichiers:**
- `Application/Services/JwtService.cs`

### 6. Dependency Injection Pattern
Intégré via le conteneur IoC d'ASP.NET Core dans `Program.cs`

## 🏗️ Architecture

```
Backend/
├── Core/                           # Domain Layer
│   ├── Entities/                   # Entités du domaine
│   │   ├── Vehicle.cs
│   │   ├── Customer.cs
│   │   ├── Rental.cs
│   │   ├── Payment.cs
│   │   ├── Maintenance.cs
│   │   └── User.cs
│   └── Interfaces/                 # Interfaces
│       ├── IRepository.cs
│       ├── IUnitOfWork.cs
│       ├── IVehicleRepository.cs
│       ├── IRentalRepository.cs
│       └── IPricingStrategy.cs
│
├── Infrastructure/                 # Data Access Layer
│   ├── Data/
│   │   └── CarRentalDbContext.cs
│   ├── Repositories/
│   │   ├── Repository.cs
│   │   ├── VehicleRepository.cs
│   │   └── RentalRepository.cs
│   └── UnitOfWork/
│       └── UnitOfWork.cs
│
├── Application/                    # Business Logic Layer
│   ├── Services/
│   │   ├── JwtService.cs
│   │   ├── RentalService.cs
│   │   └── PricingStrategies/
│   │       ├── StandardPricingStrategy.cs
│   │       ├── LoyaltyPricingStrategy.cs
│   │       ├── SeasonalPricingStrategy.cs
│   │       └── WeekendPricingStrategy.cs
│   ├── Factories/
│   │   ├── IPricingStrategyFactory.cs
│   │   └── PricingStrategyFactory.cs
│   └── DTOs/
│       ├── AuthDtos.cs
│       └── RentalDtos.cs
│
└── Controllers/                    # API Layer
    ├── AuthController.cs
    ├── VehiclesController.cs
    ├── CustomersController.cs
    └── RentalsController.cs
```

## 🛠️ Technologies Utilisées

- **.NET 9**
- **ASP.NET Core Web API**
- **Entity Framework Core 9.0**
- **SQL Server** (compatible avec SSMS)
- **JWT Bearer Authentication**
- **Swagger/OpenAPI**
- **BCrypt.Net** pour le hashing des mots de passe

## 📦 Installation

### Prérequis

- .NET 9 SDK
- **SQL Server** (Express, Developer ou Standard)
- **SQL Server Management Studio (SSMS)** - Recommandé
- Visual Studio 2022 ou VS Code

### Étapes

1. **Cloner le projet**
   ```bash
   cd Backend
   ```

2. **Restaurer les packages NuGet**
   ```bash
   dotnet restore
   ```

3. **Configurer SQL Server**
   
   **Option A : SQL Server avec Windows Authentication (Recommandé)**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=CarRentalDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

   **Option B : SQL Server Express avec instance nommée**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=CarRentalDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

   **Option C : SQL Server avec SQL Authentication**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=CarRentalDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

   📘 **Pour plus de détails** : Voir [SQL_SERVER_SETUP.md](SQL_SERVER_SETUP.md)

4. **Vérifier que SQL Server est démarré**
   ```powershell
   # PowerShell (Admin)
   Get-Service -Name "MSSQLSERVER"  # ou "MSSQL$SQLEXPRESS"
   ```

5. **Lancer l'application**
   ```bash
   dotnet run
   ```

   La base de données `CarRentalDB` sera créée automatiquement au démarrage avec les tables et données de test.

6. **Accéder à Swagger UI**
   
   Ouvrir le navigateur: `https://localhost:5001` ou `http://localhost:5000`

7. **Vérifier la base de données dans SSMS**
   - Ouvrir SSMS
   - Se connecter à `localhost` (ou `localhost\SQLEXPRESS`)
   - Vérifier que la base `CarRentalDB` existe
   - Explorer les tables : Vehicles, Customers, Rentals, etc.

## ⚙️ Configuration

### JWT Settings (appsettings.json)

```json
"Jwt": {
  "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!@#$%",
  "Issuer": "CarRentalAPI",
  "Audience": "CarRentalClient",
  "ExpirationMinutes": "1440"
}
```

### SQL Server Connection

La chaîne de connexion actuelle :
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CarRentalDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### Données de Test Pré-chargées

L'application est livrée avec des données de démonstration:

**Utilisateur Admin:**
- Username: `admin`
- Password: `Admin@123`
- Role: `Admin`

**Véhicules:**
- Toyota Corolla (Compact) - 35€/jour
- BMW X5 (SUV) - 85€/jour
- Mercedes-Benz C-Class (Luxury) - 120€/jour
- Honda Civic (Economy) - 28€/jour

**Client de test:**
- John Doe (Gold Tier)

## 🚀 Utilisation de l'API

### 1. S'authentifier

**POST** `/api/auth/login`

```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Réponse:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@carrental.com",
  "role": "Admin"
}
```

### 2. Utiliser le Token

Dans Swagger UI:
1. Cliquer sur le bouton **Authorize** 🔒
2. Entrer: `Bearer [votre-token]`
3. Cliquer sur **Authorize**

Dans Postman/autres clients:
- Header: `Authorization: Bearer [votre-token]`

### 3. Créer une Réservation

**POST** `/api/rentals`

```json
{
  "customerId": 1,
  "vehicleId": 1,
  "startDate": "2025-01-25T10:00:00",
  "endDate": "2025-01-30T10:00:00",
  "pricingStrategy": "loyalty"
}
```

### 4. Calculer le Prix

**POST** `/api/rentals/calculate-price`

```json
{
  "vehicleId": 1,
  "customerId": 1,
  "startDate": "2025-01-25T10:00:00",
  "endDate": "2025-01-30T10:00:00",
  "pricingStrategy": "seasonal"
}
```

## 📚 Endpoints

### Authentication

| Méthode | Endpoint | Description | Auth |
|---------|----------|-------------|------|
| POST | `/api/auth/register` | Créer un compte | ❌ |
| POST | `/api/auth/login` | Se connecter | ❌ |

### Vehicles

| Méthode | Endpoint | Description | Auth |
|---------|----------|-------------|------|
| GET | `/api/vehicles` | Liste tous les véhicules | ❌ |
| GET | `/api/vehicles/{id}` | Détails d'un véhicule | ❌ |
| GET | `/api/vehicles/available?startDate&endDate` | Véhicules disponibles | ❌ |
| GET | `/api/vehicles/category/{category}` | Par catégorie | ❌ |
| GET | `/api/vehicles/status/{status}` | Par statut | ❌ |
| POST | `/api/vehicles` | Créer un véhicule | ✅ Admin/Employee |
| PUT | `/api/vehicles/{id}` | Modifier un véhicule | ✅ Admin/Employee |
| DELETE | `/api/vehicles/{id}` | Supprimer un véhicule | ✅ Admin |

### Customers

| Méthode | Endpoint | Description | Auth |
|---------|----------|-------------|------|
| GET | `/api/customers` | Liste tous les clients | ✅ Admin/Employee |
| GET | `/api/customers/{id}` | Détails d'un client | ✅ |
| POST | `/api/customers` | Créer un client | ✅ |
| PUT | `/api/customers/{id}` | Modifier un client | ✅ |
| DELETE | `/api/customers/{id}` | Supprimer un client | ✅ Admin |

### Rentals

| Méthode | Endpoint | Description | Auth |
|---------|----------|-------------|------|
| GET | `/api/rentals` | Liste toutes les locations | ✅ Admin/Employee |
| GET | `/api/rentals/{id}` | Détails d'une location | ✅ |
| GET | `/api/rentals/customer/{customerId}` | Locations d'un client | ✅ |
| POST | `/api/rentals` | Créer une réservation | ✅ |
| POST | `/api/rentals/calculate-price` | Calculer le prix | ✅ |
| PUT | `/api/rentals/{id}/complete` | Terminer une location | ✅ Admin/Employee |
| PUT | `/api/rentals/{id}/cancel` | Annuler une location | ✅ Admin/Employee |

## 🎯 Exemples d'Utilisation des Design Patterns

### Strategy Pattern - Comparaison des Prix

Pour un véhicule à 100€/jour, location de 5 jours, client Gold:

```
Standard:   100€ × 5 = 500€
Loyalty:    500€ - 10% (Gold) = 450€
Seasonal:   500€ + 25% (Été) = 625€
Weekend:    500€ + (2 jours × 15€) = 530€
```

### Factory Pattern - Création de Stratégie

```csharp
var factory = new PricingStrategyFactory();
var strategy = factory.CreateStrategy("loyalty");
var price = strategy.CalculatePrice(vehicle, startDate, endDate, customer);
```

### Unit of Work - Transaction Multiple

```csharp
await _unitOfWork.BeginTransactionAsync();
try
{
    await _unitOfWork.Repository<Rental>().AddAsync(rental);
    await _unitOfWork.Repository<Payment>().AddAsync(payment);
    await _unitOfWork.CommitTransactionAsync();
}
catch
{
    await _unitOfWork.RollbackTransactionAsync();
}
```

## 📊 Diagrammes UML

### Diagramme de Classes Simplifié

```
┌─────────────────┐
│    Vehicle      │
├─────────────────┤
│ + Id            │
│ + Brand         │
│ + Model         │
│ + DailyRate     │
│ + Status        │
└─────────────────┘
        △
        │ 1..*
        │
┌─────────────────┐       ┌─────────────────┐
│    Rental       │◇──────│   Customer      │
├─────────────────┤       ├─────────────────┤
│ + Id            │       │ + Id            │
│ + StartDate     │       │ + FirstName     │
│ + EndDate       │       │ + LastName      │
│ + TotalCost     │       │ + Email         │
│ + Status        │       │ + Tier          │
└─────────────────┘       └─────────────────┘
        △
        │ 1
        │
┌─────────────────┐
│    Payment      │
├─────────────────┤
│ + Id            │
│ + Amount        │
│ + Method        │
│ + Status        │
└─────────────────┘
```

## 🔐 Sécurité

- Mots de passe hashés avec **BCrypt**
- Authentification **JWT** avec expiration
- Autorisation basée sur les rôles
- Validation des entrées avec **Data Annotations**
- Protection CORS configurée
- **SQL Server** avec authentification Windows (sécurisé)

## 🗄️ Base de Données

### Vérifier avec SSMS

```sql
-- Se connecter à localhost dans SSMS
USE CarRentalDB;

-- Voir toutes les tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Vérifier les données
SELECT * FROM Vehicles;
SELECT * FROM Customers;
SELECT * FROM Users;
```

### Réinitialiser la base de données

```sql
USE master;
GO
ALTER DATABASE CarRentalDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE CarRentalDB;
GO
-- Puis relancer l'application
```

## 📝 Notes pour le Rapport

### Avantages des Design Patterns Utilisés

1. **Repository Pattern**
   - ✅ Séparation des préoccupations
   - ✅ Testabilité améliorée
   - ✅ Réutilisabilité du code
   - ✅ Flexibilité pour changer de technologie de persistance

2. **Unit of Work Pattern**
   - ✅ Gestion cohérente des transactions
   - ✅ Amélioration des performances
   - ✅ Évite les incohérences de données

3. **Strategy Pattern**
   - ✅ Flexibilité dans le calcul des prix
   - ✅ Facile d'ajouter de nouvelles stratégies
   - ✅ Respecte le principe Open/Closed

4. **Factory Pattern**
   - ✅ Centralise la création d'objets
   - ✅ Réduit le couplage
   - ✅ Facilite les tests

5. **Dependency Injection**
   - ✅ Faible couplage
   - ✅ Haute testabilité
   - ✅ Maintenance simplifiée

## 📚 Documentation Complémentaire

- **[QUICKSTART.md](QUICKSTART.md)** : Guide de démarrage rapide avec exemples
- **[RAPPORT_DOCUMENTATION.md](RAPPORT_DOCUMENTATION.md)** : Documentation complète pour le rapport universitaire avec diagrammes UML
- **[PROJECT_STRUCTURE.md](PROJECT_STRUCTURE.md)** : Structure détaillée du projet
- **[SQL_SERVER_SETUP.md](SQL_SERVER_SETUP.md)** : Guide de configuration SQL Server et SSMS

## 👨‍💻 Auteur

Projet universitaire - Système de Gestion de Location de Voitures

## 📄 Licence

Ce projet est développé dans un cadre éducatif.
