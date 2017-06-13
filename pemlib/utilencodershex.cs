﻿using System;
using System.IO;
using System.Text;

namespace Org.BouncyCastle.Utilities.Encoders
{
  /**
	 * Encode and decode byte arrays (typically from binary to 7-bit ASCII
	 * encodings).
	 */
	public interface IEncoder
	{
		int Encode(byte[] data, int off, int length, Stream outStream);
		int Decode(byte[] data, int off, int length, Stream outStream);
		int DecodeString(string data, Stream outStream);
	}

    public class HexEncoder : IEncoder
    {
        protected readonly byte[] encodingTable =
        {
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',
            (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f'
        };

        /*
         * set up the decoding table.
         */
        protected readonly byte[] decodingTable = new byte[128];

        protected void InitialiseDecodingTable()
        {
            Arrays.Fill(decodingTable, (byte)0xff);

            for (int i = 0; i < encodingTable.Length; i++)
            {
                decodingTable[encodingTable[i]] = (byte)i;
            }

            decodingTable['A'] = decodingTable['a'];
            decodingTable['B'] = decodingTable['b'];
            decodingTable['C'] = decodingTable['c'];
            decodingTable['D'] = decodingTable['d'];
            decodingTable['E'] = decodingTable['e'];
            decodingTable['F'] = decodingTable['f'];
        }

        public HexEncoder()
        {
            InitialiseDecodingTable();
        }

        /**
        * encode the input data producing a Hex output stream.
        *
        * @return the number of bytes produced.
        */
        public int Encode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            for (int i = off; i < (off + length); i++)
            {
                int v = data[i];

                outStream.WriteByte(encodingTable[v >> 4]);
                outStream.WriteByte(encodingTable[v & 0xf]);
            }

            return length * 2;
        }

        private static bool Ignore(char c)
        {
            return c == '\n' || c =='\r' || c == '\t' || c == ' ';
        }

        /**
        * decode the Hex encoded byte data writing it to the given output stream,
        * whitespace characters will be ignored.
        *
        * @return the number of bytes produced.
        */
        public int Decode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            byte b1, b2;
            int outLen = 0;
            int end = off + length;

            while (end > off)
            {
                if (!Ignore((char)data[end - 1]))
                {
                    break;
                }

                end--;
            }

            int i = off;
            while (i < end)
            {
                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                b1 = decodingTable[data[i++]];

                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                b2 = decodingTable[data[i++]];

                if ((b1 | b2) >= 0x80)
                    throw new IOException("invalid characters encountered in Hex data");

                outStream.WriteByte((byte)((b1 << 4) | b2));

                outLen++;
            }

            return outLen;
        }

        /**
        * decode the Hex encoded string data writing it to the given output stream,
        * whitespace characters will be ignored.
        *
        * @return the number of bytes produced.
        */
        public int DecodeString(
            string	data,
            Stream	outStream)
        {
            byte    b1, b2;
            int     length = 0;

            int     end = data.Length;

            while (end > 0)
            {
                if (!Ignore(data[end - 1]))
                {
                    break;
                }

                end--;
            }

            int i = 0;
            while (i < end)
            {
                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                b1 = decodingTable[data[i++]];

                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                b2 = decodingTable[data[i++]];

                if ((b1 | b2) >= 0x80)
                    throw new IOException("invalid characters encountered in Hex data");

                outStream.WriteByte((byte)((b1 << 4) | b2));

                length++;
            }

            return length;
        }
    }
    /// <summary>
    /// Class to decode and encode Hex.
    /// </summary>
    public sealed class Hex
    {
        private static readonly IEncoder encoder = new HexEncoder();

        private Hex()
        {
        }

        public static string ToHexString(
            byte[] data)
        {
            return ToHexString(data, 0, data.Length);
        }

        public static string ToHexString(
            byte[]	data,
            int		off,
            int		length)
        {
            byte[] hex = Encode(data, off, length);
            return Encoding.UTF8.GetString(hex, 0, hex.Length);
        }

        /**
         * encode the input data producing a Hex encoded byte array.
         *
         * @return a byte array containing the Hex encoded data.
         */
        public static byte[] Encode(
            byte[] data)
        {
            return Encode(data, 0, data.Length);
        }

        /**
         * encode the input data producing a Hex encoded byte array.
         *
         * @return a byte array containing the Hex encoded data.
         */
        public static byte[] Encode(
            byte[]	data,
            int		off,
            int		length)
        {
            MemoryStream bOut = new MemoryStream(length * 2);

            encoder.Encode(data, off, length, bOut);

            return bOut.ToArray();
        }

        /**
         * Hex encode the byte data writing it to the given output stream.
         *
         * @return the number of bytes produced.
         */
        public static int Encode(
            byte[]	data,
            Stream	outStream)
        {
            return encoder.Encode(data, 0, data.Length, outStream);
        }

        /**
         * Hex encode the byte data writing it to the given output stream.
         *
         * @return the number of bytes produced.
         */
        public static int Encode(
            byte[]	data,
            int		off,
            int		length,
            Stream	outStream)
        {
            return encoder.Encode(data, off, length, outStream);
        }

        /**
         * decode the Hex encoded input data. It is assumed the input data is valid.
         *
         * @return a byte array representing the decoded data.
         */
        public static byte[] Decode(
            byte[] data)
        {
            MemoryStream bOut = new MemoryStream((data.Length + 1) / 2);

            encoder.Decode(data, 0, data.Length, bOut);

            return bOut.ToArray();
        }

        /**
         * decode the Hex encoded string data - whitespace will be ignored.
         *
         * @return a byte array representing the decoded data.
         */
        public static byte[] Decode(
            string data)
        {
            MemoryStream bOut = new MemoryStream((data.Length + 1) / 2);

            encoder.DecodeString(data, bOut);

            return bOut.ToArray();
        }

        /**
         * decode the Hex encoded string data writing it to the given output stream,
         * whitespace characters will be ignored.
         *
         * @return the number of bytes produced.
         */
        public static int Decode(
            string	data,
            Stream	outStream)
        {
            return encoder.DecodeString(data, outStream);
        }
    }
}