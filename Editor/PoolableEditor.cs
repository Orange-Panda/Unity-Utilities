using LMirman.Utilities.ObjectPool;
using UnityEditor;
using UnityEngine;

namespace LMirman.Utilities.Editor
{
	[CustomEditor(typeof(Poolable))]
	public class PoolableEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (Application.isPlaying)
			{
				EditorGUILayout.LabelField("Pool settings can't be modified during play mode.");
				return;
			}

			SerializedProperty poolSettings = serializedObject.FindProperty("poolSettings");
			SerializedProperty capacityLimitBehavior = poolSettings.FindPropertyRelative("capacityLimitBehavior");
			SerializedProperty poolCapacity = poolSettings.FindPropertyRelative("poolCapacity");
			EditorGUILayout.PropertyField(capacityLimitBehavior);
			if (capacityLimitBehavior.intValue != (int)ObjectPool.ObjectPool.PoolLimitBehavior.None)
			{
				poolCapacity.intValue = EditorGUILayout.IntField("Pool Capacity", Mathf.Max(poolCapacity.intValue, 1));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}