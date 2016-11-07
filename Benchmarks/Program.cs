using System;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var switcher = new BenchmarkSwitcher(new[] {
				typeof(FossilDelta)
			});
			switcher.Run(args);
		}
	}
}
