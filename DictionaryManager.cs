﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.Experimental.IO;

namespace MailClient.Dictionaries
{
	public static class DictionaryManager
	{
		private static string builtinDictFolder;
		private static string userDictFolder;
		private static List<DictionaryFilePair> files;
		private static List<DictionaryFileSetting> fileSettings;

		public static string UserDictionaryDir
		{
			get { return userDictFolder; }
		}

		static DictionaryManager()
		{
			builtinDictFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Dictionaries");
			userDictFolder = Path.Combine(Program.DataStore.Location, "Dictionaries");
		}


		public static IEnumerable<DictionaryFileSetting> FileSettings
		{
			get
			{
				if (fileSettings == null)
					ReloadFiles();

				foreach (DictionaryFileSetting fileSetting in fileSettings)
				{
					yield return fileSetting;
				}
			}
		}

		public static IEnumerable<DictionaryFilePair> DictFiles
		{
			get
			{
				if (files == null)
					ReloadFiles();

				return files;
			}
		}


		/// <summary>
		/// Tries to locate dictionary file best matching the culture.
		/// Otherwise the default dictionary is returned.
		/// </summary>
		/// <param name="culture">example (en-US)</param>
		/// <returns></returns>
		public static DictionaryFilePair FindBestDictionary(CultureInfo culture)
		{
			CultureInfo ancestorCulture;
			do
			{
				DictionaryFilePair pair;
				if (TryCreateDictionaryPair(culture.Name, out pair))
					return pair;
				ancestorCulture = culture;
			} while ((culture = culture.Parent) != CultureInfo.InvariantCulture);

			// try to find default culture
			try
			{
				CultureInfo specific = CultureInfo.CreateSpecificCulture(ancestorCulture.Name);

				DictionaryFilePair pair;
				if (TryCreateDictionaryPair(specific.ToString(), out pair))
					return pair;
			}
			catch (ArgumentException)
			{
				// Because zh-CHT has ancestor zh-Hant, which does not have a specific culture associated with it. 
				System.Diagnostics.Debug.Assert(false);
			}
			// any dictionary with the same language
			foreach (DictionaryFilePair filePair in DictFiles)
			{
				try
				{
					if (filePair.CultureString.StartsWith(ancestorCulture.Name, StringComparison.InvariantCultureIgnoreCase))
						return filePair;
				}
				catch (ArgumentException)
				{ }
			}

			// return default
			return DefaultDictionary;
		}

		public static DictionaryFilePair DefaultDictionary
		{
			get
			{
				DictionaryFilePair pair;
				TryCreateDictionaryPair("en-US", out pair);
				return pair;
			}
		}

		public static bool TryCreateDictionaryPair(string culture, out DictionaryFilePair pair)
		{
			string fileName = String.Format("{0}.dic", culture);

			// try to find it firstly in the built-in dictionary folder, secondly in the user folder
			string path = Path.Combine(builtinDictFolder, fileName);

			if (LongPathFile.Exists(path))
			{
				fileName = path;
				pair = DictionaryFilePair.FromFileName(fileName);
				return true;
			}
			else
			{
				path = Path.Combine(userDictFolder, fileName);

				if (LongPathFile.Exists(path))
				{
					fileName = path;
					pair = DictionaryFilePair.FromFileName(fileName);
					return true;
				}
				else
				{
					pair = new DictionaryFilePair("", "");
					return false;
				}
			}
		}


		public static void ReloadFiles()
		{
			files = new List<DictionaryFilePair>();
			fileSettings = new List<DictionaryFileSetting>();

			if (LongPathDirectory.Exists(builtinDictFolder))
				loadDictionariesFromFolder(builtinDictFolder);

			if (LongPathDirectory.Exists(userDictFolder))
				loadDictionariesFromFolder(userDictFolder);
		}

		private static void loadDictionariesFromFolder(string folder)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(folder);
			List<FileInfo> dicFiles = new List<FileInfo>(dirInfo.GetFiles("*.dic"));
			foreach (FileInfo fileInfo in dirInfo.GetFiles("*.aff"))
			{
				foreach (FileInfo fi in dicFiles)
				{
					if (Path.GetFileNameWithoutExtension(fileInfo.Name).Equals(Path.GetFileNameWithoutExtension(fi.Name)))
					{
						DictionaryFilePair dfp = new DictionaryFilePair(fileInfo.FullName, fi.FullName);
						files.Add(dfp);
						try
						{
							DictionaryFileSetting fileSetting = new DictionaryFileSetting(dfp);
							if (!fileSettings.Contains(fileSetting))
								fileSettings.Add(fileSetting);
						}
						catch (Exception)
						{ }

					}
				}
			}
		}

	}
}
