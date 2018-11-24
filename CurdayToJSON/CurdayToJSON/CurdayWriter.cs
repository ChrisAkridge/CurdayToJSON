using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CurdayToJSON.Extensions;

namespace CurdayToJSON
{
	internal static class CurdayWriter
	{
		public static void Write(Curday curday, string filePath)
		{
			// Preconditions: curday must not be null
			// filePath must be non-null and valid

			BinaryWriter file = new BinaryWriter(File.OpenWrite(filePath));


			file.Dispose();
		}

		private static void WriteHeader(CurdayHeader header, BinaryWriter file)
		{
			// Write Diagnostics BCK (ASCII char, one byte)
			file.Write((byte)header.DiagnosticsBCK);

			// Write Diagnostics FWD (ASCII char, one byte)
			file.Write((byte)header.DiagnosticsFWD);

			// Write Scroll Speed (ASCII char, one byte, number in range 0..7 inclusive)
			file.Write((byte)(header.ScrollSpeed + 0x30));

			// Write Number of Text Ads Allowed A (ASCII char, one byte, number in range 0..9
			// inclusive)
			file.Write((byte)(header.NumberOfTextAdsAllowedA + 0x30));

			// Write Number of Text Ads Allowed B (ASCII char, one byte, number in range 0..9
			// inclusive)
			file.Write((byte)(header.NumberOfTextAdsAllowedB + 0x30));

			// Write Number of Lines in Text Ad (ASCII char, one byte, number in range 0..9
			// inclusive)
			file.Write((byte)(header.NumberOfLinesInTextAd + 0x30));

			// Write GRPH DST (ASCII char, one byte, either Y or N)
			file.Write((header.GrphDST) ? 'Y' : 'N');

			// Write ETX SOH (two bytes)
			file.Write((byte)0x03);
			file.Write((byte)0x01);

			// Write timezone (ASCII char, one byte)
			file.Write((byte)(header.Timezone + 0x30));

			// Write flags (8 ASCII chars, eight bytes, all Y or N)
			file.Write((header.UnknownFlag1) ? 'Y' : 'N');
			file.Write((header.UnknownFlag2) ? 'Y' : 'N');
			file.Write((header.UnknownFlag3) ? 'Y' : 'N');
			file.Write((header.UnknownFlag4) ? 'Y' : 'N');
			file.Write((header.UnknownFlag5) ? 'Y' : 'N');
			file.Write((header.UnknownFlag6) ? 'Y' : 'N');
			file.Write((header.UnknownFlag7) ? 'Y' : 'N');
			file.Write((header.UnknownFlag8) ? 'Y' : 'N');

			// Write Diagnostics VIN (ASCII char, one byte)
			file.Write(header.DiagnosticsVIN);

			// Write two null bytes
			file.Write((byte)0x00);
			file.Write((byte)0x00);

			// Write the unknown value (ASCII char, one byte)
			file.Write(header.UnknownValue1);

			// Write null byte
			file.Write((byte)0x00);

			// Write data revision number (null-terminated string, takes the form "DREV #")
			string dataRevisionNumber = $"DREV {header.DataRevisionValue}";
			file.WriteNTString(dataRevisionNumber);

			// Write weather airport code (null-terminated string).
			file.WriteNTString(header.WeatherAirportCode);

			// Write weather city display name (null-terminated string).
			file.WriteNTString(header.WeatherCityDisplayName);

			// Write julian date
			file.Write(header.JulianDate);

			// Write number of channels
			file.WriteNTString(header.NumberOfChannels.ToString());

			// Write unknown values
			file.WriteNTString(header.UnknownValue2.ToString());
			file.WriteNTString(header.UnknownValue3.ToString());
		}

		private static void WriteChannel(CurdayChannel channel, BinaryWriter file)
		{
			// Write the channel separator (ASCII '[')
			file.Write('[');

			// Write the channel number (5 ASCII characters)
			for (int i = 0; i < 5; i++)
			{
				file.Write(channel.ChannelNumber[i]);
			}

			// Write six null bytes
			for (int i = 0; i < 6; i++) { file.Write((byte)0x00); }

			// Write the source ID (6 ASCII characters, padded with NULs)
			for (int i = 0; i < 6; i++)
			{
				if (i < channel.SourceID.Length)
				{
					file.Write(channel.SourceID[i]);
				}
				else { file.Write((byte)0x00); }
			}

			// Write a null byte
			file.Write((byte)0x00);

			// Write the call letters (6 ASCII characters, padded with NULs)
			for (int i = 0; i < 6; i++)
			{
				if (i < channel.CallLetters.Length)
				{
					file.Write(channel.CallLetters[i]);
				}
				else { file.Write((byte)0x00); }
			}

			// Write two null bytes
			file.Write((byte)0x00);
			file.Write((byte)0x00);

			// Write flags 1 (1 byte)
			file.Write((byte)channel.Flags1);

			// Write timeslot mask (6 bytes)
			for (int i = 0; i < 6; i++)
			{
				file.Write(channel.TimeslotMask[i]);
			}

			// Write blackout mask (6 bytes)
			for (int i = 0; i < 6; i++)
			{
				file.Write(channel.BlackoutMask[i]);
			}

			// Write flags 2 (1 byte)
			file.Write(channel.Flags2);

			// Write background color (2 bytes)
			// === ENDIAN MIGHT BE WRONG ===
			file.Write(channel.BackgroundColor);

			// Write brush color (2 bytes)
			// === ENDIAN MIGHT BE WRONG ===
			file.Write(channel.BrushID);

			// Write two null bytes
			file.Write((byte)0x00);
			file.Write((byte)0x00);

			// Write flags 3 (1 byte)
			file.Write((byte)channel.Flags3);

			// Write the source ID again
			for (int i = 0; i < 6; i++)
			{
				if (i < channel.SourceID.Length)
				{
					file.Write(channel.SourceID[i]);
				}
				else { file.Write((byte)0x00); }
			}

			// Write programs

		}

		private static void WriteProgram(CurdayProgram program, BinaryWriter file)
		{
			// Write a null byte (I guess)
			file.Write((byte)0x00);

			// Write time slot (null-terminated string)
			file.WriteNTString(program.TimeSlot);

			// Write program flags (if there are any) (null-terminated string)
			if (!string.IsNullOrEmpty(program.ProgramFlags))
			{
				file.WriteNTString(program.ProgramFlags);
			}

			// Write program type (if there is one) (null-terminated string)
			if (!string.IsNullOrEmpty(program.ProgramType))
			{
				file.WriteNTString(program.ProgramType);
			}

			// Write movie category (is there is one) (null-terminated string)
			if (!string.IsNullOrEmpty(program.MovieCategory))
			{
				file.WriteNTString(program.MovieCategory);
			}

			// WYLO: programs don't need all their fields
			// or maybe they do (and they write one-byte NTString for empty fields)
		}
	}
}
