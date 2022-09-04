using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Built in implementation for recycling particle systems using the <see cref="ObjectPool"/> system.
	/// </summary>
	[RequireComponent(typeof(ParticleSystem), typeof(Poolable))]
	public class PoolableParticleSystem : MonoBehaviour
	{
		[SerializeField]
		private bool clearSystemOnRetrieve;

		private Poolable poolable;
		private ParticleSystem system;

		private void Awake()
		{
			poolable = GetComponent<Poolable>();
			system = GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = system.main;
			main.stopAction = ParticleSystemStopAction.Callback;
			poolable.ObjectRetrieved += PoolableOnObjectRetrieved;
		}

		private void PoolableOnObjectRetrieved()
		{
			if (clearSystemOnRetrieve)
			{
				system.Clear();
			}

			system.Play();
		}

		private void OnParticleSystemStopped()
		{
			poolable.Return();
		}
	}
}