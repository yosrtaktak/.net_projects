# 📋 RAPPORT DE PROJET
## Système de Gestion de Location de Voitures
### Application .NET 9 avec Blazor WebAssembly

---

## Table des Matières

1. [Introduction](#1-introduction)
2. [Architecture du Projet](#2-architecture-du-projet)
3. [Design Patterns Utilisés](#3-design-patterns-utilisés)
4. [Conception - Diagrammes](#4-conception---diagrammes)
5. [Captures d'Écran](#5-captures-décran)
6. [Conclusion](#6-conclusion)

---

## 1. Introduction

### 1.1 Contexte du Projet

Ce projet consiste en le développement d'un **système complet de gestion de location de voitures** utilisant les technologies modernes de Microsoft. L'application permet aux clients de parcourir, réserver et gérer leurs locations de véhicules, tandis que les administrateurs et employés peuvent gérer la flotte, les clients et les opérations de location.

### 1.2 Objectifs

- **Modernisation** : Utilisation de .NET 9 et Blazor WebAssembly pour une application web moderne et performante
- **Maintenabilité** : Application de design patterns reconnus pour faciliter l'évolution du code
- **Sécurité** : Implémentation d'ASP.NET Core Identity avec authentification JWT
- **Scalabilité** : Architecture en couches permettant une extension facile des fonctionnalités

### 1.3 Technologies Utilisées

| Technologie | Version | Utilisation |
|-------------|---------|-------------|
| .NET | 9.0 | Framework principal |
| Blazor WebAssembly | - | Interface utilisateur |
| ASP.NET Core | 9.0 | API Backend |
| Entity Framework Core | 9.0 | ORM / Accès aux données |
| SQL Server | - | Base de données |
| MudBlazor | - | Composants UI |
| JWT | - | Authentification |

---

## 2. Architecture du Projet

### 2.1 Structure de la Solution

Le projet suit une **architecture en couches (Clean Architecture)** avec une séparation claire des responsabilités :

```
📁 Solution Car Rental System
├── 📁 Backend/
│   ├── 📁 Core/                    # Couche Domaine
│   │   ├── 📁 Entities/            # Entités métier
│   │   └── 📁 Interfaces/          # Contrats/Abstractions
│   │
│   ├── 📁 Application/             # Couche Application
│   │   ├── 📁 DTOs/                # Objets de transfert
│   │   ├── 📁 Services/            # Services métier
│   │   └── 📁 Factories/           # Fabriques
│   │
│   ├── 📁 Infrastructure/          # Couche Infrastructure
│   │   ├── 📁 Data/                # DbContext & Seeders
│   │   ├── 📁 Repositories/        # Implémentation Repository
│   │   └── 📁 UnitOfWork/          # Pattern Unit of Work
│   │
│   └── 📁 Controllers/             # API REST
│
└── 📁 Frontend/
    ├── 📁 Pages/                   # Pages Blazor
    ├── 📁 Layout/                  # Layouts (Admin/Customer)
    ├── 📁 Models/                  # Modèles côté client
    └── 📁 Services/                # Services HTTP
```

### 2.2 Entités du Domaine

```
┌─────────────────────────────────────────────────────────────────┐
│                        ENTITÉS PRINCIPALES                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────────┐         ┌──────────────────┐              │
│  │ ApplicationUser  │         │     Category     │              │
│  ├──────────────────┤         ├──────────────────┤              │
│  │ - Id             │         │ - Id             │              │
│  │ - FirstName      │         │ - Name           │              │
│  │ - LastName       │         │ - Description    │              │
│  │ - Email          │         │ - IsActive       │              │
│  │ - DriverLicense  │         │ - DisplayOrder   │              │
│  │ - Tier           │         └────────┬─────────┘              │
│  └────────┬─────────┘                  │                        │
│           │                            │ 1                      │
│           │ 1                          │                        │
│           │                            ▼ *                      │
│           │                  ┌──────────────────┐               │
│           │                  │     Vehicle      │               │
│           │                  ├──────────────────┤               │
│           │                  │ - Id             │               │
│           │                  │ - Brand          │               │
│           │                  │ - Model          │               │
│           │                  │ - Year           │               │
│           │                  │ - DailyRate      │               │
│           │                  │ - Status         │               │
│           │                  │ - Mileage        │               │
│           │                  └────────┬─────────┘               │
│           │                           │                         │
│           │ *                         │ 1                       │
│           ▼                           ▼ *                       │
│  ┌──────────────────┐       ┌──────────────────┐               │
│  │      Rental      │◄──────│   Maintenance    │               │
│  ├──────────────────┤       ├──────────────────┤               │
│  │ - Id             │       │ - Id             │               │
│  │ - StartDate      │       │ - Description    │               │
│  │ - EndDate        │       │ - Cost           │               │
│  │ - TotalCost      │       │ - Date           │               │
│  │ - Status         │       │ - Type           │               │
│  └────────┬─────────┘       └──────────────────┘               │
│           │                                                     │
│           │ 1                                                   │
│           ▼                                                     │
│  ┌──────────────────┐       ┌──────────────────┐               │
│  │     Payment      │       │  VehicleDamage   │               │
│  ├──────────────────┤       ├──────────────────┤               │
│  │ - Id             │       │ - Id             │               │
│  │ - Amount         │       │ - Description    │               │
│  │ - PaymentDate    │       │ - RepairCost     │               │
│  │ - Method         │       │ - ReportedDate   │               │
│  └──────────────────┘       └──────────────────┘               │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Design Patterns Utilisés

### 3.1 Pattern Strategy (Stratégie de Tarification)

#### 📌 Description
Le **Pattern Strategy** permet de définir une famille d'algorithmes, d'encapsuler chacun d'eux et de les rendre interchangeables. Dans notre projet, il est utilisé pour calculer les prix de location selon différentes stratégies.

#### 📐 Diagramme de Classes - Strategy Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                     STRATEGY PATTERN                             │
│                   (Stratégies de Tarification)                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│              ┌─────────────────────────────┐                    │
│              │    <<interface>>            │                    │
│              │    IPricingStrategy         │                    │
│              ├─────────────────────────────┤                    │
│              │ + CalculatePrice()          │                    │
│              │ + StrategyName              │                    │
│              └──────────────┬──────────────┘                    │
│                             │                                    │
│          ┌──────────────────┼──────────────────┐                │
│          │                  │                  │                │
│          ▼                  ▼                  ▼                │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐       │
│  │  Standard     │  │   Loyalty     │  │   Seasonal    │       │
│  │  Pricing      │  │   Pricing     │  │   Pricing     │       │
│  │  Strategy     │  │   Strategy    │  │   Strategy    │       │
│  ├───────────────┤  ├───────────────┤  ├───────────────┤       │
│  │ Prix normal   │  │ -5% Silver    │  │ +25% été      │       │
│  │ = jours ×     │  │ -10% Gold     │  │ +25% décembre │       │
│  │   tarif jour  │  │ -15% Platinum │  │               │       │
│  └───────────────┘  └───────────────┘  └───────────────┘       │
│                                                                  │
│                     ┌───────────────┐                           │
│                     │   Weekend     │                           │
│                     │   Pricing     │                           │
│                     │   Strategy    │                           │
│                     ├───────────────┤                           │
│                     │ +15% samedi   │                           │
│                     │ +15% dimanche │                           │
│                     └───────────────┘                           │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### 💻 Implémentation

**Interface IPricingStrategy :**
```csharp
public interface IPricingStrategy
{
    decimal CalculatePrice(Vehicle vehicle, DateTime startDate, 
                          DateTime endDate, ApplicationUser user);
    string StrategyName { get; }
}
```

**Stratégie Standard :**
```csharp
public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard Pricing";

    public decimal CalculatePrice(Vehicle vehicle, DateTime startDate, 
                                  DateTime endDate, ApplicationUser user)
    {
        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;
        return vehicle.DailyRate * days;
    }
}
```

**Stratégie Fidélité (avec réductions selon le tier client) :**
```csharp
public class LoyaltyPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Loyalty Pricing";

    public decimal CalculatePrice(Vehicle vehicle, DateTime startDate, 
                                  DateTime endDate, ApplicationUser user)
    {
        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;
        
        var basePrice = vehicle.DailyRate * days;
        
        // Réduction selon le niveau du client
        var discount = user.Tier switch
        {
            CustomerTier.Silver => 0.05m,     // 5% de réduction
            CustomerTier.Gold => 0.10m,       // 10% de réduction
            CustomerTier.Platinum => 0.15m,   // 15% de réduction
            _ => 0m
        };
        
        return basePrice * (1 - discount);
    }
}
```

#### ✅ Avantages du Pattern Strategy

| Avantage | Description |
|----------|-------------|
| **Extensibilité** | Ajout de nouvelles stratégies sans modifier le code existant |
| **Principe Open/Closed** | Le système est ouvert à l'extension, fermé à la modification |
| **Testabilité** | Chaque stratégie peut être testée indépendamment |
| **Flexibilité** | Changement de stratégie à l'exécution selon le contexte |
| **Séparation des préoccupations** | Chaque algorithme est isolé dans sa propre classe |

---

### 3.2 Pattern Factory (Fabrique de Stratégies)

#### 📌 Description
Le **Pattern Factory** fournit une interface pour créer des objets sans spécifier leurs classes concrètes. Il est utilisé conjointement avec le Pattern Strategy pour instancier la bonne stratégie de tarification.

#### 📐 Diagramme de Classes - Factory Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                      FACTORY PATTERN                             │
│                  (Fabrique de Stratégies)                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌────────────────────────────┐                                 │
│  │     <<interface>>          │                                 │
│  │  IPricingStrategyFactory   │                                 │
│  ├────────────────────────────┤                                 │
│  │ + CreateStrategy(type)     │                                 │
│  │ + GetAvailableStrategies() │                                 │
│  └─────────────┬──────────────┘                                 │
│                │                                                 │
│                ▼                                                 │
│  ┌────────────────────────────┐                                 │
│  │  PricingStrategyFactory    │                                 │
│  ├────────────────────────────┤                                 │
│  │ - _strategies: Dictionary  │───────┐                        │
│  ├────────────────────────────┤       │                        │
│  │ + CreateStrategy(type)     │       │ crée                   │
│  │ + GetAvailableStrategies() │       │                        │
│  └────────────────────────────┘       ▼                        │
│                                ┌──────────────────┐             │
│                                │ IPricingStrategy │             │
│                                └──────────────────┘             │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### 💻 Implémentation

```csharp
public interface IPricingStrategyFactory
{
    IPricingStrategy CreateStrategy(string strategyType);
    IEnumerable<string> GetAvailableStrategies();
}

public class PricingStrategyFactory : IPricingStrategyFactory
{
    private readonly Dictionary<string, Func<IPricingStrategy>> _strategies;

    public PricingStrategyFactory()
    {
        _strategies = new Dictionary<string, Func<IPricingStrategy>>
                      (StringComparer.OrdinalIgnoreCase)
        {
            { "standard", () => new StandardPricingStrategy() },
            { "loyalty", () => new LoyaltyPricingStrategy() },
            { "seasonal", () => new SeasonalPricingStrategy() },
            { "weekend", () => new WeekendPricingStrategy() }
        };
    }

    public IPricingStrategy CreateStrategy(string strategyType)
    {
        if (_strategies.TryGetValue(strategyType, out var factory))
        {
            return factory();
        }
        return new StandardPricingStrategy(); // Par défaut
    }

    public IEnumerable<string> GetAvailableStrategies()
    {
        return _strategies.Keys;
    }
}
```

#### ✅ Avantages du Pattern Factory

| Avantage | Description |
|----------|-------------|
| **Centralisation** | La création d'objets est centralisée en un seul point |
| **Découplage** | Le code client ne connaît pas les classes concrètes |
| **Maintenance** | Facilite l'ajout de nouvelles stratégies |
| **Configuration** | Permet la configuration dynamique des créations |

---

### 3.3 Pattern Repository

#### 📌 Description
Le **Pattern Repository** agit comme une abstraction de la couche d'accès aux données. Il fournit une collection d'objets comme si c'était une collection en mémoire.

#### 📐 Diagramme de Classes - Repository Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                     REPOSITORY PATTERN                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│          ┌─────────────────────────────┐                        │
│          │      <<interface>>          │                        │
│          │      IRepository<T>         │                        │
│          ├─────────────────────────────┤                        │
│          │ + GetByIdAsync(id)          │                        │
│          │ + GetAllAsync()             │                        │
│          │ + FindAsync(predicate)      │                        │
│          │ + AddAsync(entity)          │                        │
│          │ + Update(entity)            │                        │
│          │ + Remove(entity)            │                        │
│          │ + CountAsync()              │                        │
│          └──────────────┬──────────────┘                        │
│                         │                                        │
│      ┌──────────────────┼──────────────────┐                    │
│      │                  │                  │                    │
│      ▼                  ▼                  ▼                    │
│ ┌──────────┐     ┌──────────┐      ┌──────────┐                │
│ │IVehicle  │     │ IRental  │      │ICategory │                │
│ │Repository│     │Repository│      │Repository│                │
│ └────┬─────┘     └────┬─────┘      └────┬─────┘                │
│      │                │                  │                      │
│      ▼                ▼                  ▼                      │
│ ┌──────────┐     ┌──────────┐      ┌──────────┐                │
│ │Vehicle   │     │ Rental   │      │Category  │                │
│ │Repository│     │Repository│      │Repository│                │
│ └──────────┘     └──────────┘      └──────────┘                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### 💻 Implémentation

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CarRentalDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CarRentalDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    // ... autres méthodes
}
```

#### ✅ Avantages du Pattern Repository

| Avantage | Description |
|----------|-------------|
| **Abstraction** | Cache la complexité de l'accès aux données |
| **Testabilité** | Permet le mocking pour les tests unitaires |
| **Centralisation** | Logique d'accès aux données en un seul endroit |
| **Flexibilité** | Changement de source de données sans impact |

---

### 3.4 Pattern Unit of Work

#### 📌 Description
Le **Pattern Unit of Work** maintient une liste d'objets affectés par une transaction et coordonne l'écriture des modifications.

#### 📐 Diagramme de Classes - Unit of Work Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                    UNIT OF WORK PATTERN                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────────────────┐                                │
│  │      <<interface>>          │                                │
│  │       IUnitOfWork           │                                │
│  ├─────────────────────────────┤                                │
│  │ + Repository<T>()           │                                │
│  │ + CommitAsync()             │                                │
│  │ + BeginTransactionAsync()   │                                │
│  │ + CommitTransactionAsync()  │                                │
│  │ + RollbackTransactionAsync()│                                │
│  └──────────────┬──────────────┘                                │
│                 │                                                │
│                 ▼                                                │
│  ┌─────────────────────────────┐                                │
│  │        UnitOfWork           │                                │
│  ├─────────────────────────────┤                                │
│  │ - _context: DbContext       │                                │
│  │ - _repositories: Dictionary │                                │
│  │ - _transaction              │                                │
│  ├─────────────────────────────┤                                │
│  │ + Repository<T>()           │────► IRepository<T>            │
│  │ + CommitAsync()             │                                │
│  │ + BeginTransactionAsync()   │                                │
│  │ + CommitTransactionAsync()  │                                │
│  │ + RollbackTransactionAsync()│                                │
│  └─────────────────────────────┘                                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### 💻 Implémentation

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> CommitAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly CarRentalDbContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(CarRentalDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>).MakeGenericType(type);
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(type, repositoryInstance!);
        }
        
        return (IRepository<T>)_repositories[type];
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Gestion des transactions...
}
```

#### ✅ Avantages du Pattern Unit of Work

| Avantage | Description |
|----------|-------------|
| **Cohérence** | Garantit l'intégrité des données |
| **Performance** | Un seul commit pour plusieurs opérations |
| **Transactions** | Gestion automatique des transactions |
| **Coordination** | Orchestre plusieurs repositories |

---

## 4. Conception - Diagrammes

### 4.1 Diagramme de Classes Complet

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        DIAGRAMME DE CLASSES                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   ┌───────────────────┐           ┌───────────────────┐                     │
│   │   IdentityUser    │           │    Category       │                     │
│   │   (ASP.NET)       │           ├───────────────────┤                     │
│   └─────────┬─────────┘           │ +Id: int          │                     │
│             │                     │ +Name: string     │                     │
│             ▼                     │ +Description      │                     │
│   ┌───────────────────┐           │ +IsActive: bool   │                     │
│   │  ApplicationUser  │           │ +DisplayOrder     │                     │
│   ├───────────────────┤           └─────────┬─────────┘                     │
│   │ +FirstName        │                     │ 1                             │
│   │ +LastName         │                     │                               │
│   │ +DriverLicense    │                     │                               │
│   │ +DateOfBirth      │                     ▼ *                             │
│   │ +Address          │           ┌───────────────────┐                     │
│   │ +Tier: CustomerTier│◄─────────│     Vehicle       │                     │
│   │ +RegistrationDate │     1   * ├───────────────────┤                     │
│   │ +Rentals: List    │           │ +Id: int          │                     │
│   └─────────┬─────────┘           │ +Brand: string    │                     │
│             │                     │ +Model: string    │                     │
│             │ 1                   │ +Year: int        │                     │
│             │                     │ +DailyRate: decimal│                    │
│             │                     │ +Status: VehicleStatus│                 │
│             │                     │ +Mileage: int     │                     │
│             ▼ *                   │ +CategoryId: int  │                     │
│   ┌───────────────────┐           └─────────┬─────────┘                     │
│   │      Rental       │                     │ 1                             │
│   ├───────────────────┤                     │                               │
│   │ +Id: int          │◄────────────────────┘                               │
│   │ +UserId: string   │           *                                         │
│   │ +VehicleId: int   │                                                     │
│   │ +StartDate        │           ┌───────────────────┐                     │
│   │ +EndDate          │     1   * │   Maintenance     │                     │
│   │ +TotalCost        │◄──────────├───────────────────┤                     │
│   │ +Status           │           │ +Id: int          │                     │
│   │ +ActualReturnDate │           │ +VehicleId: int   │                     │
│   └─────────┬─────────┘           │ +Description      │                     │
│             │                     │ +Cost: decimal    │                     │
│             │ 1                   │ +Date: DateTime   │                     │
│             │                     │ +Type: string     │                     │
│             ▼                     └───────────────────┘                     │
│   ┌───────────────────┐                                                     │
│   │     Payment       │           ┌───────────────────┐                     │
│   ├───────────────────┤           │  VehicleDamage    │                     │
│   │ +Id: int          │           ├───────────────────┤                     │
│   │ +RentalId: int    │           │ +Id: int          │                     │
│   │ +Amount: decimal  │           │ +VehicleId: int   │                     │
│   │ +PaymentDate      │           │ +RentalId: int?   │                     │
│   │ +Method: string   │           │ +Description      │                     │
│   │ +TransactionId    │           │ +RepairCost       │                     │
│   └───────────────────┘           │ +ReportedDate     │                     │
│                                   └───────────────────┘                     │
│                                                                              │
│   ┌─────────────────────────────────────────────────────────────────────┐   │
│   │                          ÉNUMÉRATIONS                                │   │
│   ├─────────────────────────────────────────────────────────────────────┤   │
│   │  VehicleStatus          RentalStatus         CustomerTier            │   │
│   │  ├── Available          ├── Reserved         ├── Standard            │   │
│   │  ├── Reserved           ├── Active           ├── Silver              │   │
│   │  ├── Rented             ├── Completed        ├── Gold                │   │
│   │  ├── Maintenance        └── Cancelled        └── Platinum            │   │
│   │  └── Retired                                                         │   │
│   └─────────────────────────────────────────────────────────────────────┘   │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Diagramme de Cas d'Utilisation

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                      DIAGRAMME DE CAS D'UTILISATION                          │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│                        ┌────────────────────────┐                           │
│                        │    Système Location    │                           │
│                        │      de Voitures       │                           │
│                        └────────────────────────┘                           │
│                                                                              │
│  ┌─────────┐                                              ┌─────────┐       │
│  │         │         ┌─────────────────────┐              │         │       │
│  │  Client │─────────│   S'authentifier    │──────────────│  Admin  │       │
│  │         │         └─────────────────────┘              │         │       │
│  └────┬────┘                                              └────┬────┘       │
│       │                                                        │            │
│       │              ┌─────────────────────┐                   │            │
│       ├──────────────│  Parcourir véhicules │───────────────────┤            │
│       │              └─────────────────────┘                   │            │
│       │                                                        │            │
│       │              ┌─────────────────────┐                   │            │
│       ├──────────────│  Réserver véhicule  │                   │            │
│       │              └─────────────────────┘                   │            │
│       │                                                        │            │
│       │              ┌─────────────────────┐                   │            │
│       ├──────────────│  Voir mes locations │                   │            │
│       │              └─────────────────────┘                   │            │
│       │                                                        │            │
│       │              ┌─────────────────────┐                   │            │
│       ├──────────────│   Annuler location  │                   │            │
│       │              └─────────────────────┘                   │            │
│       │                                                        │            │
│       │              ┌─────────────────────┐                   │            │
│       └──────────────│   Gérer mon profil  │                   │            │
│                      └─────────────────────┘                   │            │
│                                                                │            │
│                      ┌─────────────────────┐                   │            │
│                      │  Gérer les véhicules │───────────────────┤            │
│                      └─────────────────────┘                   │            │
│                                                                │            │
│                      ┌─────────────────────┐                   │            │
│                      │   Gérer locations   │───────────────────┤            │
│                      └─────────────────────┘                   │            │
│                                                                │            │
│                      ┌─────────────────────┐                   │            │
│                      │   Gérer clients     │───────────────────┤            │
│                      └─────────────────────┘                   │            │
│                                                                │            │
│                      ┌─────────────────────┐                   │            │
│                      │  Gérer catégories   │───────────────────┤            │
│                      └─────────────────────┘                   │            │
│                                                                │            │
│                      ┌─────────────────────┐                   │            │
│                      │ Gérer maintenances  │───────────────────┤            │
│                      └─────────────────────┘                   │            │
│                                                                │            │
│                      ┌─────────────────────┐                   │            │
│                      │  Générer rapports   │───────────────────┘            │
│                      └─────────────────────┘                                │
│                                                                              │
│  ┌─────────┐                                                                │
│  │         │         ┌─────────────────────┐                                │
│  │ Employé │─────────│ Gérer les locations │                                │
│  │         │         └─────────────────────┘                                │
│  └────┬────┘                                                                │
│       │              ┌─────────────────────┐                                │
│       ├──────────────│  Voir les clients   │                                │
│       │              └─────────────────────┘                                │
│       │              ┌─────────────────────┐                                │
│       └──────────────│ Enregistrer dommages│                                │
│                      └─────────────────────┘                                │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 4.3 Diagramme de Séquence - Création d'une Location

```
┌─────────────────────────────────────────────────────────────────────────────┐
│            DIAGRAMME DE SÉQUENCE - CRÉATION D'UNE LOCATION                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   Client      Controller      RentalService    PricingFactory   Repository  │
│     │              │               │                 │               │       │
│     │  POST /api/rentals           │                 │               │       │
│     │──────────────►               │                 │               │       │
│     │              │               │                 │               │       │
│     │              │ CreateRentalAsync              │               │       │
│     │              │───────────────►                │               │       │
│     │              │               │                 │               │       │
│     │              │               │ GetByIdAsync(userId)            │       │
│     │              │               │─────────────────────────────────►       │
│     │              │               │                 │               │       │
│     │              │               │◄────────────────────────────────│       │
│     │              │               │   user                          │       │
│     │              │               │                 │               │       │
│     │              │               │ IsVehicleAvailable              │       │
│     │              │               │─────────────────────────────────►       │
│     │              │               │                 │               │       │
│     │              │               │◄────────────────────────────────│       │
│     │              │               │   true                          │       │
│     │              │               │                 │               │       │
│     │              │               │ CreateStrategy("loyalty")       │       │
│     │              │               │─────────────────►               │       │
│     │              │               │                 │               │       │
│     │              │               │◄────────────────│               │       │
│     │              │               │ LoyaltyStrategy │               │       │
│     │              │               │                 │               │       │
│     │              │               │ CalculatePrice()                │       │
│     │              │               │─────────────────►               │       │
│     │              │               │                 │               │       │
│     │              │               │◄────────────────│               │       │
│     │              │               │   price         │               │       │
│     │              │               │                 │               │       │
│     │              │               │ AddAsync(rental)                │       │
│     │              │               │─────────────────────────────────►       │
│     │              │               │                 │               │       │
│     │              │               │ CommitAsync()                   │       │
│     │              │               │─────────────────────────────────►       │
│     │              │               │                 │               │       │
│     │              │◄──────────────│                 │               │       │
│     │              │   rental      │                 │               │       │
│     │              │               │                 │               │       │
│     │◄─────────────│               │                 │               │       │
│     │  201 Created │               │                 │               │       │
│     │  + rental    │               │                 │               │       │
│     │              │               │                 │               │       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 4.4 Diagramme de Séquence - Authentification

```
┌─────────────────────────────────────────────────────────────────────────────┐
│              DIAGRAMME DE SÉQUENCE - AUTHENTIFICATION                        │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│   Client       AuthController    UserManager     JwtService                 │
│     │               │                │               │                       │
│     │ POST /api/auth/login           │               │                       │
│     │───────────────►                │               │                       │
│     │               │                │               │                       │
│     │               │ FindByEmailAsync(email)        │                       │
│     │               │────────────────►               │                       │
│     │               │                │               │                       │
│     │               │◄───────────────│               │                       │
│     │               │    user        │               │                       │
│     │               │                │               │                       │
│     │               │ CheckPasswordAsync(user, pwd)  │                       │
│     │               │────────────────►               │                       │
│     │               │                │               │                       │
│     │               │◄───────────────│               │                       │
│     │               │    true        │               │                       │
│     │               │                │               │                       │
│     │               │ GetRolesAsync(user)            │                       │
│     │               │────────────────►               │                       │
│     │               │                │               │                       │
│     │               │◄───────────────│               │                       │
│     │               │    roles       │               │                       │
│     │               │                │               │                       │
│     │               │ GenerateToken(user, roles)     │                       │
│     │               │────────────────────────────────►                       │
│     │               │                │               │                       │
│     │               │◄───────────────────────────────│                       │
│     │               │               JWT token        │                       │
│     │               │                │               │                       │
│     │◄──────────────│                │               │                       │
│     │  200 OK       │                │               │                       │
│     │  + token      │                │               │                       │
│     │  + expiration │                │               │                       │
│     │  + role       │                │               │                       │
│     │               │                │               │                       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 5. Captures d'Écran

### 5.1 Page d'Accueil (Interface Client)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  🚗 Car Rental System                    Home | Browse Vehicles | [Login]    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ╔═══════════════════════════════════════════════════════════════════════╗  │
│  ║                                                                       ║  │
│  ║                 🚗 Welcome to Car Rental System                       ║  │
│  ║                                                                       ║  │
│  ║              Find the perfect vehicle for your journey                ║  │
│  ║                                                                       ║  │
│  ║         [Browse Vehicles]        [Login to Rent]                      ║  │
│  ║                                                                       ║  │
│  ╚═══════════════════════════════════════════════════════════════════════╝  │
│                                                                              │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐              │
│  │  ⚡ Quick       │  │  ✓ Secure       │  │  💰 Best        │              │
│  │    Booking      │  │    & Safe       │  │    Prices       │              │
│  │                 │  │                 │  │                 │              │
│  │  Book in just   │  │  All vehicles   │  │  Competitive    │              │
│  │  a few clicks   │  │  maintained     │  │  rates with     │              │
│  │                 │  │  & insured      │  │  loyalty rewards│              │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘              │
│                                                                              │
│                      Vehicle Categories                                      │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐                │
│  │ 🚗        │  │ 🚙        │  │ 💎        │  │ 🏎️        │                │
│  │ Economy   │  │ SUV       │  │ Luxury    │  │ Sports    │                │
│  │ Budget-   │  │ Family-   │  │ Premium   │  │ High-     │                │
│  │ friendly  │  │ sized     │  │ comfort   │  │ performance│               │
│  └───────────┘  └───────────┘  └───────────┘  └───────────┘                │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Tableau de Bord Administrateur

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  🚗 Car Rental System                               [Admin] ▼  [Logout]      │
├─────────────────────────────────────────────────────────────────────────────┤
│  ┌─────────┐                                                                │
│  │ 📊      │  ╔═══════════════════════════════════════════════════════╗    │
│  │Dashboard│  ║  👋 Welcome Back, Admin!                              ║    │
│  │         │  ║  Here's your system overview for today                ║    │
│  │ 🚗      │  ╚═══════════════════════════════════════════════════════╝    │
│  │Vehicles │                                                                │
│  │         │  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐ │
│  │ 👥      │  │ Total      │ │ Available  │ │ Active     │ │ Total      │ │
│  │Customers│  │ Vehicles   │ │            │ │ Rentals    │ │ Customers  │ │
│  │         │  │     15     │ │     10     │ │     3      │ │     25     │ │
│  │ 📋      │  │ In Fleet   │ │ Ready      │ │ In Progress│ │ Registered │ │
│  │Rentals  │  └────────────┘ └────────────┘ └────────────┘ └────────────┘ │
│  │         │                                                                │
│  │ 📁      │  ⚡ Quick Actions                                             │
│  │Categories│ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐             │
│  │         │  │[+Add Vehicle]│ │[Manage      ]│ │[View        ]│             │
│  │ 🔧      │  │             │ │ Vehicles    │ │ Customers   │             │
│  │Maint.   │  └─────────────┘ └─────────────┘ └─────────────┘             │
│  │         │                                                                │
│  │ 📈      │  Vehicle Status Overview      Rental Status Overview          │
│  │Reports  │  ┌─────────────────────┐      ┌─────────────────────┐        │
│  └─────────┘  │ ✓ Available    [10]│      │ 📅 Reserved    [5] │        │
│               │ 🚗 Rented      [ 3]│      │ ▶️ Active       [3] │        │
│               │ 🔧 Maintenance [ 2]│      │ ✓ Completed   [15]│        │
│               │ ❌ Retired     [ 0]│      │ ❌ Cancelled   [2] │        │
│               └─────────────────────┘      └─────────────────────┘        │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.3 Page de Réservation de Véhicule

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  🚗 Car Rental System                    Home | Browse Vehicles | My Rentals │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ◄ Back to Vehicles                                                         │
│                                                                              │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │  ┌─────────────────┐                                                  │  │
│  │  │                 │    BMW X5                                        │  │
│  │  │   [Image BMW]   │    ★★★★★ SUV                                     │  │
│  │  │                 │                                                  │  │
│  │  │                 │    Year: 2024 | Mileage: 5,000 km               │  │
│  │  │                 │    Fuel: Diesel | Seats: 7                       │  │
│  │  └─────────────────┘                                                  │  │
│  │                                                                       │  │
│  │  ═══════════════════════════════════════════════════════════════════ │  │
│  │                                                                       │  │
│  │  📅 Rental Details                                                    │  │
│  │                                                                       │  │
│  │  Start Date:  [  2024-12-15  📅]     End Date:  [  2024-12-20  📅]   │  │
│  │                                                                       │  │
│  │  💳 Pricing Strategy:                                                 │  │
│  │  ┌──────────────────────────────────────────┐                        │  │
│  │  │  ○ Standard Pricing                      │                        │  │
│  │  │  ● Loyalty Pricing (-10% Gold Member)    │                        │  │
│  │  │  ○ Seasonal Pricing                      │                        │  │
│  │  │  ○ Weekend Pricing                       │                        │  │
│  │  └──────────────────────────────────────────┘                        │  │
│  │                                                                       │  │
│  │  ╔═══════════════════════════════════════════════════════════════╗   │  │
│  │  ║  Price Summary                                                ║   │  │
│  │  ║  ─────────────────────────────────────────────────────────── ║   │  │
│  │  ║  Daily Rate:        85.00 € × 5 days = 425.00 €              ║   │  │
│  │  ║  Loyalty Discount:                    - 42.50 €              ║   │  │
│  │  ║  ─────────────────────────────────────────────────────────── ║   │  │
│  │  ║  TOTAL:                               382.50 €               ║   │  │
│  │  ╚═══════════════════════════════════════════════════════════════╝   │  │
│  │                                                                       │  │
│  │               [Confirm Reservation]    [Cancel]                       │  │
│  │                                                                       │  │
│  └───────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.4 Liste des Véhicules

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  🚗 Car Rental System                                      Browse Vehicles   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Filter by Category: [All ▼]   Price Range: [0€ - 200€]   [Search 🔍]       │
│                                                                              │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ [Image]     │  │ [Image]     │  │ [Image]     │  │ [Image]     │        │
│  │             │  │             │  │             │  │             │        │
│  │ Toyota      │  │ BMW X5      │  │ Mercedes    │  │ Honda       │        │
│  │ Corolla     │  │             │  │ C-Class     │  │ Civic       │        │
│  │             │  │             │  │             │  │             │        │
│  │ Compact     │  │ SUV         │  │ Luxury      │  │ Economy     │        │
│  │             │  │             │  │             │  │             │        │
│  │ 35€/jour    │  │ 85€/jour    │  │ 120€/jour   │  │ 28€/jour    │        │
│  │             │  │             │  │             │  │             │        │
│  │ ✓ Available │  │ ✓ Available │  │ ✓ Available │  │ ✓ Available │        │
│  │             │  │             │  │             │  │             │        │
│  │ [Details]   │  │ [Details]   │  │ [Details]   │  │ [Details]   │        │
│  │ [Book Now]  │  │ [Book Now]  │  │ [Book Now]  │  │ [Book Now]  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘        │
│                                                                              │
│                        ◄ 1  2  3  4  5 ►                                    │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5.5 Gestion des Locations (Admin)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  📋 Manage Rentals                                        [+ Create Rental]  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Filter: [Status ▼] [Date Range 📅] [Vehicle ▼] [Search...]  [Apply]        │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ ID │ Customer      │ Vehicle        │ Start     │ End       │ Status   ││
│  ├────┼───────────────┼────────────────┼───────────┼───────────┼──────────┤│
│  │ 1  │ John Doe      │ Toyota Corolla │ 2024-12-10│ 2024-12-15│ 🟢Active ││
│  │    │ john@mail.com │ ABC123         │           │           │          ││
│  │    │               │                │           │ Total: 175€│ [Actions]││
│  ├────┼───────────────┼────────────────┼───────────┼───────────┼──────────┤│
│  │ 2  │ Jane Smith    │ BMW X5         │ 2024-12-12│ 2024-12-18│ 🔵Reserved│
│  │    │ jane@mail.com │ XYZ789         │           │           │          ││
│  │    │               │                │           │ Total: 510€│ [Actions]││
│  ├────┼───────────────┼────────────────┼───────────┼───────────┼──────────┤│
│  │ 3  │ Bob Wilson    │ Mercedes C-Class│2024-12-01│ 2024-12-05│ ✓Complete││
│  │    │ bob@mail.com  │ LUX456         │           │           │          ││
│  │    │               │                │           │ Total: 600€│ [Actions]││
│  └─────────────────────────────────────────────────────────────────────────┘│
│                                                                              │
│  Showing 3 of 25 rentals                           ◄ 1  2  3  ...  5 ►      │
│                                                                              │
│  ┌─────────────────────────────────────────────────────────────────────────┐│
│  │ Summary: Reserved: 5 | Active: 3 | Completed: 15 | Cancelled: 2        ││
│  └─────────────────────────────────────────────────────────────────────────┘│
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 6. Conclusion

### 6.1 Récapitulatif des Design Patterns

| Pattern | Utilisation | Bénéfice Principal |
|---------|-------------|-------------------|
| **Strategy** | Stratégies de tarification | Flexibilité des algorithmes de calcul de prix |
| **Factory** | Création de stratégies | Centralisation et découplage de l'instanciation |
| **Repository** | Accès aux données | Abstraction et testabilité |
| **Unit of Work** | Gestion des transactions | Cohérence et intégrité des données |

### 6.2 Points Forts du Projet

1. **Architecture Propre** : Séparation claire des responsabilités entre les couches
2. **Extensibilité** : Ajout facile de nouvelles fonctionnalités grâce aux patterns utilisés
3. **Sécurité** : Authentification JWT robuste avec ASP.NET Core Identity
4. **Interface Moderne** : UI responsive avec MudBlazor et Blazor WebAssembly
5. **Maintenabilité** : Code organisé et documenté facilitant les évolutions futures

### 6.3 Perspectives d'Amélioration

- Implémentation du **Pattern Observer** pour les notifications en temps réel
- Ajout du **Pattern Decorator** pour les options de location (GPS, siège enfant, etc.)
- Intégration d'un système de paiement en ligne
- Mise en place de tests unitaires et d'intégration complets
- Déploiement sur Azure avec CI/CD

---

## 📚 Références

- Microsoft .NET Documentation
- Design Patterns: Elements of Reusable Object-Oriented Software (Gang of Four)
- Clean Architecture by Robert C. Martin
- MudBlazor Documentation

---

**Auteur** : Équipe de Développement  
**Date** : Décembre 2024  y
**Version** : 1.0

---

*Ce rapport a été généré dans le cadre du projet de Système de Location de Voitures utilisant .NET 9 et Blazor WebAssembly.*
