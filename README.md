## adventofcode.benchmark

This is a unified repo for benchmarking optimized solutions for various adventurers.

### How to Run

This repo does not download puzzle files automatically. It is generally assumed that consumers have already downloaded files for their own daily competition.

Instead, files must be placed under `AdventOfCode.Runner/Input/<year>/<day>.input.txt`. They are expected to be pure files, which are ASCII and LF. 
CRLF will likely break some optimized programs.

Once files are placed, you may simply run the program, and it will provide options for which puzzle(s) to benchmark. You can also provide year and day
information via command-line parameters, e.g. `-y 2023 -d 1`.

### How to contribute

Add your benchmark code, along with your name/username in `AdventOfCode.Common.Attributes.CodeType`, and submit a PR.
