using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// Abstract base class for an overlay within the <see cref="OverlayManager"/> system.
	/// Inherit from this to create custom behavior for the overlays your application uses.
	/// </summary>
	[RequireComponent(typeof(GraphicRaycaster), typeof(Canvas)), PublicAPI]
	public abstract class OverlayInterface : MonoBehaviour
	{
		[Header("Overlay Interface")]
		[Tooltip("Default selectable to highlight on Open")]
		protected Selectable selectable;
		[Tooltip("The item that is always enabled and disabled when this overlay is active.")]
		protected GameObject container;

		protected OverlayManager overlayManager;
		protected GraphicRaycaster raycaster;
		protected Canvas canvas;

		/// <summary>
		/// Is this overlay interface currently active and in focus?
		/// </summary>
		public bool IsOpen { get; private set; }

		/// <summary>
		/// Should the cursor or other input features be revealed that are normally hidden during game while this overlay is active?
		/// </summary>
		/// <remarks>
		/// This doesn't implicitly do anything but can be overriden and tested against to reveal the cursor during overlays or whatever your application needs.
		/// </remarks>
		public virtual bool RevealInput => true;

		/// <summary>
		/// When this overlay is active should input elsewhere be allowed?
		/// </summary>
		/// <remarks>
		/// This doesn't implicitly do anything but can be overriden and tested against to do things such as prevent players from controlling the game world while certain overlays are active.
		/// </remarks>
		public virtual bool LockInput => true;

		protected virtual void Awake()
		{
			overlayManager = GetComponentInParent<OverlayManager>();
			canvas = GetComponent<Canvas>();
			raycaster = GetComponent<GraphicRaycaster>();
		}

		/// <summary>
		/// Set this interface to be active.
		/// </summary>
		/// <remarks>
		/// Usually you should use <see cref="OverlayManager.SetActiveInterface(string)"/> instead.
		/// Can be overriden to have special behavior when opened, but make sure to call base.Open();
		/// </remarks>
		public virtual void Open()
		{
			IsOpen = true;
			SetVisualActive(true);

			if (selectable)
			{
				selectable.Select();
			}
		}

		/// <summary>
		/// Close this overlay interface and deselect any selectable that is currently selected.
		/// </summary>
		public virtual void Close()
		{
			IsOpen = false;
			SetVisualActive(false);

			if (overlayManager.ActiveInterface == this)
			{
				overlayManager.ActiveInterface = null;
			}

			EventSystem.current.SetSelectedGameObject(null);
		}

		/// <summary>
		/// Invoked by the overlay system to set the visual state of the overlay.
		/// </summary>
		/// <param name="value">If the overlay should be visible or not.</param>
		public virtual void SetVisualActive(bool value)
		{
			canvas.enabled = value;
			raycaster.enabled = value;
			if (container)
			{
				container.SetActive(value);
			}
		}

		/// <summary>
		/// Called by the <see cref="OverlayManager"/> to try to close the overlay instance because the user wants to close this overlay or open another one.
		/// Your interface can accept or reject the request depending on the rules of your interface's design.
		/// </summary>
		/// <remarks>
		/// Even if the method returns true you must still explicitly invoke <see cref="Close"/> in your override.
		/// The most direct handling of this method is
		/// <code>
		/// Close();
		/// return true;
		/// </code>
		/// </remarks>
		/// <returns>The result of the close request. True if the <see cref="Close"/> method was invoked, false if it was not - likely due to a required field.</returns>
		public abstract bool RequestClose();
	}
}