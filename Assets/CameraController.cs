using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    public GameObject vrCamera, regularCamera;

    private void Awake()
    {
        vrCamera.SetActive(XRSettings.enabled);
        regularCamera.SetActive(!XRSettings.enabled);
    }
}