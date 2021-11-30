using System;

namespace PandoTests.PandoSave.TestStateTrees;

public record TestTree(string Name, TestTree.A MyA, TestTree.B MyB)
{
	public readonly struct A
	{
		public readonly int Age;

		public A(int age) { Age = age; }
	}

	public readonly struct B
	{
		public readonly DateTime Time;
		public readonly int Cents;

		public B(DateTime time, int cents)
		{
			Time = time;
			Cents = cents;
		}
	}
}