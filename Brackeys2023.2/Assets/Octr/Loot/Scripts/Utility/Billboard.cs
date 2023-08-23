using UnityEngine;

namespace octr.Loot.Utility
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            // Get a reference to the main camera's transform
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            // Calculate the rotation needed to face the camera
            Vector3 lookDirection = _cameraTransform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(-lookDirection, Vector3.up);

            // Apply the rotation to the object
            transform.rotation = rotation;
        }
    }
}

