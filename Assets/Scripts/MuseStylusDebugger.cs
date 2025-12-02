using UnityEngine;

/// <summary>
/// Minimal example showing how to consume the Muse stylus wrapper and log
/// interesting state changes.
/// </summary>
public sealed class MuseStylusDebugger : MonoBehaviour
{
    [Tooltip("Optional object that will follow the stylus pose when present.")]
    public Transform debugVisual;

    [Tooltip("Scale applied to the raw stylus position to bring it into world space.")]
    public float positionScale = 1f;

    private void OnEnable()
    {
        MuseStylus.OnStylusConnected += HandleConnected;
        MuseStylus.OnStylusDisconnected += HandleDisconnected;
        MuseStylus.OnButtonsChanged += HandleButtons;
        MuseStylus.OnTipContactChanged += HandleTipContact;
        MuseStylus.OnTipHoverChanged += HandleTipHover;
    }

    private void OnDisable()
    {
        MuseStylus.OnStylusConnected -= HandleConnected;
        MuseStylus.OnStylusDisconnected -= HandleDisconnected;
        MuseStylus.OnButtonsChanged -= HandleButtons;
        MuseStylus.OnTipContactChanged -= HandleTipContact;
        MuseStylus.OnTipHoverChanged -= HandleTipHover;
    }

    private void Update()
    {
        var state = MuseStylus.CurrentState;
        if (!state.isAvailable)
            return;

        if (debugVisual != null)
        {
            debugVisual.SetPositionAndRotation(state.Position * positionScale, state.Orientation);
        }

        // Example: Log pressure continuously in Editor or Development builds.
        Debug.Log($"Stylus pressure: {state.pressure:F3}, buttons mask: 0x{state.buttons:X}");
    }

    private void HandleConnected() => Debug.Log("Muse Stylus connected");
    private void HandleDisconnected() => Debug.Log("Muse Stylus disconnected");
    private void HandleButtons(int mask) => Debug.Log($"Muse Stylus buttons changed: 0x{mask:X}");
    private void HandleTipContact(bool value) => Debug.Log($"Tip contact: {value}");
    private void HandleTipHover(bool value) => Debug.Log($"Tip hover: {value}");
}
