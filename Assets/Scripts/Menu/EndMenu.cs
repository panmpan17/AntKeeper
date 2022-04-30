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
    private TextMeshProUGUI animalCounText;
    [SerializeField]
    private TextMeshProUGUI fireAntCounText;
    [SerializeField]
    private GameObject replayButton;
    [SerializeField]
    private GameObject mainMenuButton;

    [Header("Stars")]
    [SerializeField]
    private GameObject[] stars;
    [SerializeField]
    private float starAnimationTime;
    [SerializeField]
    private Vector3 starStartScale;
    [SerializeField]
    private AnimationCurveReference starFadeCurve;
    [SerializeField]
    private AnimationCurveReference starScaleCurve;

    [Header("Score Calculation")]
    [SerializeField]
    [Range(0, 1f)]
    private float oneStarRequireAnimals = 0.5f;
    [SerializeField]
    [Range(0, 1f)]
    private float twoStarRequireAnimals = 0.7f;
    [SerializeField]
    [Range(0, 1f)]
    private float threeStarRequireAnimals = 0.9f;

    private StarApearAnimaion _animatingStar;
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
        mainMenuButton.SetActive(false);
    }

    void Update()
    {
        if (fadeTimer.Running)
        {
            if (fadeTimer.UnscaleUpdateTimeEnd)
            {
                fadeTimer.Running = false;
                StartCoroutine(ShowStatic());
                _canvasGroup.alpha = 1;
            }

            _canvasGroup.alpha = fadeTimer.Progress;
        }

        if (_animatingStar != null)
        {
            _animatingStar.Timer.UnscaleUpdate();

            _animatingStar.RectTransform.localScale = Vector3.LerpUnclamped(starStartScale, Vector3.one, starScaleCurve.Value.Evaluate(_animatingStar.Timer.Progress));

            Color color = _animatingStar.Image.color;
            color.a = starFadeCurve.Value.Evaluate(_animatingStar.Timer.Progress);
            _animatingStar.Image.color = color;

            if (_animatingStar.Timer.Ended)
            {
                _animatingStar = null;
            }
        }
    }

    public void Open()
    {
        _canvas.enabled = _canvasGroup.enabled = enabled = true;
        fadeTimer.Reset();
    }

    IEnumerator ShowStatic()
    {
        int animalCount = GridManager.ins.CountAliveAnimal();
        int fireAntCount = GridManager.ins.CountFireAnt();

        for (int i = 0; i <= animalCount; i++)
        {
            yield return null;
            animalCounText.text = string.Format("{0} / {1}", i, GridManager.ins.OriginAnimalCount);
        }
        yield return new WaitForSecondsRealtime(1);

        for (int i = 0; i <= fireAntCount; i++)
        {
            yield return null;
            fireAntCounText.text = i.ToString();
        }
        yield return new WaitForSecondsRealtime(1);

        float animalSuvivePercentage = (float)animalCount / (float)GridManager.ins.OriginAnimalCount;

        if (animalSuvivePercentage > oneStarRequireAnimals)
        {
            AddShowStarAnimation(0);
            while (_animatingStar != null) yield return null;

            if (animalSuvivePercentage > twoStarRequireAnimals)
            {
                yield return new WaitForSecondsRealtime(0.6f);
                AddShowStarAnimation(1);
                while (_animatingStar != null) yield return null;

                if (animalSuvivePercentage > threeStarRequireAnimals)
                {
                    yield return new WaitForSecondsRealtime(0.6f);
                    AddShowStarAnimation(2);
                }
            }
        }

        yield return new WaitForSecondsRealtime(0.6f);

        replayButton.SetActive(true);
        mainMenuButton.SetActive(true);
        EventSystem.current.SetSelectedGameObject(replayButton);
    }

    void AddShowStarAnimation(int index)
    {
        GameObject starGameObject = stars[index];

        _animatingStar = new StarApearAnimaion {
            RectTransform = starGameObject.GetComponent<RectTransform>(),
            Image = starGameObject.GetComponent<Image>(),
            Timer = new Timer(starAnimationTime),
        };

        _animatingStar.RectTransform.localScale = starStartScale;

        Color color = _animatingStar.Image.color;
        color.a = 0;
        _animatingStar.Image.color = color;

        starGameObject.SetActive(true);
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

    private class StarApearAnimaion
    {
        public RectTransform RectTransform;
        public Image Image;
        public Timer Timer;
    }
}
