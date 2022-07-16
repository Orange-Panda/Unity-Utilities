using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Encryption
{
	/// <summary>
	/// IMPORTANT: The default encryption keys are obviously public due to the nature of this package being open source.
	/// Please consider a more secure Encryptor if your data is actually important.
	/// The main purpose of this encryption then is to not make the data completely inaccessible, but rather to make it a little more obscure for the average user to modify their game data.
	/// In other words: the alternative here is to store game data in plain text json so at least it is stored as a slightly more secure file that <i>most</i> users won't modify.
	/// </summary>
	private const string DefaultKey = "n7cRY4o5XFbjs44hL68AzDA4hGjAJRhJ";
	private const string DefaultInitialValue = "RrbEcXchnNDt4d5r";

	public static readonly Encryptor DefaultEncryptor = new AesEncryptor(DefaultKey, DefaultInitialValue);

	public abstract class Encryptor
	{
		public abstract byte[] Encrypt(string message);
		public abstract string Decrypt(byte[] message);
	}
	
	public class AesEncryptor : Encryptor
	{
		private readonly byte[] key;
		private readonly byte[] initialValue;
		
		public AesEncryptor(string key, string initialValue)
		{
			this.key = Encoding.UTF8.GetBytes(key);
			this.initialValue = Encoding.UTF8.GetBytes(initialValue);
		}
		
		public override byte[] Encrypt(string message)
		{
			using AesManaged aes = new AesManaged();
			aes.Key = key;
			aes.IV = initialValue;
			aes.Padding = PaddingMode.PKCS7;
			using MemoryStream memoryStream = new MemoryStream();
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(key, initialValue), CryptoStreamMode.Write);
			using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
			{
				streamWriter.Write(message);
			}
			byte[] result = memoryStream.ToArray();
			return result;
		}

		public override string Decrypt(byte[] message)
		{
			using AesManaged aes = new AesManaged();
			aes.Key = key;
			aes.IV = initialValue;
			aes.Padding = PaddingMode.PKCS7;
			using MemoryStream memoryStream = new MemoryStream(message);
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(key, initialValue), CryptoStreamMode.Read);
			using StreamReader streamReader = new StreamReader(cryptoStream);
			string result = streamReader.ReadToEnd();
			return result;
		}
	}
}