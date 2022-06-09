using System.Collections;
using System.Collections.Generic;
using MPack;
using TMPro;
using UnityEngine;

public class StartCountDown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip beepAudio;
    [SerializeField]
    private AudioClip startAudio;

    [SerializeField]
    [LauguageID]
    private int readyText;
    [SerializeField]
    [LauguageID]
    private int startText;

#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private bool skipStartCountDown;
#endif

    void Start()
    {
        GetComponent<Canvas>().enabled = true;
    }

    public void StartCounting()
    {
        StartCoroutine(C_StartCounting());
    }

    IEnumerator C_StartCounting()
    {
#if UNITY_EDITOR
        if (skipStartCountDown)
        {
            CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
            StartGame();
            yield break;
        }
#endif

        GetComponent<Canvas>().enabled = true;

        var waitOneSec = new WaitForSeconds(1.2f);

        yield return new WaitForSeconds(0.5f);
        text.text = LanguageMgr.GetTextById(readyText);
        yield return waitOneSec;
        CameraManager.ins.SwitchCamera(CameraManager.CameraState.FollowPlayer);
        audioSource.PlayOneShot(beepAudio);
        text.text = "3";

        yield return waitOneSec;
        audioSource.PlayOneShot(beepAudio);
        text.text = "2";

        yield return waitOneSec;
        audioSource.PlayOneShot(beepAudio);
        text.text = "1";

        yield return waitOneSec;
        text.text = LanguageMgr.GetTextById(startText);
        audioSource.PlayOneShot(startAudio);

        yield return waitOneSec;
        StartGame();
    }

    void StartGame()
    {
        HUDManager.ins.enabled = true;
        GameManager.ins.enabled = true;
        gameObject.SetActive(false);
    }
}
