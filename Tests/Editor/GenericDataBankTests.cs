using LMirman.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

public class GenericDataBankTests
{
	#region HasKey() Tests
	[Test]
	public void HasKey_DoesHaveKey_ReturnsTrue()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		
		stubData.SetStruct(key, true);
		bool actual = stubData.HasKey(key);

		Assert.True(actual);
	}
	
	[Test]
	public void HasKey_DoesNotHaveKey_ReturnsFalse()
	{
		var stubData = new GenericDataBank();
		string key = "key";

		bool actual = stubData.HasKey(key);
		
		Assert.False(actual);
	}
	#endregion

	#region Struct Get/Set Methods
	[Test]
	public void TryGetStruct_FloatValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		float expected = 0.9f;

		stubData.SetStruct(key, expected);
		stubData.TryGetStruct(key, out float actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetStruct_IntValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		int expected = 10;

		stubData.SetStruct(key, expected);
		stubData.TryGetStruct(key, out int actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetStruct_BoolValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		bool expected = true;

		stubData.SetStruct(key, expected);
		stubData.TryGetStruct(key, out bool actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetStruct_ByteValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		byte expected = 2;

		stubData.SetStruct(key, expected);
		stubData.TryGetStruct(key, out byte actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetStruct_CharValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		char expected = 'a';

		stubData.SetStruct(key, expected);
		stubData.TryGetStruct(key, out char actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetStruct_DoesHaveValue_ReturnsTrue()
	{
		var stubData = new GenericDataBank();
		string key = "key";

		stubData.SetStruct(key, true);
		bool actual = stubData.TryGetStruct(key, out bool _);

		Assert.True(actual);
	}

	[Test]
	public void TryGetStruct_DoesNotHaveValue_ReturnsFalse()
	{
		var stubData = new GenericDataBank();
		string key = "key";

		bool actual = stubData.TryGetStruct(key, out bool _);

		Assert.False(actual);
	}

	[TestCase("")]
	[TestCase("           ")]
	[TestCase(null)]
	[TestCase("\0")]
	[TestCase("\n")]
	public void TryGetStruct_EdgeCaseInput_DoesNotThrow(string key)
	{
		var stubData = new GenericDataBank();
		int expected = 10;

		stubData.SetStruct(key, expected);
		stubData.TryGetStruct(key, out int actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetStruct_LargeEntryCount_CanFunction()
	{
		var stubData = new GenericDataBank();

		void code()
		{
			for (int i = 0; i < 10000; i++)
			{
				stubData.SetStruct(i.ToString(), i);
				stubData.TryGetStruct(i.ToString(), out int _);
			}
		}

		Assert.DoesNotThrow(code);
	}
	
	[Test]
	public void GetStruct_DoesHaveValue_ReturnsSetValue()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		bool expected = true;

		stubData.SetStruct(key, expected);
		bool actual = stubData.GetStruct(key, false);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void GetStruct_DoesNotHaveValue_ReturnsDefault()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		bool expected = true;

		bool actual = stubData.GetStruct(key, expected);

		Assert.AreEqual(actual, expected);
	}
	#endregion

	#region Class Get/Set Methods
	[Test]
	public void TryGetClass_StringValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		string expected = "value";

		stubData.SetClass(key, expected);
		stubData.TryGetClass(key, out string actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetClass_StringValue_OriginalChangeDoesNotPost()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		string expected = "value";

		stubData.SetClass(key, expected);
		stubData.TryGetClass(key, out string actual);
		expected = "not value";

		Assert.AreNotEqual(actual, expected);
	}

	[Test]
	public void TryGetClass_SampleClass_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		SampleClass expected = new SampleClass(1, "value");

		stubData.SetClass(key, expected);
		stubData.TryGetClass(key, out SampleClass actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetClass_SampleClass_OriginalChangeDoesNotPost()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		SampleClass expected = new SampleClass(1, "value");

		stubData.SetClass(key, expected);
		stubData.TryGetClass(key, out SampleClass actual);
		expected.floatValue = 2;
		expected.stringValue = "not value";

		Assert.AreNotEqual(actual, expected);
	}

	[Test]
	public void TryGetClass_LargeEntryCount_CanFunction()
	{
		var stubData = new GenericDataBank();

		void code()
		{
			for (int i = 0; i < 1000; i++)
			{
				SampleClass expected = new SampleClass(i, "value");
				stubData.SetClass(i.ToString(), expected);
				stubData.TryGetClass(i.ToString(), out SampleClass actual);
			}
		}

		Assert.DoesNotThrow(code);
	}
	#endregion
	
	#region General Get/Set Methodss
	[Test]
	public void TryGetGeneralValue_FloatValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		float expected = 0.9f;

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out float actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_IntValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		int expected = 10;

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out int actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_BoolValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		bool expected = true;

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out bool actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_ByteValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		byte expected = 2;

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out byte actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_CharValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		char expected = 'a';

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out char actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_DoesHaveValue_ReturnsTrue()
	{
		var stubData = new GenericDataBank();
		string key = "key";

		stubData.SetStruct(key, true);
		bool actual = stubData.TryGetValue(key, out bool _);

		Assert.True(actual);
	}

	[Test]
	public void TryGetGeneralValue_DoesNotHaveValue_ReturnsFalse()
	{
		var stubData = new GenericDataBank();
		string key = "key";

		bool actual = stubData.TryGetValue(key, out bool _);

		Assert.False(actual);
	}

	[TestCase("")]
	[TestCase("           ")]
	[TestCase(null)]
	[TestCase("\0")]
	[TestCase("\n")]
	public void TryGetGeneralValue_EdgeCaseInput_DoesNotThrow(string key)
	{
		var stubData = new GenericDataBank();
		int expected = 10;

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out int actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_LargeEntryCountStruct_CanFunction()
	{
		var stubData = new GenericDataBank();

		void code()
		{
			for (int i = 0; i < 10000; i++)
			{
				stubData.SetStruct(i.ToString(), i);
				stubData.TryGetValue(i.ToString(), out int _);
			}
		}

		Assert.DoesNotThrow(code);
	}
	
	[Test]
	public void GetGeneralValue_DoesHaveValue_ReturnsSetValue()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		bool expected = true;

		stubData.SetStruct(key, expected);
		bool actual = stubData.GetValue(key, false);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void GetGeneralValue_DoesNotHaveValue_ReturnsDefault()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		bool expected = true;

		bool actual = stubData.GetValue(key, expected);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_IConvertibleStringValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		string expected = "value";

		stubData.SetClass(key, expected);
		stubData.TryGetValue(key, out IConvertible output);
		string actual = (string)Convert.ChangeType(output, typeof(string));

		Assert.AreEqual(actual, expected);
	}
	
	[Test]
	public void TryGetGeneralValue_IConvertibleIntValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		int expected = 4;

		stubData.SetStruct(key, expected);
		stubData.TryGetValue(key, out IConvertible output);
		int actual = (int)Convert.ChangeType(output, typeof(int));

		Assert.AreEqual(actual, expected);
	}
	
	[Test]
	public void TryGetGeneralValue_StringValue_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		string expected = "value";

		stubData.SetClass(key, expected);
		stubData.TryGetValue(key, out string actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_StringValue_OriginalChangeDoesNotPost()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		string expected = "value";

		stubData.SetClass(key, expected);
		stubData.TryGetValue(key, out string actual);
		expected = "not value";

		Assert.AreNotEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_SampleClass_ValueMatches()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		SampleClass expected = new SampleClass(1, "value");

		stubData.SetClass(key, expected);
		stubData.TryGetValue(key, out SampleClass actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_SampleClass_OriginalChangeDoesNotPost()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		SampleClass expected = new SampleClass(1, "value");

		stubData.SetClass(key, expected);
		stubData.TryGetValue(key, out SampleClass actual);
		expected.floatValue = 2;
		expected.stringValue = "not value";

		Assert.AreNotEqual(actual, expected);
	}

	[Test]
	public void TryGetGeneralValue_LargeEntryCountClass_CanFunction()
	{
		var stubData = new GenericDataBank();

		void code()
		{
			for (int i = 0; i < 1000; i++)
			{
				SampleClass expected = new SampleClass(i, "value");
				stubData.SetClass(i.ToString(), expected);
				stubData.TryGetValue(i.ToString(), out SampleClass actual);
			}
		}

		Assert.DoesNotThrow(code);
	}
	#endregion

	#region Remove Entry Methods
	[Test]
	public void RemoveEntry_HadEntry_RemovesEntry()
	{
		var stubData = new GenericDataBank();
		string key = "key1";
		bool expected = false;

		stubData.SetStruct(key, true);
		stubData.RemoveEntry(key);
		bool actual = stubData.HasKey(key);

		Assert.AreEqual(actual, expected);
	}
	
	[Test]
	public void DeleteAllEntries_HadEntries_RemovesEntries()
	{
		var stubData = new GenericDataBank();
		string key = "key1";
		bool expected = false;

		stubData.SetStruct(key, true);
		stubData.DeleteAllEntries();
		bool actual = stubData.HasKey(key);

		Assert.AreEqual(actual, expected);
	}
	#endregion

	#region Serialization Tests
	[Test]
	public void ReadWrite_JSONData_DoesMatch()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		SampleClass expected = new SampleClass(1, "value");

		stubData.SetClass(key, expected);
		string serialized = JsonConvert.SerializeObject(stubData);
		GenericDataBank deserialized = JsonConvert.DeserializeObject<GenericDataBank>(serialized);
		deserialized.TryGetClass(key, out SampleClass actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void ReadWrite_StringData_DoesMatch()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		string expected = "value";

		stubData.SetClass(key, expected);
		string serialized = JsonConvert.SerializeObject(stubData);
		GenericDataBank deserialized = JsonConvert.DeserializeObject<GenericDataBank>(serialized);
		deserialized.TryGetClass(key, out string actual);

		Assert.AreEqual(actual, expected);
	}

	[Test]
	public void ReadWrite_FloatData_DoesMatch()
	{
		var stubData = new GenericDataBank();
		string key = "key";
		float expected = 1;

		stubData.SetStruct(key, expected);
		string serialized = JsonConvert.SerializeObject(stubData);
		GenericDataBank deserialized = JsonConvert.DeserializeObject<GenericDataBank>(serialized);
		deserialized.TryGetStruct(key, out float actual);

		Assert.AreEqual(actual, expected);
	}
	#endregion

	[Serializable]
	public class SampleClass
	{
		public float floatValue;
		public string stringValue;

		public SampleClass(float floatValue, string stringValue)
		{
			this.floatValue = floatValue;
			this.stringValue = stringValue;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			else
			{
				SampleClass other = (SampleClass)obj;
				return Math.Abs(floatValue - other.floatValue) < 0.0001f && stringValue == other.stringValue;
			}
		}

		public override int GetHashCode()
		{
			int hashCode = 1042650979;
			hashCode = (hashCode * -1521134295) + floatValue.GetHashCode();
			hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(stringValue);
			return hashCode;
		}
	}
}