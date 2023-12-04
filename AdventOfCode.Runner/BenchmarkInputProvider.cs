namespace AdventOfCode.Runner;

public static class BenchmarkInputProvider
{
	public static PuzzleInput GetRawInput(int year, int day)
	{
		var inputFile = @$"Inputs\{year}\day{day:00}.input.txt";
		if (!Directory.Exists("Inputs"))
		{
			inputFile = @"..\..\..\..\..\..\..\" + inputFile;
		}

		return new(
			File.ReadAllBytes(inputFile),
			File.ReadAllText(inputFile),
			File.ReadAllLines(inputFile));
	}
}
