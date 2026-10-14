using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using TMPro;
using UnityEngine;

namespace _Scripts
{
    public class NexiUI_System : Singleton_MonoBehaviour<NexiUI_System>
    {
        [SerializeField] private TextMeshProUGUI aiDialogueText;
        [SerializeField] private GameObject hologramObject, textBubbleObject;
        
        public void SetDialogueText(string text)
        {
            var filteredText = RemoveEmojis(text);
            aiDialogueText.text = filteredText;
        }

        private static string RemoveEmojis(string input) => System.Text.RegularExpressions.Regex.Replace(input, @"\p{Cs}", "");

        public void EnableHologram(bool enable) => hologramObject.SetActive(enable);
        public void EnableTextBubble(bool enable) => textBubbleObject.SetActive(enable);
        
    }
}