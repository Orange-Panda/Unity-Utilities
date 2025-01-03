# Unity Utilities

A Unity package that implements various common game development functionality such as:

- **Game Files**: for reading and writing persistent data to the disk
- **Object Pools**: for reusing frequently created/destroyed objects such as projectiles
- **UGUI Components**: for creating tab groups, panel groups, and confirmation winds
- **Misc. Extension Methods**: for implementing shorthand functions for common behavior
- **Lookup Table**: for finding assets/objects through a string key lookup
- **Time Manager**: for pausing functionality that maintains a non-one timescale override
- **Clamped Data Types**: for defining a numeric value that is always constrained between a min and max value

## Requirements

- A Unity project of 2021.2 or later
	- Earlier unity releases are not officially supported by this package

## Installation

1. Install the package via Git in the Package Manager
	1. Ensure you have Git installed and your Unity Version supports Git package manager imports (2019+)
	2. In Unity go to `Window -> Package Manager`
	3. Press the + icon at the top left of the Package Manager window
	4. Choose "Add package from Git URL"
	5. Enter the following into the field and press enter:
		1. Tip: You can append a version to the end of the Git URL to lock it to a specific version such as `https://github.com/Orange-Panda/Unity-Utilities.git#v1.8.0`
   ```
   https://github.com/Orange-Panda/Unity-Utilities.git
   ```

### Extensions

- `List.InBounds` and `Array.InBounds` to check for valid indecies.
- Color: `MoveTowards` and `MoveTowardsAlpha` for moving values of a color by a distance delta
- Color: `SetRGB`, `SetA` for copying specific values from one color to another
- `int.LayerMaskContains` to check if a mask contains a particular layer
- `Vector2.Rotate` for rotating a vector about the Z axis

### Object Pool

Reuse frequently instantiated and destroyed objects

#### Object Pool Quick Start

1. Attach a `Poolable` component to a prefab
2. Use `ObjectPool.Instantiate` to instantiate a pooled object
3. Use `Poolable.Return` in place of `Destroy` to disable a object for recycling in a future instantiation
4. Implement `Poolable.OnReturn` or `Poolable.OnRetrieve` to reset variables when recycling
   1. Optional: Add `PoolableReturnAfterDelay` to automatically return objects after some amount of time

### UGUI Components

- **Action Emitter**: Handle functional UI actions such as `Press Y to reset settings` or `Press X to inspect item` 
- **Aspect Ratio Calculator**: Automatically update `AspectRatioFitter` values.
- **Auto Scroll**: Automatically update a scroll rect when a new object is selected.
- **Confirmation Windows**: Prompt user to accept/decline an action
- **Overlays**: Handle full screen ui elements that the user is expected to input for
- **UI Groups**: Control a mutually exclusive list of ui elements such as:
	- **Tab UI Groups**: Ordered list of ui elements to cycle through via previous and next inputs
	- **Panel UI Groups**: Hierarchy list of ui elements that can be traversed upwards to parent panels

## Getting Help

- Use the [Issues](https://github.com/Orange-Panda/Unity-Utilities/issues) or [Discussions](https://github.com/Orange-Panda/Unity-Utilities/discussions) of this GitHub repository for support.

## Credits

This package is developed by [Luke Mirman](https://lukemirman.com/).