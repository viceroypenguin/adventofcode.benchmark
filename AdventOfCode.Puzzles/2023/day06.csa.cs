namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 06, CodeType.Csa)]
public sealed partial class Day_06_Csa : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		var span = input.Span;

		int timesLineEnd = span.IndexOf((byte)'\n');
		ReadOnlySpan<byte> timesLine = span.Slice(0, timesLineEnd);
		ReadOnlySpan<byte> distanceLine = span.Slice(timesLineEnd + 1, span.Length - timesLineEnd - 2);

		long part1 = 1;
		long part2Time = 0;
		long part2Dist = 0;
		// distance and time are right aligned columns, so just iterate 
		while (timesLine.Length > 0)
		{
			long time = ParseNum(ref timesLine, ref part2Time);
			long distance = ParseNum(ref distanceLine, ref part2Dist);
			part1 *= NumWaysToWin(time, distance);
		}

		long part2 = NumWaysToWin(part2Time, part2Dist);

		return (part1.ToString(), part2.ToString());
	}

	private static long ParseNum(ref ReadOnlySpan<byte> line, ref long part2Var)
	{
		byte c;
		int nextStart = line.IndexOfAnyInRange((byte)'0', (byte)'9');
		line = line.Slice(nextStart);
		long time = 0;
		int i = 0;
		while (i < line.Length && (c = line[i]) != ' ')
		{
			time = time * 10 + c - '0';
			part2Var = part2Var * 10 + c - '0';
			i++;
		}

		line = line.Slice(i);
		return time;
	}

	static long NumWaysToWin(long time, long distance)
	{
		// solve for distance = (time - x) * x
		// x = (time +- sqrt(time^2 - 4 * distance)) / 2
		// time^2 overflows the long on part 2, so we can rewrite it as follows:
		// x = (time +- sqrt(time - 2 * sqrt(distance)) * sqrt(time + 2 * sqrt(distance))) / 2

		double sqrtDist = Math.Sqrt(distance);
		double sqrt = Math.Sqrt(time - 2 * sqrtDist) * Math.Sqrt(time + 2 * sqrtDist);
		long low = Convert.ToInt64(Math.Ceiling((time - sqrt) / 2));
		long high = Convert.ToInt64(Math.Floor((time + sqrt) / 2));

		// handle ties or precision issues

		if ((time - low) * low <= distance)
			low++;

		if ((time - high) * high <= distance)
			high--;

		return high - low + 1;
	}
}
