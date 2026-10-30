using RepoXR.Assets;
using UnityEngine.InputSystem;

namespace RepoXR.Input;

public class Actions
{
    public static Actions Instance { get; private set; } = new();
    
    public InputAction HeadPosition { get; private set; }
    public InputAction HeadRotation { get; private set; }
    public InputAction HeadTrackingState { get; private set; }
    
    public InputAction LeftHandPosition { get; private set; }
    public InputAction LeftHandRotation { get; private set; }
    public InputAction LeftHandTrackingState { get; private set; }
    
    public InputAction RightHandPosition { get; private set; }
    public InputAction RightHandRotation { get; private set; }
    public InputAction RightHandTrackingState { get; private set; }
    
    public InputAction EyeGazePosition { get; private set; }
    public InputAction EyeGazeRotation { get; private set; }

    private Actions()
    {
        HeadPosition = AssetCollection.DefaultXRActions.FindAction("Head/Position");
        HeadRotation = AssetCollection.DefaultXRActions.FindAction("Head/Rotation");
        HeadTrackingState = AssetCollection.DefaultXRActions.FindAction("Head/Tracking State");

        LeftHandPosition = AssetCollection.DefaultXRActions.FindAction("Left/Position");
        LeftHandRotation = AssetCollection.DefaultXRActions.FindAction("Left/Rotation");
        LeftHandTrackingState = AssetCollection.DefaultXRActions.FindAction("Left/Tracking State");

        RightHandPosition = AssetCollection.DefaultXRActions.FindAction("Right/Position");
        RightHandRotation = AssetCollection.DefaultXRActions.FindAction("Right/Rotation");
        RightHandTrackingState = AssetCollection.DefaultXRActions.FindAction("Right/Tracking State");

        EyeGazePosition = AssetCollection.DefaultXRActions.FindAction("Eye Gaze/Position");
        EyeGazeRotation = AssetCollection.DefaultXRActions.FindAction("Eye Gaze/Rotation");

        AssetCollection.DefaultXRActions.Enable();

        InputSystem.onActionChange += OnActionChange;
        InputSystem.onBeforeUpdate += EnableDisabledActions;
    }

    // Quest port, R.E.P.O. 0.5.0 scene handoff: RepoXR's head and hand trackers (TrackedPoseDriver) and UI
    // controllers (XRI ActionBasedController) all share these actions, and each of them disables its actions when it
    // is disabled or destroyed. Without a handoff the old scene goes away before the new one starts, and the new
    // scene's trackers enable the actions again. With 0.5.0's handoff the new Main scene starts first, then the Reload
    // scene goes away, and its camera's tracker disabled the head actions under the new camera: the head pose stopped
    // reaching the camera in the singleplayer lobby and every level after it (the view no longer followed the head;
    // the tutorial loads without a handoff). RepoXR enables the whole asset at startup and never disables any of it
    // on purpose, so anything disabled is enabled again before the next input update
    private static bool actionDisabled;
    private static int reported;

    private static void OnActionChange(object target, InputActionChange change)
    {
        if (change != InputActionChange.ActionDisabled || target is not InputAction action ||
            action.actionMap?.asset != AssetCollection.DefaultXRActions)
            return;

        actionDisabled = true;
        if (reported++ < 20)
            Logger.LogInfo($"[Input] {action.actionMap?.name}/{action.name} was disabled; enabling it again");
    }

    private static void EnableDisabledActions()
    {
        if (!actionDisabled)
            return;

        actionDisabled = false;
        foreach (var map in AssetCollection.DefaultXRActions.actionMaps)
        foreach (var action in map.actions)
            if (!action.enabled)
                action.Enable();
    }

    public InputAction this[string name] => VRInputSystem.Instance.Actions[name];
}