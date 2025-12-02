//
//  GCStylusWrapper.mm
//  Unity wrapper for Apple GameController GCStylus
//

#import <Foundation/Foundation.h>
#import <GameController/GameController.h>

#ifdef __cplusplus
extern "C" {
#endif

// Structure to hold stylus data
typedef struct {
    float touchX;
    float touchY;
    float azimuthAngle;
    float altitudeAngle;
    float force;
    bool isInRange;
    bool touching;
} StylusData;

// Global storage for stylus data
static StylusData currentStylusData = {0};
static GCStylus* _currentStylus = nil;

// Initialize stylus monitoring
void GCStylus_Initialize() {
    @autoreleasepool {
        // Monitor for stylus changes
        [[NSNotificationCenter defaultCenter] addObserverForName:GCControllerDidConnectNotification
                                                          object:nil
                                                           queue:[NSOperationQueue mainQueue]
                                                      usingBlock:^(NSNotification *note) {
            GCController *controller = note.object;
            if (controller.physicalInputProfile.buttons[GCInputButtonHome]) {
                // This is a device that supports stylus input
                if (@available(iOS 14.0, *)) {
                    // Check for stylus input
                }
            }
        }];
        
        // Try to get current stylus if available
        if (@available(iOS 14.0, *)) {
            for (GCController *controller in [GCController controllers]) {
                // GCStylus is accessed through device motion or specific input
                // For now, we'll set up the structure
            }
        }
    }
}

// Update stylus data (should be called each frame)
void GCStylus_Update() {
    @autoreleasepool {
        if (@available(iOS 14.0, *)) {
            // Get the current stylus instance
            // Note: GCStylus is typically accessed through touch events
            // This is a simplified version for demonstration
            
            if (_currentStylus != nil) {
                currentStylusData.azimuthAngle = _currentStylus.azimuthAngle;
                currentStylusData.altitudeAngle = _currentStylus.altitudeAngle;
                // Force is available through touch input
            }
        }
    }
}

// Get current stylus data
StylusData GCStylus_GetData() {
    return currentStylusData;
}

// Check if stylus is available
bool GCStylus_IsAvailable() {
    if (@available(iOS 14.0, *)) {
        return _currentStylus != nil;
    }
    return false;
}

// Get touch position X
float GCStylus_GetTouchX() {
    return currentStylusData.touchX;
}

// Get touch position Y
float GCStylus_GetTouchY() {
    return currentStylusData.touchY;
}

// Get azimuth angle (rotation around z-axis)
float GCStylus_GetAzimuthAngle() {
    return currentStylusData.azimuthAngle;
}

// Get altitude angle (tilt from surface)
float GCStylus_GetAltitudeAngle() {
    return currentStylusData.altitudeAngle;
}

// Get force/pressure
float GCStylus_GetForce() {
    return currentStylusData.force;
}

// Check if stylus is in range
bool GCStylus_IsInRange() {
    return currentStylusData.isInRange;
}

// Check if stylus is touching
bool GCStylus_IsTouching() {
    return currentStylusData.touching;
}

// Set stylus data (for touch event integration)
void GCStylus_SetData(float touchX, float touchY, float azimuth, float altitude, float force, bool inRange, bool touching) {
    currentStylusData.touchX = touchX;
    currentStylusData.touchY = touchY;
    currentStylusData.azimuthAngle = azimuth;
    currentStylusData.altitudeAngle = altitude;
    currentStylusData.force = force;
    currentStylusData.isInRange = inRange;
    currentStylusData.touching = touching;
}

// Cleanup
void GCStylus_Shutdown() {
    @autoreleasepool {
        [[NSNotificationCenter defaultCenter] removeObserver:nil];
        _currentStylus = nil;
    }
}

#ifdef __cplusplus
}
#endif
