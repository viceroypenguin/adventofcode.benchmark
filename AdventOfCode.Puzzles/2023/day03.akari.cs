using CommunityToolkit.HighPerformance;
using System.Buffers;

namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 03, CodeType.Akari)]
public partial class Day_03_Akari : IPuzzle
{
	private static readonly Vector2[] adjacent = new Vector2[]
	{
		new Vector2(0, 1),
		new Vector2(1, 0),
		new Vector2(0, -1),
		new Vector2(-1, 0),
		new Vector2(-1, -1),
		new Vector2(1, 1),
		new Vector2(-1, 1),
		new Vector2(1, -1),
	};

	public (string, string) Solve(PuzzleInput input)
	{
		return (
			InternalPart1(input.Text.ToCharArray()).ToString(),
			InternalPart2(input.Text.ToCharArray()).ToString());
	}

	private static readonly SearchValues<char> Symbols = SearchValues.Create("0123456789.");

	private static ulong InternalPart1(char[] input)
	{
		ulong sum = 0;
		int stride = Array.IndexOf(input, '\n');
		var mutableMap = new Span2D<char>(input, 0, input.Length / (stride + 1), stride, 1);
		for (var y = 0; y < mutableMap.Height; y++)
		{
			for (var x = 0; x < mutableMap.Width; x++)
			{
				if (char.IsDigit(mutableMap[y, x]) || mutableMap[y, x] == '.')
				{
					continue;
				}

				foreach (Vector2 direction in adjacent)
				{
					if (y + direction.Y < 0 || y + direction.Y >= mutableMap.Height)
						continue;
					if (x + direction.X < 0 || x + direction.X >= mutableMap.Width)
						continue;

					if (!char.IsDigit(mutableMap[y + direction.Y, x + direction.X]))
					{
						continue;
					}

					SweepAndReplace(mutableMap, x + direction.X, y + direction.Y, out ulong number);
					sum += number;
				}
			}
		}

		return sum;
	}

	private static ulong InternalPart2(char[] input)
	{
		ulong sum = 0;

		int stride = Array.IndexOf(input, '\n');
		var mutableMap = new Span2D<char>(input, 0, input.Length / (stride + 1), stride, 1);
		for (var y = 0; y < mutableMap.Height; y++)
		{
			Span<char> line = mutableMap.GetRowSpan(y);

			var x = 0;
			for (; x < mutableMap.Width; x++)
			{
				int advance = line[x..].IndexOfAnyExcept(Symbols);
				if (advance == -1)
				{
					break;
				}

				x += advance;

				if (mutableMap[y, x] != '*')
				{
					continue;
				}

				var adjacentCount = 0;
				ulong sumOfAdjacent = 1;
				foreach (Vector2 direction in adjacent)
				{
					if (y + direction.Y < 0 || y + direction.Y >= mutableMap.Height)
						continue;
					if (x + direction.X < 0 || x + direction.X >= mutableMap.Width)
						continue;

					if (!char.IsDigit(mutableMap[y + direction.Y, x + direction.X]))
					{
						continue;
					}

					SweepAndReplace(mutableMap, x + direction.X, y + direction.Y, out ulong number);
					adjacentCount++;
					sumOfAdjacent *= number;
				}

				if (adjacentCount == 2)
				{
					sum += sumOfAdjacent;
				}
			}
		}

		return sum;
	}

	private static void SweepAndReplace(Span2D<char> mutableMap, int x, int y, out ulong output)
	{
		int xStart = x;
		while (xStart > 0 && char.IsDigit(mutableMap[y, xStart - 1]))
		{
			xStart--;
		}

		int xEnd = x;
		while (xEnd < mutableMap.Width - 1 && char.IsDigit(mutableMap[y, xEnd + 1]))
		{
			xEnd++;
		}

		Span<char> number = mutableMap.GetRowSpan(y).Slice(xStart, xEnd - xStart + 1);
		output = ulong.Parse(number);
		for (int i = xStart; i <= xEnd; i++)
		{
			mutableMap[y, i] = '.';
		}
	}

	private record struct Vector2(int X, int Y);
}
