using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// An optional <see cref="MonoBehaviour"/> class that automatically subscribes to C# events of the <see cref="Poolable"/> to make implementation easier.
	/// </summary>
	[RequireComponent(typeof(Poolable))]
	public class PoolableBehaviour : MonoBehaviour
	{
		protected Poolable poolable;

		protected void Awake()
		{
			poolable = GetComponent<Poolable>();
			poolable.ObjectPopulated += OnPopulated;
			poolable.ObjectRetrieved += OnRetrieved;
			poolable.ObjectReturned += OnReturned;
			poolable.ObjectDisposed += OnDisposed;
		}

		protected virtual void OnPopulated() { }

		protected virtual void OnRetrieved() { }

		protected virtual void OnReturned() { }

		protected virtual void OnDisposed() { }
	}
}