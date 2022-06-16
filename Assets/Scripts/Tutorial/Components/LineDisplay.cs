using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MPack;
using TMPro;
using System;

public class LineDisplay : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;
    [SerializeField]
    private TextMeshProUGUI uiText;
    [SerializeField]
    [LauguageID]
    private int displayText;
    [SerializeField]
    private GameObject skipIndicate;

    [Header("Timing")]
    [SerializeField]
    private float betweenLine;
    [SerializeField]
    private float betweenParagraph;
    // private Timer _timer;
    
    private InputObserver observer;

    void Start()
    {
        StartCoroutine(C_Display());
    }

    IEnumerator C_Display()
    {
        string[] lines = LanguageMgr.GetTextById(displayText).Split("\n");
        WaitForSeconds betweenLineWait = new WaitForSeconds(betweenLine);
        WaitForSeconds betweenParagraphWait = new WaitForSeconds(betweenParagraph);

        StringBuilder builder = new StringBuilder();

        uiText.text = "";

        yield return betweenParagraphWait;

        for (int i = 0; i < lines.Length; i++)
        {
            builder.Append(lines[i]);
            builder.AppendLine();
            uiText.text = builder.ToString();

            if (lines[i] == "")
            {
                yield return betweenParagraph;
            }
            else
            {
                yield return betweenLineWait;
            }
        }

        skipIndicate.SetActive(true);

        // while (true)
        // {
        //     if (Keyboard.current.anyKey.wasPressedThisFrame)
        //         step.Skip();
        // }

        observer = new InputObserver();
        IDisposable disposable = InputSystem.onAnyButtonPress.Subscribe(observer);
        observer.Callback += delegate
        {
            step.Skip();
            disposable.Dispose();
        };
    }

    public class InputObserver : IObserver<InputControl>
    {
        public event System.Action Callback;

        public void OnCompleted() {}

        public void OnError(Exception error) {}

        public void OnNext(InputControl value)
        {
            // throw new NotImplementedException();
            Callback?.Invoke();
        }
    }
}
