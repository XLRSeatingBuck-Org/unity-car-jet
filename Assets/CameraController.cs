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
        
        // no vsync if in vr, yes vsync if not in vr
        QualitySettings.vSyncCount = vr ? 0 : 1;
    }
}