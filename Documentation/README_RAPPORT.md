# ?? Rapport d'Analyse Statique - Guide de Génération

## ?? Fichiers Disponibles

| Fichier | Description | Format |
|---------|-------------|--------|
| `RAPPORT_ANALYSE_STATIQUE.md` | Rapport complet en Markdown | Markdown |
| `Convert-ToWord.ps1` | Conversion avec Pandoc (recommandé) | Script |
| `Create-WordDoc.ps1` | Conversion avec MS Word | Script |

## ?? Méthodes de Génération du Document Word

### Méthode 1: Avec Pandoc (RECOMMANDÉE) ?

**Avantages**: 
- Meilleur formatage
- Tables et tableaux correctement formatés
- Coloration syntaxique du code
- Table des matières automatique

**Installation de Pandoc**:

```powershell
# Option 1: Chocolatey
choco install pandoc

# Option 2: Winget
winget install --id JohnMacFarlane.Pandoc

# Option 3: Téléchargement manuel
# https://pandoc.org/installing.html
```

**Utilisation**:

```powershell
cd Documentation
.\Convert-ToWord.ps1
```

Le script créera automatiquement `RAPPORT_ANALYSE_STATIQUE.docx`

---

### Méthode 2: Avec Microsoft Word

**Prérequis**: Microsoft Word installé sur votre machine

**Utilisation**:

```powershell
cd Documentation
.\Create-WordDoc.ps1
```

**Note**: Cette méthode fait une conversion basique. Le formatage peut nécessiter des ajustements manuels.

---

### Méthode 3: Conversion Manuelle

**Si les scripts ne fonctionnent pas**:

1. Ouvrir `RAPPORT_ANALYSE_STATIQUE.md` dans Visual Studio Code
2. Installer l'extension "Markdown PDF" ou "Word"
3. Clic droit ? "Markdown: Export (docx)"

**OU**

1. Ouvrir Microsoft Word
2. Fichier ? Ouvrir ? Sélectionner le fichier `.md`
3. Word convertira automatiquement le Markdown
4. Fichier ? Enregistrer sous ? `.docx`

---

### Méthode 4: Convertisseur en Ligne

Si rien ne fonctionne:

1. Aller sur https://pandoc.org/try/
2. Coller le contenu du fichier `.md`
3. Sélectionner "To: docx"
4. Cliquer sur "Convert"
5. Télécharger le fichier généré

---

## ?? Contenu du Rapport

Le rapport contient **22 pages** couvrant:

### ? Section 1: Informations Générales
- Équipe et dates
- Objectifs de l'analyse
- Méthodologie (3 activités statiques)

### ? Section 2: Activités Statiques
- **Activité 1**: Revue de code manuelle (pair review)
- **Activité 2**: Analyse automatisée (.NET Roslyn)
- **Activité 3**: Analyse SonarQube

### ? Section 3: Résultats
- **37 problèmes détectés**
- **28 problèmes corrigés** (76%)
- Classification par sévérité: Critique, Majeur, Mineur, Info

### ? Section 4: Problèmes Détaillés
- 6 catégories: Sécurité, Qualité, Architecture, Performance, Maintenabilité, Standards
- Code "avant/après" pour chaque correction
- Responsables et statuts

### ? Section 5: Bonnes Pratiques
- Points forts identifiés
- Architecture Clean
- Patterns de conception
- Sécurité robuste

### ? Section 6: Métriques
- Statistiques du code (19,813 LOC)
- Complexité cyclomatique
- Dette technique (6h 30min)
- Score global: **8.68/10** ????

### ? Section 7: Outils & Méthodes
- Configuration SonarQube
- Analyseurs Roslyn
- Commandes d'analyse

### ? Section 8: Conclusion
- Synthèse
- Recommandations
- Approbation du code

---

## ?? Personnalisation Requise

Avant de remettre le rapport, compléter:

1. **Page 1**: Noms des 2 membres du groupe
2. **Page 1**: Date exacte de révision
3. **Page 20**: Signatures des membres
4. **Optionnel**: Ajouter des captures d'écran SonarQube

### Emplacements à compléter:

```markdown
| **Réviseurs** | Membre 1: [Nom à compléter]
                 Membre 2: [Nom à compléter] |
```

```markdown
**Membre 1**: ________________________________  
*Nom*: [À compléter]  
*Date*: _____________

**Membre 2**: ________________________________  
*Nom*: [À compléter]  
*Date*: _____________
```

---

## ?? Checklist de Remise

Avant de soumettre le rapport:

- [ ] Document converti en `.docx`
- [ ] Noms des membres ajoutés
- [ ] Dates complétées
- [ ] Signatures ajoutées (si impression)
- [ ] Captures d'écran SonarQube (optionnel)
- [ ] Vérification orthographique
- [ ] Table des matières générée (automatique avec Pandoc)
- [ ] Numérotation des pages

---

## ?? Statistiques du Rapport

| Métrique | Valeur |
|----------|--------|
| **Nombre de pages** | 22 |
| **Nombre de mots** | ~8,500 |
| **Tableaux** | 35 |
| **Exemples de code** | 25 |
| **Problèmes documentés** | 37 |
| **Corrections effectuées** | 28 |
| **Score qualité final** | 8.68/10 |

---

## ?? Dépannage

### Erreur: "pandoc n'est pas reconnu"

**Solution**: Installer pandoc (voir Méthode 1)

### Erreur: "Microsoft Word n'est pas installé"

**Solution**: Utiliser la Méthode 1 (Pandoc) ou Méthode 3 (manuelle)

### Erreur: "Accès refusé lors de la sauvegarde"

**Solution**: 
1. Fermer tous les documents Word ouverts
2. Exécuter PowerShell en tant qu'administrateur
3. Réessayer

### Le formatage est incorrect

**Solution**: 
1. Utiliser Pandoc (Méthode 1) pour un meilleur formatage
2. OU ajuster manuellement dans Word après conversion

---

## ?? Support

Si vous rencontrez des problèmes:

1. Vérifier que tous les fichiers sont dans le dossier `Documentation/`
2. Essayer les différentes méthodes dans l'ordre
3. Consulter les messages d'erreur détaillés

---

## ?? Ressources Complémentaires

- **Pandoc**: https://pandoc.org/
- **Markdown Guide**: https://www.markdownguide.org/
- **SonarQube**: https://docs.sonarqube.org/

---

**Créé**: Décembre 2024  
**Version**: 1.0  
**Format source**: Markdown (UTF-8)

---

? **Bon courage pour votre remise!** ?
