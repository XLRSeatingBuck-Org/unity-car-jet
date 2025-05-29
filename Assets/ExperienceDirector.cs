using System;
using System.Collections;
using TMPro;
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
    public TMP_Text statusText;

    public float loadTime = 2, menuTime = 5;
    
    private bool _fireExtinguished;

    private bool isCarScene => SceneManager.GetActiveScene().name == "Cesium Car";

    private void Awake()
    {
        Instance = this;
        
        fireBeacon.GetComponentInChildren<MeshRenderer>().enabled = true;
        homeBeacon.GetComponentInChildren<MeshRenderer>().enabled = false;
        loadingFader.alpha = 1;
        Time.timeScale = 0;
        loseGroup.alpha = 0;
        winGroup.alpha = 0;
        
        SetStatus(isCarScene ? "Leave station" : "Take off");
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private IEnumerator Start()
    {
        var osmLoader = FindAnyObjectByType<OsmLoader>();
        if (osmLoader && true/*temp*/) yield return new WaitUntil(() => osmLoader.Loaded);
        else yield return new WaitForSecondsRealtime(loadTime);
        Debug.Log("loaded!");
        loadingFader.alpha = 0;
        Time.timeScale = 1;
    }

    [ContextMenu("test extinguish")]
    public void OnFireExtinguished()
    {
        Debug.Log("extinguished!");
        
        fireBeacon.GetComponentInChildren<MeshRenderer>().enabled = false;
        homeBeacon.GetComponentInChildren<MeshRenderer>().enabled = true;
        // TODO: maybe put text or something
        // TODO: sound
        
        SetStatus("Go home (green beacon)");

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
    
    public enum LoseType { Crashed, Spread }

    public void OnLose(LoseType loseType) => StartCoroutine(_OnLose(loseType));

    private IEnumerator _OnLose(LoseType loseType)
    {
        loseGroup.alpha = 1;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(menuTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetStatus(string text)
    {
        statusText.text = $"STATUS:\n{text}";
    }

    public void OnEnterBeacon(GameObject beacon)
    {
        Debug.Log($"beacon enter {beacon}", beacon);
        if (beacon == fireBeacon && !_fireExtinguished) SetStatus("Put out fire");
        if (beacon == homeBeacon && _fireExtinguished) SetStatus(isCarScene ? "Park" : "Land");
    }
    public void OnExitBeacon(GameObject beacon)
    {
        Debug.Log($"beacon exit {beacon}", beacon);
        if (beacon == homeBeacon && !_fireExtinguished) SetStatus("Go to fire (orange beacon)");
    }
}
