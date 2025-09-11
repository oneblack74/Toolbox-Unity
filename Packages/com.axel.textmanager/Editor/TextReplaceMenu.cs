using UnityEditor;
using UnityEngine;
using TMPro;

namespace Axel.TextManager.Editor
{
    public class TextReplaceMenu
    {
        [MenuItem("GameObject/UI/Localized Text", false, 2000)]
        public static void CreateLocalizedText(MenuCommand menuCommand)
        {
            // Crée un GameObject
            GameObject go = new GameObject("LocalizedText");

            // L’attache au parent sélectionné (si c’est un Canvas)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Ajoute les composants
            var text = go.AddComponent<TextMeshProUGUI>();
            go.AddComponent<Runtime.TextReplace>();

            // Valeur par défaut (pour voir quelque chose direct)
            text.text = "Key";

            // Sélectionne l’objet créé
            Selection.activeGameObject = go;
        }
    }
}