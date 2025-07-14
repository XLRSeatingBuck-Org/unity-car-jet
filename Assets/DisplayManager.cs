using UnityEngine;
 
public class DisplayManager : MonoBehaviour
{
    public Camera camera1; // Display 1 → Monitor 2
    public Camera camera2; // Display 2 → Monitor 3

 
    void Start()
    {
        // Activate all available displays
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
 
        // Unity display index mapping (0 = Monitor 1, 1 = Monitor 2, 2 = Monitor 3)
        // So remap displays like this:
        camera1.targetDisplay = 1; // Display 1 on Monitor 2
        camera2.targetDisplay = 2; // Display 2 on Monitor 3

 
        // IMPORTANT: You must also match this in each camera’s settings in the Inspector
    }
}