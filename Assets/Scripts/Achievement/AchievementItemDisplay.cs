using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MPack;

public class AchievementItemDisplay : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private LanguageText nameText;
    [SerializeField]
    private LanguageText descriptioinText;

    public void SetupUnlock(AchievementItem item)
    {
        icon.sprite = item.Icon;
        nameText.ChangeId(item.NameLanguageID);
        descriptioinText.ChangeId(item.DescriptionLanguageID);
    }

    public void SetupLock(AchievementItem item)
    {
        icon.sprite = item.LockIcon;
        nameText.ChangeId(item.NameLanguageID);
        descriptioinText.ChangeId(item.DescriptionLanguageID);
    }

    public void SetupHidden(AchievementItem item, Sprite hiddenIcon, int hiddenDescriptionID)
    {
        icon.sprite = hiddenIcon;
        nameText.ChangeId(item.NameLanguageID);
        descriptioinText.ChangeId(hiddenDescriptionID);
    }
}
