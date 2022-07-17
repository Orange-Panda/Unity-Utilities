# Changelog
All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/) and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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