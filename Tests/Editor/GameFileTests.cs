using NUnit.Framework;

namespace LMirman.Utilities.Tests.Editor
{
	public class GameFileTests
	{
		[TestCase("saveName", "/")]
		[TestCase("directoryName/saveName", "/directoryName/")]
		[TestCase("directory1/directory2/saveName", "/directory1/directory2/")]
		[TestCase(@"directory1\directory2\saveName", "/directory1/directory2/")]
		[TestCase(@"directory1\directory2\saveName\", "/directory1/directory2/")]
		public void Constructor_GameFile_CreatesLocalDirectory(string input, string expected)
		{
			GameFile<string> stub = new GameFile<string>(input, Encryption.DefaultEncryptor);
			Assert.AreEqual(expected, stub.localFileDirectory);
		}

		[TestCase("saveName", "saveName")]
		[TestCase("directoryName/saveName", "saveName")]
		[TestCase("directory1/directory2/saveName", "saveName")]
		[TestCase(@"directory1\directory2\saveName", "saveName")]
		[TestCase(@"directory1\directory2\saveName\", "saveName")]
		public void Constructor_GameFile_CreatesFileName(string input, string expected)
		{
			GameFile<string> stub = new GameFile<string>(input, Encryption.DefaultEncryptor);
			Assert.AreEqual(expected, stub.fileName);
		}
	}
}