using System;
using System.Linq;
using RepoXR.Assets;
using RepoXR.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.UI;

namespace RepoXR.UI;

public class XRRayInteractorManager : MonoBehaviour
{
    public static XRRayInteractorManager? Instance { get; private set; }

    private XRRayInteractor leftInteractor;
    private XRRayInteractor rightInteractor;
    private ActionBasedController leftActions;
    private ActionBasedController rightActions;
    private LineRenderer leftRenderer;
    private LineRenderer rightRenderer;

    private ActiveController activeController;
    
    private void Awake()
    {
        Instance = this;
        
        (leftActions, leftInteractor, leftRenderer) = CreateInteractorController("Left");
        (rightActions, rightInteractor, rightRenderer) = CreateInteractorController("Right");
        
        leftActions.uiPressAction.action.performed += LeftControllerPressed;
        rightActions.uiPressAction.action.performed += RightControllerPressed;
        
        UpdateActiveController(ActiveController.Right);
    }

    private float nextInputReport;
    private int inputReports;

    // Quest port: what the Input System sees, a few times after the menu opens (devices, the UI press bindings and
    // their values, the hand pose), to tell missing devices from unbound actions from a missed click
    private void ReportInput()
    {
        var devices = string.Join(", ", InputSystem.devices.Select(d => $"{d.layout}{(d.enabled ? "" : " (disabled)")}"));
        Logger.LogInfo($"[UI] input devices: {devices}");
        foreach (var (hand, controller) in new[] { ("Left", leftActions), ("Right", rightActions) })
        {
            var press = controller.uiPressAction.action;
            var position = controller.positionAction.action;
            Logger.LogInfo(press == null
                ? $"[UI] {hand} UI Press: no action"
                : $"[UI] {hand} UI Press: enabled {press.enabled}, map {press.actionMap?.name} enabled {press.actionMap?.enabled}, " +
                  $"bindings [{string.Join("; ", press.bindings.Select(b => b.effectivePath))}], " +
                  $"controls [{string.Join("; ", press.controls.Select(c => c.path))}], pressed {press.IsPressed()}, " +
                  $"position {(position == null ? "none" : $"{position.enabled} {position.controls.Count} controls {position.ReadValue<Vector3>()}")}");
        }
    }

    private float nextModuleCheck;

    /// <summary>
    /// Quest port: the rays only see UI through XRI's XRUIInputModule, and an EventSystem runs one input module, the
    /// current EventSystem one. R.E.P.O. 0.5.0's scene EventSystem has a StandaloneInputModule; when XRI put its module
    /// elsewhere or the game's module stayed first, the rays hit nothing ("ray on nothing" with the trigger working).
    /// Keep exactly one EventSystem, the one with the XRUIInputModule, and only that module enabled on it
    /// </summary>
    private static void EnsureXRUIInputModule()
    {
        var systems = FindObjectsOfType<EventSystem>(true);
        var xrModule = FindObjectsOfType<XRUIInputModule>(true).FirstOrDefault();
        if (!xrModule)
        {
            var host = EventSystem.current ? EventSystem.current : systems.FirstOrDefault();
            if (!host)
                return;
            xrModule = host.gameObject.AddComponent<XRUIInputModule>();
        }

        var main = xrModule.GetComponent<EventSystem>();
        var changes = new System.Collections.Generic.List<string>();
        if (!xrModule.enabled)
        {
            xrModule.enabled = true;
            changes.Add("XRUIInputModule enabled");
        }
        foreach (var module in main.GetComponents<BaseInputModule>())
            if (module != xrModule && module.enabled)
            {
                module.enabled = false;
                changes.Add($"{module.GetType().Name} off");
            }
        foreach (var system in systems)
            if (system != main && system.enabled)
            {
                system.enabled = false;
                changes.Add($"EventSystem '{system.name}' off");
            }
        if (!main.enabled || !main.gameObject.activeInHierarchy)
        {
            main.enabled = true;
            main.gameObject.SetActive(true);
            changes.Add($"EventSystem '{main.name}' on");
        }
        if (EventSystem.current != main)
        {
            EventSystem.current = main;
            changes.Add($"current EventSystem '{main.name}'");
        }

        if (changes.Count > 0)
            Logger.LogInfo($"[UI] input module: {string.Join(", ", changes)}; EventSystems: " +
                           string.Join("; ", systems.Select(e => $"{e.name} [{string.Join(", ", e.GetComponents<BaseInputModule>().Select(m => m.GetType().Name + (m.enabled ? "" : " (off)")))}]")));
    }

    // Quest port: one log line per trigger press while a menu is up, to see why a click does or does not land
    private void Update()
    {
        if (Time.unscaledTime >= nextModuleCheck)
        {
            nextModuleCheck = Time.unscaledTime + 1f;
            try
            {
                EnsureXRUIInputModule();
            }
            catch (Exception e)
            {
                Logger.LogWarning($"[UI] input module check failed: {e.Message}");
            }
        }

        if (inputReports < 6 && Time.unscaledTime >= nextInputReport)
        {
            nextInputReport = Time.unscaledTime + 5f;
            inputReports++;
            try
            {
                ReportInput();
            }
            catch (Exception e)
            {
                Logger.LogWarning($"[UI] input report failed: {e.Message}");
            }
        }

        var (interactor, actions) = GetActiveInteractor();
        if (!actions.uiPressAction.action.WasPressedThisFrame())
            return;

        var hit = interactor.TryGetCurrentUIRaycastResult(out var result) && result.gameObject
            ? result.gameObject.name
            : "nothing";
        var menu = MenuManager.instance;
        var current = EventSystem.current;
        Logger.LogInfo($"[UI] event system {(current ? current.name : "none")}, module {(current && current.currentInputModule ? current.currentInputModule.GetType().Name : "none")}, " +
                       $"UI interaction {interactor.enableUIInteraction}");
        try
        {
            ProbeUIRay(interactor);
        }
        catch (Exception e)
        {
            Logger.LogWarning($"[UI] probe failed: {e.Message}");
        }
        Logger.LogInfo($"[UI] trigger ({activeController}): ray on {hit}; menu target {(menu ? menu.currentMenuID : "-")}, " +
                       $"keyboard navigating {(menu && menu.isKeyboardNavigating)}, page {(menu && menu.currentMenuPage ? menu.currentMenuPage.name : "-")}, " +
                       $"input mode {SemiFunc.InputMode()}, UI click {SemiFunc.InputUIClick()}");
    }

    /// <summary>
    /// Quest port: why the ray finds no UI. The interactor's own view (3D and UI hits), then the menu canvas checked by
    /// hand: where the ray meets its plane, how far, whether that point is on the canvas, which raycast targets are
    /// under it, the canvas's raycasters and masks, and colliders in the way
    /// </summary>
    private static void ProbeUIRay(XRRayInteractor interactor)
    {
        var origin = interactor.rayOriginTransform ? interactor.rayOriginTransform : interactor.transform;
        var ray = new Ray(origin.position, origin.forward);
        interactor.TryGetCurrentRaycast(out var hit3D, out _, out var hitUI, out _, out var uiClosest);
        Logger.LogInfo($"[UI] probe: ray {ray.origin} -> {ray.direction}, max {interactor.maxRaycastDistance} m, " +
                       $"line {interactor.lineType}, mask {interactor.raycastMask.value}, 3D hit {(hit3D.HasValue ? hit3D.Value.collider.name + " @" + hit3D.Value.distance.ToString("F1") : "none")}, " +
                       $"UI hit {(hitUI.HasValue && hitUI.Value.gameObject ? hitUI.Value.gameObject.name : "none")}, UI closest {uiClosest}");

        Logger.LogInfo("[UI] probe event systems: " + string.Join("; ", FindObjectsOfType<EventSystem>(true).Select(e =>
            $"{e.name} #{e.GetInstanceID()} {(e.enabled && e.gameObject.activeInHierarchy ? "on" : "off")}{(e == EventSystem.current ? " (current)" : "")} " +
            $"[{string.Join(", ", e.GetComponents<BaseInputModule>().Select(m => $"{m.GetType().Name}{(m.enabled ? "" : " off")}"))}]")) +
            $"; XRUIInputModules: {FindObjectsOfType<XRUIInputModule>(true).Length}");

        var canvas = HUDCanvas.instance ? HUDCanvas.instance.GetComponent<Canvas>() : null;
        if (!canvas)
            return;
        var t = canvas.transform;
        var plane = new Plane(-t.forward, t.position);
        var onPlane = plane.Raycast(ray, out var enter);
        var point = ray.GetPoint(enter);
        var local = t.InverseTransformPoint(point);
        var rect = ((RectTransform)t).rect;
        Logger.LogInfo($"[UI] probe canvas {canvas.name}: mode {canvas.renderMode}, pos {t.position}, rot {t.eulerAngles}, " +
                       $"scale {t.lossyScale}, rect {rect}, worldCamera {(canvas.worldCamera ? canvas.worldCamera.name : "none")}, " +
                       $"plane hit {onPlane} at {enter:F1} m, local {local}, inside {rect.Contains(local)}, facing {Vector3.Dot(ray.direction, t.forward) > 0}");
        Logger.LogInfo("[UI] probe raycasters: " + string.Join(", ", canvas.GetComponents<BaseRaycaster>()
            .Select(r => $"{r.GetType().Name}{(r.enabled ? "" : " (off)")}")) +
            "; masks on the way down: " + string.Join(", ", canvas.GetComponentsInChildren<RectMask2D>(true).Take(5)
            .Select(m => $"{m.name} {m.enabled} {((RectTransform)m.transform).rect}")));

        var under = canvas.GetComponentsInChildren<Graphic>()
            .Where(g => g.raycastTarget && g.isActiveAndEnabled)
            .Where(g =>
            {
                var gl = g.rectTransform.InverseTransformPoint(point);
                return g.rectTransform.rect.Contains(new Vector2(gl.x, gl.y));
            })
            .Select(g => g.name).Take(8).ToList();
        Logger.LogInfo($"[UI] probe targets under the ray point: {(under.Count == 0 ? "none" : string.Join(", ", under))}");

        var blockers = Physics.RaycastAll(ray, 100f).OrderBy(h => h.distance).Take(5)
            .Select(h => $"{h.collider.name} (layer {h.collider.gameObject.layer}) @{h.distance:F1}");
        Logger.LogInfo($"[UI] probe colliders on the ray: {string.Join(", ", blockers)}");
    }

    private void OnDestroy()
    {
        Instance = null;
        
        leftActions.uiPressAction.action.performed -= LeftControllerPressed;
        rightActions.uiPressAction.action.performed -= RightControllerPressed;
    }

    public void SetVisible(bool visible)
    {
        leftInteractor.GetComponent<XRInteractorLineVisual>().enabled = visible;
        rightInteractor.GetComponent<XRInteractorLineVisual>().enabled = visible;
    }

    public void SetLineSortingOrder(int sortingOrder)
    {
        leftRenderer.sortingOrder = sortingOrder;
        rightRenderer.sortingOrder = sortingOrder;
    }
    
    private void LeftControllerPressed(InputAction.CallbackContext obj)
    {
        UpdateActiveController(ActiveController.Left);
    }

    private void RightControllerPressed(InputAction.CallbackContext obj)
    {
        UpdateActiveController(ActiveController.Right);
    }

    public Vector2 GetUIHitPosition(RectTransform? rect)
    {
        var (interactor, _) = GetActiveInteractor();
        if (!interactor.TryGetCurrentUIRaycastResult(out var result))
            return Vector2.one * -1000;

        var canvas = rect is null
            ? result.gameObject.GetComponentInParent<Canvas>().transform
            : rect.GetComponentInParent<Canvas>().transform;

        var local = canvas.InverseTransformPoint(result.worldPosition);
        return new Vector2(local.x, local.y);
    }

    public RectTransform GetUIHitRectTransform()
    {
        var (interactor, _) = GetActiveInteractor();
        return !interactor.TryGetCurrentUIRaycastResult(out var result)
            ? HUDCanvas.instance.GetComponent<RectTransform>()
            : result.gameObject.GetComponent<RectTransform>();
    }

    public bool GetTriggerDown()
    {
        var (_, actions) = GetActiveInteractor();
        return actions.uiPressAction.action.WasPressedThisFrame();
    }

    public bool GetTriggerUp()
    {
        var (_, actions) = GetActiveInteractor();
        return actions.uiPressAction.action.WasReleasedThisFrame();
    }

    public bool GetTriggerButton()
    {
        var (_, actions) = GetActiveInteractor();
        return actions.uiPressAction.action.IsPressed();
    }

    public float GetUIScrollX()
    {
        var (_, actions) = GetActiveInteractor();
        return actions.uiScrollAction.action.ReadValue<Vector2>().x;
    }

    public float GetUIScrollY()
    {
        var (_, actions) = GetActiveInteractor();
        return actions.uiScrollAction.action.ReadValue<Vector2>().y;
    }

    private void UpdateActiveController(ActiveController newValue)
    {
        activeController = newValue;
        
        var oldInteractor = newValue == ActiveController.Left ? rightInteractor : leftInteractor;
        var newInteractor = newValue == ActiveController.Left ? leftInteractor : rightInteractor;

        var oldVisual = oldInteractor.GetComponent<XRInteractorLineVisual>();
        var newVisual = newInteractor.GetComponent<XRInteractorLineVisual>();

        oldVisual.lineLength = 1;
        newVisual.lineLength = 20;

        oldVisual.invalidColorGradient = oldVisual.validColorGradient = new Gradient
        {
            mode = GradientMode.Blend,
            alphaKeys =
            [
                new GradientAlphaKey(0.05f, 0),
                new GradientAlphaKey(0.05f, 0.8f),
                new GradientAlphaKey(0f, 1),
            ],
            colorKeys =
            [
                new GradientColorKey(Color.white, 0),
                new GradientColorKey(Color.white, 1)
            ]
        };
        
        newVisual.invalidColorGradient = new Gradient
        {
            mode = GradientMode.Blend,
            alphaKeys =
            [
                new GradientAlphaKey(0.2f, 0),
                new GradientAlphaKey(0.2f, 1),
            ],
            colorKeys =
            [
                new GradientColorKey(Color.white, 0),
                new GradientColorKey(Color.white, 1)
            ]
        };
        
        newVisual.validColorGradient = new Gradient
        {
            mode = GradientMode.Blend,
            alphaKeys =
            [
                new GradientAlphaKey(1, 0),
                new GradientAlphaKey(1, 1),
            ],
            colorKeys =
            [
                new GradientColorKey(Color.white, 0),
                new GradientColorKey(Color.white, 1)
            ]
        };
    }

    public (XRRayInteractor, ActionBasedController) GetActiveInteractor()
    {
        return activeController switch
        {
            ActiveController.Left => (leftInteractor, leftActions),
            ActiveController.Right => (rightInteractor, rightActions),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private (ActionBasedController, XRRayInteractor, LineRenderer) CreateInteractorController(string hand)
    {
        var go = new GameObject($"{hand} Controller")
        {
            transform =
            {
                parent = transform
            },
            layer = 5
        };

        var controller = go.AddComponent<ActionBasedController>();
        var interactor = go.AddComponent<XRRayInteractor>();
        var visual = go.AddComponent<XRInteractorLineVisual>();
        var renderer = go.GetComponent<LineRenderer>();
        
        interactor.raycastMask = 1 << 5;
        // Quest port: R.E.P.O. 0.5.0's main menu room puts the menu canvas 20-25 m from the camera; XRI's default reach
        // is 30 m, a ray at an angle from the hand could fall short
        interactor.maxRaycastDistance = 100f;

        visual.lineBendRatio = 1;
        visual.invalidColorGradient = new Gradient
        {
            mode = GradientMode.Blend,
            alphaKeys =
            [
                new GradientAlphaKey(0.1f, 0),
                new GradientAlphaKey(0.1f, 1),
            ],
            colorKeys =
            [
                new GradientColorKey(Color.white, 0),
                new GradientColorKey(Color.white, 1)
            ]
        };
        visual.enabled = true;
        visual.lineLength = 20;
        renderer.sortingOrder = 4;

        renderer.material = AssetCollection.DefaultLine;
        
        AddActionBasedControllerBinds(controller, hand);
        
        return (controller, interactor, renderer);
    }

    private static void AddActionBasedControllerBinds(ActionBasedController controller, string hand)
    {
        controller.enableInputTracking = true;
        controller.positionAction =
            new InputActionProperty(hand == "Left"
                ? Actions.Instance.LeftHandPosition
                : Actions.Instance.RightHandPosition);
        controller.rotationAction =
            new InputActionProperty(hand == "Left"
                ? Actions.Instance.LeftHandRotation
                : Actions.Instance.RightHandRotation);
        controller.trackingStateAction = new InputActionProperty(hand == "Left"
            ? Actions.Instance.LeftHandTrackingState
            : Actions.Instance.RightHandTrackingState);

        controller.enableInputActions = true;
        controller.selectAction = new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Select"));
        controller.selectActionValue =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Select Value"));
        controller.activateAction =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Activate"));
        controller.activateActionValue =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Activate Value"));
        controller.uiPressAction = new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/UI Press"));
        controller.uiPressActionValue =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/UI Press Value"));
        controller.uiScrollAction =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/UI Scroll"));
        controller.rotateAnchorAction =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Rotate Anchor"));
        controller.translateAnchorAction =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Translate Anchor"));
        controller.scaleToggleAction =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Scale Toggle"));
        controller.scaleDeltaAction =
            new InputActionProperty(AssetCollection.DefaultXRActions.FindAction($"{hand}/Scale Delta"));
    }

    private enum ActiveController
    {
        Left,
        Right
    }
}