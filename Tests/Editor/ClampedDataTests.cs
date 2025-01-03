using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace LMirman.Utilities.Tests.Editor
{
	public class ClampedDataTests
	{
		[TestCase(1, 0, 2)]
		[TestCase(1f, 0f, 2f)]
		[TestCase(1.0, 0.0, 2.0)]
		public void Value_Constructor_SetsInitialValue<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(initialValue, min, max);

			Assert.AreEqual(stubData.Value, initialValue);
		}

		[TestCase(0, 1, 2)]
		[TestCase(0f, 1f, 2f)]
		[TestCase(0.0, 1.0, 2.0)]
		public void Value_Constructor_ClampsToMin<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(initialValue, min, max);

			Assert.AreEqual(stubData.Value, min);
		}

		[TestCase(3, 1, 2)]
		[TestCase(3f, 1f, 2f)]
		[TestCase(3.0, 1.0, 2.0)]
		public void Value_Constructor_ClampsToMax<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(initialValue, min, max);

			Assert.AreEqual(stubData.Value, max);
		}

		[TestCase(1, 0, 2)]
		[TestCase(1f, 0f, 2f)]
		[TestCase(1.0, 0.0, 2.0)]
		public void Value_SetValue_SetsInitialValue<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(min, min, max);

			stubData.Value = initialValue;

			Assert.AreEqual(stubData.Value, initialValue);
		}

		[TestCase(0, 1, 2)]
		[TestCase(0f, 1f, 2f)]
		[TestCase(0.0, 1.0, 2.0)]
		public void Value_SetValue_ClampsToMin<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(max, min, max);

			stubData.Value = initialValue;

			Assert.AreEqual(stubData.Value, min);
		}

		[TestCase(3, 1, 2)]
		[TestCase(3f, 1f, 2f)]
		[TestCase(3.0, 1.0, 2.0)]
		public void Value_SetValue_ClampsToMax<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(min, min, max);

			stubData.Value = initialValue;

			Assert.AreEqual(stubData.Value, max);
		}

		[TestCase(1, 0, 2)]
		[TestCase(1f, 0f, 2f)]
		[TestCase(1.0, 0.0, 2.0)]
		public void Value_ImplicitCast_IsValid<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(min, min, max);

			stubData.Value = initialValue;
			T value = stubData;

			Assert.AreEqual(value, initialValue);
		}

		[TestCase(1, 0, 2)]
		[TestCase(1f, 0f, 2f)]
		[TestCase(1.0, 0.0, 2.0)]
		public void JsonSerialization_WriteRead_MaintainsValue<T>(T initialValue, T min, T max) where T : IComparable
		{
			var stubData = new ClampedValue<T>(initialValue, min, max);

			string serialized = JsonConvert.SerializeObject(stubData);
			ClampedValue<T> deserialized = JsonConvert.DeserializeObject<ClampedValue<T>>(serialized);

			Assert.AreEqual(deserialized.Value, initialValue);
		}

		[Serializable]
		private class TestClassA
		{
			public ClampedField<int> clampedValue = new ClampedField<int>(25, 20, 30);
		}

		[Serializable]
		private class TestClassB
		{
			public ClampedField<int> clampedValue = new ClampedField<int>(5, 0, 10);
		}

		[Test]
		public void JsonSerialization_Encapsulated_UsesClassDefault()
		{
			var stubData = new TestClassA();

			string serialized = JsonConvert.SerializeObject(stubData);
			TestClassB deserialized = JsonConvert.DeserializeObject<TestClassB>(serialized);

			Assert.AreEqual(deserialized.clampedValue.Value, 10);
		}
	}
}