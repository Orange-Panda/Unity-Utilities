using LMirman.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

public class GameFileTests
{
	[TestCase("saveName", "/")]
	[TestCase("directoryName/saveName", "/directoryName/")]
	[TestCase("directory1/directory2/saveName", "/directory1/directory2/")]
	[TestCase("directory1\\directory2\\saveName", "/directory1/directory2/")]
	[TestCase("directory1\\directory2\\saveName\\", "/directory1/directory2/")]
	public void Constructor_GameFile_CreatesLocalDirectory(string input, string expected)
	{
		var stub = new GameFile<string>(input, Encryption.DefaultEncryptor);
		Assert.AreEqual(expected, stub.localFileDirectory);
	}
	
	[TestCase("saveName", "saveName")]
	[TestCase("directoryName/saveName", "saveName")]
	[TestCase("directory1/directory2/saveName", "saveName")]
	[TestCase("directory1\\directory2\\saveName", "saveName")]
	[TestCase("directory1\\directory2\\saveName\\", "saveName")]
	public void Constructor_GameFile_CreatesFileName(string input, string expected)
	{
		var stub = new GameFile<string>(input, Encryption.DefaultEncryptor);
		Assert.AreEqual(expected, stub.fileName);
	}
}