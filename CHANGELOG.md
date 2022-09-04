# Changelog
All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [v1.1.0] - 2022-09-03

### Added
- Added the `Yielders` class for caching frequently used coroutine yield instructions
	- Has support for `WaitForFixedUpdate`, `WaitForEndOfFrame`, `WaitForSeconds`, and `WaitForSecondsRealtime`
- Added the `ObjectPool` system, a static class for caching frequently instantiated/destroyed prefabs

## [1.0.1] - 2022-08-03

### Added
- Added `SetVisualActive(bool)` method to OverlayInterface which is called during open, close, and load.

### Changed
- `Close()` on OverlayInterface is no longer called by the OverlayManager when a panel is loaded. Thus, `Open()` is now guaranteed to be invoked before `Close()`.

### Fixed
- Fixed a typo in PanelGroup

## [1.0.0] - 2022-07-16

### Added
- Package created.
- Includes:
	- Action Emitter: For managing dynamic events, particularly in ui.
	- Aspect Ratio Calculator: Assistant to the `AspectRatioFitter` in the canvas ui system.
	- Auto Scroll: Component for automatically updating a scroll rect when a new object is selected.
	- Confirmation Windows: Infrastructure for prompts that the user can accept/decline
	- Encryption: Built in AES encryptor for `Game Files`
	- Extensions: Convenient extension methods for general situations
	- Game File: Infrastructure for saving serializiable data to the persistent data path
	- Lookup Table: Infrastructure for finding assets/objects through a string key lookup
	- Overlays: Handling opening/closing full screen ui elements that the user is required to input from
	- Time Manager: Pause/Unpause functionality that maintains a non one time scale override.
	- UI Groups: Functionality for showing and hiding ui components anywhere
		- Tab UI Groups: Ordered list of ui elements to cycle through
		- Panel UI Groups: Hierarchy list of ui elements that can be traversed upwards