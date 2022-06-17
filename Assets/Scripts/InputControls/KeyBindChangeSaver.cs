using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyBindChangeSaver : MonoBehaviour
{
    [SerializeField]
    private string saveFile;
    [SerializeField]
    private InputActionAsset inputActionsAsset;

    void Start()
    {
        Load();
    }

    public void Load()
    {
        string path = Path.Join(Application.persistentDataPath, saveFile);
        if (File.Exists(path))
            inputActionsAsset.LoadBindingOverridesFromJson(File.ReadAllText(path));
    }

    public void Save()
    {
        string path = Path.Join(Application.persistentDataPath, saveFile);
        StreamWriter stream = File.CreateText(path);
        stream.Write(inputActionsAsset.SaveBindingOverridesAsJson());
        stream.Close();
    }
}
