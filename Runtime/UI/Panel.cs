using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LMirman.Utilities
{
	/// <summary>
	/// A ui element within a <see cref="PanelGroup"/>.
	/// </summary>
	[PublicAPI]
	public class Panel : UIGroupItem
	{
		[Header("Panel Options")]
		[SerializeField]
		[Tooltip("An optional selectable component to select on the event system when this panel is set active.")]
		private GameObject initialSelectable;
		[SerializeField] 
		[Tooltip("The panel to set active when this panel is cancelled. If no parent is defined, the ability to return out of this panel is disabled.")]
		private Panel parentPanel;
	
		[Header("Parent Override Rules")]
		[SerializeField]
		[Tooltip("When true, clear a parent panel override when this panel is disabled.")]
		private bool clearParentOverrideOnDisable;
		[SerializeField]
		[Tooltip("When true, clear a parent panel override when this panel is returned from.")]
		private bool clearParentOverrideOnReturn = true;

		private Panel parentOverride;

		public Panel ParentPanel => parentOverride != null ? parentOverride : parentPanel;

		public override void SetActive(bool value)
		{
			gameObject.SetActive(value);
			if (value && initialSelectable != null)
			{
				EventSystem.current.SetSelectedGameObject(initialSelectable);
			}

			if (!value && clearParentOverrideOnDisable)
			{
				ClearParentOverride();
			}
		}

		public virtual void OnReturn(PanelGroup group)
		{
			if (ParentPanel)
			{
				group.SetActiveItem(ParentPanel.Key);

				if (clearParentOverrideOnReturn)
				{
					ClearParentOverride();
				}
			}
		}

		public void SetParentOverride(Panel panel)
		{
			parentOverride = panel;
		}
	
		public void ClearParentOverride()
		{
			parentOverride = null;
		}
	}
}