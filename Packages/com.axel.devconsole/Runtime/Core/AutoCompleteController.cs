using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(DeveloperConsole))]
[RequireComponent(typeof(CommandExecutor))]
public class AutoCompleteController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform suggestionsPanel;
    [SerializeField] private TMP_Text suggestionItemPrefab;
    [SerializeField] private int maxVisible = 8;

    [Header("Behaviour")]
    [SerializeField] private bool completeOnEnter = true;
    [SerializeField] private bool caseInsensitive = true;

    private DeveloperConsole console;
    private CommandExecutor executor;
    private TMP_InputField input;

    private readonly List<string> suggestions = new();
    private readonly List<TMP_Text> itemPool = new();
    private int selectedIndex = -1;
    private bool isOpen = false;
    public bool IsOpen { get { return isOpen; } private set { isOpen = value; } }

    private void Awake()
    {
        console = GetComponent<DeveloperConsole>();
        executor = GetComponent<CommandExecutor>();
    }

    private void OnEnable()
    {
        input = console.InputField;
        input.onValueChanged.AddListener(OnTextChanged);
        CloseMenu();
    }

    private void OnDisable()
    {
        input.onValueChanged.RemoveListener(OnTextChanged);
        CloseMenu();
    }

    private void Update()
    {
        if (input == null || !input.isFocused) return;

        if (IsOpen)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex = Mathf.Clamp(selectedIndex + 1, 0, suggestions.Count - 1);
                RefreshHighlight();
                return;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, suggestions.Count - 1);
                RefreshHighlight();
                return;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                ApplySelection();
                return;
            }
            if (completeOnEnter && UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                ApplySelection(); // puis DeveloperConsole gère le submit
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                CloseMenu();
                return;
            }
        }
        else
        {
            // Ouvre/refresh avec Tab même si le texte n’a pas changé
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                BuildSuggestions();
                return;
            }
        }
    }

    private void OnTextChanged(string _) => BuildSuggestions();

    private void BuildSuggestions()
    {
        ParseInput(out string cmdName, out int argIndex, out string prefix);

        suggestions.Clear();

        if (argIndex == 0)
        {
            // Complétion du nom de commande
            IEnumerable<ConsoleCommand> src = executor.Commands;
            if (!string.IsNullOrEmpty(prefix))
                src = src.Where(c => StartsWith(c.commandName, prefix));
            suggestions.AddRange(src.Select(c => c.commandName));
        }
        else
        {
            // Complétion des args à partir de .options
            if (executor.TryGetCommand(cmdName, out var cmd))
            {
                foreach (var s in SuggestFromOptions(cmd.options, argIndex, prefix))
                    suggestions.Add(s);
            }
        }

        var distinctSuggestions = suggestions.Distinct().Take(64).ToList();
        suggestions.Clear();
        suggestions.AddRange(distinctSuggestions);

        if (suggestions.Count == 0)
        {
            CloseMenu();
            return;
        }

        OpenOrRefreshMenu();
    }

    private IEnumerable<string> SuggestFromOptions(string[] options, int argIndex, string prefix)
    {
        if (options == null || options.Length == 0)
            yield break;

        // 2 syntaxes supportées :
        //  1) "flat": ["on","off","toggle"]
        //  2) "positionnelle": ["1:alice|bob|charlie", "2:0|10|100"]
        bool anyPositional = options.Any(o => o.Contains(":"));

        if (!anyPositional)
        {
            foreach (var o in options)
                if (StartsWith(o, prefix)) yield return o;
        }
        else
        {
            int wanted = argIndex; // 1 = premier arg après le nom
            foreach (var o in options)
            {
                int colon = o.IndexOf(':');
                if (colon <= 0) continue;

                // parse "N:val1|val2|val3"
                var indexPart = o.Substring(0, colon).Trim();
                if (!int.TryParse(indexPart, out int pos)) continue;
                if (pos != wanted) continue;

                var valuesPart = o.Substring(colon + 1);
                var values = valuesPart.Split('|');

                foreach (var v in values)
                    if (StartsWith(v, prefix)) yield return v;
            }
        }
    }

    private bool StartsWith(string a, string b)
        => string.IsNullOrEmpty(b)
           ? true
           : (caseInsensitive
              ? a.StartsWith(b, System.StringComparison.OrdinalIgnoreCase)
              : a.StartsWith(b));

    private void OpenOrRefreshMenu()
    {
        EnsurePoolSize(Mathf.Min(suggestions.Count, maxVisible));

        for (int i = 0; i < itemPool.Count; i++)
        {
            if (i < suggestions.Count)
            {
                itemPool[i].text = suggestions[i];
                itemPool[i].gameObject.SetActive(true);

                int idx = i;
                var btn = itemPool[i].GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => { selectedIndex = idx; ApplySelection(); });
                }
            }
            else
            {
                itemPool[i].gameObject.SetActive(false);
            }
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, suggestions.Count - 1);
        RefreshHighlight();

        suggestionsPanel.gameObject.SetActive(true);
        IsOpen = true;
    }

    private void EnsurePoolSize(int count)
    {
        while (itemPool.Count < count)
        {
            var item = Instantiate(suggestionItemPrefab, suggestionsPanel);
            item.gameObject.SetActive(false);
            itemPool.Add(item);
        }
    }

    private void RefreshHighlight()
    {
        for (int i = 0; i < itemPool.Count; i++)
        {
            bool sel = (i == selectedIndex);
            itemPool[i].fontStyle = sel ? FontStyles.Bold : FontStyles.Normal;

            // petit chevron visuel
            var t = itemPool[i].text;
            if (t.StartsWith("> ")) t = t.Substring(2);
            if (sel) t = "> " + t;
            itemPool[i].text = t;
        }
    }

    private void ApplySelection()
    {
        if (!IsOpen || selectedIndex < 0 || selectedIndex >= suggestions.Count) return;

        ParseInput(out string _, out int argIndex, out _);
        string chosen = suggestions[selectedIndex];

        // remplace uniquement le token courant
        string text = input.text;
        GetTokenBounds(input, out int tokenStart, out int tokenLen, out bool endsWithSpace);
        string before = text.Substring(0, tokenStart);
        string after = text.Substring(tokenStart + tokenLen);

        bool addSpace = (argIndex == 0) || (!endsWithSpace && argIndex > 0);
        string completed = before + chosen + (addSpace ? " " : "") + after;

        console.WriteToInputField(completed);

        // rebuild pour enchaîner d'autres complétions d'args
        BuildSuggestions();
    }

    private void CloseMenu()
    {
        suggestions.Clear();
        foreach (var it in itemPool) it.gameObject.SetActive(false);
        suggestionsPanel.gameObject.SetActive(false);
        selectedIndex = -1;
        IsOpen = false;
    }

    // ---- parsing & token helpers ----

    private void ParseInput(out string commandName, out int argIndex, out string prefix)
    {
        string text = input.text;
        GetTokenBounds(input, out int tokenStart, out int tokenLen, out bool endsWithSpace);

        var tokens = text.Split(' ')
                         .Where(t => t.Length > 0)
                         .ToList();

        if (endsWithSpace)
        {
            argIndex = tokens.Count; // on édite le prochain arg (vide)
            prefix = string.Empty;
        }
        else
        {
            prefix = tokenLen > 0 ? text.Substring(tokenStart, tokenLen) : string.Empty;

            int idx = 0;
            int pos = 0;
            foreach (var t in text.Split(' '))
            {
                if (t.Length == 0) { pos++; continue; }
                int start = pos;
                int len = t.Length;
                int end = start + len;

                if (tokenStart >= start && tokenStart <= end)
                {
                    argIndex = idx;
                    goto Done;
                }
                pos = end + 1;
                idx++;
            }
            argIndex = tokens.Count;
        }

    Done:
        commandName = tokens.Count > 0 ? tokens[0] : string.Empty;
    }

    private void GetTokenBounds(TMP_InputField field, out int tokenStart, out int tokenLen, out bool endsWithSpace)
    {
        string text = field.text;
        int caret = field.caretPosition;
        endsWithSpace = caret > 0 && caret <= text.Length && text[caret - 1] == ' ';

        if (text.Length == 0) { tokenStart = 0; tokenLen = 0; return; }

        int start = caret - 1;
        while (start >= 0 && text[start] != ' ') start--;
        tokenStart = start + 1;

        int end = caret;
        while (end < text.Length && text[end] != ' ') end++;
        tokenLen = Mathf.Max(0, end - tokenStart);
    }
}
