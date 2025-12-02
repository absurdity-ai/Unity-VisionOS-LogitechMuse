using Logitech.MuseStylus;
using UnityEngine;

/// Simple debug helper that prints stylus state changes.
public class MuseStylusDebugger : MonoBehaviour
{
    [SerializeField] private Transform debugProbe;

    private void OnEnable()
    {
        MuseStylus.OnStylusConnected += HandleConnected;
        MuseStylus.OnStylusDisconnected += HandleDisconnected;
        MuseStylus.OnButtonDown += HandleButtonDown;
        MuseStylus.OnButtonUp += HandleButtonUp;
        MuseStylus.OnTipContactChanged += HandleTip;
    }

    private void OnDisable()
    {
        MuseStylus.OnStylusConnected -= HandleConnected;
        MuseStylus.OnStylusDisconnected -= HandleDisconnected;
        MuseStylus.OnButtonDown -= HandleButtonDown;
        MuseStylus.OnButtonUp -= HandleButtonUp;
        MuseStylus.OnTipContactChanged -= HandleTip;
    }

    private void Update()
    {
        // Drive a debug transform with the reported pose.
        if (debugProbe != null && MuseStylus.IsAvailable)
        {
            var state = MuseStylus.CurrentState;
            debugProbe.position = state.position;
            debugProbe.rotation = state.orientation;
        }
    }

    private void HandleConnected() => Debug.Log("Muse stylus connected");
    private void HandleDisconnected() => Debug.Log("Muse stylus disconnected");
    private void HandleButtonDown(int index) => Debug.Log($"Button {index} down");
    private void HandleButtonUp(int index) => Debug.Log($"Button {index} up");
    private void HandleTip(bool touching) => Debug.Log(touching ? "Tip touching" : "Tip hovering");
}
