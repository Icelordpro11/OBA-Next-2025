using UnityEngine;

public class ScanSpawner_Manager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    
    public void AddScanToSpatialAnchor(bool disableScan)
    {
        OVRSpatialAnchor anchor = FindAnyObjectByType<OVRSpatialAnchor>();
        
        GameObject scan = Instantiate(prefab, anchor.transform);
        scan.name = "OfficeScan";
        if (disableScan)
        {
            scan.gameObject.GetComponent<ScanAdjustment_System>().DisableRenderer();
        }
    }
}
