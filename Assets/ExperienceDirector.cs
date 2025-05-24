using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// controls the main experience (moving beacons around, displaying text maybe, etc)
/// </summary>
public class ExperienceDirector : MonoBehaviour
{
    public static ExperienceDirector Instance;
    
    public GameObject fireBeacon, homeBeacon;
    public CanvasGroup loadingFader;
    public CanvasGroup loseGroup, winGroup;

    public float loadTime = 2, menuTime = 5;
    
    private bool _fireExtinguished;

    private void Awake()
    {
        Instance = this;
        
        fireBeacon.SetActive(true);
        homeBeacon.SetActive(false);
        loadingFader.alpha = 1;
        Time.timeScale = 0;
        loseGroup.alpha = 0;
        winGroup.alpha = 0;
    }

    private IEnumerator Start()
    {
        var osmLoader = FindAnyObjectByType<OsmLoader>();
        if (osmLoader && false/*temp*/) yield return new WaitUntil(() => osmLoader.Loaded);
        else yield return new WaitForSecondsRealtime(loadTime);
        Debug.Log("loaded!");
        loadingFader.alpha = 0;
        Time.timeScale = 1;
    }

    [ContextMenu("test extinguish")]
    public void OnFireExtinguished()
    {
        fireBeacon.SetActive(false);
        homeBeacon.SetActive(true);
        // TODO: maybe put text or something
        // TODO: sound

        _fireExtinguished = true;
    }

    private void Update()
    {
        if (_fireExtinguished && CrashController.Instance.StoppedAtHome)
        {
            StartCoroutine(OnWin());
        }
    }

    private IEnumerator OnWin()
    {
        winGroup.alpha = 1;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(menuTime);
        Application.Quit();
    }

    public void OnCrash()
    {
        StartCoroutine(_OnCrash());
    }

    private IEnumerator _OnCrash()
    {
        loseGroup.alpha = 1;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(menuTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
