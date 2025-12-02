# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2025-12-02

### Added
- Initial release of Unity VisionOS Logitech Muse GCStylus wrapper
- GCStylus static API for direct access to stylus data
- GCStylusManager MonoBehaviour component for easy integration
- Native Objective-C++ plugin for iOS/visionOS
- Support for:
  - Touch position tracking
  - Azimuth angle (rotation around z-axis)
  - Altitude angle (tilt from surface)
  - Force/pressure sensing
  - In-range detection
  - Touch state tracking
- Event system for touch and range state changes
- Example usage script (GCStylusExample.cs)
- Comprehensive documentation
- Unity Package Manager support
- Assembly definition files for proper code organization
