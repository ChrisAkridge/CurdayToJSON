using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CurdayToJSON.Extensions;

namespace CurdayToJSON
{
	internal static class CurdayReader
	{
		// http://prevueguide.com/wiki/Prevue_Emulation:Curday.dat_and_Nxtday.dat
		
		internal static Curday Read(string inputFilePath)
		{
			BinaryReader reader = new BinaryReader(File.OpenRead(inputFilePath), Encoding.ASCII);

			Curday result = new Curday();
			result.Header = ReadHeader(reader);

			// Read channels
			result.Channels = new List<CurdayChannel>();
			bool lastChannel = false;

			while (!lastChannel)
			{
				result.Channels.Add(ReadChannel(reader, out lastChannel));
			}

			// Set number of channels in header
			result.Header.NumberOfChannels = result.Channels.Count;
			return result;
		}

		private static CurdayHeader ReadHeader(BinaryReader reader)
		{
			CurdayHeader result = new CurdayHeader();
			// Read Diagnostics BCK (ASCII char, one byte)
			result.DiagnosticsBCK = reader.ReadChar();

			// Read Diagnostics FWD (ASCII char, one byte)
			result.DiagnosticsFWD = reader.ReadChar();

			// Read Scroll Speed (ASCII char, one byte, number in range 0..7 inclusive)
			char scrollSpeed = reader.ReadChar();
			if (!(new char[] { '0', '1', '2', '3', '4', '5', '6', '7' }).Contains(scrollSpeed)) { throw new IOException($"Read invalid scroll speed value {scrollSpeed}. Expected number between 0 and 7."); }
			result.ScrollSpeed = int.Parse(scrollSpeed.ToString());

			// Read Number of Text Ads Allowed A (ASCII char, one byte, number in range 0..9 inclusive)
			char numberOfTextAdsAllowedA = reader.ReadChar();
			if (!char.IsNumber(numberOfTextAdsAllowedA)) { throw new IOException($"Read invalid number of text ads allowed A value {numberOfTextAdsAllowedA}. Expected number between 0 and 9."); }
			result.NumberOfTextAdsAllowedA = int.Parse(numberOfTextAdsAllowedA.ToString());

			// Read Number of Text Ads Allowed B (ASCII char, one byte, number in range 0..9 inclusive)
			char numberOfTextAdsAllowedB = reader.ReadChar();
			if (!char.IsNumber(numberOfTextAdsAllowedB)) { throw new IOException($"Read invalid number of text ads allowed B value {numberOfTextAdsAllowedB}. Expected number between 0 and 9."); }
			result.NumberOfTextAdsAllowedB = int.Parse(numberOfTextAdsAllowedB.ToString());

			// Read Number of Lines in Text Ad (ASCII char, one byte, number in range 0..9 inclusive)
			char numberOfLinesInTextAd = reader.ReadChar();
			if (!char.IsNumber(numberOfLinesInTextAd)) { throw new IOException($"Read invalid number of lines in text ad {numberOfLinesInTextAd}. Expected number between 0 and 9."); }
			result.NumberOfLinesInTextAd = int.Parse(numberOfLinesInTextAd.ToString());

			// Read GRPH DST (ASCII char, one byte, either Y or N)
			char grphDST = reader.ReadChar();
			result.GrphDST = ReadYNFlag(grphDST, "GRPH DST");

			// Skip ETX SOH (two bytes).
			reader.BaseStream.Position += 2;

			// Read timezone (ASCII char, one byte)
			result.Timezone = int.Parse(reader.ReadChar().ToString());

			// Read flags (8 ASCII chars, eight bytes, all Y or N)
			result.UnknownFlag1 = ReadYNFlag(reader.ReadChar(), "unknown flag 1");
			result.UnknownFlag2 = ReadYNFlag(reader.ReadChar(), "unknown flag 2");
			result.UnknownFlag3 = ReadYNFlag(reader.ReadChar(), "unknown flag 3");
			result.UnknownFlag4 = ReadYNFlag(reader.ReadChar(), "unknown flag 4");
			result.UnknownFlag5 = ReadYNFlag(reader.ReadChar(), "unknown flag 5");
			result.UnknownFlag6 = ReadYNFlag(reader.ReadChar(), "unknown flag 6");
			result.UnknownFlag7 = ReadYNFlag(reader.ReadChar(), "unknown flag 7");
			result.UnknownFlag8 = ReadYNFlag(reader.ReadChar(), "unknown flag 8");

			// Read Diagnostics VIN (ASCII char, one byte)
			result.DiagnosticsVIN = reader.ReadChar();

			// Skip two null bytes
			reader.BaseStream.Position += 2;

			// Read the unknown value (ASCII char, one bytes)
			result.UnknownValue1 = reader.ReadChar();

			// Skip null byte
			reader.BaseStream.Position += 1;

			// Read data revision number (null-terminated string, takes the form "DREV #").
			string dataRevisionText = reader.ReadNTString();
			string dataRevisionNumberString = dataRevisionText.Split(' ')[1];
			int dataRevisionNumber;
			if (!int.TryParse(dataRevisionNumberString, out dataRevisionNumber)) { throw new IOException($"Invalid data revision number value {dataRevisionNumberString}. Expected number."); }
			result.DataRevisionValue = dataRevisionNumber;

			// Read weather airport code (null-terminated string).
			result.WeatherAirportCode = reader.ReadNTString();

			// Read weather city display name (null-terminated string).
			result.WeatherCityDisplayName = reader.ReadNTString();

			// Read julian date
			string julianDateString = reader.ReadNTString();
			byte julianDate;
			if (!byte.TryParse(julianDateString, out julianDate)) { throw new IOException($"Invalid Julian date value {julianDateString}. Expected number between 0 and 255."); }
			result.JulianDate = julianDate;

			// Read number of channels (null-terminated string).
			result.NumberOfChannels = int.Parse(reader.ReadNTString());

			// Read two unknown values
			result.UnknownValue2 = int.Parse(reader.ReadNTString());
			result.UnknownValue3 = int.Parse(reader.ReadNTString());

			return result;
		}

		private static CurdayChannel ReadChannel(BinaryReader reader, out bool lastChannel)
		{
			CurdayChannel result = new CurdayChannel();

			// Skip the channel separator (ASCII '[')
			reader.BaseStream.Position += 1;

			// Read the channel number (5 ASCII characters)
			result.ChannelNumber = new string(reader.ReadChars(5));

			// Skip six null bytes
			reader.BaseStream.Position += 6;

			// Read the source ID (6 ASCII characters, padded with NULs)
			result.SourceID = new string(reader.ReadChars(6)).Replace("\0", "");

			// Skip the null byte
			reader.BaseStream.Position += 1;

			// Read the call letters (6 ASCII characters, padded with NULs)
			result.CallLetters = new string(reader.ReadChars(6)).Replace("\0", "");

			// Skip two null bytes
			reader.BaseStream.Position += 2;

			// Read flags 1 (1 byte)
			result.Flags1 = (ChannelFlags1)reader.ReadByte();

			// Read timeslot mask (6 bytes)
			result.TimeslotMask = new SixByteMask(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

			// Read blackout mask (6 bytes)
			result.BlackoutMask = new SixByteMask(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

			// Read flags 2 (1 byte)
			result.Flags2 = reader.ReadByte();

			// Read background color (2 bytes)
			result.BackgroundColor = reader.ReadUInt16();

			// Read brush ID (2 bytes)
			result.BrushID = reader.ReadUInt16();

			// Skip two null bytes
			reader.BaseStream.Position += 2;

			// Read flags 3 (1 byte)
			result.Flags3 = (ChannelsFlags3)reader.ReadByte();

			// Skip the duplicate source ID.
			string duplicateSourceID = new string(reader.ReadChars(6)).Replace("\0", "");

			// Read programs
			result.Programs = new List<CurdayProgram>();
			bool lastProgram = false;

			while (!lastProgram)
			{
				result.Programs.Add(ReadProgram(reader, out lastProgram));
			}

			lastChannel = (reader.PeekChar() == -1);

			return result;
		}

		private static CurdayProgram ReadProgram(BinaryReader reader, out bool lastProgram)
		{
			CurdayProgram result = new CurdayProgram();

			// Skip next byte if null
			if (reader.PeekChar() == '\0') { reader.BaseStream.Position += 1; }

			// Read time slot (null-terminated string)
			result.TimeSlot = reader.ReadNTString();
			if (reader.PeekChar() == '[' || reader.PeekChar() == -1) { goto end; }

			// Read program flags (null-terminated string)
			result.ProgramFlags = reader.ReadNTString();
			if (reader.PeekChar() == '[' || reader.PeekChar() == -1) { goto end; }

			// Read program type (null-terminated string)
			result.ProgramType = reader.ReadNTString();
			if (reader.PeekChar() == '[' || reader.PeekChar() == -1) { goto end; }

			// Read movie category (null-terminated string)
			result.MovieCategory = reader.ReadNTString();
			if (reader.PeekChar() == '[' || reader.PeekChar() == -1) { goto end; }

			// Skip unknown values
			reader.BaseStream.Position += 2;
			if (reader.PeekChar() == '[' || reader.PeekChar() == -1) { goto end; }

			// Read program name (null-terminated string)
			result.ProgramName = reader.ReadNTString();

		end:
			lastProgram = (reader.PeekChar() == '[' || reader.PeekChar() == -1);
			return result;
		}

		private static bool ReadYNFlag(char flag, string valueName)
		{
			if (flag != 'Y' && flag != 'N') { throw new IOException($"Read invalid {valueName} value {flag}. Expected Y or N."); }
			return flag == 'Y';
		}
	}
}
