using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using _Scripts.Systems_Scripts;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _Scripts.Scriptable_Object_Scripts
{
    [CreateAssetMenu(fileName = "ObaAgent", menuName = "AI/ObaAgent")]
    public class SO_ObaAgent : ScriptableObject
    {
        [Header("OBA Config")]
        [SerializeField] private string publicKey = "03b058d877ec4276bb63dd1c6e1f3768";
        
        public async UniTask FetchAllDataAsync(string ppn)
        {
            try
            {
                var resolverUrl = $"https://lab.aquabrowser.com/oba/api/v1/resolver/ppn/?id={ppn}&authorization={publicKey}";
                var resolverXml = await GetHttpStringAsync(resolverUrl);

                var id = ExtractIdFromResolverXml(resolverXml);
                if (string.IsNullOrEmpty(id))
                {
                    Debug.LogError("❌ No ID found for PPN.");
                    FallBack_System.Instance.Fetch_FallbackAudio();
                    return;
                }

                var availabilityUrl = $"https://zoeken.oba.nl/api/v1/availability/?id=%7Coba-catalogus%7C{id}&authorization={publicKey}";
                var availabilityXml = await GetHttpStringAsync(availabilityUrl);

                var data = ExtractFullData(availabilityXml);
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);

                Agents_Helper.SaveJsonToFile(json, $"ObaAgentFullResponse.json");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Exception: {e.Message}");
                FallBack_System.Instance.Fetch_FallbackAudio();
            }
        }

        private static async UniTask<string> GetHttpStringAsync(string url)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static string ExtractIdFromResolverXml(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                var itemIdElement = doc.Descendants("itemid").FirstOrDefault();
                return itemIdElement?.Value.Trim().Replace("|oba-catalogus|", "");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to parse resolver XML: {e.Message}");
                FallBack_System.Instance.Fetch_FallbackAudio();
                return null;
            }
        }

        private static List<LocationItem> ExtractFullData(string xml)
        {
            var result = new List<LocationItem>();
            try
            {
                var doc = XDocument.Parse(xml);
                foreach (var location in doc.Descendants("location"))
                {
                    var holding = location.Element("holding");
                    var address = holding?.Element("address");
                    var items = location.Element("items")?.Elements("item") ?? Enumerable.Empty<XElement>();

                    result.AddRange(items.Select(item => new LocationItem
                    {
                        locationName = location.Attribute("name")?.Value,
                        locationId = location.Attribute("id")?.Value,
                        available = location.Attribute("available")?.Value == "true",
                        shelfmark = item.Element("shelfmark")?.Value?.Trim(),
                        status = item.Element("status")?.Value?.Trim(),
                        subloc = item.Element("subloc")?.Value?.Trim(),
                        returndate = item.Element("returndate")?.Value?.Trim(),
                        publication = item.Element("publication")?.Value?.Trim(),
                        publisher = item.Element("publication")?.Attribute("publisher")?.Value,
                        placehold = item.Element("placehold")?.Value?.Trim(),
                        street = address?.Element("street")?.Value,
                        number = address?.Element("number")?.Value,
                        city = address?.Element("city")?.Value,
                        postcode = address?.Element("postcode")?.Value,
                        phone = address?.Element("phone")?.Value,
                        mail = address?.Element("mail")?.Value,
                        url = address?.Element("url")?.Value
                    }));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Failed to parse availability XML: {e.Message}");
                FallBack_System.Instance.Fetch_FallbackAudio();
            }

            return result;
        }

        [Serializable]
        public class LocationItem
        {
            public string locationName;
            public string locationId;
            public bool available;
            public string shelfmark;
            public string status;
            public string subloc;
            public string returndate;
            public string publication;
            public string publisher;
            public string placehold;
            public string street;
            public string number;
            public string city;
            public string postcode;
            public string phone;
            public string mail;
            public string url;
        }
    }
}
