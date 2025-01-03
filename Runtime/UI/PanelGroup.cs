using JetBrains.Annotations;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// A panel group is a hierarchy of ui elements where each panel refers to a parent element that can be returned to.<br/>
	/// Panels can be opened either directly by their key or navigated upwards through the hierarchy by returning.
	/// </summary>
	/// <remarks>
	/// Panel groups are useful in cases such as navigating main menu screens.<br/>
	/// <br/>
	/// A limitation of the panel system is the hierarchy is naive to the history of panel navigation.<br/>
	/// This is mainly problematic in cases where a panel can be opened from multiple sources, essentially having multiple parent panels.
	/// For example: If a 'Panel C' can be opened via 'Panel A' or 'Panel B' only one of these panels can be marked as the <see cref="Panel.parentPanel"/>.
	/// The <see cref="Panel.parentOverride"/> is a band-aid to this situation: which allows you to temporarily override the parent it returns to.
	/// However, this situation should ideally be avoided by using different panels for each parent node (A and B having their own separate Panel C).
	/// If that is not sufficient you may want to consider overriding <see cref="UIGroup{T}"/> and <see cref="UIGroupItem"/> and engineering a more advanced navigation system.
	/// </remarks>
	[PublicAPI]
	public class PanelGroup : UIGroup<Panel>
	{
		/// <summary>
		/// True when there is a panel active and it has a parent node to return to.
		/// </summary>
		public bool HasParentPanel => currentItem != null && currentItem.ParentPanel != null;
		/// <summary>
		/// True when there is a panel active and it does <b>not</b> have a parent node to return to.
		/// </summary>
		public bool HasOrphanPanel => currentItem != null && currentItem.ParentPanel == null;

		/// <summary>
		/// Tells the currently active panel to return to the parent node.
		/// </summary>
		public void Return()
		{
			if (currentItem)
			{
				currentItem.OnReturn(this);
			}
		}
	}
}