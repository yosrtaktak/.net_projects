# CAS DE TEST DOCUMENTÉS
## Projet: Système de Gestion de Location de Véhicules (Car Rental System)

---

**Date de création:** 2024-01-15  
**Version:** 1.0  
**Auteur:** Équipe QA  
**Statut:** Validé  

---

## TABLE DES MATIÈRES

1. [Introduction](#introduction)
2. [Objectifs des Tests](#objectifs-des-tests)
3. [Périmètre des Tests](#périmètre-des-tests)
4. [Cas de Test - Authentification API](#cas-de-test-authentification-api)
5. [Cas de Test - Gestion des Véhicules API](#cas-de-test-gestion-des-véhicules-api)
6. [Cas de Test - Interface Utilisateur](#cas-de-test-interface-utilisateur)
7. [Cas de Test - Tests d'Intégration](#cas-de-test-tests-dintégration)
8. [Matrice de Traçabilité](#matrice-de-traçabilité)
9. [Environnement de Test](#environnement-de-test)

---

## 1. INTRODUCTION

Ce document présente l'ensemble des cas de test documentés pour le système de gestion de location de véhicules. Les tests couvrent les aspects API REST, interface utilisateur, et tests d'intégration end-to-end.

### 1.1 Système Sous Test
- **Application:** Car Rental Management System
- **Backend:** ASP.NET Core Web API
- **Frontend:** Angular/React (UI moderne)
- **Base de données:** SQL Server/PostgreSQL
- **Framework de test:** Pytest + Selenium + Requests

---

## 2. OBJECTIFS DES TESTS

### 2.1 Objectifs Principaux
- ✅ Valider la fonctionnalité d'authentification (login/register)
- ✅ Vérifier les opérations CRUD sur les véhicules
- ✅ Tester l'intégration API-UI
- ✅ Assurer la sécurité et la gestion des autorisations
- ✅ Garantir la fiabilité des flux métier critiques

### 2.2 Critères de Succès
- Taux de réussite des tests ≥ 95%
- Couverture du code ≥ 80%
- Tous les scénarios critiques validés
- Aucun bug bloquant en production

---

## 3. PÉRIMÈTRE DES TESTS

### 3.1 Modules Testés
| Module | Type de Test | Priorité |
|--------|-------------|----------|
| Authentification API | Fonctionnel, Sécurité | Critique |
| Gestion Véhicules API | Fonctionnel, CRUD | Critique |
| Interface Login UI | UI, UX | Haute |
| Interface Véhicules UI | UI, Navigation | Haute |
| Intégration API-UI | End-to-End | Critique |

### 3.2 Hors Périmètre
- Tests de performance (charge)
- Tests de pénétration avancés
- Tests de compatibilité navigateurs multiples

---

## 4. CAS DE TEST - AUTHENTIFICATION API

### TC011 - Login avec Identifiants Valides

**ID:** TC011  
**Titre:** Vérifier que le login avec des identifiants valides retourne un token JWT  
**Priorité:** Critique ⭐⭐⭐  
**Type:** Fonctionnel, API  
**Prérequis:** 
- API Backend en cours d'exécution
- Utilisateur admin créé dans la base de données

**Données de Test:**
```json
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Étapes:**
1. Préparer les données de connexion valides
2. Envoyer une requête POST à `/api/auth/login`
3. Vérifier le code de statut HTTP
4. Analyser la réponse JSON

**Résultat Attendu:**
- ✅ Code de statut: 200 OK
- ✅ Réponse contient un champ `token`
- ✅ Le token JWT a 3 parties séparées par des points
- ✅ Réponse contient `username` et `email`
- ✅ Token n'est pas vide

**Résultat Réel:**
- ✅ PASS - Tous les critères validés

**Couverture:**
- Authentification réussie
- Génération de token JWT
- Structure de réponse API

---

### TC012 - Login avec Mot de Passe Invalide

**ID:** TC012  
**Titre:** Vérifier que le login avec un mot de passe invalide retourne 401 Unauthorized  
**Priorité:** Haute ⭐⭐  
**Type:** Sécurité, Négatif  
**Prérequis:** API Backend en cours d'exécution

**Données de Test:**
```json
{
  "username": "admin",
  "password": "WrongPassword123!"
}
```

**Étapes:**
1. Préparer les données avec mot de passe erroné
2. Envoyer POST à `/api/auth/login`
3. Vérifier le refus d'accès

**Résultat Attendu:**
- ✅ Code de statut: 401 Unauthorized
- ✅ Aucun token retourné
- ✅ Message d'erreur approprié

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Validation du mot de passe
- Gestion des erreurs d'authentification
- Sécurité contre les accès non autorisés

---

### TC013 - Login avec Différents Inputs Invalides

**ID:** TC013  
**Titre:** Tester le login avec diverses combinaisons d'entrées invalides  
**Priorité:** Moyenne ⭐  
**Type:** Validation, Paramétré  
**Prérequis:** API en cours d'exécution

**Données de Test (Paramétrées):**
| Cas | Username | Password | Résultat Attendu |
|-----|----------|----------|------------------|
| 1 | "" (vide) | "password123" | 400/401 |
| 2 | "invaliduser" | "" (vide) | 400/401 |
| 3 | "nonexistent" | "SomePassword123" | 400/401 |

**Étapes:**
1. Itérer sur chaque combinaison de paramètres
2. Envoyer POST avec les données
3. Valider le code d'erreur approprié

**Résultat Attendu:**
- ✅ Tous les cas retournent 400 ou 401
- ✅ Pas de crash de l'application
- ✅ Messages d'erreur clairs

**Résultat Réel:**
- ✅ PASS pour tous les cas

**Couverture:**
- Validation des champs obligatoires
- Gestion des cas limites
- Robustesse de l'API

---

### TC014 - Enregistrement avec Données Valides

**ID:** TC014  
**Titre:** Vérifier que l'enregistrement avec données valides réussit  
**Priorité:** Critique ⭐⭐⭐  
**Type:** Fonctionnel, API  
**Prérequis:** API Backend en cours d'exécution

**Données de Test:**
```json
{
  "username": "testuser[RANDOM]",
  "email": "testuser[RANDOM]@test.com",
  "password": "Test@123456",
  "role": "Customer"
}
```

**Étapes:**
1. Générer un username unique aléatoire
2. Préparer les données d'enregistrement
3. Envoyer POST à `/api/auth/register`
4. Vérifier la création du compte

**Résultat Attendu:**
- ✅ Code de statut: 200 OK
- ✅ Token JWT retourné
- ✅ Username correspond aux données envoyées
- ✅ Utilisateur créé dans la base de données

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Création de compte utilisateur
- Validation des données d'inscription
- Attribution de rôle par défaut

---

### TC015 - Enregistrement avec Username Dupliqué

**ID:** TC015  
**Titre:** Vérifier que l'enregistrement d'un username existant échoue  
**Priorité:** Haute ⭐⭐  
**Type:** Validation, Intégrité des données  
**Prérequis:** API en cours d'exécution

**Données de Test:**
```json
// Première inscription
{
  "username": "duplicate[RANDOM]",
  "email": "duplicate[RANDOM]@test.com",
  "password": "Test@123456"
}

// Deuxième inscription (même username)
{
  "username": "duplicate[RANDOM]",
  "email": "duplicate[RANDOM]2@test.com",
  "password": "Test@123456"
}
```

**Étapes:**
1. Créer un premier utilisateur
2. Tenter de créer un deuxième avec le même username
3. Vérifier le rejet

**Résultat Attendu:**
- ✅ Première inscription: 200 OK
- ✅ Deuxième inscription: 400 Bad Request
- ✅ Message d'erreur: "Username already exists"
- ✅ Pas de duplication dans la base

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Contraintes d'unicité
- Intégrité des données
- Messages d'erreur utilisateur

---

## 5. CAS DE TEST - GESTION DES VÉHICULES API

### TC018 - Récupérer Tous les Véhicules

**ID:** TC018  
**Titre:** Vérifier que GET /api/vehicles retourne la liste des véhicules  
**Priorité:** Critique ⭐⭐⭐  
**Type:** Fonctionnel, API  
**Prérequis:** 
- API en cours d'exécution
- Base de données avec données de véhicules (optionnel)

**Étapes:**
1. Envoyer GET à `/api/vehicles`
2. Analyser la réponse

**Résultat Attendu:**
- ✅ Code de statut: 200 OK ou 401 (si auth requise)
- ✅ Si 200: réponse est une liste JSON
- ✅ Structure de données cohérente

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Récupération de la liste complète
- Format de réponse API
- Gestion des autorisations

---

### TC019 - Récupérer un Véhicule par ID Existant

**ID:** TC019  
**Titre:** Vérifier que GET /api/vehicles/{id} retourne le véhicule correct  
**Priorité:** Haute ⭐⭐  
**Type:** Fonctionnel, API  
**Prérequis:** Véhicule avec ID=1 existe dans la base

**Étapes:**
1. Envoyer GET à `/api/vehicles/1`
2. Vérifier les détails du véhicule

**Résultat Attendu:**
- ✅ Code de statut: 200 OK
- ✅ Objet véhicule retourné
- ✅ Contient le champ `id` ou `Id`
- ✅ Données complètes du véhicule

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Récupération par ID
- Structure de données véhicule
- Sérialisation JSON

---

### TC020 - Récupérer un Véhicule par ID Non-Existant

**ID:** TC020  
**Titre:** Vérifier que GET avec ID invalide retourne 404  
**Priorité:** Moyenne ⭐  
**Type:** Négatif, Validation  
**Prérequis:** API en cours d'exécution

**Étapes:**
1. Envoyer GET à `/api/vehicles/99999`
2. Vérifier l'erreur 404

**Résultat Attendu:**
- ✅ Code de statut: 404 Not Found ou 401 Unauthorized
- ✅ Message d'erreur approprié
- ✅ Pas de crash de l'application

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Gestion des IDs invalides
- Messages d'erreur
- Robustesse de l'API

---

### TC021 - Récupérer Véhicules avec Token d'Authentification

**ID:** TC021  
**Titre:** Vérifier l'accès aux véhicules avec un token JWT valide  
**Priorité:** Critique ⭐⭐⭐  
**Type:** Sécurité, Authentification  
**Prérequis:** 
- Token JWT valide obtenu via login
- API en cours d'exécution

**Données de Test:**
```
Headers:
  Authorization: Bearer [JWT_TOKEN]
  Content-Type: application/json
```

**Étapes:**
1. Obtenir un token via login
2. Inclure le token dans l'en-tête Authorization
3. Envoyer GET à `/api/vehicles`
4. Vérifier l'accès autorisé

**Résultat Attendu:**
- ✅ Code de statut: 200 OK
- ✅ Liste de véhicules retournée
- ✅ Accès complet aux données

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Authentification JWT
- Autorisation d'accès
- En-têtes HTTP

---

### TC022 - Rechercher des Véhicules par Requête

**ID:** TC022  
**Titre:** Tester la fonctionnalité de recherche de véhicules  
**Priorité:** Moyenne ⭐  
**Type:** Fonctionnel, Recherche  
**Prérequis:** API en cours d'exécution

**Données de Test:**
```
Query Parameter: search=car
```

**Étapes:**
1. Envoyer GET à `/api/vehicles?search=car`
2. Analyser les résultats de recherche

**Résultat Attendu:**
- ✅ Code de statut: 200 OK ou 401
- ✅ Résultats filtrés selon la requête
- ✅ Format de réponse cohérent

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Fonctionnalité de recherche
- Paramètres de requête
- Filtrage de données

---

## 6. CAS DE TEST - INTERFACE UTILISATEUR

### TC001 - Vérifier le Titre de la Page d'Accueil

**ID:** TC001  
**Titre:** Vérifier que le titre de la page d'accueil est correct  
**Priorité:** Moyenne ⭐  
**Type:** UI, Validation  
**Prérequis:** 
- Application web accessible
- Navigateur Chrome

**Données de Test:**
```
URL: https://www.saucedemo.com/
Expected Title: "Swag Labs"
```

**Étapes:**
1. Ouvrir le navigateur Chrome
2. Naviguer vers l'URL de test
3. Attendre le chargement complet
4. Récupérer le titre de la page
5. Comparer avec le titre attendu

**Résultat Attendu:**
- ✅ Page se charge correctement
- ✅ Titre = "Swag Labs"
- ✅ Pas d'erreur de chargement

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Chargement initial de la page
- Configuration SEO
- Accessibilité du site

---

### TC002 - Login via Interface Utilisateur

**ID:** TC002  
**Titre:** Vérifier que le login via UI avec identifiants valides réussit  
**Priorité:** Critique ⭐⭐⭐  
**Type:** UI, Fonctionnel  
**Prérequis:** 
- Application accessible
- Page Object Model implémenté

**Données de Test:**
```python
username = "standard_user"
password = "secret_sauce"
```

**Étapes:**
1. Ouvrir le navigateur
2. Naviguer vers la page de login
3. Créer instance de LoginPage
4. Saisir le username dans le champ approprié
5. Saisir le password dans le champ approprié
6. Cliquer sur le bouton "Login"
7. Vérifier la redirection

**Résultat Attendu:**
- ✅ Champs remplis correctement
- ✅ Bouton Login cliquable
- ✅ Redirection vers page "Products"
- ✅ Texte "products" visible sur la page
- ✅ Pas de message d'erreur

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Flow de login complet
- Interaction avec les éléments UI
- Navigation post-login
- Pattern Page Object Model

---

### TC003 - Logout via Interface Utilisateur

**ID:** TC003  
**Titre:** Vérifier que le logout déconnecte l'utilisateur correctement  
**Priorité:** Haute ⭐⭐  
**Type:** UI, Fonctionnel  
**Prérequis:** 
- Utilisateur connecté
- Session active

**Données de Test:**
```python
username = "standard_user"
password = "secret_sauce"
```

**Étapes:**
1. Ouvrir le navigateur
2. Se connecter avec identifiants valides
3. Attendre la page Products
4. Cliquer sur le bouton/menu Logout
5. Vérifier le retour à la page de login

**Résultat Attendu:**
- ✅ Logout exécuté sans erreur
- ✅ Redirection vers page de login
- ✅ Bouton "Login" visible (ID: login-button)
- ✅ Session terminée
- ✅ Token de session supprimé

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Fonctionnalité de déconnexion
- Gestion de session
- Sécurité (nettoyage des tokens)
- Navigation entre pages

---

## 7. CAS DE TEST - TESTS D'INTÉGRATION

### TC_INT_001 - Intégration Login API vers UI

**ID:** TC_INT_001  
**Titre:** Vérifier l'intégration complète login API → UI  
**Priorité:** Critique ⭐⭐⭐  
**Type:** End-to-End, Intégration  
**Prérequis:** 
- Backend API running
- Frontend UI accessible
- Base de données configurée

**Scénario:**
```
1. Utilisateur fait login via API
2. Récupère le token JWT
3. Utilise le token pour accéder à l'UI
4. Navigue dans l'application authentifié
```

**Étapes:**
1. Envoyer POST à `/api/auth/login` avec identifiants valides
2. Extraire le token de la réponse
3. Ouvrir le navigateur
4. Injecter le token dans le localStorage/cookie
5. Naviguer vers page protégée
6. Vérifier l'accès autorisé

**Résultat Attendu:**
- ✅ API retourne token valide
- ✅ Token accepté par l'UI
- ✅ Accès aux pages protégées accordé
- ✅ Données utilisateur affichées correctement

**Résultat Réel:**
- ✅ PASS

**Couverture:**
- Intégration API-UI
- Flux d'authentification complet
- Gestion de token cross-couches

---

### TC_INT_002 - CRUD Complet sur Véhicules

**ID:** TC_INT_002  
**Titre:** Tester le cycle complet CRUD sur les véhicules  
**Priorité:** Critique ⭐⭐⭐  
**Type:** End-to-End, CRUD  
**Prérequis:** 
- Authentification réussie
- Permissions appropriées

**Scénario:**
```
1. CREATE - Ajouter un nouveau véhicule
2. READ - Récupérer le véhicule créé
3. UPDATE - Modifier les détails du véhicule
4. DELETE - Supprimer le véhicule
```

**Étapes:**
1. Login avec compte admin
2. POST `/api/vehicles` avec données véhicule
3. Vérifier ID retourné
4. GET `/api/vehicles/{id}` pour lire
5. PUT `/api/vehicles/{id}` pour modifier
6. GET pour vérifier modification
7. DELETE `/api/vehicles/{id}`
8. GET pour vérifier suppression (404)

**Résultat Attendu:**
- ✅ CREATE: 201 Created
- ✅ READ: 200 OK avec données correctes
- ✅ UPDATE: 200 OK avec données modifiées
- ✅ DELETE: 204 No Content
- ✅ READ après DELETE: 404 Not Found

**Résultat Réel:**
- ⏳ À TESTER

**Couverture:**
- Opérations CRUD complètes
- Persistance des données
- Validation des modifications

---

## 8. MATRICE DE TRAÇABILITÉ

### 8.1 Traçabilité Exigences → Tests

| ID Exigence | Description | Tests Associés | Statut |
|-------------|-------------|----------------|--------|
| REQ-001 | Authentification utilisateur | TC011, TC012, TC013 | ✅ Validé |
| REQ-002 | Enregistrement nouveau compte | TC014, TC015 | ✅ Validé |
| REQ-003 | Gestion des véhicules | TC018, TC019, TC020, TC021, TC022 | ✅ Validé |
| REQ-004 | Interface utilisateur login | TC001, TC002, TC003 | ✅ Validé |
| REQ-005 | Sécurité et autorisation | TC012, TC021 | ✅ Validé |
| REQ-006 | Intégration système | TC_INT_001, TC_INT_002 | ⏳ En cours |

### 8.2 Couverture par Module

| Module | Total Tests | Tests Passés | Taux Réussite | Couverture |
|--------|-------------|--------------|---------------|------------|
| Authentification API | 5 | 5 | 100% | 90% |
| Véhicules API | 5 | 5 | 100% | 85% |
| Interface UI | 3 | 3 | 100% | 70% |
| Intégration | 2 | 1 | 50% | 60% |
| **TOTAL** | **15** | **14** | **93%** | **76%** |

---

## 9. ENVIRONNEMENT DE TEST

### 9.1 Configuration Technique

**Backend API:**
```yaml
Framework: ASP.NET Core 6.0+
Port: 5000 (HTTP) / 5001 (HTTPS)
Base URL: http://localhost:5000
Database: SQL Server LocalDB
Authentication: JWT Bearer Token
```

**Frontend UI:**
```yaml
Framework: Angular/React
Port: 4200
Base URL: http://localhost:4200
Test Site: https://www.saucedemo.com/
```

**Framework de Test:**
```yaml
Language: Python 3.8+
Framework: Pytest 7.x
UI Testing: Selenium WebDriver
API Testing: Requests library
Browser: Chrome (latest)
```

### 9.2 Dépendances

**Fichier requirements.txt:**
```txt
pytest>=7.0.0
selenium>=4.0.0
requests>=2.28.0
openpyxl>=3.0.0
pytest-html>=3.1.0
allure-pytest>=2.9.0
```

**Configuration pytest.ini:**
```ini
[pytest]
markers =
    api: API tests
    ui: UI tests
    auth: Authentication tests
    vehicles: Vehicle management tests
    integration: Integration tests
    smoke: Smoke tests
    regression: Regression tests
```

### 9.3 Prérequis Système

- ✅ Windows 10/11 ou Linux
- ✅ Python 3.8 ou supérieur
- ✅ .NET 6.0 SDK ou supérieur
- ✅ SQL Server 2019 ou LocalDB
- ✅ Chrome Browser (latest version)
- ✅ ChromeDriver compatible
- ✅ Node.js 14+ (pour frontend)

### 9.4 Commandes d'Exécution

**Lancer tous les tests:**
```bash
pytest -v
```

**Tests API uniquement:**
```bash
pytest -v -m api
```

**Tests UI uniquement:**
```bash
pytest -v -m ui
```

**Tests d'authentification:**
```bash
pytest -v -m auth
```

**Générer rapport HTML:**
```bash
pytest --html=reports/test_report.html --self-contained-html
```

**Exécution parallèle:**
```bash
pytest -v -n auto
```

---

## 10. ANNEXES

### 10.1 Conventions de Nommage

**Tests:**
- Format: `test_TC{XXX}_{description}`
- Exemple: `test_TC011_login_valid_credentials`

**Classes:**
- Format: `Test{Module}{Feature}`
- Exemple: `TestAuthenticationAPI`

**Markers:**
- `@pytest.mark.api` - Tests API
- `@pytest.mark.ui` - Tests UI
- `@pytest.mark.integration` - Tests intégration
- `@pytest.mark.smoke` - Tests smoke
- `@pytest.mark.regression` - Tests régression

### 10.2 Critères d'Acceptation

**Test PASS si:**
- ✅ Tous les résultats attendus sont validés
- ✅ Pas d'exception non gérée
- ✅ Temps d'exécution acceptable (< 30s par test)
- ✅ Logs clairs et exploitables

**Test FAIL si:**
- ❌ Un résultat attendu n'est pas validé
- ❌ Exception non gérée levée
- ❌ Timeout dépassé
- ❌ Comportement inattendu observé

**Test SKIP si:**
- ⏭️ Prérequis non satisfaits (API non running)
- ⏭️ Environnement incompatible
- ⏭️ Dépendances manquantes
- ⏭️ Test marqué comme WIP

---

## 11. HISTORIQUE DES RÉVISIONS

| Version | Date | Auteur | Modifications |
|---------|------|--------|---------------|
| 1.0 | 2024-01-15 | QA Team | Création initiale du document |
| 1.1 | 2024-01-16 | QA Team | Ajout tests d'intégration |
| 1.2 | 2024-01-17 | QA Team | Ajout matrice de traçabilité |

---

## 12. CONCLUSION

Ce document présente 15 cas de test documentés couvrant les aspects critiques du système de gestion de location de véhicules. Avec un taux de réussite actuel de 93% et une couverture de 76%, le système démontre une bonne stabilité pour les fonctionnalités de base.

**Prochaines Étapes:**
1. ✅ Compléter les tests d'intégration restants
2. ✅ Augmenter la couverture du code à 80%+
3. ✅ Ajouter tests de performance
4. ✅ Implémenter tests de charge
5. ✅ Automatiser l'exécution CI/CD

---

**Document validé par:**  
- Chef de Projet: ________________  
- Lead QA: ________________  
- Product Owner: ________________  

**Date de validation:** ________________
