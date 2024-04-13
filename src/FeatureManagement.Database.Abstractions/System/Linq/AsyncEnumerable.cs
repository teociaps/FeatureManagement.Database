namespace System.Linq;

internal static class AsyncEnumerable
{
    /// <summary>
    /// Returns an empty async-enumerable sequence.
    /// </summary>
    /// <typeparam name="TValue">The type used for the <see cref="IAsyncEnumerable{T}"/> type parameter of the resulting sequence.</typeparam>
    /// <returns>An async-enumerable sequence with no elements.</returns>
    public static IAsyncEnumerable<TValue> Empty<TValue>() => EmptyAsyncIterator<TValue>.Instance;

    internal sealed class EmptyAsyncIterator<TValue> : IAsyncEnumerable<TValue>, IAsyncEnumerator<TValue>
    {
        public static readonly EmptyAsyncIterator<TValue> Instance = new();

        public TValue Current => default!;

        public ValueTask<bool> MoveNextAsync() => new(false);

        public IAsyncEnumerator<TValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this;
        }

        public ValueTask DisposeAsync() => default;
    }
}