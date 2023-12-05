namespace AdventOfCode.Puzzles._2023;

[Puzzle(2023, 03, CodeType.Lisa)]
public partial class Day_03_Lisa : IPuzzle
{
	public (string, string) Solve(PuzzleInput input)
	{
		_schematic = input.Lines.Select(x => x.ToCharArray()).ToArray();

		return (
			SumPartNumbers().ToString(),
			SumGearRatios().ToString());
	}

	private char[][] _schematic = default!;

	private int SumPartNumbers()
	{
		var sum = 0;

		for (var row = 0; row < _schematic.Length; row++)
		{
			for (var column = 0; column < _schematic[row].Length; column++)
			{
				if (!char.IsDigit(_schematic[row][column])) continue;

				if (TryGetPartNumber(row, ref column, out var partNumber)) sum += partNumber;
			}
		}

		return sum;
	}

	private bool TryGetPartNumber(int row, ref int column, out int partNumber)
	{
		var number = _schematic[row][column].ToString();
		partNumber = 0;

		while (column++ < _schematic[row].Length - 1)
		{
			var current = _schematic[row][column];

			if (!char.IsDigit(current)) break;

			number += current;
		}

		return HasAdjacentSymbol(row, column - number.Length, number.Length) && int.TryParse(number, out partNumber);
	}

	private bool HasAdjacentSymbol(int row, int column, int length)
	{
		for (var rowMod = -1; rowMod <= 1; rowMod++)
		{
			for (var columnMod = -1; columnMod <= length; columnMod++)
			{
				if (rowMod == 0 && columnMod >= 0 && columnMod < length) continue;

				var newRow = row + rowMod;
				var newColumn = column + columnMod;

				if (IsWithinBounds(newRow, newColumn) && IsSymbol(_schematic[newRow][newColumn])) return true;
			}
		}

		return false;
	}

	private static bool IsSymbol(char character) => !char.IsDigit(character) && character != '.';

	private int SumGearRatios()
	{
		var sum = 0;

		for (var row = 0; row < _schematic.Length; row++)
		{
			if (!_schematic[row].Contains('*')) continue;
			for (var column = 0; column < _schematic[row].Length; column++)
			{
				var current = _schematic[row][column];
				if (current != '*') continue;

				var neighbours = GetNumberNeighbours(row, column);
				if (neighbours.Length != 2) continue;

				sum += int.Parse(neighbours[0]) * int.Parse(neighbours[1]);
			}
		}

		return sum;
	}

	private string[] GetNumberNeighbours(int row, int column)
	{
		var foundNumbers = new HashSet<string>();

		for (var rowMod = -1; rowMod <= 1; rowMod++)
		{
			for (var columnMod = -1; columnMod <= 1; columnMod++)
			{
				if (rowMod == 0 && columnMod == 0) continue;

				var newRow = row + rowMod;
				var newColumn = column + columnMod;

				if (IsWithinBounds(newRow, newColumn) && char.IsDigit(_schematic[newRow][newColumn]))
				{
					foundNumbers.Add(GetEntireNumber(newRow, newColumn));
				}
			}
		}

		return foundNumbers.ToArray();
	}

	private string GetEntireNumber(int row, int column)
	{
		while (column != 0)
		{
			if (char.IsDigit(_schematic[row][column - 1])) column--;
			else break;
		}

		var number = "" + _schematic[row][column];
		while (column < _schematic[row].Length - 1)
		{
			if (char.IsDigit(_schematic[row][column + 1]))
			{
				column++;
				number += _schematic[row][column];
			}
			else break;
		}


		return number;
	}

	private bool IsWithinBounds(int row, int column) =>
		row >= 0 && row < _schematic.Length && column >= 0 && column < _schematic[row].Length;
}
