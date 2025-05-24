using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    public GameObject vrCamera, regularCamera;

    private void Awake()
    {
        var vr = XRSettings.enabled || false/*temp*/;
        vrCamera.SetActive(vr);
        regularCamera.SetActive(!vr);
    }
}