using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class CameraManager : MonoBehaviour
{
    public static CameraManager ins;

    [SerializeField]
    private CinemachineVirtualCamera followPlayerVCamera;

    [SerializeField]
    private CinemachineVirtualCamera fullMapVCamera;

    [SerializeField]
    private CameraState cameraState;

    [SerializeField]
    private ValueWithEnable<CameraState> overrideCameraState;

    public enum CameraState
    {
        FollowPlayer,
        FullMap,
    }

    void Awake()
    {
        ins = this;

#if UNITY_EDITOR
        if (overrideCameraState.Enable)
            cameraState = overrideCameraState.Value;
#endif

        followPlayerVCamera.enabled = cameraState == CameraState.FollowPlayer;
        fullMapVCamera.enabled = cameraState == CameraState.FullMap;
    }

    public void SwitchCamera(CameraState state)
    {
        cameraState = state;
        followPlayerVCamera.enabled = cameraState == CameraState.FollowPlayer;
        fullMapVCamera.enabled = cameraState == CameraState.FullMap;
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ValueWithEnable<CameraManager.CameraState>))]
public class ValueWithEnableCameraState : OverrideBaseDrawer { }
#endif