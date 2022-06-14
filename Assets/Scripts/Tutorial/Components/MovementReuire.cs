using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MPack;

public class MovementReuire : MonoBehaviour
{
    [SerializeField]
    private AbstractTutorialStep step;

    [SerializeField]
    private Timer requireWalkDuration;

    [SerializeField]
    private Timer requreDashDuration;

    [SerializeField]
    private FillBarControl fillBar;

    private PlayerMovement _playerMovement;

    void OnEnable()
    {
        fillBar.SetFillAmount(0);

        _playerMovement = GameObject.FindWithTag(PlayerBehaviour.Tag).GetComponent<PlayerMovement>();

        _playerMovement.OnWalkStarted += OnWalkStart;
        _playerMovement.OnWalkEnded += OnWalkEnd;

        _playerMovement.OnDashStarted += OnDashStart;
        _playerMovement.OnDashEnded += OnDashEnd;
    }

    void OnDisable()
    {
        _playerMovement.OnWalkStarted -= OnWalkStart;
        _playerMovement.OnWalkEnded -= OnWalkEnd;

        _playerMovement.OnDashStarted -= OnDashStart;
        _playerMovement.OnDashEnded -= OnDashEnd;

        _playerMovement = null;
    }

    void OnWalkStart() => requireWalkDuration.Running = true;
    void OnWalkEnd() => requireWalkDuration.Running = false;

    void OnDashStart() => requreDashDuration.Running = true;
    void OnDashEnd() => requreDashDuration.Running = false;

    void Update()
    {
        bool update = false;
        if (requireWalkDuration.Running)
        {
            update = true;
            requireWalkDuration.Update();
        }
        if (requreDashDuration.Running)
        {
            update = true;
            requreDashDuration.Update();
        }

        if (update)
        {
            float totalProgres = (Mathf.Clamp(requireWalkDuration.RunTime, 0, requireWalkDuration.TargetTime) +
                                  Mathf.Clamp(requreDashDuration.RunTime, 0, requreDashDuration.TargetTime)) /
                                  (requireWalkDuration.TargetTime + requreDashDuration.TargetTime);
            fillBar.SetFillAmount(totalProgres);

            if (requireWalkDuration.Ended && requreDashDuration.Ended)
            {
                step.Skip();
            }
        }
    }
}
