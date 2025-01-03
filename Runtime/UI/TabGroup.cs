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
		[Tooltip("When true, previous tab commands will loop back from the first index to the last. When false, reaching the first index and going to previous tab will stay at current index. This behavior is also done with next tab at the last index")]
		private bool overflowScrolling;

		/// <summary>
		/// Move to the next tab in the tab list order.
		/// </summary>
		public void SetNextTab()
		{
			if (overflowScrolling || currentIndex != items.Count - 1)
			{
				int index = (currentIndex + 1) % items.Count;
				SetActiveItem(index);
			}
		}

		/// <summary>
		/// Move to the previous tab in the tab list order.
		/// </summary>
		public void SetPreviousTab()
		{
			if (overflowScrolling || currentIndex != 0)
			{
				int index = (currentIndex + items.Count - 1) % items.Count;
				SetActiveItem(index);
			}
		}
	}
}