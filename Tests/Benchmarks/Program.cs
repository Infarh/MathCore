using BenchmarkDotNet.Configs;

using Benchmarks;
using Benchmarks.Vectorization;


// https://benchmarkdotnet.org/articles/features/parameterization.html

//_ = BenchmarkRunner.Run<Polynoms>();
//_ = BenchmarkRunner.Run<StringSplitTest>();
//_ = BenchmarkRunner.Run<StringPtrParseDoubleTest>();
//_ = BenchmarkRunner.Run<SwapTest>();
//_ = BenchmarkRunner.Run<PowerIterationTest>();
//_ = BenchmarkRunner.Run<ParseDoubleTest>();
//_ = BenchmarkRunner.Run<ParseIntTest>();
//_ = BenchmarkRunner.Run<CortegeCtorTest>();
//_ = BenchmarkRunner.Run<MD5Test>();
//_ = BenchmarkRunner.Run<SHA256Test>();
//_ = BenchmarkRunner.Run<SHA512Test>();
//_ = BenchmarkRunner.Run<PowerTest>();
//_ = BenchmarkRunner.Run<VectorizationSumBenchmark>();
_ = BenchmarkRunner.Run<ToHexStringTest>();
