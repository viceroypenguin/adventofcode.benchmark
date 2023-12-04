namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 01, CodeType.Lisa)]
public partial class Day_01_Lisa : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		return (GetSum(input.Text).ToString(), FindActualCalibrationValues(input.Text).ToString());
	}

	private static int FindActualCalibrationValues(string input)
	{
		string[] numbers = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
		string[] mask = { "o1e", "t2o", "t3e", "f4r", "f5e", "s6x", "s7n", "e8t", "n9e" };

		var masked = input;
		for (var i = 0; i < numbers.Length; i++)
		{
			masked = masked.Replace(numbers[i], mask[i]);
		}

		return GetSum(masked);
	}

	private static int GetSum(string input)
	{
		var sum = 0;

		foreach (var line in input.AsSpan().EnumerateLines())
		{
			if (line.Length == 0)
				break;

			sum += (line[line.IndexOfAnyInRange('0', '9')] - '0') * 10;
			sum += line[line.LastIndexOfAnyInRange('0', '9')] - '0';
		}

		return sum;
	}
}
