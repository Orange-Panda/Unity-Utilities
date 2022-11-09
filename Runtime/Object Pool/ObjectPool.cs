using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LMirman.Utilities
{
	public static class ObjectPool
	{
		/// <summary>
		/// All object pools using the template prefab as a key.
		/// </summary>
		private static readonly Dictionary<GameObject, Pool> Pools = new Dictionary<GameObject, Pool>();

		/// <summary>
		/// Ensures that there is a <see cref="Pool"/> reference for this <paramref name="template"/> in the <see cref="Pools"/>.
		/// </summary>
		/// <param name="template"></param>
		private static void VerifyLookupPresence(GameObject template)
		{
			if (!Pools.ContainsKey(template))
			{
				Pools.Add(template, new Pool(template));
			}
		}

		/// <summary>
		/// Pulls an idle object or populates a new one for the <see cref="template"/> object.
		/// </summary>
		/// <remarks>
		/// This does NOT call <see cref="Poolable.OnRetrieved"/> so make sure to invoke this at the end of initialization!
		/// </remarks>
		private static Poolable GetObject(GameObject template)
		{
			VerifyLookupPresence(template);
			return Pools[template].GetObject();
		}

		public static Poolable Instantiate(GameObject template)
		{
			Poolable poolable = GetObject(template);
			Pools[template].OnRetrieveObject(poolable);
			return poolable;
		}

		public static Poolable Instantiate(GameObject template, Transform parent)
		{
			Poolable poolable = GetObject(template);
			poolable.transform.SetParent(parent);
			Pools[template].OnRetrieveObject(poolable);
			return poolable;
		}

		public static Poolable Instantiate(GameObject template, Vector3 position, Quaternion rotation, Space space = Space.World)
		{
			Poolable poolable = GetObject(template);
			SetPositionAndRotation(poolable, position, rotation, space);
			Pools[template].OnRetrieveObject(poolable);
			return poolable;
		}

		public static Poolable Instantiate(GameObject template, Vector3 position, Quaternion rotation, Transform parent, Space space = Space.World)
		{
			Poolable poolable = GetObject(template);
			poolable.transform.SetParent(parent);
			SetPositionAndRotation(poolable, position, rotation, space);
			Pools[template].OnRetrieveObject(poolable);
			return poolable;
		}

		private static void SetPositionAndRotation(Poolable poolable, Vector3 position, Quaternion rotation, Space space)
		{
			if (space == Space.Self)
			{
				poolable.transform.localPosition = position;
				poolable.transform.localRotation = rotation;
			}
			else
			{
				poolable.transform.SetPositionAndRotation(position, rotation);
			}
		}

		internal static void NotifyObjectReturned(Poolable poolable)
		{
			VerifyLookupPresence(poolable.template);
			Pools[poolable.template].ReturnObject(poolable);
		}

		internal static void NotifyObjectDisposed(Poolable poolable)
		{
			VerifyLookupPresence(poolable.template);
			Pools[poolable.template].DisposeObject(poolable);
		}

		public static Pool GetPool(GameObject template)
		{
			VerifyLookupPresence(template);
			return Pools[template];
		}

		/// <summary>
		/// Goes through every single <see cref="Pool"/> and disposes of all currently idle objects.
		/// </summary>
		/// <remarks>
		/// Not necessary to utilize unless your application has rapidly changing object demands.
		/// </remarks>
		public static void DisposeAllIdlePools()
		{
			foreach (Pool pool in Pools.Values)
			{
				pool.DisposeIdle();
			}
		}

		public class Pool
		{
			/// <summary>
			/// All of the pooled objects regardless of active status
			/// </summary>
			private readonly List<Poolable> populatedObjects = new List<Poolable>();

			/// <summary>
			/// The pooled objects that are currently active in the scene.
			/// </summary>
			private readonly List<Poolable> activeObjects = new List<Poolable>();

			/// <summary>
			/// The pooled objects that are currently waiting to be activated.
			/// </summary>
			private readonly List<Poolable> idleObjects = new List<Poolable>();

			private readonly PoolSettings settings;
			private readonly GameObject template;

			public int ObjectCount => populatedObjects.Count;
			public int ActiveCount => activeObjects.Count;
			public int IdleCount => idleObjects.Count;

			public Pool(GameObject template)
			{
				this.template = template;
				if (template.TryGetComponent(out Poolable poolable))
				{
					settings = poolable.poolSettings.Copy();
					settings.poolCapacity = Mathf.Max(settings.poolCapacity, 1);
				}
				else
				{
					throw new ArgumentException(
						"The template provided does not have a \"Poolable\" component attached!\nEnsure there is a poolable component present and it is at the root level of the template.");
				}
			}

			internal Poolable GetObject()
			{
				if (idleObjects.Count > 0)
				{
					return RetrieveIdleObject();
				}
				else if (activeObjects.Count >= settings.poolCapacity)
				{
					switch (settings.capacityLimitBehavior)
					{
						case PoolLimitBehavior.None:
						case PoolLimitBehavior.DisposeOnReturn:
							return PopulateNewObject();
						case PoolLimitBehavior.RetrieveOldestActive:
							return RetrieveOldest();
						case PoolLimitBehavior.RecycleOldestActive:
							return RecycleOldest();
						case PoolLimitBehavior.RejectPopulation:
							return null;
						case PoolLimitBehavior.ThrowException:
							throw new Exception("Tried to instantiate a pooled object but the pool is at capacity!");
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					return PopulateNewObject();
				}
			}

			private Poolable PopulateNewObject()
			{
				GameObject gameObject = Object.Instantiate(template);
				gameObject.SetActive(false);
				if (gameObject.TryGetComponent(out Poolable poolable))
				{
					populatedObjects.Add(poolable);
					poolable.OnPopulated(template);
					return poolable;
				}
				else
				{
					Object.Destroy(gameObject);
					throw new ArgumentException(
						"The template provided does not have a \"Poolable\" component attached!\nEnsure there is a poolable component present and it is at the root level of the template.");
				}
			}

			private Poolable RetrieveOldest()
			{
				Poolable target = activeObjects[0];
				activeObjects.RemoveAt(0);
				activeObjects.Add(target);
				return target;
			}

			private Poolable RecycleOldest()
			{
				Poolable target = activeObjects[0];
				target.Return();
				return target;
			}

			private Poolable RetrieveIdleObject()
			{
				return idleObjects[idleObjects.Count - 1];
			}

			internal void OnRetrieveObject(Poolable poolable)
			{
				activeObjects.Add(poolable);
				idleObjects.Remove(poolable);
				poolable.OnRetrieved();
			}

			/// <summary>
			/// Return an object to the <see cref="idleObjects"/> when it is done being used in the scene.
			/// </summary>
			/// <param name="poolable">The object to return to the pool.</param>
			internal void ReturnObject(Poolable poolable)
			{
				if (!populatedObjects.Contains(poolable))
				{
					throw new Exception(
						$"The poolable object \"{poolable.name}\" tried to return to the pool but it is no longer being tracked.\nThis is likely a result of Returning to pool after being disposed or an internal error.");
				}
				else if (idleObjects.Contains(poolable))
				{
					throw new Exception(
						$"The poolable object \"{poolable.name}\" tried to return to the pool but it is already in the idle pool!\nAvoid invoking Return() on the Poolable object when it is not currently active.");
				}

				activeObjects.Remove(poolable);
				idleObjects.Add(poolable);

				// Dispose of excessive objects if the idle pool is over capacity.
				// We compare to the idle objects pool instead of populatedObjects so we can at least somewhat benefit from the pool.
				// If we were to use populatedObjects it would just perpetually create/destroy objects when over capacity, defeating the purpose of the pool system.
				if (settings.capacityLimitBehavior == PoolLimitBehavior.DisposeOnReturn && idleObjects.Count > settings.poolCapacity)
				{
					Object.Destroy(poolable.gameObject);
				}
			}

			/// <summary>
			/// Removes the <paramref name="poolable"/> from the pool entirely.
			/// This should only be done when the object is destined to be destroyed, otherwise it might be permanently in the scene.
			/// </summary>
			/// <param name="poolable">The object to dispose from the pool.</param>
			internal void DisposeObject(Poolable poolable)
			{
				populatedObjects.Remove(poolable);
				activeObjects.Remove(poolable);
				idleObjects.Remove(poolable);
			}

			/// <summary>
			/// Dispose all <see cref="idleObjects"/>.
			/// </summary>
			public void DisposeIdle()
			{
				Queue<Poolable> invokeTargets = new Queue<Poolable>();
				foreach (Poolable poolable in idleObjects)
				{
					invokeTargets.Enqueue(poolable);
				}
				
				while (invokeTargets.Count > 0)
				{
					Poolable target = invokeTargets.Dequeue();
					Object.Destroy(target.gameObject);
				}
			}
			
			/// <summary>
			/// Return all <see cref="activeObjects"/> to the <see cref="idleObjects"/> and then dispose of everything.
			/// </summary>
			public void DisposeAll()
			{
				Queue<Poolable> invokeTargets = new Queue<Poolable>();
				foreach (Poolable poolable in activeObjects)
				{
					invokeTargets.Enqueue(poolable);
				}

				while (invokeTargets.Count > 0)
				{
					Poolable target = invokeTargets.Dequeue();
					target.Return();
				}
				
				DisposeIdle();
			}
		}

		[Serializable]
		public class PoolSettings
		{
			[Tooltip("What behavior the pool should have when the pool is at/beyond its capacity.")]
			public PoolLimitBehavior capacityLimitBehavior = PoolLimitBehavior.None;

			[Tooltip("The maximum number of objects to keep in the object pool. Only relevant if \"capacityLimitBehavior\" is enabled")]
			public int poolCapacity = 32;

			public PoolSettings()
			{

			}

			public PoolSettings(PoolLimitBehavior capacityLimitBehavior, int poolCapacity)
			{
				this.capacityLimitBehavior = capacityLimitBehavior;
				this.poolCapacity = poolCapacity;
			}

			public PoolSettings Copy()
			{
				return new PoolSettings(capacityLimitBehavior, poolCapacity);
			}
		}

		public enum PoolLimitBehavior
		{
			/// <summary>
			/// Does not enforce any capacity limit on the pool.
			/// </summary>
			/// <remarks>
			/// The pool can have theoretically unlimited objects populating it.
			/// Useful if you know you are going to need a uncertain amount of objects but will never have an excessive amount at any time.
			/// </remarks>
			None,

			/// <summary>
			/// The object will be instantiated over the capacity, however when an object is returned to the pool when over capacity it is immediately disposed.
			/// </summary>
			/// <remarks>
			/// This is useful for gameplay objects that you don't want to keep an excessive amount of in the pool.
			/// </remarks>
			DisposeOnReturn,

			/// <summary>
			/// Immediately retrieves the oldest active poolable object. This does <b>not</b> invoke Return()!
			/// </summary>
			/// <remarks>
			/// Mainly useful for reusing uncommon visual effects when at capacity. Avoid using this for objects relevant to gameplay.
			/// </remarks>
			RetrieveOldestActive,

			/// <summary>
			/// Immediately returns and then retrieves the oldest active poolable object back to the pool.
			/// </summary>
			/// <remarks>
			/// Mainly useful for reusing uncommon visual effects when at capacity. Avoid using this for objects relevant to gameplay.
			/// Unlike <see cref="RetrieveOldestActive"/>, this is useful if you have important code to run when the object is returned to the idle pool.
			/// </remarks>
			RecycleOldestActive,

			/// <summary>
			/// If trying to instantiate a new object when at capacity: disregard the attempt without throwing an exception.
			/// </summary>
			/// <remarks>
			/// Useful if you want an upper limit on number of active visual effects. Avoid using this for objects relevant to gameplay.
			/// </remarks>
			RejectPopulation,

			/// <summary>
			/// If trying to instantiate a new object when at capacity: an exception is thrown.
			/// </summary>
			/// <remarks>
			/// This is useful for objects you want to manually keep track of and prevent exceeding a limit.
			/// Since an exception you will be notified when your system has failed.
			/// </remarks>
			ThrowException
		}
	}
}