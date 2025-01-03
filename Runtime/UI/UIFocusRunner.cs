using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// In scene runner that will prevent certain UI functionality when focus is intended to be on something else.
	/// </summary>
	/// <remarks>
	/// For example the <see cref="ConfirmationWindow"/>, while active, considers itself in focus while active.
	/// This allows other ui elements that react to certain inputs regardless of what is selected to block the input if something <see cref="UIFunctions.IsFocused"/>.
	/// </remarks>
	public class UIFocusRunner : MonoBehaviour
	{
		private readonly List<object> focusObjects = new List<object>();
		private int blockFrames = BlockCount;
		private const int BlockCount = 3;

		internal bool IsFocused => blockFrames > 0;
	
		private void Update()
		{
			if (focusObjects.Count > 0)
			{
				blockFrames = BlockCount;
			}
			else if (blockFrames > 0)
			{
				blockFrames--;
			}
		}

		internal void Add(object blocker)
		{
			focusObjects.Add(blocker);
			blockFrames = BlockCount;
		}

		internal void Remove(object blocker)
		{
			for (int i = focusObjects.Count - 1; i >= 0; i--)
			{
				if (focusObjects[i] == blocker || focusObjects[i] == null)
				{
					focusObjects.RemoveAt(i);
				}
			}
		}
	}
}