/*
 * BsPatch.cs for deltaq
 * Copyright (c) 2014 J. Zebedee
 * 
 * BsDiff.net is Copyright 2010 Logos Bible Software
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

using System;
using System.IO;

namespace deltaq.BsDiff
{
    public static class BsPatch
    {
        /// <summary>
        /// Opens a BSDIFF-format patch at a specific position
        /// </summary>
        /// <param name="offset">Zero-based offset into the patch</param>
        /// <param name="length">Length of the Stream from offset, or 0 for the rest of the patch</param>
        /// <returns>Readable, seekable stream with specified offset and length</returns>
        public delegate Stream OpenPatchStream(long offset, long length);

        /// <summary>
        /// Applies a BSDIFF-format patch to an original and produces the updated version
        /// </summary>
        /// <param name="input">Byte array of the original (older) data</param>
        /// <param name="diff">Byte array of the BSDIFF-format patch data</param>
        /// <param name="output">Writable stream where the updated data will be written</param>
        public static void Apply(byte[] input, byte[] diff, Stream output)
        {
            OpenPatchStream openPatchStream = (uOffset, uLength) =>
            {
                var offset = (int)uOffset;
                var length = (int)uLength;
                return new MemoryStream(diff, offset,
                    uLength > 0
                        ? length
                        : diff.Length - offset);
            };

            Stream controlStream, diffStream, extraStream;
            var newSize = CreatePatchStreams(openPatchStream, out controlStream, out diffStream, out extraStream);

            // prepare to read three parts of the patch in parallel
            ApplyInternal(newSize, new MemoryStream(input), controlStream, diffStream, extraStream, output);
        }

        /// <summary>
        /// Applies a BSDIFF-format patch to an original and produces the updated version
        /// </summary>
        /// <param name="input">Readable, seekable stream of the original (older) data</param>
        /// <param name="openPatchStream"><see cref="OpenPatchStream"/></param>
        /// <param name="output">Writable stream where the updated data will be written</param>
        public static void Apply(Stream input, OpenPatchStream openPatchStream, Stream output)
        {
            Stream controlStream, diffStream, extraStream;
            var newSize = CreatePatchStreams(openPatchStream, out controlStream, out diffStream, out extraStream);

            // prepare to read three parts of the patch in parallel
            ApplyInternal(newSize, input, controlStream, diffStream, extraStream, output);
        }

        private static long CreatePatchStreams(OpenPatchStream openPatchStream, out Stream ctrl, out Stream diff, out Stream extra)
        {
            // read header
            long controlLength, diffLength, newSize;
            using (var patchStream = openPatchStream(0, BsDiff.HeaderSize))
            {
                // check patch stream capabilities
                if (!patchStream.CanRead)
                    throw new ArgumentException("Patch stream must be readable", nameof(openPatchStream));
                if (!patchStream.CanSeek)
                    throw new ArgumentException("Patch stream must be seekable", nameof(openPatchStream));

                var header = new byte[BsDiff.HeaderSize];
                patchStream.Read(header, 0, BsDiff.HeaderSize);

                // check for appropriate magic
                var signature = header.ReadLong();
                if (signature != BsDiff.Signature)
                    throw new InvalidOperationException("Corrupt patch");

                // read lengths from header
                controlLength = header.ReadLongAt(8);
                diffLength = header.ReadLongAt(16);
                newSize = header.ReadLongAt(24);

                if (controlLength < 0 || diffLength < 0 || newSize < 0)
                    throw new InvalidOperationException("Corrupt patch");
            }

            // prepare to read three parts of the patch in parallel
            Stream
                compressedControlStream = openPatchStream(BsDiff.HeaderSize, controlLength),
                compressedDiffStream = openPatchStream(BsDiff.HeaderSize + controlLength, diffLength),
                compressedExtraStream = openPatchStream(BsDiff.HeaderSize + controlLength + diffLength, 0);

            // decompress each part (to read it)
            ctrl = BsDiff.GetEncodingStream(compressedControlStream, false);
            diff = BsDiff.GetEncodingStream(compressedDiffStream, false);
            extra = BsDiff.GetEncodingStream(compressedExtraStream, false);

            return newSize;
        }

        private static void ApplyInternal(long newSize, Stream input, Stream ctrl, Stream diff, Stream extra, Stream output)
        {
            if (!input.CanRead)
                throw new ArgumentException("Input stream must be readable", nameof(input));
            if (!input.CanSeek)
                throw new ArgumentException("Input stream must be seekable", nameof(input));
            if (!output.CanWrite)
                throw new ArgumentException("Output stream must be writable", nameof(output));

            using (ctrl)
            using (diff)
            using (extra)
            using (var inputReader = new BinaryReader(input))
                while (output.Position < newSize)
                {
                    //read control data:
                    // set of triples (x,y,z) meaning

                    // add x bytes from oldfile to x bytes from the diff block;
                    var addSize = ctrl.ReadLong();
                    // copy y bytes from the extra block;
                    var copySize = ctrl.ReadLong();
                    // seek forwards in oldfile by z bytes;
                    var seekAmount = ctrl.ReadLong();

                    // sanity-check
                    if (output.Position + addSize > newSize)
                        throw new InvalidOperationException("Corrupt patch");

                    // read diff string in chunks
                    foreach (var newData in diff.BufferedRead(addSize))
                    {
                        var inputData = inputReader.ReadBytes(newData.Length);

                        // add old data to diff string
                        for (var i = 0; i < newData.Length; i++)
                            newData[i] += inputData[i];

                        output.Write(newData, 0, newData.Length);
                    }

                    // sanity-check
                    if (output.Position + copySize > newSize)
                        throw new InvalidOperationException("Corrupt patch");

                    // read extra string in chunks
                    foreach (var extraData in extra.BufferedRead(copySize))
                    {
                        output.Write(extraData, 0, extraData.Length);
                    }

                    // adjust position
                    input.Seek(seekAmount, SeekOrigin.Current);
                }
        }
    }
}
