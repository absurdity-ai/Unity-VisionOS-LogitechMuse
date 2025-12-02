# Unity VisionOS Logitech Muse - GCStylus Wrapper

A Unity wrapper for Apple's [GCStylus](https://developer.apple.com/documentation/gamecontroller/gcstylus) API from the GameController framework. This package enables support for Apple Pencil and other stylus input devices on iOS and visionOS platforms.

## Features

- ✅ Access to stylus touch position
- ✅ Azimuth angle (rotation around z-axis)
- ✅ Altitude angle (tilt from surface)
- ✅ Force/pressure sensing
- ✅ In-range detection
- ✅ Touch state tracking
- ✅ Event-driven callbacks
- ✅ Easy-to-use MonoBehaviour component
- ✅ Cross-platform compatible (iOS, visionOS)

## Installation

### Via Unity Package Manager

1. Open Unity Package Manager (Window > Package Manager)
2. Click the "+" button and select "Add package from git URL"
3. Enter: `https://github.com/absurdity-ai/Unity-VisionOS-LogitechMuse.git`

### Via package.json

Add this to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.absurdity.visionos.logitechmuse": "https://github.com/absurdity-ai/Unity-VisionOS-LogitechMuse.git"
  }
}
```

## Quick Start

### Method 1: Using GCStylusManager Component (Recommended)

1. Add the `GCStylusManager` component to a GameObject in your scene:

```csharp
using UnityEngine;
using AbsurdityAI.VisionOS.Input;

public class MyScript : MonoBehaviour
{
    private GCStylusManager stylusManager;

    void Start()
    {
        stylusManager = GetComponent<GCStylusManager>();
        
        // Subscribe to events
        stylusManager.OnTouchBegan += HandleTouchBegan;
        stylusManager.OnTouchEnded += HandleTouchEnded;
    }

    void Update()
    {
        if (stylusManager.IsAvailable)
        {
            StylusData data = stylusManager.CurrentData;
            Debug.Log($"Position: {data.touchX}, {data.touchY}");
            Debug.Log($"Force: {data.force}");
        }
    }

    void HandleTouchBegan()
    {
        Debug.Log("Stylus touch began!");
    }

    void HandleTouchEnded()
    {
        Debug.Log("Stylus touch ended!");
    }
}
```

### Method 2: Using Static API

```csharp
using UnityEngine;
using AbsurdityAI.VisionOS.Input;

public class MyScript : MonoBehaviour
{
    void Start()
    {
        // Initialize once
        GCStylus.Initialize();
    }

    void Update()
    {
        // Update every frame
        GCStylus.Update();

        if (GCStylus.IsAvailable())
        {
            // Access individual properties
            Vector2 touchPos = GCStylus.TouchPosition;
            float force = GCStylus.Force;
            float azimuth = GCStylus.AzimuthAngle;
            float altitude = GCStylus.AltitudeAngle;
            bool isTouching = GCStylus.IsTouching;
            bool isInRange = GCStylus.IsInRange;

            // Or get all data at once
            StylusData data = GCStylus.GetData();
        }
    }

    void OnDestroy()
    {
        GCStylus.Shutdown();
    }
}
```

## API Reference

### GCStylus (Static Class)

#### Methods

- `Initialize()` - Initialize the stylus wrapper (call once at startup)
- `Update()` - Update stylus data (call each frame)
- `GetData()` - Get all stylus data as a StylusData struct
- `IsAvailable()` - Check if stylus is currently available
- `SetData(...)` - Manually set stylus data (for custom integration)
- `Shutdown()` - Cleanup resources

#### Properties

- `TouchX` (float) - X coordinate of touch position
- `TouchY` (float) - Y coordinate of touch position
- `TouchPosition` (Vector2) - Touch position as Vector2
- `AzimuthAngle` (float) - Rotation around z-axis in radians
- `AltitudeAngle` (float) - Tilt from surface in radians
- `Force` (float) - Pressure/force (0.0 to 1.0)
- `IsInRange` (bool) - True if stylus is near the screen
- `IsTouching` (bool) - True if stylus is touching the screen

### StylusData (Struct)

```csharp
public struct StylusData
{
    public float touchX;
    public float touchY;
    public float azimuthAngle;    // Radians
    public float altitudeAngle;   // Radians
    public float force;           // 0.0 to 1.0
    public bool isInRange;
    public bool touching;
}
```

### GCStylusManager (MonoBehaviour)

#### Events

- `OnTouchBegan` - Triggered when stylus starts touching
- `OnTouchEnded` - Triggered when stylus stops touching
- `OnStylusEntered` - Triggered when stylus enters range
- `OnStylusExited` - Triggered when stylus exits range

#### Properties

- `CurrentData` (StylusData) - Current stylus data
- `IsAvailable` (bool) - True if stylus is available

#### Inspector Settings

- `initializeOnAwake` - Auto-initialize on Awake
- `updateEveryFrame` - Auto-update every frame
- `showDebugInfo` - Log debug info to console
- `drawGizmos` - Draw stylus position in Scene view

## Platform Support

- **iOS**: iOS 14.0+
- **visionOS**: visionOS 1.0+
- **Editor**: Simulation mode (returns default values)

## Requirements

- Unity 2022.3 or later
- iOS/visionOS build support installed
- Apple device with stylus support (iPad with Apple Pencil, etc.)

## Examples

See the included `GCStylusExample.cs` script for a complete example demonstrating:
- Event handling
- 3D visualization of stylus position and orientation
- Force-based scaling
- Stroke rendering

## Technical Details

### Native Integration

The package includes a native Objective-C++ plugin (`GCStylusWrapper.mm`) that interfaces with Apple's GameController framework. The plugin:

1. Monitors for controller connections
2. Accesses GCStylus data
3. Provides C-compatible functions for Unity

### Thread Safety

All native calls are wrapped with `@autoreleasepool` blocks and should be called from the main thread.

### Performance

The wrapper is lightweight and suitable for real-time updates. Call `GCStylus.Update()` once per frame in your Update loop.

## Troubleshooting

### "GCStylus is only available on iOS and visionOS platforms"

This warning appears when running in the editor or on unsupported platforms. This is expected behavior - the API will return default values on these platforms.

### Stylus not detected

1. Ensure your device supports stylus input (e.g., iPad with Apple Pencil)
2. Check that the Apple Pencil is paired with your device
3. Verify that your app has the necessary permissions
4. Make sure you called `GCStylus.Initialize()` before use

### Build Issues

If you encounter build errors:
1. Ensure iOS/visionOS build support is installed in Unity Hub
2. Check that the plugin is properly configured in the Plugins/iOS folder
3. Verify the GameController framework is linked in Xcode project settings

## License

This package is provided as-is. Please refer to the repository license for details.

## Contributing

Contributions are welcome! Please submit issues and pull requests on the GitHub repository.

## Credits

Developed by Absurdity AI

Apple, Apple Pencil, GameController, and GCStylus are trademarks of Apple Inc.
