namespace Pando.Repositories
{
	public readonly struct SnapshotEntry
	{
		public readonly ulong Hash;
		public readonly ulong ParentHash;
		public readonly ulong RootNodeHash;

		public SnapshotEntry(ulong hash, ulong parentHash, ulong rootNodeHash)
		{
			Hash = hash;
			ParentHash = parentHash;
			RootNodeHash = rootNodeHash;
		}

		public void Deconstruct(out ulong hash, out ulong parentHash, out ulong rootNodeHash)
		{
			hash = Hash;
			parentHash = ParentHash;
			rootNodeHash = RootNodeHash;
		}
	}

	internal readonly struct SnapshotData
	{
		public readonly ulong ParentHash;
		public readonly ulong RootNodeHash;

		public SnapshotData(ulong parentHash, ulong rootNodeHash)
		{
			ParentHash = parentHash;
			RootNodeHash = rootNodeHash;
		}

		public void Deconstruct(out ulong parentHash, out ulong rootNodeHash)
		{
			parentHash = ParentHash;
			rootNodeHash = RootNodeHash;
		}
	}

	internal readonly struct DataSlice
	{
		public readonly int Start;
		public readonly int Length;

		public DataSlice(int start, int length)
		{
			Start = start;
			Length = length;
		}

		public void Deconstruct(out int start, out int length)
		{
			start = Start;
			length = Length;
		}
	}
}
