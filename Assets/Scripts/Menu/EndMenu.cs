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
    [SerializeField]
    private AnimationCurveReference numberIncreaseCurve;
    [SerializeField]
    private Timer numberInrcreaseTimer = new Timer(0.4f);

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

        yield return wait;
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

        int count = statistic.ResultAnimalCount;

        yield return StartCoroutine(C_NumberIncrease((progress) =>
        {
            animalText.text = (Mathf.FloorToInt(count * progress)).ToString() + right;
        }));
    }

    IEnumerator C_ShowAntNestCount(GameStatic statistic)
    {
        int nativeAntCount = statistic.NativeAntAliveCount;
        int fireAntCount = statistic.FireAntAliveCount;

        yield return StartCoroutine(C_NumberIncrease((progress) =>
        {
            aliveNestsText.text = string.Format(
                nestTextFormat,
                Mathf.FloorToInt(nativeAntCount * progress),
                Mathf.FloorToInt(fireAntCount * progress));
        }));
    }

    IEnumerator C_ShowDestroyNestsCount(GameStatic statistic)
    {
        int nativeAntCount = statistic.BucketDestroyNativeAntCount;
        int fireAntCount = statistic.BucketDestroyFireAntCount;

        yield return StartCoroutine(C_NumberIncrease((progress) =>
        {
            destroyNestsText.text = string.Format(
                nestTextFormat,
                Mathf.FloorToInt(nativeAntCount * progress),
                Mathf.FloorToInt(fireAntCount * progress));
        }));
    }
    IEnumerator C_ShowBreedNestsCount(GameStatic statistic)
    {
        int nativeAntCount = statistic.BreedNativeAntCount;
        int fireAntCount = statistic.BreedFireAntCount;

        yield return StartCoroutine(C_NumberIncrease((progress) =>
        {
            breedNestsText.text = string.Format(
                nestTextFormat,
                Mathf.FloorToInt(nativeAntCount * progress),
                Mathf.FloorToInt(fireAntCount * progress));
        }));
    }
    IEnumerator C_ShowScore(GameStatic statistic)
    {
        int score = Mathf.CeilToInt(statistic.CalculateScore());
        yield return StartCoroutine(C_NumberIncrease((progress) => {
            scoreText.text = Mathf.FloorToInt(score * progress).ToString();
        }));
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

    IEnumerator C_NumberIncrease(System.Action<float> progressUpdate)
    {
        progressUpdate.Invoke(0);
        numberInrcreaseTimer.Reset();

        while (!numberInrcreaseTimer.UnscaleUpdateTimeEnd)
        {
            progressUpdate.Invoke(numberIncreaseCurve.Value.Evaluate(numberInrcreaseTimer.Progress));
            yield return null;
        }

        progressUpdate.Invoke(1);
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
