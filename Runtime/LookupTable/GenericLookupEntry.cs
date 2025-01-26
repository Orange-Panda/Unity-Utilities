using JetBrains.Annotations;
using System;
using UnityEngine;

namespace LMirman.Utilities.LookupTable
{
	[PublicAPI]
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
}