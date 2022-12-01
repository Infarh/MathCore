using BenchmarkDotNet.Running;

using Benchmarks;

// https://benchmarkdotnet.org/articles/features/parameterization.html

//_ = BenchmarkRunner.Run<Polynoms>();
//_ = BenchmarkRunner.Run<StringSplitTest>();
//_ = BenchmarkRunner.Run<StringPtrParseDoubleTest>();
//_ = BenchmarkRunner.Run<SwapTest>();
_ = BenchmarkRunner.Run<PowerIterationTest>();
//_ = BenchmarkRunner.Run<ParseDoubleTest>();
//_ = BenchmarkRunner.Run<ParseIntTest>();
//_ = BenchmarkRunner.Run<CortegeCtorTest>();