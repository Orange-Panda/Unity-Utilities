using Newtonsoft.Json;
using System;

/// <summary>
/// A datatype that is always kept between a minimum and maximum value.
/// </summary>
/// <typeparam name="T">Type for the value you want to keep keep track of and limit within the boundaries</typeparam>
[Serializable]
public class ClampedValue<T> where T : IComparable
{
	protected T currentValue;
	protected T currentMin;
	protected T currentMax;
	
	/// <summary>
	/// The current value
	/// </summary>
	public T Value { get => currentValue; set => currentValue = GetInBounds(value); }
	
	/// <summary>
	/// The lower bound for the value.
	/// </summary>
	/// <remarks>
	/// <see cref="Value"/> will always be greater than this.<br/>
	/// There is <b>never</b> any check this is not greater than <see cref="Max"/> so it is your responsibility to validate the bounds.
	/// </remarks>
	public virtual T Min { get => currentMin; set { currentMin = value; Validate(); } }
	
	/// <summary>
	/// The upper bound for the value.
	/// </summary>
	/// <remarks>
	/// <see cref="Value"/> will never be greater than this unless the minimum value is greater than the maximum.
	/// </remarks>
	public virtual T Max { get => currentMax; set { currentMax = value; Validate(); } }

	public static implicit operator T(ClampedValue<T> value) => value.Value;

	/// <summary>
	/// Create a definition for a value that is always kept between a <paramref name="min"/> and <paramref name="max"/> value.
	/// </summary>
	/// <remarks>
	/// There is <b>never</b> any check the <paramref name="min"/> is not greater than <paramref name="max"/> so it is your responsibility to validate the bounds.<br/>
	/// If the <paramref name="min"/> does happen to be greater than <paramref name="max"/>: <see cref="Value"/> will always be equal to <paramref name="min"/>.
	/// </remarks>
	/// <param name="initialValue">The starting value</param>
	/// <param name="min">The lower bound for the value</param>
	/// <param name="max">The upper bound for the value</param>
	public ClampedValue(T initialValue, T min, T max)
	{
		currentMax = max;
		currentMin = min;
		Value = initialValue;
	}

	/// <summary>
	/// Ensures the value is within range.
	/// </summary>
	/// <remarks>
	/// Changing <see cref="Min"/> or <see cref="Max"/> already validates so you usually do not need to call this manually.
	/// </remarks>
	public void Validate()
	{
		Value = currentValue;
	}

	private T GetInBounds(T value)
	{
		if (Compare(value, Eval.Less, Min))
		{
			return Min;
		}
		else if (Compare(value, Eval.Greater, Max))
		{
			return Max;
		}
		else
		{
			return value;
		}
	}

	private static bool Compare(T a, Eval eval, T b)
	{
		// -1 - a < b
		//  0 - a == b
		//  1 - a > b
		int result = a.CompareTo(b);
		switch (result)
		{
			case -1:
				return eval.HasFlag(Eval.Less);
			case 0:
				return eval.HasFlag(Eval.Equal);
			case 1:
				return eval.HasFlag(Eval.Greater);
			default:
				throw new ArgumentException();
		}
	}

	[Flags]
	private enum Eval
	{
		/// <summary>
		/// &lt;
		/// </summary>
		Less = 1,
		/// <summary>
		/// ==
		/// </summary>
		Equal = 2,
		/// <summary>
		/// &gt;
		/// </summary>
		Greater = 4
	}
}

/// <summary>
/// Clamped field is a variant of <see cref="ClampedValue{T}"/> which ignores the json serialization of min/max values.<br/>
/// This is particularly useful when setting boundaries within the defaults in an instance of a file such a preferences where you do not want the min/max to be modified by the user.
/// </summary>
/// <remarks>
/// ClampedField is volatile in the serialization process because it requires:<br/>
/// - To be nested within another object<br/>
/// - Must have a default definition<br/><br/>
/// If you are only serializing the value not nested within an object you must use <see cref="ClampedValue{T}"/> instead since it will have no fallback default definition.
/// </remarks>
/// <example>
/// A use case of clamped field, where the default definition should control the min/max boundaries when serializing.
/// This functions because the JsonConvert deserializer will use the default definition.
/// <code>
/// [Serializable]
/// public class GamePreferences
/// {
///		public ClampedField&lt;float&gt; sensitivity = new ClampedField&lt;float&gt;(2, 1, 8);
/// }
/// </code>
/// </example>
/// <typeparam name="T">Type for the value you want to keep keep track of and limit within the boundaries</typeparam>
public class ClampedField<T> : ClampedValue<T> where T : IComparable
{
	[JsonIgnore]
	public override T Min { get => currentMin; set { currentMin = value; Validate(); } }
	
	[JsonIgnore]
	public override T Max { get => currentMax; set { currentMax = value; Validate(); } }

	/// <summary>
	/// Create a definition for a value that is always kept between a <paramref name="min"/> and <paramref name="max"/> value.
	/// </summary>
	/// <remarks>
	/// There is <b>never</b> any check the <paramref name="min"/> is not greater than <paramref name="max"/> so it is your responsibility to validate the bounds.<br/>
	/// If the <paramref name="min"/> does happen to be greater than <paramref name="max"/>: <see cref="ClampedValue&lt;T&gt;.Value"/> will always be equal to <paramref name="min"/>.
	/// </remarks>
	/// <param name="initialValue">The starting value</param>
	/// <param name="min">The lower bound for the value</param>
	/// <param name="max">The upper bound for the value</param>
	public ClampedField(T initialValue, T min, T max) : base(initialValue, min, max)
	{
		
	}
}