using System.Runtime.CompilerServices;

namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 04, CodeType.Akari)]
public partial class Day_04_Akari : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		return (
			InternalPart1(input.Lines).ToString(),
			InternalPart2(input.Lines).ToString());
	}

	private static long InternalPart1(string[] lines)
	{
		long sum = 0;

		Span<byte> count = stackalloc byte[100];
		foreach (string line in lines)
		{
			if (line.Length == 0)
			{
				continue;
			}

			MatchCollection matches = Regex.Matches(line, @"(\d+)");
			for (var i = 11; i < matches.Count; ++i)
			{
				string value = matches[i].Value;
				int index = int.Parse(value);
				count[index]++;
			}

			var winCount = 0;
			for (var i = 1; i <= 10; ++i)
			{
				string value = matches[i].Value;
				int index = int.Parse(value);
				if (count[index] > 0)
				{
					winCount++;
				}
			}

			// Zero out the count buffer
			Unsafe.InitBlock(ref count[0], 0, 100);

			if (winCount > 0)
			{
				sum += 1 << (winCount - 1);
			}
		}

		return sum;
	}

	private static long InternalPart2(string[] lines)
	{
		long total = 0;

		Span<byte> rowMatchCount = stackalloc byte[100];
		Span<int> multiplier = stackalloc int[255];
		for (int i = 0; i < 255; i++)
		{
			multiplier[i] = 1;
		}

		for (var gameIdx = 0; gameIdx < lines.Length; gameIdx++)
		{
			string line = lines[gameIdx];
			if (line.Length == 0)
			{
				continue;
			}

			total += multiplier[gameIdx];
			MatchCollection matches = Regex.Matches(line, @"(\d+)");
			for (var i = 11; i < matches.Count; ++i)
			{
				string value = matches[i].Value;
				int index = int.Parse(value);
				rowMatchCount[index]++;
			}

			var winCount = 0;
			for (var i = 1; i <= 10; ++i)
			{
				string value = matches[i].Value;
				int index = int.Parse(value);
				if (rowMatchCount[index] > 0)
				{
					winCount++;
				}
			}

			// Zero out the count buffer
			Unsafe.InitBlock(ref rowMatchCount[0], 0, 100);

			for (int i = gameIdx + 1; i < gameIdx + 1 + winCount; ++i)
			{
				multiplier[i] += multiplier[gameIdx];
			}
		}

		return total;
	}
}
