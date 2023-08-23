using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CustomCameraControl : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;
    private bool canRotate = false;

    private void Awake()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        if (freeLookCamera)
        {
            CinemachineCore.GetInputAxis = HandleAxisInputOverride;
        }
    }

    private float HandleAxisInputOverride(string axisName)
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

    private void Update()
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

    private void OnDestroy()
    {
        // Reset the delegate to avoid potential problems elsewhere
        CinemachineCore.GetInputAxis = Input.GetAxis;
    }
}
