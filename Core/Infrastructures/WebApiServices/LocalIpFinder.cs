using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TGS.Core.Infrastructures.WebApiServices
{
    public interface ILocalIpFinder
    {
        bool IsLocal(string ip);
        IpInfo GetIpInfo(string ip);
    }

    /// <summary>
    /// based on:
    /// https://tools.ietf.org/html/rfc6890
    /// </summary>
    public sealed class LocalIpFinder : ILocalIpFinder
    {
        // https://tools.ietf.org/html/rfc6890
        private static readonly Ip4AndMask[] Locals =
        {
            new(@"0.0.0.0/8"),
            new(@"10.0.0.0/8"),
            new(@"100.64.0.0/10"),
            new(@"127.0.0.0/8"),
            new(@"169.254.0.0/16"),
            new(@"172.16.0.0/12"),
            new(@"192.0.0.0/24"),
            new(@"192.0.0.0/29"),
            new(@"192.0.2.0/24"),
            new(@"192.88.99.0/24"),
            new(@"192.168.0.0/16"),
            new(@"198.18.0.0/15"),
            new(@"198.51.100.0/24"),
            new(@"203.0.113.0/24"),
            new(@"240.0.0.0/4"),
            new(@"255.255.255.255/32")
        };

        private static readonly HashSet<byte> FirstBytesForLocals =
            new(Locals.Select(e => e.FirstByte)
                    .Union( // any IP in the range of 240.x.x.x - 255.x.x.x is local, or 240.0.0.0/4
                        Enumerable.Range(240, 15).Select(i => (byte) i)));

        private readonly ConcurrentDictionary<string, bool> _knownLocalIps =
            new(StringComparer.OrdinalIgnoreCase);

        bool ILocalIpFinder.IsLocal(string ip)
        {
            if (_knownLocalIps.ContainsKey(ip))
            {
                return true;
            }

            if (!IsLocalInternal(ip, out _))
            {
                return false;
            }

            _knownLocalIps.AddOrUpdate(ip, true, (s, b) => true);
            return true;
        }

        IpInfo ILocalIpFinder.GetIpInfo(string ip)
        {
            if (!IsLocalInternal(ip, out var isValid))
            {
                return new() {IsLocal = false, IsValid = isValid};
            }

            _knownLocalIps.AddOrUpdate(ip, true, (s, b) => true);
            return new() {IsLocal = true, IsValid = isValid};
        }

        private static bool IsLocalInternal(string ip, out bool isValid)
        {
            ip = ip.Trim();
            if (!IPAddress.TryParse(ip, out var address))
            {
                isValid = false;
                return false;
            }

            isValid = true;
            // if this is ip v6 address
            if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return address.IsIPv6LinkLocal || address.IsIPv6SiteLocal;
            }

            var bytes = address.GetAddressBytes();
            // that eliminates 90% of possible addresses starting with byte that is never local
            if (!FirstBytesForLocals.Contains(bytes[0]))
            {
                return false;
            }

            var ipAsUInt = BitConverter.ToUInt32(bytes, 0);
            return Locals.Any(data => (ipAsUInt & data.Mask) == data.Ip);
        }

        internal static uint ComputeMask(int bitLimit)
        {
            uint mask = 0;
            var i = 0;
            for (var byteIndex = 0; byteIndex < 4; ++byteIndex)
            {
                var b = 0;
                for (var bitIndex = 0; bitIndex < 8; ++bitIndex)
                {
                    if (i == bitLimit) break;
                    ++i;
                    b |= 1 << (7 - bitIndex);
                }

                mask |= (uint) (b << (8 * byteIndex));
                if (i == bitLimit) break;
            }

            return mask;
        }

        private readonly struct Ip4AndMask
        {
            internal Ip4AndMask(string info)
            {
                var arr = info.Split('/');
                var ip = arr[0];
                var bits = arr.Length < 2 ? 8 : int.Parse(arr[1].Trim());
                var address = IPAddress.Parse(ip);
                var bytes = address.GetAddressBytes();
                FirstByte = bytes[0];
                Ip = BitConverter.ToUInt32(bytes, 0);
                Mask = ComputeMask(bits);
            }

            internal readonly byte FirstByte;
            internal readonly uint Ip;
            internal readonly uint Mask;
        }
    }

    public struct IpInfo
    {
        public bool IsLocal;
        public bool IsValid;
    }
}