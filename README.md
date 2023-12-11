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

<details>
<summary>Day 2</summary>

| Owner           | Mean       | Error     | StdDev    | Allocated |
|---------------- |-----------:|----------:|----------:|----------:|
| @ClxS           | 142.808 us | 1.6774 us | 1.4869 us |  461090 B |
| @CameronAavik   |   2.536 us | 0.0093 us | 0.0082 us |      64 B |
| @TwilightVanish |   7.439 us | 0.0516 us | 0.0483 us |   12120 B |
| @viceroypenguin |   3.704 us | 0.0316 us | 0.0264 us |      64 B |

</details>

<details>
<summary>Day 3</summary>

| Owner           | Mean       | Error     | StdDev    | Allocated |
|---------------- |-----------:|----------:|----------:|----------:|
| @ClxS           |  81.832 us | 0.4992 us | 0.4670 us |   79089 B |
| @CameronAavik   |   9.042 us | 0.1156 us | 0.0965 us |     664 B |
| @TwilightVanish | 165.686 us | 2.0577 us | 1.9248 us |  436778 B |
| @viceroypenguin |  13.345 us | 0.2646 us | 0.3047 us |      80 B |

</details>

<details>
<summary>Day 4</summary>

| Owner           | Mean        | Error     | StdDev    | Allocated |
|---------------- |------------:|----------:|----------:|----------:|
| @ClxS           | 1,481.27 us | 14.915 us | 13.952 us | 4736212 B |
| @CameronAavik   |    10.26 us |  0.063 us |  0.059 us |     944 B |
| @TwilightVanish |    44.46 us |  0.327 us |  0.289 us |   45672 B |
| @viceroypenguin |    12.74 us |  0.054 us |  0.050 us |      72 B |

</details>

<details>
<summary>Day 5</summary>

| Owner           | Mean      | Error     | StdDev    | Allocated |
|---------------- |----------:|----------:|----------:|----------:|
| @CameronAavik   |  7.428 us | 0.0442 us | 0.0345 us |    7856 B |
| @TwilightVanish | 52.279 us | 0.4968 us | 0.4647 us |   91832 B |
| @viceroypenguin | 10.091 us | 0.0833 us | 0.0779 us |      80 B |

</details>

<details>
<summary>Day 6</summary>

| Owner           | Mean     | Error    | StdDev   | Allocated |
|---------------- |---------:|---------:|---------:|----------:|
| @CameronAavik   | 97.55 ns | 0.863 ns | 0.765 ns |      80 B |
| @viceroypenguin | 84.79 ns | 0.785 ns | 0.734 ns |      80 B |

</details>

<details>
<summary>Day 7</summary>

| Owner           | Mean     | Error    | StdDev   | Allocated |
|---------------- |---------:|---------:|---------:|----------:|
| @CameronAavik   | 78.22 us | 0.669 us | 0.626 us |   16129 B |
| @viceroypenguin | 83.00 us | 0.986 us | 0.922 us |      81 B |

</details>

<details>
<summary>Day 8</summary>

| Owner           | Mean     | Error   | StdDev  | Allocated |
|---------------- |---------:|--------:|--------:|----------:|
| @CameronAavik   | 147.3 us | 1.12 us | 1.05 us |   21426 B |
| @viceroypenguin | 291.1 us | 1.37 us | 1.21 us |      88 B |

</details>

<details>
<summary>Day 9</summary>

| Owner           | Mean     | Error    | StdDev   | Allocated |
|---------------- |---------:|---------:|---------:|----------:|
| @CameronAavik   | 16.09 us | 0.140 us | 0.124 us |      80 B |
| @viceroypenguin | 28.87 us | 0.175 us | 0.164 us |      80 B |

</details>

<details>
<summary>Day 10</summary>

| Owner           | Mean      | Error    | StdDev   | Allocated |
|---------------- |----------:|---------:|---------:|----------:|
| @CameronAavik   |  36.99 us | 0.152 us | 0.135 us |    5040 B |
| @viceroypenguin | 145.15 us | 0.533 us | 0.499 us |      64 B |

</details>

<details>
<summary>Day 11</summary>

| Owner           | Mean       | Error     | StdDev    | Allocated |
|---------------- |-----------:|----------:|----------:|----------:|
| @CameronAavik   |   1.883 us | 0.0120 us | 0.0113 us |     672 B |
| @viceroypenguin |   1.877 us | 0.0119 us | 0.0105 us |      88 B |

</details>

### How to Run

This repo does not download puzzle files automatically. It is generally assumed that consumers have already downloaded files for their own daily competition.

Instead, files must be placed under `AdventOfCode.Runner/Input/<year>/<day>.input.txt`. They are expected to be pure files, which are ASCII and LF. 
CRLF will likely break some optimized programs.

Once files are placed, you may simply run the program, and it will provide options for which puzzle(s) to benchmark. You can also provide year and day
information via command-line parameters, e.g. `-y 2023 -d 1`.

### How to contribute

Add your benchmark code, along with your name/username in `AdventOfCode.Common.Attributes.CodeType`, and submit a PR.
