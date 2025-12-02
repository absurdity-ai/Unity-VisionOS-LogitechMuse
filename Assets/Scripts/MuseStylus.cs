using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

/// <summary>
/// Unity-facing wrapper for the native Logitech Muse stylus plugin. Provides
/// connection state, per-frame stylus data, and managed events.
/// </summary>
public static class MuseStylus
{
    [StructLayout(LayoutKind.Sequential)]
    public struct State
    {
        [MarshalAs(UnmanagedType.I1)] public bool isAvailable;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float orientationX;
        public float orientationY;
        public float orientationZ;
        public float orientationW;
        public float pressure;
        public uint buttons;
        [MarshalAs(UnmanagedType.I1)] public bool isTipContact;
        [MarshalAs(UnmanagedType.I1)] public bool isTipNearSurface;

        public Vector3 Position => new(positionX, positionY, positionZ);
        public Quaternion Orientation => new(orientationX, orientationY, orientationZ, orientationW);
    }

    private const string DllName = "__Internal";

    [DllImport(DllName)] private static extern bool MuseStylus_IsAvailable();
    [DllImport(DllName)] private static extern void MuseStylus_GetState(out State state);
    [DllImport(DllName)] private static extern void MuseStylus_Update();
    [DllImport(DllName)] private static extern void MuseStylus_SetEventCallbacks(
        StylusSimpleEvent onConnect,
        StylusSimpleEvent onDisconnect,
        StylusButtonEvent onButtonsChanged,
        StylusBoolEvent onTipContactChanged,
        StylusBoolEvent onTipHoverChanged);

    private delegate void StylusSimpleEvent();
    private delegate void StylusButtonEvent(int mask);
    private delegate void StylusBoolEvent(bool value);

    public static event Action OnStylusConnected;
    public static event Action OnStylusDisconnected;
    public static event Action<int> OnButtonsChanged;
    public static event Action<bool> OnTipContactChanged;
    public static event Action<bool> OnTipHoverChanged;

    public static bool IsAvailable => MuseStylus_IsAvailable();
    public static State CurrentState { get; private set; }

    private static readonly StylusSimpleEvent connectCb = OnNativeConnect;
    private static readonly StylusSimpleEvent disconnectCb = OnNativeDisconnect;
    private static readonly StylusButtonEvent buttonsCb = OnNativeButtons;
    private static readonly StylusBoolEvent tipContactCb = OnNativeTipContact;
    private static readonly StylusBoolEvent tipHoverCb = OnNativeTipHover;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        MuseStylus_SetEventCallbacks(connectCb, disconnectCb, buttonsCb, tipContactCb, tipHoverCb);
        EnsureUpdater();
    }

    internal static void Tick()
    {
        MuseStylus_Update();
        MuseStylus_GetState(out var state);
        var wasAvailable = CurrentState.isAvailable;
        CurrentState = state;

        // In case callbacks were missed, also raise connection transitions here.
        if (!wasAvailable && state.isAvailable)
            OnStylusConnected?.Invoke();
        else if (wasAvailable && !state.isAvailable)
            OnStylusDisconnected?.Invoke();
    }

    [MonoPInvokeCallback(typeof(StylusSimpleEvent))]
    private static void OnNativeConnect() => OnStylusConnected?.Invoke();

    [MonoPInvokeCallback(typeof(StylusSimpleEvent))]
    private static void OnNativeDisconnect() => OnStylusDisconnected?.Invoke();

    [MonoPInvokeCallback(typeof(StylusButtonEvent))]
    private static void OnNativeButtons(int mask) => OnButtonsChanged?.Invoke(mask);

    [MonoPInvokeCallback(typeof(StylusBoolEvent))]
    private static void OnNativeTipContact(bool value) => OnTipContactChanged?.Invoke(value);

    [MonoPInvokeCallback(typeof(StylusBoolEvent))]
    private static void OnNativeTipHover(bool value) => OnTipHoverChanged?.Invoke(value);

    /// <summary>
    /// Ensures a hidden updater MonoBehaviour exists in the scene to poll the native plugin each frame.
    /// </summary>
    private static void EnsureUpdater()
    {
        if (UnityEngine.Object.FindObjectOfType<MuseStylusUpdater>() != null)
            return;

        var go = new GameObject("MuseStylusUpdater");
        go.hideFlags = HideFlags.HideAndDontSave;
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<MuseStylusUpdater>();
    }
}

/// <summary>
/// MonoBehaviour that ticks the native plugin every frame. Created automatically
/// by <see cref="MuseStylus"/> on startup.
/// </summary>
public sealed class MuseStylusUpdater : MonoBehaviour
{
    private void Update()
    {
        MuseStylus.Tick();
    }
}
