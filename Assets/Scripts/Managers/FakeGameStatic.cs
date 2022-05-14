using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGameStatic : MonoBehaviour, IGameStaticProvider
{
    [SerializeField]
    private GameStatic gameStatic;

    public GameStatic CollectGameStatic()
    {
        return gameStatic;
    }

    void Start()
    {
        FindObjectOfType<EndMenu>(true).Open();
    }
}
