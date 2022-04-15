using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MPack;


public class HUDManager : MonoBehaviour
{
    public static HUDManager ins;

    [SerializeField]
    private TextMeshProUGUI timeText;

    [Header("Warn")]
    [SerializeField]
    private Color warnColor;
    [SerializeField]
    private int warningTime;
    private bool _startWarning;

    [Header("Danger")]
    [SerializeField]
    private Color dangerColor;
    [SerializeField]
    private int dangerTime;
    [SerializeField]
    private float rotateRange;
    [SerializeField]
    private int rotateCount;
    [SerializeField]
    [ShortTimer]
    private Timer rotateTimer;
    private bool _startDanger;
    private int _rotateCount;

    [SerializeField]
    [Space(10)]
    [Header("Animal")]
    private TextMeshProUGUI animalCountText;

    void Awake()
    {
        ins = this;
    }

    void Start()
    {
        GameManager.ins.GameTimeChanged += OnGameTimeChange;
        UpdateAnimalCount();
    }

    void Update()
    {
        if (rotateTimer.Running)
        {
            if (rotateTimer.UpdateEnd)
            {
                if (++_rotateCount >= rotateCount)
                {
                    rotateTimer.Running = false;
                    timeText.transform.rotation = Quaternion.identity;
                }
                else
                {
                    timeText.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-rotateRange, rotateRange));
                    rotateTimer.Reset();
                }
            }
        }
    }

    void OnGameTimeChange(int gameTime)
    {
        if (!_startWarning && gameTime <= warningTime)
        {
            _startWarning = true;
            timeText.color = warnColor;
        }

        if (!_startDanger)
        {
            if (gameTime <= dangerTime)
            {
                _startDanger = true;
                timeText.color = dangerColor;
                rotateTimer.Reset();
            }
        }
        else
        {
            _rotateCount = 0;
            rotateTimer.Reset();
        }

        var minute = gameTime / 60;
        var second = gameTime % 60;
        timeText.text = string.Format("Time {0}:{1}", minute, second.ToString("D2"));
    }

    public void UpdateAnimalCount()
    {
        animalCountText.text = string.Format("Animal Count {0}", GridManager.ins.CountAliveAnimal());
    }
}
