using UnityEngine;
using UnityEngine.UI;

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

	public void RecalculateAspect()
	{
#if UNITY_EDITOR
		GetComponents();
#endif
		if (layout != null)
		{
			float height = layout.preferredHeight;
			float width = layout.preferredWidth;
			aspectRatioFitter.aspectRatio = Mathf.Clamp(width / height, 0.001f, 1000f);
		}
	}
}
