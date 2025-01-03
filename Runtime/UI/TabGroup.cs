using JetBrains.Annotations;
using UnityEngine;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// A tab group is a sequence of ui elements that follow an linked order.<br/>
	/// Tabs can be opened either directly by their index or can be moved between the previous or following tab in the linked order.
	/// </summary>
	/// <remarks>
	/// Tab groups are useful in cases such as categories under a single ui like an options menu or an inventory with various unique categories.
	/// </remarks>
	[PublicAPI]
	public class TabGroup : UIGroup<Tab>
	{
		[SerializeField]
		[Tooltip("When true, previous tab commands will loop back from the first index to the last. " +
		         "When false, reaching the first index and going to previous tab will stay at current index. " +
		         "This behavior is also done with next tab at the last index")]
		protected bool overflowScrolling;

		public bool OverflowScrolling => overflowScrolling;

		/// <summary>
		/// Check if this tab group can move to the next tab in the sequence.
		/// </summary>
		/// <param name="tab">The next tab in the sequence. May be null if there is no next tab or the actual item is null.</param>
		/// <returns>True if there is a next tab index in the sequence. False if this is the last tab and <see cref="overflowScrolling"/> is false.</returns>
		public bool TryGetNextTab(out Tab tab)
		{
			if (overflowScrolling || currentIndex != items.Count - 1)
			{
				int index = (currentIndex + 1) % items.Count;
				tab = items[index];
				return true;
			}

			tab = null;
			return false;
		}

		/// <summary>
		/// Check if this tab group can move to the previous tab in the sequence.
		/// </summary>
		/// <param name="tab">The previous tab in the sequence. May be null if there is no previous tab or the actual item is null.</param>
		/// <returns>True if there is a previous tab index in the sequence. False if this is the first tab and <see cref="overflowScrolling"/> is false.</returns>
		public bool TryGetPreviousTab(out Tab tab)
		{
			if (overflowScrolling || currentIndex != 0)
			{
				int index = (currentIndex + items.Count - 1) % items.Count;
				tab = items[index];
				return true;
			}

			tab = null;
			return false;
		}

		/// <summary>
		/// Move to the next tab in the tab list order.
		/// </summary>
		public void SetNextTab()
		{
			if (TryGetNextTab(out Tab tab))
			{
				SetActiveItem(tab.Key);
			}
		}

		/// <summary>
		/// Move to the previous tab in the tab list order.
		/// </summary>
		public void SetPreviousTab()
		{
			if (TryGetPreviousTab(out Tab tab))
			{
				SetActiveItem(tab.Key);
			}
		}
	}
}