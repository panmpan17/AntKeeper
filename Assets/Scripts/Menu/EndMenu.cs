using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;
using TMPro;

public class EndMenu : MonoBehaviour
{
    [SerializeField]
    [ShortTimer]
    private Timer fadeTimer;

    [SerializeField]
    private GameObject[] firstStars;
    [SerializeField]
    private TextMeshProUGUI animalCounText;
    [SerializeField]
    private TextMeshProUGUI fireAntCounText;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;

        for (int i = 0; i < firstStars.Length; i++)
        {
            firstStars[i].gameObject.SetActive(false);
        }

        _canvas.enabled = _canvasGroup.enabled = enabled = false;
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
            animalCounText.text = i.ToString();
        }
        yield return new WaitForSecondsRealtime(1);

        for (int i = 0; i <= fireAntCount; i++)
        {
            yield return null;
            fireAntCounText.text = i.ToString();
        }
        yield return new WaitForSecondsRealtime(1);


        for (int i = 0; i < firstStars.Length; i++)
        {
            firstStars[i].gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
