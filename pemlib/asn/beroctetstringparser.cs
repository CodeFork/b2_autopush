﻿using System;
using System.IO;

using Org.BouncyCastle.Utilities.IO;

namespace Org.BouncyCastle.Asn1
{
	public class BerOctetStringParser : Asn1OctetStringParser
	{
		private readonly Asn1StreamParser _parser;

		internal BerOctetStringParser( Asn1StreamParser parser) { _parser = parser; }

		public Stream GetOctetStream() { return new ConstructedOctetStream(_parser); }

		public Asn1Object ToAsn1Object()
		{
			try
			{
      	MemoryStream buf = new MemoryStream();
			  byte[] bs = new byte[512];
			  int numRead;
			  while ((numRead = GetOctetStream().Read(bs, 0, bs.Length)) > 0) { buf.Write(bs, 0, numRead); }

				return new BerOctetString(buf.ToArray());
			}
			catch (IOException e)
			{
				throw new Asn1ParsingException("IOException converting stream to byte array: " + e.Message, e);
			}
		}
	}
}