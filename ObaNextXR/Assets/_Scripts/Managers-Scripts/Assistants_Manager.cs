using System;
using System.IO;
using _Scripts.Controllers_Scripts;
using _Scripts.Scriptable_Object_Scripts;
using _Scripts.Systems_Scripts;
using Artem_Library.Attribute_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using Artem_Library.Library_Scripts.Systems_Scripts.Logger_System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Managers_Scripts
{
    public class Assistants_Manager : Singleton_MonoBehaviour<Assistants_Manager>
    {
        [Header("Agents")] [SerializeField, ExpandableScript]
        private SO_ChatGPTAgent firstAgent;

        [SerializeField, ExpandableScript] private SO_ChatGPTAgent searchQueryAgent;
        [SerializeField, ExpandableScript] private SO_ChatGPTAgent vergelijkingsQueryAgent;

        [Header("TypeSense")] 
        [SerializeField, ExpandableScript] private SO_TypeSenseAgent typeSense;

        [Space]
        [Header("Dialogue")] 
        [SerializeField, TextArea(3, 5)] private string dialogueSentences;

        [Header("Debug")] 
        [SerializeField, ExpandableScript] private AdvancedLogger_System logger;

        [Space]
        [SerializeField, TextArea(3, 5)] private string testSentence = "Heb je boeken over dieren met een lange nek?";
        [SerializeField, TextArea(3, 5)] private string agentMessage;
        [SerializeField, TextArea(3, 2)] private string responseMessage;

        [Header("Events")]
        [SerializeField] private UnityEvent continueConversationEvent, bookFoundEvent;

        public string ThreadId { get; private set; }

        [ContextMenu("Start Conversation With Test Sentence")]
        public void TestAssistantWithSentence() => RunAssistantChain(testSentence).Forget();

        [ContextMenu("Respond to Assistant")]
        public void AnswerAssistantWithResponse() => ContinueWithUserInput(responseMessage).Forget();

        [ContextMenu("Stop Conversation")]
        public void StopConversation()
        {
            logger?.LogInfo("Conversation stopped and context cleared.");
            ThreadId = null;
            agentMessage = string.Empty;
            responseMessage = string.Empty;
        }

        public async UniTask RunAssistantChain(string inputSentence)
        {
            logger?.LogInfo($"Starting assistant chain with input: {inputSentence}");

            ThreadId = await firstAgent.Create_Thread();
            if (string.IsNullOrEmpty(ThreadId))
            {
                logger?.LogError("Failed to create thread.");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return;
            }

            var queryResponse = await ContinueUntilUserNeeded(ThreadId, inputSentence);
            if (string.IsNullOrEmpty(queryResponse))
            {
                logger?.LogWarning("Query response was empty or waiting for user input.");
                return;
            }

            agentMessage = queryResponse;
            await HandleQuery_Response(queryResponse);
        }

        public async UniTask ContinueWithUserInput(string userInput)
        {
            logger?.LogInfo($"Continuing conversation with user input: {userInput}");

            if (string.IsNullOrEmpty(ThreadId))
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                logger?.LogError("No existing thread ID. Please run the assistant chain first.");
                return;
            }

            var queryResponse = await ContinueUntilUserNeeded(ThreadId, userInput);
            if (string.IsNullOrEmpty(queryResponse))
            {
                logger?.LogWarning("Query response was empty or waiting for user input.");
                return;
            }

            agentMessage = queryResponse;
            logger?.LogInfo("Handling query response...");
            await HandleQuery_Response(queryResponse);
        }

        private async UniTask<string> ContinueUntilUserNeeded(string id, string initialMessage)
        {

            var lastResponse = await firstAgent.ContinueThreaded_Conversation(id, initialMessage);
            if (string.IsNullOrEmpty(lastResponse))
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                logger?.LogError("Initial response from assistant was empty.");
                return null;
            }

            logger?.LogInfo($"Assistant response received: {lastResponse}");

            if (IsQueryIntentRecognized(lastResponse))
            {
                logger?.LogInfo("Query intent recognized in assistant response.");
                return lastResponse;
            }

            continueConversationEvent.Invoke();
            await OpenAITTS_System.Instance.Speak_Async(lastResponse);
            ActivateContinue_Conversation(lastResponse);
            Debug.Log($"Continue the conversation");
            
            agentMessage = lastResponse;
            return null;
        }
        
        private async UniTask HandleQuery_Response(string response)
        {
            if (!IsQueryIntentRecognized(response))
            {
                logger?.LogInfo("No known query type found in assistant response. Treating as final answer.");
                return;
            }

            string finalResponse;

            if (response.Contains("SEARCH_QUERY"))
            {
                finalResponse = await searchQueryAgent.GetAssistant_Response(response);
                logger?.LogInfo($"Search query final response: {finalResponse}");
            }
            else
            {
                finalResponse = await vergelijkingsQueryAgent.GetAssistant_Response(response);
                logger?.LogInfo($"Vergelijkings query final response: {finalResponse}");
            }

            await OpenAITTS_System.Instance.Speak_Async(dialogueSentences);
            NexiUI_System.Instance.SetDialogueText(dialogueSentences);

            await ParseAndSendToTypeSense(finalResponse);
            Activate_Hologram();
            bookFoundEvent.Invoke();
        }
        
        private async UniTask ParseAndSendToTypeSense(string jsonResponse)
        {
            logger?.LogInfo("Parsing agent response and preparing for TypeSense...");

            if (!Agents_Helper.IsJson(jsonResponse))
            { 
                logger?.LogWarning("Final response is not valid JSON.");
                return;
            }

            var searchItem = Newtonsoft.Json.Linq.JObject.Parse(jsonResponse);
            searchItem["prefix"] = false;

            var root = new Newtonsoft.Json.Linq.JObject
            {
                ["searches"] = new Newtonsoft.Json.Linq.JArray { searchItem }
            };

            var formattedJson = root.ToString();

            logger?.LogInfo($"Formatted JSON: {formattedJson}");

            // Save JSON to persistentDataPath as Typesense_response.json
            var filePath = Path.Combine(Application.persistentDataPath, "AgentResponse.json");
            try
            {
                await File.WriteAllTextAsync(filePath, formattedJson);
                logger?.LogInfo($"✅ Successfully saved JSON to: {filePath}");
            }
            catch (Exception ex)
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
               logger?.LogError($"❌ Failed to save JSON to persistentDataPath: {ex.Message}");
            }

            await typeSense.SendParsed_JsonToTypeSenseAsync(formattedJson);
            logger?.LogInfo("Response successfully sent to TypeSense.");
        }
        
        private static bool IsQueryIntentRecognized(string response) =>
            response.Contains("SEARCH_QUERY") || response.Contains("VERGELIJKINGS_QUERY");
        
        private static void ActivateContinue_Conversation(string lastResponse)
        {
            OpenAISTT_System.Instance.CanRecord = false;
            Loading_Controller.Instance.EnableLoading(false);
            NexiUI_System.Instance.SetDialogueText(lastResponse);
        }
        
        private static void Activate_Hologram()
        {
            PPNLoader_System.Instance.LoadPpn_Data();
            NexiUI_System.Instance.EnableHologram(true);
        }
    }
}
