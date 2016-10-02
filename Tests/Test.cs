using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
	[TestFixture ()]
	public class Test
	{

		[Test ()]
		public void TestCreateAndApply()
		{
			int nTests = 5;

			for (int i = 1; i <= nTests; i++) 
			{
				byte[] origin = System.IO.File.ReadAllBytes ("../../data/" + i + "/origin");
				byte[] target = System.IO.File.ReadAllBytes ("../../data/" + i + "/target");
				byte[] goodDelta = System.IO.File.ReadAllBytes ("../../data/" + i + "/delta");
				byte[] delta = Fossil.Delta.Create (origin, target);

				Assert.AreEqual (delta, goodDelta);

				byte[] applied = Fossil.Delta.Apply(origin, delta);
				Assert.AreEqual(applied, target);
			}
		}

		[Test ()]
		public void TestApplyTruncatedDelta ()
		{
			byte[] origin = System.IO.File.ReadAllBytes ("../../data/1/origin");
			byte[] delta = System.IO.File.ReadAllBytes ("../../data/1/delta");

			// Apply successfully
			Assert.DoesNotThrow (() => Fossil.Delta.Apply (origin, delta));

			// Let's corrupt our delta
			Stack<byte> deltaBytes = new Stack<byte> (delta);
			deltaBytes.Pop ();
			byte[] corruptedDelta = deltaBytes.ToArray ();

			// Apply should throw exception
			Assert.Throws<Exception> (() => Fossil.Delta.Apply(origin, corruptedDelta));
		}

	}
}

