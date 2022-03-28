using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractHoldItem : MonoBehaviour
{
    public PlayerBehaviour PlayerBahviour { get; protected set; }

    public void Setup(PlayerBehaviour playerBehaviour)
    {
        PlayerBahviour = playerBehaviour;
    }

    public abstract void OnInteract();

    public abstract void OnInteractStart();
    public abstract void OnInteractEnd();

    public abstract void OnDash();
}
