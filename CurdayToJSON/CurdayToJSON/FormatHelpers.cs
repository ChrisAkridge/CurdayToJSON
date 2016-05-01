using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurdayToJSON
{
	internal static class FormatHelpers
	{
		public static string CurdayTimeSlotToTime(string timeSlot)
		{
			// Time slot format: starts at value 1 (3:00am), goes to 48 (2:30am)
			// 0 and 49 are valid values but I don't know why they're there
			// just pass them through

			if (timeSlot == "0" || timeSlot == "49" || string.IsNullOrEmpty(timeSlot)) { return timeSlot; }

			DateTime baseTimeSlot = new DateTime(2016, 5, 1, 3, 0, 0);
			int timeSlotAsNumber = int.Parse(timeSlot);

			return baseTimeSlot.AddHours(timeSlotAsNumber / 2d).ToString("h:mm tt");
		}

		public static string TimeToCurdayTimeSlot(DateTime dateTime)
		{
			DateTime baseTimeSlot = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 3, 0, 0);
			if (dateTime.Hour < 3) { baseTimeSlot = baseTimeSlot.Subtract(TimeSpan.FromDays(1d)); }

			TimeSpan difference = dateTime - baseTimeSlot;
			return ((int)Math.Floor((difference.TotalHours * 2d) + 1d)).ToString();
		}

		public static string ProgramTypeToNamedType(string programType)
		{
			switch (programType)
			{
				case "1": return "Movie";
				case "5": return "Sports";
				case "19": return "Children";
				case "22": return "News";
				case "34": return "General";
				default: return programType;
			}
		}

		public static string NamedTypeToProgramType(string namedType)
		{
			if (string.IsNullOrEmpty(namedType)) { return namedType; }
			switch (namedType.Trim().ToLowerInvariant())
			{
				case "movie": return "1";
				case "sports": return "5";
				case "children": return "19";
				case "news": return "22";
				case "general": return "34";
				default: return namedType;
			}
		}

		public static string MovieCategoryToNamedCategory(string movieCategory)
		{
			switch (movieCategory)
			{
				case "1": return "Adult";
				case "4": return "Comedy";
				case "5":
				case "7":
				case "10": return "Musical";
				case "18": return "SciFi";
				default: return movieCategory;
			}
		}

		public static string NamedCategoryToMovieCategory(string namedCategory)
		{
			if (string.IsNullOrEmpty(namedCategory)) { return namedCategory; }
			switch (namedCategory.Trim().ToLowerInvariant())
			{
				case "adult": return "1";
				case "comedy": return "4";
				case "musical": return "5";
				case "scifi": return "18";
				default: return namedCategory;
			}
		}

		public static string BooleanToYNFlag(bool value)
		{
			return (value) ? "Y" : "N";
		}

		public static int DateToJulianDate(DateTime dateTime)
		{
			DateTime startOfYear = new DateTime(dateTime.Year, 1, 1, 0, 0, 0);
			TimeSpan difference = dateTime - startOfYear;
			return (int)(difference.TotalDays + 1d) % 256;
		}
	}
}
