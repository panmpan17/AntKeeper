using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;


[CreateAssetMenu]
public class AchievementItem : ScriptableObject
{
    public string ID;
    public Sprite Icon;
    public Sprite LockIcon;
    public bool Hidden;

    [LauguageID]
    public int NameLanguageID;
    [LauguageID]
    public int DescriptionLanguageID;
}
