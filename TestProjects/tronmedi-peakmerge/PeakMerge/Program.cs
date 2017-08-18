using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeakMerge
{
	class Program
	{
		static void Main(string[] args)
		{
			var options = new Options();
			if (CommandLine.Parser.Default.ParseArguments(args, options))
			{
				new PeakMerger(options.InputFiles, options.OutputFile).Process();
			}
			else
			{
				// Display the default usage information
				Console.WriteLine(options.GetUsage());
			}
		}
	}
}
