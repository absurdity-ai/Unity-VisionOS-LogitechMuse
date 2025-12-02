# Integration Guide

## Integrating with Unity's Touch System

Since Apple's GCStylus data is accessed through UITouch events on iOS/visionOS, you'll need to integrate the wrapper with Unity's existing touch input system. Here's how:

### Basic Integration

The most common approach is to use Unity's `Input.touches` array and manually set the stylus data when a touch is detected:

```csharp
using UnityEngine;
using AbsurdityAI.VisionOS.Input;

public class StylusIntegration : MonoBehaviour
{
    void Start()
    {
        GCStylus.Initialize();
    }

    void Update()
    {
        GCStylus.Update();
        
        // Process Unity touches and update stylus data
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            // On iOS/visionOS, you can access additional properties
            // through platform-specific code or plugins
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // Update stylus data with touch information
                // Note: You'll need to get azimuth/altitude from native touch events
                GCStylus.SetData(
                    touch.position.x,
                    touch.position.y,
                    0f,  // azimuth - requires native touch integration
                    0f,  // altitude - requires native touch integration
                    touch.pressure,
                    true,  // in range when touching
                    touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved
                );
            }
        }
    }
}
```

### Advanced: Native Touch Event Integration

For full access to GCStylus data, you need to hook into native UITouch events. This requires extending the native plugin:

1. Create a Unity view controller subclass in native code
2. Override touch event methods (`touchesBegan`, `touchesMoved`, etc.)
3. Extract stylus properties from UITouch objects
4. Pass data to the Unity C# wrapper

Example native code (to be added to the plugin):

```objc
- (void)touchesBegan:(NSSet<UITouch *> *)touches withEvent:(UIEvent *)event {
    [super touchesBegan:touches withEvent:event];
    
    UITouch *touch = [touches anyObject];
    if (@available(iOS 12.1, *)) {
        // Extract stylus data from UITouch
        CGPoint location = [touch locationInView:self.view];
        CGFloat azimuth = [touch azimuthAngleInView:self.view];
        CGFloat altitude = touch.altitudeAngle;
        CGFloat force = touch.force / touch.maximumPossibleForce;
        
        // Send to Unity wrapper
        GCStylus_SetData(
            location.x,
            location.y,
            azimuth,
            altitude,
            force,
            true,  // in range
            true   // touching
        );
    }
}
```

### Using with Unity's New Input System

If you're using Unity's Input System package, you can create a custom input device:

```csharp
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using AbsurdityAI.VisionOS.Input;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(displayName = "Apple Stylus")]
public class AppleStylusDevice : InputDevice
{
    public Vector2Control position { get; private set; }
    public AxisControl azimuth { get; private set; }
    public AxisControl altitude { get; private set; }
    public AxisControl pressure { get; private set; }
    public ButtonControl inRange { get; private set; }
    public ButtonControl tip { get; private set; }

    static AppleStylusDevice()
    {
        InputSystem.RegisterLayout<AppleStylusDevice>();
    }

    protected override void FinishSetup()
    {
        base.FinishSetup();
        
        position = GetChildControl<Vector2Control>("position");
        azimuth = GetChildControl<AxisControl>("azimuth");
        altitude = GetChildControl<AxisControl>("altitude");
        pressure = GetChildControl<AxisControl>("pressure");
        inRange = GetChildControl<ButtonControl>("inRange");
        tip = GetChildControl<ButtonControl>("tip");
    }

    public void UpdateFromGCStylus()
    {
        var data = GCStylus.GetData();
        
        InputState.Change(position, new Vector2(data.touchX, data.touchY));
        InputState.Change(azimuth, data.azimuthAngle);
        InputState.Change(altitude, data.altitudeAngle);
        InputState.Change(pressure, data.force);
        InputState.Change(inRange, data.isInRange ? 1f : 0f);
        InputState.Change(tip, data.touching ? 1f : 0f);
    }
}
```

## Best Practices

1. **Initialize Once**: Call `GCStylus.Initialize()` once at app startup
2. **Update Per Frame**: Call `GCStylus.Update()` in your Update loop
3. **Check Availability**: Always check `GCStylus.IsAvailable()` before accessing data
4. **Platform Guards**: Use `#if UNITY_IOS || UNITY_VISIONOS` for platform-specific code
5. **Cleanup**: Call `GCStylus.Shutdown()` when your app closes

## Troubleshooting

### Data Always Returns Zero

This usually means the native touch events aren't being captured. You need to:
- Implement native touch event handlers
- Call `GCStylus.SetData()` from those handlers
- Or integrate with a Unity touch/input plugin that exposes stylus data

### Angles in Wrong Units

Remember that azimuth and altitude angles are in radians, not degrees. Convert using:
```csharp
float degrees = Mathf.Rad2Deg * radians;
```

### Performance Issues

- Don't call `GCStylus.GetData()` multiple times per frame
- Cache the data at the start of Update if you need it multiple times
- Use events from `GCStylusManager` rather than polling when possible
