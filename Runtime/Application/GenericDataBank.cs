using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// A generic data bank that can store and retrieve values of any type at runtime.
	/// </summary>
	/// <remarks>
	/// This is useful for storing data values in cases where the setter does not want to explicitly point to a specific variable somewhere.
	/// In such a case: setters only needs access to the bank where they can store and retrieve their values rather than needing to create new variables for everything.  
	/// </remarks>
	[Serializable]
	public class GenericDataBank
	{
		[JsonProperty]
		private Dictionary<string, object> data = new Dictionary<string, object>();

		/// <summary>
		/// Checks if a value is present for a particular key.
		/// </summary>
		/// <param name="key">The key to check the presence of</param>
		/// <returns>True if a value for the key exists, otherwise returns false.</returns>
		public bool HasKey(string key)
		{
			key = ValidateKey(key);
			return data.ContainsKey(key);
		}

		/// <summary>
		/// Set a struct data structure to the data bank.
		/// </summary>
		/// <typeparam name="T">The type of struct to write the data as.</typeparam>
		/// <param name="key">The exact key used to store the data in the data bank.</param>
		/// <param name="value">The value to write the data as.</param>
		public void SetStruct<T>(string key, T value) where T : struct
		{
			SetObject(key, value);
		}

		/// <summary>
		/// Set a class data structure to the data bank.
		/// </summary>
		/// <typeparam name="T">The type of class to write the data as.</typeparam>
		/// <param name="key">The exact key used to store the data in the data bank.</param>
		/// <param name="value">The value to write the data as.</param>
		public void SetClass<T>(string key, T value) where T : class
		{
			object objectValue = JsonConvert.SerializeObject(value);
			SetObject(key, objectValue);
		}

		/// <summary>
		/// For internal usage. Actually set the data to the <see cref="data"/> bank.
		/// </summary>
		/// <param name="key">The exact key used to store the data in the data bank.</param>
		/// <param name="value">The value to write the data as.</param>
		private void SetObject(string key, object value)
		{
			key = ValidateKey(key);

			if (data.ContainsKey(key))
			{
				data[key] = value;
			}
			else
			{
				data.Add(key, value);
			}
		}

		/// <summary>
		/// Attempts to get a struct data structure from the data bank.
		/// </summary>
		/// <typeparam name="T">The type of struct to output the data as.</typeparam>
		/// <param name="key">The exact key used to identify the data in the data bank.</param>
		/// <param name="value">The output of the retrieval. When successful will be the value deserialized from the data bank. When unsuccessful will be the default value of <typeparamref name="T"/>.</param>
		/// <returns>True if the struct was present in the save file and was deserialized. Otherwise, false.</returns>
		public bool TryGetStruct<T>(string key, out T value) where T : struct
		{
			key = ValidateKey(key);

			try
			{
				if (!data.TryGetValue(key, out object dataValue))
				{
					value = default;
					return false;
				}
				else if (dataValue is T output)
				{
					value = output;
					return true;
				}
				else
				{
					value = (T)Convert.ChangeType(dataValue, typeof(T));
					return true;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Internal error occurred getting value, using default instead.\n{e}");
				value = default;
				return false;
			}
		}

		/// <summary>
		/// Get a struct data structure from the data bank. If it was not present, <paramref name="defaultValue"/> is used instead.
		/// </summary>
		/// <param name="key">The exact key used to identify the data in the data bank.</param>
		/// <param name="defaultValue">The value to return if there was not a value defined at <see cref="key"/>.</param>
		/// <typeparam name="T">The type of struct to output the data as.</typeparam>
		/// <returns>The struct value in the data bank if it was present otherwise returns the <paramref name="defaultValue"/></returns>
		public T GetStruct<T>(string key, T defaultValue = default) where T : struct
		{
			if (TryGetStruct(key, out T output))
			{
				return output;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Attempts to get a class data structure from the data bank.
		/// </summary>
		/// <typeparam name="T">The type of class to output the data as.</typeparam>
		/// <param name="key">The exact key used to store the data in the data bank.</param>
		/// <param name="value">The output of the retrieval. When successful will be the value deserialized from the data bank. When unsuccessful will be the default value of <typeparamref name="T"/>.</param>
		/// <returns>True if the class was present in the data bank and was deserialized. Otherwise, false.</returns>
		public bool TryGetClass<T>(string key, out T value) where T : class
		{
			key = ValidateKey(key);

			try
			{
				if (data.TryGetValue(key, out object dataValue))
				{
					value = JsonConvert.DeserializeObject<T>((string)dataValue);
					return true;
				}
				else
				{
					value = default;
					return false;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Internal error occurred getting value, using default instead.\n{e}");
				value = default;
				return false;
			}
		}

		/// <summary>
		/// Get a class data structure from the data bank. If it was not present, <paramref name="defaultValue"/> is used instead.
		/// </summary>
		/// <param name="key">The exact key used to identify the data in the data bank.</param>
		/// <param name="defaultValue">The value to return if there was not a value defined at <see cref="key"/>.</param>
		/// <typeparam name="T">The type of class to output the data as.</typeparam>
		/// <returns>The class value in the data bank if it was present otherwise returns the <paramref name="defaultValue"/></returns>
		public T GetClass<T>(string key, T defaultValue = default) where T : class
		{
			if (TryGetClass(key, out T output))
			{
				return output;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Attempts to get any data structure from the data bank without type restrictions.
		/// </summary>
		/// <remarks>
		/// You are encouraged to use <see cref="TryGetClass{T}"/> or <see cref="TryGetStruct{T}"/> if you know your type ahead of time.
		/// However, this method is useful in specific cases where the input data type is unconstrained.
		/// </remarks>
		/// <typeparam name="T">The type to output the data as.</typeparam>
		/// <param name="key">The exact key used to store the data in the data bank.</param>
		/// <param name="value">The output of the retrieval. When successful will be the value deserialized from the data bank. When unsuccessful will be the default value of <typeparamref name="T"/>.</param>
		/// <returns>True if the value was present in the data bank and was deserialized. Otherwise, false.</returns>
		public bool TryGetValue<T>(string key, out T value)
		{
			key = ValidateKey(key);

			try
			{
				if (!data.TryGetValue(key, out object dataValue))
				{
					value = default;
					return false;
				}
				else if (dataValue is string json)
				{
					// Do not try to parse the request type in the specific case of the request type being of IConvertible
					if (typeof(T) == typeof(IConvertible))
					{
						IConvertible convertibleValue = JsonConvert.DeserializeObject<string>(json);
						value = (T)convertibleValue;
						return true;
					}
					
					value = JsonConvert.DeserializeObject<T>(json);
					return true;
				}
				else if (dataValue is T output)
				{
					value = output;
					return true;
				}
				else
				{
					value = (T)Convert.ChangeType(dataValue, typeof(T));
					return true;
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"Unable to get value, using default instead.\n{e}");
				value = default;
				return false;
			}
		}

		/// <summary>
		/// Get any data structure from the data bank. If it was not present, <paramref name="defaultValue"/> is used instead.
		/// </summary>
		/// <remarks>
		/// You are encouraged to use <see cref="GetClass{T}"/> or <see cref="GetStruct{T}"/> if you know your type ahead of time.
		/// However, this method is useful in specific cases where the input data type is unconstrained.
		/// </remarks>
		/// <param name="key">The exact key used to identify the data in the data bank.</param>
		/// <param name="defaultValue">The value to return if there was not a value defined at <see cref="key"/>.</param>
		/// <typeparam name="T">The type to output the data as.</typeparam>
		/// <returns>The value in the data bank if it was present otherwise returns the <paramref name="defaultValue"/></returns>
		public T GetValue<T>(string key, T defaultValue)
		{
			if (TryGetValue(key, out T output))
			{
				return output;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Remove a particular entry from the data bank via its <paramref name="key"/>
		/// </summary>
		/// <param name="key">The exact key used to store the data in the data bank.</param>
		/// <returns>True if there was an entry that has been removed, false otherwise.</returns>
		public bool RemoveEntry(string key)
		{
			key = ValidateKey(key);
			if (data.ContainsKey(key))
			{
				data.Remove(key);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Remove <b>ALL</b> values from the data bank.
		/// </summary>
		public void DeleteAllEntries()
		{
			data.Clear();
		}

		/// <summary>
		/// For internal usage. Cleans the key provided so it is usable.
		/// </summary>
		/// <param name="key">The key to validate</param>
		/// <returns>A usable string that will not throw errors.</returns>
		private static string ValidateKey(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				key = string.Empty;
			}

			return key;
		}
	}
}