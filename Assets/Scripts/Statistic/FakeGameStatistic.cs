using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGameStatistic : MonoBehaviour
{
    [SerializeField]
    private GameStatic gameStatic;
    [SerializeField]
    private StatisticProvider statisticProvider;

    public GameStatic CollectGameStatic()
    {
        return gameStatic;
    }

    void Start()
    {
        statisticProvider.Get = CollectGameStatic;
        FindObjectOfType<EndMenu>(true).Open();
    }
}
