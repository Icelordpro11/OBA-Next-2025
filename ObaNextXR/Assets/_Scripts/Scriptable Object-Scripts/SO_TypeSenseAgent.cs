using System;
using System.Text;
using _Scripts.Systems_Scripts;
using Artem_Library.Attribute_Scripts;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Scriptable_Object_Scripts
{
    [CreateAssetMenu(fileName = "TypeSenseAgent", menuName = "AI/TypeSenseAgent", order = 0)]
    public class SO_TypeSenseAgent : ScriptableObject
    {
        [Header("TypeSense Config")]
        [SerializeField] private string apiKey = "your-api-key-here";
        [SerializeField] private string typeSenseUrl = "http://your-typesense-url/collections/your-collection-name/documents";

        public async UniTask SendParsed_JsonToTypeSenseAsync(string json)
        {
            if (Agents_Helper.IsJson(json))
            {
                try
                {
                    var parsedJson = JsonConvert.DeserializeObject(json);
                    var prettyJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                    Debug.Log("Sending parsed JSON to TypeSense:\n" + prettyJson);
                    await PostToTypeSense_Async(json);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing JSON before sending to TypeSense: " + ex.Message);
                }
            }
            else
            {
                FallBack_System.Instance.Fetch_FallbackAudio();
                Debug.LogWarning("Provided string is not a valid JSON format. Skipping send.");
            }
        }

        private async UniTask PostToTypeSense_Async(string json)
        {
            using var request = new UnityWebRequest(typeSenseUrl, "POST");
            var jsonToSend = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-TYPESENSE-API-KEY", apiKey);

            await request.SendWebRequest().ToUniTask();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseJson = request.downloadHandler.text;
                Debug.Log("Successfully sent data to TypeSense.");

                try
                {
                    var slimData = ExtractEssential_Fields(responseJson);
                    var slimJson = JsonConvert.SerializeObject(slimData, Formatting.Indented);
                    Agents_Helper.SaveJsonToFile(slimJson, "Typesense_response.json");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error filtering TypeSense response: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError($"Error sending data to TypeSense: {request.responseCode} - {request.error}\n{request.downloadHandler.text}");
            }
        }

        private static object ExtractEssential_Fields(string json)
        {
            var resultList = new System.Collections.Generic.List<object>();
            var seenEntries = new System.Collections.Generic.HashSet<string>();

            var root = JObject.Parse(json);

            if (root["results"]?[0]?["hits"] is not JArray hits)
            {
                Debug.LogWarning("No hits found in the JSON response.");
                return resultList;
            }

            foreach (var hit in hits)
            {
                var document = hit["document"];
                if (document == null) continue;

                var ppn = document["ppn"]?.ToString()?.Trim();
                var titel = document["titel"]?.ToString()?.Trim();
                var auteur = document["auteur"]?.ToString()?.Trim();
                var taal = document["taal"]?.ToString()?.Trim();
                var plaatskenmerk = document["plaatskenmerk"]?.ToString()?.Trim();

                if (string.IsNullOrEmpty(ppn) || string.IsNullOrEmpty(titel) || string.IsNullOrEmpty(auteur))
                    continue;

                // Composite key to detect duplicates
                var entryKey = $"{ppn}|{titel}|{auteur}";
                if (!seenEntries.Add(entryKey)) continue;

                resultList.Add(new
                {
                    ppn,
                    titel,
                    auteur,
                    taal,
                    plaatskenmerk
                });
            }

            return resultList;
        }

    }
}