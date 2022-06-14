using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager ins;

    [SerializeField]
    private float waitBetweenStep;
    private WaitForSeconds waitBetweenStepCouroutine;
    [SerializeField]
    private AbstractTutorialStep[] steps;

#if UNITY_EDITOR
    [SerializeField]
#endif
    private int currentStep = 0;

    void Awake()
    {
        ins = this;

        for (int i = 0; i < steps.Length; i++)
        {
            steps[i].gameObject.SetActive(false);
        }

        waitBetweenStepCouroutine = new WaitForSeconds(waitBetweenStep);

#if UNITY_EDITOR
        StartCoroutine(SkipToSteps());
#else
        steps[currentStep].gameObject.SetActive(true);
        steps[currentStep].StartStep();
#endif
    }

    IEnumerator SkipToSteps()
    {
        for (int i = 0; i < steps.Length && i < currentStep; i++)
        {
            steps[i].gameObject.SetActive(true);
            steps[i].StartStep();
            yield return null;
            steps[i].SkipWithoutCallNext();
            steps[i].gameObject.SetActive(false);
            yield return null;
        }

        steps[currentStep].gameObject.SetActive(true);
        steps[currentStep].StartStep();
    }

    public void NextStep()
    {
        steps[currentStep].gameObject.SetActive(false);

        currentStep++;

        if (currentStep >= steps.Length)
            return;

        StartCoroutine(WaitBetweenStep());
    }

    IEnumerator WaitBetweenStep()
    {
        yield return waitBetweenStepCouroutine;

        steps[currentStep].gameObject.SetActive(true);
        steps[currentStep].StartStep();
    }
}
