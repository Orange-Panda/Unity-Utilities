using UnityEngine;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// A ui element to fill in a <see cref="UIGroup{T}"/> with.
	/// </summary>
	public abstract class UIGroupItem : MonoBehaviour
	{
		[SerializeField]
		protected string key;

		public string Key => key;

		public abstract void SetActive(bool value);
	}
}