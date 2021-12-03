using Pando.Exceptions;

namespace Pando;

public interface IPandoSaver<T>
{
	/// Saves the given state tree snapshot, then returns a hash of the snapshot that can be used to retrieve it.
	ulong SaveSnapshot(T tree);

	/// Retrieves the state tree identified by the given hash.
	/// <exception cref="HashNotFoundException">Thrown if the given hash does not exist.</exception>
	T GetSnapshot(ulong hash);

	SnapshotChain<T> GetFullSnapshotChain();
}
