using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace PeakMerge
{
	class Options
	{
		[Option('o', "output", Required =true)]
		public string OutputFile { get; set; }

		[ValueList(typeof(List<string>))]
		public List<string> InputFiles { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			var usage = new StringBuilder();
			usage.AppendLine("Peak Merge Application");
			usage.AppendLine("Syntax: peakmerge -o <output-file> <input-file-list>");
			return usage.ToString();
		}


	}
}
