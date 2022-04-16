using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractHoldItem : MonoBehaviour
{
    public PlayerBehaviour PlayerBehaviour { get; protected set; }

    public void Setup(PlayerBehaviour playerBehaviour)
    {
        PlayerBehaviour = playerBehaviour;
    }

    public abstract void OnInteract();

    public abstract void OnInteractStart();
    public abstract void OnInteractEnd();

    public abstract void OnFacingChanged();
    public abstract void OnDash();

    public abstract void ChangeRendererSorting(int layerID, int order);
}
