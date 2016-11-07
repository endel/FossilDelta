using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	public class DeltaCompressionDotNet
	{
		byte[] sample1Delta;
		byte[] sample2Delta;
		byte[] sample3Delta;
		byte[] sample4Delta;

		[Benchmark]
		public byte[] CreateDelta1()
		{
			sample1Delta = Fossil.Delta.Create(Samples.origin1, Samples.target1);
			return sample1Delta;
		}

		[Benchmark]
		public byte[] CreateDelta2()
		{
			sample2Delta = Fossil.Delta.Create(Samples.origin2, Samples.target2);
			return sample2Delta;
		}

		[Benchmark]
		public byte[] CreateDelta3()
		{
			sample3Delta = Fossil.Delta.Create(Samples.origin3, Samples.target3);
			return sample3Delta;
		}

		[Benchmark]
		public byte[] CreateDelta4()
		{
			sample4Delta = Fossil.Delta.Create(Samples.origin4, Samples.target4);
			return sample4Delta;
		}

		[Benchmark]
		public byte[] ApplyDelta1()
		{
			return Fossil.Delta.Apply(Samples.origin1, sample1Delta);
		}

		[Benchmark]
		public byte[] ApplyDelta2()
		{
			return Fossil.Delta.Apply(Samples.origin2, sample2Delta);
		}

		[Benchmark]
		public byte[] ApplyDelta3()
		{
			return Fossil.Delta.Apply(Samples.origin3, sample3Delta);
		}

		[Benchmark]
		public byte[] ApplyDelta4()
		{
			return Fossil.Delta.Apply(Samples.origin4, sample4Delta);
		}

	}
}

