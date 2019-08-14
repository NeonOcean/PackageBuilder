using System;
using System.Diagnostics;
using System.IO;
using s4pi.Interfaces;
using s4pi.Package;

namespace PackageBuilder {
	public static class Main {
		public static bool Run () {
			if(Entry.PrintHelp) {
				PrintHelp();
			}

			IPackage package = null;

			if(Entry.BasePackageFilePath == null || !File.Exists(Entry.BasePackageFilePath)) {
				package = Package.NewPackage(0);
			} else {
				MemoryStream packageStream = new MemoryStream();
				BinaryWriter packageStreamWriter = new BinaryWriter(packageStream);

				using(BinaryReader packageFileReader = new BinaryReader(new FileStream(Entry.BasePackageFilePath, FileMode.Open, FileAccess.Read))) {
					packageStreamWriter.Write(packageFileReader.ReadBytes((int)packageFileReader.BaseStream.Length));
					packageStream.Flush();
				}

				package = Packages.OpenPackageStream(0, packageStream);
			}

			Packages.RemoveFiles(package, Entry.RemoveFileNames);
			Packages.AddFiles(package, Entry.SourceFilePaths);

			for(int targetPackageFilePathIndex = 0; targetPackageFilePathIndex < Entry.TargetPackageFilePaths.Count; targetPackageFilePathIndex++) {
				string targetDirectory = Path.GetDirectoryName(Entry.TargetPackageFilePaths[targetPackageFilePathIndex]);

				if(!Directory.Exists(targetDirectory)) {
					Directory.CreateDirectory(targetDirectory);
				}

				package.SaveAs(Entry.TargetPackageFilePaths[targetPackageFilePathIndex]);
			}

			Entry.Completed = true;
			return true;
		}

		public static void PrintHelp () {
			Console.WriteLine(
				"Builds a Sims 4 package file from source files. \n" +
				"\n" +
				Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) + " [-h] [-b[filepath]] [-t[filepath]] [-s[filepath;...] \n" +
				"-r [filename;...] \n" +
				"\n" +
				" -h\t\t\tPrints this help message. \n" +
				"\n" +
				" -b [filepath]\t\tDesignates a base package file, source files will be \n" +
				"\t\t\tadded to it and will replacing any duplicates. \n" +
				"\t\t\tThe path can be relative to the working directory. \n" +
				"\n" +
				" -t [filepath]\t\tDesignates the file path the package will be saved to. \n" +
				"\t\t\tOverriding the base file is allowed. The path can be \n" +
				"\t\t\trelative to the working directory. \n" +
				"\n" +
				" -s [filepath;...]\tDesignates the source file paths. Add multiple source \n" +
				"\t\t\tfiles by separating the paths with a semicolon. The \n" +
				"\t\t\tpaths can be relative to the working directory. \n" +
				"\n" +
				" -r [filename;...]\tDesignates files to remove from the base package. \n" +
				"\t\t\tAdd multiple files by separating the names with a \n" +
				"\t\t\tsemicolon. \n");
		}
	}
}
