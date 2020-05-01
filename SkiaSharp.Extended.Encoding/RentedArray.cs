using System;
using System.Buffers;

namespace SkiaSharp.Extended.Encoding
{
	internal readonly ref struct RentedArray<T>
	{
		public static RentedArray<T> Rent(int length, bool clear = false)
		{
			var array = new RentedArray<T>(length);
			if (clear)
				array.Span.Clear();
			return array;
		}

		internal RentedArray(int length)
		{
			Array = ArrayPool<T>.Shared.Rent(length);
			Span = new Span<T>(Array, 0, length);
		}

		public readonly T[] Array;

		public readonly Span<T> Span;

		public int Length => Span.Length;

		public ref T this[int index] =>
			ref Span[index];

		public Span<T> Slice(int start, int length) =>
			Span.Slice(start, length);

		public Span<T> Slice(int start) =>
			Span.Slice(start);

		public void Fill(T value) =>
			Span.Fill(value);

		public void Dispose() =>
			ArrayPool<T>.Shared.Return(Array);

		public static explicit operator T[](RentedArray<T> scope) =>
			scope.Array;

		public static implicit operator Span<T>(RentedArray<T> scope) =>
			scope.Span;

		public static implicit operator ReadOnlySpan<T>(RentedArray<T> scope) =>
			scope.Span;
	}
}
