# ?? Guide Rapide - Analyse Statique avec SonarQube

## ?? Ce que vous devez faire

En tant que **groupe de 2 personnes**, vous devez réaliser **3 activités d'analyse statique**:

### ? Activité 1: Revue de Code Manuelle (3 heures)
**Ce qui est fait**: Le rapport contient déjà tous les résultats
**Votre tâche**: Lire et comprendre les problèmes identifiés

### ? Activité 2: Analyse Automatisée (2 heures)
**Ce qui est fait**: Analyse Roslyn effectuée (12 warnings documentés)
**Votre tâche**: Vérifier les corrections dans le rapport

### ? Activité 3: SonarQube (3 heures)
**À FAIRE**: Installer et exécuter SonarQube (voir ci-dessous)

---

## ?? Installation SonarQube (15 minutes)

### Option 1: Docker (RECOMMANDÉ)

```powershell
# 1. Télécharger l'image
docker pull sonarqube:community

# 2. Démarrer SonarQube
docker run -d --name sonarqube -p 9000:9000 sonarqube:community

# 3. Attendre le démarrage (2-3 minutes)
Write-Host "Attente du démarrage de SonarQube..."
Start-Sleep -Seconds 120

# 4. Ouvrir dans le navigateur
Start-Process "http://localhost:9000"

# Login par défaut:
# Username: admin
# Password: admin
# (vous devrez changer le mot de passe au premier login)
```

### Option 2: Installation Locale

1. Télécharger SonarQube Community: https://www.sonarqube.org/downloads/
2. Extraire le ZIP
3. Exécuter `bin/windows-x86-64/StartSonar.bat`
4. Ouvrir http://localhost:9000

---

## ?? Configuration SonarQube (10 minutes)

### 1. Créer un Projet

```
1. Login ? "Create Project Manually"
2. Project key: car-rental-system
3. Display name: Car Rental System
4. Branch: main
5. Cliquer "Set Up"
```

### 2. Générer un Token

```
1. "With Jenkins" ? "Other CI"
2. Generate Token
3. Nom du token: car-rental-analysis
4. COPIER LE TOKEN (vous ne pourrez plus le voir!)
```

### 3. Choisir la Technologie

```
1. Sélectionner ".NET"
2. Sélectionner "Windows"
```

---

## ?? Exécution de l'Analyse (10 minutes)

### Installer le Scanner

```powershell
# Dans le dossier du projet
dotnet tool install --global dotnet-sonarscanner
```

### Lancer l'Analyse

```powershell
# REMPLACER <YOUR_TOKEN> par le token généré précédemment

# 1. Démarrer l'analyse
dotnet sonarscanner begin /k:"car-rental-system" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="<YOUR_TOKEN>"

# 2. Build le projet
dotnet build

# 3. Terminer l'analyse
dotnet sonarscanner end /d:sonar.login="<YOUR_TOKEN>"
```

**Durée**: ~2-3 minutes

---

## ?? Captures d'Écran à Prendre

Après l'analyse, prendre des captures d'écran de:

### 1. Overview Dashboard
- Score global (A/B/C/D/E)
- Nombre de bugs
- Nombre de vulnérabilités
- Code smells
- Coverage

### 2. Issues
- Liste des problèmes détectés
- Filtrer par sévérité

### 3. Measures
- Complexité
- Duplication
- Lines of code

### Où prendre les captures:
```
http://localhost:9000/dashboard?id=car-rental-system
```

---

## ?? Compléter le Rapport

### 1. Ajouter les Captures d'Écran

Dans le document Word, section "Annexe C":
```
1. Insérer ? Image
2. Ajouter vos 3 captures d'écran
3. Ajouter des légendes
```

### 2. Remplir les Métriques SonarQube

Dans le rapport, page 13, compléter:

```markdown
### Vue d'Ensemble du Projet

| Métrique | Valeur | Seuil | Statut |
|----------|--------|-------|--------|
| **Bugs** | [VOTRE VALEUR] | 0 | ?/? |
| **Vulnérabilités** | [VOTRE VALEUR] | 0 | ?/? |
| **Code Smells** | [VOTRE VALEUR] | < 50 | ?/? |
| **Dette Technique** | [VOTRE VALEUR] | < 10h | ?/? |
| **Couverture de Tests** | [VOTRE VALEUR] | > 70% | ?/? |
| **Duplication** | [VOTRE VALEUR] | < 3% | ?/? |
```

### 3. Ajouter Vos Noms

Page 1:
```markdown
| **Réviseurs** | Membre 1: [VOTRE NOM]<br>Membre 2: [NOM DU PARTENAIRE] |
```

Page 20:
```markdown
**Membre 1**: [VOTRE NOM]
*Date*: [DATE DU JOUR]

**Membre 2**: [NOM DU PARTENAIRE]
*Date*: [DATE DU JOUR]
```

---

## ?? Timeline Recommandée

| Tâche | Durée | Responsable |
|-------|-------|-------------|
| Installation SonarQube | 15 min | Membre 1 |
| Configuration projet | 10 min | Membre 1 |
| Exécution analyse | 10 min | Membre 2 |
| Captures d'écran | 5 min | Membre 2 |
| Lecture du rapport | 30 min | Les 2 |
| Complétion des données | 15 min | Membre 1 |
| Conversion en Word | 5 min | Membre 2 |
| Relecture finale | 20 min | Les 2 |
| **TOTAL** | **1h 50min** | - |

---

## ?? Checklist Finale

### Avant la Remise

- [ ] SonarQube installé et exécuté
- [ ] Captures d'écran prises
- [ ] Métriques SonarQube remplies dans le rapport
- [ ] Noms des 2 membres ajoutés
- [ ] Dates complétées
- [ ] Document converti en .docx
- [ ] Rapport relu par les 2 membres
- [ ] Captures d'écran insérées dans le document
- [ ] Orthographe vérifiée
- [ ] Signatures ajoutées (si impression)

### Documents à Remettre

1. ? `RAPPORT_ANALYSE_STATIQUE.docx` (document principal)
2. ? Captures d'écran SonarQube (dans le document)
3. ? Code source du projet (si demandé)

---

## ?? Résultats Attendus

D'après le projet actuel, vous devriez obtenir:

### Métriques Attendues

| Métrique | Valeur Attendue | Commentaire |
|----------|-----------------|-------------|
| **Bugs** | 0 | Excellent |
| **Vulnérabilités** | 0 | Excellent |
| **Code Smells** | 40-50 | Acceptable |
| **Dette Technique** | 6-8h | Bon |
| **Couverture** | 70-75% | Bon |
| **Duplication** | 2-3% | Excellent |
| **Complexité** | 3-5 (moy) | Bon |

### Score Global

**Attendu**: A ou B (Excellent ou Très Bon)

Si vous obtenez C ou moins:
- Vérifier que le build a réussi
- Relancer l'analyse
- Vérifier les exclusions (Migrations/, etc.)

---

## ?? Problèmes Courants

### SonarQube ne démarre pas

**Solution**:
```powershell
# Vérifier que le port 9000 est libre
netstat -ano | findstr :9000

# Si occupé, tuer le processus
taskkill /PID <PID> /F
```

### Erreur "dotnet-sonarscanner not found"

**Solution**:
```powershell
# Réinstaller
dotnet tool uninstall --global dotnet-sonarscanner
dotnet tool install --global dotnet-sonarscanner

# Redémarrer PowerShell
```

### Erreur d'authentification

**Solution**:
- Vérifier que le token est correct
- Régénérer un nouveau token si nécessaire
- S'assurer qu'il n'y a pas d'espaces avant/après le token

### L'analyse ne remonte aucun fichier

**Solution**:
```powershell
# Vérifier que vous êtes à la racine du projet
Get-Location  # Doit être: .../yosrtaktak/.net_projects

# Vérifier que le build réussit
dotnet build  # Doit afficher "Build succeeded"
```

---

## ?? Conseils

### Pour une Meilleure Note

1. **Comprendre les résultats** - Ne pas juste copier-coller
2. **Expliquer les corrections** - Dire pourquoi c'était un problème
3. **Justifier les choix** - Pourquoi certains warnings sont acceptés
4. **Ajouter des commentaires** - Annoter les captures d'écran
5. **Être précis** - Utiliser les vrais chiffres de votre analyse

### Ce qui Impressionne

- ? Screenshots annotés
- ? Comprendre la dette technique
- ? Expliquer les patterns utilisés
- ? Justifier les scores obtenus
- ? Proposer des améliorations

### Ce qu'il faut Éviter

- ? Laisser des [À compléter]
- ? Inventer des chiffres
- ? Copier-coller sans comprendre
- ? Oublier les captures d'écran
- ? Ne pas signer le document

---

## ?? Ressources Utiles

### Documentation

- **SonarQube**: https://docs.sonarqube.org/
- **Roslyn Analyzers**: https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/
- **C# Coding Conventions**: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/

### Vidéos Tutoriels

- SonarQube Setup: https://www.youtube.com/results?search_query=sonarqube+dotnet+tutorial
- Static Analysis: https://www.youtube.com/results?search_query=static+code+analysis+c%23

---

## ? Validation

### Tester Votre Compréhension

Pouvez-vous expliquer:

1. Qu'est-ce qu'une analyse statique?
2. Quelle est la différence entre un bug et un code smell?
3. Pourquoi la complexité cyclomatique est importante?
4. Comment réduire la dette technique?
5. Qu'est-ce qu'une vulnérabilité de sécurité?

**Si oui**: Vous êtes prêt(e) pour la remise! ??  
**Si non**: Relire le rapport pages 5-15

---

## ?? Pour Aller Plus Loin

Si vous avez du temps supplémentaire:

1. **Corriger les Code Smells restants**
   - Identifier les problèmes dans SonarQube
   - Appliquer les corrections suggérées
   - Relancer l'analyse

2. **Améliorer la Couverture de Tests**
   - Ajouter des tests unitaires
   - Viser 80%+ de couverture

3. **Optimiser les Performances**
   - Implémenter la pagination
   - Ajouter du cache

---

**Dernière mise à jour**: Décembre 2024  
**Version**: 1.0  
**Temps estimé**: 2 heures pour tout compléter

---

?? **Bonne chance avec votre analyse statique!** ??
