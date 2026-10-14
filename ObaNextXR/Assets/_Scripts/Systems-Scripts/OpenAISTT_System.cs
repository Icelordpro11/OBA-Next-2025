using System;
using System.IO;
using _Scripts.Controllers_Scripts;
using _Scripts.Managers_Scripts;
using _Scripts.Systems_Scripts;
using Artem_Library.Attribute_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts
{
    public class OpenAISTT_System : Singleton_MonoBehaviour<OpenAISTT_System>
    {
        [Header("OpenAI Config")]
        [SerializeField] private string apiKey;

        [Header("Recording Settings")] 
        [SerializeField, BoolConverter] private bool isRecording, isStoped;

        [field: SerializeField, BoolConverter]  public bool CanRecord { get; set; }

        private string _savedFilePath;
        private string _lastResponse;
        private const int SampleRate = 16000;
        private AudioClip _audioClip;

        [ContextMenu("🎤 Stop Conversation")]
        public void StopConversation()
        {
            CanRecord = false;
            _lastResponse = null;
            Assistants_Manager.Instance.StopConversation();
        }

        [ContextMenu("🎤 Start")]
        public void StartRecording()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("🚫 No microphone detected!");
                return;
            }

            Debug.Log("🎙️ Recording started...");
            
            isRecording = true;
            if (!CanRecord && !isStoped) _audioClip = Microphone.Start(null, false, 1000, SampleRate);
        }

        [ContextMenu("🎤 Stop")]
        public void StopRecording()
        {
            Debug.Log("🛑 Recording stopped.");
            isRecording = false;

            if (CanRecord) return;
            StopRecordingWithDelay().Forget();
        }

        private async UniTask StopRecordingWithDelay()
        {
            isStoped = true;
            await UniTask.Delay(2000);
            isStoped = false;
            if (!isRecording)
            {
                Debug.Log("Official recording stopped.");
                Microphone.End(null);
                CanRecord = true;
                NexiFace_System.Instance.FacePlay("NexiFace_Loading");
                Loading_Controller.Instance.EnableLoading(true);
                NexiUI_System.Instance.EnableTextBubble(false);
                await RecordAndSend();
            }
        }

        private async UniTask RecordAndSend()
        {
            _savedFilePath = Path.Combine(Application.persistentDataPath, "recording_dutch.wav");
            Save_ClipAsWav(_audioClip, _savedFilePath);
            Debug.Log($"💾 Audio saved to: {_savedFilePath}");

            await TranscribeAudio(_savedFilePath);

            // ✅ Only continue if the transcription was successful
            if (string.IsNullOrWhiteSpace(_lastResponse))
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                Debug.LogWarning("⚠️ No valid response from transcription, assistant chain will not be triggered.");
                return;
            }
            
            GameManager.Instance.EnableMauwWatch(false);

            if (string.IsNullOrEmpty(Assistants_Manager.Instance.ThreadId))
            {
                NexiUI_System.Instance.EnableHologram(false);
                await Assistants_Manager.Instance.RunAssistantChain(_lastResponse);
            }
            else
            {
                await Assistants_Manager.Instance.ContinueWithUserInput(_lastResponse);
            }
        }


        private void Save_ClipAsWav(AudioClip clip, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? string.Empty);
            var wavData = WavUtility_Helper.FromAudioClip(clip, out _, true);
            File.WriteAllBytes(filePath, wavData);
        }

        private async UniTask TranscribeAudio(string filePath)
        {
            if (!File.Exists(filePath))
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                return;
            }

            var audioData = await File.ReadAllBytesAsync(filePath);
            var form = new WWWForm();
            form.AddBinaryData("file", audioData, "recording_dutch.wav", "audio/wav");
            form.AddField("model", "whisper-1");
            form.AddField("language", "nl");

            using var request = UnityWebRequest.Post("https://api.openai.com/v1/audio/transcriptions", form);
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    _lastResponse = null;
                    FallBack_System.Instance.Fetch_FallbackAudio();
                    return;
                }

                var responseJson = request.downloadHandler.text;
                _lastResponse = responseJson;
                Debug.Log("📄 Dutch Transcription Result:\n" + responseJson);
            }
            catch (Exception ex)
            {
                _lastResponse = null;
                Debug.LogError("🔥 Exception during transcription request:\n" + ex.Message);
            }
        }
    }
}
