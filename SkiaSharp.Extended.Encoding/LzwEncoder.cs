using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

namespace SkiaSharp.Extended.Encoding
{
	// Encodes and compresses the image data using dynamic Lempel-Ziv compression.
	//
	// Adapted from Jef Poskanzer's Java port by way of J. M. G. Elliott. K Weiner 12/00
	//
	// GIFCOMPR.C - GIF Image compression routines
	//
	// Lempel-Ziv compression based on 'compress'.
	// GIF modifications by David Rowley (mgardi@watdcsu.waterloo.edu)
	//
	// GIF Image compression - modified 'compress'
	//
	// Based on: compress.c - File compression ala IEEE Computer, June 1984.
	// By Authors:  Spencer W. Thomas      (decvax!harpo!utah-cs!utah-gr!thomas)
	//              Jim McKie              (decvax!mcvax!jim)
	//              Steve Davies           (decvax!vax135!petsd!peora!srd)
	//              Ken Turkowski          (decvax!decwrl!turtlevax!ken)
	//              James A. Woods         (decvax!ihnp4!ames!jaw)
	//              Joe Orost              (decvax!vax135!petsd!joe)

	internal unsafe sealed class LzwEncoder : IDisposable
	{
		private const int AccumulatorsSize = 256;
		// 80% occupancy
		private const int HashSize = 5003;
		// the amount to shift each code
		private const int HashShift = 4;
		// the maximum number of bits/code
		private const int MaxBits = 12;
		// should NEVER generate this code
		private const int MaxMaxCode = 1 << MaxBits;
		// mask used when shifting pixel values
		private static readonly int[] Masks =
		{
			0b0,
			0b1,
			0b11,
			0b111,
			0b1111,
			0b11111,
			0b111111,
			0b1111111,
			0b11111111,
			0b111111111,
			0b1111111111,
			0b11111111111,
			0b111111111111,
			0b1111111111111,
			0b11111111111111,
			0b111111111111111,
			0b1111111111111111
		};

		// the storage for the packet accumulator
		private readonly byte[] accumulators;

		// the tables
		private readonly int[] hashTable;
		private readonly int[] codeTable;

		// the initial code size
		private int initialCodeSize;
		// the initial number of bits
		private int initialBits;

		// Block compression parameters - after all codes are used up, and compression rate changes, start over
		private bool clearFlag;
		// the number of bits/code
		private int bitCount;
		// the maximum code, given bitCount
		private int maxCode;
		// the clear code
		private int clearCode;
		// the end-of-file code
		private int eofCode;
		// the first unused entry
		private int freeEntry;
		// the number of characters so far in this 'packet'
		private int accumulatorCount;

		// the current bits
		private int currentBits;
		private int currentAccumulator;

		public LzwEncoder()
		{
			accumulators = ArrayPool<byte>.Shared.Rent(AccumulatorsSize);
			accumulators.AsSpan().Clear();

			hashTable = ArrayPool<int>.Shared.Rent(HashSize);
			hashTable.AsSpan().Clear();

			codeTable = ArrayPool<int>.Shared.Rent(HashSize);
			codeTable.AsSpan().Clear();
		}

		public void Encode(Stream stream, SKQuantizedFrame quantizedFrame)
		{
			initialCodeSize = Math.Max(2, quantizedFrame.ColorDepth);
			initialBits = initialCodeSize + 1;

			clearFlag = false;
			bitCount = initialBits;
			maxCode = GetMaxcode(bitCount);
			clearCode = 1 << (initialBits - 1);
			eofCode = clearCode + 1;
			freeEntry = clearCode + 2;
			accumulatorCount = 0;

			currentBits = 0;
			currentAccumulator = 0;

			hashTable.AsSpan().Fill(-1);

			Output(stream, clearCode);

			// write "initial code size" byte
			stream.WriteByte((byte)initialCodeSize);

			// compress and write the pixel data
			Compress(stream, quantizedFrame.IndexedPixels);

			// write block terminator
			stream.WriteByte(0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetMaxcode(int bitCount) => (1 << bitCount) - 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddCharacter(Stream stream, byte c, byte* accumulatorsRef)
		{
			accumulatorsRef[accumulatorCount++] = c;
			if (accumulatorCount >= 254)
				FlushPacket(stream);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ClearBlock(Stream stream)
		{
			hashTable.AsSpan().Fill(-1);
			freeEntry = clearCode + 2;
			clearFlag = true;

			Output(stream, clearCode);
		}

		private void Compress(Stream stream, ReadOnlySpan<byte> indexedPixels)
		{
			fixed (int* hashTableRef = hashTable)
			fixed (int* codeTableRef = codeTable)
			{
				int entry = indexedPixels[0];

				for (int index = 1; index < indexedPixels.Length; index++)
				{
					byte code = indexedPixels[index];
					int freeCode = (code << MaxBits) + entry;
					int hashIndex = (code << HashShift) ^ entry;

					if (hashTableRef[hashIndex] == freeCode)
					{
						entry = codeTableRef[hashIndex];
						continue;
					}

					// non-empty slot
					if (hashTableRef[hashIndex] >= 0)
					{
						int disp = 1;
						if (hashIndex != 0)
						{
							disp = HashSize - hashIndex;
						}

						do
						{
							if ((hashIndex -= disp) < 0)
							{
								hashIndex += HashSize;
							}

							if (hashTableRef[hashIndex] == freeCode)
							{
								entry = codeTableRef[hashIndex];
								break;
							}
						}
						while (hashTableRef[hashIndex] >= 0);

						if (hashTableRef[hashIndex] == freeCode)
						{
							continue;
						}
					}

					Output(stream, entry);
					entry = code;
					if (freeEntry < MaxMaxCode)
					{
						codeTableRef[hashIndex] = freeEntry++; // code -> hashtable
						hashTableRef[hashIndex] = freeCode;
					}
					else
					{
						ClearBlock(stream);
					}
				}

				// output the final code
				Output(stream, entry);
				Output(stream, eofCode);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FlushPacket(Stream stream)
		{
			stream.WriteByte((byte)accumulatorCount);
			stream.Write(accumulators, 0, accumulatorCount);
			accumulatorCount = 0;
		}

		private void Output(Stream outs, int code)
		{
			fixed (byte* accumulatorsRef = accumulators)
			{
				currentAccumulator &= Masks[currentBits];

				if (currentBits > 0)
					currentAccumulator |= code << currentBits;
				else
					currentAccumulator = code;

				currentBits += bitCount;

				while (currentBits >= 8)
				{
					AddCharacter(outs, (byte)(currentAccumulator & 0xFF), accumulatorsRef);
					currentAccumulator >>= 8;
					currentBits -= 8;
				}

				// if the next entry is going to be too big for the code size, then increase it, if possible
				if (freeEntry > maxCode || clearFlag)
				{
					if (clearFlag)
					{
						maxCode = GetMaxcode(bitCount = initialBits);
						clearFlag = false;
					}
					else
					{
						++bitCount;
						maxCode = bitCount == MaxBits
							? MaxMaxCode
							: GetMaxcode(bitCount);
					}
				}

				if (code == eofCode)
				{
					// at EOF, write the rest of the buffer
					while (currentBits > 0)
					{
						AddCharacter(outs, (byte)(currentAccumulator & 0xFF), accumulatorsRef);
						currentAccumulator >>= 8;
						currentBits -= 8;
					}

					if (accumulatorCount > 0)
						FlushPacket(outs);
				}
			}
		}

		public void Dispose()
		{
			ArrayPool<byte>.Shared.Return(accumulators);
			ArrayPool<int>.Shared.Return(hashTable);
			ArrayPool<int>.Shared.Return(codeTable);
		}
	}
}
