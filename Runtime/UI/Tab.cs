using UnityEngine;
using UnityEngine.EventSystems;

namespace LMirman.Utilities
{
	/// <summary>
	/// A ui element within a <see cref="TabGroup"/>
	/// </summary>
	public class Tab : UIGroupItem
	{
		[Header("Tab Options")]
		[SerializeField]
		[Tooltip("An optional selectable component to select on the event system when this tab is set active.")]
		private GameObject initialSelectable;

		public override void SetActive(bool value)
		{
			gameObject.SetActive(value);
			if (initialSelectable != null && initialSelectable.activeInHierarchy)
			{
				EventSystem.current.SetSelectedGameObject(initialSelectable);
			}
		}
	}
}