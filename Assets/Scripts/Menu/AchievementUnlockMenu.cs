using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MPack;
using TMPro;

public class AchievementUnlockMenu : MonoBehaviour
{
    [Header("Tween Timing")]
    [SerializeField]
    [Range(0, 1)]
    private float fadeTime;
    [SerializeField]
    private float onScreenTime = 4;
    [SerializeField]
    private float waitTime = 1;
    
    [Header("Reference")]
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [Header("Audio")]
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip unlockSound;

    [Header("Achievement")]
    [SerializeField]
    private AchievementItem breedInvasive;
    [SerializeField]
    private AchievementItem breedNative;

    [SerializeField]
    private AchievementItem identifyAll;

    [SerializeField]
    private AchievementItem killAll;
    [SerializeField]
    private AchievementItem killAllNative;
    [SerializeField]
    private AchievementItem killAllInvasive;

    [SerializeField]
    private AchievementItem killInvasiveTenTime;

    void Awake()
    {
        canvasGroup.alpha = 0;
    }

    public IEnumerator C_StartUnlock(GameStatic statistic)
    {
        List<AchievementItem> unlocks = new List<AchievementItem>();

        if (TestBreedInvasiveAnt(statistic)) unlocks.Add(breedInvasive);
        if (TestBreedNativeAnt(statistic)) unlocks.Add(breedNative);


        if (TestIdentifyAll(statistic)) unlocks.Add(identifyAll);


        if (TestKillAll(statistic)) unlocks.Add(killAll);
        else if (TestKillAllNative(statistic)) unlocks.Add(killAllNative);
        else if (TestKillAllInvastive(statistic)) unlocks.Add(killAllInvasive);

        if (TestKillFireAntTenTime(statistic))unlocks.Add(killInvasiveTenTime);


        // Remove already unlocks achievement
        AchievementStoreData.Read(out AchievementStoreData data);
        data.Append(unlocks);
        data.Save();


        if (unlocks.Count == 0)
        {
            Debug.Log("No achievement unlock");
            yield break;
        }

        for (int i = 0; i < unlocks.Count; i++)
        {
            yield return StartCoroutine(C_ShowAchievement(unlocks[i]));
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    IEnumerator C_ShowAchievement(AchievementItem item)
    {
        audioSource.PlayOneShot(unlockSound);

        iconImage.sprite = item.Icon;
        nameText.text = LanguageMgr.GetTextById(item.NameLanguageID);
        descriptionText.text = LanguageMgr.GetTextById(item.DescriptionLanguageID);

        Timer timer = new Timer(fadeTime);

        while (!timer.UnscaleUpdateTimeEnd)
        {
            canvasGroup.alpha = timer.Progress;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSecondsRealtime(onScreenTime);

        timer.Reset();
        while (!timer.UnscaleUpdateTimeEnd)
        {
            canvasGroup.alpha = 1 - timer.Progress;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }


    bool TestBreedInvasiveAnt(GameStatic statistic) => statistic.BreedFireAntCount >= 10;
    bool TestBreedNativeAnt(GameStatic statistic) => statistic.BreedNativeAntCount >= 10;

    bool TestIdentifyAll(GameStatic statistic)
    {
        if (statistic.NativeAnts.Length + statistic.FireAnts.Length < 3)
        {
            return false;
        }

        for (int i = 0; i < statistic.NativeAnts.Length; i++)
        {
            if (!statistic.NativeAnts[i].Revealed)
                return false;
        }

        for (int i = 0; i < statistic.FireAnts.Length; i++)
        {
            if (!statistic.FireAnts[i].Revealed)
                return false;
        }

        return true;
    }

    bool TestKillAll(GameStatic statistic) => statistic.NativeAnts.Length + statistic.FireAnts.Length == 0;
    bool TestKillAllNative(GameStatic statistic) => statistic.NativeAnts.Length == 0;
    bool TestKillAllInvastive(GameStatic statistic) => statistic.FireAnts.Length == 0;
    bool TestKillFireAntTenTime(GameStatic statistic) => statistic.BucketDestroyFireAntCount >= 10;
}
