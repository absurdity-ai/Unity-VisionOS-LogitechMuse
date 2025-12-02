# Unity-VisionOS-LogitechMuse
Unity 6.2 wrapper for https://developer.apple.com/documentation/gamecontroller/gcstylus

## Using the plugin
1. Place the contents of `NativePlugin/` into `Assets/Plugins/visionOS/` inside your Unity project. Ensure the Swift file is added to the generated Xcode project and the target links against **GameController.framework**. Unity will compile the Swift source as part of the visionOS Player build.
2. Add the C# scripts from `Assets/Scripts/` to your project. The `MuseStylus` class auto-installs a polling updater at runtime and exposes events for connection, buttons, and tip contact.
3. Optionally drop `MuseStylusDebugger` on a GameObject to print stylus state and drive a debug transform. Build for visionOS to exercise the plugin on-device.
