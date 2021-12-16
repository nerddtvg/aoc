using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using System.Linq;

using System.Diagnostics;

#nullable enable

namespace AdventOfCode.Solutions.Year2021
{

    class Day16 : ASolution
    {
        public List<Packet> packets = new List<Packet>();

        public enum PacketType
        {
            Other0 = 0,
            Other1 = 1,
            Other2 = 2,
            Other3 = 3,
            Literal = 4,
            Other5 = 5,
            Other6 = 6,
            Other7 = 7,
            Other8 = 8
        }

        public class Packet
        {
            public int version = -1;
            public PacketType type = PacketType.Other0;
            public Int64 literal = -1;
            public int lengthType = -1;
            public int length = -1;
            public List<Packet> children = new List<Packet>();
        }

        public Day16() : base(16, 2021, "Packet Decoder")
        {
            // Literal: 2021
            // DebugInput = "D2FE28";

            // Opeator: with 2 literal subpackets
            // DebugInput = "38006F45291200";

            // Operaetor: with 3 literal subpackets
            // DebugInput = "EE00D40C823060";

            // Opeator: with operator (with operator with literal), sum == 16
            // DebugInput = "8A004A801A8002F478";
            Debug.Assert(SumPacketVersions(ParsePackets(ToBinaryString("8A004A801A8002F478")).packets) == 16);

            // Opeator: sum = 23
            // DebugInput = "620080001611562C8802118E34";
            Debug.Assert(SumPacketVersions(ParsePackets(ToBinaryString("620080001611562C8802118E34")).packets) == 12);

            // Opeator: sum = 23
            // DebugInput = "C0015000016115A2E0802F182340";
            Debug.Assert(SumPacketVersions(ParsePackets(ToBinaryString("C0015000016115A2E0802F182340")).packets) == 23);

            // Opeator: sum = 31
            // DebugInput = "A0016C880162017C3686B18A3D4780";
            Debug.Assert(SumPacketVersions(ParsePackets(ToBinaryString("A0016C880162017C3686B18A3D4780")).packets) == 31);

            this.packets = ParsePackets(ToBinaryString(Input)).packets;
        }

        public int SumPacketVersions(List<Packet> packets)
        {
            return packets.Sum(p => p.version + SumPacketVersions(p.children));
        }

        public string ToBinaryString(string input)
        {
            var ret = string.Empty;

            foreach(var ch in input.ToCharArray().Select(ch => ch.ToString()))
            {
                // Take in a hex character
                // Convert that to a byte
                // Convert that to base2 string
                ret += Convert.ToString(Convert.ToByte(ch, fromBase: 16), toBase: 2).PadLeft(4, '0');
            }

            return ret;
        }

        private (int len, List<Packet> packets) ParsePackets(string inputPackets, int maxCount = 0)
        {
            var packetCount = 0;

            var ret = new List<Packet>();

            int i = 0;

            // Work through the string and parse some packets
            // Our minimum packet size is 11 and we could have unused zeros at the end
            for (i = 0; i < inputPackets.Length - 11 && (maxCount == 0 || packetCount < maxCount); )
            {
                // Get the first three digits as version, second three for type
                var version = inputPackets.Substring(i, 3);

                var type = inputPackets.Substring(i + 3, 3);

                // Bump i up by 6 to skip the type and version
                i += 6;

                int skip = 0;

                if (type == "100")
                {
                    // Literal packet (6)
                    (skip, Packet? packet) = ParseLiteral(Convert.ToInt32(version, fromBase: 2), inputPackets.Substring(i));

                    // Check that we have a value
                    if (packet == null)
                        throw new Exception("Invalid packet");

                    // Got the literal packet, add it to the list
                    ret.Add(packet);
                    packetCount++;
                }
                else
                {
                    // Operator
                    (skip, Packet? packet) = ParseOperator(Convert.ToInt32(version, fromBase: 2), (PacketType)Convert.ToInt32(type, fromBase: 2), inputPackets.Substring(i));

                    // Check that we have a value
                    if (packet == null)
                        throw new Exception("Invalid packet");

                    // Got the literal packet, add it to the list
                    ret.Add(packet);
                    packetCount++;
                }

                // For the next loop, skip ahead
                i += skip;
            }

            return (i, ret);
        }

        private (int skip, Packet? packet) ParseOperator(int version, PacketType type, string input)
        {
            var lengthType = input[0];
            var length = 0;
            var offset = 1;

            var parent = new Packet()
            {
                type = type,
                version = version
            };

            if (lengthType == '1')
            {
                // 11-bit number representing the COUNT of subpackets
                length = Convert.ToInt32(input.Substring(offset, 11), fromBase: 2);
                offset += 11;

                // Get the LENGTH number of packets
                (var skip, var packets) = ParsePackets(input.Substring(offset), maxCount: length);
                parent.children = packets;

                offset += skip;
                parent.lengthType = 1;
            }
            else
            {
                // 15-bit number representing the LENGTH of subpackets
                length = Convert.ToInt32(input.Substring(offset, 15), fromBase: 2);
                offset += 15;

                // Get the packets that exist inside this length
                (var skip, var packets) = ParsePackets(input.Substring(offset, length));
                parent.children = packets;

                offset += skip;
                parent.lengthType = 0;
            }

            // Fix the length
            parent.length = length;

            return (offset, parent);
        }

        private (int skip, Packet? packet) ParseLiteral(int version, string input)
        {
            // Grab groups of 5 characters and find the group that starts with zero
            var literal = string.Empty;
            int q = 0;

            for (q = 0; q < input.Length - 4; q+=5)
            {
                var part = input.Substring(q, 5);

                // Add on the last 4 of a 5-char group
                literal += part.Substring(1);

                // Found the end of the groups
                if (part[0] == '0')
                    break;
            }

            // Somehow we got an empty string? Probably invalid
            if (string.IsNullOrEmpty(literal))
            {
                return (0, null);
            }

            // Account for that last increment that didn't happen
            return (q + 5, new Packet()
            {
                type = PacketType.Literal,
                version = version,
                literal = Convert.ToInt64(literal, fromBase: 2)
            });
        }

        protected override string? SolvePartOne()
        {
            return SumPacketVersions(this.packets).ToString();
        }

        protected override string? SolvePartTwo()
        {
            return null;
        }
    }
}

#nullable restore
