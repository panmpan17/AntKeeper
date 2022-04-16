using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager ins;

    [SerializeField]
    private CinemachineVirtualCamera followPlayerVCamera;

    [SerializeField]
    private CinemachineVirtualCamera fullMapVCamera;

    [SerializeField]
    private CameraState cameraState;

    public enum CameraState
    {
        FollowPlayer,
        FullMap,
    }

    void Awake()
    {
        ins = this;

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
