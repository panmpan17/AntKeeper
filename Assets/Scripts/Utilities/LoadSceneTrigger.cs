using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadSceneTrigger : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    public void Trigger()
    {
        SceneManager.LoadScene(sceneName);
    }
}
