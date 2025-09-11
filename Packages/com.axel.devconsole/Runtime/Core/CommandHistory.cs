using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(DeveloperConsole))]
[RequireComponent(typeof(AutoCompleteController))]
public class CommandHistory : MonoBehaviour
{
    [Header("History Settings")]
    [SerializeField] private int maxEntries = 100;
    [Tooltip("Si activé, ↑/↓ ne parcourt que les commandes commençant par le texte déjà présent dans l'input.")]
    [SerializeField] private bool filterByPrefix = false;

    private DeveloperConsole console;
    private AutoCompleteController autoComplete;
    private TMP_InputField input;

    private readonly List<string> entries = new List<string>();
    // index dans entries quand on parcourt l'historique ; -1 = pas dans l'historique
    private int browseIndex = -1;

    // Pour restaurer ce que l'utilisateur tapait avant d'entrer dans l'historique
    private string liveBuffer = string.Empty;

    private void Awake()
    {
        console = GetComponent<DeveloperConsole>();
        autoComplete = GetComponent<AutoCompleteController>();
    }

    private void OnEnable()
    {
        input = console.InputField;
        console.OnCommandSubmitted += Add;
    }

    private void OnDisable()
    {
        console.OnCommandSubmitted -= Add;
    }

    private void Update()
    {
        if (input == null || !input.isFocused) return;

        // Navigation ↑ / ↓
        if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShowPrevious();
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowNext();
        }
    }

    private void Add(string cmd)
    {
        if (string.IsNullOrWhiteSpace(cmd)) return;

        // Évite les doublons consécutifs
        if (entries.Count == 0 || entries[entries.Count - 1] != cmd)
            entries.Add(cmd);

        // Taille max
        if (entries.Count > maxEntries)
            entries.RemoveAt(0);

        ResetBrowsing();
    }

    private void ResetBrowsing()
    {
        browseIndex = -1;
        liveBuffer = string.Empty;
    }

    private void ShowPrevious()
    {
        string prefix = filterByPrefix ? input.text : null;

        // Première entrée dans l'historique : mémoriser le buffer
        if (browseIndex == -1)
        {
            liveBuffer = input.text;
            // positionne sur la fin (on va chercher en partant du dernier)
            browseIndex = entries.Count;
        }

        int nextIndex = FindPrevIndex(browseIndex - 1, prefix);
        if (nextIndex >= 0)
        {
            browseIndex = nextIndex;
            Write(entries[browseIndex]);
        }
        else
        {
            // rien de plus haut : rester où on est
        }
    }

    private void ShowNext()
    {
        if (browseIndex == -1)
        {
            // déjà sur le buffer courant : rien à faire
            return;
        }

        string prefix = filterByPrefix ? input.text : null;

        int nextIndex = FindNextIndex(browseIndex + 1, prefix);
        if (nextIndex >= 0 && nextIndex < entries.Count)
        {
            browseIndex = nextIndex;
            Write(entries[browseIndex]);
        }
        else
        {
            // On a dépassé la fin de l'historique -> restaurer le buffer courant
            browseIndex = -1;
            Write(liveBuffer);
        }
    }

    private int FindPrevIndex(int start, string prefix)
    {
        for (int i = start; i >= 0; i--)
        {
            if (prefix == null || entries[i].StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    private int FindNextIndex(int start, string prefix)
    {
        for (int i = start; i < entries.Count; i++)
        {
            if (prefix == null || entries[i].StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    private void Write(string text)
    {
        // Utilise l’API existante de la console pour setter le champ + caret + focus
        console.WriteToInputField(text);
    }

    // Optionnel : call via context menu pour vider l'historique
    [ContextMenu("Clear History")]
    public void ClearHistory()
    {
        entries.Clear();
        ResetBrowsing();
    }
}
