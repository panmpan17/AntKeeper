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

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
        statisticProvider.Get = CollectGameStatic;
        MenuManager.ins.OpenMenu("End");
    }
}
