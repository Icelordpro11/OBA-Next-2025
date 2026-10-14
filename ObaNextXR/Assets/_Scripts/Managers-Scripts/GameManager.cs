using System;
using Artem_Library.Attribute_Scripts;
using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using Artem_Library.Library_Scripts.Systems_Scripts.Logger_System;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace _Scripts.Managers_Scripts
{
    public class GameManager : Singleton_MonoBehaviour<GameManager>
    {
        [SerializeField] private Transform rightHandTarget;
        [SerializeField] private Transform leftHandTarget;
        [SerializeField] private GameObject mauwWatchObj;
        private FSM_Controller _nexiFsmController;
        public Transform RightHandTarget => rightHandTarget;
        public Transform LeftHandTarget => leftHandTarget;
    
        public void SetNexiListening()
        {
         
            _nexiFsmController = FSM_Controller.Instance;
            _nexiFsmController.SetNexiListeningState();
        }

        public void EnableMauwWatch(bool enable)
        {
            mauwWatchObj.SetActive(enable);
            
            if (enable)
            {
                mauwWatchObj.GetComponent<HandDetection_System>().EnableHandDetection();
            }
            else
            {
                mauwWatchObj.GetComponent<HandDetection_System>().StopHandDetection();
            }
        }
    }
}