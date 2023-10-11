using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Base class for a group of ui elements, an assortment of objects to enable and disable at runtime. 
	/// </summary>
	/// <remarks>
	/// A UIGroup assumes that the items within are mutually exclusive.
	/// Setting a particular item as active will disable other objects.
	/// </remarks>
	/// <typeparam name="T">The <see cref="UIGroupItem"/> this UI group represents.</typeparam>
	[PublicAPI]
	public abstract class UIGroup<T> : MonoBehaviour where T : UIGroupItem
	{
		[SerializeField, Tooltip("Items that are loaded by this UIGroup in Start.")]
		protected List<T> initialItems;

		protected int currentIndex;
		protected T currentItem;

		public T CurrentItem => currentItem;
		public int CurrentIndex => currentIndex;

		protected readonly Dictionary<string, int> itemLookup = new Dictionary<string, int>();
		protected readonly List<T> items = new List<T>();

		public event Action<T> ItemChanged = delegate { };
		public event Action<int> ItemIndexChanged = delegate {  };

		protected virtual void Start()
		{
			ReloadInitialItems();
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
			// Ignore request if we are trying to set the currently active item.
			if (currentItem == item)
			{
				return;
			}

			// Disable current item: mutually exclusive rule.
			if (currentItem != null)
			{
				currentItem.SetActive(false);
			}

			item.SetActive(true);
			currentIndex = index;
			currentItem = item;
			OnItemChanged(item);
			OnIndexChanged(index);
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

		public void DisableAllItems()
		{
			foreach (T item in items)
			{
				item.SetActive(false);
			}

			currentIndex = -1;
			currentItem = null;
			OnItemChanged(null);
			OnIndexChanged(-1);
		}

		protected virtual void OnItemChanged(T item)
		{
			ItemChanged.Invoke(item);
		}

		protected virtual void OnIndexChanged(int index)
		{
			ItemIndexChanged.Invoke(index);
		}

		#region List Mutation
		/// <summary>
		/// Insert a new item into this UI group
		/// </summary>
		public void InsertItem(T value)
		{
			itemLookup.Add(value.Key, items.Count);
			items.Add(value);
		}

		/// <summary>
		/// Remove an instance of an item from this UI group
		/// </summary>
		public void RemoveItem(T value)
		{
			items.Remove(value);
			RefreshMutatedValues();
		}

		/// <summary>
		/// Remove all instances of an item from this UI group
		/// </summary>
		public void RemoveAllItemInstances(T value)
		{
			items.RemoveAll(item => item == value);
			RefreshMutatedValues();
		}

		/// <summary>
		/// Remove an instance of an item from this UI group
		/// </summary>
		public void RemoveItemAtIndex(int value)
		{
			items.RemoveAt(value);
			RefreshMutatedValues();
		}

		/// <summary>
		/// Remove all items from the group's list and repopulate it with values from <see cref="initialItems"/>.
		/// </summary>
		public void ReloadInitialItems()
		{
			itemLookup.Clear();
			items.Clear();

			for (int i = 0; i < initialItems.Count; i++)
			{
				if (initialItems[i] == null)
				{
					Debug.LogError($"Game Object \"{gameObject.name}\" contains invalid ui group reference at index {i}.");
					continue;
				}

				InsertItem(initialItems[i]);
			}
		}

		protected void RefreshMutatedValues()
		{
			int previousIndex = currentIndex;
			RebuildLookup();
			UpdateIndex();
			if (previousIndex != currentIndex)
			{
				OnIndexChanged(currentIndex);
			}
		}

		/// <summary>
		/// Rebuild lookup table.
		/// Only necessary if <see cref="items"/> was mutated.
		/// </summary>
		private void RebuildLookup()
		{
			itemLookup.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				T uiGroupItem = items[i];
				itemLookup.Add(uiGroupItem.Key, i);
			}
		}

		/// <summary>
		/// Update the current index on item removal, since it may have changed the index of the current item.
		/// </summary>
		private void UpdateIndex()
		{
			if (currentItem != null)
			{
				currentIndex = items.FindIndex(item => item == currentItem);
			}
			else
			{
				currentIndex = -1;
			}
		}
		#endregion
	}
}