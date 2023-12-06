## adventofcode.benchmark

This is a unified repo for benchmarking optimized solutions for various adventurers.

### Results

#### Year 2023

All benchmarks for 2023 are done with the following summary:
```
// * Summary *

BenchmarkDotNet v0.13.10, Windows 11 (10.0.22621.2715/22H2/2022Update/SunValley2)
12th Gen Intel Core i7-12700H, 1 CPU, 20 logical and 14 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2 [AttachedDebugger]
  DefaultJob : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
```

<details>
<summary>Day 1</summary>

| Owner           | Mean        | Error    | StdDev   | Allocated |
|---------------- |------------:|---------:|---------:|----------:|
| @ClxS           | 1,005.52 us | 7.109 us | 6.650 us |  414161 B |
| @CameronAavik   |    17.50 us | 0.207 us | 0.194 us |      64 B |
| @TwilightVanish |   109.03 us | 1.990 us | 1.862 us |  384184 B |
| @viceroypenguin |    25.41 us | 0.311 us | 0.291 us |      64 B |

</details>

### How to Run

This repo does not download puzzle files automatically. It is generally assumed that consumers have already downloaded files for their own daily competition.

Instead, files must be placed under `AdventOfCode.Runner/Input/<year>/<day>.input.txt`. They are expected to be pure files, which are ASCII and LF. 
CRLF will likely break some optimized programs.

Once files are placed, you may simply run the program, and it will provide options for which puzzle(s) to benchmark. You can also provide year and day
information via command-line parameters, e.g. `-y 2023 -d 1`.

### How to contribute

Add your benchmark code, along with your name/username in `AdventOfCode.Common.Attributes.CodeType`, and submit a PR.
