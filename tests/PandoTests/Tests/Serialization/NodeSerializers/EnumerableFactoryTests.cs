using System.Collections.Generic;
using System.Collections.Immutable;
using FluentAssertions;
using Pando.Serialization.NodeSerializers.EnumerableFactory;
using Xunit;

namespace PandoTests.Tests.Serialization.NodeSerializers;

public abstract class BaseEnumerableFactoryTests<TEnumerable> where TEnumerable : IEnumerable<int>
{
	protected abstract IEnumerableFactory<TEnumerable, int> Factory { get; }
	protected abstract bool OrderMatters { get; }

	[Theory]
	[InlineData(new int[] { })]
	[InlineData(new[] { 1, 2, 3 })]
	public void Should_create_correct_enumerable(int[] data)
	{
		var actual = Factory.Create(data);

		actual.Should().BeEquivalentTo(data);

		if (OrderMatters)
		{
			actual.Should().ContainInOrder(data);
		}
	}
}

public abstract class SetEnumerableFactoryTests<TEnumerable> : BaseEnumerableFactoryTests<TEnumerable> where TEnumerable : ISet<int>
{
	[Fact]
	public void Should_not_contain_duplicates()
	{
		int[] data = { 1, 1, 2 };
		var actual = Factory.Create(data);

		actual.Should().BeEquivalentTo(new[] { 1, 2 });
	}
}

public class EnumerableFactoryTests
{
	public class ArrayFactoryTests : BaseEnumerableFactoryTests<int[]>
	{
		protected override IEnumerableFactory<int[], int> Factory => new ArrayFactory<int>();
		protected override bool OrderMatters => true;
	}

	public class ImmutableArrayFactoryTests : BaseEnumerableFactoryTests<ImmutableArray<int>>
	{
		protected override IEnumerableFactory<ImmutableArray<int>, int> Factory => new ImmutableArrayFactory<int>();
		protected override bool OrderMatters => true;
	}

	public class ListFactoryTests : BaseEnumerableFactoryTests<List<int>>
	{
		protected override IEnumerableFactory<List<int>, int> Factory => new ListFactory<int>();
		protected override bool OrderMatters => true;
	}

	public class ImmutableListFactoryTests : BaseEnumerableFactoryTests<ImmutableList<int>>
	{
		protected override IEnumerableFactory<ImmutableList<int>, int> Factory => new ImmutableListFactory<int>();
		protected override bool OrderMatters => true;
	}

	public class HashSetFactoryTests : SetEnumerableFactoryTests<HashSet<int>>
	{
		protected override IEnumerableFactory<HashSet<int>, int> Factory => new HashSetFactory<int>();
		protected override bool OrderMatters => false;
	}

	public class ImmutableHashSetFactoryTests : SetEnumerableFactoryTests<ImmutableHashSet<int>>
	{
		protected override IEnumerableFactory<ImmutableHashSet<int>, int> Factory => new ImmutableHashSetFactory<int>();
		protected override bool OrderMatters => false;
	}
}
