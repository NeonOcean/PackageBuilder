using System;
using System.Collections.Generic;
using System.IO;

namespace PackageBuilder {
	static class Entry {
		public static bool Completed {
			get; set;
		}

		public static bool PrintHelp {
			get; set;
		}

		public static string BasePackageFilePath {
			get; set;
		}

		public static List<string> TargetPackageFilePaths {
			get; set;
		} = new List<string>();

		public static List<string> SourceFilePaths {
			get; set;
		} = new List<string>();

		public static List<string> RemoveFileNames {
			get; set;
		} = new List<string>();

		static void Main () {
			AppDomain.CurrentDomain.ProcessExit += OnExit;

			try {
				if(!ReadArguments()) {
					return;
				}

				PackageBuilder.Main.Run();
			} catch(Exception e) {
				Console.Error.WriteLine(e.ToString());
			}
		}

		private static void OnExit (object sender, EventArgs e) {
			if(!Completed) {
				if(Environment.ExitCode == 0) {
					Environment.ExitCode = 1;
				}
			} else {
				Environment.ExitCode = 0;
			}
		}

		private static bool ReadArguments () {
			string[] arguments = Environment.GetCommandLineArgs();

			if(arguments.Length <= 1) {
				PrintHelp = true;
				return true;
			}

			for(int argumentIndex = 1; argumentIndex < arguments.Length; argumentIndex++) {
				if(arguments[argumentIndex].Equals("-?", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/?", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("-h", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/h", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("-help", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/help", StringComparison.OrdinalIgnoreCase)) {
					PrintHelp = true;
					continue;
				}

				if(arguments[argumentIndex].Equals("-b", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/b", StringComparison.OrdinalIgnoreCase)) {
					if(argumentIndex != arguments.Length - 1) {
						BasePackageFilePath = arguments[argumentIndex + 1];
						argumentIndex++;
						continue;
					}
				}

				if(arguments[argumentIndex].Equals("-t", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/t", StringComparison.OrdinalIgnoreCase)) {
					if(argumentIndex != arguments.Length - 1) {
						TargetPackageFilePaths.Add(arguments[argumentIndex + 1]);
						argumentIndex++;
						continue;
					}
				}

				if(arguments[argumentIndex].Equals("-s", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/s", StringComparison.OrdinalIgnoreCase)) {
					if(argumentIndex != arguments.Length - 1) {
						SourceFilePaths.AddRange(arguments[argumentIndex + 1].Split(';'));
						argumentIndex++;
						continue;
					}
				}

				if(arguments[argumentIndex].Equals("-r", StringComparison.OrdinalIgnoreCase) ||
					arguments[argumentIndex].Equals("/r", StringComparison.OrdinalIgnoreCase)) {
					if(argumentIndex != arguments.Length - 1) {
						RemoveFileNames.AddRange(arguments[argumentIndex + 1].Split(';'));
						argumentIndex++;
						continue;
					}
				}
			}

			try {
				if(BasePackageFilePath != null) {
					BasePackageFilePath = Path.GetFullPath(BasePackageFilePath);
				}
			} catch(Exception e) {
				throw new Exception("Failed to parse argument '-b'", e);
			}

			try {
				for(int targetPackageFilePathIndex = 0; targetPackageFilePathIndex < TargetPackageFilePaths.Count; targetPackageFilePathIndex++) {
					TargetPackageFilePaths[targetPackageFilePathIndex] = Path.GetFullPath(TargetPackageFilePaths[targetPackageFilePathIndex]);
					new FileInfo(TargetPackageFilePaths[targetPackageFilePathIndex]);
				}
			} catch(Exception e) {
				throw new Exception("Failed to parse argument '-t'", e);
			}

			try {
				for(int sourceFilePathIndex = 0; sourceFilePathIndex < SourceFilePaths.Count; sourceFilePathIndex++) {
					SourceFilePaths[sourceFilePathIndex] = Path.GetFullPath(SourceFilePaths[sourceFilePathIndex]);
					FileInfo sourceFileInfo = new FileInfo(SourceFilePaths[sourceFilePathIndex]);

					if(!sourceFileInfo.Exists) {
						Console.Error.WriteLine("Cannot find source file '" + sourceFileInfo.FullName + "'.");
						return false;
					}
				}
			} catch(Exception e) {
				throw new Exception("Failed to parse argument '-s'", e);
			}

			return true;
		}
	}
}
