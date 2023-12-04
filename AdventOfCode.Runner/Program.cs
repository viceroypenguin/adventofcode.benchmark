using System.Runtime.CompilerServices;
using AdventOfCode.Runner;
using DocoptNet;
using Spectre.Console;

var runner = new PuzzleRunner();
var console = AnsiConsole.Create(new AnsiConsoleSettings());

if (args.Length > 0)
{
	static int ShowHelp(string help) { Console.WriteLine(help); return 0; }
	static int OnError(string error) { Console.Error.WriteLine(error); return 1; }

	return ProgramArguments.CreateParser()
		.Parse(args) switch
	{
		IArgumentsResult<ProgramArguments> { Arguments: var arguments } => Main(arguments),
		IHelpResult { Help: var help } => ShowHelp(help),
		IInputErrorResult { Error: var error } => OnError(error),
		var result => throw new SwitchExpressionException(result),
	};

	int Main(ProgramArguments arguments)
	{
		var puzzles = runner.GetPuzzles().AsEnumerable();

		if (arguments.OptYear != null)
		{
			puzzles = puzzles.Where(p => p.Year == Convert.ToInt32(arguments.OptYear));
			if (arguments.OptDay != null)
				puzzles = puzzles.Where(p => p.Day == Convert.ToInt32(arguments.OptDay));
		}

		runner.BenchmarkPuzzles(puzzles);

		return 0;
	}
}
else
{
	var font = FigletFont.Default;
	var f = new FigletText(font, "Advent of Code")
	{
		Color = ConsoleColor.Green
	};

	console.Write(f);

	var puzzles = runner.GetPuzzles();

	if (!puzzles.Any(p => p.CodeType != CodeType.Original))
	{
		console.Markup("[red]Could not find any puzzles. Exiting.[/]");
		return 0;
	}

	(var year, puzzles) = PickYear(puzzles);

	console.MarkupLineInterpolated($"Running year [red]{year}[/].");

	puzzles = PickPuzzles(puzzles, year);

	console.MarkupLineInterpolated($"Running puzzle(s) [red]{string.Join(", ", puzzles.Select(x => x.Day))}[/].");

	runner.BenchmarkPuzzles(puzzles);

	return 0;

	(int, IReadOnlyCollection<PuzzleModel>) PickYear(IReadOnlyCollection<PuzzleModel> puzzles)
	{
		var years = puzzles
			.Where(p => p.CodeType != CodeType.Original)
			.Select(x => x.Year).Distinct().OrderBy(x => x)
			.ToList();

		var year = years[^1];

		if (years.Count > 1)
		{
			year = console.Prompt(
				new SelectionPrompt<int>()
					.Title("Which [green]year[/] do you want to execute?")
					.PageSize(20)
					.AddChoices(years));
		}

		return (year, puzzles.Where(x => x.Year == year).ToList());
	}

	IReadOnlyCollection<PuzzleModel> PickPuzzles(IReadOnlyCollection<PuzzleModel> puzzles, int year)
	{
		var selectedDays = console.Prompt(
			new MultiSelectionPrompt<int>()
				.Title("Which [green]year[/] do you want to execute?")
				.PageSize(20)
				.MoreChoicesText("[grey](Move up and down to reveal more days)[/]")
				.InstructionsText(
					"[grey](Press [blue]<space>[/] to toggle a day, " +
					"[green]<enter>[/] to accept)[/]")
				.AddChoiceGroup(
					year,
					puzzles.Select(x => x.Day).Distinct().Order()));

		return puzzles
			.Where(p => selectedDays.Contains(p.Day))
			.OrderBy(p => p.Day)
			.ThenBy(p => (int)p.CodeType)
			.ToList();
	}
}
