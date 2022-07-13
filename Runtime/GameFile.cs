using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Generic class that can quickly read/write files to the player's <see cref="Application.persistentDataPath"/>.
/// </summary>
/// <typeparam name="T">The serializable data that the file represents.</typeparam>
public class GameFile<T>
{
	private readonly string dataPath;
	private readonly string jsonPath;
	private readonly FileType fileType;

	/// <summary>
	/// Determines if <see cref="Data"/> has been written to or loaded.
	/// </summary>
	/// <remarks>
	/// This is relevant for the <see cref="WriteFile"/> function: which will refuse to save if the data has not been validated yet.
	/// This value can be set publicly so it is your responsibility to let the GameFile know your data is has been initialized.
	/// </remarks>
	public bool ValidData { get; set; }
	
	/// <summary>
	/// The data which is to be read/written to the file. 
	/// </summary>
	/// <remarks>
	/// Ensure that <see cref="T"/> is serializable or else the data may not save as expected.
	/// </remarks>
	public T Data { get; set; }

	/// <summary>
	/// Create an instance of the game file that can be accessed by other classes.
	/// </summary>
	/// <param name="fileName">The name of the file on the disc. Do not include any extensions in this name.</param>
	/// <param name="fileType">The type of file this is saved as to the disk.</param>
	public GameFile(string fileName, FileType fileType = FileType.Encrypted)
	{
		dataPath = $"{Application.persistentDataPath}/{fileName}.dat";
		jsonPath = $"{Application.persistentDataPath}/{fileName}.json";
		this.fileType = fileType;
	}

	#region Write File Functionality
	/// <summary>
	/// Write the current <see cref="Data"/> to the system storage. 
	/// </summary>
	/// <remarks>
	/// Will reject the attempt if <see cref="ValidData"/> is not true. Consider validating the data before attempting to write it.
	/// </remarks>
	/// <returns>True if the write attempt was successful, false if it was not.</returns>
	public bool WriteFile()
	{
		if (!ValidData)
		{
#if UNITY_EDITOR
			Debug.LogWarning("There was an attempt to write a file but its data was never validated. Avoiding save to prevent data loss.");
#endif
			return false;
		}

		switch (fileType)
		{
			case FileType.Encrypted:
				WriteFileAsBytes();
				break;
			case FileType.RawJson:
				WriteFileAsJson();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		return true;
	}

	private void WriteFileAsBytes()
	{
		string json = JsonConvert.SerializeObject(Data, Formatting.None);
		File.WriteAllBytes(dataPath, Encrypt(json));
	}

	private void WriteFileAsJson()
	{
		string jsonData = JsonConvert.SerializeObject(Data, Formatting.Indented);
		File.WriteAllText(jsonPath, jsonData);
	}
	#endregion

	#region Read File Functionality
	/// <summary>
	/// Load the file from system storage and set its value to <see cref="Data"/>.
	/// </summary>
	/// <remarks>
	/// This function will automatically set <see cref="ValidData"/> to true if it was successful.
	/// </remarks>
	/// <returns>True if the data was loaded, false if it was not (such as when the file does not exist).</returns>
	public bool ReadFile()
	{
		switch (fileType)
		{
			case FileType.Encrypted:
				return ReadFileFromBytes();
			case FileType.RawJson:
				return ReadFileFromJson();
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private bool ReadFileFromBytes()
	{
		if (!File.Exists(dataPath))
		{
			return false;
		}

		byte[] byteData = File.ReadAllBytes(dataPath);
		string jsonData = Decrypt(byteData);
		Data = JsonConvert.DeserializeObject<T>(jsonData);
		ValidData = true;
		return true;
	}

	private bool ReadFileFromJson()
	{
		if (!File.Exists(jsonPath))
		{
			return false;
		}

		string fileText = File.ReadAllText(jsonPath);
		Data = JsonConvert.DeserializeObject<T>(fileText);
		ValidData = true;
		return true;
	}
	#endregion

	#region Delete File Functionality
	/// <summary>
	/// Delete the file on the disk.
	/// </summary>
	/// <remarks>
	/// This function does not reset the value of <see cref="Data"/>.
	/// </remarks>
	/// <returns>True if it existed and has now been deleted, false if it was not present.</returns>
	public bool DeleteFile()
	{
		switch (fileType)
		{
			case FileType.Encrypted:
				return DeleteBytesFile();
			case FileType.RawJson:
				return DeleteJsonFile();
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private bool DeleteBytesFile()
	{
		if (!File.Exists(dataPath))
		{
			return false;
		}

		File.Delete(dataPath);
		return true;
	}

	private bool DeleteJsonFile()
	{
		if (!File.Exists(jsonPath))
		{
			return false;
		}

		File.Delete(jsonPath);
		return true;
	}
	#endregion

	#region Migration Functions
	/// <summary>
	/// Migrate the file at <see cref="jsonPath"/> to the <see cref="dataPath"/> thus encrypting it.
	/// </summary>
	/// <returns>True if the json file exists and was migrated properly, false if it was not able to be migrated.</returns>
	public bool ImportJsonToBytes()
	{
		if (!File.Exists(jsonPath))
		{
			return false;
		}

		string fileText = File.ReadAllText(jsonPath);
		T objectData = JsonConvert.DeserializeObject<T>(fileText);
		string jsonData = JsonConvert.SerializeObject(objectData, Formatting.None);
		File.WriteAllBytes(dataPath, Encrypt(jsonData));
		return true;
	}

	/// <summary>
	/// Migrate the file at <see cref="dataPath"/> to the <see cref="jsonPath"/> thus decrypting it.
	/// </summary>
	/// <returns>True if the data file exists and was migrated properly, false if it was not able to be migrated.</returns>
	public bool ExportBytesToJson()
	{
		if (!File.Exists(dataPath))
		{
			return false;
		}
		
		string fileText = Decrypt(File.ReadAllBytes(dataPath));
		T objectData = JsonConvert.DeserializeObject<T>(fileText);
		string jsonData = JsonConvert.SerializeObject(objectData, Formatting.Indented);
		File.WriteAllText(jsonPath, jsonData);
		return true;
	}
	#endregion
	
	#region Encryption
	// IMPORTANT: These encryption keys are obviously public due to the nature of this package being open source and naively having them be constant.
	// Please consider a more secure solution if you data is actually important.
	// The main purpose of this encryption then is to not make the data completely inaccessible, but rather to make it a little more obscure for the average user to modify their game data.
	// In other words: the alternative here is to store game data in plain text json so at least it is stored as a slightly more secure file.
	private const string Key = "n7cRY4o5XFbjs44hL68AzDA4hGjAJRhJ";
	private static readonly byte[] KeyBytes = Encoding.UTF8.GetBytes(Key);
	private const string InitialValue = "RrbEcXchnNDt4d5r";
	private static readonly byte[] InitialValueBytes = Encoding.UTF8.GetBytes(InitialValue);

	private static byte[] Encrypt(string message)
	{
		using AesManaged aes = new AesManaged();
		aes.Key = KeyBytes;
		aes.IV = InitialValueBytes;
		aes.Padding = PaddingMode.PKCS7;
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(KeyBytes, InitialValueBytes), CryptoStreamMode.Write);
		using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
		{
			streamWriter.Write(message);
		}
		byte[] result = memoryStream.ToArray();
		return result;
	}

	private static string Decrypt(byte[] message)
	{
		using AesManaged aes = new AesManaged();
		aes.Key = KeyBytes;
		aes.IV = InitialValueBytes;
		aes.Padding = PaddingMode.PKCS7;
		using MemoryStream memoryStream = new MemoryStream(message);
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(KeyBytes, InitialValueBytes), CryptoStreamMode.Read);
		using StreamReader streamReader = new StreamReader(cryptoStream);
		string result = streamReader.ReadToEnd();
		return result;
	}
	#endregion

	public enum FileType
	{
		/// <summary>
		/// Save the file as an exposed and unencrypted JSON file.
		/// </summary>
		RawJson,
		/// <summary>
		/// Save the file as an encrypted data file.
		/// </summary>
		/// <remarks>
		/// <b>WARNING:</b> Files using this encryption are <b>NOT</b> necessarily secure and can potentially be opened by the end user if they were to decompile the code or look at the open source package for the encryption keys!
		/// </remarks>
		Encrypted
	}
}