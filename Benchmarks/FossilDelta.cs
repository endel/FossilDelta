using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	public class FossilDelta
	{

		[Benchmark]
		public byte[] CreateDelta()
		{
			return Fossil.Delta.Create(Samples.origin1, Samples.target1);
		}
	}
}

