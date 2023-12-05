using System.Buffers;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using CommunityToolkit.HighPerformance;

namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 03, CodeType.Csa)]
public sealed partial class Day_03_Csa : IPuzzle
{
	private static readonly SearchValues<byte> _symbols = SearchValues.Create("0123456789."u8);

	public (string, string) Solve(PuzzleInput input)
	{
		var span = input.Span;

		int width = span.IndexOf((byte)'\n');
		int rowLength = width + 1;
		int height = span.Length / rowLength;

		// For each row, this stores the offset of the last digit that has already been processed
		// This is used to prevent double-counting across vector boundaries
		int[] rowLastDigitOffsets = new int[height];

		int part1 = 0;
		int part2 = 0;

		for (int vecStartIndex = 0; vecStartIndex + 1 < width; vecStartIndex += Vector256<byte>.Count - 2)
		{
			ref byte spanRef = ref MemoryMarshal.GetReference(span);
			uint symbolsMask = uint.MaxValue;
			uint digitsMask = uint.MaxValue;
			uint gearsMask = 0x7FFFFFFE; // On each iteration, ignore the first and last gear

			if (vecStartIndex == 0)
			{
				gearsMask = 0x7FFFFFFF; // On the first iteration, we want to include the first gear
			}
			else if (vecStartIndex + Vector256<byte>.Count > rowLength)
			{
				// Since the span doesn't line up perfectly with 256 bit vectors, the last vector will have some overlap with the previous iteration
				int newI = rowLength - Vector256<byte>.Count;
				symbolsMask = uint.MaxValue >> 1; // For the last iteration, the last element is a newline so we exclude it
				digitsMask = uint.MaxValue << (vecStartIndex - newI); // Ignore any digits that overlap the previous iteration
				gearsMask <<= vecStartIndex - newI; // Ignore any gears that overlap the previous iteration
				vecStartIndex = newI;
			}

			uint curDigits = 0;
			uint curSymbols = 0;

			Vector256<byte> next = Vector256.LoadUnsafe(ref spanRef, (nuint)vecStartIndex);
			ExtractBitSets(next, out uint nextDigits, out uint nextGears, out uint nextSymbols);
			nextGears &= gearsMask;
			nextSymbols &= symbolsMask;

			for (int row = 0; row < height; row++)
			{
				int spanOffset = vecStartIndex + rowLength * row;

				uint prevDigits = curDigits;
				uint prevSymbols = curSymbols;
				curDigits = nextDigits;
				curSymbols = nextSymbols;
				uint curGears = nextGears;

				// If this is the last row, then the next row should be treated as all periods
				next = row < height - 1 ? Vector256.LoadUnsafe(ref spanRef, (nuint)(spanOffset + rowLength)) : Vector256.Create((byte)'.');

				ExtractBitSets(next, out nextDigits, out nextGears, out nextSymbols);
				nextGears &= gearsMask;
				nextSymbols &= symbolsMask;

				// Returns a bitset indicating if there is a symbol in any direction from a given index
				uint symbolsAboveAndBelow = prevSymbols | curSymbols | nextSymbols;
				uint symbolsInAllDirections = symbolsAboveAndBelow | (symbolsAboveAndBelow << 1) | (symbolsAboveAndBelow >> 1);
				uint digitSymbolIndexes = curDigits & digitsMask & symbolsInAllDirections;

				// Prevent double counting by ignoring numbers that have already been processed
				if (rowLastDigitOffsets[row] >= spanOffset)
					digitSymbolIndexes &= uint.MaxValue << (rowLastDigitOffsets[row] - spanOffset + 1);

				int k = 0;
				while (digitSymbolIndexes != 0)
				{
					int zeroes = BitOperations.TrailingZeroCount(digitSymbolIndexes);
					int num = FindNumAtIndex(k + zeroes, curDigits, span, spanOffset, out int numEndIndex);

					part1 += num;
					int digitsToSkip = numEndIndex - (spanOffset + k);
					k += digitsToSkip;
					rowLastDigitOffsets[row] = numEndIndex;

					digitSymbolIndexes = (uint)((ulong)digitSymbolIndexes >> digitsToSkip);
				}

				int gearIndex = 0;
				while (curGears != 0)
				{
					int zeroes = BitOperations.TrailingZeroCount(curGears);
					int gearRatio = GetGearRatio(gearIndex + zeroes, prevDigits, curDigits, nextDigits, span, spanOffset, rowLength);
					part2 += gearRatio;

					curGears = (uint)((ulong)curGears >> (zeroes + 1));
					gearIndex += zeroes + 1;
				}
			}
		}

		return (part1.ToString(), part2.ToString());
	}

	private static void ExtractBitSets(Vector256<byte> next, out uint nextDigits, out uint nextGears, out uint nextSymbols)
	{
		var nextDigitsVec = Vector256.LessThan(next - Vector256.Create((byte)'0'), Vector256.Create((byte)10));
		nextDigits = Vector256.ExtractMostSignificantBits(nextDigitsVec);
		var nextPeriodsVec = Vector256.Equals(next, Vector256.Create((byte)'.'));
		nextSymbols = Vector256.ExtractMostSignificantBits(~(nextDigitsVec | nextPeriodsVec));
		nextGears = Vector256.ExtractMostSignificantBits(Vector256.Equals(next, Vector256.Create((byte)'*')));
	}

	private static int FindNumAtIndex(int digitIndex, uint digitsBitSet, ReadOnlySpan<byte> span, int spanOffset, out int numEndIndex)
	{
		// Using the digitsBitSet, will find the start and end index of the number containing the digit at digitIndex
		int numStartIndex = 32 - BitOperations.LeadingZeroCount(~(digitsBitSet | (uint.MaxValue << digitIndex))) + spanOffset;
		numEndIndex = BitOperations.TrailingZeroCount(~(digitsBitSet | (uint)((ulong)uint.MaxValue >> (32 - digitIndex)))) + spanOffset;

		byte c;
		int num = 0;

		// Process number before span vector
		if (numStartIndex == spanOffset)
		{
			int mul = 10;
			int numStartspan = numStartIndex;
			while ((c = span[--numStartspan]) is >= (byte)'0' and <= (byte)'9')
			{
				num += mul * (c - '0');
				mul *= 10;
			}
		}

		// Process number inside span vector
		num += span[numStartIndex] - '0';
		for (int i = numStartIndex + 1; i < numEndIndex; i++)
			num = 10 * num + span[i] - '0';

		// Process number after span vector
		if (numEndIndex == spanOffset + Vector256<byte>.Count)
		{
			while ((c = span[numEndIndex]) is >= (byte)'0' and <= (byte)'9')
			{
				num = num * 10 + c - '0';
				numEndIndex++;
			}
		}

		return num;
	}

	private static int GetGearRatio(int gearIndex, uint prevDigits, uint curDigits, uint nextDigits, ReadOnlySpan<byte> span, int spanOffset, int rowLength)
	{
		int gearRatio = 1;
		int numNumbersOnGear = 0;

		// check north
		if ((prevDigits & (1 << gearIndex)) != 0)
		{
			gearRatio *= FindNumAtIndex(gearIndex, prevDigits, span, spanOffset - rowLength, out _);
			numNumbersOnGear++;
		}
		else
		{
			// check north-west
			if ((prevDigits & (1 << (gearIndex - 1))) != 0)
			{
				gearRatio *= FindNumAtIndex(gearIndex - 1, prevDigits, span, spanOffset - rowLength, out _);
				numNumbersOnGear++;
			}

			// check north-east
			if ((prevDigits & (1 << (gearIndex + 1))) != 0)
			{
				gearRatio *= FindNumAtIndex(gearIndex + 1, prevDigits, span, spanOffset - rowLength, out _);
				numNumbersOnGear++;
			}
		}

		// check west
		if ((curDigits & (1 << (gearIndex - 1))) != 0)
		{
			gearRatio *= FindNumAtIndex(gearIndex - 1, curDigits, span, spanOffset, out _);
			numNumbersOnGear++;
		}

		// check east
		if ((curDigits & (1 << (gearIndex + 1))) != 0)
		{
			gearRatio *= FindNumAtIndex(gearIndex + 1, curDigits, span, spanOffset, out _);
			numNumbersOnGear++;
		}

		// check south
		if ((nextDigits & (1 << gearIndex)) != 0)
		{
			gearRatio *= FindNumAtIndex(gearIndex, nextDigits, span, spanOffset + rowLength, out _);
			numNumbersOnGear++;
		}
		else
		{
			// check south-west
			if ((nextDigits & (1 << (gearIndex - 1))) != 0)
			{
				gearRatio *= FindNumAtIndex(gearIndex - 1, nextDigits, span, spanOffset + rowLength, out _);
				numNumbersOnGear++;
			}

			// check south-east
			if ((nextDigits & (1 << (gearIndex + 1))) != 0)
			{
				gearRatio *= FindNumAtIndex(gearIndex + 1, nextDigits, span, spanOffset + rowLength, out _);
				numNumbersOnGear++;
			}
		}

		if (numNumbersOnGear == 2)
			return gearRatio;

		return 0;
	}
}
