using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	public class FossilDelta
	{
		static byte[] sample1Delta;
		static byte[] sample2Delta;
		static byte[] sample3Delta;
		static byte[] sample4Delta;
		static byte[] sample5Delta;

		public FossilDelta()
		{
			sample1Delta = Fossil.Delta.Create(Samples.origin1, Samples.target1);
			sample2Delta = Fossil.Delta.Create(Samples.origin2, Samples.target2);
			sample3Delta = Fossil.Delta.Create(Samples.origin3, Samples.target3);
			sample4Delta = Fossil.Delta.Create(Samples.origin4, Samples.target4);
			sample5Delta = Fossil.Delta.Create(Samples.origin5, Samples.target5);
		}

		[Benchmark]
		public byte[] CreateDelta1()
		{
			return Fossil.Delta.Create(Samples.origin1, Samples.target1);
		}

		[Benchmark]
		public byte[] CreateDelta2()
		{
			return Fossil.Delta.Create(Samples.origin2, Samples.target2);
		}

		[Benchmark]
		public byte[] CreateDelta3()
		{
			return Fossil.Delta.Create(Samples.origin3, Samples.target3);
		}

		[Benchmark]
		public byte[] CreateDelta4()
		{
			return Fossil.Delta.Create(Samples.origin4, Samples.target4);
		}

		[Benchmark]
		public byte[] CreateDelta5()
		{
			return Fossil.Delta.Create(Samples.origin5, Samples.target5);
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

		[Benchmark]
		public byte[] ApplyDelta5()
		{
			return Fossil.Delta.Apply(Samples.origin5, sample5Delta);
		}

	}
}

