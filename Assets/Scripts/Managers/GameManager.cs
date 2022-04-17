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

    public event System.Action OnGameReady;
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

        if (overrideGameTime.Enable)
            gameTime = overrideGameTime.Value;
    }

    public void StartGame()
    {
        enabled = true;
        GameTimeChanged?.Invoke(gameTime);

        HUDManager.ins.enabled = true;

        OnGameReady?.Invoke();

        if (skipStartCountDown)
        {
            CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
            HUDManager.ins.UpdateAnimalCount();
            HUDManager.ins.HideCountDownText();
            Time.timeScale = 1;
            OnGameStart?.Invoke();
        }
        else
        {
            StartCoroutine(StartCountDown());
        }
    }

    IEnumerator StartCountDown()
    {
        var waitOneSec = new WaitForSecondsRealtime(1.2f);

        yield return new WaitForSecondsRealtime(0.5f);
        HUDManager.ins.ChangeCountDownText("Ready?");
        yield return waitOneSec;
        CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
        HUDManager.ins.ChangeCountDownText("3");

        yield return waitOneSec;
        HUDManager.ins.ChangeCountDownText("2");

        yield return waitOneSec;
        HUDManager.ins.ChangeCountDownText("1");

        yield return waitOneSec;
        HUDManager.ins.ChangeCountDownText("Start!");

        yield return waitOneSec;
        HUDManager.ins.UpdateAnimalCount();
        HUDManager.ins.HideCountDownText();
        Time.timeScale = 1;
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
        
        var player = GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerBehaviour>();
        player.Input.enabled = false;

        // Debug.Log(114);
        yield return new WaitForSecondsRealtime(3);
        // Debug.Log(116);
        FindObjectOfType<EndMenu>(true).Open();
    }
}