using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu]
public class PromptItem : ScriptableObject
{
    public Sprite Icon;
    [LauguageID]
    public int DescriptionLanguageID;
    public float DisplayDuration;

    public void Show()
    {
        PromptDisplay.ins.Display(this);
    }
}
