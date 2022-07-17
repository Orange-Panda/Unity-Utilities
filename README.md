# Unity Utilities
An all in one package that handles many common unity development needs.

## Download and Installation
Options for installation:
- Option A: Package Manager with Git (Recommended)
	1. Ensure you have Git installed and your Unity Version supports Git package manager imports (2019+)
	2. In Unity go to Window -> Package Manager
	3. Press the + icon in the top left
	4. Click "Add package from Git URL"
	5. Enter the following into the field and press enter: 
```
https://github.com/Orange-Panda/Unity-Utilities.git
```
- Option B: Package Manager from Disk
	1. Download the repository and note down where the repository is saved.
	2. In Unity go to Window -> Package Manager
	3. Press the + icon in the top left
	4. Click "Add package from disk"
	5. Select the file from Step 1
- Option C: Import Package Manually
	1. Download the repository
	2. In Unity's project window drag the folder into the "Packages" folder on the left hand side beside the "Assets" folder


## What's Included?
- **Action Emitter**: For managing dynamic events, particularly in ui.
- **Aspect Ratio Calculator**: Assistant to the `AspectRatioFitter` in the canvas ui system.
- **Auto Scroll**: Component for automatically updating a scroll rect when a new object is selected.
- **Confirmation Windows**: Infrastructure for prompts that the user can accept/decline
- **Encryption**: Built in AES encryptor for `Game Files`
- **Extensions**: Convenient extension methods for general situations
- **Game File**: Infrastructure for saving serializiable data to the persistent data path
- **Lookup Table**: Infrastructure for finding assets/objects through a string key lookup
- **Overlays**: Handling opening/closing full screen ui elements that the user is required to input from
- **Time Manager**: Pause/Unpause functionality that maintains a non one time scale override.
- **UI Groups**: Functionality for showing and hiding ui components anywhere
	- **Tab UI Groups**: Ordered list of ui elements to cycle through
	- **Panel UI Groups**: Hierarchy list of ui elements that can be traversed upwards