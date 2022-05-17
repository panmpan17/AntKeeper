using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MPack;


public class CameraManager : MonoBehaviour
{
    public static CameraManager ins;

    [SerializeField]
    private CinemachineVirtualCamera followPlayerVCamera;
    [SerializeField]
    private CinemachineVirtualCamera fullMapVCamera;
    [SerializeField]
    private CinemachineVirtualCamera screenShotVCamera;

    [SerializeField]
    private CameraState cameraState;


#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField]
    private ValueWithEnable<CameraState> overrideCameraState;
#endif

    public enum CameraState
    {
        FollowPlayer,
        FullMap,
        ScreenShot,
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
        screenShotVCamera.enabled = cameraState == CameraState.ScreenShot;
    }

    public void SwitchCamera(CameraState state)
    {
        cameraState = state;
        followPlayerVCamera.enabled = cameraState == CameraState.FollowPlayer;
        fullMapVCamera.enabled = cameraState == CameraState.FullMap;
        screenShotVCamera.enabled = cameraState == CameraState.ScreenShot;
    }
}