using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

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

				using (var input = new MemoryStream(origin))
				using (var output = new MemoryStream())
				{
					Fossil.Delta.Apply(input, delta, output);
					Assert.AreEqual(output.ToArray(), target);
				}
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
			byte[] corruptedDelta = new byte[delta.Length - 1];
			Array.Copy(delta, 0, corruptedDelta, 0, corruptedDelta.Length);

			// Apply should throw exception
			var ex = Assert.Throws<Exception> (() => Fossil.Delta.Apply(origin, corruptedDelta));
			Assert.AreEqual("unknown delta operator", ex.Message);

			using (var input = new MemoryStream(origin))
			using (var output = new MemoryStream())
			{
				ex = Assert.Throws<Exception> (() => Fossil.Delta.Apply(input, corruptedDelta, output));
				Assert.AreEqual("unknown delta operator", ex.Message);
			}
		}

		[Test ()]
		public void TestApplyBadChecksumDelta ()
		{
			byte[] origin = System.IO.File.ReadAllBytes ("../../data/2/origin");
			byte[] delta = System.IO.File.ReadAllBytes ("../../data/2/delta");

			// Apply successfully
			Assert.DoesNotThrow (() => Fossil.Delta.Apply (origin, delta));

			// Let's corrupt checksum in delta
			delta[delta.Length-2]--;

			// Apply should throw exception
			var ex = Assert.Throws<Exception> (() => Fossil.Delta.Apply(origin, delta));
			Assert.AreEqual("bad checksum", ex.Message);

			using (var input = new MemoryStream(origin))
			using (var output = new MemoryStream())
			{
				ex = Assert.Throws<Exception> (() => Fossil.Delta.Apply(input, delta, output));
				Assert.AreEqual("bad checksum", ex.Message);
			}
		}

	}
}

