using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities.LookupTable
{
	[PublicAPI]
	public abstract class LookupCollectionAsset<T> : ScriptableObject where T : ILookupCollectionEntry
	{
		public abstract IEnumerable<T> Entries { get; }

		public abstract void InsertEntry(T entry);

		public virtual T DefaultValue => default;
	}
}