using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Scripts.Controllers_Scripts;
using _Scripts.Systems_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Newtonsoft.Json;

namespace _Scripts
{
    public class PPNLoader_System : Singleton_MonoBehaviour<PPNLoader_System>
    {
        [Header("Data Handling")]
        [SerializeField, Tooltip("Higher it goes longer takes to load in ")] 
        private float ppnResolution;

        [Header("Configuration Settings")]
        [SerializeField] private string jsonFilePath = "Typesense_response.json";  
        [SerializeField] private string baseUrl = "https://cover.biblion.nl/coverlist.dll/?doctype=morebutton&bibliotheek=oba&style=5&ppn=";

        [Header("Prefab Settings")]
        [SerializeField] private int maxPrefabsToShow;
        [SerializeField] private Transform contentParent;
        [SerializeField] private GameObject prefab;
        [SerializeField] public List<GameObject> instantiateList;
        
        public Transform ContentParent => contentParent;

        [ContextMenu("Load PPN Data")] 
        public void LoadPpn_Data()
        {
            NexiFace_System.Instance.FacePlay("NexiFace_Happy");
            OpenAISTT_System.Instance.CanRecord = false;
            Loading_Controller.Instance.EnableLoading(false);
            
            ClearInstantiated_Objects();
            instantiateList?.Clear();
            Loading_PPN().Forget();
        }

        private void ClearInstantiated_Objects()
        {
            foreach (var obj in instantiateList)
            {
#if UNITY_EDITOR
                DestroyImmediate(obj);
#else
                Destroy(obj);
#endif
            }
        }

        private async UniTaskVoid Loading_PPN()
        {
            var jsonContent = await ReadJson_File();
            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("❌ JSON content is empty or could not be read.");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return;
            }

            List<PPN_Data> ppnList;
            try
            {
                ppnList = JsonConvert.DeserializeObject<List<PPN_Data>>(jsonContent);
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to parse the TypeSense JSON response: {e.Message}");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return;
            }

            if (ppnList == null || ppnList.Count == 0)
            {
                Debug.LogError("❌ No PPNs found in the JSON file. Stopping the process.");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return;
            }

            var selectedPpns = ppnList.Take(maxPrefabsToShow).ToArray();
            var loadTasks = selectedPpns.Select(Instantiate_Ppn).ToList();

            await UniTask.WhenAll(loadTasks);

            SaveJsonToFile(jsonContent);
        }
        
        private async UniTask<string> ReadJson_File()
        {
            var filePath = Path.Combine(Application.persistentDataPath, jsonFilePath);
            if (!File.Exists(filePath))
            {
                Debug.LogError($"File not found at {filePath}");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return null;
            }

            try
            {
                using var reader = new StreamReader(filePath);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to read JSON from persistent data path: {ex.Message}");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return null;
            }
        }
        
        private async UniTask Instantiate_Ppn(PPN_Data data)
        {
            if (string.IsNullOrEmpty(data.ppn) || string.IsNullOrEmpty(data.titel)) return;
            
            var texture = await Get_Image(data.ppn);
            if (texture is null) return; 
            
            var instance = Instantiate(prefab, contentParent);
            instantiateList.Add(instance);
            instance.GetComponent<BookV2_Controller>().PpnNumber = data.ppn;

            var imageComponent = instance.transform.Find("Book Thumbnail - Image")?.GetComponent<Image>();
            var textComponent = instance.transform.Find("Book Title - TMP")?.GetComponent<TextMeshProUGUI>();

            if (imageComponent) ApplyTexture_Image(texture, imageComponent);
            if (textComponent) textComponent.text = data.titel;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            await UniTask.DelayFrame(2); 
#endif
        }

        private async UniTask<Texture2D> Get_Image(string ppn)
        {
            var fullUrl = $"{baseUrl}{ppn}&isbn=&lid=&aut=&ti=&size={ppnResolution}";
            using var request = UnityWebRequestTexture.GetTexture(fullUrl);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) return DownloadHandlerTexture.GetContent(request);
            Debug.LogWarning($"Image not found for PPN: {ppn} | Error: {request.error}");
            return null;
        }
        
        private static void ApplyTexture_Image(Texture2D texture, Image targetImage)
        {
            if (targetImage is null || !texture) return;

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = sprite;
        }
        
        private static void SaveJsonToFile(string jsonContent)
        {
            var filePath = Path.Combine(Application.persistentDataPath, "Typesense_response.json");

            try
            {
                File.WriteAllText(filePath, jsonContent);
                Debug.Log($"✅ Successfully saved JSON to: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Failed to save JSON to persistentDataPath: {ex.Message}");
                FallBack_System.Instance.Fetch_FallbackAudio();
            }
        }
    }

    [Serializable]
    public class PPN_Data
    {
        public string ppn;
        public string titel;
    }
}
