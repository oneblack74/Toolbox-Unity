using UnityEngine;

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
