using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TGS.Core.Infrastructures.Security
{
    public interface IDataToStringEncoder
    {
        string Encode(byte[] bytes);
        byte[] Decode(string encoded);
    }

    public sealed class Base62TokenEncoder : IDataToStringEncoder
    {
        private static readonly char[] Base62CodingSpace =
            "2iopZG1MJcby0muXAvjUKez74PgQLhOlNdw5ftrIq8xsk63HYSTWVanDCR9BEF" // shuffled version of "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
            .ToCharArray();

        private static readonly IDictionary<char, int> Base62CodingSpaceIndexByChar =
            Base62CodingSpace
            .Select((ch, i) => Tuple.Create(ch, i))
            .ToDictionary(t => t.Item1, t => t.Item2);

        string IDataToStringEncoder.Encode(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("Empty value passed to be encoded");
            }

            StringBuilder sb = new StringBuilder();
            BitStream stream = new BitStream(bytes);         // Set up the BitStream
            byte[] read = new byte[1];                          // Only read 6-bit at a time
            while (true)
            {
                read[0] = 0;
                int length = stream.Read(read, 0, 6);           // Try to read 6 bits
                if (length == 6)                                // Not reaching the end
                {
                    if (read[0] >> 3 == 0x1f)            // First 5-bit is 11111
                    {
                        sb.Append(Base62CodingSpace[61]);
                        stream.Seek(-1, SeekOrigin.Current);    // Leave the 6th bit to next group
                    }
                    else if (read[0] >> 3 == 0x1e)       // First 5-bit is 11110
                    {
                        sb.Append(Base62CodingSpace[60]);
                        stream.Seek(-1, SeekOrigin.Current);
                    }
                    else                                        // Encode 6-bit
                    {
                        sb.Append(Base62CodingSpace[read[0] >> 2]);
                    }
                }
                else if (length == 0)                           // Reached the end completely
                {
                    break;
                }
                else                                            // Reached the end with some bits left
                {
                    // Padding 0s to make the last bits to 6 bit
                    sb.Append(Base62CodingSpace[read[0] >> 8 - length]);
                    break;
                }
            }
            return sb.ToString();
        }

        byte[] IDataToStringEncoder.Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                throw new ArgumentException("Empty value passed to be decoded");
            }

            // Character count
            int count = 0;

            // Set up the BitStream
            BitStream stream = new BitStream(encoded.Length * 6 / 8);

            for (var i = 0; i < encoded.Length; ++i)
            {
                var c = encoded[i];

                // Look up coding table
                int index = Base62CodingSpaceIndexByChar[c];

                // If end is reached
                if (count == encoded.Length - 1)
                {
                    // Check if the ending is good
                    int mod = (int)(stream.Position % 8);
                    if (mod == 0)
                        throw new InvalidDataException("an extra character was found");

                    if ((index >> (8 - mod)) > 0)
                        throw new InvalidDataException("invalid ending character was found");

                    stream.Write(new[] { (byte)(index << mod) }, 0, 8 - mod);
                }
                else
                {
                    // If 60 or 61 then only write 5 bits to the stream, otherwise 6 bits.
                    if (index == 60)
                    {
                        stream.Write(new byte[] { 0xf0 }, 0, 5);
                    }
                    else if (index == 61)
                    {
                        stream.Write(new byte[] { 0xf8 }, 0, 5);
                    }
                    else
                    {
                        stream.Write(new[] { (byte)index }, 2, 6);
                    }
                }
                count++;
            }

            // Dump out the bytes
            byte[] result = new byte[stream.Position / 8];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(result, 0, result.Length * 8);
            return result;
        }
    }
}