using System.Collections.Generic;
using UnityEngine;

public class CommandExecutor : MonoBehaviour
{
    [SerializeField] private ConsoleCommand[] commands;

    private Dictionary<string, ConsoleCommand> commandDictionary;
    public IReadOnlyList<ConsoleCommand> Commands => commands;

    private void Awake()
    {
        commandDictionary = new Dictionary<string, ConsoleCommand>();
        foreach (var command in commands)
        {
            if (commandDictionary.ContainsKey(command.commandName))
            {
                Debug.LogError("Command " + command.commandName + " already exists in the dictionary.");
                continue;
            }
            commandDictionary.Add(command.commandName, command);
        }
    }

    public bool TryGetCommand(string name, out ConsoleCommand cmd)
        => commandDictionary.TryGetValue(name, out cmd);


    public string ExcecuteCommand(string input)
    {
        string[] inputSplit = input.Split(' ');

        // split le nom de la commande
        string commandName = inputSplit[0];

        // split les options de la commande
        string[] args = new string[inputSplit.Length - 1];
        for (int i = 1; i < inputSplit.Length; i++)
        {
            args[i - 1] = inputSplit[i];
        }

        Debug.Log("Command name: " + commandName);
        Debug.Log("Command args: " + string.Join(", ", args));

        if (commandDictionary.ContainsKey(commandName))
        {
            string rep = commandDictionary[commandName].Excecute(args);

            return "Command executed: " + rep + "\n";
        }
        else
        {
            return "Command not found\n";
        }
    }

}
