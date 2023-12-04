using AdventOfCode.Common.Utility;

namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 04, CodeType.Lisa)]
public partial class Day_04_Lisa : IPuzzle
{
	private Scratcher[] _scratchers = default!;

	public (string, string) Solve(PuzzleInput input)
	{
		_scratchers = ParseScratchers(input);

		var part1 = GetScratcherPoints();
		var part2 = GetScratcherCount();

		return (part1.ToString(), part2.ToString());
	}

	private int GetScratcherPoints()
	{
		var points = 0;

		for (var i = 0; i < _scratchers.Length; i++)
		{
			var scratcher = _scratchers[i];
			var currentPoints = 0;

			for (var j = 0; j < scratcher.ScratchedNumbers.Length; j++)
			{
				if (!scratcher.WinningNumbers.Contains(scratcher.ScratchedNumbers[j])) continue;
				if (currentPoints == 0) currentPoints = 1;
				else currentPoints *= 2;
			}

			points += currentPoints;
		}


		return points;
	}

	private int GetScratcherCount()
	{
		var scratcherCount = 0;

		for (var i = 0; i < _scratchers.Length; i++)
		{
			var scratcher = _scratchers[i];
			var currentWins = 0;

			for (var j = 0; j < scratcher.ScratchedNumbers.Length; j++)
			{
				if (!scratcher.WinningNumbers.Contains(scratcher.ScratchedNumbers[j])) continue;
				currentWins++;
				_scratchers[i + currentWins].Count += scratcher.Count;
			}

			scratcherCount += scratcher.Count;
		}

		return scratcherCount;
	}

	private static Scratcher[] ParseScratchers(PuzzleInput input)
	{
		var scratchers = new Scratcher[input.Lines.Length];

		var index = 0;
		foreach (var line in input.Text.AsSpan().EnumerateLines())
		{
			if (line.Length == 0)
				break;

			var scratched = new int[10];
			var winning = new int[25];

			ParseWithOffset(scratched, line, 10);
			ParseWithOffset(winning, line, 42);

			scratchers[index] = new Scratcher(scratched, winning);
			index++;
		}

		return scratchers;
	}

	private static void ParseWithOffset(int[] targetArray, ReadOnlySpan<char> line, int startIndex)
	{
		for (var i = 0; i < targetArray.Length; i++)
		{
			targetArray[i] = CustomParser.GreedyParseInt(line.Slice(startIndex + 3 * i, 2));
		}
	}

	private record struct Scratcher(int[] ScratchedNumbers, int[] WinningNumbers, int Count = 1);
}
