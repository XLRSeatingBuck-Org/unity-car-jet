using UnityEngine;
 
public class DisplayManager : MonoBehaviour
{
    // public Camera camera1; 
    // public Camera camera2; 

 
    void Start()
    {
        // Activate all available displays
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        
        // // remap displays like this:
        // camera1.targetDisplay = 1; //
        // camera2.targetDisplay = 2; //
    }
}