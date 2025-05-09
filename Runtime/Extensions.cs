using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Static class defining various extension methods which are generally useful for game development.
	/// </summary>
	[PublicAPI]
	public static class Extensions
	{
		/// <summary>
		/// Shorthand way of determining if an index is within the bounds a collection.
		/// </summary>
		/// <remarks>
		/// This only validates the index is within the collection bounds, it does <b>not</b> validate if the entry at that index is null.
		/// </remarks>
		/// <param name="index">The index to check within the array bounds for</param>
		/// <param name="collection">The collection to check (typically an array or List&lt;T&gt;)</param>
		/// <returns>True if the index is a non-negative number which is less than the collection length.</returns>
		public static bool InBounds(this int index, ICollection collection)
		{
			return index >= 0 && index < collection.Count;
		}

		/// <inheritdoc cref="InBounds(int, System.Collections.ICollection)"/>
		public static bool InBounds(this ICollection collection, int index)
		{
			return index.InBounds(collection);
		}

		/// <summary>
		/// Moves a color <see cref="current"/> towards <see cref="target"/> color at a limited speed
		/// </summary>
		/// <param name="current">The current color value</param>
		/// <param name="target">The color value to move the current color value towards</param>
		/// <param name="maxDistanceDelta">The speed limit of the movement</param>
		/// <returns>The moved color towards the target</returns>
		[Pure]
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
		[Pure]
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
		[Pure]
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
		/// <returns>The resulting color using <see cref="current"/>'s RGB value and <see cref="target"/>'s value</returns>
		[Pure]
		public static Color SetA(this Color current, Color target)
		{
			current.a = target.a;
			return current;
		}

		/// <summary>
		/// Modify the <see cref="current"/> color's alpha value to the <see cref="target"/> alpha value while maintaining <see cref="current"/>'s RGB value.
		/// </summary>
		/// <param name="current">The color whose RGB value will be used</param>
		/// <param name="target">The alpha value to set</param>
		/// <returns>The resulting color using <see cref="current"/>'s RGB value and <see cref="target"/> alpha value</returns>
		[Pure]
		public static Color SetA(this Color current, float target)
		{
			current.a = target;
			return current;
		}

		/// <summary>
		/// Check if a layer mask contains a particular layer.
		/// </summary>
		/// <param name="mask">The layer mask to check for a particular layer value</param>
		/// <param name="layer">The layer to check for within <see cref="mask"/>. See: <see cref="GameObject.layer"/></param>
		/// <returns>True if the <see cref="mask"/> has <see cref="layer"/>, false if it does not.</returns>
		public static bool LayerMaskContains(this int mask, int layer)
		{
			return mask == (mask | (1 << layer));
		}

		/// <summary>
		/// Rotate a Vector2 about the Z axis.
		/// </summary>
		/// <remarks>
		/// Does <i>not</i> mutate the value of the Vector2 operated upon.
		/// </remarks>
		/// <param name="vector">The source vector to transform.</param>
		/// <param name="degrees">
		/// The degrees to rotate the vector about the Z axis.
		/// Uses the same rotation rules as Unity's left-handed coordinate system: positive values rotate counter-clockwise, negative values rotate clockwise.
		/// </param>
		/// <returns>The resulting Vector2 from the rotation transformation.</returns>
		[Pure]
		public static Vector2 Rotate(this Vector2 vector, float degrees)
		{
			float delta = degrees * Mathf.Deg2Rad;
			float cos = Mathf.Cos(delta);
			float sin = Mathf.Sin(delta);
			float newX = (vector.x * cos) - (vector.y * sin);
			float newY = (vector.x * sin) + (vector.y * cos);
			return new Vector2(newX, newY);
		}
	}
}