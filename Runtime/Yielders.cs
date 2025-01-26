using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities
{
	// Based off of: https://forum.unity.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/#post-2151707
	/// <summary>
	/// Static class that caches references to common <see cref="YieldInstruction"/>s.
	/// </summary>
	/// <remarks>
	/// Usually when yielding you would create a new instance of the yield instruction.
	/// This allocates some garbage which needs to be cleaned up later.
	/// For applications with a significant amount of yield instruction calls this can save a moderate amount of memory on the garbage collector.
	/// </remarks>
	/// <example>
	/// <code>
	///		yield return Yielders.WaitForEndOfFrame;
	/// </code>
	/// <code>
	///		yield return Yielders.WaitForFixedUpdate;
	/// </code>
	/// <code>
	///		yield return Yielders.WaitForSeconds(0.75f);
	/// </code>
	///	<code>
	///		yield return Yielders.WaitForSecondsRealtime(0.25f);
	/// </code>
	/// </example>
	[PublicAPI]
	public static class Yielders
	{
		private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsMap = new Dictionary<float, WaitForSeconds>();
		private static readonly Dictionary<float, WaitForSecondsRealtime> WaitForSecondsRealtimeMap = new Dictionary<float, WaitForSecondsRealtime>();
		public static WaitForFixedUpdate WaitForFixedUpdate { get; } = new WaitForFixedUpdate();
		public static WaitForEndOfFrame WaitForEndOfFrame { get; } = new WaitForEndOfFrame();

		/// <summary>
		/// Get a cached reference to a WaitForSeconds yield instruction of the given value.
		/// </summary>
		/// <remarks>
		/// Since these values are permanently cached, avoid randomized floats since they are unlikely to ever produce the same value.
		/// </remarks>
		/// <param name="value">The amount of seconds to wait for</param>
		/// <returns>A cached reference to a WaitForSeconds yield instruction of the given value.</returns>
		public static WaitForSeconds WaitForSeconds(float value)
		{
			if (WaitForSecondsMap.TryGetValue(value, out WaitForSeconds entry))
			{
				return entry;
			}
			else
			{
				entry = new WaitForSeconds(value);
				WaitForSecondsMap.Add(value, entry);
				return entry;
			}
		}

		/// <summary>
		/// Get a cached reference to a WaitForSecondsRealtime yield instruction of the given value.
		/// </summary>
		/// <remarks>
		/// Since these values are permanently cached, avoid randomized floats since they are unlikely to ever produce the same value.
		/// </remarks>
		/// <param name="value">The amount of realtime seconds to wait for</param>
		/// <returns>A cached reference to a WaitForSecondsRealtime yield instruction of the given value.</returns>
		public static WaitForSecondsRealtime WaitForSecondsRealtime(float value)
		{
			if (WaitForSecondsRealtimeMap.TryGetValue(value, out WaitForSecondsRealtime entry))
			{
				return entry;
			}
			else
			{
				entry = new WaitForSecondsRealtime(value);
				WaitForSecondsRealtimeMap.Add(value, entry);
				return entry;
			}
		}
	}
}