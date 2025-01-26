using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// Component which can have generic <see cref="ActionEvent"/> assigned to list indices via <see cref="SetAction"/>.
	/// These actions can then be displayed on UI elements with <see cref="GetAction"/> and invoked by other components through <see cref="InvokeAction"/>.
	/// </summary>
	/// <remarks>
	/// There is an arbitrary limit of 32 actions that can be allocated per <see cref="ActionEmitter"/> instance.
	/// </remarks>
	/// <example>
	/// An options menu can create UI actions such as a 'Save Settings' and 'Revert Settings' action.
	/// These can then be invoked by a UI button or input key.
	/// </example>
	[PublicAPI]
	public class ActionEmitter : MonoBehaviour
	{
		/// <summary>
		/// Invoked when the actions list has been changed.
		/// </summary>
		/// <example>
		/// This event is useful to update your UI components to display the <see cref="Actions"/>
		/// </example>
		public event Action ActionsUpdated = delegate { };

		/// <summary>
		/// Invoked when am <see cref="ActionEvent"/> has been invoked.
		/// </summary>
		public event Action<ActionEvent> ActionInvoked = delegate { };

		/// <summary>
		/// The current actions that can be invoked.
		/// </summary>
		/// <remarks>
		/// You should usually use <see cref="GetAction"/> to check this list.
		/// </remarks>
		public List<ActionEvent> Actions { get; } = new List<ActionEvent>();

		/// <summary>
		/// Manually fire the <see cref="ActionsUpdated"/> event.
		/// </summary>
		public void InvokeActionsUpdated()
		{
			ActionsUpdated.Invoke();
		}

		/// <summary>
		/// Invoke a particular action by its index in the <see cref="Actions"/> list.
		/// </summary>
		/// <param name="index">The index of the action to invoke.</param>
		public void InvokeAction(int index)
		{
			if (!index.InBounds(Actions) || Actions[index] == null)
			{
				return;
			}

			ActionEvent actionEvent = Actions[index];
			actionEvent.onTrigger.Invoke();
			ActionInvoked.Invoke(actionEvent);
		}

		/// <summary>
		/// Invoke a particular action by its index in the <see cref="Actions"/> list.
		/// </summary>
		/// <param name="index">The index to set the action into.</param>
		/// <param name="function">The event to trigger when the action is invoked.</param>
		public void SetAction(int index, ActionEvent function)
		{
			index = Mathf.Clamp(index, 0, 31);
			while (!index.InBounds(Actions))
			{
				Actions.Add(null);
			}

			Actions[index] = function;
			InvokeActionsUpdated();
		}

		/// <summary>
		/// Remove all currently set actions.
		/// </summary>
		public void ClearActions()
		{
			for (int i = 0; i < Actions.Count; i++)
			{
				Actions[i] = null;
			}

			InvokeActionsUpdated();
		}

		/// <summary>
		/// Get an action by its index.
		/// </summary>
		/// <param name="index">The index of the action to get.</param>
		/// <returns>The <see cref="ActionEvent"/> that was found at that index, or null if no entry was present at that index.</returns>
		[CanBeNull]
		public ActionEvent GetAction(int index)
		{
			return index.InBounds(Actions) ? Actions[index] : null;
		}

		/// <summary>
		/// An action that can be invoked by an <see cref="ActionEmitter"/>
		/// </summary>
		[Serializable, PublicAPI]
		public class ActionEvent
		{
			public string name;
			public UnityEvent onTrigger;
		}
	}
}