using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private Camera blackoutCam1;
    private Camera blackoutCam2;

    private bool displaysActive = false;
    private float lastKeyPressTime = 0f;
    private float doublePressThreshold = 0.4f;

    void Start()
    {
        // Create blackout cameras
        blackoutCam1 = CreateBlackoutCamera(1);
        blackoutCam2 = CreateBlackoutCamera(2);

        // Don't activate additional displays yet
        camera1.enabled = false;
        camera2.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            float timeSinceLastPress = Time.time - lastKeyPressTime;

            if (timeSinceLastPress <= doublePressThreshold)
            {
                // Double press – deactivate displays
                DeactivateDisplays();
                displaysActive = false;
            }
            else
            {
                // Single press – activate displays
                if (!displaysActive)
                {
                    ActivateDisplays();
                    displaysActive = true;
                }
            }

            lastKeyPressTime = Time.time;
        }
    }

    void ActivateDisplays()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        camera1.targetDisplay = 1;
        camera2.targetDisplay = 2;

        camera1.enabled = true;
        camera2.enabled = true;

        blackoutCam1.enabled = false;
        blackoutCam2.enabled = false;
    }

    void DeactivateDisplays()
    {
        camera1.enabled = false;
        camera2.enabled = false;

        blackoutCam1.enabled = true;
        blackoutCam2.enabled = true;
    }

    Camera CreateBlackoutCamera(int displayIndex)
    {
        GameObject camObj = new GameObject("BlackoutCamera_Display" + displayIndex);
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.cullingMask = 0; // Don't render any layers
        cam.targetDisplay = displayIndex;
        cam.enabled = false; // Start disabled
        DontDestroyOnLoad(camObj); // Optional: keep it across scenes
        return cam;
    }
}
