using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// A UI window that is displayed in front of all other components, for the user to make a decision or accept a prompt.
	/// </summary>
	[PublicAPI]
	public class ConfirmationWindow : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("An optional selectable component to select on the event system when this confirmation window is set initialized.")]
		protected Selectable focusSelectable;
		[SerializeField]
		[Tooltip("Text to automatically set when the confirmation window is initialized.")]
		protected TMP_Text titleTextMesh;
		[SerializeField]
		[Tooltip("Text to automatically set when the confirmation window is initialized.")]
		protected TMP_Text descriptionTextMesh;
		[SerializeField]
		[Tooltip("Text to automatically set when the confirmation window is initialized.")]
		protected TMP_Text submitTextMesh;
		[SerializeField]
		[Tooltip("Text to automatically set when the confirmation window is initialized.")]
		protected TMP_Text cancelTextMesh;

		protected bool hasTakenAction;
		protected bool hasRunClose;
		protected Action onSubmit;
		protected Action onCancel;

		/// <summary>
		/// Invoked when the confirmation window is initialized.
		/// </summary>
		public event Action<Request> OnInitialize = delegate { };

		protected virtual void OnEnable()
		{
			UIFunctions.AddFocus(this);
		}

		protected virtual void OnDisable()
		{
			UIFunctions.RemoveFocus(this);
		}

		/// <summary>
		/// Submit the 'positive' choice for the confirmation window.
		/// </summary>
		public virtual void Submit()
		{
			if (hasTakenAction)
			{
				return;
			}

			hasTakenAction = true;
			if (onSubmit != null)
			{
				onSubmit.Invoke();
				ClearActions();
			}

			Close();
		}

		/// <summary>
		/// Submit the 'negative' choice for the confirmation window.
		/// </summary>
		public virtual void Cancel()
		{
			if (hasTakenAction)
			{
				return;
			}

			hasTakenAction = true;
			if (onCancel != null)
			{
				onCancel.Invoke();
				ClearActions();
			}

			Close();
		}

		public virtual void Close()
		{
			if (hasRunClose)
			{
				return;
			}

			hasRunClose = true;
			UIFunctions.RemoveFocus(this);
			Destroy(gameObject);
		}

		public virtual void Initialize(Request request)
		{
			onSubmit = request.onSubmit;
			onCancel = request.onCancel;
			SelectDefaultSelectable();
			SetTextComponent(titleTextMesh, request.title);
			SetTextComponent(descriptionTextMesh, request.description);
			SetTextComponent(submitTextMesh, request.submitText);
			SetTextComponent(cancelTextMesh, request.cancelText);
			OnInitialize.Invoke(request);
		}

		protected void SelectDefaultSelectable()
		{
			if (focusSelectable != null)
			{
				EventSystem.current.SetSelectedGameObject(focusSelectable.gameObject);
			}
		}

		protected void SetTextComponent(TMP_Text textMesh, string value)
		{
			if (textMesh != null)
			{
				textMesh.text = value;
			}
		}

		protected virtual void ClearActions()
		{
			onSubmit = null;
			onCancel = null;
		}

		/// <summary>
		/// Data structure for making a request to open a confirmation window through <see cref="UIFunctions.CreateConfirmationWindow(Request)"/>.
		/// </summary>
		public class Request
		{
			/// <summary>
			/// The action to invoke when the user submits.
			/// This could be considered the 'positive' response such as accepting a question.
			/// </summary>
			/// <example>
			/// Apply a reset to the user's preferences.
			/// </example>
			public readonly Action onSubmit;
			/// <summary>
			/// The action to invoke when the user cancels.
			/// This could be considered the 'negative' response such as backing out of the question or rejecting it.
			/// </summary>
			/// <example>
			/// Back out of making a reset to the user's preferences.
			/// </example>
			public readonly Action onCancel;

			/// <summary>
			/// The title of the request.
			/// </summary>
			/// <example>
			/// "Reset Preferences?"
			/// </example>
			public readonly string title;
			/// <summary>
			/// The description of the request.
			/// </summary>
			/// <example>
			/// "Are you sure you would like to reset your preferences? This action can not be reversed."
			/// </example>
			public readonly string description;
			/// <summary>
			/// The description of the <see cref="onCancel"/> action.
			/// </summary>
			/// <example>
			/// "Submit"
			/// </example>
			public readonly string submitText;
			/// <summary>
			/// The description of the <see cref="onCancel"/> action.
			/// </summary>
			/// <example>
			/// "Cancel"
			/// </example>
			public readonly string cancelText;

			/// <summary>
			/// Create a new request format to send to the <see cref="UIFunctions.CreateConfirmationWindow(Request)"/>.
			/// </summary>
			public Request(Action onSubmit, Action onCancel, string title = "Request", string description = "Are you sure?", string submitText = "Submit",
				string cancelText = "Cancel")
			{
				this.onSubmit = onSubmit;
				this.onCancel = onCancel;
				this.title = title;
				this.description = description;
				this.submitText = submitText;
				this.cancelText = cancelText;
			}
		}
	}
}