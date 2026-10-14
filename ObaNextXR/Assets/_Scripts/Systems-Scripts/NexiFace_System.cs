using Artem_Library.Library_Scripts.ScriptsTemplate_Scripts;
using UnityEngine;

namespace _Scripts.Systems_Scripts
{
    public class NexiFace_System : Singleton_MonoBehaviour<NexiFace_System>
    {
        [SerializeField] private Animator faceAnim;

        public void FacePlay(string faceName) => faceAnim.Play(faceName);
    }
}

