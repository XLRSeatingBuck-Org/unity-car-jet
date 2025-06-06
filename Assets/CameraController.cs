using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

/// <summary>
/// set up the right camera if in vr or not
/// </summary>
public class CameraController : MonoBehaviour
{
    public GameObject vrCamera, regularCamera;
    public InputActionReference switchCameraInput;

    private void Start()
    {
        var vr = XRSettings.enabled;
        vrCamera.SetActive(vr);
        regularCamera.SetActive(!vr);
        
        // no vsync if in vr, yes vsync if not in vr
        QualitySettings.vSyncCount = vr ? 0 : 1;
        
        // set ui to world space if in vr
        if (XRSettings.enabled)
        {
            var ui = FindAnyObjectByType<Canvas>();
            ui.renderMode = RenderMode.WorldSpace;
            ui.transform.parent = vrCamera.GetComponentInChildren<Camera>().transform;
            ui.transform.localPosition = new(0, 0, .5f);
            ui.transform.localRotation = Quaternion.identity;
            ui.transform.localScale = Vector3.one * .0005f;
        }
    }

    private void Update()
    {
        if (switchCameraInput.action.WasPressedThisFrame())
        {
            vrCamera.SetActive(!vrCamera.activeSelf);
            regularCamera.SetActive(!regularCamera.activeSelf);
        }
    }
}