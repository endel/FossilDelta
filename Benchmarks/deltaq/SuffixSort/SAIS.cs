/*
 * SAIS.cs for deltaq
 * Copyright (c) 2014 J. Zebedee
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
/*
 * SAIS.cs for SAIS-CSharp
 * Copyright (c) 2010 Yuta Mori. All Rights Reserved.
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace deltaq.SuffixSort
{
    /// <summary>
    ///     An implementation of the induced sorting based suffix array construction algorithm.
    /// </summary>
    public class SAIS : ISuffixSort
    {
        private const int MinBucketSize = 256;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetCounts(IList<int> T, IList<int> c, int n, int k)
        {
            int i;
            for (i = 0; i < k; ++i)
                c[i] = 0;

            for (i = 0; i < n; ++i)
                c[T[i]]++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetBuckets(IList<int> c, IList<int> b, int k, bool end)
        {
            int i, sum = 0;
            for (i = 0; i < k; ++i)
            {
                sum += c[i];
                b[i] = end ? sum : sum - c[i];
            }
        }

        /* sort all type LMS suffixes */

        private void LMS_sort(IList<int> T, IList<int> sa, IList<int> c, IList<int> b, int n, int k)
        {
            int bb, i, j;
            int c0, c1;

            /* compute SAl */
            if (Equals(c, b))
                GetCounts(T, c, n, k);
            GetBuckets(c, b, k, false); /* find starts of buckets */

            j = n - 1;
            bb = b[c1 = T[j]];
            --j;
            sa[bb++] = (T[j] < c1) ? ~j : j;
            for (i = 0; i < n; ++i)
            {
                if (0 < (j = sa[i]))
                {
                    if ((c0 = T[j]) != c1)
                    {
                        b[c1] = bb;
                        bb = b[c1 = c0];
                    }
                    --j;
                    sa[bb++] = (T[j] < c1) ? ~j : j;
                    sa[i] = 0;
                }
                else if (j < 0)
                {
                    sa[i] = ~j;
                }
            }

            /* compute SAs */
            if (Equals(c, b))
                GetCounts(T, c, n, k);
            GetBuckets(c, b, k, true); /* find ends of buckets */

            for (i = n - 1, bb = b[c1 = 0]; 0 <= i; --i)
            {
                if (0 < (j = sa[i]))
                {
                    if ((c0 = T[j]) != c1)
                    {
                        b[c1] = bb;
                        bb = b[c1 = c0];
                    }
                    --j;
                    sa[--bb] = (T[j] > c1) ? ~(j + 1) : j;
                    sa[i] = 0;
                }
            }
        }

        private int LMS_post_proc(IList<int> T, IList<int> sa, int n, int m)
        {
            int i, j, p, q;
            int qlen, name;
            int c0, c1;

            /* compact all the sorted substrings into the first m items of SA
                2*m must be not larger than n (proveable) */
            for (i = 0; (p = sa[i]) < 0; ++i)
            {
                sa[i] = ~p;
            }

            if (i < m)
            {
                for (j = i, ++i; ; ++i)
                {
                    if ((p = sa[i]) < 0)
                    {
                        sa[j++] = ~p;
                        sa[i] = 0;
                        if (j == m)
                        {
                            break;
                        }
                    }
                }
            }

            /* store the length of all substrings */
            i = n - 1;
            j = n - 1;
            c0 = T[n - 1];
            do
            {
                c1 = c0;
            } while ((0 <= --i) && ((c0 = T[i]) >= c1));
            for (; 0 <= i; )
            {
                do
                {
                    c1 = c0;
                } while ((0 <= --i) && ((c0 = T[i]) <= c1));
                if (0 <= i)
                {
                    sa[m + ((i + 1) >> 1)] = j - i;
                    j = i + 1;
                    do
                    {
                        c1 = c0;
                    } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                }
            }

            /* find the lexicographic names of all substrings */
            for (i = 0, name = 0, q = n, qlen = 0; i < m; ++i)
            {
                p = sa[i];
                int plen = sa[m + (p >> 1)];
                bool diff = true;
                if ((plen == qlen) && ((q + plen) < n))
                {
                    for (j = 0;
                        (j < plen) && (T[p + j] == T[q + j]);
                        ++j)
                    {
                    }
                    if (j == plen)
                    {
                        diff = false;
                    }
                }
                if (diff)
                {
                    ++name;
                    q = p;
                    qlen = plen;
                }
                sa[m + (p >> 1)] = name;
            }

            return name;
        }

        private void InduceSA(IList<int> T, int[] sa, IList<int> c, IList<int> b, int n, int k)
        {
            int bb, i, j;
            int c0, c1;

            /* compute SAl */
            if (Equals(c, b))
                GetCounts(T, c, n, k);
            GetBuckets(c, b, k, false); /* find starts of buckets */

            j = n - 1;
            bb = b[c1 = T[j]];
            sa[bb++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
            for (i = 0; i < n; ++i)
            {
                j = sa[i];
                sa[i] = ~j;
                if (0 < j)
                {
                    if ((c0 = T[--j]) != c1)
                    {
                        b[c1] = bb;
                        bb = b[c1 = c0];
                    }
                    sa[bb++] = ((0 < j) && (T[j - 1] < c1)) ? ~j : j;
                }
            }

            /* compute SAs */
            if (Equals(c, b))
                GetCounts(T, c, n, k);
            GetBuckets(c, b, k, true); /* find ends of buckets */

            for (i = n - 1, bb = b[c1 = 0]; 0 <= i; --i)
            {
                if (0 < (j = sa[i]))
                {
                    if ((c0 = T[--j]) != c1)
                    {
                        b[c1] = bb;
                        bb = b[c1 = c0];
                    }
                    sa[--bb] = ((j == 0) || (T[j - 1] > c1)) ? ~j : j;
                }
                else
                {
                    sa[i] = ~j;
                }
            }
        }

        /* find the suffix array SA of T[0..n-1] in {0..k-1}^n
           use a working space (excluding T and SA) of at most 2n+O(1) for a constant alphabet */

        private void sais_main(IList<int> T, int[] sa, int fs, int n, int k)
        {
            IList<int> c, b;
            int i, j, bb, m;
            int name;
            int c0, c1;
            uint flags;

            if (k <= MinBucketSize)
            {
                c = new int[k];
                if (k <= fs)
                {
                    b = sa.Slice(n + fs - k, sa.Length - (n + fs - k));
                    flags = 1;
                }
                else
                {
                    b = new int[k];
                    flags = 3;
                }
            }
            else if (k <= fs)
            {
                c = sa.Slice(n + fs - k, sa.Length - (n + fs - k));
                if (k <= (fs - k))
                {
                    b = sa.Slice(n + fs - k * 2, sa.Length - (n + fs - k * 2));
                    flags = 0;
                }
                else if (k <= (MinBucketSize * 4))
                {
                    b = new int[k];
                    flags = 2;
                }
                else
                {
                    b = c;
                    flags = 8;
                }
            }
            else
            {
                c = b = new int[k];
                flags = 4 | 8;
            }

            /* stage 1: reduce the problem by at least 1/2
               sort all the LMS-substrings */
            GetCounts(T, c, n, k);
            GetBuckets(c, b, k, true); /* find ends of buckets */
            for (i = 0; i < n; ++i)
            {
                sa[i] = 0;
            }

            bb = -1;
            i = n - 1;
            j = n;
            m = 0;
            c0 = T[n - 1];
            do
            {
                c1 = c0;
            } while ((0 <= --i) && ((c0 = T[i]) >= c1));

            for (; 0 <= i; )
            {
                do
                {
                    c1 = c0;
                } while ((0 <= --i) && ((c0 = T[i]) <= c1));
                if (0 <= i)
                {
                    if (0 <= bb)
                    {
                        sa[bb] = j;
                    }
                    bb = --b[c1];
                    j = i;
                    ++m;
                    do
                    {
                        c1 = c0;
                    } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                }
            }
            if (1 < m)
            {
                LMS_sort(T, sa, c, b, n, k);
                name = LMS_post_proc(T, sa, n, m);
            }
            else if (m == 1)
            {
                sa[bb] = j + 1;
                name = 1;
            }
            else
            {
                name = 0;
            }

            /* stage 2: solve the reduced problem
               recurse if names are not yet unique */
            if (name < m)
            {
                if ((flags & 4) != 0)
                {
                    c = null;
                    b = null;
                }
                if ((flags & 2) != 0)
                {
                    b = null;
                }
                int newfs = (n + fs) - (m * 2);
                if ((flags & (1 | 4 | 8)) == 0)
                {
                    if ((k + name) <= newfs)
                    {
                        newfs -= k;
                    }
                    else
                    {
                        flags |= 8;
                    }
                }

                // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                for (i = m + (n >> 1) - 1, j = m * 2 + newfs - 1; m <= i; --i)
                {
                    if (sa[i] != 0)
                    {
                        sa[j--] = sa[i] - 1;
                    }
                }

                sais_main(sa.Slice(m + newfs, sa.Length - (m + newfs)), sa, newfs, m, name);

                i = n - 1;
                j = m * 2 - 1;
                c0 = T[n - 1];
                do
                {
                    c1 = c0;
                } while ((0 <= --i) && ((c0 = T[i]) >= c1));

                for (; 0 <= i; )
                {
                    do
                    {
                        c1 = c0;
                    } while ((0 <= --i) && ((c0 = T[i]) <= c1));

                    if (0 <= i)
                    {
                        sa[j--] = i + 1;
                        do
                        {
                            c1 = c0;
                        } while ((0 <= --i) && ((c0 = T[i]) >= c1));
                    }
                }

                for (i = 0; i < m; ++i)
                {
                    sa[i] = sa[m + sa[i]];
                }
                if ((flags & 4) != 0)
                {
                    c = b = new int[k];
                }
                if ((flags & 2) != 0)
                {
                    b = new int[k];
                }
            }

            /* stage 3: induce the result for the original problem */
            if ((flags & 8) != 0)
            {
                GetCounts(T, c, n, k);
            }
            /* put all left-most S characters into their buckets */
            if (1 < m)
            {
                GetBuckets(c, b, k, true); /* find ends of buckets */
                i = m - 1;
                j = n;
                int p = sa[m - 1];
                c1 = T[p];
                do
                {
                    // ReSharper disable once PossibleNullReferenceException
                    int q = b[c0 = c1];

                    // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
                    while (q < j)
                    {
                        sa[--j] = 0;
                    }

                    do
                    {
                        sa[--j] = p;
                        if (--i < 0)
                        {
                            break;
                        }
                        p = sa[i];
                    } while ((c1 = T[p]) == c0);
                } while (0 <= i);

                while (0 < j)
                {
                    sa[--j] = 0;
                }
            }

            InduceSA(T, sa, c, b, n, k);
        }

        /*- Suffixsorting -*/
        /* byte */

        /// <summary>
        ///     Constructs the suffix array of a given string (as byte array) in linear time.
        /// </summary>
        /// <param name="T">input bytes</param>
        /// <returns>0 if no error occurred, -1 or -2 otherwise</returns>
        public int[] Sort(byte[] T)
        {
            if (T == null)
                throw new ArgumentNullException(nameof(T));

            var sa = new int[T.Length + 1];

            if (T.Length <= 1)
            {
                if (T.Length == 1)
                {
                    sa[0] = 0;
                }
            }
            else
                sais_main(new IntAccessor(T), sa, 0, T.Length, 256);

            return sa;
        }

        private class IntAccessor : IList<int>
        {
            private readonly byte[] _buffer;

            public IntAccessor(byte[] buf)
            {
                _buffer = buf;
            }

            public int IndexOf(int item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, int item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public int this[int index]
            {
                get { return _buffer[index]; }
                set { _buffer[index] = (byte)value; }
            }

            public void Add(int item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(int item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return _buffer.Length; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(int item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<int> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}