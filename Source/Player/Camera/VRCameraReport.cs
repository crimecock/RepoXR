using System.Linq;
using System.Text;
using RepoXR.Input;
using UnityEngine;

namespace RepoXR.Player.Camera;

/// <summary>
/// Quest port, diagnostics: every 2 seconds for the first 40 seconds of a scene, the head pose, the rotations along
/// the camera's parents and what drives the camera ("[Camera] ..."). Written to find why the view turned opposite to
/// the head after 0.5.0's scene handoff
/// </summary>
internal class VRCameraReport : MonoBehaviour
{
    private float next;
    private int reports;

    private void LateUpdate()
    {
        if (reports >= 20 || Time.unscaledTime < next)
            return;

        next = Time.unscaledTime + 2f;
        reports++;

        try
        {
            var text = new StringBuilder($"[Camera] {gameObject.scene.name}/{name}: ");
            var head = TrackingInput.Instance.HeadTransform;
            text.Append($"head local {head.localEulerAngles:F0}; chain");
            for (var t = transform; t; t = t.parent)
            {
                text.Append($" {t.name} {t.localEulerAngles:F0}");
                if (t.name == "Camera Controller")
                    break;
            }

            text.Append($"; world {transform.eulerAngles:F0}; drivers [");
            text.Append(string.Join(", ", GetComponents<Behaviour>().Select(b => $"{b.GetType().Name}{(b.enabled ? "" : " off")}")));
            text.Append($"]; stereo {GetComponent<UnityEngine.Camera>().stereoEnabled}, main {UnityEngine.Camera.main?.name}/{UnityEngine.Camera.main?.gameObject.scene.name}");

            if (VRCameraAim.instance is { } aim)
                text.Append($"; aim ({aim.gameObject.scene.name}) active {aim.IsActive}, y {aim.rotationY.eulerAngles:F0}, xz {aim.rotationXZ.eulerAngles:F0}");

            if (reports % 5 == 1)
            {
                text.Append("; cameras:");
                foreach (var camera in UnityEngine.Camera.allCameras)
                    text.Append($" {camera.name}({camera.gameObject.scene.name}, depth {camera.depth}, " +
                                $"{(camera.targetTexture ? "texture " + camera.targetTexture.name : "screen")}, eye {camera.stereoTargetEye}, " +
                                $"world {camera.transform.eulerAngles:F0})");
                text.Append("; scenes:");
                for (var i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                    text.Append($" {UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).name}");
            }

            Logger.LogInfo(text.ToString());
        }
        catch (System.Exception e)
        {
            Logger.LogWarning($"[Camera] report failed: {e.Message}");
        }
    }
}
