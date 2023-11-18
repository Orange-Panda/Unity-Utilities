using System;
using UnityEngine;

namespace LMirman.Utilities
{
	public sealed class Poolable : MonoBehaviour
	{
		internal GameObject template;

		public ObjectPool.PoolSettings poolSettings;

		public bool IsPopulated { get; private set; }
		public bool IsActive { get; private set; }
		public bool IsDisposed { get; private set; }

		public event Action ObjectPopulated = delegate { };
		public event Action ObjectRetrieved = delegate { };
		public event Action ObjectReturned = delegate { };
		public event Action ObjectDisposed = delegate { };

		private void Start()
		{
			if (IsPopulated)
			{
				return;
			}

			Debug.LogError("A poolable prefab was instantiated outside of the ObjectPool system!" +
						   "Poolables must be created using the ObjectPool.Instantiate() method." +
						   "This object will be destroyed to prevent the object from lingering forever.");
			Destroy(gameObject);
		}

		/// <summary>
		/// Invoked by the <see cref="ObjectPool"/> when the object is created for the first time.
		/// </summary>
		internal void OnPopulated(GameObject sourceTemplate)
		{
			if (IsPopulated)
			{
				throw new Exception($"Tried to populate the \"{name}\" poolable but it has already been populated!");
			}

			template = sourceTemplate;
			IsPopulated = true;
			ObjectPopulated.Invoke();
		}

		/// <summary>
		/// Invoked by the <see cref="ObjectPool"/> when the object is pulled into the scene from the pool
		/// </summary>
		internal void OnRetrieved()
		{
			IsActive = true;
			gameObject.SetActive(true);
			ObjectRetrieved.Invoke();
		}

		/// <summary>
		/// Return the object from the scene back into the pool.
		/// </summary>
		public void Return()
		{
			if (IsDisposed)
			{
				throw new Exception("There was an attempt to return a disposed object to the object pool!\nOnce an item has been disposed/destroyed it is unusable.");
			}
			else if (!IsActive)
			{
				Debug.LogWarning("There was an attempt to return a poolable object into the object pool, but it is already inactive.");
				return;
			}

			IsActive = false;
			gameObject.SetActive(false);
			transform.SetParent(null);
			ObjectPool.NotifyObjectReturned(this);
			ObjectReturned.Invoke();
		}

		private void OnDestroy()
		{
			IsDisposed = true;
			IsActive = false;
			ObjectPool.NotifyObjectDisposed(this);
			ObjectDisposed.Invoke();
		}
	}
}