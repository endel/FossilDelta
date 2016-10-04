using System;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var summary = BenchmarkRunner.Run<FossilDelta>();
			Console.WriteLine (summary.ToString());
		}
	}
}
