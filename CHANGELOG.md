# Changelog

All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [v2.1.0] - UNRELEASED

### Added

- Added support for creating `OverlayManager` overlays on transforms other than the manager's transform

## [v2.0.1] - 2023-11-18

### Fixed

- Fixed unintentional exception being thrown in Poolable `OnDestroy` when a poolable object is destroyed from `Object.Instantiate`

## [v2.0.0] - 2023-11-18

This major release has been created due to several breaking changes to the `ObjectPool` system.
Upgrading to this release will require updating syntax of scripts that utilize poolables.

### Added

- Added `Poolable.Instance` class, enabling usage of poolables that expire when the object is returned.
- Added `Poolable.Identifier` property: a unique id that is assigned to a poolable when retrieved and revoked when returned.

### Changed

- Poolable objects that are incorrectly created using `Object.Instantiate` are now immediately destroyed and log an error.
- ⚠️Breaking: The `ObjectPool.Instantiate` methods now return a `Poolable.Instance` instead of the `Poolable` itself.
	- To update: replace variable type with Poolable.Instance.
	- Use `Poolable.Instance.IsActive` to check if the instance is not null
		- Instances can be implicitly cast to bool for this check as well.
	- Use `Poolable.Instance.Poolable` to access the Poolable behavior as before
		- Will return null when the Poolable has been returned or disposed.

### Removed

- ⚠️Breaking: Removed public access to `Poolable.poolSettings`
	- The pool settings defined on the Poolable component should not be modified at runtime.
- ⚠️Breaking: Removed set access for values of `PoolSettings`
	- Modifying these values directly had very erratic behavior.

## [v1.10.0] - 2023-11-15

### Added

- Added public read-only property `Poolable` to `PoolableBehaviour`.

## [v1.9.0] - 2023-11-08

### Added

- Added JsonConvert error handling support to `GameFile`
	- Can be overriden by setting `GameFile.jsonSerializeSettings` and `GameFile.jsonDeserializeSettings`

## [v1.8.0] - 2023-10-10

### Added

- Added `int.LayerMaskContains` extension method
- Added `Vector2.Rotate` extension method
- Added `[PublicAPI]` attribute to most classes
- Added methods to `UIGroup<T>` to mutate list of items
- Added `ItemIndexChanged` event to `UIGroup<T>`
- Added public get-only properties to `UIGroup<T>` for inspecting current values

### Changed

- UIGroup now sets current item to `null` and current index to `-1` when disabling all items. Previously kept old value.

## [v1.7.1] - 2023-06-12

### Fixed

- Fixed logical error in `GameFile` get byte[] functions

## [v1.7.0] - 2023-06-12

### Added

- Added methods to `GameFile` which returns byte[] for the current file data.
- Added methods to `GameFile` which loads a given byte[] directly into the file.
- Added official support for subdirectories in `GameFile`

## [v1.6.0] - 2023-06-12

### Added

- Added `.editorconfig` to project to enforce code styling rules internally.

### Changed

- Several readonly fields on `GameFile` are now publicly accessible.

## [v1.5.0] - 2022-11-21

### Added

- `GameFile` now records the engine and system time when it is load/written.

## [v1.4.0] - 2022-11-18

### Added

- Added an optional `DefaultValue` property to the LookupTable system.
- Added ability to control the `Space` in which an ObjectPool instantiates
- Added support for the `IConvertible` type in the Generic Data Bank

## [v1.3.0] - 2022-10-26

### Added

- Added `ClampedValue<T>` and `ClampedField<T>` classes which ensure a value is within boundaries which are set at the objects contruction.

## [v1.2.0] - 2022-10-12

### Added

- Added `Generic Data Bank` - a way to store generic objects/variables into a string based dictionary.

## [v1.1.2] - 2022-10-09

### Added

- Added support for horizontal scrolling on the `Autoscroll` component.
- Added fields to separately enable/disable Horizontal and Vertical management on the `Autoscroll` component. Both default to enabled.
- Added `PoolableBehaviour` class: A template class that automatically subscribes to `Poolable`'s C# events and invokes virtual methods that your own inheriting classes can override.
	- This can be used to make developing pooled objects slightly more convenient but is not required.

### Changed

- The `DisableAllItems()` function on UIGroup is now `public`. Was previously `protected`.

## [v1.1.1] - 2022-09-11

### Added

- Added `PoolableReturnAfterDelay`, a component which will automatically return a poolable object after some time.
- Added `PoolableUnityEventEmitter`, a component which will fire UnityEvents for the matching poolable C# events.

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