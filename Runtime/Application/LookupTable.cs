using LMirman.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// An asset based dictionary that pairs a string key with an arbitrary <see cref="T"/> object.
	/// </summary>
	/// <typeparam name="T">The object type to use for the values in the lookup table.</typeparam>
	public class LookupTable<T> where T : ILookupCollectionEntry
	{
		private readonly string resourcePath;
	
		/// <summary>
		/// The lookup table asset in the project.
		/// </summary>
		/// <remarks>Avoid utilizing this at runtime! The main purpose of this being public to enable you to write editor scripts to modify the asset itself.</remarks>
		public LookupCollectionAsset<T> Asset { get; private set; }
		public Dictionary<string, T> Lookup { get; } = new Dictionary<string, T>();
		public List<T> List { get; } = new List<T>();
		public T DefaultValue { get; private set; }

		/// <summary>
		/// Create an instance of a lookup table based on a particular lookup table asset.
		/// </summary>
		/// <param name="resourcePath">The path to use when loading the asset from resources using <see cref="Resources.Load(string)"/></param>
		/// <param name="immediateReload">Should the constructor automatically call <see cref="ReloadResource"/>?</param>
		public LookupTable(string resourcePath, bool immediateReload = false)
		{
			this.resourcePath = resourcePath;

			if (immediateReload)
			{
				ReloadResource();
			}
		}

		/// <summary>
		/// Reload the <see cref="Asset"/> from the resources folder, resetting the entries in the <see cref="Lookup"/> and <see cref="List"/> collections.
		/// </summary>
		public void ReloadResource()
		{
			Lookup.Clear();
			List.Clear();

			Asset = Resources.Load<LookupCollectionAsset<T>>(resourcePath);
			if (Asset != null)
			{
				foreach (T entry in Asset.Entries)
				{
					if (Lookup.ContainsKey(entry.Key))
					{
						Debug.LogError($"Duplicate key found with value {entry.Key}");
						continue;
					}
				
					Lookup.Add(entry.Key, entry);
					List.Add(entry);
				}

				DefaultValue = Asset.DefaultValue;
			}
			else
			{
				Debug.LogError($"No lookup table found at: {resourcePath}");
			}
		}

		/// <summary>
		/// Check if a key is matches the recommended guidelines for key syntax.
		/// </summary>
		/// <param name="key">The key to verify the syntax of</param>
		/// <param name="message">A message indicating any syntax problems that were encountered.</param>
		/// <returns>True if the key was validated with no errors found, false if the key has a syntax problem.</returns>
		public static bool ValidateKey(string key, out string message)
		{
			message = string.Empty;
			if (string.IsNullOrWhiteSpace(key))
			{
				message = "The item key should not be null or empty.";
				return false;
			}

			bool value = true;
			if (key.Contains(' '))
			{
				message += "The item key should not contain any spaces.\n";
				value = false;
			}

			if (key.Any(char.IsUpper))
			{
				message += "The item key should not have any capital letters in it.\n";
				value = false;
			}

			if (key.Any(letter => !char.IsLetterOrDigit(letter) && letter != '_' && letter != '-'))
			{
				message += "The item key should not contain any non-alphanumeric characters.\n";
				value = false;
			}

			message = message.TrimEnd('\n');
			return value;
		}
	}
}

public abstract class LookupCollectionAsset<T> : ScriptableObject where T : ILookupCollectionEntry
{
	public abstract IEnumerable<T> Entries { get; }

	public abstract void InsertEntry(T entry);

	public virtual T DefaultValue => default;
}

public interface ILookupCollectionEntry
{
	/// <summary>
	/// The key value for the entry in the <see cref="LookupTable{T}"/>.
	/// </summary>
	/// <remarks>
	/// It is encouraged that your keys follow the following rules, although for most not necessarily required for the lookup table to function.<br/>
	///  - The key should not be null or white space.<br/>
	///  - The key should not be shared by any other object within the lookup table.<br/>
	///  - The key should not contain any spaces.<br/>
	///  - The key should only use lowercase alphanumeric characters. (0-9, a-z)<br/>
	///  - The key should not include any accented or exotic ascii characters.<br/>
	/// </remarks>
	public string Key { get; }
}

[Serializable]
public class GenericLookupEntry<T> : ILookupCollectionEntry
{
	[SerializeField]
	private string key;
	[SerializeField]
	private T value;
	
	public string Key => key;
	public T Value => value;
}