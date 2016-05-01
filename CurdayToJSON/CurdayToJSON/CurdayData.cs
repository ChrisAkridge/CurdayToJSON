using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CurdayToJSON.Extensions;

namespace CurdayToJSON
{
	internal sealed class Curday
	{
		public CurdayHeader Header { get; set; }
		public List<CurdayChannel> Channels { get; set; }
	}

	internal sealed class CurdayHeader
	{
		public char DiagnosticsBCK { get; set; }
		public char DiagnosticsFWD { get; set; }
		public int ScrollSpeed { get; set; }
		public int NumberOfTextAdsAllowedA { get; set; }
		public int NumberOfTextAdsAllowedB { get; set; }
		public int NumberOfLinesInTextAd { get; set; }
		public bool GrphDST { get; set; }
		public int Timezone { get; set; }
		public bool UnknownFlag1 { get; set; }
		public bool UnknownFlag2 { get; set; }
		public bool UnknownFlag3 { get; set; }
		public bool UnknownFlag4 { get; set; }
		public bool UnknownFlag5 { get; set; }
		public bool UnknownFlag6 { get; set; }
		public bool UnknownFlag7 { get; set; }
		public bool UnknownFlag8 { get; set; }
		public char DiagnosticsVIN { get; set; }
		public char UnknownValue1 { get; set; }
		public int DataRevisionValue { get; set; }
		public string WeatherAirportCode { get; set; }
		public string WeatherCityDisplayName { get; set; }
		public byte JulianDate { get; set; }
		public int NumberOfChannels { get; set; }
		public int UnknownValue2 { get; set; }
		public int UnknownValue3 { get; set; }
	}

	[DebuggerDisplay("{ChannelNumber} {CallLetters} ({Programs.Count} programs)")]
	internal sealed class CurdayChannel
	{
		public const char ChannelSeparator = '[';
		public string ChannelNumber { get; set; }
		public string SourceID { get; set; }
		public string CallLetters { get; set; }
		public ChannelFlags1 Flags1 { get; set; }
		public SixByteMask TimeslotMask { get; set; }
		public SixByteMask BlackoutMask { get; set; }
		public byte Flags2 { get; set; }
		public ushort BackgroundColor { get; set; }
		public ushort BrushID { get; set; }
		public ChannelsFlags3 Flags3 { get; set; }

		public List<CurdayProgram> Programs { get; set; }
	}

	[DebuggerDisplay("{ProgramName}")]
	internal sealed class CurdayProgram
	{
		public const byte ProgramSeparator = 0x00;
		public string TimeSlot { get; set; }
		public string ProgramFlags { get; set; }
		public string ProgramType { get; set; }
		public string MovieCategory { get; set; }
		public const char UnknownValue = '0';
		public string ProgramName { get; set; }
	}

	internal sealed class SixByteMask
	{
		private byte[] bytes;
		public byte this[int index]
		{
			get
			{
				return bytes[index];
			}
			set
			{
				bytes[index] = value;
			}
		}

		public SixByteMask(byte a, byte b, byte c, byte d, byte e, byte f)
		{
			bytes = new byte[6];
			bytes[0] = a;
			bytes[1] = b;
			bytes[2] = c;
			bytes[3] = d;
			bytes[4] = e;
			bytes[5] = f;	
		}

		public override string ToString()
		{
			StringBuilder resultBuilder = new StringBuilder();

			foreach (byte b in bytes)
			{
				resultBuilder.Append(b.ToString("X2"));
			}

			return resultBuilder.ToString();
		}
	}

	[Flags]
	internal enum ChannelFlags1
	{
		Invalid = 0x00,
		None = 0x01,

		/// <summary>
		/// Red highlight gradient.
		/// </summary>
		HILITESRC = 0x02,

		/// <summary>
		/// Marks channel as summary source; summary of channel's listings appears at bottom of listings.
		/// </summary>
		SUMBYSRC = 0x04,

		/// <summary>
		/// Removes channel from listings.
		/// </summary>
		VideoTagDisable = 0x08,

		/// <summary>
		/// Marks channel as PPV source.
		/// </summary>
		CAF_PPVSRC = 0x10,

		/// <summary>
		/// Indicates listings may not change.
		/// </summary>
		DITTO = 0x20,

		/// <summary>
		/// Blue gradient highlight.
		/// </summary>
		ALTHILITESRC = 0x40,
		
		/// <summary>
		/// Marks channel as stereo.
		/// </summary>
		STEREO = 0x80
	}

	[Flags]
	internal enum ChannelsFlags3
	{
		Invalid = 0x00,
		Grid = 0x01,
		MR = 0x02,
		DNICHE = 0x04,
		DMPLEX = 0x08,
		CF2_PPV = 0x10,
		Reserved1 = 0x20,
		Reserved2 = 0x40,
		Reserved4 = 0x80
	}
}
