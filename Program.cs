using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace FileExtractor
{
	class Program
	{
		class Options
		{
			[Value(0, Required = false)]
			[Option('f', "File Pattern", Required = false, HelpText = "The file pattern used for the search")]
			public string SearchPattern { get; set; }

			[Value(1, Required = false)]
			[Option('t', "Target Path", Required = false, HelpText = "The target folder where the extracted files will end up")]
			public string TargetPath { get; set; }

			[Value(2, Required = false)]
			[Option('p', "path", Required = false, HelpText = "Path to search, will search working directory by default")]
			public string Path { get; set; }

			[Option('i', "ignore target", Required = false, HelpText = "If true it will not copy files that are already in target", Default = true)]
			public bool IgnoreInTarget { get; set; }
		}


		static void Main(string[] args)
		{
			string searchPattern = null, targetPath = null, sourcePath = null;
			bool ignoreInTarget = true;

			ParserResult<Options> parserResult = Parser.Default.ParseArguments<Options>(args);
			parserResult.WithParsed(o =>
			{
				if (o.TargetPath == null)
				{
					Console.WriteLine("Give a path to where the files should end up");
					targetPath = Console.ReadLine();
				}
				else
				{
					targetPath = o.TargetPath;
				}

				if (o.Path == null)
				{
					sourcePath = Directory.GetCurrentDirectory();
				}
				else
				{
					sourcePath = o.Path;
				}

				if (o.SearchPattern == null)
				{
					Console.WriteLine("Please provide a search pattern");
					searchPattern = Console.ReadLine();
				}
				else
				{
					searchPattern = o.SearchPattern;
				}

				ignoreInTarget = o.IgnoreInTarget;
			});

			parserResult.WithNotParsed(o =>
			{
				Environment.Exit(0);
			});

			Console.WriteLine("Will export all files from \"" + sourcePath + "\" that matches \"" + searchPattern + "\" to \"" + targetPath + "\"");
			Directory.CreateDirectory(targetPath);

			IEnumerable<string> filesPaths = Directory.EnumerateFiles(sourcePath, searchPattern, SearchOption.AllDirectories);
			IEnumerable<string> targetPathfiles = Directory.EnumerateFiles(targetPath, searchPattern, SearchOption.AllDirectories).Select(p => Path.GetFileName(p));

			filesPaths = filesPaths.Where(p => !targetPathfiles.Contains(Path.GetFileName(p)));

			int count = filesPaths.Count();
			int i = 0;

			Console.WriteLine("Copy start");
			foreach (string file in filesPaths)
			{
				i++;
				string targetFile = Path.Combine(targetPath, Path.GetFileName(file));
				File.Copy(file, targetFile, true);
				Console.WriteLine("[" + i + "/" + count + "] Coppied: " + file);
			}
		}
	}
}
