# Cas de Test Détaillés - Système de Location de Voitures

## Format Standard des Cas de Test

Tous les cas de test suivent le format suivant:

| Champ | Description |
|-------|-------------|
| ID Cas de test | Identifiant unique (ex: TC001) |
| Titre | Description courte du test |
| Créé par | Nom du testeur |
| Revue par | Nom du reviewer |
| Version | Numéro de version |
| Nom du testeur | Exécutant |
| Date de test | Date d'exécution |
| Statut | Pass / Fail / Not Executed / Blocked |
| Niveau de test | Unitaire / Intégration / Système |
| Technique | Boîte noire / Boîte blanche |
| Type | Fonctionnel / Non-fonctionnel |
| Priorité | Haute / Moyenne / Basse |

---

## 1. Tests Unitaires - Services

### TC001: Calculer le prix de location avec dates valides

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC001 |
| **Titre** | Calculer le prix de location avec dates valides |
| **Créé par** | Équipe Test |
| **Revue par** | Chef de Projet |
| **Version** | 1.0 |
| **Date de création** | Décembre 2024 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Unitaire |
| **Technique** | Boîte noire - Valeurs limites |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |

#### Prérequis:
1. Service RentalService initialisé
2. Mock des dépendances configuré

#### Jeu de données de test:
```csharp
StartDate: DateTime.Now.AddDays(1)
EndDate: DateTime.Now.AddDays(3)
DailyRate: 50.0 TND
```

#### Scénario de test:
Vérifier que le calcul du prix est correct pour une période de location valide

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Appeler CalculateRentalPrice avec dates valides | Méthode s'exécute sans exception | | ? Not Executed |
| 2 | Vérifier le prix retourné | Prix = 100 TND (2 jours × 50 TND) | | ? Not Executed |
| 3 | Valider que le calcul exclut le jour de début | Nombre de jours = 2 | | ? Not Executed |

#### Résultat attendu:
- Prix calculé: 100.0 TND
- Aucune exception levée

---

### TC002: Calculer le prix avec date de fin avant date de début

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC002 |
| **Titre** | Date de fin avant date de début doit lever une exception |
| **Créé par** | Équipe Test |
| **Revue par** | Chef de Projet |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Unitaire |
| **Technique** | Boîte noire - Validation des erreurs |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |

#### Prérequis:
1. Service RentalService initialisé

#### Jeu de données de test:
```csharp
StartDate: DateTime.Now.AddDays(3)
EndDate: DateTime.Now.AddDays(1)  // Avant StartDate
DailyRate: 50.0 TND
```

#### Scénario de test:
Vérifier que le système rejette les dates invalides (ordre inversé)

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Appeler CalculateRentalPrice avec dates inversées | ArgumentException levée | | ? Not Executed |
| 2 | Vérifier le message d'erreur | Message contient "date" et "invalide" | | ? Not Executed |

#### Résultat attendu:
- Exception: ArgumentException
- Message: "La date de fin doit être après la date de début"

---

### TC003: Calculer le prix avec tarif journalier négatif

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC003 |
| **Titre** | Tarif négatif doit être rejeté |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Unitaire |
| **Technique** | Boîte noire - Classes d'équivalence (valeurs invalides) |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |

#### Jeu de données de test:
```csharp
StartDate: DateTime.Now.AddDays(1)
EndDate: DateTime.Now.AddDays(3)
DailyRate: -50.0 TND  // Valeur négative invalide
```

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Appeler CalculateRentalPrice avec tarif négatif | ArgumentException levée | | ? Not Executed |
| 2 | Vérifier le message d'erreur | Message indique tarif invalide | | ? Not Executed |

---

## 2. Tests d'Intégration - API

### TC011: Login avec identifiants valides

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC011 |
| **Titre** | Connexion API avec credentials valides retourne un token |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Intégration |
| **Technique** | Boîte noire - Scénario nominal |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |

#### Prérequis:
1. API Backend lancée sur http://localhost:5001
2. Base de données seeded avec utilisateur admin
3. Client HTTP configuré

#### Jeu de données de test:
```json
{
  "email": "admin@carrental.com",
  "password": "Admin@123"
}
```

#### Scénario de test:
Vérifier que l'authentification fonctionne et retourne un token JWT valide

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | POST /api/auth/login avec credentials valides | Status Code: 200 OK | | ? Not Executed |
| 2 | Vérifier le corps de la réponse | Contient propriété "token" non vide | | ? Not Executed |
| 3 | Vérifier le format du token | Token JWT valide (3 parties séparées par .) | | ? Not Executed |
| 4 | Vérifier les claims du token | Contient email et rôle | | ? Not Executed |

#### Résultat attendu:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "admin@carrental.com",
  "role": "Admin"
}
```

---

### TC012: Login avec mot de passe incorrect

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC012 |
| **Titre** | Connexion avec MDP incorrect retourne 401 |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Intégration |
| **Technique** | Boîte noire - Cas d'erreur |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |

#### Jeu de données de test:
```json
{
  "email": "admin@carrental.com",
  "password": "WrongPassword123"
}
```

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | POST /api/auth/login avec MDP incorrect | Status Code: 401 Unauthorized | | ? Not Executed |
| 2 | Vérifier qu'aucun token n'est retourné | Réponse ne contient pas de token | | ? Not Executed |
| 3 | Vérifier le message d'erreur | Message approprié affiché | | ? Not Executed |

---

## 3. Tests Système - UI Selenium

### TC023: Se connecter via l'interface avec credentials valides

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC023 |
| **Titre** | Se connecter avec des identifiants valides |
| **Créé par** | Équipe Test |
| **Revue par** | Chef de Projet |
| **Version** | 1.0 |
| **Nom du testeur** | Testeur UI |
| **Date de test** | ? À définir |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Système |
| **Technique** | Boîte noire - Scénario utilisateur complet |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |
| **Automatisation** | ? Selenium WebDriver |

#### Prérequis:
1. Accéder à Google Chrome (version récente)
2. Effacer le localStorage (pas de token existant)
3. Application Frontend lancée sur http://localhost:5000
4. Backend API disponible

#### Jeu de données de test:
```
Email: admin@carrental.com
Password: Admin@123
```

#### Scénario de test:
Vérifier qu'un utilisateur peut se connecter via l'interface web et accéder à son espace

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Aller vers http://localhost:5000/login | La page de connexion s'affiche correctement | | ? Not Executed |
| 2 | Vérifier que les champs email et password sont présents | Les 2 champs sont visibles et actifs | | ? Not Executed |
| 3 | Entrer l'email dans le champ approprié | Le champ accepte la valeur sans problème | | ? Not Executed |
| 4 | Entrer le mot de passe | Le champ password masque les caractères | | ? Not Executed |
| 5 | Cliquer sur le bouton de connexion | Le bouton est cliquable | | ? Not Executed |
| 6 | Attendre la redirection | L'utilisateur est redirigé vers /dashboard ou /home | | ? Not Executed |
| 7 | Vérifier l'URL actuelle | L'URL ne contient plus "/login" | | ? Not Executed |
| 8 | Vérifier le localStorage | Un token JWT est stocké | | ? Not Executed |

#### Résultat attendu:
- ? Connexion réussie
- ? Redirection vers l'espace approprié
- ? Token stocké dans localStorage
- ? Interface utilisateur affiche les informations correctes

#### Critères d'échec:
- ? Pas de redirection après 5 secondes
- ? Message d'erreur affiché
- ? Reste sur la page de login

---

### TC024: Se connecter avec mot de passe incorrect via UI

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC024 |
| **Titre** | Connexion UI avec mot de passe incorrect affiche erreur |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Système |
| **Technique** | Boîte noire - Cas d'erreur UI |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |
| **Automatisation** | ? Selenium WebDriver |

#### Jeu de données de test:
```
Email: admin@carrental.com
Password: WrongPassword123
```

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Naviguer vers /login | Page de connexion affichée | | ? Not Executed |
| 2 | Entrer email valide | Champ accepte la valeur | | ? Not Executed |
| 3 | Entrer mot de passe incorrect | Champ accepte la valeur | | ? Not Executed |
| 4 | Cliquer sur connexion | Bouton déclenche l'action | | ? Not Executed |
| 5 | Attendre la réponse (max 3 sec) | Message d'erreur s'affiche | | ? Not Executed |
| 6 | Vérifier le contenu du message | Message indique "mot de passe incorrect" ou similaire | | ? Not Executed |
| 7 | Vérifier l'URL | Utilisateur reste sur /login | | ? Not Executed |
| 8 | Vérifier localStorage | Aucun token n'est stocké | | ? Not Executed |

---

### TC028: Parcourir la liste des véhicules disponibles

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC028 |
| **Titre** | Afficher la liste des véhicules disponibles |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Système |
| **Technique** | Boîte noire - Consultation |
| **Type** | Fonctionnel |
| **Priorité** | ? Haute |
| **Automatisation** | ? Selenium WebDriver |

#### Prérequis:
1. Base de données contient au moins 3 véhicules disponibles
2. Application lancée
3. Navigateur configuré

#### Scénario de test:
Vérifier qu'un utilisateur peut consulter les véhicules disponibles à la location

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Naviguer vers /vehicles ou /browse | Page de véhicules s'affiche | | ? Not Executed |
| 2 | Attendre le chargement (max 5 sec) | Véhicules affichés sous forme de cartes | | ? Not Executed |
| 3 | Compter le nombre de véhicules | Au moins 1 véhicule visible | | ? Not Executed |
| 4 | Vérifier les informations affichées | Chaque carte contient: marque, modèle, prix | | ? Not Executed |
| 5 | Vérifier la présence d'images | Images de véhicules chargées | | ? Not Executed |
| 6 | Scroller la page | Tous les véhicules sont accessibles | | ? Not Executed |

---

## 4. Tests Non-Fonctionnels

### TC032: Test de performance - Temps de chargement

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC032 |
| **Titre** | La page des véhicules se charge en moins de 5 secondes |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Système |
| **Technique** | Mesure de performance |
| **Type** | Non-fonctionnel - Performance |
| **Priorité** | Moyenne |
| **Automatisation** | ? Selenium WebDriver |

#### Prérequis:
1. Connexion internet stable
2. Serveur Backend et Frontend opérationnels
3. Base de données contient ~20 véhicules

#### Critères de performance:
- **Excellent**: < 2 secondes
- **Bon**: 2-3 secondes
- **Acceptable**: 3-5 secondes
- **Inacceptable**: > 5 secondes

#### Étapes de test:

| Étape # | Étapes | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|--------|-------------------|-----------------|-----------|
| 1 | Noter le temps de début | Timestamp enregistré | | ? Not Executed |
| 2 | Naviguer vers /vehicles | Navigation démarrée | | ? Not Executed |
| 3 | Attendre l'affichage complet | Tous les éléments visibles | | ? Not Executed |
| 4 | Noter le temps de fin | Timestamp enregistré | | ? Not Executed |
| 5 | Calculer la durée | Temps < 5 secondes | | ? Not Executed |

#### Résultat attendu:
- Temps de chargement: **< 5 secondes**
- Tous les véhicules affichés
- Images chargées

---

### TC033: Test d'ergonomie - Design responsive

| Champ | Valeur |
|-------|--------|
| **ID Cas de test** | TC033 |
| **Titre** | L'application s'adapte à différentes résolutions d'écran |
| **Créé par** | Équipe Test |
| **Version** | 1.0 |
| **Statut** | ? Not Yet Executed |
| **Niveau** | Système |
| **Technique** | Test de compatibilité |
| **Type** | Non-fonctionnel - Ergonomie |
| **Priorité** | Moyenne |
| **Automatisation** | ? Selenium WebDriver |

#### Résolutions à tester:
1. **Desktop**: 1920x1080
2. **Tablet**: 768x1024
3. **Mobile**: 375x667

#### Étapes de test:

| Étape # | Résolution | Résultats Attendus | Résultats Réels | Pass/Fail |
|---------|------------|-------------------|-----------------|-----------|
| 1 | 1920x1080 | Interface s'affiche correctement, navigation accessible | | ? Not Executed |
| 2 | 768x1024 | Menu adapté, cartes redimensionnées, texte lisible | | ? Not Executed |
| 3 | 375x667 | Menu burger, cartes empilées, scrolling fluide | | ? Not Executed |

---

## 5. Récapitulatif des Tests

| Catégorie | Nombre de Tests | Statut |
|-----------|-----------------|--------|
| Tests Unitaires | 5 | ? À exécuter |
| Tests d'Intégration | 12 | ? À exécuter |
| Tests Système | 11 | ? À exécuter |
| Tests Non-Fonctionnels | 5 | ? À exécuter |
| **TOTAL** | **33** | **? À exécuter** |

## 6. Instructions d'Exécution

### Tests Unitaires et d'Intégration
```bash
# Naviguer vers le projet de tests
cd Backend.Tests

# Exécuter tous les tests
dotnet test

# Exécuter avec couverture de code
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Exécuter une catégorie spécifique
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

### Tests Selenium
```bash
# Naviguer vers le projet de tests UI
cd Frontend.Tests

# Exécuter les tests Selenium
dotnet test

# Exécuter un test spécifique
dotnet test --filter "FullyQualifiedName~TC023"
```

## 7. Maintenance des Cas de Test

- **Mise à jour**: Après chaque modification de l'application
- **Révision**: Mensuelle ou après sprint
- **Archivage**: Tests obsolètes déplacés vers /Archived

---

**Version**: 1.0  
**Date**: Décembre 2024  
**Auteurs**: Équipe Test  
**Statut**: ? Approuvé pour exécution
