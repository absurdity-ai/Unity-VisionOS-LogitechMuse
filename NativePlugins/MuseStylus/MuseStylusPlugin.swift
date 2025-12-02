import Foundation
import GameController
import simd

// Mirror of the C struct defined in MuseStylusPlugin.h. The field order and types
// must stay aligned with the C definition and the C# struct used by Unity.
public struct MuseStylusState {
    public var isAvailable: Bool = false
    public var positionX: Float = 0
    public var positionY: Float = 0
    public var positionZ: Float = 0
    public var orientationX: Float = 0
    public var orientationY: Float = 0
    public var orientationZ: Float = 0
    public var orientationW: Float = 1
    public var pressure: Float = 0
    public var buttons: UInt32 = 0
    public var isTipContact: Bool = false
    public var isTipNearSurface: Bool = false
}

// Callback type aliases matching the C header.
typealias MuseStylusSimpleEventCallback = @convention(c) () -> Void
typealias MuseStylusButtonEventCallback = @convention(c) (Int32) -> Void
typealias MuseStylusBoolEventCallback = @convention(c) (Bool) -> Void

// Manager that watches GCController notifications and keeps track of the current stylus.
final class MuseStylusManager {
    static let shared = MuseStylusManager()

    private var stylus: GCStylus?
    private var state = MuseStylusState()
    private var lastButtonMask: Int32 = 0
    private var lastTipContact: Bool = false
    private var lastTipHover: Bool = false
    private var callbacks: (connect: MuseStylusSimpleEventCallback?,
                            disconnect: MuseStylusSimpleEventCallback?,
                            buttons: MuseStylusButtonEventCallback?,
                            tipContact: MuseStylusBoolEventCallback?,
                            tipHover: MuseStylusBoolEventCallback?) = (nil, nil, nil, nil, nil)

    private init() {
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(controllerDidConnect(_:)),
                                               name: .GCControllerDidBecomeCurrent,
                                               object: nil)
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(controllerDidConnect(_:)),
                                               name: .GCControllerDidConnect,
                                               object: nil)
        NotificationCenter.default.addObserver(self,
                                               selector: #selector(controllerDidDisconnect(_:)),
                                               name: .GCControllerDidDisconnect,
                                               object: nil)
        updateStylusReferenceFromConnectedControllers()
    }

    @objc private func controllerDidConnect(_ notification: Notification) {
        updateStylusReferenceFromConnectedControllers()
    }

    @objc private func controllerDidDisconnect(_ notification: Notification) {
        if let controller = notification.object as? GCController, controller.stylus == stylus {
            stylus = nil
            state = MuseStylusState()
            callbacks.disconnect?()
        }
        updateStylusReferenceFromConnectedControllers()
    }

    private func updateStylusReferenceFromConnectedControllers() {
        if let found = GCController.controllers().first(where: { $0.stylus != nil })?.stylus {
            if stylus !== found {
                stylus = found
                callbacks.connect?()
            }
        } else {
            if stylus != nil {
                callbacks.disconnect?()
            }
            stylus = nil
        }
    }

    func poll() {
        guard let stylus = stylus else {
            state = MuseStylusState()
            return
        }

        // Read pose if present. GCStylus offers a pose that includes translation and orientation.
        if let pose = stylus.pose {
            let transform = pose.transform
            let translation = transform.columns.3
            state.positionX = translation.x
            state.positionY = translation.y
            state.positionZ = translation.z
            let rotation = simd_quatf(transform)
            state.orientationX = rotation.imag.x
            state.orientationY = rotation.imag.y
            state.orientationZ = rotation.imag.z
            state.orientationW = rotation.real
        }

        // Pressure
        if let pressureInput = stylus.pressure {
            state.pressure = pressureInput.value
        }

        // Buttons
        var buttonMask: UInt32 = 0
        if let primary = stylus.primaryButton, primary.isPressed { buttonMask |= 1 << 0 }
        if let secondary = stylus.secondaryButton, secondary.isPressed { buttonMask |= 1 << 1 }
        if let accessory = stylus.accessoryButton, accessory.isPressed { buttonMask |= 1 << 2 }
        state.buttons = buttonMask

        let buttonMaskInt = Int32(bitPattern: buttonMask)
        if buttonMaskInt != lastButtonMask {
            lastButtonMask = buttonMaskInt
            callbacks.buttons?(buttonMaskInt)
        }

        // Tip contact and hover information.
        var tipContact = false
        var tipHover = false
        if let tip = stylus.tipContact { tipContact = tip.isPressed }
        if let hover = stylus.tipNearSurface { tipHover = hover.isPressed }
        state.isTipContact = tipContact
        state.isTipNearSurface = tipHover

        if tipContact != lastTipContact {
            lastTipContact = tipContact
            callbacks.tipContact?(tipContact)
        }
        if tipHover != lastTipHover {
            lastTipHover = tipHover
            callbacks.tipHover?(tipHover)
        }

        state.isAvailable = true
    }

    func currentState() -> MuseStylusState {
        return state
    }

    func setCallbacks(connect: MuseStylusSimpleEventCallback?,
                      disconnect: MuseStylusSimpleEventCallback?,
                      buttons: MuseStylusButtonEventCallback?,
                      tipContact: MuseStylusBoolEventCallback?,
                      tipHover: MuseStylusBoolEventCallback?) {
        callbacks = (connect, disconnect, buttons, tipContact, tipHover)
    }
}

// MARK: - C API exported to Unity

@_cdecl("MuseStylus_IsAvailable")
public func MuseStylus_IsAvailable() -> Bool {
    return MuseStylusManager.shared.currentState().isAvailable
}

@_cdecl("MuseStylus_GetState")
public func MuseStylus_GetState(_ outState: UnsafeMutablePointer<MuseStylusState>?) {
    guard let pointer = outState else { return }
    MuseStylusManager.shared.poll()
    pointer.pointee = MuseStylusManager.shared.currentState()
}

@_cdecl("MuseStylus_Update")
public func MuseStylus_Update() {
    MuseStylusManager.shared.poll()
}

@_cdecl("MuseStylus_SetEventCallbacks")
public func MuseStylus_SetEventCallbacks(_ onConnect: MuseStylusSimpleEventCallback?,
                                         _ onDisconnect: MuseStylusSimpleEventCallback?,
                                         _ onButtonsChanged: MuseStylusButtonEventCallback?,
                                         _ onTipContactChanged: MuseStylusBoolEventCallback?,
                                         _ onTipHoverChanged: MuseStylusBoolEventCallback?) {
    MuseStylusManager.shared.setCallbacks(connect: onConnect,
                                          disconnect: onDisconnect,
                                          buttons: onButtonsChanged,
                                          tipContact: onTipContactChanged,
                                          tipHover: onTipHoverChanged)
}
