import Foundation
import GameController
import simd

/// Internal manager that tracks the currently connected Logitech Muse stylus.
/// The manager listens for controller connection notifications and caches
/// the first controller that exposes a `stylus` profile.
final class MuseStylusManager {
    static let shared = MuseStylusManager()

    private let queue = DispatchQueue(label: "com.logitech.muse.stylus")
    private var stylus: GCStylus?

    private init() {
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(controllerDidConnect(_:)),
            name: .GCControllerDidBecomeCurrent,
            object: nil
        )
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(controllerDidDisconnect(_:)),
            name: .GCControllerDidStopBeingCurrent,
            object: nil
        )

        NotificationCenter.default.addObserver(
            self,
            selector: #selector(controllerDidConnect(_:)),
            name: .GCControllerDidConnect,
            object: nil
        )
        NotificationCenter.default.addObserver(
            self,
            selector: #selector(controllerDidDisconnect(_:)),
            name: .GCControllerDidDisconnect,
            object: nil
        )

        // Prime the cache with any already-connected controllers.
        updateCurrentStylus()
    }

    @objc private func controllerDidConnect(_ notification: Notification) {
        updateCurrentStylus()
    }

    @objc private func controllerDidDisconnect(_ notification: Notification) {
        // Drop the cached stylus and search again in case another stylus exists.
        stylus = nil
        updateCurrentStylus()
    }

    /// Returns the currently tracked GCStylus instance, if any.
    func currentStylus() -> GCStylus? {
        var result: GCStylus?
        queue.sync {
            result = stylus
        }
        return result
    }

    /// Scans the connected controllers for a stylus profile and caches the first one.
    private func updateCurrentStylus() {
        queue.sync {
            // If we already have a stylus and it is still attached, keep it.
            if let cached = stylus, cached.controller?.isAttachedToDevice == true {
                return
            }

            let controllers = GCController.controllers()
            for controller in controllers {
                if let stylusProfile = controller.stylus {
                    stylus = stylusProfile
                    return
                }
            }
            stylus = nil
        }
    }
}

// MARK: - C Exports

/// Returns true when a stylus is connected.
@_cdecl("MuseStylus_IsAvailable")
public func MuseStylus_IsAvailable() -> Bool {
    return MuseStylusManager.shared.currentStylus() != nil
}

/// Populates a C-compatible struct with the current stylus state.
/// Safe to call even when no stylus is present; the struct will be zeroed.
@_cdecl("MuseStylus_GetState")
public func MuseStylus_GetState(_ outState: UnsafeMutablePointer<MuseStylusState>?) {
    guard let outState = outState else { return }

    var state = MuseStylusState(
        position: MuseStylusVector3(x: 0, y: 0, z: 0),
        orientation: MuseStylusQuaternion(x: 0, y: 0, z: 0, w: 1),
        pressure: 0,
        buttons: 0,
        tipContact: 0,
        hovering: 0
    )

    guard let stylus = MuseStylusManager.shared.currentStylus() else {
        outState.pointee = state
        return
    }

    // Position and orientation come from the stylus pose reported by GameController.
    if let pose = stylus.pose {
        let pos = pose.position
        state.position = MuseStylusVector3(x: pos.x, y: pos.y, z: pos.z)

        // Convert the simd quaternion to our C-friendly struct.
        let quat = pose.orientation
        state.orientation = MuseStylusQuaternion(x: quat.imag.x, y: quat.imag.y, z: quat.imag.z, w: quat.real)
    }

    // Pressure is normalized 0..1. Clamp to avoid NaN when absent.
    state.pressure = Float(stylus.pressure)

    // The Logitech Muse has at least a primary button; map any available buttons to a bitmask.
    var mask: UInt32 = 0
    if stylus.primaryButton?.isPressed == true {
        mask |= 1 << 0
    }
    if stylus.secondaryButton?.isPressed == true {
        mask |= 1 << 1
    }
    state.buttons = mask

    state.tipContact = stylus.isTouchingSurface ? 1 : 0
    state.hovering = stylus.isInRange ? 1 : 0

    outState.pointee = state
}
