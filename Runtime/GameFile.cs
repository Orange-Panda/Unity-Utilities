using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

/// <summary>
/// Generic class that can quickly read/write files to the player's <see cref="Application.persistentDataPath"/>.
/// </summary>
/// <typeparam name="T">The serializable data that the file represents.</typeparam>
public class GameFile<T>
{
	public event Action<GameFile<T>> OnFileWritten = delegate { };
	public event Action<GameFile<T>> OnFileRead = delegate { };
	public event Action<GameFile<T>> OnFileDeleted = delegate { };

	private readonly string dataPath;
	private readonly string jsonPath;
	private readonly Encryption.Encryptor encryptor;
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
	/// <param name="encryptor">The method of encryption for the file. Consider using <see cref="Encryption.DefaultEncryptor"/> if your file does not need to be secure.</param>
	/// <param name="fileType">The type of file this is saved as to the disk.</param>
	public GameFile(string fileName, Encryption.Encryptor encryptor, FileType fileType = FileType.Encrypted)
	{
		dataPath = $"{Application.persistentDataPath}/{fileName}.dat";
		jsonPath = $"{Application.persistentDataPath}/{fileName}.json";
		this.encryptor = encryptor;
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
		File.WriteAllBytes(dataPath, encryptor.Encrypt(json));
		OnFileWritten.Invoke(this);
	}

	private void WriteFileAsJson()
	{
		string jsonData = JsonConvert.SerializeObject(Data, Formatting.Indented);
		File.WriteAllText(jsonPath, jsonData);
		OnFileWritten.Invoke(this);
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
		if (PeekFileFromBytes(out T data))
		{
			Data = data;
			ValidData = true;
			OnFileRead.Invoke(this);
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool ReadFileFromJson()
	{
		if (PeekFileFromJson(out T data))
		{
			Data = data;
			ValidData = true;
			OnFileRead.Invoke(this);
			return true;
		}
		else
		{
			return false;
		}
	}
	#endregion
	
	#region Peek File Functionality
	public bool PeekFile(out T data)
	{
		switch (fileType)
		{
			case FileType.Encrypted:
				return PeekFileFromBytes(out data);
			case FileType.RawJson:
				return PeekFileFromJson(out data);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private bool PeekFileFromBytes(out T data)
	{
		if (!File.Exists(dataPath))
		{
			data = default;
			return false;
		}

		byte[] byteData = File.ReadAllBytes(dataPath);
		string jsonData = encryptor.Decrypt(byteData);
		data = JsonConvert.DeserializeObject<T>(jsonData);
		return true;
	}

	private bool PeekFileFromJson(out T data)
	{
		if (!File.Exists(jsonPath))
		{
			data = default;
			return false;
		}

		string fileText = File.ReadAllText(jsonPath);
		data = JsonConvert.DeserializeObject<T>(fileText);
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
		OnFileDeleted.Invoke(this);
		return true;
	}

	private bool DeleteJsonFile()
	{
		if (!File.Exists(jsonPath))
		{
			return false;
		}

		File.Delete(jsonPath);
		OnFileDeleted.Invoke(this);
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
		File.WriteAllBytes(dataPath, encryptor.Encrypt(jsonData));
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
		
		string fileText = encryptor.Decrypt(File.ReadAllBytes(dataPath));
		T objectData = JsonConvert.DeserializeObject<T>(fileText);
		string jsonData = JsonConvert.SerializeObject(objectData, Formatting.Indented);
		File.WriteAllText(jsonPath, jsonData);
		return true;
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