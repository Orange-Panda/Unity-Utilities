using UnityEditor;
using UnityEngine;

namespace LMirman.Utilities
{
	[CustomEditor(typeof(AspectRatioCalculator))]
	public class AspectRatioCalculatorEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Recalculate Aspect"))
			{
				foreach (GameObject selectedObject in Selection.gameObjects)
				{
					if (selectedObject.TryGetComponent(out AspectRatioCalculator calculator))
					{
						calculator.RecalculateAspect();
					}
				}
			}
		}
	}
}