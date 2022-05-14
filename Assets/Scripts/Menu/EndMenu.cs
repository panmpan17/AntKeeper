using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MPack;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    [SerializeField]
    [ShortTimer]
    private Timer fadeTimer;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject replayButton;
    [SerializeField]
    private TextMeshProUGUI resultAnimalCount;
    [SerializeField]
    private TextMeshProUGUI originalAnimalCount;

    [SerializeField]
    private RectTransform mask;
    [SerializeField]
    private RectTransform fireAntBar;
    [SerializeField]
    private RectTransform nativeAntBar;
    [SerializeField]
    private AnimationCurve barWidthCurve;

    [Header("Stars")]
    [SerializeField]
    private GameObject[] stars;
    [SerializeField]
    private float starApearGap = 0.6f;
    [SerializeField]
    private float starAnimationTime;
    [SerializeField]
    private Vector3 starStartScale;
    [SerializeField]
    private AnimationCurveReference starFadeCurve;
    [SerializeField]
    private AnimationCurveReference starScaleCurve;
    private WaitForSecondsRealtime starGapWait;

    [Header("Score Calculation")]
    [SerializeField]
    private float oneStarScore = 0.5f;
    [SerializeField]
    private float twoStarScore = 0.7f;
    [SerializeField]
    private float threeStarScore = 0.9f;

    [SerializeField]
    private GameObject staticProvider;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(false);
        }

        _canvas.enabled = _canvasGroup.enabled = enabled = false;
        replayButton.SetActive(false);

        starGapWait = new WaitForSecondsRealtime(starApearGap);
    }

    public void Open()
    {
        _canvas.enabled = _canvasGroup.enabled = enabled = true;
        fadeTimer.Reset();

        resultAnimalCount.text = "0";
        originalAnimalCount.text = "/";

        fireAntBar.sizeDelta = new Vector2(0, fireAntBar.sizeDelta.y);
        nativeAntBar.sizeDelta = new Vector2(0, nativeAntBar.sizeDelta.y);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        fadeTimer.Reset();
        while (!fadeTimer.UnscaleUpdateTimeEnd)
        {
            _canvasGroup.alpha = fadeTimer.Progress;
            yield return null;
        }

        StartCoroutine(ShowStatic());
        _canvasGroup.alpha = 1;
    }

    IEnumerator ShowStatic()
    {
        var statistic = staticProvider.GetComponent<IGameStaticProvider>().CollectGameStatic();

        yield return StartCoroutine(ShowAnimalCount(statistic));
        yield return StartCoroutine(ShowArea(statistic));
        yield return StartCoroutine(ShowStars(statistic));
        yield return starGapWait;

        replayButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(replayButton);
    }

    IEnumerator ShowAnimalCount(GameStatic statistic)
    {
        resultAnimalCount.text = "0";
        originalAnimalCount.text = "/" + statistic.OriginalAnimalCount.ToString();

        yield return null;

        for (int count = 1; count <= statistic.ResultAnimalCount; count++)
        {
            resultAnimalCount.text = count.ToString();
            yield return null;
        }
    }

    IEnumerator ShowArea(GameStatic statistic)
    {
        float fireAntWidth = statistic.FireAntAreaPercentage * mask.sizeDelta.x;
        float nativeAntWidth = statistic.NativeAntAreaPercentage * mask.sizeDelta.x + fireAntWidth;

        Timer timer = new Timer(1);

        while (!timer.UnscaleUpdateTimeEnd)
        {
            fireAntBar.sizeDelta = new Vector2(Mathf.Lerp(0, fireAntWidth, barWidthCurve.Evaluate(timer.Progress)), fireAntBar.sizeDelta.y);
            yield return null;
        }

        fireAntBar.sizeDelta = new Vector2(fireAntWidth, fireAntBar.sizeDelta.y);
        nativeAntBar.sizeDelta = new Vector2(fireAntWidth, nativeAntBar.sizeDelta.y);

        timer.Reset();

        while (!timer.UnscaleUpdateTimeEnd)
        {
            nativeAntBar.sizeDelta = new Vector2(Mathf.Lerp(fireAntWidth, nativeAntWidth, barWidthCurve.Evaluate(timer.Progress)), fireAntBar.sizeDelta.y);
            yield return null;
        }

        nativeAntBar.sizeDelta = new Vector2(nativeAntWidth, nativeAntBar.sizeDelta.y);
    }

    IEnumerator ShowStars(GameStatic statistic)
    {
        float score = statistic.CalculateScore();
        Debug.Log(score);

        if (score < oneStarScore) yield break;

        yield return StartCoroutine(StarAnimation(stars[0]));

        if (score < twoStarScore) yield break;

        yield return starGapWait;
        yield return StartCoroutine(StarAnimation(stars[1]));

        if (score < threeStarScore) yield break;

        yield return starGapWait;
        yield return StartCoroutine(StarAnimation(stars[2]));
    }

    IEnumerator StarAnimation(GameObject star)
    {

        var rectTransform = star.GetComponent<RectTransform>();
        var image = star.GetComponent<Image>();
        var timer = new Timer(starAnimationTime);
        
        Color color = image.color;
        star.SetActive(true);

        while (!timer.UnscaleUpdateTimeEnd)
        {
            rectTransform.localScale = Vector3.LerpUnclamped(starStartScale, Vector3.one, starScaleCurve.Value.Evaluate(timer.Progress));

            color.a = starFadeCurve.Value.Evaluate(timer.Progress);
            image.color = color;
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
}
