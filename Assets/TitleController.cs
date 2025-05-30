using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    public void CarButtonPressed() => SceneManager.LoadScene(1);
    public void PlaneButtonPressed() => SceneManager.LoadScene(2);
}