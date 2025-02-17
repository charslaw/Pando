using System;
using Pando.Serialization;
using Pando.Serialization.Collections;
using Pando.Serialization.Generic;
using Pando.Serialization.Primitives;

namespace PandoTests.Tests.Repositories.TestStateTrees;

public record TestTree(string Name, TestTree.A MyA, TestTree.B MyB) : IGenericSerializable<TestTree, string, TestTree.A, TestTree.B>
{
	public static TestTree Construct(string name, A myA, B myB) => new(name, myA, myB);

	public record A(int Age) : IGenericSerializable<A, int> {
		public static A Construct(int age) => new(age);
	}

	public record B(DateTime Time, int Cents) : IGenericSerializable<B, DateTime, int> {
		public static B Construct(DateTime time, int cents) => new(time, cents);
	}

	public static IPandoSerializer<TestTree> GenericSerializer() =>
		new GenericNodeSerializer<TestTree, string, A, B>(
			StringSerializer.ASCII,
			new GenericNodeSerializer<A, int>(Int32LittleEndianSerializer.Default),
			new GenericNodeSerializer<B, DateTime, int>(DateTimeToBinarySerializer.Default, Int32LittleEndianSerializer.Default)
		);
}
