# Unity Utilities
An all in one package that handles common unity development needs such as game files, object pools, UGUI features, and various QOL functions.

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

## Features
- **Game File**: Infrastructure for saving serializiable data in a json format to the persistent data path
- **Encryption**: Encryptor for `Game Files` including a built in AES encryptor
- **Lookup Table**: Infrastructure for finding assets/objects through a string key lookup
- **Time Manager**: Pause/Unpause functionality that maintains a non one time scale override.
- **Clamped Data Types**: Strictly maintain a numeric value between a min and max value.

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