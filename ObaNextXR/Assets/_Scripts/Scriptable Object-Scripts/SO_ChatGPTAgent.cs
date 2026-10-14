using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Scripts.Systems_Scripts;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Scriptable_Object_Scripts
{
    [CreateAssetMenu(fileName = "ChatGPTAgent", menuName = "AI/ChatGPTAgent")]
    public class SO_ChatGPTAgent : ScriptableObject
    {
        [Header("OpenAI Config")]
        [SerializeField] private string apiKey;
        [SerializeField] private string assistantId;
        [SerializeField] private float responseTimeout = 30f;
        
        public async UniTask<string> GetAssistant_Response(string prompt)
        {
            var threadId = await Create_Thread();
            if (string.IsNullOrEmpty(threadId)) return null;

            var posted = await Post_Message(threadId, prompt);
            if (!posted) return null;

            var runId = await Run_Assistant(threadId);
            if (string.IsNullOrEmpty(runId)) return null;

            return await FetchLatest_Message(threadId);
        }

        public async UniTask<string> Create_Thread()
        {
            var json = await Send_Request("https://api.openai.com/v1/threads", "POST", "{}");
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<ThreadResponse>(json).id;
        }

        private async UniTask<bool> Post_Message(string threadId, string prompt)
        {
            var url = $"https://api.openai.com/v1/threads/{threadId}/messages";
            var payload = JsonConvert.SerializeObject(new { role = "user", content = prompt });
            var response = await Send_Request(url, "POST", payload);
            return !string.IsNullOrEmpty(response);
        }

        private async UniTask<string> Run_Assistant(string threadId)
        {
            var url     = $"https://api.openai.com/v1/threads/{threadId}/runs";
            var payload = JsonConvert.SerializeObject(new { assistant_id = assistantId });
            var resp    = await Send_Request(url, "POST", payload);
            if (string.IsNullOrEmpty(resp)) return null;

            var runId     = JsonConvert.DeserializeObject<RunResponse>(resp).id;
            var startTime = Time.time;

            while (Time.time - startTime < responseTimeout)
            {
                // 1) Check status
                var statusJson = await Send_Request($"{url}/{runId}", "GET");
                var status     = JsonConvert.DeserializeObject<RunResponse>(statusJson)?.status;
                if (status == "completed")
                    return await FetchLatest_Message(threadId);

                // 2) Yield until next frame (~16 ms on 60 Hz)
                await UniTask.Yield();
            }

            Debug.LogError("Assistant timed out.");
            FallBack_System.Instance.Fetch_FallbackAudio();
            return null;
        }
        
        private async UniTask<string> FetchLatest_Message(string threadId)
        {
            var url = $"https://api.openai.com/v1/threads/{threadId}/messages";
            var response = await Send_Request(url, "GET");

            if (string.IsNullOrEmpty(response)) return null;

            var messages = JsonConvert.DeserializeObject<MessageResponse>(response);
            return messages.data?
                .Where(m => m.role == "assistant")
                .OrderByDescending(m => m.createdAt)
                .FirstOrDefault()?.content.FirstOrDefault()?.text.value;
        }

        private async UniTask<string> Send_Request(string url, string method, string payload = null)
        {
            using var request = new UnityWebRequest(url, method);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");

            if (!string.IsNullOrEmpty(payload))
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));

            await request.SendWebRequest();

            return request.result == UnityWebRequest.Result.Success ? request.downloadHandler.text : null;
        }
        
        public async UniTask<string> ContinueThreaded_Conversation(string threadId, string message)
        {
            var posted = await Post_Message(threadId, message);
            if (!posted) return null;

            var runId = await Run_Assistant(threadId);
            if (string.IsNullOrEmpty(runId)) return null;

            return await FetchLatest_Message(threadId);
        }
        
        [Serializable] private class ThreadResponse { public string id; }
        [Serializable] private class RunResponse { public string id; public string status; }
        [Serializable] private class MessageResponse { public List<Message> data; }
        [Serializable] private class Message { public string role; public long createdAt; public List<MessageContent> content; }
        [Serializable] private class MessageContent { public MessageText text; }
        [Serializable] private class MessageText { public string value; }
    }
}