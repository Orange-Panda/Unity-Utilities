using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic way to create dynamic/contextual actions.
/// </summary>
/// <remarks>
/// There is an arbitrary limit of 32 actions that can be allocated per <see cref="ActionEmitter"/> instance.
/// </remarks>
/// <example>
/// An options menu can create UI actions such as a 'Save Settings' and 'Revert Settings' action.
/// These can then be invoked by a UI button or input key.
/// </example>
public class ActionEmitter : MonoBehaviour
{
	/// <summary>
	/// Invoked when the actions list has been changed or after an action has been invoked.
	/// </summary>
	public event Action ActionsUpdated = delegate { };

	/// <summary>
	/// The current actions that can be invoked.
	/// </summary>
	/// <remarks>
	/// You should usually use <see cref="GetAction"/> to check this list.
	/// </remarks>
	public List<ActionEvent> Actions { get; } = new List<ActionEvent>();

	/// <summary>
	/// Fire the <see cref="ActionsUpdated"/> event.
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
		if (index.InBounds(Actions) && Actions[index] != null)
		{
			Actions[index].onTrigger.Invoke();
		}
		InvokeActionsUpdated();
	}

	/// <summary>
	/// Invoke a particular action by its index in the <see cref="Actions"/> list.
	/// </summary>
	/// <param name="index">The index to set the action action into.</param>
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
	public ActionEvent GetAction(int index)
	{
		return index.InBounds(Actions) ? Actions[index] : null;
	}
	
	/// <summary>
	/// An action that can be invoked by an <see cref="ActionEmitter"/>
	/// </summary>
	[Serializable]
	public class ActionEvent
	{
		public string name;
		public UnityEvent onTrigger;
	}
}
