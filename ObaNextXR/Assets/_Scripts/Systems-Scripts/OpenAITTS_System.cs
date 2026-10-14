using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using _Scripts;
using _Scripts.Systems_Scripts;
using Artem_Library.Attribute_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class OpenAITTS_System : Singleton_MonoBehaviour<OpenAITTS_System>
{
    [Header("OpenAI Config")]
    [SerializeField] private string apiKey;
    [SerializeField] private Voice_Settings voiceSettings = new ();

    [Header("Audio Output")]
    [SerializeField, ExpandableScript] private AudioSource audioSource;

    [Header("Test Input")]
    [TextArea(3, 5)] [SerializeField] private string testText = "Hi!";

    [ContextMenu("🔊 Test TTS")]
    public void TestSpeak() => Speak_Async(testText).Forget();

    public async UniTask Speak_Async(string input)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogError("🛑 API Key is missing!");
            return;
        }

        var payload = new TtsRequest_Payload
        {
            model = "tts-1",
            input = input,
            voice = voiceSettings.VoiceName.ToLower(),
            responseFormat = "mp3",
            speed = voiceSettings.speed
        };

        var json = JsonUtility.ToJson(payload);

        using var request = new UnityWebRequest("https://api.openai.com/v1/audio/speech", "POST");
        var postData = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ TTS request failed: " + request.error);
            return;
        }

        var mp3Data = request.downloadHandler.data;

        var textCheck = Encoding.UTF8.GetString(mp3Data, 0, Mathf.Min(mp3Data.Length, 150));
        if (textCheck.StartsWith("{") && textCheck.Contains("error"))
        {
            Debug.LogError("🛑 JSON Error from OpenAI:\n" + textCheck);
            return;
        }

        const string fileName = "tts_output.mp3";
        var savePath = Path.Combine(Application.persistentDataPath, fileName);
        await File.WriteAllBytesAsync(savePath, mp3Data);
        Debug.Log($"✅ Saved MP3 to: {savePath}");

        await Task.Delay(500);

        try
        {
            await PlayAudio(savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("🚨 Failed to load audio: " + e.Message);
        }
    }

    private async UniTask PlayAudio(string filePath)
    {
        try
        {
            UnityWebRequest www;  

#if UNITY_ANDROID && !UNITY_EDITOR
        string fileUri = "file://" + filePath;
        www = UnityWebRequestMultimedia.GetAudioClip(fileUri, AudioType.MPEG);
#else
            var fileUri = "file://" + filePath;
            www = UnityWebRequestMultimedia.GetAudioClip(fileUri, AudioType.MPEG);
#endif

            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ Failed to load audio: " + www.error);
                return;
            }

            var clip = DownloadHandlerAudioClip.GetContent(www);

            if (clip == null || clip.length == 0f)
            {
                Debug.LogError("❌ AudioClip is null or empty!");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
            NexiFace_System.Instance.FacePlay("NexiFace_DefaultTalking");
            NexiUI_System.Instance.EnableTextBubble(true);
            Debug.Log("🔊 Playback started.");
        }
        catch (Exception e)
        {
            Debug.LogError("🚨 Error playing audio: " + e.Message);
        }
    }

    [Serializable]
    private class TtsRequest_Payload
    {
        public string model;
        public string input;
        public string voice;
        public string responseFormat;
        public float speed;
    }

    [Serializable]
    public class Voice_Settings
    {
        public Voice_Name voice = OpenAITTS_System.Voice_Name.Nova;
        [Range(0.25f, 4f)] public float speed = 1f;

        public string VoiceName => voice.ToString().ToLower();
    }

    public enum Voice_Name
    {
        Alloy,
        Echo,
        Fable,
        Onyx,
        Nova,
        Shimmer
    }
}
