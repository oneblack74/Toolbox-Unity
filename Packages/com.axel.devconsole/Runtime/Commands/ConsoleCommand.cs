using UnityEngine;
using UnityEngine.Events;

public abstract class ConsoleCommand : ScriptableObject
{
    public string commandName;
    public string[] options;

    // options = new[]
    // {
    //     "1:alice|bob|charlie", // 1er argument
    //     "2:0|10|100"           // 2e argument
    // };

    public abstract string Excecute(string[] args);

}
