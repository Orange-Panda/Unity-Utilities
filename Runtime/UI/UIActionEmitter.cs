using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIActionEmitter : MonoBehaviour
{
	public event Action ActionsUpdated = delegate { };

	public List<ActionEvent> Actions { get; } = new List<ActionEvent>();

	public void InvokeActionsUpdated()
	{
		ActionsUpdated.Invoke();
	}

	public void InvokeAction(int index)
	{
		if (index.InBounds(Actions) && Actions[index] != null)
		{
			Actions[index].onTrigger.Invoke();
		}
		InvokeActionsUpdated();
	}
	
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

	public void ClearActions()
	{
		for (int i = 0; i < Actions.Count; i++)
		{
			Actions[i] = null;
		}
		InvokeActionsUpdated();
	}

	public ActionEvent GetAction(int index)
	{
		return index.InBounds(Actions) ? Actions[index] : null;
	}
	
	[Serializable]
	public class ActionEvent
	{
		public string name;
		public UnityEvent onTrigger;
	}
}
