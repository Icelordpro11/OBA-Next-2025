using System.Collections.Generic;
using UnityEngine;


public class RandomMaterial_Controller : MonoBehaviour
{

    [SerializeField] private List<Material> listMaterials;
    [SerializeField] private new Renderer renderer;

    public void RandomMaterial()
    {
        if (listMaterials == null || listMaterials.Count == 0) return;

        var currentMaterial = renderer.sharedMaterial;
        var filteredList = listMaterials.FindAll(mat => mat != currentMaterial);

        if (filteredList.Count == 0) return;

        var newMat = filteredList[Random.Range(0, filteredList.Count)];
        renderer.material = newMat;
    }
}

