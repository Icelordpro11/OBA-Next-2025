using System.IO;
using System.Linq;
using _Scripts.Managers_Scripts;
using _Scripts.Scriptable_Object_Scripts;
using _Scripts.Systems_Scripts;
using Artem_Library.Attribute_Scripts;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace _Scripts.Controllers_Scripts
{
    public class BookV2_Controller : MonoBehaviour
    {
        [field: SerializeField] public string PpnNumber { get; set; }
        
        [SerializeField, ExpandableScript] private SO_ChatGPTAgent chatAgent;

        [ContextMenu("🚀 Send Typesense Book Info To Agent")]
        public void SendBookInfoToAgentFromJson()
        {
            NexiFace_System.Instance.FacePlay("NexiFace_Loading");
            PPNLoader_System.Instance.ContentParent.gameObject.SetActive(false);
            Loading_Controller.Instance.EnableHologramLoading(true);
            GameManager.Instance.EnableMauwWatch(false);
            Agents_Helper.StopAllHandDetections();
            ProcessBookData().Forget();
        }

        private async UniTaskVoid ProcessBookData()
        {
            var jsonPath = Path.Combine(Application.persistentDataPath, "Typesense_response.json");

            if (!File.Exists(jsonPath))
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                Debug.LogError($"❌ File not found at: {jsonPath}");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var books = JsonConvert.DeserializeObject<JArray>(jsonContent);

            var targetBook = books?.FirstOrDefault(b => b["ppn"]?.ToString() == PpnNumber);
            if (targetBook == null)
            {
                Debug.LogWarning($"⚠️ No book found with PPN: {PpnNumber}");
                return;
            }

            var formattedPrompt = $"Hier zijn de gegevens van een boek:\n{targetBook.ToString(Formatting.Indented)}";
            Debug.Log($"📤 Sending book to agent:\n{formattedPrompt}");

            var agentResponse = await chatAgent.GetAssistant_Response(formattedPrompt);

            if (string.IsNullOrEmpty(agentResponse))
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                Debug.LogError("❌ Failed to get a response from the agent.");
                return;
            }

            var savePath = Path.Combine(Application.persistentDataPath, "ShelfFindAgent_Response.json");
            await File.WriteAllTextAsync(savePath, JsonConvert.SerializeObject(new { response = agentResponse }, Formatting.Indented));

            Debug.Log($"✅ Agent response saved to: {savePath}");
            FindShelf(agentResponse).Forget();
        }

        private static async UniTask FindShelf(string item)
        {
            var shelfTransform = ShelfFinder_System.Instance.GetTransformForBookshelf(item);

            if (shelfTransform is not null)
            {
                await Agents_Helper.SpeakAndSetText($"Dat boek vind je in {item}. Ga je mee?");
                NexiFace_System.Instance.FacePlay("NexiFace_Happy");
                NexiUI_System.Instance.EnableHologram(false);
                PPNLoader_System.Instance.ContentParent.gameObject.SetActive(true);
                NexiUI_System.Instance.EnableTextBubble(true);
                Loading_Controller.Instance.EnableHologramLoading(false);
                FSM_Controller.Instance.SetCategoryNexi(shelfTransform);
                await UniTask.Delay(2000);
                NexiUI_System.Instance.EnableTextBubble(false);
                GameManager.Instance.EnableMauwWatch(true);
                Assistants_Manager.Instance.StopConversation();
            }
            
            Debug.Log($"📚 Shelf found! Move agent to: {shelfTransform!.name}");
        }

    }
}
