﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 17, CodeType.Csa)]
public partial class Day_17_Csa : IPuzzle
{
	// The solutions to parts 1 and 2 are very similar, but there are enough small differences that I couldn't find
	// any nice way to abstract them away so I have just written each part separately.
	//
	// The solution is essentially A* using Manhattan Distance as the heuristic. I use a bucket queue and a bitset to
	// iterate through the possible states in order. A state is essentially (int x, int y, bool isVertical) where x,y
	// represents the position and isVertical represents if the last move to reach this space was a vertical or
	// horizontal move. I pack these three values into a ushort, with the least significant bit representing isVertical
	// and the bits above it equal to y * width + x. When I process a new state, I first check if it is seen in the
	// bitset and if it isn't, I iterate through all the possible full strides I can make from my current position.
	// This means that I am not moving one step at a time, but each turn I am alternating between a horizontal move and
	// a vertical move.
	public (string, string) Solve(PuzzleInput pi)
	{
		var input = pi.Span;

		int width = input.IndexOf((byte)'\n');
		int rowLength = width + 1;
		int height = input.Length / rowLength;

		int part1 = SolvePart1(ref MemoryMarshal.GetReference(input), width, height);
		int part2 = SolvePart2(ref MemoryMarshal.GetReference(input), width, height);

		return (part1.ToString(), part2.ToString());
	}

	public static int SolvePart1(ref byte input, int width, int height)
	{
		int rowLength = width + 1;
		int numStates = width * height * 2;
		int targetState = (height - 1) * width + (width - 1);

		const int xMul = 2;
		int yMul = 2 * height;

		ulong[] seen = new ulong[(numStates - 1) / 64 + 1];
		ref ulong seenRef = ref MemoryMarshal.GetArrayDataReference(seen);

		List<ushort>[] buckets = new List<ushort>[32]; // Only 32 buckets needed to handle all possible moves from the best current state
		for (int i = 0; i < buckets.Length; i++)
			buckets[i] = new List<ushort>(800);

		int distanceAtBucketStart = width + height - 2;
		int bucketPtr = 0;
		buckets[0].Add(0);
		buckets[0].Add(1);

		while (true)
		{
			List<ushort> bucket = buckets[bucketPtr];

			for (int i = 0; i < bucket.Count; i++)
			{
				ushort element = bucket[i];
				ref ulong seenBitset = ref Unsafe.Add(ref seenRef, (uint)element / 64);
				ulong elementBit = 1UL << element;
				if ((seenBitset & elementBit) != 0)
					continue;
				seenBitset |= elementBit;

				int packedXY = Math.DivRem(element, 2, out int isHorizontal);
				if (packedXY == targetState)
					return distanceAtBucketStart;

				int y = Math.DivRem(packedXY, width, out int x);
				int rowOffset = y * rowLength + x;

				if (isHorizontal == 0)
				{
					int total = 0;
					int maxX = Math.Min(4, width - x);
					for (int x2 = 1; x2 < maxX; x2++)
					{
						total += Unsafe.Add(ref input, rowOffset + x2) - '0' - 1;
						buckets[(bucketPtr + total) % 32].Add((ushort)(element + xMul * x2 + 1));
					}

					total = 0;
					int minX = Math.Max(-3, -x);
					for (int x2 = -1; x2 >= minX; x2--)
					{
						total += Unsafe.Add(ref input, rowOffset + x2) - '0' + 1;
						buckets[(bucketPtr + total) % 32].Add((ushort)(element + xMul * x2 + 1));
					}
				}
				else
				{
					int total = 0;
					int maxY = Math.Min(4, height - y);
					for (int y2 = 1; y2 < maxY; y2++)
					{
						total += Unsafe.Add(ref input, rowOffset + rowLength * y2) - '0' - 1;
						buckets[(bucketPtr + total) % 32].Add((ushort)(element + yMul * y2 - 1));
					}

					total = 0;
					int minY = Math.Max(-3, -y);
					for (int y2 = -1; y2 >= minY; y2--)
					{
						total += Unsafe.Add(ref input, rowOffset + rowLength * y2) - '0' + 1;
						buckets[(bucketPtr + total) % 32].Add((ushort)(element + yMul * y2 - 1));
					}
				}
			}

			bucket.Clear();
			bucketPtr = (bucketPtr + 1) % 32;
			distanceAtBucketStart++;
		}
	}

	public static int SolvePart2(ref byte input, int width, int height)
	{
		int rowLength = width + 1;
		int numStates = width * height * 2;
		int targetState = (height - 1) * width + (width - 1);

		const int xMul = 2;
		int yMul = 2 * height;

		ulong[] seen = new ulong[(numStates - 1) / 64 + 1];
		ref ulong seenRef = ref MemoryMarshal.GetArrayDataReference(seen);

		List<ushort>[] buckets = new List<ushort>[128]; // Only 128 buckets needed to handle all possible moves from the best current state
		for (int i = 0; i < buckets.Length; i++)
			buckets[i] = new List<ushort>(1024);

		int distanceAtBucketStart = width + height - 2;
		int bucketPtr = 0;
		buckets[0].Add(0);
		buckets[0].Add(1);

		while (true)
		{
			List<ushort> bucket = buckets[bucketPtr];

			for (int i = 0; i < bucket.Count; i++)
			{
				ushort element = bucket[i];
				ref ulong seenBitset = ref Unsafe.Add(ref seenRef, (uint)element / 64);
				ulong elementBit = 1UL << element;
				if ((seenBitset & elementBit) != 0)
					continue;
				seenBitset |= elementBit;

				int packedXY = Math.DivRem(element, 2, out int isHorizontal);
				if (packedXY == targetState)
					return distanceAtBucketStart;

				int y = Math.DivRem(packedXY, width, out int x);
				int rowOffset = y * rowLength + x;


				if (isHorizontal == 0)
				{
					if (x < width - 4)
					{
						int total = 0;
						for (int x2 = 1; x2 < 4; x2++)
							total += Unsafe.Add(ref input, rowOffset + x2) - '0' - 1;

						int maxX = Math.Min(11, width - x);
						for (int x2 = 4; x2 < maxX; x2++)
						{
							total += Unsafe.Add(ref input, rowOffset + x2) - '0' - 1;
							buckets[(bucketPtr + total) % 128].Add((ushort)(element + xMul * x2 + 1));
						}
					}

					if (x >= 4)
					{
						int total = 0;
						for (int x2 = -1; x2 >= -3; x2--)
							total += Unsafe.Add(ref input, rowOffset + x2) - '0' + 1;

						int minX = Math.Max(-10, -x);
						for (int x2 = -4; x2 >= minX; x2--)
						{
							total += Unsafe.Add(ref input, rowOffset + x2) - '0' + 1;
							buckets[(bucketPtr + total) % 128].Add((ushort)(element + xMul * x2 + 1));
						}
					}
				}
				else
				{
					if (y < height - 4)
					{
						int total = 0;
						for (int y2 = 1; y2 < 4; y2++)
							total += Unsafe.Add(ref input, rowOffset + rowLength * y2) - '0' - 1;

						int maxY = Math.Min(11, height - y);
						for (int y2 = 4; y2 < maxY; y2++)
						{
							total += Unsafe.Add(ref input, rowOffset + rowLength * y2) - '0' - 1;
							buckets[(bucketPtr + total) % 128].Add((ushort)(element + yMul * y2 - 1));
						}
					}

					if (y >= 4)
					{
						int total = 0;
						for (int y2 = -1; y2 >= -3; y2--)
							total += Unsafe.Add(ref input, rowOffset + rowLength * y2) - '0' + 1;

						int minY = Math.Max(-10, -y);
						for (int y2 = -4; y2 >= minY; y2--)
						{
							total += Unsafe.Add(ref input, rowOffset + rowLength * y2) - '0' + 1;
							buckets[(bucketPtr + total) % 128].Add((ushort)(element + yMul * y2 - 1));
						}
					}
				}
			}

			bucket.Clear();
			bucketPtr = (bucketPtr + 1) % 128;
			distanceAtBucketStart++;
		}
	}
}
