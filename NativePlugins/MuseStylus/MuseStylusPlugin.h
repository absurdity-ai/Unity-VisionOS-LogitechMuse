#ifndef MUSE_STYLUS_PLUGIN_H
#define MUSE_STYLUS_PLUGIN_H

#include <stdbool.h>
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

// Plain C representation of the stylus state shared with Unity via P/Invoke.
typedef struct MuseStylusState
{
    bool isAvailable;
    float positionX;
    float positionY;
    float positionZ;
    float orientationX;
    float orientationY;
    float orientationZ;
    float orientationW;
    float pressure;
    uint32_t buttons;
    bool isTipContact;
    bool isTipNearSurface;
} MuseStylusState;

// Callback types invoked from the plugin to Unity.
typedef void (*MuseStylusSimpleEventCallback)(void);
typedef void (*MuseStylusButtonEventCallback)(int32_t buttonMask);
typedef void (*MuseStylusBoolEventCallback)(bool value);

bool MuseStylus_IsAvailable(void);
void MuseStylus_GetState(MuseStylusState *outState);
void MuseStylus_Update(void);
void MuseStylus_SetEventCallbacks(MuseStylusSimpleEventCallback onConnect,
                                  MuseStylusSimpleEventCallback onDisconnect,
                                  MuseStylusButtonEventCallback onButtonsChanged,
                                  MuseStylusBoolEventCallback onTipContactChanged,
                                  MuseStylusBoolEventCallback onTipHoverChanged);

#ifdef __cplusplus
} // extern "C"
#endif

#endif // MUSE_STYLUS_PLUGIN_H
