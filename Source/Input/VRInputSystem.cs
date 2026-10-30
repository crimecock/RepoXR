using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using RepoXR.Assets;
using RepoXR.Managers;
using RepoXR.UI.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RepoXR.Input;

public class VRInputSystem : MonoBehaviour
{
    public static VRInputSystem Instance => _instance ?? InputManager.instance.gameObject.AddComponent<VRInputSystem>();
    private static VRInputSystem? _instance;

    private PlayerInput playerInput;

    public InputActionAsset Actions => playerInput.actions;
    public string CurrentControlScheme => playerInput.currentControlScheme;

    private Dictionary<string, bool> inputToggle = [];

    private void Awake()
    {
        _instance = this;

        ApplyPortDefaults(AssetCollection.VRInputs);

        playerInput = gameObject.AddComponent<PlayerInput>();
        playerInput.actions = AssetCollection.VRInputs;
        playerInput.defaultActionMap = "VR Actions";
        playerInput.defaultControlScheme = "Oculus";
        playerInput.neverAutoSwitchControlSchemes = false;
        playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

        if (VRSession.IsLeftHanded)
        {
            playerInput.actions.LoadBindingOverridesFromJson(AssetCollection.RemappableControls.defaultLeftHandedBindings);
            ApplyPortLeftHandedDefaults(playerInput.actions);
            playerInput.actions.LoadBindingOverridesFromJson(Plugin.Config.ControllerBindingsOverrideLeft.Value, false);
        }
        else
            playerInput.actions.LoadBindingOverridesFromJson(Plugin.Config.ControllerBindingsOverride.Value);

        playerInput.ActivateInput();

        inputToggle =
            JsonConvert.DeserializeObject<Dictionary<string, bool>>(Plugin.Config.InputToggleBindings.Value) ?? [];
    }

    private float nextSchemeCheck;

    /// <summary>
    /// RepoXR's control schemes name the controller layouts they need (Oculus Touch, Quest Pro, Index, ...). A Quest 3
    /// on its own runtime reports Touch Plus controllers, which no scheme names: the input still worked (the bindings
    /// are on generic XR controllers), but there was no control scheme, and the controls page said "DISCONNECTED" and
    /// allowed no rebinding. With XR controllers present and no scheme, the closest scheme is picked for them
    /// </summary>
    private void Update()
    {
        if (!string.IsNullOrEmpty(playerInput.currentControlScheme) || Time.unscaledTime < nextSchemeCheck)
            return;

        nextSchemeCheck = Time.unscaledTime + 1;

        var controllers = new List<InputDevice>();
        foreach (var device in InputSystem.devices)
            if (device is UnityEngine.InputSystem.XR.XRController && device.added)
                controllers.Add(device);

        if (controllers.Count == 0)
            return;

        var layouts = string.Join(", ", controllers.ConvertAll(device => device.layout));
        var scheme = layouts.IndexOf("Quest", StringComparison.OrdinalIgnoreCase) >= 0 ? "Meta Quest" : "Oculus";
        if (playerInput.actions.FindControlSchemeIndex(scheme) < 0)
            scheme = "Oculus";

        try
        {
            playerInput.SwitchCurrentControlScheme(scheme, controllers.ToArray());
            Logger.LogInfo($"No control scheme matched the controllers ({layouts}): using {scheme}");
        }
        catch (Exception e)
        {
            Logger.LogWarning($"No control scheme matched the controllers ({layouts}), and {scheme} could not be used: {e.Message}");
            nextSchemeCheck = Time.unscaledTime + 30;
        }
    }

    private const string MenuButton = "<XRController>{LeftHand}/{MenuButton}";

    /// <summary>
    /// Quest port defaults: the pause menu moves to the menu button of the left controller (≡), which nothing used,
    /// and the X button it frees mutes the microphone (ToggleMute, unbound in RepoXR). These change the default
    /// bindings, so bindings changed in Settings → Controls still win, and resetting a control comes back to these
    /// </summary>
    private static void ApplyPortDefaults(InputActionAsset actions)
    {
        // On PC RepoXR's own bindings stay: not every controller has a left menu button
        if (!Plugin.IsQuest)
            return;

        try
        {
            ChangeFirstBinding(actions.FindAction("Menu"), MenuButton);
            ChangeFirstBinding(actions.FindAction("ToggleMute"), "<XRController>{LeftHand}/primaryButton");
        }
        catch (Exception e)
        {
            Logger.LogWarning($"Could not apply the Quest port's default bindings: {e}");
        }
    }

    /// <summary>
    /// RepoXR's left-handed defaults mirror the controllers and put the menu on the right A button: the menu button
    /// only exists on the left controller, so it stays there, and A mutes
    /// </summary>
    private static void ApplyPortLeftHandedDefaults(InputActionAsset actions)
    {
        if (!Plugin.IsQuest)
            return;

        try
        {
            actions.FindAction("Menu")?.ApplyBindingOverride(0, MenuButton);
            actions.FindAction("ToggleMute")?.ApplyBindingOverride(0, "<XRController>{RightHand}/primaryButton");
        }
        catch (Exception e)
        {
            Logger.LogWarning($"Could not apply the Quest port's left-handed bindings: {e}");
        }
    }

    private static void ChangeFirstBinding(InputAction? action, string path)
    {
        if (action == null || action.bindings.Count == 0 || action.bindings[0].path == path)
            return;

        action.ChangeBinding(0).WithPath(path);
    }

    private void OnEnable()
    {
        playerInput.actions["Chat"].performed += ChatPerformed;
        playerInput.actions["Chat"].canceled += ChatCanceled;
        playerInput.onControlsChanged += OnControlsChanged;

        Plugin.Config.LeftHandDominant.SettingChanged += DominantHandChanged;
    }

    private void OnDisable()
    {
        playerInput.actions["Chat"].performed -= ChatPerformed;
        playerInput.actions["Chat"].canceled -= ChatCanceled;
        playerInput.onControlsChanged -= OnControlsChanged;

        Plugin.Config.LeftHandDominant.SettingChanged -= DominantHandChanged;
    }

    // Special chat input section

    private Coroutine? chatHoldCoroutine;
    private bool chatHoldTriggered;
    private int chatChoice;    

    private void ChatPerformed(InputAction.CallbackContext context)
    {
        // Disable chat when expression radial is open
        if (ExpressionRadial.instance &&
            (ExpressionRadial.instance.isActive || ExpressionRadial.instance.closedLastPress))
            return;

        // Disable chat when pause menu is open
        if (!SemiFunc.MenuLevel() && MenuManager.instance.currentMenuPage)
            return;
        
        chatHoldTriggered = false;
        chatHoldCoroutine = StartCoroutine(HoldTimer());
    }

    private void ChatCanceled(InputAction.CallbackContext context)
    {
        // Disable chat when expression radial is open
        if (ExpressionRadial.instance &&
            (ExpressionRadial.instance.isActive || ExpressionRadial.instance.closedLastPress))
        {
            ExpressionRadial.instance.closedLastPress = false;
            return;
        }
        
        // Disable chat when pause menu is open
        if (!SemiFunc.MenuLevel() && MenuManager.instance.currentMenuPage)
            return;
        
        if (chatHoldCoroutine != null)
        {
            StopCoroutine(chatHoldCoroutine);
            chatHoldCoroutine = null;
        }

        if (chatHoldTriggered)
        {
            chatHoldTriggered = false;
            return;
        }

        chatChoice = 1;
    }

    private IEnumerator HoldTimer()
    {
        yield return new WaitForSeconds(0.3f);

        if (!playerInput.actions["Chat"].IsPressed())
            yield break;

        chatHoldTriggered = true;
        chatChoice = 2;
    }

    public bool ChatPressed()
    {
        if (chatChoice != 1)
            return false;

        chatChoice = 0;
        return true;
    }

    public bool ExpressionPressed()
    {
        if (chatChoice != 2)
            return false;

        chatChoice = 0;
        return true;
    }
    
    // Other input system related methods

    public void ActivateInput()
    {
        playerInput.ActivateInput();
    }

    public void DeactivateInput()
    {
        playerInput.DeactivateInput();
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
    
    public void InputToggleRebind(string inputAction, bool toggle)
    {
        inputToggle[inputAction] = toggle;
        
        SaveInputToggles();
    }

    public bool InputToggleGet(string action)
    {
        if (inputToggle.TryGetValue(action, out var value))
            return value;
        
        // Check for default
        foreach (var control in AssetCollection.RemappableControls.controls)
            if (control.currentInput.action.name == action)
            {
                inputToggle[action] = control.defaultToggle;

                return control.defaultToggle;
            }

        // R.E.P.O. 0.5.0: holding on to a vehicle has its own toggle, on in the game by default (one press gets on, the
        // next one gets off); the VR actions have no key for it, it rides on Grab
        if (action == "VehicleGrab")
            return true;

        // If all else fails: default to hold
        return false;
    }

    private void SaveInputToggles()
    {
        Plugin.Config.InputToggleBindings.Value = JsonConvert.SerializeObject(inputToggle);
    }

    /// <summary>
    /// Reload bindings on dominant hand change
    /// </summary>
    private void DominantHandChanged(object sender, EventArgs e)
    {
        if (VRSession.IsLeftHanded)
        {
            playerInput.actions.LoadBindingOverridesFromJson(AssetCollection.RemappableControls.defaultLeftHandedBindings);
            ApplyPortLeftHandedDefaults(playerInput.actions);
            playerInput.actions.LoadBindingOverridesFromJson(Plugin.Config.ControllerBindingsOverrideLeft.Value, false);
        }
        else
            playerInput.actions.LoadBindingOverridesFromJson(Plugin.Config.ControllerBindingsOverride.Value);
    }

    private static void OnControlsChanged(PlayerInput input)
    {
        Logger.LogInfo($"Detected controllers: {input.currentControlScheme}");
    }
}