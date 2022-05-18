using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractHoldItem : MonoBehaviour
{
    public PlayerBehaviour PlayerBehaviour { get; protected set; }

    public virtual void OnPickUpByHand(PlayerBehaviour playerBehaviour)
    {
        PlayerBehaviour = playerBehaviour;
    }

    public void OnPlaceToGround(AbstractGroundInteractive groundInteractive)
    {
        PlayerBehaviour.ProgressBar.gameObject.SetActive(false);
        groundInteractive.PlaceDownItem(this);
        PlayerBehaviour = null;
    }

    public virtual bool CanPlaceDownToGrounInteractive(out AbstractGroundInteractive groundInteractive)
    {
        if (GridManager.ins.TryFindGroundInteractive(PlayerBehaviour.SelectedGridPosition, out groundInteractive))
        {
            return groundInteractive.CanItemPlaceDown(this);
        }
        return false;
    }
    public abstract void OnInteractStart();
    public abstract void OnInteractEnd();

    public abstract void OnSelectedGridChanged();
    public abstract void OnFacingChanged();
    public abstract void OnDash();

    public abstract void ChangeRendererSorting(int layerID, int order);
}
