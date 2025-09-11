using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Axel.TextManager.Runtime
{
    public class TextManager : MonoBehaviour
    {
        public static TextManager Instance;

        [SerializeField] private TextAsset texteAsset;

        private Dictionary<string, string> textes;
        public static event Action OnLanguageChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Parse();
                OnLanguageChanged?.Invoke();
                transform.parent = null;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }



        void OnEnable()
        {
            LanguageManager.OnLanguageChanged += ChangeLanguage;
        }

        void OnDisable()
        {
            LanguageManager.OnLanguageChanged -= ChangeLanguage;
        }

        public void Parse()
        {
            textes = new Dictionary<string, string>();
            if (texteAsset == null) { Debug.LogWarning("No text assets found."); return; }

            var lines = texteAsset.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2) { Debug.LogError("CSV appears to be empty or malformed."); return; }

            var headers = lines[0].Trim().Split(';');

            // Sélection de la langue, avec fallback si LanguageManager n'est pas prêt
            var lm = LanguageManager.Instance;
            var lang = (lm != null) ? lm.CurrentLanguage : LanguageManager.Language.English;

            int langIndex = -1;
            switch (lang)
            {
                case LanguageManager.Language.French: langIndex = Array.IndexOf(headers, "French"); break;
                case LanguageManager.Language.English: langIndex = Array.IndexOf(headers, "English"); break;
            }
            if (langIndex == -1) { Debug.LogError($"Language '{lang}' not found in CSV headers."); return; }

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var values = line.Split(';');
                if (values.Length <= langIndex)
                {
                    Debug.LogWarning($"Line {i + 1} does not contain enough columns for the selected language.");
                    continue;
                }

                string key = values[0].Trim();
                string value = values[langIndex].Trim().Replace("\\n", "\n").Replace("\\r", "\r");

                if (!textes.ContainsKey(key)) textes.Add(key, value);
                else textes[key] = value;
            }
        }

        public string GetText(string key)
        {
            if (textes != null && textes.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
                return value;
            return key;
        }

        public string GetText(string key, IReadOnlyDictionary<string, object> vars)
        {
            var template = GetText(key);
            if (string.IsNullOrEmpty(template) || vars == null || vars.Count == 0) return template;

            string result = Regex.Replace(template, @"\{([a-zA-Z0-9_]+)\}", m =>
            {
                var name = m.Groups[1].Value;
                return vars.TryGetValue(name, out var val) && val != null ? val.ToString() : m.Value;
            });
            return result;
        }

        public string GetText(string key, params object[] args)
        {
            var template = GetText(key);
            if (string.IsNullOrEmpty(template) || args == null || args.Length == 0) return template;
            try { return string.Format(template, args); }
            catch { return template; }
        }

        public void ChangeLanguage(LanguageManager.Language lang)
        {
            Debug.Log($"Changing language to: {lang}");
            Parse();
            OnLanguageChanged?.Invoke();
        }

    }
}