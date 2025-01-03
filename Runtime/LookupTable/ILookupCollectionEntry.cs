namespace LMirman.Utilities
{
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
}