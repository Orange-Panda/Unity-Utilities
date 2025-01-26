using JetBrains.Annotations;
using UnityEngine;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// Handle functionality to assist other ui elements in synchronizing their behavior.
	/// </summary>
	[PublicAPI]
	public static class UIFunctions
	{
		/// <summary>
		/// Is there currently a UI element in focus?
		/// </summary>
		/// <remarks>
		/// When a UI element is in focus that means other ui elements should prevent certain inputs.
		/// </remarks>
		public static bool IsFocused => focusRunner != null && focusRunner.IsFocused;

		private static UIFocusRunner focusRunner;

		/// <summary>
		/// The most recently create confirmation window using <see cref="CreateConfirmationWindow"/>
		/// </summary>
		[CanBeNull]
		public static ConfirmationWindow MostRecentConfirmation { get; private set; }

		private static void VerifyFocusInstance()
		{
			if (focusRunner != null)
			{
				return;
			}

			GameObject newFocus = new GameObject("UI Focus Runner");
			focusRunner = newFocus.AddComponent<UIFocusRunner>();
		}

		/// <summary>
		/// Add an object to the <see cref="UIFocusRunner"/> to let other ui elements know that input should be ignored.
		/// </summary>
		/// <remarks>
		/// Make sure to remove focus later, when you are done, with <see cref="RemoveFocus"/>.
		/// </remarks>
		/// <param name="blocker">The identifier for the object that is in focus.</param>
		public static void AddFocus(object blocker)
		{
			VerifyFocusInstance();
			focusRunner.Add(blocker);
		}

		/// <summary>
		/// Remove an object to the <see cref="UIFocusRunner"/> to let other ui elements know that it is done.
		/// </summary>
		/// <param name="blocker">The identifier for the object that is in focus. Ensure this is identical to the one used in <see cref="AddFocus"/></param>
		public static void RemoveFocus(object blocker)
		{
			if (focusRunner)
			{
				focusRunner.Remove(blocker);
			}
		}

		/// <summary>
		/// Create a <see cref="ConfirmationWindow"/> to fulfill a particular prompt for the user to submit/decline.
		/// </summary>
		/// <remarks>
		/// If there is already a pending confirmation window will run `Close()` on it to dismiss it and create a new confirmation window.
		/// </remarks>
		public static void CreateConfirmationWindow(ConfirmationWindow.Request request, GameObject windowPrefab, Canvas canvas)
		{
			if (MostRecentConfirmation != null)
			{
				MostRecentConfirmation.Close();
				MostRecentConfirmation = null;
			}

			GameObject windowObject = Object.Instantiate(windowPrefab, canvas != null ? canvas.transform : Object.FindObjectOfType<Canvas>().transform);
			MostRecentConfirmation = windowObject.GetComponent<ConfirmationWindow>();
			if (MostRecentConfirmation == null)
			{
				Debug.LogError($"The window prefab provided of name \"{windowObject.name}\" does not have a ConfirmationWindow component! " +
				               "The component may not be on a child object", windowObject);
				return;
			}

			MostRecentConfirmation.Initialize(request);
		}
	}
}