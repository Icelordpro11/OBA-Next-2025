using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Controllers_Scripts
{
    public class Bunny_Controller : MonoBehaviour
    {
        [SerializeField] private List<string> animationsStates;
        [SerializeField] private Animator animator;

        public void RandomAnimation()
        {
            var randomAnim = animationsStates[Random.Range(0, animationsStates.Count)];
            animator.SetTrigger(randomAnim);
        }
    }
}

