using System.Runtime.CompilerServices;

namespace AdventOfCode.Common.Utility;

public static class CustomParser
{
	public static int ParseInt(string input)
	{
		var value = 0;
		for (var i = 0; i < input.Length; i++) value = value * 10 + (input[i] - '0');
		return value;
	}

	public static int ParseInt(ReadOnlySpan<char> input)
	{
		var value = 0;
		for (var i = 0; i < input.Length; i++) value = value * 10 + (input[i] - '0');
		return value;
	}

	public static int CheckedParseInt(string input)
	{
		var value = 0;
		for (var i = 0; i < input.Length; i++)
		{
			if (input[i] is < '0' or > '9') break;
			value = value * 10 + (input[i] - '0');
		}

		return value;
	}

	public static int GreedyParseInt(ReadOnlySpan<char> input)
	{
		var value = 0;
		for (var i = 0; i < input.Length; i++)
		{
			if (input[i] is < '0' or > '9') continue;
			value = value * 10 + (input[i] - '0');
		}

		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
	public static int SignedParseInt(ReadOnlySpan<char> input, int offset)
	{
		var negative = false;
		var start = offset;

		if (input[offset] == '-')
		{
			negative = true;
			start++;
		}

		var value = 0;
		for (var i = start; i < input.Length; i++)
		{
			value = value * 10 + (input[i] - '0');
		}

		return negative ? -value : value;
	}
}
