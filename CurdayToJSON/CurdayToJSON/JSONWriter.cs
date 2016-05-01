using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CurdayToJSON
{
	internal static class JSONWriter
	{
		private static object GetSerializableObjects(Curday curday)
		{
			List<object> channelObjects = new List<object>(curday.Channels.Count);
			foreach (var channel in curday.Channels)
			{
				channelObjects.Add(GetSerializableObjects(channel));
			}

			return new
			{
				header = GetSerializableObjects(curday.Header),
				channels = channelObjects
			};
		}

		private static object GetSerializableObjects(CurdayHeader header)
		{
			string unknownFlagsString = $"{FormatHelpers.BooleanToYNFlag(header.UnknownFlag1)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag2)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag3)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag4)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag5)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag6)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag7)}{FormatHelpers.BooleanToYNFlag(header.UnknownFlag8)}";

			return new
			{
				diagnosticsBCK = header.DiagnosticsBCK,
				diagnosticsFWD = header.DiagnosticsFWD,
				scrollSpeed = header.ScrollSpeed,
				numberOfTextAdsAllowedA = header.NumberOfTextAdsAllowedA,
				numberOfTextAdsAllowedB = header.NumberOfTextAdsAllowedB,
				numberOfLinesInTextAd = header.NumberOfLinesInTextAd,
				grphDST = header.GrphDST,
				timezone = header.Timezone,
				unknownFlags = unknownFlagsString,
				diagnosticsVIN = header.DiagnosticsVIN,
				unknownValue1 = header.UnknownValue1,
				dataRevisionNumber = header.DataRevisionValue,
				weatherAirportCode = header.WeatherAirportCode,
				weatherDisplayCityName = header.WeatherCityDisplayName,
				julianDate = header.JulianDate,
				numberOfChannels = header.NumberOfChannels,
				unknownValue2 = header.UnknownValue2,
				unknownValue3 = header.UnknownValue3
			};
		}

		private static object GetSerializableObjects(CurdayChannel channel)
		{
			List<object> programObjects = new List<object>(channel.Programs.Count);
			foreach (var program in channel.Programs)
			{
				programObjects.Add(GetSerializableObjects(program));
			}

			return new
			{
				number = channel.ChannelNumber,
				sourceID = channel.SourceID,
				callLetters = channel.CallLetters,
				flags1 = channel.Flags1.ToString(),
				timeslotMask = channel.TimeslotMask.ToString(),
				blackoutMask = channel.BlackoutMask.ToString(),
				flags2 = channel.Flags2,
				backgroundColor = $"0x{channel.BackgroundColor:X4}",
				brushID = $"0x{channel.BrushID:X4}",
				flags3 = channel.Flags3.ToString(),
				programs = programObjects
			};
		}

		private static object GetSerializableObjects(CurdayProgram program)
		{
			return new
			{
				timeSlot = FormatHelpers.CurdayTimeSlotToTime(program.TimeSlot),
				flags = program.ProgramFlags,
				type = FormatHelpers.ProgramTypeToNamedType(program.ProgramType),
				movieCategory = FormatHelpers.MovieCategoryToNamedCategory(program.MovieCategory),
				name = program.ProgramName
			};
		}

		public static string Serialize(Curday curday)
		{
			return JObject.FromObject(GetSerializableObjects(curday)).ToString();
		}
	}
}
