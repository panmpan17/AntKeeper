using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public struct AchievementStoreData
{
    private const string FilePath = "achievement";


    public static bool Read(out AchievementStoreData data)
    {
        string path = Path.Join(Application.persistentDataPath, FilePath);

        if (File.Exists(path))
        {
            var formatter = new BinaryFormatter();
            FileStream stream = File.Open(path, FileMode.Open);
            data = (AchievementStoreData)formatter.Deserialize(stream);
            stream.Close();

            return true;
        }

        data = new AchievementStoreData { Unlockeds = new string[0] };
        return false;
    }

    public string[] Unlockeds;


    public void Append(string[] unlocks)
    {
        var newUnlockeds = new List<string>(Unlockeds);
        for (int i = 0; i < unlocks.Length; i++)
        {
            if (!newUnlockeds.Contains(unlocks[i]))
                newUnlockeds.Add(unlocks[i]);
        }
    }

    public void Append(List<AchievementItem> items)
    {
        var newUnlockeds = new List<string>(Unlockeds);
        for (int i = 0; i < items.Count; i++)
        {
            if (!newUnlockeds.Contains(items[i].ID))
                newUnlockeds.Add(items[i].ID);
        }
        Unlockeds = newUnlockeds.ToArray();
    }

    public void Save()
    {
        string path = Path.Join(Application.persistentDataPath, FilePath);

        var formatter = new BinaryFormatter();
        FileStream stream = File.Open(path, FileMode.Create);
        formatter.Serialize(stream, this);
        stream.Close();
    }
}