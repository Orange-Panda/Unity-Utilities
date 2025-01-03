using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace LMirman.Utilities.ObjectPool
{
	/// <summary>
	/// Built in component for the <see cref="Poolable"/> object that will automatically fire a UnityEvent when the matching C# event is invoked on <see cref="Poolable"/>.
	/// </summary>
	[RequireComponent(typeof(Poolable)), PublicAPI]
	public class PoolableUnityEventEmitter : MonoBehaviour
	{
		[SerializeField] 
		private UnityEvent onPopulated = new UnityEvent();
		[SerializeField] 
		private UnityEvent onRetrieved = new UnityEvent();
		[SerializeField] 
		private UnityEvent onReturned = new UnityEvent();
		[SerializeField] 
		private UnityEvent onDisposed = new UnityEvent();

		public UnityEvent OnPopulated => onPopulated;
		public UnityEvent OnRetrieved => onRetrieved;
		public UnityEvent OnReturned => onReturned;
		public UnityEvent OnDisposed => onDisposed;

		private Poolable poolable;
		
		private void Awake()
		{
			poolable = GetComponent<Poolable>();
			poolable.ObjectPopulated += () => onPopulated.Invoke();
			poolable.ObjectRetrieved += () => onRetrieved.Invoke();
			poolable.ObjectReturned += () => onReturned.Invoke();
			poolable.ObjectDisposed += () => onDisposed.Invoke();
		}
	}
}