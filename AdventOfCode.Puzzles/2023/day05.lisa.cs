namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 05, CodeType.Lisa)]
public partial class Day_51_Lisa : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		var almanac = ParseAlmanac(input.Lines);
		return (
			GetLowestLocation(almanac).ToString(),
			GetRangeLowestLocation(almanac).ToString());
	}

	private Almanac ParseAlmanac(string[] lines)
	{
		var seeds = ParseSeeds(lines);

		return new Almanac(seeds, GetSeedRanges(seeds), ParseMaps(lines));
	}

	private static long[] ParseSeeds(string[] lines)
	{
		return lines[0].Split(": ")[1].Split(' ').Select(long.Parse).ToArray();
	}

	private MapRange[][] ParseMaps(string[] lines)
	{
		var maps = new List<MapRange[]>();
		var currentMap = new List<MapRange>();
		for (var i = 3; i < lines.Length; i++)
		{
			var line = lines[i];
			if (line == "")
			{
				maps.Add(currentMap.ToArray());
				currentMap.Clear();
				i++;
				continue;
			}

			var parsed = line.Split(' ').Select(long.Parse).ToArray();
			currentMap.Add(new MapRange(parsed[1], parsed[1] + parsed[2] - 1, parsed[0] - parsed[1]));
		}
		maps.Add(currentMap.ToArray());

		return maps.ToArray();
	}

	private static long GetLowestLocation(Almanac almanac)
	{
		for (var m = 0; m < almanac.Maps.Length; m++)
		{
			var map = almanac.Maps[m];
			for (var s = 0; s < almanac.IndividualSeeds.Length; s++)
			{
				var seed = almanac.IndividualSeeds[s];
				for (var r = 0; r < map.Length; r++)
				{
					var mapRange = map[r];
					if (seed < mapRange.SourceStart || seed > mapRange.SourceEnd) continue;

					almanac.IndividualSeeds[s] += mapRange.Modifier;
					break;
				}
			}
		}

		return almanac.IndividualSeeds.Min();
	}

	private long GetRangeLowestLocation(Almanac almanac)
	{
		var seedRangesQueue = new Queue<SeedRange>(almanac.SeedRanges);
		var splinters = new List<SeedRange>();

		for (var m = 0; m < almanac.Maps.Length; m++)
		{
			ProcessSeedRanges(almanac.Maps[m], seedRangesQueue, splinters);
			seedRangesQueue = new Queue<SeedRange>(splinters);
			splinters.Clear();
		}

		return seedRangesQueue.Min(x => x.Start);
	}

	private void ProcessSeedRanges(MapRange[] map, Queue<SeedRange> seedRangesQueue, List<SeedRange> formedSplinters)
	{
		while (seedRangesQueue.TryDequeue(out var seedRange))
		{
			RecursiveProcessSeedRange(map, seedRange, formedSplinters);
		}
	}

	private void RecursiveProcessSeedRange(MapRange[] map, SeedRange seedRange, List<SeedRange> formedSplinters)
	{
		if (TryProcessSeedRange(map, seedRange, formedSplinters)) return;
		formedSplinters.Add(seedRange);
	}

	private bool TryProcessSeedRange(MapRange[] map, SeedRange seedRange, List<SeedRange> formedSplinters)
	{
		for (var r = 0; r < map.Length; r++)
		{
			var mapRange = map[r];
			if (seedRange.Start >= mapRange.SourceStart && seedRange.End <= mapRange.SourceEnd)
			{
				seedRange.Modify(mapRange.Modifier);
				formedSplinters.Add(seedRange);
				return true;
			}

			if (seedRange.Start >= mapRange.SourceStart && seedRange.Start <= mapRange.SourceEnd)
			{
				var splinter = seedRange with { End = mapRange.SourceEnd };
				splinter.Modify(mapRange.Modifier);
				formedSplinters.Add(splinter);

				RecursiveProcessSeedRange(map, seedRange with { Start = mapRange.SourceEnd + 1 }, formedSplinters);

				return true;
			}

			if (seedRange.End >= mapRange.SourceStart && seedRange.End <= mapRange.SourceEnd)
			{
				var splinter = seedRange with { Start = mapRange.SourceStart };
				splinter.Modify(mapRange.Modifier);
				formedSplinters.Add(splinter);

				RecursiveProcessSeedRange(map, seedRange with { End = mapRange.SourceStart - 1 }, formedSplinters);

				return true;
			}
		}

		return false;
	}

	private static SeedRange[] GetSeedRanges(long[] seeds)
	{
		return seeds.Chunk(2).Select(x => new SeedRange(x.First(), x.First() + x.Last() - 1)).ToArray();
	}

	private struct SeedRange
	{
		public long Start { get; set; }
		public long End { get; set; }

		public SeedRange(long start, long end)
		{
			Start = start;
			End = end;
		}

		public void Modify(long modifier)
		{
			Start += modifier;
			End += modifier;
		}
	}
	private record struct Almanac(long[] IndividualSeeds, SeedRange[] SeedRanges, MapRange[][] Maps);
	private record struct MapRange(long SourceStart, long SourceEnd, long Modifier);
}
