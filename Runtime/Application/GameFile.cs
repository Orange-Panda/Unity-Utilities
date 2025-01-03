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
		/// <summary>
		/// Invoked at the end of <see cref="WriteFileAsBytes"/> and <see cref="WriteFileAsJson"/> methods.
		/// </summary>
		public event Action<GameFile<T>> FileWritten = delegate { };

		/// <summary>
		/// Invoked at the end of <see cref="LoadDataFromBytes"/>, <see cref="LoadDataFromJson"/>, <see cref="ReadFileFromBytes"/>, and <see cref="ReadFileFromJson"/> methods.
		/// </summary>
		public event Action<GameFile<T>> FileRead = delegate { };

		/// <summary>
		/// Invoked at the end of <see cref="DeleteBytesFile"/> and <see cref="DeleteJsonFile"/> methods.
		/// </summary>
		public event Action<GameFile<T>> FileDeleted = delegate { };

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
		public JsonSerializerSettings jsonSerializeSettings;
		public JsonSerializerSettings jsonDeserializeSettings;

		/// <summary>
		/// Determines if <see cref="Data"/> has been written to or loaded.
		/// </summary>
		/// <remarks>
		/// This is relevant for the <see cref="WriteFile()"/> function: which will refuse to save if the data has not been validated yet.
		/// This value can be set publicly so it is your responsibility to let the GameFile know your data has been initialized.
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
		/// The absolute path this file will save to depending on the value of <see cref="fileType"/>.
		/// </summary>
		/// <seealso cref="dataPath"/>
		/// <seealso cref="jsonPath"/>
		public string Path => fileType == FileType.Encrypted ? dataPath : jsonPath;

		/// <summary>
		/// The local path within <see cref="Application.persistentDataPath"/> this file will save to depending on the value of <see cref="fileType"/>.
		/// </summary>
		/// <seealso cref="localDataPath"/>
		/// <seealso cref="localJsonPath"/>
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
			jsonSerializeSettings = new JsonSerializerSettings { Error = HandleError };
			jsonDeserializeSettings = new JsonSerializerSettings { Error = HandleError };
		}

		#region Get Functions
		/// <returns>This file's <see cref="Data"/> object as a byte array depending on value of <see cref="fileType"/></returns>
		public byte[] GetDataAsByteArray()
		{
			return fileType switch
			{
				FileType.Encrypted => GetDataAsEncryptedByteArray(),
				FileType.RawJson => GetDataAsJsonByteArray(),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		/// <returns>The <paramref name="data"/> parameter as a byte array depending on value of <see cref="fileType"/></returns>
		public byte[] GetDataAsByteArray(T data)
		{
			return fileType switch
			{
				FileType.Encrypted => GetDataAsEncryptedByteArray(data),
				FileType.RawJson => GetDataAsJsonByteArray(data),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		/// <returns>This file's <see cref="Data"/> object as an encrypted byte array</returns>
		public byte[] GetDataAsEncryptedByteArray()
		{
			return GetDataAsEncryptedByteArray(Data);
		}

		/// <returns>The <paramref name="data"/> parameter as an encrypted byte array</returns>
		public byte[] GetDataAsEncryptedByteArray(T data)
		{
			string json = JsonConvert.SerializeObject(data, Formatting.None, jsonSerializeSettings);
			return encryptor.Encrypt(json);
		}

		/// <returns>This file's <see cref="Data"/> object as a json byte array</returns>
		public byte[] GetDataAsJsonByteArray()
		{
			return GetDataAsJsonByteArray(Data);
		}

		/// <returns>The <paramref name="data"/> parameter as a json byte array</returns>
		public byte[] GetDataAsJsonByteArray(T data)
		{
			string json = JsonConvert.SerializeObject(data, Formatting.None, jsonSerializeSettings);
			return Encoding.UTF8.GetBytes(json);
		}

		/// <summary>
		/// Convert a byte array into an object of type <typeparamref name="T"/> depending on <see cref="fileTypeValue"/> parameter.
		/// </summary>
		public T GetDataFromByteArray(byte[] bytes, FileType fileTypeValue)
		{
			return fileType switch
			{
				FileType.Encrypted => GetDataFromEncryptedByteArray(bytes),
				FileType.RawJson => GetDataFromJsonByteArray(bytes),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		/// <summary>
		/// Convert a byte array into an object of type <typeparamref name="T"/> depending on this file's <see cref="fileType"/> value.
		/// </summary>
		public T GetDataFromByteArray(byte[] bytes)
		{
			return fileType switch
			{
				FileType.Encrypted => GetDataFromEncryptedByteArray(bytes),
				FileType.RawJson => GetDataFromJsonByteArray(bytes),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		/// <summary>
		/// Convert an encrypted byte array into an object of type <typeparamref name="T"/>.
		/// </summary>
		public T GetDataFromEncryptedByteArray(byte[] bytes)
		{
			string jsonData = encryptor.Decrypt(bytes);
			return JsonConvert.DeserializeObject<T>(jsonData, jsonDeserializeSettings);
		}

		/// <summary>
		/// Convert a json byte array into an object of type <typeparamref name="T"/>.
		/// </summary>
		public T GetDataFromJsonByteArray(byte[] bytes)
		{
			string jsonData = Encoding.UTF8.GetString(bytes);
			return JsonConvert.DeserializeObject<T>(jsonData, jsonDeserializeSettings);
		}
		#endregion

		#region Write File Functionality
		/// <inheritdoc cref="WriteFile(FileType)"/>
		/// <summary>
		/// Write the current <see cref="Data"/> to the system storage in format of this file's <see cref="fileType"/>.
		/// </summary>
		public bool WriteFile()
		{
			return WriteFile(fileType);
		}

		/// <summary>
		/// Write the current <see cref="Data"/> to the system storage in specified format.
		/// </summary>
		/// <remarks>
		/// Will reject the attempt if <see cref="ValidData"/> is not true. Consider validating the data before attempting to write it.
		/// </remarks>
		/// <param name="fileTypeValue">The format to write the file as</param>
		/// <returns>True if the write attempt was successful, false if it was not.</returns>
		public bool WriteFile(FileType fileTypeValue)
		{
			if (!ValidData)
			{
#if UNITY_EDITOR
				Debug.LogWarning("There was an attempt to write a file but its data was never validated. Avoiding save to prevent data loss.");
#endif
				return false;
			}

			switch (fileTypeValue)
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
			UpdateFileSyncTime();
			FileWritten.Invoke(this);
		}

		private void WriteFileAsJson()
		{
			Directory.CreateDirectory(fileDirectory);
			File.WriteAllBytes(jsonPath, GetDataAsJsonByteArray());
			UpdateFileSyncTime();
			FileWritten.Invoke(this);
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
			return fileType switch
			{
				FileType.Encrypted => ReadFileFromBytes(),
				FileType.RawJson => ReadFileFromJson(),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private bool ReadFileFromBytes()
		{
			if (!PeekFileFromBytes(out T data))
			{
				return false;
			}

			Data = data;
			ValidData = true;
			UpdateFileSyncTime();
			FileRead.Invoke(this);
			return true;
		}

		private bool ReadFileFromJson()
		{
			if (!PeekFileFromJson(out T data))
			{
				return false;
			}

			Data = data;
			ValidData = true;
			UpdateFileSyncTime();
			FileRead.Invoke(this);
			return true;
		}
		#endregion

		#region Load Data Functionality
		/// <summary>
		/// Load a specific byte array into this file's <see cref="Data"/>.
		/// </summary>
		/// <remarks>
		/// Typically you should use the <see cref="ReadFile"/> method instead.
		/// </remarks>
		/// <seealso cref="ReadFile"/>
		/// <seealso cref="GetDataAsByteArray()"/>
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
			UpdateFileSyncTime();
			FileRead.Invoke(this);
		}

		private void LoadDataFromJson(byte[] bytes)
		{
			Data = GetDataFromJsonByteArray(bytes);
			ValidData = true;
			UpdateFileSyncTime();
			FileRead.Invoke(this);
		}
		#endregion

		#region Peek File Functionality
		/// <summary>
		/// Observe the contents of the game file on disk <b><i>without</i></b> loading it into <see cref="Data"/>.
		/// </summary>
		/// <remarks>
		/// Useful for displaying or comparing contents of the save file on disk without overwriting <see cref="Data"/>.
		/// </remarks>
		/// <param name="data">The contents of the file, if peek was successful.</param>
		/// <returns>True if the file exists and had its contents output to <paramref name="data"/>.</returns>
		public bool PeekFile(out T data)
		{
			return fileType switch
			{
				FileType.Encrypted => PeekFileFromBytes(out data),
				FileType.RawJson => PeekFileFromJson(out data),
				_ => throw new ArgumentOutOfRangeException()
			};
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
		/// This function does <b>not</b> reset the value of <see cref="Data"/>.
		/// </remarks>
		/// <returns>True if it existed and has now been deleted, false if it was not present.</returns>
		public bool DeleteFile()
		{
			return fileType switch
			{
				FileType.Encrypted => DeleteBytesFile(),
				FileType.RawJson => DeleteJsonFile(),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private bool DeleteBytesFile()
		{
			if (!File.Exists(dataPath))
			{
				return false;
			}

			File.Delete(dataPath);
			FileDeleted.Invoke(this);
			return true;
		}

		private bool DeleteJsonFile()
		{
			if (!File.Exists(jsonPath))
			{
				return false;
			}

			File.Delete(jsonPath);
			FileDeleted.Invoke(this);
			return true;
		}
		#endregion

		#region Migration Functions
		/// <summary>
		/// Migrate the file at <see cref="jsonPath"/> to the <see cref="dataPath"/> thus encrypting it.
		/// </summary>
		/// <returns>True if the json file exists and was migrated properly, false if it was not able to migrate.</returns>
		public bool ImportJsonToBytes()
		{
			if (!File.Exists(jsonPath))
			{
				return false;
			}

			string fileText = File.ReadAllText(jsonPath);
			T objectData = JsonConvert.DeserializeObject<T>(fileText, jsonDeserializeSettings);
			string jsonData = JsonConvert.SerializeObject(objectData, Formatting.None, jsonSerializeSettings);
			File.WriteAllBytes(dataPath, encryptor.Encrypt(jsonData));
			return true;
		}

		/// <summary>
		/// Migrate the file at <see cref="dataPath"/> to the <see cref="jsonPath"/> thus decrypting it.
		/// </summary>
		/// <returns>True if the data file exists and was migrated properly, false if it was not able to migrate.</returns>
		public bool ExportBytesToJson()
		{
			if (!File.Exists(dataPath))
			{
				return false;
			}

			string fileText = encryptor.Decrypt(File.ReadAllBytes(dataPath));
			T objectData = JsonConvert.DeserializeObject<T>(fileText, jsonDeserializeSettings);
			string jsonData = JsonConvert.SerializeObject(objectData, Formatting.Indented, jsonSerializeSettings);
			File.WriteAllText(jsonPath, jsonData);
			return true;
		}
		#endregion

		#region Error Handling
		private void HandleError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
		{
			Debug.LogWarning($"An issue was encountered when deserializing game file \"{fileName}\"! This is likely due to invalid data being provided.\n{errorArgs.ErrorContext.Error.Message}");
			errorArgs.ErrorContext.Handled = true;
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

		/// <summary>
		/// Definition of the format a file should be saved to the disk as.
		/// </summary>
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