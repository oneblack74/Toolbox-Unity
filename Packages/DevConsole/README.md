# Axel Developer Console (com.axel.devconsole)

Console de debug en jeu avec **commandes ScriptableObject**, **historique** ou **auto-complétion** façon terminal. Fournit un prefab prêt à l’emploi et des exemples de commandes.

## Installation (Unity Package Manager)

- Unity → **Window → Package Manager** → **+** → *Add package from git URL…*
- Collez :  
  `https://github.com/<vous>/Toolbox-Unity.git?path=/Packages/com.axel.devconsole#v0.1.0`

> Requiert **TextMeshPro**. Le package le référence automatiquement si le `package.json` contient `"com.unity.textmeshpro"`.

## Samples

Après installation, ouvrez **Package Manager → com.axel.devconsole → Samples → Starter → Import**.  
Cela ajoute sous `Assets/Samples/com.axel.devconsole/...` :
- **Prefabs/** : `DeveloperConsole.prefab`, `SuggestionItemPrefab.prefab`
- **ScriptableObjects/** : `SayHelloCommand.asset`, `AddItemCommand.asset`

## Quick Start

1. **Importez le Sample** puis glissez **`DeveloperConsole.prefab`** dans votre scène.
2. Dans l’Inspector du prefab, **activez soit _CommandHistory_ soit _AutoCompleteController_** (désactivez l’autre).  
   > ⚠️ Ne pas activer les deux en même temps (conflit des raccourcis/flux).
3. Ajoutez vos **commandes** (assets `.asset`) dans la liste `commands` du `CommandExecutor`.
4. Jouez la scène :  
   - **Ouvrir/fermer** la console (touche configurée dans votre script, ex: `²`).  
   - **Historique** : ↑ / ↓ (si `CommandHistory` actif).  
   - **Auto-complétion** : ↑ / ↓ + TAB (si `AutoCompleteController` actif).

## Comment créer une commande

### 1) Script (Runtime)

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "SayHelloCommand", menuName = "DeveloperConsole/Commands/SayHello")]
public class SayHelloCommand : ConsoleCommand
{
    public override string Execute(string[] args)
    {
        var who = (args != null && args.Length > 0) ? args[0] : "world";
        return $"Hello {who}!";
    }
}
```

```csharp
[CreateAssetMenu(fileName = "AddItemCommand", menuName = "DeveloperConsole/Commands/AddItemCommand")]
public class AddItemCommand : ConsoleCommand
{
    public override string Excecute(string[] args)
    {
        if (args.Length < 2)
        {
            return "Usage: additem <itemName> <quantity>";
        }

        string itemName = args[0];
        if (!int.TryParse(args[1], out int quantity) || quantity <= 0)
        {
            return "Quantity must be a positive integer.";
        }

        return $"Added {quantity} of {itemName} to inventory.";
    }
}
```