# Text Manager (com.axel.textmanager)

Contient toutes les langues dans un `.csv` (1ère colonne = Key, colonnes suivantes = langues).  
Supporte des **variables nommées** `{player}`, `{count}`, etc.

## Hierarchy (exemple)

```
├─ Localization/
│   └─ Text.csv
├─ Prefabs/
│   └─ TextManager/
│       └─ TextManager.prefab
└─ Scripts/
    └─ TextManager/
        ├─ LanguageManager.cs
        ├─ TextManager.cs
        ├─ TextReplace.cs
        └─ TextReplaceMenu.cs
```

---

## Utilisation
- Placez le **prefab `TextManager`** dans vos scènes (ou un GO avec `LanguageManager` + `TextManager`).
- Pour afficher un texte, utilisez `TextReplace` sur vos UI **TextMeshProUGUI**.  
  Vous pouvez créer un texte localisé depuis le menu **GameObject → UI → Localized Text**.
- Le `.csv` contient la **Key** et une colonne par langue. Les clés commencent par `$`.  
  Exemple : `$PLAY;Jouer;Play`.
- Variables : définissez `{player}` dans le `.csv` et, sur le `TextReplace`, renseignez **Serialized Vars** :  
  - Name → `player`  
  - Value → `Axel`  
  Exemple : `$HELLO;Bonjour {player}!;Hello {player}!`

## API rapide
```csharp
// Sans variables
var s = TextManager.Instance.GetText("$PLAY");

// Avec variables nommées
var txt = TextManager.Instance.GetText("$HELLO", new Dictionary<string, object> { ["player"] = "Axel" });
```
