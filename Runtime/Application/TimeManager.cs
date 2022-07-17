using System;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Responsible for controlling the <see cref="Time.timeScale"/> in a manner that maintains time scale between pausing/resuming the game.
	/// </summary>
	/// <remarks>
	/// Due to the existence of this class: avoid changing <see cref="Time.timeScale"/> without utilizing this one!
	/// </remarks>
	public static class TimeManager
	{
		/// <summary>
		/// True if the game is currently paused, false if it is not.
		/// </summary>
		public static bool IsPaused { get; private set; }
		/// <summary>
		/// The current practical time scale of the game, with consideration to <see cref="IsPaused"/>.
		/// Evaluates to 0 when <see cref="IsPaused"/> or <see cref="TimeScalar"/> if not <see cref="IsPaused"/>
		/// </summary>
		public static float TimeScale => IsPaused ? 0 : TimeScalar;
		/// <summary>
		/// The time scale the game wants to be at regardless of <see cref="IsPaused"/>.
		/// </summary>
		public static float TimeScalar { get; private set; } = 1;
	
		/// <summary>
		/// Fired when the pause state has changed. True if paused, false if unpaused.
		/// </summary>
		public static event Action<bool> OnPauseChanged = delegate { };
	
		/// <summary>
		/// Pause the game simulation, freezing time.
		/// </summary>
		public static void Pause()
		{
			IsPaused = true;
			Time.timeScale = TimeScale;
			OnPauseChanged.Invoke(true);
		}

		/// <summary>
		/// Resume the game simulation, returning the simulation speed to <see cref="TimeScalar"/>.
		/// </summary>
		public static void Unpause()
		{
			IsPaused = false;
			Time.timeScale = TimeScale;
			OnPauseChanged.Invoke(false);
		}

		/// <summary>
		/// Reset the scalar back to realtime.
		/// </summary>
		public static void ClearScalar()
		{
			SetScalar(1);
		}

		/// <summary>
		/// Set the simulation speed scalar for when the game is unpaused.
		/// </summary>
		/// <param name="value">The simulation speed scalar</param>
		public static void SetScalar(float value)
		{
			TimeScalar = Mathf.Clamp(value, 0, 32);
			Time.timeScale = TimeScale;
		}
	}
}