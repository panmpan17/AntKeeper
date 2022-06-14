using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTutorialStep : MonoBehaviour
{
    public abstract void StartStep();
    public abstract void Skip();
    public abstract void SkipWithoutCallNext();

    protected IEnumerator C_WaitSecond(float second, System.Action eventAction)
    {
        yield return new WaitForSeconds(second);
        eventAction.Invoke();
    }

    protected IEnumerator C_WaitSecondRealtime(float second, System.Action eventAction)
    {
        yield return new WaitForSecondsRealtime(second);
        eventAction.Invoke();
    }
}
