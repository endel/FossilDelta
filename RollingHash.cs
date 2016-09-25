using System;

namespace Fossil
{
	public class RollingHash
	{
		private Int32 a;
		private Int32 b;
		private Int32 i;
		private Int32[] z;

		public RollingHash ()
		{
			this.a = 0;
			this.b = 0;
			this.i = 0;
			this.z = new Int32[Delta.NHASH];
		}

		/**
		 * Initialize the rolling hash using the first NHASH characters of z[]
		 */
		public void Init (byte[] z, int pos)
		{
			int a = 0, b = 0, i, x;
			for(i = 0; i < Delta.NHASH; i++){
				x = z[pos+i];
				a = (a + x) & 0xffff;
				b = (b + (Delta.NHASH-i)*x) & 0xffff;
				this.z[i] = x;
			}
			this.a = a & 0xffff;
			this.b = b & 0xffff;
			this.i = 0;
		}

		/**
		 * Advance the rolling hash by a single character "c"
		 */
		public void Next (int c) {
			var old = this.z[this.i];
			this.z[this.i] = c;
			this.i = (this.i+1)&(Delta.NHASH-1);
			this.a = (this.a - old + c) & 0xffff;
			this.b = (this.b - Delta.NHASH*old + this.a) & 0xffff;
		}


		/**
		 * Return a 32-bit hash value
		 */
		public int Value () {
			return ((this.a & 0xffff) | (this.b & 0xffff) << 16);
		}
			
	}
}

