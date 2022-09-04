using UnityEditor;
using UnityEngine;

namespace LMirman.Utilities
{
	[CustomEditor(typeof(Poolable))]
	public class PoolableEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			Poolable poolable = serializedObject.targetObject as Poolable;
			if (poolable != null)
			{
				poolable.poolSettings.capacityLimitBehavior = (ObjectPool.PoolLimitBehavior)EditorGUILayout.EnumPopup("Pool Capacity Limit Behavior", poolable.poolSettings.capacityLimitBehavior);
				if (poolable.poolSettings.capacityLimitBehavior != ObjectPool.PoolLimitBehavior.None)
				{
					poolable.poolSettings.poolCapacity = EditorGUILayout.IntField("Pool Capacity", Mathf.Max(poolable.poolSettings.poolCapacity, 1));
				}
			}
			else
			{
				EditorGUILayout.LabelField("Editor error finding poolable.");
			}
		}
	}
}