#ifndef MUSE_STYLUS_PLUGIN_H
#define MUSE_STYLUS_PLUGIN_H

#include <stdint.h>
#include <stdbool.h>

#ifdef __cplusplus
extern "C" {
#endif

// Simple vector/quaternion helpers shared between Swift and C# via P/Invoke
typedef struct {
    float x;
    float y;
    float z;
} MuseStylusVector3;

typedef struct {
    float x;
    float y;
    float z;
    float w;
} MuseStylusQuaternion;

// Aggregate state returned to Unity each frame
typedef struct {
    MuseStylusVector3 position;   // Stylus position in meters (visionOS world space)
    MuseStylusQuaternion orientation; // Orientation as a quaternion (right-handed)
    float pressure;               // Normalized pressure 0..1
    uint32_t buttons;             // Bitmask for any hardware buttons on the stylus
    uint8_t tipContact;           // 1 if the tip is touching a surface
    uint8_t hovering;             // 1 if the stylus is in range/hovering
} MuseStylusState;

// Returns true when a Logitech Muse stylus is currently available.
bool MuseStylus_IsAvailable(void);

// Writes the current stylus state into outState. Safe to call even when unavailable.
void MuseStylus_GetState(MuseStylusState *outState);

#ifdef __cplusplus
}
#endif

#endif // MUSE_STYLUS_PLUGIN_H
