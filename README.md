# Unity VisionOS Logitech Muse - GCStylus Wrapper

Unity wrapper for Apple's [GCStylus API](https://developer.apple.com/documentation/gamecontroller/gcstylus) from the GameController framework.

This package enables support for Apple Pencil and other stylus input devices on iOS and visionOS platforms within Unity.

## Features

- ✅ Full access to stylus touch position, orientation, and pressure
- ✅ Event-driven callbacks for touch and range detection
- ✅ Easy-to-use MonoBehaviour component
- ✅ Cross-platform support (iOS, visionOS)
- ✅ Comprehensive API with both static and component-based access

## Quick Start

```csharp
using AbsurdityAI.VisionOS.Input;

// Initialize
GCStylus.Initialize();

// In Update loop
GCStylus.Update();
if (GCStylus.IsTouching)
{
    Vector2 pos = GCStylus.TouchPosition;
    float force = GCStylus.Force;
    // Use stylus data...
}
```

## Documentation

Full documentation is available in the [Documentation~/README.md](Documentation~/README.md) file.

## Installation

Via Unity Package Manager:
```
https://github.com/absurdity-ai/Unity-VisionOS-LogitechMuse.git
```

## Requirements

- Unity 2022.3+
- iOS 14.0+ or visionOS 1.0+
- Apple device with stylus support

## License

See LICENSE file for details.
