# Unity-VisionOS-LogitechMuse
Unity 6.2 wrapper for https://developer.apple.com/documentation/gamecontroller/gcstylus

## Integration
1. Copy `NativePlugins/MuseStylus/MuseStylusPlugin.swift` and `MuseStylusPlugin.h` into the generated Xcode project under a target that builds only for visionOS. Make sure the files are included in the player target and that the GameController framework is linked.
2. Place the C# scripts inside your Unity project (for example, `Assets/Scripts/MuseStylus.cs` and `Assets/Scripts/MuseStylusDebugger.cs`). The wrapper uses `DllImport("__Internal")`, so no additional editor configuration is required beyond ensuring the native plugin is present for the visionOS build.
3. Add `MuseStylusDebugger` to a scene to visualize stylus input or subscribe to the static events on `MuseStylus` for connection, button, and tip contact notifications. The `MuseStylus` singleton automatically creates a hidden updater to poll native data every frame.
