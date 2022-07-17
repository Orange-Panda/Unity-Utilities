using System;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Base class for a group of ui elements, an assortment of objects to enable and disable at runtime. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class UIGroup<T> : MonoBehaviour where T : UIGroupItem
	{
		[SerializeField] 
		protected List<T> initialItems;

		protected int currentIndex;
		protected T currentItem;
	
		protected readonly Dictionary<string, int> itemLookup = new Dictionary<string, int>();
		protected readonly List<T> items = new List<T>();

		public event Action<T> ItemChanged = delegate { };

		protected virtual void Start()
		{
			for (int i = 0; i < initialItems.Count; i++)
			{
				if (initialItems[i] == null)
				{
					Debug.LogError($"Game Object \"{gameObject.name}\" contains invalid ui group reference at {i}.");
					continue;
				}
			
				itemLookup.Add(initialItems[i].Key, i);
				items.Add(initialItems[i]);
			}

			DisableAllItems();
			SetActiveItem(0);
		}

		public void SetActiveItem(string key)
		{
			SetActiveItem(GetIndexAtKey(key));
		}

		public void SetActiveItem(int index)
		{
			if (index < 0 || index >= items.Count)
			{
				return;
			}

			T item = items[index];
			if (currentItem != item)
			{
				if (currentItem != null)
				{
					currentItem.SetActive(false);
				}
				item.SetActive(true);
				currentIndex = index;
				currentItem = item;
				OnItemChanged(item);
			}
		}

		public int GetIndexAtKey(string key)
		{
			if (itemLookup.TryGetValue(key, out int index))
			{
				return index;
			}

			Debug.LogError($"No ui group item found with key {key}");
			return -1;
		}

		protected void DisableAllItems()
		{
			foreach (T item in items)
			{
				item.SetActive(false);
			}

			OnItemChanged(null);
		}

		protected virtual void OnItemChanged(T item)
		{
			ItemChanged.Invoke(item);
		}
	}
}