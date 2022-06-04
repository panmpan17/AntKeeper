using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MPack;

public class AchievementMenu : AbstractMenu
{
    [Header("Instainiate achievement")]
    [SerializeField]
    private AchievementItem[] achievements;
    [SerializeField]
    private AchievementItemDisplay achievementItemDisplayPrefab;
    [SerializeField]
    private Vector2 startAnchor;
    [SerializeField]
    private float padding;

    [Header("Hidden achievement")]
    [SerializeField]
    private Sprite hiddenIcon;
    [SerializeField]
    [LauguageID]
    private int hiddenAchievmmentDescriptionID;


    [Header("Reference")]
    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private RectTransform scrollContent;
    [SerializeField]
    private UnityEvent disableScroll;
    [SerializeField]
    private ScrollRectRemoteControl scrollRectRemoteControl;

    #if UNITY_EDITOR
    [Header("EditorOnly")]
    [SerializeField]
    private bool overrideAchievedID;
    [SerializeField]
    private string[] achievedIDs;
    #endif

    private string[] _achievedIDs;

    void Start()
    {
        #if UNITY_EDITOR
        if (overrideAchievedID)
            _achievedIDs = achievedIDs;
        #endif

        LoadAchievementSaveData();
        InstainiateAchievementItem();
        Open();
    }

    void LoadAchievementSaveData()
    {
        // AchievementSaveState 
    }

    void InstainiateAchievementItem()
    {
        Vector2 anchorPosition = startAnchor;

        for (int i = 0; i < achievements.Length; i++)
        {
            var achievement = achievements[i];

            AchievementItemDisplay itemDisplay = Instantiate(achievementItemDisplayPrefab, scrollContent);

            var rectTrasnform = itemDisplay.GetComponent<RectTransform>();
            rectTrasnform.anchoredPosition = anchorPosition;

            anchorPosition.y += rectTrasnform.sizeDelta.y + padding;

            int index = Array.IndexOf(_achievedIDs, achievement.ID);
            if (index == -1)
            {
                if (achievement.Hidden)
                    itemDisplay.SetupHidden(achievement, hiddenIcon, hiddenAchievmmentDescriptionID);
                else
                    itemDisplay.SetupLock(achievement);
            }
            else
                itemDisplay.SetupUnlock(achievement);

        }

        if (anchorPosition.y > scrollContent.sizeDelta.y)
        {
            float min = scrollContent.sizeDelta.y - anchorPosition.y;
            scrollContent.sizeDelta = new Vector2(scrollContent.sizeDelta.x, anchorPosition.y);
            scrollRectRemoteControl.SetMin(min);
        }
        else
        {
            disableScroll.Invoke();
        }
    }

    void Open()
    {
        EventSystem.current.SetSelectedGameObject(backButton);
    }
}
