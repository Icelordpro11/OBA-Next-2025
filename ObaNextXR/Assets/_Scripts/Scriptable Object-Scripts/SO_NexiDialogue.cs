using UnityEngine;

namespace _Scripts.Scriptable_Object_Scripts
{
    [CreateAssetMenu(fileName = "NexiDialogue_Data", menuName = "AI/NexiDialogueData", order = 0)]
    public class SO_NexiDialogue : ScriptableObject
    {
        [Header("Assistent Manager Dialogue")]
        public string responseBookFound = "Ik heb wat boeken gevonden.";
        
        [Header("Book Cover Controller Dialogue")]
        public string aanwezigSentence = "Het boek is beschikbaar op plank <color=#FFA500>{item.shelfmark}</color>.";
        public string aanwezigUnknownShelfSentence = "Het boek is aanwezig, maar de exacte plank is onbekend. Probeer sublocatie <color=#00BFFF>{item.subloc}</color>.";
        public string nietAaanwezigOpuisteLocatie = "Dit boek is te vinden in de bibliotheek bij {fallback.locationName}";
        public string uitgeleendSentence = "<color=#FF6347>Het boek is momenteel uitgeleend.</color>";
        public string nietBeschikbaarSentence = "<color=#FF0000>Het boek is niet beschikbaar.</color>";
        public string onbekendeStatusSentence = "De status van het boek is: <color=#999999>{item.status}</color>.";

    }
}