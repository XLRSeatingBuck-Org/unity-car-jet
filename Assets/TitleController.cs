using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// handles ui logic for start scene
/// </summary>
public class TitleController : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 1; // force vsync on for now
    }

    public void CarButtonPressed() => SceneManager.LoadScene(1);
    public void PlaneButtonPressed() => SceneManager.LoadScene(2);
}