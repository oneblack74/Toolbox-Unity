using System;
using UnityEngine;

namespace Axel.TextManager.Runtime
{
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance;
        public static event Action<Language> OnLanguageChanged;

        public enum Language
        {
            French,
            English
        }

        [SerializeField] private Language currentLanguage = Language.French;
        public Language CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                if (currentLanguage != value)
                {
                    currentLanguage = value;
                    OnLanguageChanged?.Invoke(currentLanguage);
                }
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
                lastLanguage = currentLanguage;
#endif
                OnLanguageChanged?.Invoke(currentLanguage);
            }
            else
            {
                Destroy(gameObject);
            }
        }

#if UNITY_EDITOR
        private Language lastLanguage;
#endif

        void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;

            if (lastLanguage != currentLanguage)
            {
                lastLanguage = currentLanguage;
                OnLanguageChanged?.Invoke(currentLanguage);
            }
#endif
        }
    }
}

