using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MPack;

public class ScreenShotTaker : MonoBehaviour
{

    [SerializeField]
    private bool continuously;
    [SerializeField]
    private int continuouslyCap;
    [SerializeField]
    private bool takeThisFrame;
    [SerializeField]
    private bool useTimer;
    [SerializeField]
    private Timer inverval;

    void Update()
    {
        if (takeThisFrame)
        {
            TakeScreenShot();
            takeThisFrame = false;
        }

        if (continuously)
        {
            TakeScreenShot();

            if (--continuouslyCap < 0)
            {
                continuously = false;
            }
        }

        if (useTimer && inverval.UpdateEnd)
        {
            inverval.Reset();
            TakeScreenShot();
        }
    }

    void TakeScreenShot()
    {
        string filePath = NewFileNameInDirectory("screeshot", ".png", Application.persistentDataPath);
        ScreenCapture.CaptureScreenshot(filePath);
    }

    string NewFileNameInDirectory(string fileName, string fileExtension, string folder)
    {
        string path = Path.Join(folder, fileName + fileExtension);
        int count = 1;
        while (File.Exists(path))
        {
            path = Path.Join(folder, string.Format("{0}-{1}{2}", fileName, count, fileExtension));
            count += 1;

            if (count >= 100)
                break;
        }

        return path;
    }
}
