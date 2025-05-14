using UnityEngine;

public class EnableExteng : MonoBehaviour
{
    // Array to hold references to 8 GameObjects
    public GameObject[] particleObjects = new GameObject[8];

    // Update is called once per frame
    void Update()
    {
        // Enable all objects when 'P' is pressed down
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (GameObject obj in particleObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        // Disable all objects when 'P' is released
        if (Input.GetKeyUp(KeyCode.P))
        {
            foreach (GameObject obj in particleObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}