using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(CommandExecutor))]
public class DeveloperConsole : MonoBehaviour
{
    public static DeveloperConsole Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private TMP_InputField inputField;
    private CommandExecutor commandExecutor;
    public event Action<string> OnCommandSubmitted;
    public TMP_InputField InputField => inputField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        commandExecutor = GetComponent<CommandExecutor>();
    }

    private void Start()
    {
        inputField.onEndEdit.AddListener(HandleInput);
    }

    private void HandleInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            inputField.text = string.Empty;
            inputField.ActivateInputField();
            return;
        }

        // Historique : notifier
        OnCommandSubmitted?.Invoke(input);

        // Affichage console + exécution
        consoleText.text += input + "\n";
        consoleText.text += commandExecutor.ExcecuteCommand(input);

        // Reset champ
        inputField.text = string.Empty;
        inputField.ActivateInputField();
    }


    public void WriteToInputField(string text)
    {
        inputField.text = text;
        inputField.caretPosition = text.Length;
        inputField.ActivateInputField();
    }

    [ContextMenu("Show Console")]
    public void ShowConsole()
    {
        gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    [ContextMenu("Hide Console")]
    public void HideConsole()
    {
        gameObject.SetActive(false);
    }


}
