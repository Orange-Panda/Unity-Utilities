using System;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities
{
	public static class Extensions
	{
		/// <summary>
		/// Shorthand way of determining if an index is within an <see cref="Array.Length"/>.
		/// </summary>
		/// <remarks>
		/// While this does validate the index is within the array it does <b>not</b> validate if the entry at that index is null.
		/// </remarks>
		/// <param name="index">The index to check within the array bounds for</param>
		/// <param name="array">The array to check</param>
		/// <returns>True if the index was a non-negative number and less than the array's length.</returns>
		public static bool InBounds(this int index, Array array)
		{
			return index >= 0 && index < array.Length;
		}

		/// <summary>
		/// Shorthand way of determining if an index is within an <see cref="List{T}.Count"/>.
		/// </summary>
		/// <remarks>
		/// While this does validate the index is within the list it does <b>not</b> validate if the entry at that index is null.
		/// </remarks>
		/// <param name="index">The index to check within the array bounds for</param>
		/// <param name="list">The list to check</param>
		/// <returns>True if the index was a non-negative number and less than the list's count.</returns>
		public static bool InBounds<T>(this int index, List<T> list)
		{
			return index >= 0 && index < list.Count;
		}
	
		/// <summary>
		/// Moves a color <see cref="current"/> towards <see cref="target"/> color at a limited speed
		/// </summary>
		/// <param name="current">The current color value</param>
		/// <param name="target">The color value to move the current color value towards</param>
		/// <param name="maxDistanceDelta">The speed limit of the movement</param>
		/// <returns>The moved color towards the target</returns>
		public static Color MoveTowards(this Color current, Color target, float maxDistanceDelta)
		{
			return Vector4.MoveTowards(current, target, maxDistanceDelta);
		}

		/// <summary>
		/// Moves a color <see cref="current"/>'s alpha value towards a <see cref="targetAlpha"/> value at a limited speed. Maintains RGB values.
		/// </summary>
		/// <param name="current">The current color value</param>
		/// <param name="targetAlpha">The target alpha value to move the current color towards</param>
		/// <param name="maxDistanceDelta">The speed limit of the movement</param>
		/// <returns>The moved color towards the target</returns>
		public static Color MoveTowardsAlpha(this Color current, float targetAlpha, float maxDistanceDelta)
		{
			return new Color(current.r, current.g, current.b, Mathf.MoveTowards(current.a, targetAlpha, maxDistanceDelta));
		}

		/// <summary>
		/// Modify the <see cref="current"/> color's RGB value to the <see cref="target"/>'s RGB value while maintaining <see cref="current"/>'s alpha value.
		/// </summary>
		/// <param name="current">The color whose alpha value will be used</param>
		/// <param name="target">The color whose RGB values will be used</param>
		/// <returns>The resulting color using <see cref="current"/>'s alpha value and <see cref="target"/>'s RGB value</returns>
		public static Color SetRGB(this Color current, Color target)
		{
			target.a = current.a;
			return target;
		}

		/// <summary>
		/// Modify the <see cref="current"/> color's alpha value to the <see cref="target"/>'s alpha value while maintaining <see cref="current"/>'s RGB value.
		/// </summary>
		/// <param name="current">The color whose RGB value will be used</param>
		/// <param name="target">The color whose alpha values will be used</param>
		/// <returns>The resulting color using <see cref="current"/>'s RGB value and <see cref="target"/>'s value value</returns>
		public static Color SetA(this Color current, Color target)
		{
			current.a = target.a;
			return current;
		}
	}
}