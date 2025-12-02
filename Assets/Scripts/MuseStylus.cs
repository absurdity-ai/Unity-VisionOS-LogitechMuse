using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Logitech.MuseStylus
{
    /// Managed mirror of the native MuseStylusState struct.
    [StructLayout(LayoutKind.Sequential)]
    public struct MuseStylusState
    {
        public Vector3 position;
        public Quaternion orientation;
        public float pressure;
        public uint buttons;
        public byte tipContact;
        public byte hovering;

        public bool IsTipContact => tipContact != 0;
        public bool IsHovering => hovering != 0;
        public bool IsButtonPressed(int index) => (buttons & (1u << index)) != 0;
    }

    /// Static helper that polls the native plugin and raises events for Unity scripts.
    public static class MuseStylus
    {
        private const string PluginName = "__Internal";

        [DllImport(PluginName)]
        private static extern bool MuseStylus_IsAvailable();

        [DllImport(PluginName)]
        private static extern void MuseStylus_GetState(out MuseStylusState state);

        private static bool _lastAvailability;
        private static MuseStylusState _currentState;
        private static MuseStylusState _previousState;

        public static event Action OnStylusConnected;
        public static event Action OnStylusDisconnected;
        public static event Action<int> OnButtonDown;
        public static event Action<int> OnButtonUp;
        public static event Action<bool> OnTipContactChanged; // true when touching

        /// The last polled stylus state; safe to read even when unavailable.
        public static MuseStylusState CurrentState => _currentState;

        /// True when the native plugin reports a stylus connection.
        public static bool IsAvailable => _lastAvailability;

        /// Ensures a polling behaviour exists in the scene.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureUpdater()
        {
            var obj = new GameObject("MuseStylusUpdater");
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            obj.AddComponent<MuseStylusUpdater>();
        }

        private static void Poll()
        {
            _previousState = _currentState;
            _currentState = default;

            var available = false;
            try
            {
                available = MuseStylus_IsAvailable();
            }
            catch (DllNotFoundException)
            {
                // When running in the editor without the plugin present we treat it as unavailable.
                available = false;
            }

            if (available)
            {
                MuseStylus_GetState(out _currentState);
            }

            HandleEvents(available);
        }

        private static void HandleEvents(bool available)
        {
            if (available != _lastAvailability)
            {
                if (available)
                    OnStylusConnected?.Invoke();
                else
                    OnStylusDisconnected?.Invoke();
            }

            // Button edges
            for (int i = 0; i < 8; i++)
            {
                var wasDown = (_previousState.buttons & (1u << i)) != 0;
                var isDown = (_currentState.buttons & (1u << i)) != 0;
                if (wasDown != isDown)
                {
                    if (isDown) OnButtonDown?.Invoke(i);
                    else OnButtonUp?.Invoke(i);
                }
            }

            // Tip contact transitions
            if (_previousState.tipContact != _currentState.tipContact)
            {
                OnTipContactChanged?.Invoke(_currentState.tipContact != 0);
            }

            _lastAvailability = available;
        }

        private sealed class MuseStylusUpdater : MonoBehaviour
        {
            private void Update()
            {
                Poll();
            }
        }
    }
}
