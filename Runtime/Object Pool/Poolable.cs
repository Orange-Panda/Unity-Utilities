using JetBrains.Annotations;
using System;
using UnityEngine;

namespace LMirman.Utilities
{
	[PublicAPI]
	public sealed class Poolable : MonoBehaviour
	{
		/// <summary>
		/// Poolables with this identifier value are not currently active.
		/// </summary>
		public const int InvalidIdentifier = -1;
		/// <summary>
		/// The identifier value to be assigned to the next poolable retrieved. 
		/// </summary>
		private static int nextIdentifierValue = 1;
		internal GameObject template;

		[SerializeField]
		private ObjectPool.PoolSettings poolSettings;

		/// <summary>
		/// When true the object has been established into the ObjectPool system.<br/><br/>
		/// A false value means the object was created with the standard Object.Instantiate method or the poolable is being accessed too early the lifecycle.
		/// </summary>
		public bool IsPopulated { get; private set; }
		/// <summary>
		/// When true the object is currently active in the game world.<br/><br/>
		/// When false the object has either been disposed or is idle in a <see cref="ObjectPool.Pool"/>.
		/// </summary>
		public bool IsActive { get; private set; }
		/// <summary>
		/// When true the object has been disposed (deleted) and is considered unusable.
		/// </summary>
		public bool IsDisposed { get; private set; }
		/// <summary>
		/// A unique value assigned to this poolable when retrieved into the game world and revoked when returned to the object pool.
		/// </summary>
		/// <remarks>
		/// Can be utilized to distinguish instances of a poolable object that has gone through the object pool cycle multiple times.<br/><br/>
		/// It is recommended to use <see cref="CreateInstanceIdentity"/> to reference a poolable until it has been returned.
		/// </remarks>
		public int Identifier { get; private set; }
		internal ObjectPool.PoolSettings PoolSettings => poolSettings;

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
			Identifier = PullIdentifierValue();
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

			if (!IsActive)
			{
				Debug.LogWarning("There was an attempt to return a poolable object into the object pool, but it is already inactive.");
				return;
			}

			IsActive = false;
			Identifier = InvalidIdentifier;
			gameObject.SetActive(false);
			transform.SetParent(null);
			ObjectPool.NotifyObjectReturned(this);
			ObjectReturned.Invoke();
		}

		private void OnDestroy()
		{
			IsDisposed = true;
			IsActive = false;
			Identifier = InvalidIdentifier;
			ObjectPool.NotifyObjectDisposed(this);
			ObjectDisposed.Invoke();
		}

		/// <summary>
		/// Create an instance identity for this poolable.
		/// </summary>
		/// <remarks>
		/// Everytime this method is called it creates a new <see cref="Instance"/> object.
		/// You are <i>highly</i> encouraged to only create an instance once when the the poolable object is instantiated.<br/><br/>
		/// Usage of this method is <b>not</b> required to make use of the object pool system.
		/// However, it does provide a native way to dereference a poolable object when it has been returned to the object pool.
		/// </remarks>
		/// <returns>Return a new <see cref="Instance"/> that points to this poolable until it is returned or disposed of.</returns>
		[Pure]
		public Instance CreateInstanceIdentity()
		{
			return new Instance(this);
		}

		private static int PullIdentifierValue()
		{
			int value = nextIdentifierValue;
			nextIdentifierValue++;
			return value;
		}

		[PublicAPI]
		public class Instance
		{
			public readonly int identifier;
			private readonly Poolable poolable;

			public bool IsActive => poolable != null && identifier == poolable.Identifier;
			public Poolable Poolable => IsActive ? poolable : null;

			public static implicit operator bool(Instance instance) => instance.IsActive;
			public static explicit operator Poolable(Instance instance) => instance.Poolable;

			internal Instance(Poolable poolable)
			{
				if (poolable == null || poolable.Identifier == InvalidIdentifier)
				{
					this.poolable = null;
					Debug.LogWarning("A poolable instance was created from an invalid poolable source. This is likely due to creating an instance from a returned or disposed poolable.");
					return;
				}

				this.poolable = poolable;
				identifier = poolable.Identifier;
			}
		}
	}
}