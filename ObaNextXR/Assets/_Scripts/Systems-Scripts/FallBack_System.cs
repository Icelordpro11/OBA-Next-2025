using System.IO;
using _Scripts.Controllers_Scripts;
using _Scripts.Managers_Scripts;
using Artem_Library.Attribute_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace _Scripts.Systems_Scripts
{
    public class FallBack_System : Singleton_MonoBehaviour<FallBack_System>
    {
        [SerializeField] private string mp3FileName;
        [SerializeField] private string fallbackTextDialogue;
        [SerializeField, ExpandableScript] private AudioSource fallbackAudio;
        [SerializeField] private UnityEvent fallbackEvent;

        [ContextMenu("Play FallBack Audio")]
        public void Fetch_FallbackAudio()
        {
            Loading_Controller.Instance.EnableLoading(false);
            NexiFace_System.Instance.FacePlay("NexiFace_Derp");
            NexiUI_System.Instance.EnableHologram(false);
            fallbackEvent.Invoke();
            NexiUI_System.Instance.EnableTextBubble(true);
            NexiUI_System.Instance.SetDialogueText(fallbackTextDialogue);
            TryPlayFallbackAudioAsync(mp3FileName, fallbackAudio).Forget();
        }
        
        private static async UniTask TryPlayFallbackAudioAsync(string filename, AudioSource audioSource)
        {
            if (string.IsNullOrEmpty(filename) || audioSource == null)
            {
                Debug.LogWarning("[FallbackAudioPlayer] Invalid filename or AudioSource is null.");
                return;
            }

            var filePath = Path.Combine(Application.streamingAssetsPath, filename);
#if UNITY_ANDROID && !UNITY_EDITOR
    string uri = filePath;
#else
            var uri = "file://" + filePath;
#endif

            if (!File.Exists(filePath) && Application.platform != RuntimePlatform.Android)
            {
                Debug.LogWarning($"[FallbackAudioPlayer] File not found at: {filePath}");
                return;
            }

            using var request = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG);
            var operation = await request.SendWebRequest().ToUniTask();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[FallbackAudioPlayer] Failed to load audio: {request.error}");
                return;
            }

            var clip = DownloadHandlerAudioClip.GetContent(request);
            if (clip == null)
            {
                Debug.LogWarning("[FallbackAudioPlayer] Loaded clip is null.");
                return;
            }

            audioSource.clip = clip;
            audioSource.Play();
        }

    }
}