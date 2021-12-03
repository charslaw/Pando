using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Pando.Repositories;
using Pando.Repositories.Utils;
using Standart.Hash.xxHash;

namespace PandoTests.Repositories.StreamRepositoryTests;

public class SnapshotOperations
{
	[Test]
	public void Should_output_snapshot_index_when_root_snapshot_added()
	{
		// Test Data
		ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;
		var expectedIndex = new byte[]
		{
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // parent hash
			0xDD, 0x00, 0xCC, 0xFF, 0xBB, 0x00, 0xAA, 0xFF  // root node hash
		};
		var expectedHash = xxHash64.ComputeHash(expectedIndex);

		// Arrange
		var snapshotIndexStream = new MemoryStream();
		using var repository = new StreamRepository(snapshotIndexStream, Stream.Null, Stream.Null);

		// Act
		repository.AddSnapshot(0, rootNodeHash);

		// Assert
		var snapshotIndex = snapshotIndexStream.ToArray();
		var actualHash = ByteConverter.GetUInt64(snapshotIndex);
		var actualIndex = snapshotIndex[8..];
		actualIndex.Should().Equal(expectedIndex);
		actualHash.Should().Be(expectedHash);
	}

	[Test]
	public void Should_output_snapshot_index_when_snapshot_added()
	{
		// Test Data
		ulong parentHash = 0x12_34_56_78_90_AB_CD_EF;
		ulong rootNodeHash = 0xFF_AA_00_BB_FF_CC_00_DD;
		var expectedIndex = new byte[]
		{
			0xEF, 0xCD, 0xAB, 0x90, 0x78, 0x56, 0x34, 0x12, // parent hash
			0xDD, 0x00, 0xCC, 0xFF, 0xBB, 0x00, 0xAA, 0xFF  // root node hash
		};
		var expectedHash = xxHash64.ComputeHash(expectedIndex);

		// Arrange
		var snapshotIndexStream = new MemoryStream();
		using var repository = new StreamRepository(snapshotIndexStream, Stream.Null, Stream.Null);

		// Act
		repository.AddSnapshot(parentHash, rootNodeHash);

		// Assert
		var snapshotIndex = snapshotIndexStream.ToArray();
		var actualHash = ByteConverter.GetUInt64(snapshotIndex);
		var actualIndex = snapshotIndex[8..];
		actualIndex.Should().Equal(expectedIndex);
		actualHash.Should().Be(expectedHash);
	}
}
