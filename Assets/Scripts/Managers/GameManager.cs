using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class GameManager : MonoBehaviour
{
    public static GameManager ins;

    [SerializeField]
    private int gameTime;

    [SerializeField]
    private PlayerBehaviour playerBehaviour;

    public event System.Action<int> GameTimeChanged;
    private Timer _oneSecondTimer = new Timer(1);

    public event System.Action OnGameStart;

    #if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipStartCountDown;
    [SerializeField]
    private ValueWithEnable<int> overrideGameTime;
    [SerializeField]
    private ValueWithEnable<float> overrideTimeScale = new ValueWithEnable<float>(1);
    #endif

    void Awake()
    {
        ins = this;

#if UNITY_EDITOR
        if (overrideGameTime.Enable)
            gameTime = overrideGameTime.Value;
#endif
    }

    void OnEnable()
    {
        playerBehaviour.Input.enabled = true;
        GameTimeChanged?.Invoke(gameTime);
        OnGameStart?.Invoke();
    }

    void Update()
    {
        if (_oneSecondTimer.UpdateEnd)
        {
            _oneSecondTimer.Reset();
            GameTimeChanged?.Invoke(--gameTime);

            if (gameTime <= 0)
            {
                StartCoroutine(GameOver());
            }
        }

    #if UNITY_EDITOR
        if (overrideTimeScale.Enable && overrideTimeScale.Value != Time.timeScale)
            Time.timeScale = overrideTimeScale.Value;
    #endif
    }

    IEnumerator GameOver()
    {
        Time.timeScale = 0;
        CameraManager.ins.SwitchCamera(CameraManager.CameraState.FullMap);
        HUDManager.ins.enabled = false;

        playerBehaviour.Input.enabled = false;

        yield return new WaitForSecondsRealtime(3);
        FindObjectOfType<EndMenu>(true).Open();
    }
}