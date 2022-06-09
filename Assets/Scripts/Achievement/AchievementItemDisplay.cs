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

    [Header("Lock")]
    [SerializeField]
    private ColorReference lockNameColor;
    [SerializeField]
    private ColorReference lockDescriptionColor;

    [Header("Hidden")]
    [SerializeField]
    private Sprite hiddenIcon;
    [SerializeField]
    [LauguageID]
    private int hiddenDescriptionID;

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
        nameText.GetComponent<GraphicColorSwitcher>().Color = lockNameColor;
        descriptioinText.ChangeId(item.DescriptionLanguageID);
        descriptioinText.GetComponent<GraphicColorSwitcher>().Color = lockDescriptionColor;
    }

    public void SetupHidden(AchievementItem item)
    {
        icon.sprite = hiddenIcon;
        nameText.ChangeId(item.NameLanguageID);
        nameText.GetComponent<GraphicColorSwitcher>().Color = lockNameColor;
        descriptioinText.ChangeId(hiddenDescriptionID);
        descriptioinText.GetComponent<GraphicColorSwitcher>().Color = lockDescriptionColor;
    }
}
