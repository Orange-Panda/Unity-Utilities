using UnityEngine;
using UnityEngine.UI;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// Component that will automatically calculate the correct aspect ratio for an attached <see cref="AspectRatioFitter"/> based on an attached component that inherits <see cref="ILayoutElement"/>.
	/// </summary>
	[RequireComponent(typeof(AspectRatioFitter))]
	public class AspectRatioCalculator : MonoBehaviour
	{
		private bool initialized;
		private AspectRatioFitter aspectRatioFitter;
		private ILayoutElement layout;

		protected void Start()
		{
			GetComponents();
		}

		private void GetComponents()
		{
			if (initialized)
			{
				return;
			}

			aspectRatioFitter = GetComponent<AspectRatioFitter>();
			layout = GetComponent<ILayoutElement>();
			initialized = true;
		}

		/// <summary>
		/// Recalculate the desired aspect ratio for the attached <see cref="AspectRatioFitter"/> based on an attached component that inherits <see cref="ILayoutElement"/>.
		/// </summary>
		public void RecalculateAspect()
		{
			GetComponents();
			if (layout == null)
			{
				return;
			}

			float height = layout.preferredHeight;
			float width = layout.preferredWidth;
			aspectRatioFitter.aspectRatio = Mathf.Clamp(width / height, 0.001f, 1000f);
		}
	}
}