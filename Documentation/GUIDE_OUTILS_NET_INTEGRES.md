# ?? GUIDE RAPIDE - Analyse Statique avec Outils .NET Intégrés

## ? Outils Déjà Installés avec .NET

Pas besoin d'installer quoi que ce soit! Visual Studio inclut:
- ? **Roslyn Analyzers** - Analyse de code en temps réel
- ? **Code Analysis** - Analyse lors du build
- ? **Error List** - Affichage des warnings et erreurs
- ? **Code Metrics** - Complexité et maintenabilité

---

## ?? 3 ÉTAPES SIMPLES (30 minutes)

### ÉTAPE 1: Compiler avec Analyse Complète (5 minutes)

#### Dans Visual Studio

1. **Ouvrir** la solution dans Visual Studio
2. **Menu Build** ? **Rebuild Solution** (ou Ctrl+Shift+B)
3. Observer la fenêtre **Output** pour voir les warnings

#### En Ligne de Commande

```bash
# Dans le dossier racine du projet
dotnet build --configuration Release /p:TreatWarningsAsErrors=false
```

**Résultat attendu**: Liste des warnings dans Output/Error List

---

### ÉTAPE 2: Consulter les Résultats (10 minutes)

#### A. Voir la Liste des Warnings

1. **View** ? **Error List** (ou Ctrl+\, E)
2. Onglet **Warnings**
3. Vous verrez tous les problèmes détectés:
   - CS warnings (C# compiler)
   - CA warnings (Code Analysis)
   - IDE suggestions

#### B. Analyser le Code (Built-in)

Menu **Analyze** ? **Run Code Analysis** ? **On Solution**

Cela ajoute des warnings supplémentaires de type CA (Code Analysis)

#### C. Voir les Métriques de Code

1. **Analyze** ? **Calculate Code Metrics** ? **For Solution**
2. Une fenêtre s'ouvre avec:
   - **Maintainability Index** (0-100)
   - **Cyclomatic Complexity**
   - **Depth of Inheritance**
   - **Lines of Code**

---

### ÉTAPE 3: Capturer & Documenter (15 minutes)

#### A. Prendre 3 Captures d'Écran

**Capture 1**: Error List avec tous les warnings
- View ? Error List ? Onglet Warnings
- Montrer CS, CA, IDE warnings

**Capture 2**: Code Metrics
- Analyze ? Calculate Code Metrics ? For Solution
- Capture du tableau avec les métriques

**Capture 3**: Exemple de Warning dans le Code
- Double-cliquer sur un warning
- Le code s'ouvre avec le problème surligné
- Capture avec la description

#### B. Exporter les Résultats

**Error List ? Right-click ? Copy**
```
Code    Description                                 Project     File                        Line
CS0108  'CategoryRepository._context' hides...     Backend     CategoryRepository.cs       10
CS8602  Dereference of a possibly null reference   Backend     ReportService.cs            85
CS1998  This async method lacks 'await'...         Frontend    Rentals.razor               221
...
```

**Code Metrics ? Right-click ? Copy All**
```
Assembly: Backend
    Maintainability Index: 78
    Cyclomatic Complexity: 450
    Depth of Inheritance: 3
    Class Coupling: 85
    Lines of Source Code: 6408
    Lines of Executable Code: 2156
```

---

## ?? REMPLIR LE RAPPORT (15 minutes)

### Ouvrir le Rapport

```
Documentation\RAPPORT_ANALYSE_STATIQUE.md
```

### Section: Analyse Roslyn (Page 6)

```markdown
#### Commande Exécutée
```bash
dotnet build --configuration Release
```

**Résultats**:
- ?? [VOTRE NOMBRE] warnings détectés
- ? 0 erreurs
- ?? Temps de build: [X.XX] secondes
```

### Section: Warnings par Type (Page 8)

```markdown
| Type | Nombre | Description |
|------|--------|-------------|
| **CS** | [X] | C# Compiler warnings |
| **CA** | [X] | Code Analysis warnings |
| **IDE** | [X] | IDE suggestions |
| **MUD** | [X] | MudBlazor warnings |
```

### Section: Code Metrics (Page 13)

```markdown
| Métrique | Backend | Frontend | Évaluation |
|----------|---------|----------|------------|
| **Maintainability Index** | [X] | [X] | ? BON (>70) |
| **Cyclomatic Complexity** | [X] | [X] | ? BON (<10) |
| **Lines of Code** | 6,408 | 13,405 | - |
| **Class Coupling** | [X] | [X] | - |
```

---

## ?? UTILISER CODE ANALYSIS (Build-in)

### Activer Code Analysis

Dans le fichier `.csproj`:

```xml
<PropertyGroup>
    <AnalysisLevel>latest</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
</PropertyGroup>
```

**C'est déjà configuré dans votre projet!**

### Règles Actives

Les analyseurs .NET détectent:
- ?? **Sécurité**: Null reference, SQL injection
- ?? **Qualité**: Code smells, complexité
- ? **Performance**: Allocations inutiles, async/await
- ?? **Style**: Conventions de nommage, formatage

---

## ?? CAPTURES D'ÉCRAN À PRENDRE

### Capture 1: Error List Complet

```
??????????????????????????????????????????????????????????????
? Error List - Onglet Warnings                               ?
??????????????????????????????????????????????????????????????
? Code    Description                          File      Line?
? CS0108  'CategoryRepository._context'...     Category   10 ?
? CS8602  Dereference of possibly null...      Report     85 ?
? CS1998  Async method lacks 'await'...        Rentals   221 ?
? MUD0002 Illegal Attribute 'Title'...         Mainten   145 ?
? IDE0005 Using directive unnecessary...       Auth       12 ?
? CA1062  Validate parameter is null...        Vehicle   234 ?
??????????????????????????????????????????????????????????????
```

### Capture 2: Code Metrics

```
??????????????????????????????????????????????????????????????
? Code Metrics Results                                       ?
??????????????????????????????????????????????????????????????
? Project: Backend                                           ?
?   Maintainability Index: 78                                ?
?   Cyclomatic Complexity: 450                               ?
?   Depth of Inheritance: 3                                  ?
?   Class Coupling: 85                                       ?
?   Lines of Code: 6,408                                     ?
?   Lines of Executable Code: 2,156                          ?
?                                                            ?
? Project: Frontend                                          ?
?   Maintainability Index: 82                                ?
?   Cyclomatic Complexity: 340                               ?
?   ...                                                      ?
??????????????????????????????????????????????????????????????
```

### Capture 3: Warning Détaillé

```
??????????????????????????????????????????????????????????????
? ReportService.cs                                           ?
??????????????????????????????????????????????????????????????
? 83  public async Task<Report> GetReport(int id)           ?
? 84  {                                                      ?
? 85      var report = await _context.Reports.FindAsync(id);?
? 86      ~~~~~~~~~~~~~~~~~~~~~~~~~~~~                       ?
? 86      ?? CS8602: Dereference of possibly null reference ?
? 87      return report.GeneratePDF();                      ?
? 88  }                                                      ?
?                                                            ?
? ?? Add null check: if (report == null) return null;       ?
??????????????????????????????????????????????????????????????
```

---

## ?? WARNINGS RÉELS DÉJÀ TROUVÉS

### CS Warnings (Compiler)

| Code | Nombre | Description | Fichiers |
|------|--------|-------------|----------|
| **CS0108** | 3 | Member hides inherited member | CategoryRepository.cs, MaintenanceRepository.cs, VehicleDamageRepository.cs |
| **CS8602** | 1 | Possible null reference | ReportService.cs:85 |
| **CS1998** | 1 | Async without await | Rentals.razor:221 |

### MUD Warnings (MudBlazor)

| Code | Nombre | Description | Fichiers |
|------|--------|-------------|----------|
| **MUD0002** | 8 | Illegal Attribute case | Maintenances.razor (4x), VehicleDamages.razor (4x) |

### Total: 12 Warnings Documentés

---

## ?? ANALYSER PAR CATÉGORIE

### Filtrer dans Error List

1. **Par Sévérité**: Cliquer colonne "!" (Error/Warning/Message)
2. **Par Type**: Filtrer par "Code" (CS, CA, IDE, MUD)
3. **Par Projet**: Filtrer par "Project" (Backend/Frontend)

### Grouper les Résultats

Error List ? Right-click ? **Group By**:
- **Category**: Compiler Error, Compiler Warning, Message
- **Project**: Backend, Frontend, Backend.Tests
- **File**: Par fichier source

---

## ?? CALCULER LES MÉTRIQUES MANUELLEMENT

### Complexité Cyclomatique Moyenne

Dans Code Metrics, noter pour chaque projet:
```
Controllers moyenne: [X]
Services moyenne: [X]
Repositories moyenne: [X]
```

**Interprétation**:
- 1-5: ? Simple
- 6-10: ?? Modéré
- 11-20: ?? Complexe
- 21+: ?? Très complexe

### Maintainability Index

```
Backend: [X]/100
Frontend: [X]/100
```

**Interprétation**:
- 80-100: ? Excellent
- 60-79: ?? Bon
- 40-59: ?? Moyen
- 0-39: ?? Difficile à maintenir

---

## ?? COMPLÉTER LE RAPPORT

### Page 6: Méthodologie - Activité 2

```markdown
### 2?? Analyse Automatisée .NET (Activité 2)

**Participants**: 2 membres  
**Durée**: 2 heures  
**Outils**: Roslyn Analyzers, .NET Code Analysis

#### Outils et Configuration

**Analyseurs Activés**:
```xml
<PropertyGroup>
    <AnalysisLevel>latest</AnalysisLevel>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
</PropertyGroup>
```

**Commandes Exécutées**:
```bash
# Build avec analyse complète
dotnet build --configuration Release

# Analyse des métriques
Analyze ? Calculate Code Metrics ? For Solution
```

**Résultats de la Compilation**:
- ?? **[VOTRE NOMBRE]** warnings détectés
- ? **0** erreurs
- ?? Temps de build: [X.XX] secondes
```

### Page 8-11: Problèmes Détectés

Les problèmes sont **déjà documentés** dans le rapport:
- CS0108 (3 occurrences)
- CS8602 (1 occurrence)
- CS1998 (1 occurrence)
- MUD0002 (8 occurrences)

**Ajoutez vos nouvelles trouvailles** si vous en avez!

### Page 13: Métriques

```markdown
### Code Metrics Results

#### Backend
| Métrique | Valeur | Évaluation |
|----------|--------|------------|
| **Maintainability Index** | [X] | ?/??/??/?? |
| **Cyclomatic Complexity** | [X] | ? Bon (< 10) |
| **Depth of Inheritance** | [X] | ? Bon (< 5) |
| **Class Coupling** | [X] | - |
| **Lines of Code** | 6,408 | - |

#### Frontend
| Métrique | Valeur | Évaluation |
|----------|--------|------------|
| **Maintainability Index** | [X] | ?/??/??/?? |
| **Cyclomatic Complexity** | [X] | ? Bon (< 10) |
| **Lines of Code** | 13,405 | - |
```

---

## ? CHECKLIST COMPLÈTE

### Analyse avec Outils .NET
- [ ] Build Solution exécuté (Ctrl+Shift+B)
- [ ] Error List consulté (View ? Error List)
- [ ] Code Analysis exécuté (Analyze ? Run Code Analysis)
- [ ] Code Metrics calculé (Analyze ? Calculate Code Metrics)
- [ ] Nombre de warnings noté
- [ ] Warnings groupés par type (CS, CA, IDE, MUD)

### Captures d'Écran
- [ ] Capture 1: Error List avec tous les warnings
- [ ] Capture 2: Code Metrics Results
- [ ] Capture 3: Exemple de warning avec code

### Rapport
- [ ] Métriques Code Metrics remplies (page 13)
- [ ] Noms des 2 membres complétés (page 1)
- [ ] Dates complétées
- [ ] Nouveaux warnings documentés (si trouvés)

### Document Final
- [ ] Converti en .docx (Ouvrir dans Word ? Enregistrer)
- [ ] 3 captures insérées (Annexe C)
- [ ] Relu et vérifié

---

## ?? TEMPS TOTAL: 30 MINUTES

| Étape | Durée |
|-------|-------|
| Build + Code Analysis | 5 min |
| Code Metrics | 5 min |
| Captures d'écran | 5 min |
| Compter/grouper warnings | 5 min |
| Remplir rapport | 15 min |
| Convertir Word + images | 5 min |
| **TOTAL** | **40 min** |

---

## ?? ASTUCES VISUAL STUDIO

### Voir Tous les Warnings

```
View ? Error List
? Cliquer sur onglet "Warnings"
? Supprimer filtres éventuels
```

### Exporter Error List

```
Error List ? Ctrl+A (tout sélectionner)
? Ctrl+C (copier)
? Coller dans Excel ou Notepad
```

### Quick Actions

Sur un warning:
- **Ctrl+.** (point) ou cliquer l'ampoule ??
- Voir suggestions de correction
- Appliquer la correction automatique

---

## ?? RÉSULTATS ATTENDUS

### Warnings Typiques

D'après l'analyse déjà effectuée:

| Type | Nombre Attendu | Commentaire |
|------|----------------|-------------|
| **CS (Compiler)** | 10-15 | Normal |
| **CA (Analysis)** | 5-10 | Suggestions qualité |
| **IDE** | 10-20 | Style et conventions |
| **MUD** | 8 | MudBlazor specifique |
| **TOTAL** | **30-50** | Acceptable |

### Métriques Attendues

| Métrique | Attendu | Commentaire |
|----------|---------|-------------|
| **Maintainability Index** | 75-85 | Bon |
| **Cyclomatic Complexity** | 3-5 (avg) | Excellent |
| **Depth of Inheritance** | 2-4 | Normal |

---

## ?? TYPES DE WARNINGS .NET

### CS (C# Compiler)

- **CS0108**: Member hides inherited member
- **CS8602**: Possible null reference
- **CS1998**: Async without await
- **CS0649**: Field never assigned

### CA (Code Analysis)

- **CA1062**: Validate arguments of public methods
- **CA1031**: Do not catch general exception types
- **CA1707**: Identifiers should not contain underscores
- **CA2007**: Do not directly await a Task

### IDE (Visual Studio)

- **IDE0005**: Using directive is unnecessary
- **IDE0011**: Add braces
- **IDE0058**: Expression value is never used
- **IDE1006**: Naming rule violation

---

## ?? DÉPANNAGE

### "Je ne vois pas Code Metrics"

**Solution**:
- Menu Analyze ? Calculate Code Metrics
- Si option manquante: Visual Studio Professional/Enterprise requis
- Alternative: Utiliser seulement Error List (suffisant)

### "Trop de warnings (> 100)"

**Normaliser**:
- C'est normal pour un gros projet
- Filtrer par Project (Backend, Frontend)
- Se concentrer sur CS (compiler) et CA (analysis)
- Ignorer certains IDE (style seulement)

### "Code Metrics échoue"

**Solution**:
- Build doit réussir d'abord
- Clean Solution ? Rebuild
- Calculer par projet plutôt que solution

---

## ? PRÊT!

**Prochaines actions**:

1. ? Build Solution (Ctrl+Shift+B)
2. ? View Error List (Ctrl+\, E)
3. ? Analyze ? Calculate Code Metrics
4. ? Prendre 3 captures
5. ? Remplir RAPPORT_ANALYSE_STATIQUE.md
6. ? Convertir en Word

---

**Créé**: Décembre 2024  
**Version**: Outils .NET Intégrés  
**Temps**: 30-40 minutes  
**Difficulté**: ? Très facile (tout est intégré!)

---

?? **Commencez maintenant! Ouvrez Visual Studio et faites Rebuild Solution!** ??
