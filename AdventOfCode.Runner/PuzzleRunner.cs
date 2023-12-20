using System.Collections.Immutable;
using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace AdventOfCode.Runner;

public class PuzzleRunner
{
	private readonly IReadOnlyList<PuzzleModel> _puzzles;
	private readonly MethodInfo _runMethod;
	private readonly Type _benchmarkClass;

	private static readonly Assembly[] assemblies =
	[
		Assembly.GetAssembly(typeof(Puzzles._2015.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2016.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2017.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2018.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2019.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2020.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2021.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2022.Day_01_Original))!,
		Assembly.GetAssembly(typeof(Puzzles._2023.Day_01_Original))!,
	];

	public PuzzleRunner()
	{
		_puzzles = GetAllPuzzles();
		_runMethod = GetType().GetMethod(nameof(RunPuzzle), BindingFlags.Static | BindingFlags.NonPublic)!;
		_benchmarkClass = typeof(PuzzleBenchmarkRunner<>);
	}

	public IReadOnlyCollection<PuzzleModel> GetPuzzles() => _puzzles;

	public IEnumerable<PuzzleResult> RunPuzzles(IEnumerable<PuzzleModel> puzzles) =>
		puzzles
			.Select(puzzle =>
			{
				var method = _runMethod.MakeGenericMethod(puzzle.PuzzleType);
				return (PuzzleResult)method.Invoke(null, new object[] { puzzle })!;
			});

	public void BenchmarkPuzzles(IEnumerable<PuzzleModel> puzzles)
	{
		var list = puzzles.ToList();

		if (!ValidatePuzzles(list, out var message))
		{
			Spectre.Console.AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
			return;
		}

		var types = list
			.Where(p => p.CodeType != CodeType.Original)
			.Select(p => _benchmarkClass.MakeGenericType(p.PuzzleType))
			.ToArray();

		_ = BenchmarkSwitcher.FromTypes(types)
			.RunAllJoined(
				DefaultConfig.Instance
					.AddColumn(new CategoryColumn(0, "Year"))
					.AddColumn(new CategoryColumn(1, "Day"))
					.AddColumn(new CategoryColumn(2, "Owner"))
					.HideColumns("Type", "Method")
					.WithCategoryDiscoverer(new CategoryDiscoverer())
					.WithOrderer(new TypeOrderer()));
	}

	private sealed class TypeOrderer : DefaultOrderer
	{
		public override IEnumerable<BenchmarkCase> GetSummaryOrder(ImmutableArray<BenchmarkCase> benchmarksCases, Summary summary) =>
			benchmarksCases
				.OrderBy(c => c.Descriptor.Categories[0])
				.ThenBy(c => c.Descriptor.Categories[1])
				.ThenBy(c => c.Descriptor.Categories[2]);
	}

	private sealed class CategoryDiscoverer : DefaultCategoryDiscoverer
	{
		public override string[] GetCategories(MethodInfo method)
		{
			var benchmarkClass = method.DeclaringType!;
			var @class = benchmarkClass.GetGenericArguments()[0];
			var attr = @class.GetCustomAttribute<PuzzleAttribute>()!;

			return
			[
				attr.Year.ToString(),
				attr.Day.ToString(),
				attr.CodeType.ToString(),
			];
		}
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0032:Use auto property", Justification = "")]
	private sealed class CategoryColumn(int categoryId, string name) : IColumn
	{
		public string Id => name;
		public string ColumnName => name;
		private readonly int _index = categoryId;

		public bool AlwaysShow => true;
		public ColumnCategory Category => ColumnCategory.Job;
		public int PriorityInCategory => _index;
		public bool IsNumeric => false;
		public UnitType UnitType => UnitType.Dimensionless;
		public string Legend => $"Puzzle Information: `{name}`";
		public bool IsAvailable(Summary summary) => true;
		public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

		public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) =>
			GetValue(summary, benchmarkCase);


		public string GetValue(Summary summary, BenchmarkCase benchmarkCase) =>
			benchmarkCase.Descriptor
				.Categories[_index];
	}

	private bool ValidatePuzzles(List<PuzzleModel> puzzles, out string message)
	{
		foreach (var g in puzzles.GroupBy(p => new { p.Year, p.Day, }))
		{
			var original = g.FirstOrDefault(p => p.CodeType == CodeType.Original);
			if (original == default)
			{
				message = $"(Year: {g.Key.Year}, Day: {g.Key.Day}) Missing `Original` version of puzzle for validation.";
				return false;
			}

			var valid = RunPuzzle(original);

			foreach (var p in g.Where(p => p.CodeType != CodeType.Original))
			{
				var q = RunPuzzle(p);
				if (q != valid)
				{
					message = $"(Year: {p.Year}, Day: {p.Day}) Puzzle `{p.CodeType}` returns a different value than the `Original`.";
					return false;
				}
			}
		}

		message = string.Empty;
		return true;
	}

	private static List<PuzzleModel> GetAllPuzzles()
	{
		return assemblies
			.SelectMany(
				assembly => assembly.GetTypes(),
				(assembly, type) => new
				{
					Type = type,
					PuzzleAttribute = type.GetCustomAttribute<PuzzleAttribute>()!,
				})
			.Where(x => x.PuzzleAttribute != null)
			.Select(x => new PuzzleModel(
				x.PuzzleAttribute.Name,
				x.PuzzleAttribute.Year,
				x.PuzzleAttribute.Day,
				x.PuzzleAttribute.CodeType,
				x.Type))
			.ToList();
	}

	private (string part1, string part2) RunPuzzle(PuzzleModel puzzle)
	{
		var method = _runMethod.MakeGenericMethod(puzzle.PuzzleType);
		return ((string, string))method.Invoke(null, new object[] { puzzle })!;
	}

	private static (string part1, string part2) RunPuzzle<TPuzzle>(PuzzleModel puzzleInfo)
		where TPuzzle : IPuzzle, new()
	{
		var puzzle = new TPuzzle();

		var rawInput = BenchmarkInputProvider
			.GetRawInput(puzzleInfo.Year, puzzleInfo.Day);

		return puzzle.Solve(rawInput);
	}
}
