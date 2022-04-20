using UnityEditor;
using MPack;

[CustomPropertyDrawer(typeof(ValueWithEnable<CameraManager.CameraState>))]
public class ValueWithEnableCameraState : ValueWithEnableDrawer { }