using UnityEngine;

namespace LMirman.Utilities.ObjectPool
{
	/// <summary>
	/// Built in implementation for recycling particle systems using the <see cref="ObjectPool"/> system.
	/// <br/>
	/// Will automatically set the particle system's stop action to callback and return it to the pool when the stop callback is invoked.
	/// When the object is retrieved from the pool it will automatically play the system again and (optionally) clear old particles from the previous instance.
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