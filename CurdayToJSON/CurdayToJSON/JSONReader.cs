using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CurdayToJSON
{
	internal static class JSONReader
	{
		public static Curday DeserializeFromFile(string filePath)
		{
			string json = File.ReadAllText(filePath);
			return Deserialize(json);
		}

		public static Curday Deserialize(string json)
		{
			JObject obj = JObject.Parse(json);

			Curday result = new Curday();

			var header = obj["header"];
			result.Header = Deserialize(header);

			var channels = (JArray)obj["channels"];
			result.Channels = Deserialize(channels);

			return result;
		}

		private static CurdayHeader Deserialize(JToken token)
		{
			CurdayHeader result = new CurdayHeader();

			string unknownFlags = (string)token["unknownFlags"];

			result.DiagnosticsBCK = (char)token["diagnosticsBCK"];
			result.DiagnosticsFWD = (char)token["diagnosticsFWD"];
			result.ScrollSpeed = (int)token["scrollSpeed"];
			result.NumberOfTextAdsAllowedA = (int)token["numberOfTextAdsAllowedA"];
			result.NumberOfTextAdsAllowedB = (int)token["numberOfTextAdsAllowedB"];
			result.NumberOfLinesInTextAd = (int)token["numberOfLinesInTextAd"];
			result.GrphDST = (bool)token["grphDST"];
			result.Timezone = (int)token["timezone"];
			result.UnknownFlag1 = (unknownFlags[0] == 'Y');
			result.UnknownFlag2 = (unknownFlags[1] == 'Y');
			result.UnknownFlag3 = (unknownFlags[2] == 'Y');
			result.UnknownFlag4 = (unknownFlags[3] == 'Y');
			result.UnknownFlag5 = (unknownFlags[4] == 'Y');
			result.UnknownFlag6 = (unknownFlags[5] == 'Y');
			result.UnknownFlag7 = (unknownFlags[6] == 'Y');
			result.UnknownFlag8 = (unknownFlags[7] == 'Y');
			result.DiagnosticsVIN = (char)token["diagnosticsVIN"];
			result.UnknownValue1 = (char)token["unknownValue1"];
			result.DataRevisionValue = (int)token["dataRevisionNumber"];
			result.WeatherAirportCode = (string)token["weatherAirportCode"];
			result.WeatherCityDisplayName = (string)token["weatherDisplayCityName"];
			result.NumberOfChannels = (int)token["numberOfChannels"];
			result.UnknownValue2 = (int)token["unknownValue2"];
			result.UnknownValue3 = (int)token["unknownValue3"];

			return result;
		}

		private static List<CurdayChannel> Deserialize(JArray array)
		{
			List<CurdayChannel> result = new List<CurdayChannel>();

			foreach (var token in array)
			{
				CurdayChannel channel = new CurdayChannel();

				channel.ChannelNumber = (string)token["number"];
				channel.SourceID = (string)token["sourceID"];
				channel.CallLetters = (string)token["callLetters"];
				channel.Flags1 = (ChannelFlags1)Enum.Parse(typeof(ChannelFlags1), (string)token["flags1"]);
				channel.TimeslotMask = SixByteMask.Parse((string)token["timeslotMask"]);
				channel.BlackoutMask = SixByteMask.Parse((string)token["blackoutMask"]);
				channel.Flags2 = (byte)token["flags2"];
				channel.BackgroundColor = Convert.ToUInt16(((string)token["backgroundColor"]).Substring(2, 4), 16);
				channel.BrushID = Convert.ToUInt16(((string)token["brushID"]).Substring(2, 4), 16);
				channel.Flags3 = (ChannelsFlags3)Enum.Parse(typeof(ChannelsFlags3), (string)token["flags3"]);

				var programs = (JArray)token["programs"];
				channel.Programs = DeserializePrograms(programs);

				result.Add(channel);
			}

			return result;
		}

		private static List<CurdayProgram> DeserializePrograms(JArray array)
		{
			List<CurdayProgram> result = new List<CurdayProgram>();

			foreach (var token in array)
			{
				CurdayProgram program = new CurdayProgram();

				string timeSlot = (string)token["timeSlot"];
				if (timeSlot == "0" || timeSlot == "49") { program.TimeSlot = timeSlot; }
				else { program.TimeSlot = FormatHelpers.TimeToCurdayTimeSlot(DateTime.Parse((string)token["timeSlot"])); }

				program.ProgramFlags = (string)token["flags"];
				program.ProgramType = FormatHelpers.NamedTypeToProgramType((string)token["type"]);
				program.MovieCategory = FormatHelpers.NamedCategoryToMovieCategory((string)token["movieCategory"]);
				program.ProgramName = (string)token["name"];

				result.Add(program);
			}

			return result;
		}
	}
}
