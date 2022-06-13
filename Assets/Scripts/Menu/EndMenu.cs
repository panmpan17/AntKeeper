using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MPack;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DigitalRuby.Tween;

public class EndMenu : AbstractMenu
{
    [Header("UI Elements")]
    [SerializeField]
    private TextMeshProUGUI animalText;
    [SerializeField]
    private TextMeshProUGUI aliveNestsText;
    [SerializeField]
    private TextMeshProUGUI destroyNestsText;
    [SerializeField]
    private TextMeshProUGUI breedNestsText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private Color nativeTextColor;
    [SerializeField]
    private Color invasiveTextColor;
    private string nestTextFormat;

    [Header("Grade Calculation")]
    [SerializeField]
    private Sprite[] gradeSprites;
    [SerializeField]
    private float[] requireScore;
    [SerializeField]
    private Image[] gradeStampImages;
    [SerializeField]
    private CanvasGroup gradeStampCanvasGroup;

    [SerializeField]
    private StatisticProvider statisticProvider;
    [SerializeField]
    private AchievementUnlockMenu achievementUnlockMenu;


    protected override void Awake()
    {
        base.Awake();
        firstSelected.SetActive(false);

        nestTextFormat = string.Format(
            "<color=#{0}>{1}</color> / <color=#{2}>{3}</color>",
            ColorUtility.ToHtmlStringRGB(nativeTextColor),
            "{0}",
            ColorUtility.ToHtmlStringRGB(invasiveTextColor),
            "{1}"
        );
        animalText.text = aliveNestsText.text = destroyNestsText.text = breedNestsText.text = scoreText.text = "";
        gradeStampCanvasGroup.alpha = 0;
    }


    protected override void FadeInFinished(ITween<float> tween)
    {
        StartCoroutine(C_ShowStatic());
    }

    IEnumerator C_ShowStatic()
    {
        var statistic = statisticProvider.Get();

        var wait = new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(C_ShowAnimalCount(statistic));
        yield return wait;
        yield return StartCoroutine(C_ShowAntNestCount(statistic));
        yield return wait;
        yield return StartCoroutine(C_ShowDestroyNestsCount(statistic));
        yield return wait;
        yield return StartCoroutine(C_ShowBreedNestsCount(statistic));
        yield return wait;
        yield return StartCoroutine(C_ShowScore(statistic));
        yield return wait;
        yield return StartCoroutine(C_ShowGrade(statistic));
        yield return wait;
        yield return StartCoroutine(achievementUnlockMenu.C_StartUnlock(statistic));

        _canvasGroup.interactable = true;
        firstSelected.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }


    IEnumerator C_ShowAnimalCount(GameStatic statistic)
    {
        string right = " / " + statistic.OriginalAnimalCount.ToString();
        animalText.text = "0" + right;

        yield return null;

        for (int count = 1; count <= statistic.ResultAnimalCount; count++)
        {
            animalText.text = count + right;
            yield return null;
        }
    }

    IEnumerator C_ShowAntNestCount(GameStatic statistic)
    {
        aliveNestsText.text = string.Format(nestTextFormat, 0, 0);

        int nativeAntCount = statistic.NativeAntAliveCount;
        int fireAntCount = statistic.FireAntAliveCount;

        for (int i = 0; i <= nativeAntCount || i <= fireAntCount; i++)
        {
            aliveNestsText.text = string.Format(
                nestTextFormat,
                Mathf.Clamp(i, 0, nativeAntCount),
                Mathf.Clamp(i, 0, fireAntCount));
            yield return null;
        }
    }

    IEnumerator C_ShowDestroyNestsCount(GameStatic statistic)
    {
        destroyNestsText.text = string.Format(nestTextFormat, 0, 0);

        int nativeAntCount = statistic.BucketDestroyNativeAntCount;
        int fireAntCount = statistic.BucketDestroyFireAntCount;

        for (int i = 0; i <= nativeAntCount || i <= fireAntCount; i++)
        {
            destroyNestsText.text = string.Format(
                nestTextFormat,
                Mathf.Clamp(i, 0, nativeAntCount),
                Mathf.Clamp(i, 0, fireAntCount));
            yield return null;
        }
    }
    IEnumerator C_ShowBreedNestsCount(GameStatic statistic)
    {
        breedNestsText.text = string.Format(nestTextFormat, 0, 0);

        int nativeAntCount = statistic.BreedNativeAntCount;
        int fireAntCount = statistic.BreedFireAntCount;

        for (int i = 0; i <= nativeAntCount || i <= fireAntCount; i++)
        {
            breedNestsText.text = string.Format(
                nestTextFormat,
                Mathf.Clamp(i, 0, nativeAntCount),
                Mathf.Clamp(i, 0, fireAntCount));
            yield return null;
        }
    }
    IEnumerator C_ShowScore(GameStatic statistic)
    {
        int score = Mathf.CeilToInt(statistic.CalculateScore());
        yield return null;

        for (int count = 1; count <= score; count++)
        {
            scoreText.text = count.ToString();
            yield return null;
        }
    }
    IEnumerator C_ShowGrade(GameStatic statistic)
    {
        int score = Mathf.CeilToInt(statistic.CalculateScore());

        Sprite sprite = gradeSprites[0];
        for (int i = 0; i < requireScore.Length; i++)
        {
            if (score >= requireScore[i])
            {
                sprite = gradeSprites[i + 1];
            }
            else
                break;
        }

        for (int i = 0; i < gradeStampImages.Length; i++)
        {
            gradeStampImages[i].sprite = sprite;
        }


        var timer = new Timer(0.5f);

        while (!timer.UnscaleUpdateTimeEnd)
        {
            gradeStampCanvasGroup.alpha = timer.Progress;
            yield return null;
        }
    }

    public void OnReplayPressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TempMainMenu");
    }


    // void OnValidate()
    // {

    //     var text = string.Format(
    //         "<color=#{0}>3</color> / <color=#{1}>10</color>",
    //         ColorUtility.ToHtmlStringRGB(nativeTextColor),
    //         ColorUtility.ToHtmlStringRGB(invasiveTextColor)
    //     );
    //     aliveNestsText.text = destroyNestsText.text = breedNestsText.text = text;
    // }
}
