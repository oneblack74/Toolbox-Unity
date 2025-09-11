using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Axel.TextManager.Runtime
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextReplace : MonoBehaviour
    {
        [SerializeField] private string key;

        [SerializeField] private NamedVar[] serializedVars;

        private TextMeshProUGUI text;
        private Dictionary<string, object> runtimeVars;

        void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        void OnEnable()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();

            if (Application.isPlaying)
            {
                if (TextManager.Instance == null) return;
                TextManager.OnLanguageChanged += ReplaceText;
                ReplaceText();
            }
            else
            {
                UpdateEditorPreview();
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying && TextManager.Instance != null)
                TextManager.OnLanguageChanged -= ReplaceText;
        }

        public void SetVars(Dictionary<string, object> vars)
        {
            runtimeVars = vars;
            ReplaceText();
        }

        public void ReplaceText()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();

            if (Application.isPlaying)
            {
                if (TextManager.Instance == null) return;

                var dict = BuildMergedVars();
                if (dict != null && dict.Count > 0)
                    text.text = TextManager.Instance.GetText(key, dict);
                else
                    text.text = TextManager.Instance.GetText(key);
            }
            else
            {
                UpdateEditorPreview();
            }
        }

        private Dictionary<string, object> BuildMergedVars()
        {
            if ((serializedVars == null || serializedVars.Length == 0) &&
                (runtimeVars == null || runtimeVars.Count == 0))
                return null;

            var dict = new Dictionary<string, object>();
            if (serializedVars != null)
                foreach (var v in serializedVars)
                    if (!string.IsNullOrEmpty(v.name)) dict[v.name] = v.value;

            if (runtimeVars != null)
                foreach (var kv in runtimeVars)
                    dict[kv.Key] = kv.Value;

            return dict;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (text == null) text = GetComponent<TextMeshProUGUI>();
            UpdateEditorPreview();
        }

        private void UpdateEditorPreview()
        {
            if (!string.IsNullOrEmpty(key)) text.text = key;
        }
#endif

        [System.Serializable]
        public struct NamedVar
        {
            public string name;
            public string value;
        }
    }
}
