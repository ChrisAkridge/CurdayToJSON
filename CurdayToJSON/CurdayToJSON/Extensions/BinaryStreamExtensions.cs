using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CurdayToJSON.Extensions
{
	public static class BinaryStreamExtensions
	{
		public static string ReadNTString(this BinaryReader reader)
		{
			StringBuilder resultBuilder = new StringBuilder();
			
			while (true)
			{
				char next = reader.ReadChar();
				if (next == 0x00) { break; }
				resultBuilder.Append(next);
			}

			return resultBuilder.ToString();
		}

		public static void WriteNTString(this BinaryWriter writer, string value)
		{
			foreach (char c in value)
			{
				writer.Write(c);
			}

			writer.Write((byte)0x00);
		}
	}
}
