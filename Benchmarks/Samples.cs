using System;
namespace Benchmarks
{
	public class Samples
	{
		public static byte[] origin1 = System.IO.File.ReadAllBytes("Tests/data/1/origin");
		public static byte[] target1 = System.IO.File.ReadAllBytes("Tests/data/1/target");

		public static byte[] origin2 = System.IO.File.ReadAllBytes("Tests/data/2/origin");
		public static byte[] target2 = System.IO.File.ReadAllBytes("Tests/data/2/target");

		public static byte[] origin3 = System.IO.File.ReadAllBytes("Tests/data/3/origin");
		public static byte[] target3 = System.IO.File.ReadAllBytes("Tests/data/3/target");

		public static byte[] origin4 = System.IO.File.ReadAllBytes("Tests/data/4/origin");
		public static byte[] target4 = System.IO.File.ReadAllBytes("Tests/data/4/target");

		public static byte[] origin5 = System.IO.File.ReadAllBytes("Tests/data/5/origin");
		public static byte[] target5 = System.IO.File.ReadAllBytes("Tests/data/5/target");

		public Samples()
		{
		}
	}
}
