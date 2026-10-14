using System;
using UnityEngine;

namespace _Scripts
{
    public class ResetTransform_System : MonoBehaviour
    {
        private Vector3 _startPos;
        private Quaternion _startRot;
        private void OnEnable()
        {
            _startPos = transform.position;
            _startRot = transform.rotation;
        }

        public void ResetPosition() => transform.position = _startPos;
       public void ResetRotation() => transform.rotation = _startRot;
        public void ResetCustomPosition(Transform customPos) => transform.position = customPos.position;
        public void ResetCustomRotation(Transform customPos) => transform.rotation = customPos.rotation;
       
        private Rigidbody _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void ResetVelocity() => _rigidbody.linearVelocity = Vector3.zero;
    }
}