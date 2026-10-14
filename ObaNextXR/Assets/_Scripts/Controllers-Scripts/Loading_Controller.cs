using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using UnityEngine;

namespace _Scripts.Controllers_Scripts
{
    public class Loading_Controller : Singleton_MonoBehaviour<Loading_Controller>
    {
        [SerializeField] private GameObject loadingPanel, hologramLoadingPanel;
    
        public void EnableLoading(bool enable) => loadingPanel.SetActive(enable);
        public void EnableHologramLoading(bool enable) => hologramLoadingPanel.SetActive(enable);
    }
}
