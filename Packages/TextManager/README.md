# Text Manager

Localisation basée sur un **CSV** : 1re colonne = `Key`, colonnes suivantes = langues.  
Supporte les **variables nommées** (`{player}`, `{count}`, …) et TextMeshPro.

---

## Installation

- Téléchargez **TextManager.unitypackage** :  
  [`Packages/TextManager/TextManager.unitypackage`](TextManager.unitypackage)
- Unity → **Assets → Import Package → Custom Package…** → importez tout.

**Contenu importé (exemple)**
- `Assets/…/TextManager/Prefabs/TextManager.prefab`
- `Assets/…/TextManager/Scripts/*`
- `Assets/…/TextManager/Localization/Text.csv` (exemple)

---

## Utilisation

1) **Prefab / Manager**
- Ajoutez le **prefab `TextManager`** dans votre scène  
  *(ou créez un GO avec `LanguageManager` + `TextManager`)*.

2) **UI localisée**
- Ajoutez le composant **`TextReplace`** sur un `TextMeshProUGUI`.  
- Menu rapide : **GameObject → UI → Localized Text**.

3) **CSV**
- La 1re colonne contient la **Key** (souvent préfixée `$`).  
  Exemple :  
```Key;French;English
$PLAY;Jouer;Play
$HELLO;Bonjour {player}!;Hello {player}!
```
- Pour injecter des variables, ajoutez-les sur `TextReplace` → **Serialized Vars** :
- Name → `player`, Value → `Axel`  → affiche “Bonjour Axel !”.

---

## API rapide

```csharp
// Sans variables
var s = TextManager.Instance.GetText("$PLAY");

// Avec variables nommées
var txt = TextManager.Instance.GetText(
  "$HELLO",
  new Dictionary<string, object> { ["player"] = "Axel" }
);
```
> TIP : si une variable manque, le placeholder {name} reste visible, pratique pour débug.
