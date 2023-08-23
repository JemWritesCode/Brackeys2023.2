using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace _Game
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CustomCameraControl : MonoBehaviour
    {
        protected CinemachineFreeLook freeLookCamera;
        protected bool canRotate = false;

        protected virtual void Awake()
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
            if (freeLookCamera)
            {
                CinemachineCore.GetInputAxis = HandleAxisInputOverride;
            }
        }

        protected virtual float HandleAxisInputOverride(string axisName)
        {
            if (axisName == "Mouse X")
            {
                if (canRotate)
                    return Input.GetAxis("Mouse X");
                else
                    return 0f;
            }
            else if (axisName == "Mouse Y")
            {
                if (canRotate)
                    return Input.GetAxis("Mouse Y");
                else
                    return 0f;
            }

            return Input.GetAxis(axisName); // For other axes, return default value
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                canRotate = true;
            }

            if (Input.GetMouseButtonUp(1))
            {
                canRotate = false;
            }
        }

        protected virtual void OnDestroy()
        {
            // Reset the delegate to avoid potential problems elsewhere
            CinemachineCore.GetInputAxis = Input.GetAxis;
        }
    }
}
