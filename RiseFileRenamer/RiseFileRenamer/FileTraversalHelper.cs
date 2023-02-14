using System;
using System.IO;

namespace RiseFileRenamer;

/// <summary>
/// Summary description for Class1
/// </summary>
public static class FileTraversalHelper
{
	public static void FileTraversal (string baseLocation, List<string> validData, string origin)
	{
		var files = Directory.GetFiles(baseLocation);
		var directories = Directory.GetDirectories(baseLocation);

		foreach ( var file in files )
		{
			CheckFileAndRename(file, validData, origin);
		}

		// First logic rename all stm to msg
		if (directories.Any(x => x.Equals($"{baseLocation}\\stm", StringComparison.OrdinalIgnoreCase)))
		{
			var stmLocaiton = directories.First(x => x.Equals($"{baseLocation}\\stm", StringComparison.OrdinalIgnoreCase));

			Directory.Move(stmLocaiton, $"{baseLocation}\\MSG");

			directories = Directory.GetDirectories(baseLocation);
        }

		// recurse dang it
		foreach ( var directory in directories )
		{
			FileTraversal(directory, validData, origin);
		}

        files = Directory.GetFiles(baseLocation);
        directories = Directory.GetDirectories(baseLocation);

		if (directories.Count() == 0 && files.Count() == 0) 
		{
			Directory.Delete(baseLocation);
		}
    }

	public static void CheckFileAndRename(string fileName, List<string> validData, string origin)
    {
		if (!validData.Any(x => x.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
		{
			var validFiles = new List<string>();
			var renameFile = string.Empty;
			var drillDownName = fileName.Replace(origin, string.Empty);
            while (!validFiles.Any())
            {
                var newValue = "\\" + drillDownName.Split('\\').Last();

                drillDownName = drillDownName.Replace(newValue, string.Empty);

				renameFile = String.IsNullOrEmpty(renameFile) ? newValue : newValue + renameFile;
                validFiles = validData.Where(x => x.Contains(drillDownName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

			bool matched = false;
			foreach ( var file in validFiles)
			{
				
				var validName = file.Remove(0, drillDownName.Length);
				var validComponents = validName.Split('.').ToList();
				var modComponents = renameFile.Split('.').ToList();

                var correctName = MatchAndReplaceComponents(validComponents, modComponents);

				if (!String.IsNullOrEmpty(correctName))
				{
					renameFile = correctName;
					matched = true;
					break;
                }
			}

			if (matched)
			{
				File.Move(fileName, origin + drillDownName  + renameFile);
			}
			else
			{
				File.Delete(fileName);
			}

		}
	}

	public static string MatchAndReplaceComponents(List<string> validComponents, List<string> modComponents)
	{
		bool different = false;
		int location = 0;

		string match = string.Empty;
		do
		{
			if (validComponents[location].Equals(modComponents[location], StringComparison.OrdinalIgnoreCase))
			{
				match = match + validComponents[location] + '.';
				location++;
			}
			else
			{
				different = true;
			}
		} while (!different && validComponents.Count > location);

		if (location < validComponents.Count - 1)
		{
			return string.Empty;
		}

		if (location == validComponents.Count)
		{
			return match.Remove(match.Length - 1, 1);
        }


		return match + validComponents[location];
	}
}
