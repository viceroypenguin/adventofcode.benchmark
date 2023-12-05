namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 02, CodeType.Lisa)]
public partial class Day_02_Lisa : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		var games = new Cube[input.Lines.Length][];
		var gameCounter = 0;

		foreach (var line in input.Text.AsSpan().EnumerateLines())
		{
			if (line.Length == 0)
				break;

			games[gameCounter++] = ParseGame(line);
		}

		return (
			SumPossibleGames(games).ToString(),
			SumCubePower(games).ToString());
	}

	private const int Red = 12;
	private const int Green = 13;
	private const int Blue = 14;

	private static int SumPossibleGames(Cube[][] games)
	{
		var sum = 0;
		for (var game = 0; game < games.Length; game++)
		{
			if (IsValid(games[game]))
			{
				sum += game + 1;
			}
		}

		return sum;
	}

	private static bool IsValid(Cube[] cubes)
	{
		for (var i = 0; i < cubes.Length; i++)
		{
			var current = cubes[i];
			switch (current.Color)
			{
				case 'r' when current.Amount > Red:
					return false;
				case 'g' when current.Amount > Green:
					return false;
				case 'b' when current.Amount > Blue:
					return false;
			}
		}

		return true;
	}

	private static int SumCubePower(Cube[][] games)
	{
		var sum = 0;
		for (var game = 0; game < games.Length; game++)
		{
			var red = 0;
			var green = 0;
			var blue = 0;

			for (var cube = 0; cube < games[game].Length; cube++)
			{
				var current = games[game][cube];
				switch (current.Color)
				{
					case 'r' when current.Amount > red:
						red = current.Amount;
						break;
					case 'g' when current.Amount > green:
						green = current.Amount;
						break;
					case 'b' when current.Amount > blue:
						blue = current.Amount;
						break;
				}
			}

			sum += red * green * blue;
		}

		return sum;
	}

	private static Cube[] ParseGame(ReadOnlySpan<char> line)
	{
		var cubeCounter = 0;
		var game = new Cube[(line.Count(" ") - 1) / 2];
		var workingLine = line[line.IndexOf(":")..];
		while (!workingLine.IsEmpty)
		{
			var amount = 0;
			var offset = 2;
			while (workingLine[offset] != ' ')
			{
				amount = amount * 10 + (workingLine[offset] - '0');
				offset++;
			}

			var letter = workingLine[offset + 1];
			offset = letter switch
			{
				'r' => offset + 4,
				'g' => offset + 6,
				'b' => offset + 5
			};

			game[cubeCounter++] = new Cube(letter, amount);
			workingLine = workingLine[offset..];
		}

		return game;
	}

	private record struct Cube(char Color, int Amount);
}
