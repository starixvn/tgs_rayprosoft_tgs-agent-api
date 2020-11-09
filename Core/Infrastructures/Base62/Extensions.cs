using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TGS.Core.Infrastructures.IO;

namespace TGS.Core.Infrastructures.Base62
{
    public static class Extensions
    {
        private static readonly char[] Base62CodingSpace =
            "2iopZG1MJcby063HYSTWVanmuXAvjLhOlNdw5ftrIq8xskDCR9BEFUKez74PgQ" // shuffled version of "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
                .ToCharArray();

        private static readonly IDictionary<char, int> Base62CodingSpaceIndexByChar =
            Base62CodingSpace
                .Select((ch, i) => new { Char = ch, Index = i })
                .ToDictionary(t => t.Char, t => t.Index);

        public static string ToBase62(this byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new ArgumentException("Empty value passed to be encoded");
            }

            var sb = new StringBuilder();
            var stream = new BitStream(bytes); // Set up the BitStream
            var read = new byte[1];            // Only read 6-bit at a time

            while (true)
            {
                read[0] = 0;
                var length = stream.Read(read, 0, 6); // Try to read 6 bits
                if (length == 6)                      // Not reaching the end
                {
                    switch (read[0] >> 3)
                    {
                        // First 5-bit is 11111
                        case 0x1f:
                            sb.Append(Base62CodingSpace[61]);
                            stream.Seek(-1, SeekOrigin.Current); // Leave the 6th bit to next group
                            break;

                        // First 5-bit is 11110
                        case 0x1e:
                            sb.Append(Base62CodingSpace[60]);
                            stream.Seek(-1, SeekOrigin.Current);
                            break;

                        // Encode 6-bit
                        default:
                            sb.Append(Base62CodingSpace[read[0] >> 2]);
                            break;
                    }
                }
                else if (length == 0) // Reached the end completely
                {
                    break;
                }
                else // Reached the end with some bits left
                {
                    // Padding 0s to make the last bits to 6 bit
                    sb.Append(Base62CodingSpace[read[0] >> (8 - length)]);
                    break;
                }
            }

            return sb.ToString();
        }

        public static byte[] FromBase62(this string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                throw new ArgumentException("Empty value passed to be decoded");
            }

            // Character count
            var count = 0;

            // Set up the BitStream
            var stream = new BitStream(encoded.Length * 6 / 8);

            foreach (var index in encoded.Select(c => Base62CodingSpaceIndexByChar[c]))
            {
                // If end is reached
                if (count == encoded.Length - 1)
                {
                    // Check if the ending is good
                    var mod = (int) (stream.Position % 8);
                    if (mod == 0)
                    {
                        throw new InvalidDataException("an extra character was found");
                    }

                    if (index >> (8 - mod) > 0)
                    {
                        throw new InvalidDataException("invalid ending character was found");
                    }

                    stream.Write(new[] {(byte) (index << mod)}, 0, 8 - mod);
                }
                else
                {
                    switch (index)
                    {
                        // If 60 or 61 then only write 5 bits to the stream, otherwise 6 bits.
                        case 60:
                            stream.Write(new byte[] {0xf0}, 0, 5);
                            break;

                        case 61:
                            stream.Write(new byte[] {0xf8}, 0, 5);
                            break;

                        default:
                            stream.Write(new[] {(byte) index}, 2, 6);
                            break;
                    }
                }

                count++;
            }

            // Dump out the bytes
            var result = new byte[stream.Position / 8];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(result, 0, result.Length * 8);

            return result;
        }
    }
}