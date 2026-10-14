using Meta.XR.EnvironmentDepth;
using UnityEngine;

public class Occulsiontoggle : MonoBehaviour
{
    [SerializeField] private EnvironmentDepthManager _environmentDepthManager;

    public void ToggleOcculsion()
    {
        switch (_environmentDepthManager.OcclusionShadersMode)
        {
            case OcclusionShadersMode.None:
                _environmentDepthManager.OcclusionShadersMode = OcclusionShadersMode.SoftOcclusion;
                break;
            case OcclusionShadersMode.SoftOcclusion:
                _environmentDepthManager.OcclusionShadersMode = OcclusionShadersMode.HardOcclusion;
                break;
            case OcclusionShadersMode.HardOcclusion:
                _environmentDepthManager.OcclusionShadersMode = OcclusionShadersMode.None;
                break;
        }
    }
}
