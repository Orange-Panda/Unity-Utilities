using JetBrains.Annotations;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Built in component for the <see cref="Poolable"/> that will automatically return it to the object pool after some time has passed.
	/// </summary>
	/// <remarks>
	/// Useful for visual effects that you don't want to explicitly manage the return of.
	/// </remarks>
	[RequireComponent(typeof(Poolable)), PublicAPI]
	public class PoolableReturnAfterDelay : MonoBehaviour
	{
		[SerializeField]
		private bool useUnscaledTime;
		[SerializeField, Min(0.001f)]
		private float returnAfterTime = 15f;

		private Poolable poolable;
		private float timer;
		private bool runningTimer;

		public float TimeRemaining => runningTimer ? timer : 0;
		public float TimerValue => timer;
		public bool Running => runningTimer;

		private void Awake()
		{
			poolable = GetComponent<Poolable>();
			poolable.ObjectRetrieved += RestartTimer;
			poolable.ObjectReturned += PauseTimer;
		}

		private void Update()
		{
			UpdateTimer();
		}

		private void UpdateTimer()
		{
			if (!runningTimer)
			{
				return;
			}

			timer = Mathf.MoveTowards(timer, 0, useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);

			if (timer > 0)
			{
				return;
			}

			PauseTimer();
			if (poolable.IsActive)
			{
				poolable.Return();
			}
		}

		/// <summary>
		/// Restart the timer to the specified <paramref name="value"/> and run the timer.
		/// </summary>
		/// <param name="value"></param>
		public void SetTimerValue(float value)
		{
			timer = value;
			runningTimer = true;
		}
		
		/// <summary>
		/// Restart the timer to the <see cref="returnAfterTime"/> and run the timer.
		/// </summary>
		public void RestartTimer()
		{
			SetTimerValue(returnAfterTime);
		}

		/// <summary>
		/// Resume the timer. Does not modify <see cref="TimeRemaining"/>.
		/// </summary>
		public void ResumeTimer()
		{
			runningTimer = true;
		}
		
		/// <summary>
		/// Pause the timer to automatically return this object.
		/// </summary>
		public void PauseTimer()
		{
			runningTimer = false;
		}
	}
}