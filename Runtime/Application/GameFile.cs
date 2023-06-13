using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LMirman.Utilities
{
	/// <summary>
	/// Generic class that can quickly read/write files to the player's <see cref="Application.persistentDataPath"/>.
	/// </summary>
	/// <typeparam name="T">The serializable data that the file represents.</typeparam>
	[PublicAPI]
	public class GameFile<T>
	{
		public event Action<GameFile<T>> OnFileWritten = delegate { };
		public event Action<GameFile<T>> OnFileRead = delegate { };
		public event Action<GameFile<T>> OnFileDeleted = delegate { };

		/// <summary>
		/// The directory of this file relative to <see cref="Application.persistentDataPath"/>.
		/// </summary>
		/// <remarks>
		/// Does not include the file name.<br/>
		/// Ends with a `\`.
		/// </remarks>
		public readonly string localFileDirectory;
		/// <summary>
		/// The absolute file directory for this game file.
		/// </summary>
		public readonly string fileDirectory;
		/// <summary>
		/// The name of the game file.
		/// </summary>
		public readonly string fileName;
		/// <summary>
		/// Absolute path that the game file will save to as an encrypted file.
		/// </summary>
		public readonly string dataPath;
		/// <summary>
		/// Local path within <see cref="Application.persistentDataPath"/> that the game file will save to as an encrypted file.
		/// </summary>
		public readonly string localDataPath;
		/// <summary>
		/// Absolute path that the game file will save to as a json file.
		/// </summary>
		public readonly string jsonPath;
		/// <summary>
		/// Local path within <see cref="Application.persistentDataPath"/> that the game file will save to as a json file.
		/// </summary>
		public readonly string localJsonPath;
		/// <summary>
		/// The encryptor being used to encrypt json file to data file.
		/// </summary>
		public readonly Encryption.Encryptor encryptor;
		/// <summary>
		/// The preferred file type to save this as to disk.
		/// </summary>
		public readonly FileType fileType;

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
		/// The path this file will save to depending on the value of <see cref="fileType"/>.
		/// </summary>
		public string Path => fileType == FileType.Encrypted ? dataPath : jsonPath;
		public string LocalPath => fileType == FileType.Encrypted ? localDataPath : localJsonPath;

		#region Last Sync Times
		/// <summary>
		/// The <see cref="Time.time"/> at which the loaded data was last in sync with system file (usually the last load/write action).
		/// </summary>
		public float LastSyncEngineTime { get; set; }

		/// <summary>
		/// The <see cref="Time.realtimeSinceStartup"/> at which the loaded data was last in sync with system file (usually the last load/write action).
		/// </summary>
		public float LastSyncEngineRealtime { get; set; }

		/// <summary>
		/// The <see cref="DateTime.UtcNow"/> Ticks value at which the loaded data was last in sync with system file (usually the last load/write action).
		/// </summary>
		public long LastSyncSystemTime { get; set; }
		#endregion

		/// <summary>
		/// Create an instance of the game file that can be accessed by other classes.
		/// </summary>
		/// <param name="localPathToFile">The local directory of the file on the disc. The final part of the path is the name of the file. Do not include any extensions in this name. Examples: `save1`, `saves/save1`, `files/data/preferences`</param>
		/// <param name="encryptor">The method of encryption for the file. Consider using <see cref="Encryption.DefaultEncryptor"/> if your file does not need to be secure.</param>
		/// <param name="fileType">The type of file this is saved as to the disk.</param>
		public GameFile(string localPathToFile, Encryption.Encryptor encryptor, FileType fileType = FileType.Encrypted)
		{
			localPathToFile = localPathToFile.Replace('\\', '/').TrimStart('/').TrimEnd('/');
			int positionOfLastSlash = localPathToFile.LastIndexOf('/');
			localFileDirectory = "/" + (positionOfLastSlash < 0 ? string.Empty : localPathToFile.Substring(0, positionOfLastSlash + 1));
			fileName = localPathToFile.Substring(positionOfLastSlash + 1);
			fileDirectory = $"{Application.persistentDataPath}{localFileDirectory}";
			localDataPath = $"{localFileDirectory.TrimStart('/')}{fileName}.dat";
			localJsonPath = $"{localFileDirectory.TrimStart('/')}{fileName}.json";
			dataPath = $"{fileDirectory}{fileName}.dat";
			jsonPath = $"{fileDirectory}{fileName}.json";
			this.encryptor = encryptor;
			this.fileType = fileType;
		}

		#region Get Functions
		public byte[] GetDataAsByteArray()
		{
			switch (fileType)
			{
				case FileType.Encrypted:
					return GetDataAsEncryptedByteArray();
				case FileType.RawJson:
					return GetDataAsJsonByteArray();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public byte[] GetDataAsByteArray(T data)
		{
			switch (fileType)
			{
				case FileType.Encrypted:
					return GetDataAsEncryptedByteArray(data);
				case FileType.RawJson:
					return GetDataAsJsonByteArray(data);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public byte[] GetDataAsEncryptedByteArray()
		{
			return GetDataAsEncryptedByteArray(Data);
		}

		public byte[] GetDataAsEncryptedByteArray(T data)
		{
			string json = JsonConvert.SerializeObject(data, Formatting.None);
			return encryptor.Encrypt(json);
		}

		public byte[] GetDataAsJsonByteArray()
		{
			return GetDataAsJsonByteArray(Data);
		}

		public byte[] GetDataAsJsonByteArray(T data)
		{
			string json = JsonConvert.SerializeObject(data, Formatting.None);
			return Encoding.UTF8.GetBytes(json);
		}

		public T GetDataFromByteArray(byte[] bytes)
		{
			switch (fileType)
			{
				case FileType.Encrypted:
					return GetDataFromEncryptedByteArray(bytes);
				case FileType.RawJson:
					return GetDataFromJsonByteArray(bytes);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public T GetDataFromEncryptedByteArray(byte[] bytes)
		{
			string jsonData = encryptor.Decrypt(bytes);
			return JsonConvert.DeserializeObject<T>(jsonData);
		}

		public T GetDataFromJsonByteArray(byte[] bytes)
		{
			string jsonData = Encoding.UTF8.GetString(bytes);
			return JsonConvert.DeserializeObject<T>(jsonData);
		}
		#endregion

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
			Directory.CreateDirectory(fileDirectory);
			File.WriteAllBytes(dataPath, GetDataAsEncryptedByteArray());
			OnFileWritten.Invoke(this);
			UpdateFileSyncTime();
		}

		private void WriteFileAsJson()
		{
			Directory.CreateDirectory(fileDirectory);
			File.WriteAllBytes(jsonPath, GetDataAsJsonByteArray());
			OnFileWritten.Invoke(this);
			UpdateFileSyncTime();
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
				UpdateFileSyncTime();
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
				UpdateFileSyncTime();
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion

		#region Load Data Functionality
		public void LoadData(byte[] bytes)
		{
			switch (fileType)
			{
				case FileType.Encrypted:
					LoadDataFromBytes(bytes);
					break;
				case FileType.RawJson:
					LoadDataFromJson(bytes);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void LoadDataFromBytes(byte[] bytes)
		{
			Data = GetDataFromEncryptedByteArray(bytes);
			ValidData = true;
			OnFileRead.Invoke(this);
			UpdateFileSyncTime();
		}

		private void LoadDataFromJson(byte[] bytes)
		{
			Data = GetDataFromJsonByteArray(bytes);
			ValidData = true;
			OnFileRead.Invoke(this);
			UpdateFileSyncTime();
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
			data = GetDataFromEncryptedByteArray(byteData);
			return true;
		}

		private bool PeekFileFromJson(out T data)
		{
			if (!File.Exists(jsonPath))
			{
				data = default;
				return false;
			}

			byte[] byteData = File.ReadAllBytes(jsonPath);
			data = GetDataFromJsonByteArray(byteData);
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

		/// <summary>
		/// Updates the sync state variables to the current time.
		/// Manually invoke this if the loaded data is in sync with the system file.
		/// </summary>
		public void UpdateFileSyncTime()
		{
			LastSyncEngineTime = Time.time;
			LastSyncEngineRealtime = Time.realtimeSinceStartup;
			LastSyncSystemTime = DateTime.UtcNow.Ticks;
		}

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
}