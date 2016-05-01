using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CurdayToJSON
{
	class Program
	{
		static void Main(string[] args)
		{
			//if (args.Length != 4 || args[0] != "-i" || args[0] != "/i" || args[2] != "-o" || args[2] != "/o")
			//{
			//	PrintUsage();
			//	return;
			//}

			//string inputFilePath = args[1];
			//string outputType = args[3].ToLower();

			//if (!File.Exists(inputFilePath))
			//{
			//	Console.WriteLine($"\tError: the file at {inputFilePath} does not exist.");
			//	return;
			//}
			//else if (outputType != "json" || outputType != "binary")
			//{
			//	Console.WriteLine($"\tError: incorrect output type {outputType}. Select either json or binary.");
			//}

			var curday = CurdayReader.Read(args[0]);
		}

		private static void PrintUsage()
		{
			Console.WriteLine("Usage: CurdayToJSON -i \\path\\to\\file -o {json, binary}");
			Console.WriteLine("\t-i: Path to input curday.dat or JSON file");
			Console.WriteLine("\t-o: Output type (json or binary). Select binary to output a curday.dat");
			Console.WriteLine("\tOutput file will have the same name as the input file, with either a DAT or JSON extension");
			Console.WriteLine();
		}
	}
}
