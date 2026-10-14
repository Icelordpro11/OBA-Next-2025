using System.IO;
using System.Linq;
using _Scripts;
using _Scripts.Scriptable_Object_Scripts;
using _Scripts.Systems_Scripts;
using Artem_Library.Attribute_Scripts;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Controllers_Scripts
{
    public class BookV1_Controller : MonoBehaviour
    {
        [field: SerializeField] public string PpnNumber { get; set; }

        [Header("UI References")]
        [SerializeField] private GameObject bookNotValidImage;
        [SerializeField] private TextMeshProUGUI notValidText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image statusImage;

        [Header("Visual Feedback Settings")]
        [SerializeField, ExpandableScript] private SO_VisualFeedback aanwezigFeedbackValues;
        [SerializeField, ExpandableScript] private SO_VisualFeedback uitgeleendFeedbackValues;
        [SerializeField, ExpandableScript] private SO_VisualFeedback locatieFeedbackValues;

        [SerializeField, ExpandableScript] private SO_ObaAgent obaAgent;
        [SerializeField, ExpandableScript] private SO_NexiDialogue dialogueSentences;
        
        private const string TargetLocation = "OBA Oosterdok (centrale)";
        private const string JsonFilename = "ObaAgentFullResponse.json";

        [ContextMenu("\uD83D\uDD0D Fetch Shelf Finder Data")]
        public void FetchShelfDataFromContextMenu()
        {
            if (string.IsNullOrEmpty(PpnNumber) || !obaAgent) return;
            FetchAndProcessDataAsync().Forget();
        }

        private async UniTask FetchAndProcessDataAsync()
        {
            await obaAgent.FetchAllDataAsync(PpnNumber);
            await UniTask.Delay(100);
            await ProcessObaResponseAsync();
        }

        private async UniTask ProcessObaResponseAsync()
        {
            // Update the path to use persistent data
            var filePath = Path.Combine(Application.persistentDataPath, JsonFilename);
            var json = await LoadJsonAsync(filePath);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("❌ Failed to load or empty JSON.");
                return;
            }

            var items = JsonConvert.DeserializeObject<SO_ObaAgent.LocationItem[]>(json);

            if (items == null || items.Length == 0)
            {
                Debug.LogWarning("⚠️ No location items in response.");
                return;
            }

            var targetLocationItem = FindTargetLocation(items);

            if (targetLocationItem != null)
            {
                await HandleTargetLocationStatus(targetLocationItem);
            }
            else
            {
                var fallback = items.FirstOrDefault();
                if (fallback != null)
                {
                    ApplyVisualFeedback(locatieFeedbackValues);
                    bookNotValidImage.SetActive(true);
                    notValidText.text = $"Beschikbaar in {fallback.locationName}";
                    NexiFace_System.Instance.FacePlay("NexiFace_Derp");
                    await SpeakAndSetText($"Dit boek is te vinden in de bibliotheek bij {fallback.locationName}", fallback);
                }
            }
        }

        private static async UniTask<string> LoadJsonAsync(string path)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
    // For Android, just read from the persistent data path directly
    if (File.Exists(path))
    {
        return await File.ReadAllTextAsync(path);
    }
    else
    {
        Debug.LogError("❌ File not found: " + path);
        return null;
    }
#else
            // For non-Android platforms, use UnityWebRequest for StreamingAssets or web paths
            if (File.Exists(path)) return await File.ReadAllTextAsync(path);
            Debug.LogError("❌ File not found: " + path);
            return null;
#endif
        }



        private static SO_ObaAgent.LocationItem FindTargetLocation(SO_ObaAgent.LocationItem[] items) =>
            items.FirstOrDefault(item => item.locationName == TargetLocation);

        private async UniTask HandleTargetLocationStatus(SO_ObaAgent.LocationItem item)
        {
            ResetAllVisuals();

            if (IsAanwezigAtTargetLocation(item))
            {
                await HandleAanwezigStatus(item);
            }
            else
            {
                await HandleNonAanwezigStatus(item);
            }
        }

        private static bool IsAanwezigAtTargetLocation(SO_ObaAgent.LocationItem item)
        {
            return item.locationName == TargetLocation &&
                   item.available &&
                   item.status?.ToLowerInvariant().Contains("aanwezig") == true;
        }

        private async UniTask HandleAanwezigStatus(SO_ObaAgent.LocationItem item)
        {
            var shelfTransform = ShelfFinder_System.Instance.GetTransformForBookshelf(item.shelfmark);

            if (shelfTransform is not null)
            {
                await SpeakAndSetText(dialogueSentences.aanwezigSentence, item);
                ApplyVisualFeedback(aanwezigFeedbackValues);
                bookNotValidImage.SetActive(false);
                NexiFace_System.Instance.FacePlay("NexiFace_Happy");
                Debug.Log($"📚 Shelf found! Move agent to: {shelfTransform.name}");
                FSM_Controller.Instance.SetCategoryNexi(shelfTransform);
            }
            else
            {
                await SpeakAndSetText(dialogueSentences.aanwezigUnknownShelfSentence, item);
                ApplyVisualFeedback(locatieFeedbackValues);
                bookNotValidImage.SetActive(true);
                notValidText.text = $"Beschikbaar op: {item.subloc ?? "Onbekende locatie"}";
            }
        }

        private async UniTask HandleNonAanwezigStatus(SO_ObaAgent.LocationItem item)
        {
            bookNotValidImage.SetActive(true);

            var status = item.status?.ToLowerInvariant() ?? "onbekende status";

            switch (status.Contains("uitgeleend"))
            {
                case false when status.Contains("niet beschikbaar"):
                    await SpeakAndSetText(dialogueSentences.nietBeschikbaarSentence, item);
                    ApplyVisualFeedback(locatieFeedbackValues);
                    notValidText.text = "NIET BESCHIKBAAR";
                    break;
                case false:
                    await SpeakAndSetText(dialogueSentences.onbekendeStatusSentence, item);
                    ApplyVisualFeedback(locatieFeedbackValues);
                    notValidText.text = item.status ?? "Onbekende status";
                    break;
                default:
                    await SpeakAndSetText(dialogueSentences.uitgeleendSentence, item);
                    ApplyVisualFeedback(uitgeleendFeedbackValues);
                    notValidText.text = "UITGELEEND";
                    break;
            }
        }

        private void ResetAllVisuals()
        {
            aanwezigFeedbackValues.Reset(statusImage);
            uitgeleendFeedbackValues.Reset(statusImage);
            locatieFeedbackValues.Reset(statusImage);
        }

        private void ApplyVisualFeedback(SO_VisualFeedback feedbackValues)
        {
            ResetAllVisuals();
            statusImage.gameObject.SetActive(true);
            feedbackValues.Apply(backgroundImage, statusImage);
        }

        private static async UniTask SpeakAndSetText(string template, SO_ObaAgent.LocationItem item)
        {
            if (item == null)
            {
                await OpenAITTS_System.Instance.Speak_Async(template);
                NexiUI_System.Instance.SetDialogueText(template);
                return;
            }

            var sentence = template
                .Replace("{item.subloc}", Clean(item.subloc))
                .Replace("{item.shelfmark}", Clean(item.shelfmark))
                .Replace("{item.locationName}", Clean(item.locationName))
                .Replace("{item.status}", Clean(item.status));

            await OpenAITTS_System.Instance.Speak_Async(sentence);
            NexiUI_System.Instance.SetDialogueText(sentence);
            return;
            
            static string Clean(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return "onbekend";
                var cleaned = value.Split('/').FirstOrDefault()?.Trim().ToLowerInvariant() ?? "onbekend";

                if (!cleaned.Contains("etage")) return cleaned;
                cleaned = cleaned.Replace("etage", "verdieping");
                cleaned = ReplaceFloorNumberWithWord(cleaned);

                return cleaned;
            }

            static string ReplaceFloorNumberWithWord(string input)
            {
                return input
                    .Replace("0e", "begane")
                    .Replace("1e", "eerste")
                    .Replace("2e", "tweede")
                    .Replace("3e", "derde")
                    .Replace("4e", "vierde")
                    .Replace("5e", "vijfde")
                    .Replace("6e", "zesde")
                    .Replace("7e", "zevende")
                    .Replace("8e", "achtste")
                    .Replace("9e", "negende");
            }
        }
    }
}
