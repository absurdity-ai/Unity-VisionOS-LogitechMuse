# API Reference

## Namespace: AbsurdityAI.VisionOS.Input

### GCStylus (Static Class)

Main static class for accessing GCStylus functionality.

#### Methods

##### `static void Initialize()`
Initializes the GCStylus wrapper. Must be called once before using any other methods.

**Example:**
```csharp
void Start() {
    GCStylus.Initialize();
}
```

##### `static void Update()`
Updates the internal stylus data. Should be called once per frame.

**Example:**
```csharp
void Update() {
    GCStylus.Update();
}
```

##### `static StylusData GetData()`
Returns all stylus data in a single structure.

**Returns:** `StylusData` struct containing all stylus information

**Example:**
```csharp
StylusData data = GCStylus.GetData();
Debug.Log($"Position: {data.touchX}, {data.touchY}");
```

##### `static bool IsAvailable()`
Checks if a stylus is currently available.

**Returns:** `bool` - true if stylus is available

**Example:**
```csharp
if (GCStylus.IsAvailable()) {
    // Use stylus data
}
```

##### `static void SetData(float touchX, float touchY, float azimuth, float altitude, float force, bool inRange, bool touching)`
Manually sets stylus data. Used for integration with native touch events.

**Parameters:**
- `touchX` - X coordinate of touch position
- `touchY` - Y coordinate of touch position
- `azimuth` - Azimuth angle in radians
- `altitude` - Altitude angle in radians
- `force` - Force/pressure (0.0 to 1.0)
- `inRange` - True if stylus is in range
- `touching` - True if stylus is touching

##### `static void Shutdown()`
Cleans up resources. Should be called when shutting down.

**Example:**
```csharp
void OnDestroy() {
    GCStylus.Shutdown();
}
```

#### Properties

##### `static float TouchX { get; }`
Gets the X coordinate of the touch position.

##### `static float TouchY { get; }`
Gets the Y coordinate of the touch position.

##### `static Vector2 TouchPosition { get; }`
Gets the touch position as a Vector2.

**Example:**
```csharp
Vector2 pos = GCStylus.TouchPosition;
```

##### `static float AzimuthAngle { get; }`
Gets the azimuth angle (rotation around z-axis) in radians.

**Range:** 0 to 2π radians (0° to 360°)

**Example:**
```csharp
float angleDegrees = GCStylus.AzimuthAngle * Mathf.Rad2Deg;
```

##### `static float AltitudeAngle { get; }`
Gets the altitude angle (tilt from surface) in radians.

**Range:** 0 to π/2 radians (0° to 90°)

**Example:**
```csharp
float tiltDegrees = GCStylus.AltitudeAngle * Mathf.Rad2Deg;
```

##### `static float Force { get; }`
Gets the force/pressure applied by the stylus.

**Range:** 0.0 to 1.0

**Example:**
```csharp
float pressure = GCStylus.Force;
float brushSize = 1.0f + pressure * 2.0f; // Scale brush with pressure
```

##### `static bool IsInRange { get; }`
Checks if the stylus is in range of the screen (hovering).

**Example:**
```csharp
if (GCStylus.IsInRange) {
    // Show cursor or preview
}
```

##### `static bool IsTouching { get; }`
Checks if the stylus is currently touching the screen.

**Example:**
```csharp
if (GCStylus.IsTouching) {
    // Draw stroke
}
```

---

### StylusData (Struct)

Structure containing all stylus data.

#### Fields

- `float touchX` - X coordinate of touch
- `float touchY` - Y coordinate of touch
- `float azimuthAngle` - Azimuth angle in radians
- `float altitudeAngle` - Altitude angle in radians
- `float force` - Force/pressure (0.0 to 1.0)
- `bool isInRange` - True if in range
- `bool touching` - True if touching

**Example:**
```csharp
StylusData data = GCStylus.GetData();
if (data.touching) {
    DrawAt(data.touchX, data.touchY, data.force);
}
```

---

### GCStylusManager (MonoBehaviour)

Component for easy integration with Unity scenes.

#### Events

##### `event Action OnTouchBegan`
Triggered when stylus starts touching the screen.

**Example:**
```csharp
stylusManager.OnTouchBegan += () => {
    Debug.Log("Touch began!");
    StartNewStroke();
};
```

##### `event Action OnTouchEnded`
Triggered when stylus stops touching the screen.

**Example:**
```csharp
stylusManager.OnTouchEnded += () => {
    Debug.Log("Touch ended!");
    FinishStroke();
};
```

##### `event Action OnStylusEntered`
Triggered when stylus enters range of the screen.

**Example:**
```csharp
stylusManager.OnStylusEntered += () => {
    cursorObject.SetActive(true);
};
```

##### `event Action OnStylusExited`
Triggered when stylus exits range of the screen.

**Example:**
```csharp
stylusManager.OnStylusExited += () => {
    cursorObject.SetActive(false);
};
```

#### Properties

##### `StylusData CurrentData { get; }`
Gets the current stylus data.

**Example:**
```csharp
StylusData data = stylusManager.CurrentData;
```

##### `bool IsAvailable { get; }`
Checks if stylus is available.

**Example:**
```csharp
if (stylusManager.IsAvailable) {
    // Process stylus input
}
```

#### Inspector Settings

##### `bool initializeOnAwake`
If true, initializes GCStylus automatically on Awake.

**Default:** `true`

##### `bool updateEveryFrame`
If true, updates stylus data every frame.

**Default:** `true`

##### `bool showDebugInfo`
If true, logs debug information to console.

**Default:** `false`

##### `bool drawGizmos`
If true, draws stylus position in Scene view.

**Default:** `true`

---

## Platform Availability

All APIs are available on:
- **iOS 14.0+**
- **visionOS 1.0+**

On other platforms, the APIs will return default values and log warnings.

## Thread Safety

All methods should be called from Unity's main thread. The native plugin uses main queue handlers for thread safety.
