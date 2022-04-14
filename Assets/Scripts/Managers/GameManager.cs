using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;

    [SerializeField]
    private int gameTime;

    public event System.Action<int> GameTimeChanged;
    private Timer _oneSecondTimer = new Timer(1);

    #if UNITY_EDITOR
    [SerializeField]
    [Range(0, 10f)]
    private float timeScale = 1;
    #endif

    void Awake()
    {
        ins = this;
    }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        GameTimeChanged.Invoke(gameTime);
    }

    void Update()
    {
        if (_oneSecondTimer.UpdateEnd)
        {
            _oneSecondTimer.Reset();
            GameTimeChanged?.Invoke(--gameTime);
        }

    #if UNITY_EDITOR
        if (timeScale != Time.timeScale)
            Time.timeScale = timeScale;
    #endif
    }
}
