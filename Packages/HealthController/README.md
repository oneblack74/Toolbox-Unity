# Health Controller

Gestion simple de la **vie** d’un acteur avec **invincibilité temporaire (i-frames)**, **événements Unity**, et **UI de barre de vie** optionnelle.

- `HealthController` : logique de PV, i-frames, événements `onHit` / `onDeath`, helpers d’édition.
- `HealthControllerUI` : met à jour visuellement une **Image** Unity UI (type **Filled**) via `fillAmount`.

---

## Installation

- Téléchargez & importez : **HealthController.unitypackage**  
  *(via votre Release GitHub ou votre dossier `Packages/HealthController/`)*
- Unity → **Assets → Import Package → Custom Package…** → importez tout.

---

## Contenu (exemple)

```
Assets/
├─ Prefabs/
│  └─ HealthController/
│     └─ HealthBar.prefab
├─ Scenes/
│  └─ HealthController/
│     └─ HealthControllerSceneExemple.unity
├─ Scripts/
│  └─ HealthController/
│     ├─ HealthController.cs
│     └─ HealthControllerUI.cs
└─ UI/
   └─ HealthController/
      └─ HealthBarUI.png
```

---

## Quick Start

1) **Ajouter le contrôleur de PV**  
   - Ajoutez `HealthController` sur votre GameObject.  
   - Configurez :  
     - **Max Health**, **Min Health** (ex. 100 / 0)  
     - **Iframes Duration** (ex. `0.5` s) — période d’invincibilité après un hit.
   - **Important** : au premier usage, appelez `Reset()` (au spawn/Start) pour initialiser `currentHealth = maxHealth`.

2) **(Optionnel) Lier l’UI**  
   - Ajoutez `HealthControllerUI` sur un objet UI.  
   - Assignez **Image** → une `Image` Unity dont le **Image Type = Filled** (Horizontal/Vertical/Radial).  
   - Référencez ce `HealthControllerUI` dans le champ du `HealthController`.  
   - La méthode `UpdateUI(float normalized)` pilote `fillAmount` (0–1).

3) **Événements**  
   - Branchez `onHit` et `onDeath` dans l’Inspector (sons, FX, respawn…).

---

## API

```csharp
// Réinitialiser la vie (plein)
health.Reset();                                   // current = max, i-frames = 0, dead = false

// Infliger des dégâts (respecte les i-frames configurées)
health.TakeDamage(10f);                           // utilise iframesDuration par défaut
health.TakeDamage(25f, 1.0f);                     // i-frames personnalisées 1s

// Mettre à jour l'UI (si liée)
health.UpdateHealthUI(current / max);             // appelé automatiquement après dommage/soin

// Afficher/masquer la barre
health.SetHealthBarVisibility(true);              // active/désactive l'Image UI
```

**Comportement interne :**
- Les dégâts s’appliquent seulement si **non invincible** et **non mort**. Les i-frames démarrent à chaque `TakeDamage`.
- `Update()` décrémente le timer d’i-frames en temps réel.
- À `minHealth` atteint → `OnDeath()` → invoque `onDeath` (une seule fois).

---

## Raccourcis Éditeur (Context Menu)

Dans le menu contextuel du composant `HealthController` :  
- **Reset Health** → appelle `Reset()`  
- **Kill** → force la mort  
- **Take 10 Damage** → applique 10 de dégâts

---

## Bonnes pratiques

- Appelez `Reset()` au **spawn** (ou dans `Start`) pour afficher la vie pleine dès le début.  
- Pour l’UI, utilisez une **Image (Filled)** avec un **mask/background** pour un rendu propre.  
- Si vous ne voulez pas d’UI, laissez la référence `HealthControllerUI` **vide** : le contrôleur ignore l’affichage.


---

## Compatibilité

- Exporté avec **Unity 6000.3.2f1** (minimum recommandé : 6000.0+).  
- Nécessite **Unity UI** (`UnityEngine.UI`) pour la barre de vie.
