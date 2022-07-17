using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Handle functionality to assist other ui elements in synchronizing their behavior.
	/// </summary>
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
		private static ConfirmationWindow confirmationWindow;
	
		private static void VerifyFocusInstance()
		{
			if (focusRunner == null)
			{
				GameObject newFocus = new GameObject("UI Focus Runner");
				focusRunner = newFocus.AddComponent<UIFocusRunner>();
			}
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
		public static void CreateConfirmationWindow(ConfirmationWindow.Request request)
		{
			if (confirmationWindow == null)
			{
				GameObject windowObject = Object.Instantiate(Resources.Load<GameObject>("UI/Confirmation Window"), request.canvas != null ? request.canvas.transform : Object.FindObjectOfType<Canvas>().transform);
				confirmationWindow = windowObject.GetComponent<ConfirmationWindow>();
			}
			else if (!confirmationWindow.isActiveAndEnabled)
			{
				confirmationWindow.gameObject.SetActive(true);
				confirmationWindow.enabled = true;
			}
			confirmationWindow.Initialize(request);
		}
	}
}