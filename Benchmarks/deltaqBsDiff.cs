using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using deltaq.BsDiff;

namespace Benchmarks
{
	public class DeltaqBsDiff
	{
		byte[] sample1Delta;
		byte[] sample2Delta;
		byte[] sample3Delta;
		byte[] sample4Delta;
		byte[] sample5Delta;

		public DeltaqBsDiff()
		{
			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin1, Samples.target1, outputStream);
				sample1Delta = outputStream.ToArray();
			}

			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin2, Samples.target2, outputStream);
				sample2Delta = outputStream.ToArray();
			}

			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin3, Samples.target3, outputStream);
				sample3Delta = outputStream.ToArray();
			}

			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin4, Samples.target4, outputStream);
				sample4Delta = outputStream.ToArray();
			}

			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin5, Samples.target5, outputStream);
				sample5Delta = outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] CreateDelta1()
		{
			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin1, Samples.target1, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] CreateDelta2()
		{
			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin2, Samples.target2, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] CreateDelta3()
		{
			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin3, Samples.target3, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] CreateDelta4()
		{
			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin4, Samples.target4, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] CreateDelta5()
		{
			using (var outputStream = new MemoryStream())
			{
				BsDiff.Create(Samples.origin5, Samples.target5, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] ApplyDelta1()
		{
			using (var outputStream = new MemoryStream())
			{
				BsPatch.Apply(Samples.origin1, sample1Delta, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] ApplyDelta2()
		{
			using (var outputStream = new MemoryStream())
			{
				BsPatch.Apply(Samples.origin2, sample2Delta, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] ApplyDelta3()
		{
			using (var outputStream = new MemoryStream())
			{
				BsPatch.Apply(Samples.origin3, sample3Delta, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] ApplyDelta4()
		{
			using (var outputStream = new MemoryStream())
			{
				BsPatch.Apply(Samples.origin4, sample4Delta, outputStream);
				return outputStream.ToArray();
			}
		}

		[Benchmark]
		public byte[] ApplyDelta5()
		{
			using (var outputStream = new MemoryStream())
			{
				BsPatch.Apply(Samples.origin5, sample5Delta, outputStream);
				return outputStream.ToArray();
			}
		}

	}
}
