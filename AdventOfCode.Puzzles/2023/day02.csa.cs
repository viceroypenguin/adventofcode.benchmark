namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 02, CodeType.Csa)]
public partial class Day_02_Csa : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		var span = input.Span;

		int part1 = 0;
		int part2 = 0;

		int gameId = 1;
		while (span.Length > 1)
		{
			int maxR = 0;
			int maxB = 0;
			int maxG = 0;

			// skip the "Game 1" part
			span = span.Slice("Game ".Length + (gameId < 10 ? 1 : (gameId < 100 ? 2 : 3)));

			while (span[0] != '\n')
			{
				// Parse integer
				byte c;
				int amt = span[2] - '0'; // look at span[2] to skip ": " or ", " or "; "
				int i = 3;
				while ((c = span[i++]) != ' ')
					amt = 10 * amt + (c - '0');

				switch (span[i])
				{
					case (byte)'r':
						maxR = Math.Max(maxR, amt);
						span = span.Slice(i + "red".Length);
						break;
					case (byte)'g':
						maxG = Math.Max(maxG, amt);
						span = span.Slice(i + "green".Length);
						break;
					case (byte)'b':
						maxB = Math.Max(maxB, amt);
						span = span.Slice(i + "blue".Length);
						break;
				}
			}

			if (maxR <= 12 && maxG <= 13 && maxB <= 14)
				part1 += gameId;

			part2 += maxR * maxB * maxG;

			span = span.Slice(1);
			gameId++;
		}

		return (part1.ToString(), part2.ToString());
	}
}
