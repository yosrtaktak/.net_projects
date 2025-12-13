# Matrice de Traçabilité - Tests

## 1. Vue d'ensemble

Ce document établit la traçabilité entre:
- **Exigences fonctionnelles** ? **Scénarios de test** ? **Cas de test** ? **Résultats**

## 2. Exigences Fonctionnelles

| ID Exigence | Description | Priorité | Statut |
|-------------|-------------|----------|---------|
| REQ-001 | L'utilisateur peut s'inscrire avec email/mot de passe | HAUTE | ? Implémenté |
| REQ-002 | L'utilisateur peut se connecter | HAUTE | ? Implémenté |
| REQ-003 | L'utilisateur peut consulter les véhicules disponibles | HAUTE | ? Implémenté |
| REQ-004 | L'utilisateur peut rechercher des véhicules | MOYENNE | ? Implémenté |
| REQ-005 | L'utilisateur peut réserver un véhicule | HAUTE | ? Implémenté |
| REQ-006 | Le système calcule le prix de location | HAUTE | ? Implémenté |
| REQ-007 | L'admin peut gérer les véhicules (CRUD) | HAUTE | ? Implémenté |
| REQ-008 | L'admin peut voir les statistiques | MOYENNE | ? Implémenté |
| REQ-009 | Le système valide les données d'entrée | HAUTE | ? Implémenté |
| REQ-010 | Le système protège les ressources par authentification | HAUTE | ? Implémenté |

## 3. Matrice de Traçabilité Complète

### 3.1 Authentification (REQ-001, REQ-002, REQ-010)

| ID Test | Niveau | Type | Technique | Exigence | Scénario | Statut | Résultat |
|---------|--------|------|-----------|----------|----------|--------|----------|
| TC011 | Intégration | Fonctionnel | Scénario nominal | REQ-002 | Connexion valide retourne token | ? Implémenté | ? À exécuter |
| TC012 | Intégration | Fonctionnel | Cas d'erreur | REQ-002 | MDP incorrect retourne 401 | ? Implémenté | ? À exécuter |
| TC013 | Intégration | Fonctionnel | Classes équivalence | REQ-009 | Données invalides ? BadRequest | ? Implémenté | ? À exécuter |
| TC014 | Intégration | Fonctionnel | Création | REQ-001 | Inscription nouvel utilisateur | ? Implémenté | ? À exécuter |
| TC015 | Intégration | Fonctionnel | Contrainte unicité | REQ-001 | Email dupliqué ? Conflit | ? Implémenté | ? À exécuter |
| TC023 | Système | Fonctionnel | Scénario utilisateur | REQ-002 | Login UI avec credentials valides | ? Implémenté | ? À exécuter |
| TC024 | Système | Fonctionnel | Cas d'erreur UI | REQ-002 | Login UI MDP incorrect | ? Implémenté | ? À exécuter |
| TC025 | Système | Fonctionnel | Validation UI | REQ-009 | Champs vides ? Erreur | ? Implémenté | ? À exécuter |
| TC026 | Système | Fonctionnel | Classes équivalence | REQ-009 | Emails invalides ? Erreur | ? Implémenté | ? À exécuter |
| TC027 | Système | Sécurité | Test sécurité | REQ-010 | MDP masqué dans DOM | ? Implémenté | ? À exécuter |

### 3.2 Gestion des Véhicules (REQ-003, REQ-004, REQ-007)

| ID Test | Niveau | Type | Technique | Exigence | Scénario | Statut | Résultat |
|---------|--------|------|-----------|----------|----------|--------|----------|
| TC006 | Unitaire | Fonctionnel | Chemin nominal | REQ-003 | GetAll retourne liste véhicules | ? Implémenté | ? À exécuter |
| TC007 | Unitaire | Fonctionnel | Valeur valide | REQ-003 | GetById retourne véhicule existant | ? Implémenté | ? À exécuter |
| TC008 | Unitaire | Fonctionnel | Valeur invalide | REQ-003 | GetById ID inexistant ? NotFound | ? Implémenté | ? À exécuter |
| TC009 | Unitaire | Fonctionnel | Valeurs limites | REQ-009 | IDs négatifs ? NotFound | ? Implémenté | ? À exécuter |
| TC010 | Unitaire | Fonctionnel | Filtrage | REQ-004 | Filtrer véhicules disponibles | ? Implémenté | ? À exécuter |
| TC016 | Intégration | Sécurité | Contrôle accès | REQ-010 | API sans auth ? 401 | ? Implémenté | ? À exécuter |
| TC017 | Intégration | Fonctionnel | API avec auth | REQ-003 | GET /vehicles avec token | ? Implémenté | ? À exécuter |
| TC018 | Intégration | Fonctionnel | API recherche | REQ-003 | GET /vehicles/{id} existant | ? Implémenté | ? À exécuter |
| TC019 | Intégration | Fonctionnel | API erreur | REQ-003 | GET /vehicles/{id} inexistant | ? Implémenté | ? À exécuter |
| TC020 | Intégration | Fonctionnel | API filtrage | REQ-004 | GET /vehicles/available | ? Implémenté | ? À exécuter |
| TC021 | Intégration | Fonctionnel | API création | REQ-007 | POST /vehicles données valides | ? Implémenté | ? À exécuter |
| TC022 | Intégration | Fonctionnel | API validation | REQ-009 | POST /vehicles données invalides | ? Implémenté | ? À exécuter |
| TC028 | Système | Fonctionnel | UI consultation | REQ-003 | Affichage liste véhicules | ? Implémenté | ? À exécuter |
| TC029 | Système | Fonctionnel | UI recherche | REQ-004 | Recherche avec terme valide | ? Implémenté | ? À exécuter |
| TC030 | Système | Fonctionnel | UI recherche vide | REQ-004 | Recherche sans résultat | ? Implémenté | ? À exécuter |
| TC031 | Système | Fonctionnel | UI navigation | REQ-003 | Navigation vers détails | ? Implémenté | ? À exécuter |

### 3.3 Location (REQ-005, REQ-006)

| ID Test | Niveau | Type | Technique | Exigence | Scénario | Statut | Résultat |
|---------|--------|------|-----------|----------|----------|--------|----------|
| TC001 | Unitaire | Fonctionnel | Valeurs limites | REQ-006 | Calcul prix dates valides | ? Implémenté | ? À exécuter |
| TC002 | Unitaire | Fonctionnel | Validation | REQ-009 | Date fin < début ? Exception | ? Implémenté | ? À exécuter |
| TC003 | Unitaire | Fonctionnel | Classe équivalence | REQ-009 | Tarif négatif ? Exception | ? Implémenté | ? À exécuter |
| TC004 | Unitaire | Fonctionnel | Classes équivalence | REQ-006 | Différentes durées ? Prix correct | ? Implémenté | ? À exécuter |
| TC005 | Unitaire | Fonctionnel | Chemin nominal | REQ-005 | Création location valide | ? Implémenté | ? À exécuter |

### 3.4 Tests Non-Fonctionnels

| ID Test | Niveau | Type | Exigence | Métrique | Cible | Statut | Résultat |
|---------|--------|------|----------|----------|-------|--------|----------|
| TC032 | Système | Performance | NFR-001 | Temps chargement | < 5s | ? Implémenté | ? À exécuter |
| TC033 | Système | Ergonomie | NFR-002 | Responsive design | 3 résolutions | ? Implémenté | ? À exécuter |
| TC027 | Système | Sécurité | NFR-003 | Protection MDP | Type=password | ? Implémenté | ? À exécuter |
| TC016 | Intégration | Sécurité | NFR-003 | Authentification API | 401 sans token | ? Implémenté | ? À exécuter |

## 4. Couverture des Exigences

| Exigence | Nombre de Tests | Tests Unitaires | Tests Intégration | Tests Système | Couverture |
|----------|----------------|-----------------|-------------------|---------------|------------|
| REQ-001 | 2 | 0 | 2 | 0 | ? 100% |
| REQ-002 | 6 | 0 | 3 | 3 | ? 100% |
| REQ-003 | 10 | 3 | 4 | 3 | ? 100% |
| REQ-004 | 4 | 1 | 1 | 2 | ? 100% |
| REQ-005 | 1 | 1 | 0 | 0 | ?? 50% |
| REQ-006 | 4 | 4 | 0 | 0 | ? 100% |
| REQ-007 | 2 | 0 | 2 | 0 | ?? 75% |
| REQ-008 | 0 | 0 | 0 | 0 | ? 0% |
| REQ-009 | 7 | 2 | 3 | 2 | ? 100% |
| REQ-010 | 4 | 0 | 2 | 2 | ? 100% |

**Couverture globale**: 33 tests couvrant 10 exigences = **82.5%**

## 5. Recommandations

### 5.1 Tests à ajouter
1. **REQ-005** (Réservation):
   - Tests système complets du processus de réservation UI
   - Tests d'intégration du workflow complet

2. **REQ-007** (Admin CRUD):
   - Tests système pour les opérations admin UI
   - Tests de mise à jour et suppression

3. **REQ-008** (Statistiques):
   - Tests unitaires des calculs statistiques
   - Tests d'intégration de l'API reports
   - Tests système de l'affichage des rapports

### 5.2 Tests de régression
Après chaque modification, ré-exécuter:
- Tous les tests de la fonctionnalité modifiée
- Tests de non-régression (tests critiques)

### 5.3 Tests de confirmation
Après correction d'un bug:
1. Créer un test reproduisant le bug
2. Vérifier que le test échoue (avant correction)
3. Appliquer la correction
4. Vérifier que le test passe

## 6. Légende

| Symbole | Signification |
|---------|---------------|
| ? | Complété / Passé |
| ? | En attente / À exécuter |
| ?? | Incomplet / Partiellement couvert |
| ? | Non couvert / Échoué |

## 7. Historique des Versions

| Version | Date | Auteur | Modifications |
|---------|------|--------|---------------|
| 1.0 | Déc 2024 | Équipe Test | Création initiale de la matrice |

---

**Dernière mise à jour**: Décembre 2024  
**Prochaine révision**: Après exécution complète des tests
